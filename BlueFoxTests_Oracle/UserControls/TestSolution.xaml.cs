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
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;

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

        private Questions_For_Tests _currentQuestion;
        private Test _currentTest = new Test();
        private int _questionNumber;
        private int _questionsCount;
        private ICollection<Questions_For_Tests> _questionsInTest;
        private string _rightAnswerString;
        private Test_Result _testProgress;

        public TestSolution()
        {
            InitializeComponent();
            _rand = new Random();
            StartPanel = StartTestPanel;
            StartButton = StartTestButton;
            StartDescription = StartDescriptionTb;
        }

        private void StartTest(object sender, RoutedEventArgs e)
        {
            StartPanel.Visibility = Visibility.Collapsed;
            LoadTestData(TestsUserControl.OnGoingTest);
            SolvingTabGrid.Visibility = Visibility.Visible;
            ResultsGrid.Visibility = Visibility.Collapsed;
            ResultsGrid.Children.Clear();
        }

        public void LoadTestData(Test test)
        {
            _currentTest = test;
            _questionNumber = 0;
            _questionsCount = _currentTest.Questions_For_Tests.Count;
            _questionsInTest = _currentTest.Questions_For_Tests;

            _testProgress = new Test_Result
            {
                User_Id = MainWindow.User.User_Id,
                Test_Id = _currentTest.Test_Id,
                Try_Count = getTryCount(_currentTest.Test_Id),
                Start_Date = DateTime.Now,
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

                        List<Answers_For_Tests> answerss = DB.GetAnswersByQuestionId(_currentQuestion.Question_Id);
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
                    ResultsGrid.Children.Add(new TestResults(_testProgress, _currentTest));
                    ResultsGrid.Children.Add(new TextBlock()
                    {
                        FontSize = 30,
                        Foreground = Brushes.DeepSkyBlue,
                        Margin = new Thickness(50),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Text = $"{Math.Round(perc, 1)}% is correct"
                    });
                    TestsUserControl.TestIsGoing = false;
                    var isPassed = perc > 50;

                    if (isPassed) _testProgress.Is_Passed = true;


                    using BlueFoxContext db = new BlueFoxContext();
                    db.Test_Progress.Add(_testProgress);
                    db.SaveChanges();
                    var userst = db.User_Stats.FirstOrDefault(u => u.User_Id == MainWindow.User.User_Id) ?? new User_Stats() { User_Id = MainWindow.User.User_Id };
                    userst.Finished_Tests_Count += 1;
                    userst.Right_Answered += _testProgress.Right_Answers_Count;
                    userst.Total_Answered += _testProgress.Questions_Count;
                    if (isPassed) userst.Passed_Tests_Count += 1;
                    var userstat = db.User_Stats.FirstOrDefault(u => u.User_Id == MainWindow.User.User_Id);
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

        private int getTryCount(int testId)
        {
            var currentTestResults = MainWindow.UserResults.Where(res => res.Test_Id == testId);
            if (!currentTestResults.Any()) return 1;
            return currentTestResults.FirstOrDefault(res => res.Try_Count == currentTestResults.Max(r => r.Try_Count)).Try_Count + 1;
        }
    }
}
