using System.Collections;
using System.Windows.Controls;

namespace Client.View
{
    /// <summary>
    /// Логика взаимодействия для UserItem.xaml
    /// </summary>
    public partial class UserItem : UserControl
    {
        public UserItem()
        {
            InitializeComponent();
        }

        public void PushMessage(object obj)
        {
            var pos = mesageList.Items.Add(obj);
            mesageList.SelectedIndex = pos;
            mesageList.ScrollIntoView(mesageList.SelectedItem);
            mesageList.SelectedIndex = -1;
        }

        public void SetUsers(IEnumerable en)
        {
            usersList.Items.Clear();
            foreach (var item in en)
                usersList.Items.Add(item);
        }
    }
}
