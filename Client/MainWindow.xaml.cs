using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Web.Script.Serialization;
using HelpLib.Config;
using HelpLib.Wrapper;
using System.Net.Sockets;
using Client.Extensions;
using Client.Manager;
using System.Net;

namespace Client
{
    public partial class MainWindow : Window, IDisposable
    {
        private OloService service;
        private ConfigFile config;
        private UDP udp;
        private RoomManager manager;

        public MainWindow()
        {
            InitializeComponent();
            Closed += (s, e) =>
            {
                Dispose();
                Close();
            };

            try
            {
                Config.Load("settings.json", out config);
            }
            catch (System.IO.FileNotFoundException)
            {
                config = new ConfigFile("0.0.0.0:14800", "127.0.0.1:14801", "defaultUser");
                Config.Save("settings.json", config);
            }
            finally
            {
                Config.GlobalConfig = config;
                service = new OloService(this);
                manager = new RoomManager();
                udp = UDP.GetInstance(config.LocalHost.Port, "0042");
                UDP.OnReceive += Receive;
                udp.Run();
            }
        }

        private void Receive(object sender, UdpReceiveResult res)
        {
            var data = OloProtocol.Encode(res.Buffer.ToStr());
            service.Parse(data);
        }

        [OloField(Name = "con_to")]
        private void ConTo(params object[] args)
        {
            var roomName = args[0].ToString();
            if (!manager.IsExists(roomName))
            {
                var userList = new Users();
                userList.Add(args[1].ToString().ToIP());
                manager.Add(roomName, userList);
                tabControl.AddTab(roomName, true);
            }
            else
                manager[roomName].Add(args[1].ToString().ToIP());
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            var text = inputTextBox.Text;
            inputTextBox.Clear();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var tab = tabControl.GetSelectTab();
                var tabName = tab.Name;
                manager.BroadcastAllUser(tabName, tabName, Config.GlobalConfig.UserName, text);
                tabControl.PushMessage(tabName, text);
            }
        }

        private void roomsButton_Click(object sender, RoutedEventArgs e)
        {
            var rooms = new Rooms();
            rooms.Owner = this;
            rooms.ShowDialog();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Dispose()
        {
            Config.Save("settings.json", Config.GlobalConfig);
            udp.Dispose();
        }
    }
}
