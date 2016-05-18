using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.View
{
    /// <summary>
    /// Логика взаимодействия для UserTabItem.xaml
    /// </summary>
    public partial class UserTabItem : UserControl
    {
        public bool IsClosed { get; set; }
        public event EventHandler OnClose;

        public UserTabItem()
        {
            InitializeComponent();
            closeTabButton.Click += (sender, e) => OnClose(sender, e);
        }

        public UserTabItem(string title, bool isClosed) : this()
        {
            Header.Text = title;
            IsClosed = isClosed;
            if (IsClosed == true)
                closeTabButton.Visibility = Visibility.Visible;
            else
                closeTabButton.Visibility = Visibility.Hidden;
        }
    }
}
