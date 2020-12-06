using System;
using System.Windows;
using System.Windows.Controls;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;
using MaterialDesignThemes.Wpf;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    /// Логика взаимодействия для TestUserControl.xaml
    /// </summary>
    public partial class ThemesUserControl : UserControl
    {
        private MainWindow _mainWindow;

        public ThemesUserControl(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            LoadWrapPanel();
        }

        public void LoadWrapPanel()
        {
            try
            {
                foreach (var theme in DB.GetThemes())
                {
                    Snackbar newSnack = new Snackbar()
                    {
                        IsActive = true,
                        Margin = new Thickness(15),
                        ActionButtonStyle = FindResource("MaterialDesignSnackbarActionMidButton") as Style,
                        ActionButtonPlacement = SnackbarActionButtonPlacementMode.Inline,
                        MessageQueue = new SnackbarMessageQueue(),
                        Message = new SnackbarMessage()
                        {
                            Content = theme.Theme_Name,
                            ActionContent = "SHOW TESTS",
                        }
                    };
                    newSnack.Message.ActionClick += Snack_OnClick;
                    ThemesWrapPanel.Children.Add(newSnack);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void Snack_OnClick(object sender, EventArgs e)
        {
            try
            {
                SnackbarMessage themeName = new SnackbarMessage();
                themeName = (SnackbarMessage)sender;
                TestsUserControl testsUserControl = new TestsUserControl(themeName.Content.ToString(), _mainWindow);
                using BlueFoxContext db = new BlueFoxContext();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }
    }
}
