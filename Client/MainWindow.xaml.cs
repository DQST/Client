﻿using System;
using System.Threading;
using System.Windows;
using HelpLib.Config;
using HelpLib.Wrapper;
using System.Net.Sockets;
using Client.Extensions;
using System.Windows.Input;
using Microsoft.Win32;
using Client.View;
using System.Windows.Controls;
using System.IO;

namespace Client
{
    public partial class MainWindow : Window, IDisposable
    {
        private OloService service;
        private ConfigFile config;
        private Network net;
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
                if (e.Key == Key.Enter)
                    sendButton_Click(s, e);
            };

            try
            {
                Config.Load("settings.json", out config);
            }
            catch (FileNotFoundException)
            {
                config = new ConfigFile("0.0.0.0:49152", "104.236.30.123:14801", "defaultUser");
                Config.Save("settings.json", config);
            }
            finally
            {
                if (Properties.Settings.Default.UniqueKey == string.Empty)
                {
                    Properties.Settings.Default.UniqueKey = Config.GetUniqueKey();
                    Properties.Settings.Default.Save();
                }

                if(Properties.Settings.Default.FileFolder == string.Empty)
                {
                    Properties.Settings.Default.FileFolder = Environment.CurrentDirectory + "\\Downloads\\";
                    Properties.Settings.Default.Save();
                }

                if (!Directory.Exists(Properties.Settings.Default.FileFolder))
                    Directory.CreateDirectory(Properties.Settings.Default.FileFolder);

                service = new OloService(this);
                net = Network.GetInstance(ref config);
                Config.GlobalConfig = config;
                Network.OnReceive += Receive;
                net.Run();

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
                Network.Send(olo.ToBytes(), host);
                Network.Send(olo.ToBytes(), host);
                Network.Send(olo.ToBytes(), host);
                Thread.Sleep(1000);
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

        // TODO: add message about upload file
        [OloField(Name = "push_file")]
        private void PushFile(params object[] args)
        {
            var roomName = args[0].ToString();
            var userName = args[1].ToString();
            var fileName = args[2].ToString();
            if (tabControl.Exists(roomName) != null)
            {
                var button = new DownloadButton(fileName);
                button.downloadButton.Click += (s, e) =>
                {
                    var bar = new MProgressBar();
                    tabControl.PushMessage(roomName, bar);
                    Network.LoadFile($"0003:{fileName}".ToBytes(), Config.GlobalConfig.RemoteHost, bar.SetMaximum, bar.SetValue);
                };
                tabControl.PushMessage(roomName, $"{userName}:");
                tabControl.PushMessage(roomName, button);
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            var text = inputTextBox.Text;
            inputTextBox.Focus();
            inputTextBox.Clear();

            var tab = tabControl.GetSelectTab();
            if (tab != null)
            {
                var roomName = tab.Header.Text;
                text = text.TrimEnd('\n', '\r');
                if (!string.IsNullOrWhiteSpace(text))
                {
                    tabControl.PushMessage(roomName, $"{Config.GlobalConfig.UserName}: {text}");
                    var olo = OloProtocol.GetOlo("broadcast_all_in_room", roomName, Config.GlobalConfig.UserName, text);
                    Network.Send(olo.ToBytes(), Config.GlobalConfig.RemoteHost);
                }
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
            net.Dispose();
        }

        private void sendFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Text files (*.txt)|*.txt|Image files (*.png, *.jpeg, *jpg, *.bmp)|*.png;*.jpeg;*.bmp;*.jpg|All files (*.*)|*.*";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (fileDialog.ShowDialog() == true)
            {
                var filePath = fileDialog.FileName;
                var tab = tabControl.GetSelectTab();
                if (tab != null)
                {
                    var count = Network.GetParts(filePath);
                    var bar = new MProgressBar(count);
                    tabControl.PushMessage(tab.Header.Text, $"{Config.GlobalConfig.UserName}: sending file \"{fileDialog.SafeFileName}\"");
                    tabControl.PushMessage(tab.Header.Text, bar);
                    Network.SendFile(filePath, tab.Header.Text, Config.GlobalConfig.RemoteHost, bar.SetValue);
                }
            }
        }
    }
}
