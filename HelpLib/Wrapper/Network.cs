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

    public enum State : int
    {
        NONE,
        CREATE_FILE,
        LOAD_FILE,
        WRITE_IN_SELECT_FILE,
        SELECT_FILE_PARTS,
    }

    public class Network : IDisposable
    {
        private static Network instance;
        private static UdpClient udpClient;
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

        public static long GetParts(string path)
        {
            long counts = 0;
            FileInfo info = new FileInfo(path);
            var fileName = info.Name;
            var fileSize = info.Length;

            if (fileSize <= 1024)
                counts = 1;
            else
                counts = fileSize / 1024;
            return counts;
        }

        public static async Task<bool> LoadFile(string fileName, IPEndPoint endPoint, Action<long> callbackMax, Action<long> callbackSetValue)
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
                SendCommand("loadfile", tcpClient);
                SendCommand(fileName, tcpClient);

                byte[] buffer = new byte[1024];
                long count = 0, part = 0;

                var path = Environment.CurrentDirectory + "\\Downloads\\";
                if (!File.Exists(path + fileName))
                    File.Create(path + fileName).Close();

                await stream.ReadAsync(buffer, 0, buffer.Length);
                var str = Encoding.UTF8.GetString(buffer);
                var size = long.Parse(str);
                count = size / 1024;
                callbackMax(count);

                try
                {
                    using (FileStream file = new FileStream(path + fileName, FileMode.Append))
                    {
                        while (await stream.ReadAsync(buffer, 0, buffer.Length) != 0)
                        {
                            if (buffer.Length == 1024)
                            {
                                await file.WriteAsync(buffer, 0, buffer.Length);
                                if (part <= count)
                                    callbackSetValue(++part);
                                var buf = Encoding.UTF8.GetBytes("NEXT");
                                stream.Write(buf, 0, buf.Length);
                            }
                        }
                    }
                }
                catch { return false; }
                finally
                {
                    stream.Close();
                    tcpClient.Close();
                }
                return true;
            }
            return false;
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
                var counts = GetParts(f.FilePath);

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
                    var buffer = new byte[1024];
                    FileInfo info = new FileInfo(f.FilePath);

                    SendCommand("create", tcpClient);
                    SendCommand(info.Name, tcpClient);
                    SendCommand("writeinselectfile", tcpClient);

                    try
                    {
                        using (FileStream file = new FileStream(f.FilePath, FileMode.Open))
                        {
                            while (file.Read(buffer, 0, buffer.Length) > 0)
                            {
                                var buf = new byte[1024];
                                stream.Read(buf, 0, buf.Length);
                                var com = Encoding.UTF8.GetString(buf);
                                if (com.IndexOf("NEXT") != -1)
                                {
                                    send:
                                    int count = tcpClient.Client.Send(buffer, 0, buffer.Length, SocketFlags.None);
                                    if (!count.Equals(buffer.Length))
                                        goto send;

                                    if (parts < counts)
                                        f.Callback(++parts);
                                }
                            }
                            var olo = OloProtocol.GetOlo("file_load", f.RoomName, 
                                Config.Config.GlobalConfig.UserName, info.Name);
                            Send(Encoding.UTF8.GetBytes(olo), Config.Config.GlobalConfig.RemoteHost);
                        }
                    }
                    finally
                    {
                        stream.Close();
                        tcpClient.Close();
                    }
                }
            }
        }

        private static void SendCommand(string message, TcpClient tcpClient)
        {
            var buf = Encoding.UTF8.GetBytes(message);
            s:
            int count = tcpClient.Client.Send(buf, 0, buf.Length, SocketFlags.None);
            if (!count.Equals(buf.Length))
                goto s;
            Thread.Sleep(100);
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
