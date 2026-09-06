using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI
{
    public partial class UserPromptMenu : UserControl
    {
        public event Action Confirmed;

        public UserPromptMenu(string userName, string ipAddress)
        {
            InitializeComponent();

            NameText.Text = userName;
            IpText.Text = ipAddress;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Confirmed?.Invoke();
        }
    }
}
