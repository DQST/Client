using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.View
{
    /// <summary>
    /// Логика взаимодействия для InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog()
        {
            InitializeComponent();
            flag.Checked += Flag_Checked;
            flag.Unchecked += Flag_Checked;
            btnDialogOk.Click += (s, e) =>
            {
                if (Regex.IsMatch(pswAnswer.Password, @"^[a-zA-Z0-9]+$") && !string.IsNullOrWhiteSpace(txtAnswer.Text))
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Incorrect password! Password must be state only from select characters:\n a-z or A-Z and numbers from 0-9.",
                        "Icorrect password", MessageBoxButton.OK, MessageBoxImage.Warning);
                    pswAnswer.Clear();
                    pswAnswer.Focus();
                }
            };
            btnDialogCancel.Click += (s, e) =>
            {
                DialogResult = false;
                Close();
            };
        }

        public InputDialog(string title, string header) : this()
        {
            Title = title;
            lblQuestion.Content = header;
        }

        private void Flag_Checked(object sender, RoutedEventArgs e)
        {
            if (flag.IsChecked.Value == true)
                stackPanel.Visibility = Visibility.Visible;
            else
                stackPanel.Visibility = Visibility.Collapsed;
        }
    }
}
