using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.View;
using System.Windows.Controls;

namespace Client.Extensions
{
    static class Ext
    {
        public static UserTabItem GetSelectTab(this TabControl control)
        {
            var tab = control.Items[control.SelectedIndex] as UserUITabItem;
            return tab.Header as UserTabItem;
        }

        public static UserUITabItem Exists(this TabControl control, string name)
        {
            foreach (UserUITabItem item in control.Items) {
                var tab = item.Header as UserTabItem;
                if (tab?.Header?.Text == name)
                    return item;
            }
            return null;
        }

        public static void AddTab(this TabControl control, string name, bool isClosed = true)
        {
            if(Exists(control, name) == null)
            {
                var tab = new UserUITabItem(name, isClosed);
                var grid = new Grid();
                grid.Children.Add(new ListBox() { Name = "msgListBox" });
                tab.Content = grid;
                int index = control.Items.Add(tab);
                control.SelectedIndex = index;
            }
        }

        public static void PushMessage(this TabControl control, string name, string message)
        {
            var rez = control.Exists(name);
            if (rez != null)
            {
                var grid = rez.Content as Grid;
                foreach (var i in grid?.Children)
                {
                    var listBox = i as ListBox;
                    if (listBox != null && listBox.Name == "msgListBox")
                    {
                        listBox.Items.Add(message);
                        return;
                    }
                }
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
