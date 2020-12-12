using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using Oracle.ManagedDataAccess.Client;

namespace BlueFoxTests_Oracle.Windows
{
    /// <summary>
    /// Логика взаимодействия для SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
            UsernameTextBox.LostKeyboardFocus += UsernameTextBox_LostKeyboardFocus;
            PasswordBox.LostKeyboardFocus += PasswordBox_LostKeyboardFocus;
            ConfirmPasswordBox.LostKeyboardFocus += ConfirmPasswordBox_LostKeyboardFocus;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void SignInInsteadButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void UsernameTextBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Regex.IsMatch(UsernameTextBox.Text, "^[a-zA-Z][a-zA-Z0-9]{2,19}$"))
                {
                    SignUpUserNameWarningTextBlock.Text = (string)TryFindResource("signup_InvalidUsernameWarning");
                    SignUpUserNameWarningTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                if (DB.UserExistsByUsername(UsernameTextBox.Text))
                {
                    SignUpUserNameWarningTextBlock.Text = (string)TryFindResource("signup_UserExistsWarning");
                    SignUpUserNameWarningTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    SignUpUserNameWarningTextBlock.Visibility = Visibility.Collapsed;
                }

                CheckData();
            }
            catch (Exception exception)
            {
                DB.Conn.Close();
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void PasswordBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            SignUpLowPasswordWarningTextBlock.Visibility = !Regex.IsMatch(PasswordBox.Password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{6,25}$")
                ? Visibility.Visible : Visibility.Collapsed;
            ConfirmPasswordBox_LostKeyboardFocus(sender, e);
            CheckData();
        }

        private void ConfirmPasswordBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            SignUpDontMatchWarningTextBlock.Visibility = PasswordBox.Password != ConfirmPasswordBox.Password && PasswordBox.Password.Length >= 6
                ? Visibility.Visible : Visibility.Collapsed;
            CheckData();
        }

        private void CheckData()
        {
            if (SignUpLowPasswordWarningTextBlock.Visibility == Visibility.Visible ||
                SignUpDontMatchWarningTextBlock.Visibility == Visibility.Visible ||
                SignUpUserNameWarningTextBlock.Visibility == Visibility.Visible)
                SignUpButton.IsEnabled = false;
            else SignUpButton.IsEnabled = true;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var conn = new OracleConnection(Config.AdminConnectionString);
                await conn.OpenAsync();
                MD5 md5 = new MD5CryptoServiceProvider();
                User newUser = new User
                {
                    Username = UsernameTextBox.Text,
                    Password_Hash = Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(PasswordBox.Password)))
                        .Substring(0, 15)
                };

                DB.AddUser(newUser);
                //DB.InitUserInfoAndStats(DB.GetUserByUsername(newUser.Username).User_Id);

                Logger.Log.Info($"Successfully registered new user \"{newUser.Username}\"(id: {newUser.User_Id}) to database");

                LoginWindow loginWindow = new LoginWindow
                {
                    LoginWarningLabel =
                    {
                        Text = (string) TryFindResource("login_RegisterSucceed"),
                        Visibility = Visibility.Visible,
                        Foreground = new SolidColorBrush(Color.FromRgb(0,180  ,0))
                    }
                };
                loginWindow.Show();
                Close();
            }
            catch (Exception exception)
            {
                DB.Conn.Close();
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }
    }
}
