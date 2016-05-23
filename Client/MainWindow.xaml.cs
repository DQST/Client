using System;
using System.Threading;
using System.Windows;
using HelpLib.Config;
using HelpLib.Wrapper;
using System.Net.Sockets;
using Client.Extensions;
using Client.Manager;

namespace Client
{
    public partial class MainWindow : Window, IDisposable
    {
        private OloService service;
        private ConfigFile config;
        private UDP udp;
        private RoomManager manager;
        private bool bridgeWork = true;

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
                config = new ConfigFile("0.0.0.0:14800", "104.236.30.123:14801", "defaultUser");
                Config.Save("settings.json", config);
            }
            finally
            {
                service = new OloService(this);
                manager = new RoomManager();
                udp = UDP.GetInstance(ref config);
                Config.GlobalConfig = config;
                UDP.OnReceive += Receive;
                udp.Run();

                tabControl.AddTab("Debug", manager, false);
                tabControl.PushMessage("Debug", $"local ip: {config.LocalHost.ToString()}");

                ThreadPool.QueueUserWorkItem(Bridge);
                Title = $"{Title} @ {config.UserName}";
            }
        }

        private void Bridge(object o)
        {
            while (bridgeWork)
            {
                for (int i = 0; i < manager.Rooms.Count; i++)
                {
                    var name = manager.Rooms[i];
                    var users = manager[name];
                    users.BraodcastNop();
                    Thread.Sleep(100);
                }
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
                tabControl.AddTab(roomName, manager);
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

        [OloField(Name = "nop")]
        private void NopMsg(params object[] args)
        {
            
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            var text = inputTextBox.Text;
            inputTextBox.Focus();
            inputTextBox.Clear();

            var tab = tabControl.GetSelectTab();
            var roomName = tab.Header.Text;
            if (!string.IsNullOrWhiteSpace(text) && manager.IsExists(roomName))
            {
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
            var settings = new Settings();
            settings.Owner = this;
            settings.ShowDialog();
            Title = $"{Title} @ {Config.GlobalConfig.UserName}";
        }

        public void Dispose()
        {
            bridgeWork = false;
            Config.Save("settings.json", Config.GlobalConfig);
            manager.Dispose();
            udp.Dispose();
        }
    }
}
