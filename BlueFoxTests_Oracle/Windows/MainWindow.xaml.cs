using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace BlueFoxTests_Oracle.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static Snackbar Snackbar;
        public static DialogHost Dialog;
        public static Grid Grid;
        public static TextBlock DialogText;
        public static User User;
        public static List<Test_Result> UserResults;
        public static int AdminId;

        public MainWindow()
        {
            DB.Initialize();
            InitializeComponent();
            User.User_Stats = DB.GetUserStats(User.User_Id);
            AdminId = DB.IsAdmin(1);
            Grid = MainGrid;
            Snackbar = MainSnackbar;
            Dialog = DialogHost;
            DialogText = DialogTxt;
        }

        public MainWindow(User user, int adminId)
        {
            if(adminId == 0) DB.OpenUserConnection();
            User = user;
            User.User_Stats = DB.GetUserStats(User.User_Id);
            InitializeComponent();
            UserResults = DB.GetUserTestResults(User.User_Id);
            AdminId = adminId;
            DialogText = DialogTxt;
            Grid = MainGrid;
            Dialog = DialogHost;
            Snackbar = MainSnackbar;
            if (AdminId == 0)
            {
                AdminTabItem.Content = "Only For Admins";
            }
        }

        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize/Restore
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    SystemCommands.MaximizeWindow(this);
                    break;
                case WindowState.Maximized:
                    SystemCommands.RestoreWindow(this);
                    break;
                case WindowState.Minimized:
                    SystemCommands.RestoreWindow(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            LeftFlyout.IsOpen = !LeftFlyout.IsOpen;
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            DB.OpenAdminConnection();
            Close();
        }

        public void EnableGrid(object sender, RoutedEventArgs e)
        {
            Grid.IsEnabled = true;
        }

        public static void ShowDialog(string text)
        {
            Dialog.IsOpen = true;
            Grid.IsEnabled = false;
            DialogText.Text = text;
        }
    }
}
