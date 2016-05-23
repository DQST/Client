using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Client.Manager;

namespace Client.View
{
    class UserUITabItem : TabItem
    {
        private RoomManager manager;

        public UserUITabItem(string title, RoomManager manager, bool isClosed)
        {
            var tab = new UserTabItem(title, isClosed);
            this.manager = manager;
            tab.OnClose += (sender, e) => 
            {
                var tabControl = GetParentTabControl();
                tabControl?.Items?.Remove(this);
                this.manager?.Remove(title);
            };
            Header = tab;
        }

        private TabControl GetParentTabControl()
        {
            TabControl controll;
            if (Parent.GetType().Equals(typeof(TabControl)))
            {
                controll = Parent as TabControl;
                return controll;
            }
            return null;
        }
    }
}
