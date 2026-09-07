using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI
{
    public partial class UserPromptMenu : UserControl
    {
        public event Action Confirmed;

        public UserPromptMenu(string userName, string ipAddress, int port)
        {
            InitializeComponent();

            NameText.Text = userName;
            IpText.Text = ipAddress;
            PortText.Text = port.ToString();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Confirmed?.Invoke();
        }
    }
}
