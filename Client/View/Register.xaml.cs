using Client.Extensions;
using HelpLib.Config;
using HelpLib.Wrapper;
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

namespace Client.View
{
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private OloService service;

        public Register()
        {
            InitializeComponent();
            service = new OloService(this);

            Closing += (s, e) => Owner.Show();

            Network.OnReceive += (s, e) =>
            {
                var data = OloProtocol.Encode(e.Buffer.ToStr());
                service.Parse(data, e.RemoteEndPoint);
            };

            register.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(login.Text) && !string.IsNullOrWhiteSpace(pass.Password))
                {
                    var olo = OloProtocol.GetOlo("register", login.Text, pass.Password);
                    Network.Send(olo.ToBytes(), Config.GlobalConfig.RemoteHost);
                }
            };
        }

        [OloField(Name = "reg_ok")]
        private void Enter(params object[] args)
        {
            MessageBox.Show(args[0].ToString(), "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
