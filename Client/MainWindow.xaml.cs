using System;
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
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public partial class MainWindow : Window, IDisposable
    {
        private OloService service;
        private ConfigFile config;
        private Network net;
        private bool bridgeWork = true;
        private Login login;

        public MainWindow()
        {
            InitializeComponent();
            Closed += (s, e) =>
            {
                Dispose();
                Close();
            };

            exitButton.Click += (s, e) => OnClosed(e);
            relogin.Click += (s, e) => { Hide(); login.Show(); };
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
                if (!Directory.Exists(Environment.CurrentDirectory + "\\Downloads\\"))
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\Downloads\\");

                service = new OloService(this);
                net = Network.GetInstance(ref config);
                Config.GlobalConfig = config;
                Network.OnReceive += Receive;
                net.Run();

                ThreadPool.QueueUserWorkItem(Bridge);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Hide();
            login  = new Login();
            login.Owner = this;
            login.Show();
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
            {
                tabControl.AddTab(roomName);
                var olo = OloProtocol.GetOlo("get_history", roomName);
                Network.Send(olo.ToBytes(), Config.GlobalConfig.RemoteHost);
            }
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

        [OloField(Name = "push_users")]
        private void SetUserList(params object[] args)
        {
            var roomName = args[0].ToString();
            var array = args[2] as IEnumerable;
            if (tabControl.Exists(roomName) != null)
                tabControl.PushUser(roomName, array);
        }

        [OloField(Name = "set_nickname")]
        private void SetNickname(params object[] args)
        {
            var oldConfig = Config.GlobalConfig;
            Config.GlobalConfig = new ConfigFile(oldConfig.LocalHost.ToString(), 
                oldConfig.RemoteHost.ToString(), args[0].ToString());
            Title = $"Chat v0.1 @ {args[0].ToString()}";
        }

        [OloField(Name = "push_file")]
        private void PushFile(params object[] args)
        {
            var roomName = args[0].ToString();
            var userName = args[1].ToString();
            var fileName = args[2].ToString();
            if (tabControl.Exists(roomName) != null)
            {
                var button = new DownloadButton(fileName);
                button.downloadButton.Click += async (s, e) =>
                {
                    var bar = new MProgressBar();
                    tabControl.PushMessage(roomName, bar);
                    var r = await Network.LoadFile($"0003:{fileName}".ToBytes(), Config.GlobalConfig.RemoteHost, bar.SetMaximum, bar.SetValue);
                    if (r) tabControl.PushMessage(roomName, $"\"{fileName}\" загружен!");
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
            Title = $"Chat v0.1 @ {Config.GlobalConfig.UserName}";
        }

        public void Dispose()
        {
            bridgeWork = false;
            Config.Save("settings.json", Config.GlobalConfig);
            Network.OnReceive -= Receive;
            net.Dispose();
        }

        private void sendFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Все файлы (*.*)|*.*|Текстовые файлы (*.txt, *.doc, *.docx)|*.txt;*.doc;*docx|Изображения (*.png, *.jpeg, *jpg, *.bmp)|*.png;*.jpeg;*.bmp;*.jpg";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (fileDialog.ShowDialog() == true)
            {
                var filePath = fileDialog.FileName;
                var tab = tabControl.GetSelectTab();
                if (tab != null)
                {
                    var count = Network.GetParts(filePath);
                    var bar = new MProgressBar(count);
                    tabControl.PushMessage(tab.Header.Text, $"{Config.GlobalConfig.UserName}: отправка файла: \"{fileDialog.SafeFileName}\"");
                    tabControl.PushMessage(tab.Header.Text, bar);
                    Network.SendFile(filePath, tab.Header.Text, bar.SetValue);
                }
            }
        }

        private void showHelp_Click(object sender, RoutedEventArgs e)
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory);
            foreach (var file in files)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(file, @"(.)\w+\.chm",
                    System.Text.RegularExpressions.RegexOptions.ECMAScript |
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                    System.Text.RegularExpressions.RegexOptions.Compiled))
                {
                    System.Diagnostics.Process.Start(file);
                    return;
                }
            }
            MessageBox.Show("Help file not found!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
