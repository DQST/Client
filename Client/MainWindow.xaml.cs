﻿using System;
using System.Threading;
using System.Windows;
using HelpLib.Config;
using HelpLib.Wrapper;
using System.Net.Sockets;
using Client.Extensions;
using System.Windows.Input;

namespace Client
{
    public partial class MainWindow : Window, IDisposable
    {
        private OloService service;
        private ConfigFile config;
        private UDP udp;
        private bool bridgeWork = true;

        public MainWindow()
        {
            InitializeComponent();
            Closed += (s, e) =>
            {
                Dispose();
                Close();
            };

            exitButton.Click += (s, e) => OnClosed(e);
            inputTextBox.KeyUp += (s, e) =>
            {
                if(e.Key == Key.Enter)
                    sendButton_Click(s, e);
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
                udp = UDP.GetInstance(ref config);
                Config.GlobalConfig = config;
                UDP.OnReceive += Receive;
                udp.Run();

                tabControl.AddTab("Debug", false);
                tabControl.PushMessage("Debug", $"local ip: {config.LocalHost.ToString()}");

                ThreadPool.QueueUserWorkItem(Bridge);
                Title = $"{Title} @ {config.UserName}";
            }
        }

        private void Bridge(object o)
        {
            while (bridgeWork)
            {
                var host = Config.GlobalConfig.RemoteHost;
                var olo = OloProtocol.GetOlo("nop", null);
                UDP.Send(olo.ToBytes(), host);
                UDP.Send(olo.ToBytes(), host);
                UDP.Send(olo.ToBytes(), host);
                Thread.Sleep(500);
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
            if (tabControl.Exists(roomName) == null)
                tabControl.AddTab(roomName);
        }

        [OloField(Name = "push_message")]
        private void PushMessage(params object[] args)
        {
            var roomName = args[0].ToString();
            var userName = args[1].ToString();
            var message = args[2].ToString();
            if (tabControl.Exists(roomName) != null)
                tabControl.PushMessage(roomName, $"{userName}: {message}");
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
            text = text.TrimEnd('\n', '\r');
            if (!string.IsNullOrWhiteSpace(text))
            {
                tabControl.PushMessage(roomName, $"{Config.GlobalConfig.UserName}: {text}");
                var olo = OloProtocol.GetOlo("broadcast_all_in_room", roomName, Config.GlobalConfig.UserName, text);
                UDP.Send(olo.ToBytes(), Config.GlobalConfig.RemoteHost);
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
            Title = $"{Title.Split('@')[0]} @ {Config.GlobalConfig.UserName}";
        }

        public void Dispose()
        {
            bridgeWork = false;
            Config.Save("settings.json", Config.GlobalConfig);
            udp.Dispose();
        }
    }
}
