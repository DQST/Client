using Client.Extensions;
using Client.View;
using HelpLib.Wrapper;
using System.Windows;
using System.Net.Sockets;
using System.Security.Cryptography;
using HelpLib.Config;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        private OloService service;

        public Rooms()
        {
            InitializeComponent();
            service = new OloService(this);

            UDP.OnReceive += UDP_OnReceive;

            updateButton.Click += UpdateButton_Click;
            createButton.Click += CreateButton_Click;
            createRoomButton.Click += CreateButton_Click;
            listBox.MouseDoubleClick += (s, e) => connect_Executed(s, null);

            Closed += (s, e) =>
            {
                UDP.OnReceive -= UDP_OnReceive;
                Close();
            };

            UpdateButton_Click(this, null);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var input = new InputDialog("Create room", "Room name:");
            input.ShowDialog();
            if (input.DialogResult.HasValue && input.DialogResult.Value)
            {
                var name = input.txtAnswer.Text;
                //var pass = input.pswAnswer.Password;
                if (!string.IsNullOrWhiteSpace(name))
                    UDP.Send(OloProtocol.GetOlo("add_room", name).ToBytes(),
                            Config.GlobalConfig.RemoteHost);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            UDP.Send(OloProtocol.GetOlo("get_rooms", null).ToBytes(),
                Config.GlobalConfig.RemoteHost);
        }

        private string GetMD5Hash(string str)
        {
            using (MD5 md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(str.ToBytes());
                return bytes.ToStr();
            }
        }

        private void UDP_OnReceive(object sender, UdpReceiveResult e)
        {
            var col = OloProtocol.Encode(e.Buffer.ToStr());
            service.Parse(col, e.RemoteEndPoint);
        }

        [OloField(Name = "room_list")]
        private void RoomList(params object[] args)
        {
            listBox.Items.Clear();
            for (int i = 0; i < args.Length - 1; i++)
                listBox.Items.Add(args[i].ToString());
        }

        private void delete_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            string name = listBox.SelectedItem as string;
            UDP.Send(OloProtocol.GetOlo("del_room", name).ToBytes(),
                Config.GlobalConfig.RemoteHost);
        }

        private void connect_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            string name = listBox.SelectedItem as string;
            if (name != null)
            {
                var olo = OloProtocol.GetOlo("con_to", name, Config.GlobalConfig.UserName);
                UDP.Send(olo.ToBytes(), Config.GlobalConfig.RemoteHost);
                Close();
                //var inputPass = new PassWindow();
                //inputPass.ShowDialog();
                //if (inputPass.DialogResult.HasValue && inputPass.DialogResult.Value)
                //{
                //    var pass = inputPass.pswAnswer.Password;
                //    UDP.Send(OloProtocol.GetOlo("con_to", name, GetMD5Hash(pass)).ToBytes(),
                //        HelpLib.Config.Config.GlobalConfig.RemoteHost);
                //    Close();
                //}
            }
        }
    }
}
