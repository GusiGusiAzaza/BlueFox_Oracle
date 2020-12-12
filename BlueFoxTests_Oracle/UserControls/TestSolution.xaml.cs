using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;
using MaterialDesignColors;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    /// Логика взаимодействия для TestSolution.xaml
    /// </summary>
    public partial class TestSolution : UserControl
    {
        private static Random _rand = new Random();
        public static StackPanel StartPanel;
        public static TextBlock StartDescription;
        public static Button StartButton;
        public static Grid _resultsGrid;

        private Questions_For_Tests _currentQuestion;
        private Test _currentTest = new Test();
        private int _questionNumber;
        private int _questionsCount;
        private List<Answers_For_Tests> _currentQuestionAnswers;
        private string _rightAnswerString;
        private Test_Result _testResult;
        private DispatcherTimer _timer;
        private TimeSpan _timeLeft;
        private readonly TimeSpan _sec1 = new TimeSpan(0, 0, 1);

        public TestSolution()
        {
            InitializeComponent();
            _resultsGrid = ResultsGrid;
            _rand = new Random();
            StartPanel = StartTestPanel;
            StartButton = StartTestButton;
            StartDescription = StartDescriptionTb;
        }

        private void StartTest(object sender, RoutedEventArgs e)
        {
            StartPanel.Visibility = Visibility.Collapsed;
            TestsUserControl._testsWrapPanel.Children.Clear();
            LoadTestData(TestsUserControl.OnGoingTest);
            TestsUserControl.TestIsGoing = true;
            SolvingTabGrid.Visibility = Visibility.Visible;
        }

        public void LoadTestData(Test test)
        {
            _currentTest = test;
            _questionNumber = 0;
            _currentTest.Questions_For_Tests = DB.GetQuestionsByTestId(_currentTest.Test_Id);
            _questionsCount = _currentTest.Questions_For_Tests.Count;

            _testResult = new Test_Result
            {
                User_Id = MainWindow.User.User_Id,
                Test_Id = _currentTest.Test_Id,
                Try_Count = getTryCount(_currentTest.Test_Id),
                Start_Date = DateTime.Now,
                Questions_Count = _questionsCount,
                Right_Answers_Count = 0,
                Is_Passed = false
            };

            _testResult.Result_Id = DB.InitTestResult(_testResult);

            if (_currentTest.Time_Limit_In_Minutes > 0)
            {
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _timeLeft = TimeSpan.FromMinutes(_currentTest.Time_Limit_In_Minutes);
                _timer.Tick += timer_Tick;
                tbTime.Text = _timeLeft.ToString();
                tbTime.Visibility = Visibility.Visible;
                _timer.Start();
            }
            tbQuestionCounter.Text = $"{_questionNumber + 1}/{_questionsCount}";
            tbQuestionCounter.Visibility = Visibility.Visible;
            _testResult.UserAnswers = new List<User_Answers>();
            LoadTestsContent();
        }

        private void LoadTestsContent()
        {
            try
            {
                var i = 0;

                SolvingTabGrid.Visibility = Visibility.Visible;
                if (_questionNumber == 0)
                {
                    TestName.Text = _currentTest.Test_Name;
                    foreach (var question in _currentTest.Questions_For_Tests) 
                        question.Answers_For_Tests = DB.GetAnswersByQuestionId(question.Question_Id);
                }
                tbQuestionCounter.Text = $"{_questionNumber + 1}/{_questionsCount}";
                foreach (var question in _currentTest.Questions_For_Tests)
                {
                    byte j = 1;
                    
                    if (i == _questionNumber)
                    {
                        _currentQuestion = question;
                        Question.Text = _currentQuestion.Question;

                        _currentQuestionAnswers = question.Answers_For_Tests;
                        Shuffle(_currentQuestionAnswers); // Assuming an extension method on List<T>

                        foreach (var answer in _currentQuestionAnswers)
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

        private void timer_Tick(object sender, EventArgs e)
        {
            if (_timeLeft.TotalSeconds > 0)
            {
                _timeLeft = _timeLeft.Subtract(_sec1);
                tbTime.Text = _timeLeft.ToString();
            }
            else
            {
                _timer.Stop();
                EndTest();
                MainWindow.ShowDialog("You didn't finish in time!");
            }
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            var isAnswered = false;
            try
            {
                if (AnswersPanel.Children.Cast<RadioButton>().Any(radio => radio.IsChecked == true)) isAnswered = true;

                if (isAnswered)
                {
                    foreach (RadioButton radio in AnswersPanel.Children)
                    {
                        if (radio.IsChecked == true)
                        {
                            if (radio.Content.ToString() == _rightAnswerString)
                            {
                                _testResult.Right_Answers_Count++;
                                AnswerCorrectIcon.Foreground = Brushes.LimeGreen;
                            }
                            else AnswerCorrectIcon.Foreground = Brushes.DarkRed;
                            _testResult.UserAnswers.Add(new User_Answers
                            {
                                Result_Id = _testResult.Result_Id,
                                Question_Id = _currentQuestion.Question_Id,
                                User_Answer = _currentQuestionAnswers.FirstOrDefault(a => a.Answer == radio.Content.ToString()).Answer_Id
                            });
                            radio.IsChecked = false;
                        }
                    }
                    _questionNumber++;
                    tbQuestionCounter.Text = $"{_questionNumber + 1}/{_questionsCount}";
                    LoadTestsContent();
                }
                else
                {
                    MessageBox.Show("Choose the right answer", "Warning");
                }

                if (_questionNumber == _questionsCount)
                {
                    EndTest();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void EndTest()
        {
            _testResult.End_Date = DateTime.Now;
            TestsUserControl.TestIsGoing = false;

            if (_testResult.Right_Answers_Count != null)
            {
                if (_testResult.Questions_Count != null)
                {
                    _testResult.Score = Math.Round(((double)_testResult.Right_Answers_Count / (double)_testResult.Questions_Count * 100), 1);
                }
            }

            _testResult.Is_Passed = _testResult.Score >= _currentTest.Passing_Score;
            _testResult.UserAnswers.ForEach(DB.AddUserAnswer);
            DB.UpdateTestResultOnFinish(_testResult);

            SolvingTabGrid.Visibility = Visibility.Collapsed;
            _resultsGrid.Visibility = Visibility.Visible;
            _resultsGrid.Children.Add(new TestResults(_testResult, _currentTest));
            _resultsGrid.Children.Add(new TextBlock
            {
                FontSize = 30,
                Foreground = Brushes.DeepSkyBlue,
                Margin = new Thickness(50),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Text = $"{Math.Round(_testResult.Score, 1)}% is correct"
            });
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

        private int getTryCount(int testId)
        {
            var currentTestResults = MainWindow.UserResults.Where(res => res.Test_Id == testId);
            if (!currentTestResults.Any()) return 1;
            return currentTestResults.LastOrDefault(res => res.Try_Count == currentTestResults.Max(r => r.Try_Count)).Try_Count + 1;
        }
    }
}
