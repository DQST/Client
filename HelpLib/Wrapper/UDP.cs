using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using HelpLib.Config;
using HelpLib.Wrapper;

namespace HelpLib.Wrapper
{
    public class UDP : IDisposable
    {
        private static UDP instance;
        private static UdpClient client;
        private static bool work;

        public static bool IsWork { get { return work; } }
        public static string Header { get; private set; }
        public static event EventHandler<UdpReceiveResult> OnReceive;

        private UDP()
        {
            instance = this;
        }

        public static UDP GetInstance(int port, string header)
        {
            client = new UdpClient(port, AddressFamily.InterNetwork);
            work = true;
            Header = header;
            return instance ?? new UDP();
        }

        public static int Send(byte[] data, IPEndPoint endPoint)
        {
            var r = client.Send(data, data.Length, endPoint);
            return r;
        }

        public async void Run()
        {
            while (work)
            {
                try
                {
                    UdpReceiveResult result = await client.ReceiveAsync();
                    OnReceive?.Invoke(this, result);
                }
                catch { }
            }
        }

        public void Dispose()
        {
            work = false;
            client.Close();
        }
    }
}
