using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;
using MaterialDesignThemes.Wpf;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    /// Логика взаимодействия для TestsUserControl.xaml
    /// </summary>
    public partial class TestsUserControl : UserControl
    {
        private MainWindow _mainWindow;
        private static WrapPanel _wrapPanel;
        private Themes_For_Tests currentTheme;
        public static bool _testIsGoing = false;


        public TestsUserControl()
        {
            InitializeComponent();
            _wrapPanel = WrapPanel;
        }

        public TestsUserControl(string themeName, MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            LoadTestsContent(themeName);
        }

        private void LoadTestsContent(string themeName)
        {
            try
            {
                using BlueFoxContext db = new BlueFoxContext();
                currentTheme = db.Themes_For_Tests.FirstOrDefault(t => t.Theme_Name == themeName);
                if (currentTheme == null)
                {
                    MessageBox.Show($"No tests were founded for \"{themeName}\" theme");
                    return;
                }

                _wrapPanel.Children.Clear();
                foreach (var test in db.Tests)
                {
                    if (test.Theme_Id == currentTheme.Theme_Id)
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
                                Content = test.Test_Name,
                                ActionContent = "START TEST",
                            }
                        };
                        newSnack.Message.ActionClick += Snack_OnClick;
                        _wrapPanel.Children.Add(newSnack);
                    }
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
            if (_testIsGoing)
            {
                MessageBox.Show("You have ongoing test! Go to the Soution Tab!", "Can't start another test");
                return;
            }
            try
            {
                var testName = (SnackbarMessage)sender;
                using BlueFoxContext db = new BlueFoxContext();
                Test test = db.Tests.FirstOrDefault(t => t.Test_Name == testName.Content.ToString());
                if (test == null || test.Questions_For_Tests.Count == 0)
                {
                    MessageBox.Show("No Questions In this Test");
                    return;
                }
                _mainWindow.LoadTestData(test);
                _mainWindow.SolvingTabGrid.Visibility = Visibility.Visible;
                _mainWindow.ResultsGrid.Visibility = Visibility.Collapsed;
                _mainWindow.ResultsGrid.Children.Clear();
                MessageBox.Show($"You started test \"{test.Test_Name}\". Go to the Solution Tab", "Test Started");
                _testIsGoing = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }
    }
}
