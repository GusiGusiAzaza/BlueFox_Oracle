using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;
using MaterialDesignThemes.Wpf;
using Theme = BlueFoxTests_Oracle.Models.Theme;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    ///     Логика взаимодействия для TestUserControl.xaml
    /// </summary>
    public partial class TestsUserControl : UserControl
    {
        private static WrapPanel _testsWrapPanel;
        public static bool TestIsGoing;
        public static Test OnGoingTest = new Test();
        private readonly TestSolution _testSolution;
        private List<Test> _tests;
        private List<Theme> _themes;
        private Theme _currentTheme;
        private Test _currentTest;

        public TestsUserControl()
        {
            InitializeComponent();
            LoadThemes();
            _testsWrapPanel = TestsWrapPanel;
        }

        private void LoadThemes()
        {
            try
            {
                ThemesWrapPanel.Children.Clear();
                _themes = DB.GetThemes();
                foreach (var theme in _themes)
                {
                    var newSnack = new Snackbar
                    {
                        IsActive = true,
                        Margin = new Thickness(10),
                        ActionButtonStyle = FindResource("MaterialDesignSnackbarActionMidButton") as Style,
                        ActionButtonPlacement = SnackbarActionButtonPlacementMode.Inline,
                        MessageQueue = new SnackbarMessageQueue(),
                        Message = new SnackbarMessage
                        {
                            Content = theme.Theme_Name,
                            ActionContent = "SHOW TESTS"
                        }
                    };
                    newSnack.Message.ActionClick += ThemeSnack_OnClick;
                    ThemesWrapPanel.Children.Add(newSnack);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void ThemeSnack_OnClick(object sender, EventArgs e)
        {
            try
            {
                var themeName = ((SnackbarMessage)sender).Content.ToString();

                if (_currentTheme == null || _currentTheme.Theme_Name != themeName)
                    LoadTests(themeName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void LoadTests(string themeName)
        {
            try
            {
                var theme = _themes.FirstOrDefault(th => th.Theme_Name == themeName);
                if (theme == null)
                {
                    MainWindow.ShowDialog("Please try again later");
                    return;
                }
                _tests = DB.GetTestsByThemeId(theme.Theme_Id);

                if (!_tests.Any())
                {
                    MainWindow.ShowDialog($"No tests were found for \"{themeName}\" theme");
                    return;
                }

                _currentTheme = theme;

                _testsWrapPanel.Children.Clear();
                TbTests.Text = $"{themeName} tests";
                TbTests.Visibility = Visibility.Visible;
                foreach (var test in _tests)
                {
                    var newSnack = new Snackbar
                    {
                        IsActive = true,
                        Margin = new Thickness(10),
                        ActionButtonStyle = FindResource("MaterialDesignSnackbarActionMidButton") as Style,
                        ActionButtonPlacement = SnackbarActionButtonPlacementMode.Inline,
                        MessageQueue = new SnackbarMessageQueue(),
                        Message = new SnackbarMessage
                        {
                            Content = test.Test_Name,
                            ActionContent = "START TEST"
                        }
                    };
                    newSnack.Message.ActionClick += TestSnack_OnClick;
                    _testsWrapPanel.Children.Add(newSnack);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void TestSnack_OnClick(object sender, EventArgs e)
        {
            if (TestIsGoing)
            {
                MessageBox.Show("You have ongoing test! Go to the Solution Tab!", "Can't start another test");
                return;
            }

            try
            {
                var testName = (SnackbarMessage) sender;
                _currentTest = DB.GetTestByName(testName.Content.ToString());
                _currentTest.Questions_For_Tests = DB.GetQuestionsByTestId(_currentTest.Test_Id);
                if (_currentTest == null || !_currentTest.Questions_For_Tests.Any())
                {
                    MainWindow.ShowDialog("Sorry. There are no questions in this test yet");
                    return;
                }

                OnGoingTest = _currentTest;
                TestSolution.StartButton.Content = $"Start \"{OnGoingTest.Test_Name}\" test";
                TestSolution.StartDescription.Text = $"You're about to start \"{OnGoingTest.Test_Name}\" test. You will have {OnGoingTest.Time_Limit_In_Minutes} " +
                                                     $"minutes to get it done. To pass this test successfully you need to get more than {OnGoingTest.Passing_Score}% " +
                                                     "of the maximum score. Choose wisely. Good luck.";
                TestSolution.StartPanel.Visibility = Visibility.Visible;
                MainWindow.ShowDialog($"You started test \"{OnGoingTest.Test_Name}\". Go to the Solution Tab");
                TestIsGoing = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void ReloadThemes(object sender, RoutedEventArgs e)
        {
            LoadThemes();
        }
    }
}