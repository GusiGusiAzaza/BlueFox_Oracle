﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;
using static System.String;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    ///     Логика взаимодействия для AdminCreateTestControl.xaml
    /// </summary>
    public partial class AdminCreateTestControl : UserControl
    {
        private readonly Questions_For_Tests _questionInDb = new Questions_For_Tests();

        private readonly Test _testInBd = new Test();
        private List<Theme> _themes = new List<Theme>();

        private Theme _themeInBd = new Theme();
        private bool _canSaveTest;
        private List<Answers_For_Tests> _listAnswerInDb;
        private List<Questions_For_Tests> _questions = new List<Questions_For_Tests>();
        private List<Test> _tests = new List<Test>();
        private bool _dontSelectEventQuestion;
        private bool _dontSelectEventTest;

        private int _numberQuestion = 1;

        private Questions_For_Tests _selectedQuestion = new Questions_For_Tests();

        private string _selectedQuestionn = Empty;
        private string _selectedTest = Empty;
        private Theme _selectedTheme = new Theme();
        private Test _selTest = new Test();

        public AdminCreateTestControl()
        {
            InitializeComponent();
            listThemes.SelectionChanged += NewTheme_Selected;
            listTests.SelectionChanged += NewTest_Selected;
            SelectedTestQuestions.SelectionChanged += SelectedTestQuestions_SelectionChanged;
            CurrentlyAddedQuestions.SelectionChanged += NewQuestion_Selected;
            try
            {
                var themes = DB.GetThemes();

                if (themes.Any())
                    foreach (var themee in themes)
                    {
                        _themes.Add(themee);
                        var theme = new ListViewItem {Content = themee.Theme_Name};
                        listThemes.Items.Add(theme);
                    }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Logger.Log.Error(exception);
            }
        }

        private void btnAddTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsNullOrEmpty(txbTheme.Text))
                {
                    MainWindow.ShowDialog("Enter Theme Name");
                }
                else if (DB.GetThemes().FirstOrDefault(t => string.Equals(t.Theme_Name.Trim(), txbTheme.Text.Trim(),
                    StringComparison.CurrentCultureIgnoreCase)) == null)
                {
                    var newTheme = new Theme {Theme_Name = txbTheme.Text};
                    DB.AddTheme(newTheme);
                    _themes = DB.GetThemes();
                    listThemes.Items.Add(new ListViewItem {Content = newTheme.Theme_Name});
                    MainWindow.Snackbar.MessageQueue.Enqueue("Added!");
                }
                else
                {
                    MainWindow.ShowDialog("Theme Exists");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void NewTheme_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                _dontSelectEventTest = true;
                _dontSelectEventQuestion = true;
                listTests.Items.Clear();
                SelectedTestQuestions.Items.Clear();

                var item = (ListViewItem) listThemes.SelectedItems[0];
                _selectedTheme = _themes.First(t => t.Theme_Name == item.Content.ToString());
                _tests = DB.GetTestsByThemeId(_selectedTheme.Theme_Id);
                foreach (var test in _tests)
                {
                    var testsViewItem = test.Is_Enabled ? new ListViewItem {Content = test.Test_Name, Background = Brushes.ForestGreen, Foreground = Brushes.White} 
                        : new ListViewItem { Content = test.Test_Name, Background = Brushes.DarkRed, Foreground = Brushes.White };

                    listTests.Items.Add(testsViewItem);
                }

                _dontSelectEventTest = false;
                _dontSelectEventQuestion = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void btnEnable_Click(object sender, RoutedEventArgs e)
        {
            if (tbEnableTest.Text == "Enable test")
            {
                DB.EnableTest(_selTest.Test_Id, true);
                tbEnableTest.Text = "Disable test";
                btnEnableTest.Background = Brushes.DarkRed;
                NewTheme_Selected(sender, e);
            }
            else if (tbEnableTest.Text == "Disable test")
            {
                DB.EnableTest(_selTest.Test_Id, false);
                tbEnableTest.Text = "Enable test";
                btnEnableTest.Background = Brushes.ForestGreen;
                NewTheme_Selected(sender, e);
            }
        }

        private void NewTest_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_dontSelectEventTest)
                {
                    _dontSelectEventQuestion = true;
                    var item = (ListViewItem) listTests.SelectedItems[0];
                    _selectedTest = item.Content.ToString();
                    SelectedTestQuestions.Items.Clear();
                    AnswersPanel.IsEnabled = true;

                    var currentTest = _tests.FirstOrDefault(t => t.Test_Name == _selectedTest);
                    _selTest = currentTest;
                    if (currentTest == null) return;
                    if (currentTest.Is_Enabled)
                    {
                        tbEnableTest.Text = "Disable test";
                        btnEnableTest.Background = Brushes.DarkRed;
                    }
                    else
                    {
                        tbEnableTest.Text = "Enable test";
                        btnEnableTest.Background = Brushes.ForestGreen;
                    }
                    _questions = DB.GetQuestionsByTestId(currentTest.Test_Id);
                    foreach (var question in _questions)
                    {
                        var existingQuestion = new ListViewItem {Content = question.Question};
                        SelectedTestQuestions.Items.Add(existingQuestion);
                    }

                    _dontSelectEventQuestion = false;
                }
                else
                {
                    _dontSelectEventTest = false;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void NewQuestion_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_dontSelectEventQuestion)
                {
                    txbAnswer1.Text = Empty;
                    txbAnswer2.Text = Empty;
                    txbAnswer3.Text = Empty;
                    txbAnswer4.Text = Empty;
                    AnswersPanel.IsEnabled = true;
                    var item = (ListViewItem) CurrentlyAddedQuestions.SelectedItems[0];
                    _selectedQuestionn = item.Content.ToString();
                }
                else
                {
                    _dontSelectEventQuestion = false;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void SelectedTestQuestions_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_dontSelectEventQuestion)
                {
                    var item = (ListViewItem) SelectedTestQuestions.SelectedItems[0];
                    var selectedQuestionTemp = item.Content.ToString();
                    var currentQuestion = _questions.First(q => q.Question == selectedQuestionTemp);
                    var answerList = new List<Answers_For_Tests>();
                    var answersForSelected = DB.GetAnswersByQuestionId(currentQuestion.Question_Id);
                    AnswersPanel.IsEnabled = false;
                    foreach (var answer in answersForSelected)
                        if (answer.Is_Right) txbAnswer1.Text = answer.Answer;
                        else answerList.Add(answer);

                    txbAnswer2.Text = answerList[0].Answer;
                    txbAnswer3.Text = answerList[1].Answer;
                    txbAnswer4.Text = answerList[2].Answer;
                }
                else
                {
                    _dontSelectEventQuestion = false;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void btnAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!IsNullOrEmpty(txbQuestion.Text))
            {
                _selectedQuestionn = txbQuestion.Text;
                _questionInDb.Question = txbQuestion.Text;

                var newItem = new ListViewItem {Content = txbQuestion.Text};

                CurrentlyAddedQuestions.Items.Add(newItem);
                txbQuestion.Text = Empty;
            }
            else
            {
                MainWindow.ShowDialog("Enter Question");
            }
        }

        private void btnAddAnswer_Click(object sender, RoutedEventArgs e)
        {
            _listAnswerInDb = new List<Answers_For_Tests>();
            try
            {
                if (CurrentlyAddedQuestions.SelectedIndex != -1)
                {
                    if (txbAnswer1.Text != Empty && txbAnswer2.Text != Empty && txbAnswer3.Text != Empty &&
                        txbAnswer4.Text != Empty)
                    {
                        if (txbAnswer1.Text == txbAnswer2.Text || txbAnswer1.Text == txbAnswer3.Text ||
                            txbAnswer1.Text == txbAnswer4.Text ||
                            txbAnswer2.Text == txbAnswer3.Text || txbAnswer2.Text == txbAnswer4.Text ||
                            txbAnswer3.Text == txbAnswer4.Text)
                        {
                            MainWindow.ShowDialog("Don't repeat the answer!");
                        }
                        else
                        {
                            for (var i = 0; i < 4; i++)
                            {
                                var answer = new Answers_For_Tests();
                                switch (i)
                                {
                                    case 0:
                                        answer.Answer = txbAnswer1.Text;
                                        _listAnswerInDb.Add(answer);
                                        break;
                                    case 1:
                                        answer.Answer = txbAnswer2.Text;
                                        _listAnswerInDb.Add(answer);
                                        break;
                                    case 2:
                                        answer.Answer = txbAnswer3.Text;
                                        _listAnswerInDb.Add(answer);
                                        break;
                                    case 3:
                                        answer.Answer = txbAnswer4.Text;
                                        _listAnswerInDb.Add(answer);
                                        break;
                                }
                            }

                            txbAnswer1.Text = Empty;
                            txbAnswer2.Text = Empty;
                            txbAnswer3.Text = Empty;
                            txbAnswer4.Text = Empty;
                            MainWindow.ShowDialog("Now you can save!");
                            _canSaveTest = true;
                        }
                    }
                    else
                    {
                        MainWindow.ShowDialog("Enter all answers!");
                    }
                }
                else
                {
                    MainWindow.ShowDialog("Choose a question for answers!");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void btnAddTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsNullOrEmpty(txbTest.Text))
                {
                    var flagTest = _tests.Any(test => string.Equals(txbTest.Text, test.Test_Name, StringComparison.CurrentCultureIgnoreCase));

                    if (!flagTest)
                    {
                        _selectedTest = txbTest.Text;

                        var newItem = new ListViewItem {Content = txbTest.Text};
                        _testInBd.Test_Name = txbTest.Text;
                        _testInBd.Admin_Id = MainWindow.AdminId;
                        _testInBd.Time_Limit_In_Minutes = int.TryParse(txbTimeLimit.Text, out var num) ? num : 0;
                        _testInBd.Passing_Score = int.TryParse(txbPassScore.Text, out num) ? num : 0;
                        listTests.Items.Add(newItem);
                        txbTest.Text = Empty;
                        txbPassScore.Text = Empty;
                        txbTimeLimit.Text = Empty;
                    }
                    else
                    {
                        MainWindow.ShowDialog("Test already exists!");
                    }
                }
                else
                {
                    MainWindow.ShowDialog("Enter test name!");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTheme != null && _testInBd != null && _questionInDb != null && _listAnswerInDb != null)
                {
                    if (IsNullOrEmpty(_themeInBd.Theme_Name) || IsNullOrEmpty(_testInBd.Test_Name))
                    {
                        _themeInBd.Theme_Name = _selectedTheme.Theme_Name;
                        _testInBd.Test_Name = _selectedTest;
                    }

                    //check if we can save
                    var flagSave = false;
                    var countAnswer = 0;

                    if (!IsNullOrEmpty(_themeInBd.Theme_Name) && !IsNullOrEmpty(_testInBd.Test_Name) &&
                        !IsNullOrEmpty(_questionInDb.Question) && _listAnswerInDb.Count == 4)
                    {
                        foreach (var t in _listAnswerInDb)
                            if (!IsNullOrEmpty(t.Answer.Trim()))
                                countAnswer++;

                        if (countAnswer == 4) flagSave = true;
                    }
                    else
                    {
                        MainWindow.ShowDialog("Enter all data!");
                        return;
                    }

                    //load to DB
                    if (flagSave)
                        try
                        {
                            var transaction = DB.Conn.BeginTransaction(IsolationLevel.ReadCommitted);
                            try
                            {
                                var theme = new Theme();
                                var flagTheme = false;

                                foreach (var teme in DB.GetThemes())
                                    if (_selectedTheme.Theme_Name == teme.Theme_Name)
                                    {
                                        flagTheme = true;
                                        theme.Theme_Id = teme.Theme_Id;
                                        theme.Theme_Name = teme.Theme_Name;
                                        break;
                                    }

                                if (flagTheme)
                                {
                                    MainWindow.Snackbar.MessageQueue.Enqueue("Adding test to selected theme...");
                                }
                                else
                                {
                                    MainWindow.ShowDialog("Choose a theme!");
                                    return;
                                }

                                var tests = DB.GetTestsByThemeId(_selectedTheme.Theme_Id);
                                var test = new Test();
                                var flagTest = false;

                                if (tests.Any())
                                    foreach (var teste in tests)
                                        if (_selectedTest == teste.Test_Name)
                                        {
                                            flagTest = true;
                                            test.Test_Id = teste.Test_Id;
                                            test.Admin_Id = teste.Admin_Id;
                                            test.Test_Name = teste.Test_Name;
                                            test.Theme_Id = teste.Theme_Id;
                                            break;
                                        }

                                if (!flagTest)
                                {
                                    var newTest = new Test
                                    {
                                        Test_Name = _testInBd.Test_Name,
                                        Admin_Id = _testInBd.Admin_Id,
                                        Time_Limit_In_Minutes = _testInBd.Time_Limit_In_Minutes,
                                        Passing_Score = _testInBd.Passing_Score,
                                        Theme_Id = theme.Theme_Id
                                    };
                                    DB.AddTest(newTest);
                                    tests.Add(newTest);
                                }
                                else
                                {
                                    MainWindow.Snackbar.MessageQueue.Enqueue("Adding question to selected test...");
                                }

                                tests = DB.GetTestsByThemeId(_selectedTheme.Theme_Id);
                                if (tests.Any())
                                    foreach (var teste in tests)
                                        if (_selectedTest == teste.Test_Name)
                                        {
                                            test.Test_Id = teste.Test_Id;
                                            test.Admin_Id = teste.Admin_Id;
                                            test.Test_Name = teste.Test_Name;
                                            test.Theme_Id = teste.Theme_Id;
                                            break;
                                        }

                                //--------------------------------------------------------------------------------------------------------------------------

                                var questions = DB.GetQuestionsByTestId(test.Test_Id);
                                var question = new Questions_For_Tests();
                                var flagQuestion = false; //проверка, есть ли вопросы в бд

                                if (questions.Any())
                                    foreach (var ques in questions)
                                        if (_selectedQuestionn == ques.Question)
                                        {
                                            flagQuestion = true;
                                            question.Question_Id = ques.Question_Id;
                                            question.Test_Id = ques.Test_Id;
                                            question.Question_Number = ques.Question_Number;
                                            question.Question = ques.Question;
                                            break;
                                        }

                                if (!flagQuestion)
                                {
                                    _numberQuestion = questions.Count + 1;
                                    var newQuestion = new Questions_For_Tests
                                    {
                                        Test_Id = test.Test_Id,
                                        Question_Number = _numberQuestion,
                                        Question = _questionInDb.Question
                                    };
                                    DB.AddQuestion(newQuestion);
                                    questions.Add(newQuestion);
                                    _numberQuestion++;
                                }
                                else
                                {
                                    MainWindow.ShowDialog("This question already exists!");
                                    return;
                                }

                                var isQuestion = false;
                                questions = DB.GetQuestionsByTestId(test.Test_Id);

                                if (questions.Any())
                                    foreach (var ques in questions)
                                        if (_selectedQuestionn == ques.Question)
                                        {
                                            isQuestion = true;
                                            question.Question_Id = ques.Question_Id;
                                            question.Test_Id = ques.Test_Id;
                                            question.Question_Number = ques.Question_Number;
                                            question.Question = ques.Question;
                                            break;
                                        }

                                //--------------------------------------------------------------------------------------------------------------------------

                                if (isQuestion)
                                {
                                    var newAnswer = new Answers_For_Tests
                                    {
                                        Question_Id = question.Question_Id
                                    };
                                    var isRight = true;
                                    for (var i = 0; i < 4; i++)
                                    {
                                        newAnswer.Is_Right = isRight;
                                        newAnswer.Answer = _listAnswerInDb[i].Answer;
                                        DB.AddAnswer(newAnswer);
                                        isRight = false;
                                    }

                                    transaction.Commit();
                                    MainWindow.Snackbar.MessageQueue.Enqueue("Question added successfully!");
                                }
                                _dontSelectEventQuestion = true;
                                _dontSelectEventTest = true;
                                var newItem = new ListViewItem {Content = question.Question};
                                _questions = questions;
                                _tests = tests;
                                SelectedTestQuestions.Items.Add(newItem);
                                CurrentlyAddedQuestions.Items.Remove(CurrentlyAddedQuestions.SelectedItems[0]);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                MessageBox.Show(ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                }
                else
                {
                    MainWindow.ShowDialog("Enter all data!");
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