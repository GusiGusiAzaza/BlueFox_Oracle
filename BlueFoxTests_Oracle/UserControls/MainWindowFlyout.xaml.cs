using System.Windows;
using System.Windows.Controls;
using BlueFoxTests_Oracle.Windows;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    /// Логика взаимодействия для MainWindowFlyout.xaml
    /// </summary>
    public partial class MainWindowFlyout : UserControl
    {
        public MainWindowFlyout()
        {
            InitializeComponent();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}