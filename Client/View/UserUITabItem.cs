using System.Windows.Controls;
using Client.Manager;

namespace Client.View
{
    class UserUITabItem : TabItem
    {
        private RoomManager manager;

        public UserUITabItem(string title, bool isClosed)
        {
            var tab = new UserTabItem(title, isClosed);
            manager = RoomManager.GetInstance();
            tab.OnClose += (sender, e) => 
            {
                var tabControl = GetParentTabControl();
                tabControl?.Items?.Remove(this);
                manager?.Remove(title);
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
