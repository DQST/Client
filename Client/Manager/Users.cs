using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;
using HelpLib.Wrapper;
using Client.Extensions;

namespace Client.Manager
{
    class Users : IEnumerable
    {
        private List<IPEndPoint> users = new List<IPEndPoint>();

        public void Add(IPEndPoint ip)
        {
            users.Add(ip);
        }

        public IEnumerator GetEnumerator()
        {
            return users.GetEnumerator();
        }

        public void RemoveAt(IPEndPoint ip)
        {
            users.Remove(ip);
        }

        public void BraodcastMessage(params object[] args)
        {
            lock (users)
            {
                foreach (var ip in users)
                    UDP.Send(OloProtocol.GetOlo("push_message", args).ToBytes(), ip);
            }
        }

        public void BraodcastMessage(IPEndPoint exceptionIP, params object[] args)
        {
            lock (users)
            {
                foreach (var ip in users)
                    if(!ip.Equals(exceptionIP))
                        UDP.Send(OloProtocol.GetOlo("push_message", args).ToBytes(), ip);
            }
        }
    }
}
