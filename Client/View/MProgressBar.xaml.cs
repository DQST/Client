using System.Windows.Controls;

namespace Client.View
{
    /// <summary>
    /// Логика взаимодействия для MProgressBar.xaml
    /// </summary>
    public partial class MProgressBar : UserControl
    {
        public MProgressBar()
        {
            InitializeComponent();
        }

        public MProgressBar(long maximum) : this()
        {
            bar.Maximum = maximum;
        }

        public void SetMaximum(long value)
        {
            bar.Dispatcher.Invoke(() => bar.Maximum = value);
        }

        public void SetValue(long value)
        {
            bar.Dispatcher.Invoke(() => bar.Value = value);
        }
    }
}
