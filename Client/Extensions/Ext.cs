using System.Text;
using Client.View;
using System.Windows.Controls;
using System.Collections;

namespace Client.Extensions
{
    static class Ext
    {
        public static UserTabItem GetSelectTab(this TabControl control)
        {
            UserUITabItem tab = null;
            if (control.Items.Count > 0)
                tab = control.Items[control.SelectedIndex] as UserUITabItem;
            return tab?.Header as UserTabItem;
        }

        public static UserUITabItem Exists(this TabControl control, string name)
        {
            foreach (UserUITabItem item in control.Items)
            {
                var tab = item.Header as UserTabItem;
                if (tab?.Header?.Text == name)
                    return item;
            }
            return null;
        }

        public static void AddTab(this TabControl control, string name, bool isClosed = true)
        {
            if (Exists(control, name) == null)
            {
                var tab = new UserUITabItem(name, isClosed);
                tab.Content = new UserItem() { Height = tab.Height };
                int index = control.Items.Add(tab);
                control.SelectedIndex = index;
            }
        }

        public static void PushUser(this TabControl control, string name, IEnumerable obj)
        {
            var rez = control.Exists(name);
            if (rez != null)
            {
                var item = rez.Content as UserItem;
                item?.SetUsers(obj);
            }
        }

        public static void PushMessage(this TabControl control, string name, object obj)
        {
            var rez = control.Exists(name);
            if (rez != null)
            {
                var item = rez.Content as UserItem;
                item?.PushMessage(obj);
            }
        }

        public static string ToStr(this byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static byte[] ToBytes(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static System.Net.IPEndPoint ToIP(this string s)
        {
            var arr = s.Split(':');
            return new System.Net.IPEndPoint(System.Net.IPAddress.Parse(arr[0]), arr[1].ToInt());
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }
    }
}
