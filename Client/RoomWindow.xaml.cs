using Client.Extensions;
using Client.View;
using HelpLib.Wrapper;
using System.Collections.Generic;
using System.Windows;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Rooms.xaml
    /// </summary>
    public partial class Rooms : Window
    {
        private class Room
        {
            private bool havePass;
            private bool youOwner;

            public bool HavePass { get { return havePass; } }
            public bool YouOwner { get { return youOwner; } }

            public Room(bool havePass, bool youOwner)
            {
                this.havePass = havePass;
                this.youOwner = youOwner;
            }
        }


        private OloService service;

        public Rooms()
        {
            InitializeComponent();
            service = new OloService(this);

            UDP.OnReceive += UDP_OnReceive;

            updateButton.Click += (s, e) => UDP.Send(OloProtocol.GetOlo("get_rooms", null).ToBytes(),
                HelpLib.Config.Config.GlobalConfig.RemoteHost);

            Closed += (s, e) =>
            {
                UDP.OnReceive -= UDP_OnReceive;
                Close();
            };

            createButton.Click += (s, e) =>
            {
                var input = new InputDialog("Create room", "Room name:");
                input.ShowDialog();
                if (input.DialogResult.HasValue && input.DialogResult.Value)
                {
                    var name = input.txtAnswer.Text;
                    var pass = input.pswAnswer.Password;
                    if(!string.IsNullOrWhiteSpace(name))
                        UDP.Send(OloProtocol.GetOlo("add_room", name, string.IsNullOrWhiteSpace(pass) ? "" : pass).ToBytes(),
                                HelpLib.Config.Config.GlobalConfig.RemoteHost);
                }
            };

            listBox.MouseDoubleClick += (s, e) => connect_Executed(s, null);
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
                listBox.Items.Add(args[i]);
        }

        private void delete_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            string name = listBox.SelectedItem as string;
            UDP.Send(OloProtocol.GetOlo("del_room", name).ToBytes(),
                HelpLib.Config.Config.GlobalConfig.RemoteHost);
        }

        private void connect_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            string name = listBox.SelectedItem as string;
            if (name != null)
            {
                UDP.Send(OloProtocol.GetOlo("con_to", name).ToBytes(), 
                    HelpLib.Config.Config.GlobalConfig.RemoteHost);
                Close();
            }
        }
    }
}
