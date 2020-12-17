using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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
        private const string _startTestMsg = "START TEST";
        private const string _viewResultMsg = "VIEW RESULT";
        public static WrapPanel _testsWrapPanel;
        public static bool TestIsGoing;
        public static Test OnGoingTest = new Test();
        private Test _currentTest;
        private Theme _currentTheme;
        private List<Test> _tests;
        private List<Theme> _themes;

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
                var themeName = ((SnackbarMessage) sender).Content.ToString();

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
                    var res = MainWindow.UserResults.LastOrDefault(res => res.Test_Id == test.Test_Id);
                    if (!test.Is_Enabled && res == null) continue;

                    var msg = res != null ? _viewResultMsg : _startTestMsg;
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
                            ActionContent = new TextBlock
                            {
                                Text = msg,
                                FontSize = 15,
                                Foreground = res == null ? Brushes.DeepSkyBlue : (res.Is_Passed ? Brushes.GreenYellow : Brushes.Red),
                            }
                        }
                    };
                    if(res == null) newSnack.Message.ActionClick += TestSnack_OnClick;
                    else newSnack.Message.ActionClick += ResultSnack_OnClick;

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
                MainWindow.ShowDialog("You have ongoing test! Go to the Solution Tab!");
                return;
            }

            try
            {
                var testName = (SnackbarMessage) sender;
                //_currentTest = DB.GetTestByName(testName.Content.ToString());
                _currentTest = _tests.Find(t => t.Test_Name == testName.Content.ToString());
                _currentTest.Questions_For_Tests = DB.GetQuestionsByTestId(_currentTest.Test_Id);
                if (_currentTest == null || !_currentTest.Questions_For_Tests.Any())
                {
                    MainWindow.ShowDialog("Sorry. There are no questions in this test yet");
                    return;
                }

                OnGoingTest = _currentTest;
                if(TestResults.ResultPanel != null)
                {
                    TestResults.ResultPanel.Visibility = Visibility.Collapsed;
                    TestResults.ResultPanel.Children.Clear();
                }
                TestSolution._resultsGrid.Visibility = Visibility.Hidden;
                TestSolution._resultsGrid.Children.Clear();
                TestSolution.StartButton.Content = $"Start \"{OnGoingTest.Test_Name}\" test";
                var str = _currentTest.Time_Limit_In_Minutes == 0
                    ? "You will have unlimited time to pass this test."
                    : $"You will have {OnGoingTest.Time_Limit_In_Minutes} minute(s) to get it done.";

                TestSolution.StartDescription.Text =
                    $"You're about to start \"{OnGoingTest.Test_Name}\" test. {str} " +
                    $"To pass this test successfully you need to get more than {OnGoingTest.Passing_Score}% " +
                    "of the maximum score. Choose wisely. Good luck.";
                TestSolution.StartPanel.Visibility = Visibility.Visible;
                MainWindow.ShowDialog($"You started test \"{OnGoingTest.Test_Name}\". Go to the Solution Tab");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void ResultSnack_OnClick(object sender, EventArgs e)
        {
            if (TestIsGoing)
            {
                MainWindow.ShowDialog("You have ongoing test! Go to the Solution Tab!");
                return;
            }
            try
            {
                var testName = (SnackbarMessage)sender;
                _currentTest = _tests.Find(t => t.Test_Name == testName.Content.ToString());
                _currentTest.Questions_For_Tests = DB.GetQuestionsByTestId(_currentTest.Test_Id);
                _currentTest.Questions_For_Tests.ForEach(q => q.Answers_For_Tests = DB.GetAnswersByQuestionId(q.Question_Id));
                var result = MainWindow.UserResults.LastOrDefault(res => res.Test_Id == _currentTest.Test_Id);
                result.UserAnswers = DB.GetUserAnswersByResultId(result.Result_Id);
                TestSolution._resultsGrid.Visibility = Visibility.Visible;
                TestSolution._resultsGrid.Children.Add(new TestResults(result, _currentTest));
                TestSolution._resultsGrid.Children.Add(new TextBlock
                {
                    FontSize = 30,
                    Foreground = Brushes.DeepSkyBlue,
                    Margin = new Thickness(50),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Text = $"{Math.Round(result.Score, 1)}% is correct"
                });
                MainWindow.ShowDialog("Check your result in the Solution Tab!");
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
            MainWindow.UserResults = DB.GetUserTestResults(MainWindow.User.User_Id);
            _testsWrapPanel.Children.Clear();
            _currentTheme = null;
        }
    }
}