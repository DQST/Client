using System.Windows.Controls;

namespace Client.View
{
    /// <summary>
    /// Логика взаимодействия для DownloadButton.xaml
    /// </summary>
    public partial class DownloadButton : UserControl
    {
        public DownloadButton()
        {
            InitializeComponent();
        }

        public DownloadButton(string file) : this()
        {
            downloadButton.Content += file;
        }
    }
}
