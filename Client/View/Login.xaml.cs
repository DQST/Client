using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HelpLib.Wrapper;
using HelpLib.Config;
using Client.Extensions;

namespace Client.View
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private OloService service;

        public Login()
        {
            InitializeComponent();
            service = new OloService(this);

            if (Properties.Settings.Default.Checked)
            {
                login.Text = Properties.Settings.Default.User_login;
                pass.Password = Properties.Settings.Default.User_password;
                saveSettings.IsChecked = Properties.Settings.Default.Checked;
            }

            Closing += (s, e) => Owner.Close();

            Network.OnReceive += (s, e) =>
            {
                var data = OloProtocol.Encode(e.Buffer.ToStr());
                service.Parse(data, e.RemoteEndPoint);
            };

            settings.Click += (s, e) =>
            {
                var settings = new Settings();
                settings.ShowDialog();
            };

            enterButton.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(login.Text) && !string.IsNullOrWhiteSpace(pass.Password))
                {
                    var olo = OloProtocol.GetOlo("login", login.Text, pass.Password);
                    Network.Send(olo.ToBytes(), Config.GlobalConfig.RemoteHost);
                    if (saveSettings.IsChecked.Value && saveSettings.IsChecked.HasValue)
                    {
                        Properties.Settings.Default.User_login = login.Text;
                        Properties.Settings.Default.User_password = pass.Password;
                        Properties.Settings.Default.Checked = true;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Properties.Settings.Default.Checked = false;
                        Properties.Settings.Default.Save();
                    }
                }
            };
        }

        [OloField(Name = "enter")]
        private void Enter(params object[] args)
        {
            MessageBox.Show(args[0].ToString(), "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            var config = Config.GlobalConfig;
            Config.GlobalConfig = new ConfigFile(config.LocalHost.ToString(), 
                config.RemoteHost.ToString(), args[1].ToString());
            Properties.Settings.Default.UserID = long.Parse(args[2].ToString());
            Properties.Settings.Default.Save();
            Owner.Title = $"Chat v0.1 @ {args[1].ToString()}";
            Owner.Show();
            Hide();
        }

        [OloField(Name = "error")]
        private void Error(params object[] args)
        {
            MessageBox.Show(args[0].ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void linkClick(object sender, RoutedEventArgs e)
        {
            Hide();
            var register = new Register();
            register.Owner = this;
            register.ShowDialog();
        }
    }
}
