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

        public MProgressBar(long max) : this()
        {
            bar.Maximum = max;
        }

        public void SetMaximum(long max)
        {
            bar.Maximum = max;
        }

        public void SetValue(long v)
        {
            bar.Value = v;
        }
    }
}
