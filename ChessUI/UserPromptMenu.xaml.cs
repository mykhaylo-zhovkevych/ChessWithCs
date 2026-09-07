using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI
{
    public partial class UserPromptMenu : UserControl
    {
        public event Action HostSelected;
        public event Action JoinSelected;

        public UserPromptMenu(string userName, string ipAddress, int port)
        {
            InitializeComponent();

            NameText.Text = userName;
            IpText.Text = ipAddress;
            PortText.Text = port.ToString();
        }

        private void Host_Click(object sender, RoutedEventArgs e)
        {
            HostSelected?.Invoke();
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            JoinSelected?.Invoke();
        }
    }
}
