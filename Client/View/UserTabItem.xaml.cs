using System;
using System.Windows;
using System.Windows.Controls;

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
