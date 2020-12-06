using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    ///     Логика взаимодействия для TestResults.xaml
    /// </summary>
    public partial class TestResults : UserControl
    {
        private Test_Result _results;
        private readonly Test _test;

        public TestResults(Test_Result results, Test test)
        {
            InitializeComponent();
            _results = results;
            _test = test;
            LoadResultsContent();
        }

        private void LoadResultsContent()
        {
            try
            {
                QuestionsGrid.Children.Clear();
                foreach (var question in _test.Questions_For_Tests)
                {
                    using var db = new BlueFoxContext();

                    var answers = db.Questions_For_Tests.FirstOrDefault(q => q.Question_Id == question.Question_Id)?.Answers_For_Tests.ToList();
                    TestSolution.Shuffle(answers);
                    var border = new Border
                    {
                        CornerRadius = new CornerRadius(20),
                        Margin = new Thickness(10, 20, 10, 20),
                        Background = new SolidColorBrush { Opacity = 0.3 }
                    };
                    var rowDefinition1 = new RowDefinition { Height = GridLength.Auto };
                    var rowDefinition2 = new RowDefinition { Height = GridLength.Auto };
                    var grid = new Grid { RowDefinitions = { rowDefinition1, rowDefinition2 } };
                    var qBorder = new Border
                    {
                        Margin = new Thickness(60, 0, 60, 0),
                        VerticalAlignment = VerticalAlignment.Top,
                        CornerRadius = new CornerRadius(10),
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.DeepSkyBlue
                    };

                    var qName = new TextBlock
                    {
                        Margin = new Thickness(5, 0, 0, 0),
                        TextWrapping = TextWrapping.Wrap,
                        Text = question.Question
                    };
                    qBorder.Child = qName;
                    var answersPanel = new StackPanel { Margin = new Thickness(60, 50, 60, 50) };
                    var answer1 = new RadioButton { Style = FindResource("MaterialDesignLightRadioButton") as Style, IsEnabled = false, Margin = new Thickness(2) };
                    var answer2 = new RadioButton { Style = FindResource("MaterialDesignLightRadioButton") as Style, IsEnabled = false, Margin = new Thickness(2) };
                    var answer3 = new RadioButton { Style = FindResource("MaterialDesignLightRadioButton") as Style, IsEnabled = false, Margin = new Thickness(2) };
                    var answer4 = new RadioButton { Style = FindResource("MaterialDesignLightRadioButton") as Style, IsEnabled = false, Margin = new Thickness(2) };
                    var aList = new List<RadioButton> { answer1, answer2, answer3, answer4 };
                    for (int i = 0; i < aList.Count; i++)
                    {
                        aList[i].Content = answers[i].Answer;
                        if (answers[i].Is_Right)
                        {
                            aList[i].Background = Brushes.Green;
                            aList[i].IsEnabled = true;
                            aList[i].BorderBrush = Brushes.GreenYellow;
                            aList[i].IsChecked = true;
                        }
                        else aList[i].Background = Brushes.Red;
                    }

                    foreach (var a in aList)
                        answersPanel.Children.Add(a);

                    grid.Children.Add(qBorder);
                    grid.Children.Add(answersPanel);
                    border.Child = grid;
                    QuestionsGrid.Children.Add(border);
                    QuestionsGrid.Visibility = Visibility.Visible;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }
    }
}