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
            service.Parse(data, res.RemoteEndPoint);
        }

        [OloField(Name = "con_to")]
        private void ConTo(params object[] args)
        {
            var roomName = args[0].ToString();
            var userIP = args[1].ToString().ToIP();
            if (!manager.IsExists(roomName) && tabControl.Exists(roomName) == null)
            {
                var userList = new Users();
                userList.Add(userIP);
                manager.Add(roomName, userList);
                tabControl.AddTab(roomName, true);
            }
            else
                manager[roomName].Add(userIP);
        }

        [OloField(Name = "push_message")]
        private void PushMessage(params object[] args)
        {
            var roomName = args[0].ToString();
            var userName = args[1].ToString();
            var message = args[2].ToString();
            var inputIP = args[3].ToString().ToIP();
            if (manager.IsExists(roomName) && tabControl.Exists(roomName) != null)
            {
                tabControl.PushMessage(roomName, $"{userName}: {message}");
                manager[roomName].BraodcastMessage(inputIP, roomName, userName, message);
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            var text = inputTextBox.Text;
            inputTextBox.Clear();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var tab = tabControl.GetSelectTab();
                var roomName = tab.Header.Text;
                manager[roomName].BraodcastMessage(roomName, Config.GlobalConfig.UserName, text);
                tabControl.PushMessage(roomName, $"{Config.GlobalConfig.UserName}: {text}");
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
