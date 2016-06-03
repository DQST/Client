using System.Windows;
using HelpLib.Config;

namespace Client
{
    public partial class Settings : Window
    {
        private UIElement last;

        public Settings()
        {
            InitializeComponent();

            userName.Text = Config.GlobalConfig.UserName;
            localIP.Text = Config.GlobalConfig.LocalHost.ToString();
            remoteHost.Text = Config.GlobalConfig.RemoteHost.ToString();

            UserItem.MouseLeftButtonUp += (s, e) =>
            {
                User.Visibility = Visibility.Visible;
                last = User;
            };

            treeView.SelectedItemChanged += (s, e) =>
            {
                if (last != null)
                    last.Visibility = Visibility.Hidden;
            };
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Config.GlobalConfig = new ConfigFile(localIP.Text, remoteHost.Text, userName.Text);
            DialogResult = true;
            Close();
        }
    }
}
