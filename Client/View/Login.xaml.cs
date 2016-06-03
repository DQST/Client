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

            Closing += (s, e) => Owner.Close();

            Network.OnReceive += (s, e) =>
            {
                var data = OloProtocol.Encode(e.Buffer.ToStr());
                service.Parse(data, e.RemoteEndPoint);
            };

            enterButton.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(login.Text) && !string.IsNullOrWhiteSpace(pass.Password))
                {
                    var olo = OloProtocol.GetOlo("login", login.Text, pass.Password);
                    Network.Send(olo.ToBytes(), Config.GlobalConfig.RemoteHost);
                }
            };
        }

        [OloField(Name = "enter")]
        public void Enter(params object[] args)
        {
            Owner.Show();
            Close();
        }

        [OloField(Name = "error")]
        public void Error(params object[] args)
        {
            MessageBox.Show(args[0].ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
