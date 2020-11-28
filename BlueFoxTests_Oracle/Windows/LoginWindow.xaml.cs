using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
    ///     Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();
        public LoginWindow()
        {
            InitializeComponent();
            UsernameTextBox.LostKeyboardFocus += UsernameTextBox_LostKeyboardFocus;
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

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            Close();
        }

        private void UsernameTextBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(UsernameTextBox.Text, "^[a-zA-Z][a-zA-Z0-9]{2,19}$"))
            {
                SignInInvalidUsernameWarning.Visibility = Visibility.Visible;
                LoginWarningLabel.Visibility = Visibility.Collapsed;
                LoginWarningIcon.Visibility = Visibility.Collapsed;
                SignInButton.IsEnabled = false;
                return;
            }

            SignInButton.IsEnabled = true;
            SignInInvalidUsernameWarning.Visibility = Visibility.Collapsed;
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(UsernameTextBox.Text, "^[a-zA-Z][a-zA-Z0-9]{2,19}$"))
            {
                SignInInvalidUsernameWarning.Visibility = Visibility.Visible;
                return;
            }

            SignInInvalidUsernameWarning.Visibility = Visibility.Collapsed;

            try
            {
                AllocConsole();
                using var conn = new OracleConnection(Config.ConnectionString);
                User user = null;
                bool isAdmin = false;
                OracleCommand getUsersCmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "GET_USERS",
                    CommandType = CommandType.StoredProcedure
                };

                OracleCommand userIsAdmin = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "USER_IS_ADMIN",
                    CommandType = CommandType.StoredProcedure
                };

                MD5 md5 = new MD5CryptoServiceProvider();
                var hash = Convert
                    .ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(PasswordBox.Password)))
                    .Substring(0, 15);

                conn.Open();
                var reader = getUsersCmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                foreach (DataRow row in dt.Rows)
                {
                    if (row["username"].ToString() == UsernameTextBox.Text && row["password_hash"].ToString() == hash)
                    {
                        user = new User
                        {
                            UserId = int.Parse(row["user_id"].ToString()),
                            Username = row["username"].ToString(),
                            PasswordHash = row["password_hash"].ToString()
                        };
                        userIsAdmin.Parameters.Add("return_value", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
                        userIsAdmin.Parameters.Add("u_id", OracleDbType.Decimal).Value = user.UserId;
                        userIsAdmin.ExecuteNonQuery();
                        isAdmin = userIsAdmin.Parameters["return_value"].Value.ToString() == "1";
                        break;
                    }
                }

                if (user != null)
                {
                    LoginWarningLabel.Visibility = Visibility.Collapsed;
                    LoginWarningIcon.Visibility = Visibility.Collapsed;

                    var mainWindow = new MainWindow(user, isAdmin);
                    Logger.Log.Info($"User \"{user.Username}\"(id: {user.UserId}) successfully logged in");
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    LoginWarningLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    LoginWarningLabel.Text = (string)TryFindResource("login_WrongPasswordWarning");
                    LoginWarningLabel.Visibility = Visibility.Visible;
                    LoginWarningIcon.Visibility = Visibility.Visible;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void CantSignInButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWarningLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            LoginWarningLabel.Text = (string)TryFindResource("login_ForgotPasswordWarning");
            LoginWarningLabel.Visibility = Visibility.Visible;
            LoginWarningIcon.Visibility = Visibility.Visible;
        }
    }
}