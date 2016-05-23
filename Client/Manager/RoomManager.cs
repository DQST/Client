using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Client.Manager
{
    class RoomManager : IEnumerable, IDisposable
    {
        private Dictionary<string, Users> rooms = new Dictionary<string, Users>();
        private static RoomManager instance;

        public List<string> Rooms { get { return rooms.Keys.ToList(); } }

        private RoomManager()
        {
            instance = this;
        }

        public static RoomManager GetInstance()
        {
            return instance ?? new RoomManager();
        }

        public void Add(string name, Users room)
        {
            rooms.Add(name, room);
        }

        public bool Remove(string name)
        {
            return rooms.Remove(name);
        }

        public bool IsExists(string name)
        {
            if (rooms.ContainsKey(name))
                return true;
            return false;
        }

        public void Dispose()
        {
            foreach (var item in rooms.Keys)
                this[item].Dispose();
            rooms.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return rooms.Keys.GetEnumerator();
        }

        public Users this[string name]
        {
            get { return rooms[name]; }
            set { rooms[name] = value; }
        }
    }
}
