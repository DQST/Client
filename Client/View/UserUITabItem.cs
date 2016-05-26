using System.Windows.Controls;
using HelpLib.Config;
using HelpLib.Wrapper;
using Client.Extensions;

namespace Client.View
{
    class UserUITabItem : TabItem
    {
        public UserUITabItem(string title, bool isClosed)
        {
            var tab = new UserTabItem(title, isClosed);
            tab.OnClose += (sender, e) => 
            {
                var tabControl = GetParentTabControl();
                tabControl?.Items?.Remove(this);
                var olo = OloProtocol.GetOlo("disconnect_from", title, Config.GlobalConfig.UserName);
                Network.Send(olo.ToBytes(), Config.GlobalConfig.RemoteHost);
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
