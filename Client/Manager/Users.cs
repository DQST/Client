using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;

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
    }
}
