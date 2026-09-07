using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI
{
    public partial class UserMenu : UserControl, INotifyPropertyChanged
    {
        public event Action<string, string> Submitted;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Ip { get; }

        private string userName = "";
        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
            }
        }

        public UserMenu(string ipAddress)
        {
            Ip = ipAddress;
            InitializeComponent();
            DataContext = this;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            UserName = "";
            InputBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Submitted?.Invoke(UserName, Ip);
        }
    }
}
