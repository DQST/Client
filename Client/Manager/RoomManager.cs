using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Client.Extensions;
using HelpLib.Wrapper;

namespace Client.Manager
{
    class RoomManager
    {
        private Dictionary<string, Users> rooms = new Dictionary<string, Users>();

        public void Add(string name, Users room)
        {
            rooms.Add(name, room);
        }

        public void Remove(string name)
        {
            rooms.Remove(name);
        }

        public bool IsExists(string name)
        {
            if (rooms.ContainsKey(name))
                return true;
            return false;
        }

        public Users this[string name]
        {
            get { return rooms[name]; }
            set { rooms[name] = value; }
        }

        public void BroadcastAllUser(string name, params object[] args)
        {
            foreach (IPEndPoint addr in this[name])
                UDP.Send(OloProtocol.GetOlo("push_message", args).ToBytes(), addr);
        }
    }
}
