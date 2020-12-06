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

        public Questions_For_Tests CurrentQuestion;
        public Test CurrentTest = new Test();
        public int QuestionNumber;
        public int QuestionsCount;
        public ICollection<Questions_For_Tests> QuestionsInTest;
        public string RightAnswerString;
        public Test_Result TestProgress;

        public TestSolution()
        {
            InitializeComponent();
            _rand = new Random();
        }

    public void LoadTestData(Test test)
        {
            CurrentTest = test;
            QuestionNumber = 0;
            QuestionsCount = CurrentTest.Questions_For_Tests.Count;
            QuestionsInTest = CurrentTest.Questions_For_Tests;

            TestProgress = new Test_Result
            {
                User_Id = MainWindow.User.User_Id,
                Test_Id = CurrentTest.Test_Id,
                Test_Date = DateTime.Now,
                Questions_Count = QuestionsInTest.Count(),
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
                if (QuestionNumber == 0)
                    TestName.Text = CurrentTest.Test_Name;
                foreach (var question in QuestionsInTest)
                {
                    byte j = 1;
                    if (i == QuestionNumber)
                    {
                        CurrentQuestion = question;
                        Question.Text = CurrentQuestion.Question;
                        using BlueFoxContext db = new BlueFoxContext();

                        List<Answers_For_Tests> answerss = db.Answers_For_Tests
                            .Where(a => a.Question_Id == CurrentQuestion.Question_Id)
                            .ToList();
                        Shuffle(answerss); // Assuming an extension method on List<T>

                        foreach (var answer in answerss)
                        {
                            if (answer.Is_Right) RightAnswerString = answer.Answer;
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
                if (QuestionNumber + 1 == QuestionsCount)
                {
                    double perc = 0;
                    if (TestProgress.Right_Answers_Count != null)
                    {
                        if (TestProgress.Questions_Count != null)
                        {
                            perc = ((double)TestProgress.Right_Answers_Count / (double)TestProgress.Questions_Count) * 100;
                        }
                    }

                    SolvingTabGrid.Visibility = Visibility.Collapsed;
                    ResultsGrid.Visibility = Visibility.Visible;
                    ResultsGrid.Children.Add(new TestResults(TestProgress, CurrentTest));
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

                    if (isPassed) TestProgress.Is_Passed = true;


                    using BlueFoxContext db = new BlueFoxContext();
                    db.Test_Progress.Add(TestProgress);
                    db.SaveChanges();
                    var userst = db.User_Stats.FirstOrDefault(u => u.User_Id == MainWindow.User.User_Id) ?? new User_Stats() { User_Id = MainWindow.User.User_Id };
                    userst.Finished_Tests_Count += 1;
                    userst.Right_Answered += TestProgress.Right_Answers_Count;
                    userst.Total_Answered += TestProgress.Questions_Count;
                    if (isPassed) userst.Right_Tests_Count += 1;
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
                            if (radio.Content.ToString() == RightAnswerString)
                            {
                                TestProgress.Right_Answers_Count++;
                                AnswerCorrectIcon.Foreground = Brushes.LimeGreen;
                            }
                            else AnswerCorrectIcon.Foreground = Brushes.DarkRed;
                            radio.IsChecked = false;
                        }
                    }
                    QuestionNumber++;
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
    }
}
