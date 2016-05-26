using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HelpLib.Config;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using System.Windows;
using System.Threading.Tasks;

namespace HelpLib.Wrapper
{
    public class Network : IDisposable
    {
        private static Network instance;
        private static UdpClient client;
        private static bool work;

        public static bool IsWork { get { return work; } }
        public static event EventHandler<UdpReceiveResult> OnReceive;

        private Network()
        {
            instance = this;
        }

        private static bool PortInUse(int port)
        {
            bool inUse = false;
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            var ipEndPoints = ipProperties.GetActiveUdpListeners();

            foreach (var item in ipEndPoints)
            {
                if (item.Port == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;
        }

        private static int GetRandomPort()
        {
            for (int i = 49152; i <= IPEndPoint.MaxPort; i++)
                if (PortInUse(i) == false)
                    return i;
            return 49152;
        }

        public static Network GetInstance(ref ConfigFile config)
        {
            if (PortInUse(config.LocalHost.Port) == true)
                config.LocalHost.Port = GetRandomPort();
            client = new UdpClient(config.LocalHost.Port, AddressFamily.InterNetwork);
            work = true;
            return instance ?? new Network();
        }

        public static int Send(byte[] data, IPEndPoint endPoint)
        {
            var r = client.Send(data, data.Length, endPoint);
            return r;
        }

        public static void SendFile(string path, string inRoom, IPEndPoint endPoint)
        {
            ThreadPool.QueueUserWorkItem(FileBridge, new string[] {path, inRoom });
        }

        private static void FileBridge(object o)
        {
            long parts = 0;
            var arr = o as string[];
            var path = arr[0];
            var room = arr[1];
            if (path != null && File.Exists(path))
            {
                const int bufferSize = 1028;
                FileInfo info = new FileInfo(path);
                var fileName = info.Name;
                var fileSize = info.Length;
                long counts;

                if (fileSize <= bufferSize)
                    counts = 1;
                else
                    counts = (long)Math.Floor(fileSize / (double)bufferSize);

                TcpClient tcp = null;
                NetworkStream stream = null;

                try
                {
                    tcp = new TcpClient();
                    tcp.Connect(Config.Config.GlobalConfig.RemoteHost);
                    stream = tcp.GetStream();
                }
                catch (SocketException e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    var buffer = new byte[bufferSize];
                    buffer[0] = 0;
                    buffer[1] = 0;
                    buffer[2] = 0;
                    buffer[3] = 1;
                    using (FileStream file = new FileStream(path, FileMode.Open))
                    {
                        while (file.Read(buffer, 4, buffer.Length - 4) > 0)
                        {
                            stream.Write(buffer, 0, buffer.Length);
                            if (parts < counts)
                                ++parts;
                        }
                        buffer = Encoding.UTF8.GetBytes($"0002:{info.Name}");
                        stream.Write(buffer, 0, buffer.Length);
                        var olo = OloProtocol.GetOlo("file_load", room, Config.Config.GlobalConfig.UserName, fileName);
                        var data = Encoding.UTF8.GetBytes(olo);
                        Send(data, Config.Config.GlobalConfig.RemoteHost);
                    }
                    stream.Close();
                    tcp.Close();
                }
            }
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
