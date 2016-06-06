using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using HelpLib.Config;
using System.Net.NetworkInformation;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;

namespace HelpLib.Wrapper
{
    class SFile<T>
    {
        public string RoomName { get; set; }
        public string FilePath { get; set; }
        public Action<T> Callback { get; set; }
    }

    public class Network : IDisposable
    {
        private static Network instance;
        private static UdpClient udpClient;
        private static bool work;
        private static int i = 0;
        const int bufferSize = 1028;

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

        private static int GetEmptyPort()
        {
            for (int i = 49152; i <= IPEndPoint.MaxPort; i++)
                if (PortInUse(i) == false)
                    return i;
            return 49152;
        }

        public static Network GetInstance(ref ConfigFile config)
        {
            if (PortInUse(config.LocalHost.Port) == true)
                config.LocalHost.Port = GetEmptyPort();
            udpClient = new UdpClient(config.LocalHost.Port, AddressFamily.InterNetwork);
            work = true;
            return instance ?? new Network();
        }

        public static int Send(byte[] data, IPEndPoint endPoint)
        {
            var r = udpClient.Send(data, data.Length, endPoint);
            return r;
        }

        public static async Task<bool> LoadFile(byte[] data, IPEndPoint endPoint, Action<long> callbackMax, Action<long> callbackSetValue)
        {
            var tcpClient = new TcpClient(AddressFamily.InterNetwork);

            try
            {
                tcpClient.Connect(Config.Config.GlobalConfig.RemoteHost);
            }
            catch (SocketException)
            {
                MessageBox.Show("Ошибка загрузки файла!\nУдаленный сервер недоступен!\nПовторите попытку позже!",
                        "Ошибка отправки файла", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (tcpClient.Connected)
            {
                var stream = tcpClient.GetStream();
                stream.Write(data, 0, data.Length);
                byte[] buffer = new byte[bufferSize];
                long count = 0, part = 0;

                var path = Environment.CurrentDirectory + "\\Downloads\\";

                if (!File.Exists(path + $".file_{i}"))
                    File.Create(path + $".file_{i}").Close();

                while (await stream.ReadAsync(buffer, 0, buffer.Length) != 0)
                {
                    var head = Encoding.UTF8.GetString(buffer, 0, 4);
                    if (head == "0000")
                    {
                        var s = Encoding.UTF8.GetString(buffer).Split(':');
                        count = long.Parse(s[1]);
                        callbackMax(count);
                    }
                    else if (head == "0001")
                    {
                        FileStream file = new FileStream(path + $".file_{i}", FileMode.Append);
                        await file.WriteAsync(buffer, 4, buffer.Length - 4);
                        file.Close();
                        if (part < count)
                            callbackSetValue(++part);
                    }
                    else if (head == "0002")
                    {
                        try
                        {
                            var s = Encoding.UTF8.GetString(buffer).Split(':');
                            File.Move(path + $".file_{i}", path + s[1]);
                            ++i;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

                stream.Close();
                tcpClient.Close();
                return true;
            }
            return false;
        }

        public static long GetParts(string path)
        {
            long counts = 0;
            FileInfo info = new FileInfo(path);
            var fileName = info.Name;
            var fileSize = info.Length;

            if (fileSize <= bufferSize)
                counts = 1;
            else
                counts = fileSize / bufferSize;

            return counts;
        }

        public static void SendFile(string path, string room, Action<long> callback)
        {
            var file = new SFile<long>();
            file.FilePath = path;
            file.RoomName = room;
            file.Callback = callback;
            ThreadPool.QueueUserWorkItem(SendBackground, file);
        }

        private static void SendBackground(object o)
        {
            var f = o as SFile<long>;
            int parts = 0;

            if (f != null && File.Exists(f.FilePath))
            {
                FileInfo info = new FileInfo(f.FilePath);
                var fileName = info.Name;
                var fileSize = info.Length;
                long counts;

                if (fileSize <= bufferSize)
                    counts = 1;
                else
                    counts = fileSize / bufferSize;

                var tcpClient = new TcpClient(AddressFamily.InterNetwork);

                try
                {
                    tcpClient.Connect(Config.Config.GlobalConfig.RemoteHost);
                }
                catch (SocketException)
                {
                    MessageBox.Show("Ошибка отправки файла!\nУдаленный сервер недоступен!\nПовторите попытку позже!",
                        "Ошибка отправки файла", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (tcpClient.Connected)
                {
                    NetworkStream stream = tcpClient.GetStream();
                    var buffer = new byte[bufferSize];

                    buffer[0] = 48;
                    buffer[1] = 48;
                    buffer[2] = 48;
                    buffer[3] = 49;
                    using (FileStream file = new FileStream(f.FilePath, FileMode.Open))
                    {
                        while (file.Read(buffer, 4, buffer.Length - 4) > 0)
                        {
                            send:
                            int count = tcpClient.Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
                            if (!count.Equals(buffer.Length))
                                goto send;

                            if (parts < counts)
                                f.Callback(++parts);
                        }
                        buffer = Encoding.UTF8.GetBytes($"0002:{info.Name}");
                        stream.Write(buffer, 0, buffer.Length);
                        var olo = OloProtocol.GetOlo("file_load", f.RoomName, Config.Config.GlobalConfig.UserName, fileName);
                        var data = Encoding.UTF8.GetBytes(olo);
                        Send(data, Config.Config.GlobalConfig.RemoteHost);
                    }
                    stream.Close();
                    tcpClient.Close();
                }
            }
        }

        public async void Run()
        {
            while (work)
            {
                try
                {
                    UdpReceiveResult result = await udpClient.ReceiveAsync();
                    OnReceive?.Invoke(this, result);
                }
                catch { }
            }
        }

        public void Dispose()
        {
            work = false;
            udpClient.Close();
        }
    }
}
