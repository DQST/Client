using System.Windows;
using HelpLib.Config;

namespace Client
{
    public partial class Settings : Window
    {
        private ConfigFile config;
        private readonly string file;

        public Settings(string file)
        {
            InitializeComponent();
            this.file = file;
            Config.Load(this.file, out config);
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            userNameText_Box.Text = config.UserName;
            localIPText_Box.Text = config.LocalHost.ToString();
            remoteIPText_Box.Text = config.RemoteHost.ToString();
        }

        private void OnCkickSave_button(object sender, RoutedEventArgs e)
        {
            config = new ConfigFile(remoteIPText_Box.Text, localIPText_Box.Text, userNameText_Box.Text);
            Config.Save(file, config);
            Close();
        }
    }
}
