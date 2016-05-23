using System.Collections.Generic;
using System.Net;
using System.Collections;
using HelpLib.Wrapper;
using Client.Extensions;
using System;

namespace Client.Manager
{
    class Users : IEnumerable, IDisposable
    {
        private List<IPEndPoint> users = new List<IPEndPoint>();

        public void Add(IPEndPoint ip)
        {
            if (!users.Contains(ip))
                users.Add(ip);
        }

        public IEnumerator GetEnumerator()
        {
            return users.GetEnumerator();
        }

        public bool Remove(IPEndPoint ip)
        {
            return users.Remove(ip);
        }

        public void BraodcastMessage(params object[] args)
        {
            lock (users)
            {
                foreach (var ip in users)
                    UDP.Send(OloProtocol.GetOlo("push_message", args).ToBytes(), ip);
            }
        }

        public void BraodcastNop()
        {
            lock (users)
            {
                for (int i = 0; i < users.Count; i++)
                    UDP.Send(OloProtocol.GetOlo("nop").ToBytes(), users[i]);
            }
        }

        public void BraodcastMessage(IPEndPoint exceptionIP, params object[] args)
        {
            lock (users)
            {
                foreach (var ip in users)
                    if (!ip.Equals(exceptionIP))
                        UDP.Send(OloProtocol.GetOlo("push_message", args).ToBytes(), ip);
            }
        }

        public void Dispose()
        {
            users.Clear();
        }
    }
}
