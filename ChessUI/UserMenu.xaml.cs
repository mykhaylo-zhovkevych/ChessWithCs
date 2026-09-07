using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI
{
    public partial class UserMenu : UserControl
    {
        private readonly string defaultHost;

        public event Action<string> Submitted;

        public UserMenu(string defaultHost)
        {
            InitializeComponent();

            this.defaultHost = defaultHost;
            InputBox.Text = defaultHost;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Text = defaultHost;
            InputBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Submitted?.Invoke(InputBox.Text.Trim());
        }
    }
}
