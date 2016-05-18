using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Client.View
{
    class UserUITabItem : TabItem
    {
        public UserUITabItem(string title, bool isClosed)
        {
            var tab = new UserTabItem(title, isClosed);
            tab.OnClose += (sender, e) => 
            {
                var tabControll = GetParentTabControl();
                tabControll?.Items?.Remove(this);
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
