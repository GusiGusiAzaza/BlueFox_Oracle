using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using MahApps.Metro.Controls;

namespace BlueFoxTests_Oracle.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static User User;
        private static Random _rand;
        public static bool IsAdmin;

        public MainWindow()
        {
            InitializeComponent();
            ThemesTab.Children.Add(new UserControls.ThemesUserControl(this));
        }

        public MainWindow(User user, bool isAdmin)
        {
            User = user;
            IsAdmin = isAdmin;
            _rand = new Random();
            InitializeComponent();

            if (!isAdmin)
            {
                AdminTabItem.Content = "Only For Admins";
            }
            ThemesTab.Children.Add(new UserControls.ThemesUserControl(this));
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
            Close();
        }



        public Questions_For_Tests _currentQuestion;

        public Test _currentTest = new Test();
        public int _questionNumber;
        public int _questionsCount;
        public ICollection<Questions_For_Tests> _questionsInTest;
        public string _rightAnswerString;
        public Test_Progress _testProgress;

        public void LoadTestData(Test test)
        {
            _currentTest = test;
            _questionNumber = 0;
            _questionsCount = _currentTest.Questions_For_Tests.Count;
            _questionsInTest = _currentTest.Questions_For_Tests;

            _testProgress = new Test_Progress
            {
                User_Id = User.User_Id,
                Test_Id = _currentTest.Test_Id,
                Test_Date = DateTime.Now,
                Questions_Count = _questionsInTest.Count(),
                Right_Answers_Count = 0,
                Is_Passed = false
            };
            LoadTestsContent();
        }

        private void LoadTestsContent()
        {
            try
            {
                var i = 0;
                SolvingTabGrid.Visibility = Visibility.Visible;
                if (_questionNumber == 0)
                    TestName.Text = _currentTest.Test_Name;
                foreach (var question in _questionsInTest)
                {
                    byte j = 1;
                    if (i == _questionNumber)
                    {
                        _currentQuestion = question;
                        Question.Text = _currentQuestion.Question;
                        using BlueFoxContext db = new BlueFoxContext();

                        List<Answers_For_Tests> answerss = db.Answers_For_Tests
                            .Where(a => a.Question_Id == _currentQuestion.Question_Id)
                            .ToList();
                        Shuffle(answerss); // Assuming an extension method on List<T>

                        foreach (var answer in answerss)
                        {
                            if (answer.Is_Right) _rightAnswerString = answer.Answer;
                            switch (j)
                            {
                                case 1:
                                    {
                                        Answer1.Content = answer.Answer;
                                        break;
                                    }
                                case 2:
                                    {
                                        Answer2.Content = answer.Answer;

                                        break;
                                    }
                                case 3:
                                    {
                                        Answer3.Content = answer.Answer;
                                        break;
                                    }
                                case 4:
                                    {
                                        Answer4.Content = answer.Answer;
                                        break;
                                    }
                                case 5: return;
                            }

                            j++;
                        }

                        return;
                    }

                    i++;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            var isAnswered = false;
            try
            {
                if (_questionNumber + 1 == _questionsCount)
                {
                    double perc = 0;
                    if (_testProgress.Right_Answers_Count != null)
                    {
                        if (_testProgress.Questions_Count != null)
                        {
                            perc = ((double)_testProgress.Right_Answers_Count / (double)_testProgress.Questions_Count) * 100;
                        }
                    }

                    SolvingTabGrid.Visibility = Visibility.Collapsed;
                    ResultsGrid.Visibility = Visibility.Visible;
                    ResultsGrid.Children.Add(new UserControls.TestResults(_testProgress, _currentTest));
                    ResultsGrid.Children.Add(new TextBlock()
                    {
                        FontSize = 30,
                        Foreground = Brushes.DeepSkyBlue,
                        Margin = new Thickness(50),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Text = $"{Math.Round(perc, 1)}% is correct"
                    });
                    UserControls.TestsUserControl._testIsGoing = false;
                    var isPassed = perc > 50;

                    if (isPassed) _testProgress.Is_Passed = true;


                    using BlueFoxContext db = new BlueFoxContext();
                    db.Test_Progress.Add(_testProgress);
                    db.SaveChanges();
                    var userst = db.User_Stats.FirstOrDefault(u => u.User_Id == User.User_Id) ?? new User_Stats() { User_Id = User.User_Id };
                    userst.Finished_Tests_Count += 1;
                    userst.Right_Answered += _testProgress.Right_Answers_Count;
                    userst.Total_Answered += _testProgress.Questions_Count;
                    if (isPassed) userst.Right_Tests_Count += 1;
                    var userstat = db.User_Stats.FirstOrDefault(u => u.User_Id == User.User_Id);
                    userstat = userst;
                    db.SaveChanges();
                    return;
                }

                if (AnswersPanel.Children.Cast<RadioButton>().Any(radio => radio.IsChecked == true)) isAnswered = true;

                if (isAnswered)
                {
                    foreach (RadioButton radio in AnswersPanel.Children)
                    {
                        if (radio.IsChecked == true)
                        {
                            if (radio.Content.ToString() == _rightAnswerString)
                            {
                                _testProgress.Right_Answers_Count++;
                                AnswerCorrectIcon.Foreground = Brushes.LimeGreen;
                            }
                            else AnswerCorrectIcon.Foreground = Brushes.DarkRed;
                            radio.IsChecked = false;
                        }
                    }
                    _questionNumber++;
                    LoadTestsContent();
                }
                else
                {
                    MessageBox.Show("Choose the right answer", "Warning");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        public static void Shuffle<T>(IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = _rand.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void AddUri_Click(object sender, EventArgs e)
        {
            bool result = Uri.TryCreate(url.Text, UriKind.Absolute, out var uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
            {
                using BlueFoxContext db = new BlueFoxContext();
                HomeWebPage page = new HomeWebPage() { WebPage_URL = url.Text };
                db.HomeWebPages.Add(page);
                db.SaveChanges();
                MessageBox.Show("Uri Saved", "Info");
            }
            else
            {
                MessageBox.Show("Incorrect URI", "Warning");
            }
        }
    }
}
