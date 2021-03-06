﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using HelpLib.Config;
using HelpLib.Wrapper;
using System.Net.NetworkInformation;

namespace HelpLib.Wrapper
{
    public class UDP : IDisposable
    {
        private static UDP instance;
        private static UdpClient client;
        private static bool work;

        public static bool IsWork { get { return work; } }
        public static event EventHandler<UdpReceiveResult> OnReceive;

        private UDP()
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
                if(item.Port == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;
        }

        private static int GetRandomPort()
        {
            for (int i = 14800; i < IPEndPoint.MaxPort; i++)
                if (PortInUse(i) == false)
                    return i;
            return IPEndPoint.MinPort;
        }

        public static UDP GetInstance(ref ConfigFile config)
        {
            if (PortInUse(config.LocalHost.Port) == true)
                config.LocalHost.Port = GetRandomPort();
            client = new UdpClient(config.LocalHost.Port, AddressFamily.InterNetwork);
            work = true;
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
