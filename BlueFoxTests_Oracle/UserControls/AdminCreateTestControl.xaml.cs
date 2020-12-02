using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
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
        private readonly List<Theme> _themes = new List<Theme>();
        private List<Test> _tests = new List<Test>();

        private readonly Theme themeInBD = new Theme();
        private bool _canSaveTest;
        private List<Answers_For_Tests> _listAnswerInDb;
        private bool dontSelectEventQuestion;
        private bool dontSelectEventTest;

        private int numberQuestion = 1;

        private string SelectedQuestionn = Empty;
        private string SelectedTestt = Empty;
        private string SelectedThemee = Empty;

        private Questions_For_Tests SelectedQuestion = new Questions_For_Tests();
        private Test SelectedTest = new Test();
        private Theme SelectedTheme = new Theme();

        public AdminCreateTestControl()
        {
            InitializeComponent();
            listThemes.SelectionChanged += NewTheme_Selected;
            listTests.SelectionChanged += NewTest_Selected;
            CurrentlyAddedQuestions.SelectionChanged += NewQuestion_Selected;
            SelectedTestQuestions.SelectionChanged += SelectedTestQuestions_SelectionChanged;
            try
            {
                var themes = DB.GetThemes();

                if (themes.Any())
                    foreach (var themee in themes)
                    {
                        _themes.Add(themee);
                        var theme = new ListViewItem { Content = themee.Theme_Name };
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
                using var db = new BlueFoxContext();
                if (IsNullOrEmpty(txbTheme.Text))
                {
                    ShowDialog("Enter Theme Name");
                }
                else if (DB.GetThemes().FirstOrDefault(t => string.Equals(t.Theme_Name.Trim(), txbTheme.Text.Trim(), StringComparison.CurrentCultureIgnoreCase)) == null)
                {
                    var newTheme = new Theme { Theme_Name = txbTheme.Text };
                    DB.AddTheme(newTheme);
                    _themes.Add(newTheme);
                    listThemes.Items.Add(new ListViewItem { Content = newTheme.Theme_Name });
                }
                else
                {
                    ShowDialog("Theme Exists");
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
                dontSelectEventTest = true;
                dontSelectEventQuestion = true;
                listTests.Items.Clear();
                SelectedTestQuestions.Items.Clear();

                var item = (ListViewItem)listThemes.SelectedItems[0];
                SelectedThemee = item.Content.ToString();
                using var db = new BlueFoxContext();
                var currentTheme = _themes.First(t => t.Theme_Name == SelectedThemee);
                _tests = DB.GetTestsByThemeId(currentTheme.Theme_Id);
                foreach (var test in _tests)
                {
                    var testsViewItem = new ListViewItem { Content = test.Test_Name };
                    listTests.Items.Add(testsViewItem);
                }

                dontSelectEventTest = false;
                dontSelectEventQuestion = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void NewTest_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!dontSelectEventTest)
                {
                    dontSelectEventQuestion = true;
                    var item = (ListViewItem)listTests.SelectedItems[0];
                    SelectedTestt = item.Content.ToString();
                    SelectedTestQuestions.Items.Clear();
                    AnswersPanel.IsEnabled = true;

                    using var db = new BlueFoxContext();

                    var currentTest = _tests.FirstOrDefault(t => t.Test_Name == SelectedTestt);
                    if (currentTest == null) return;
                    foreach (var question in DB.GetQuestionsByTestId(currentTest.Test_Id))
                    {
                        var existingQuestion = new ListViewItem { Content = question.Question };
                        SelectedTestQuestions.Items.Add(existingQuestion);
                    }

                    dontSelectEventQuestion = false;
                }
                else
                {
                    dontSelectEventTest = false;
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
                if (!dontSelectEventQuestion)
                {
                    txbAnswer1.Text = Empty;
                    txbAnswer2.Text = Empty;
                    txbAnswer3.Text = Empty;
                    txbAnswer4.Text = Empty;
                    AnswersPanel.IsEnabled = true;
                    var item = (ListViewItem)CurrentlyAddedQuestions.SelectedItems[0];
                    SelectedQuestionn = item.Content.ToString();
                }
                else
                {
                    dontSelectEventQuestion = false;
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
                if (!dontSelectEventQuestion)
                {
                    var item = (ListViewItem)SelectedTestQuestions.SelectedItems[0];
                    var selectedQuestionTemp = item.Content.ToString();
                    using var db = new BlueFoxContext();
                    var currentQuestion = db.Questions_For_Tests.First(q => q.Question == selectedQuestionTemp);
                    var answerList = new List<Answers_For_Tests>();
                    var answersForSelected =
                        db.Answers_For_Tests.Where(a => a.Question_Id == currentQuestion.Question_Id);
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
                    dontSelectEventQuestion = false;
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
                SelectedQuestionn = txbQuestion.Text;
                _questionInDb.Question = txbQuestion.Text;

                var newItem = new ListViewItem { Content = txbQuestion.Text };

                CurrentlyAddedQuestions.Items.Add(newItem);
                txbQuestion.Text = "";
            }
            else
            {
                MessageBox.Show("Enter Question");
            }
        }

        private void btnAddAnswer_Click(object sender, RoutedEventArgs e)
        {
            _listAnswerInDb = new List<Answers_For_Tests>();
            //если выбрал вопрос для которого буду создавать ответы
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
                            MessageBox.Show("Dont repeat the answer!");
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

                            txbAnswer1.Text = "";
                            txbAnswer2.Text = "";
                            txbAnswer3.Text = "";
                            txbAnswer4.Text = "";
                            MessageBox.Show("Now you can save!");
                            _canSaveTest = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter all answers!");
                    }
                }
                else
                {
                    MessageBox.Show("Choose a question for answers!");
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
                    using var db = new BlueFoxContext();
                    var flagTheme = false;

                    if (db.Tests.Any())
                        foreach (var test in db.Tests)
                            if (txbTest.Text == test.Test_Name)
                            {
                                flagTheme = true;
                                break;
                            }

                    if (!flagTheme)
                    {
                        SelectedTestt = txbTest.Text;

                        var newItem = new ListViewItem { Content = txbTest.Text };

                        _testInBd.Test_Name = txbTest.Text;
                        listTests.Items.Add(newItem);
                        txbTest.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("This theme already exists!");
                    }
                }
                else
                {
                    MessageBox.Show("Enter test name!");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (themeInBD != null && _testInBd != null && _questionInDb != null && _listAnswerInDb != null)
                {
                    //если не ввожу значения, то беру из выбранных
                    if (IsNullOrEmpty(themeInBD.Theme_Name) || IsNullOrEmpty(_testInBd.Test_Name))
                    {
                        themeInBD.Theme_Name = SelectedThemee;
                        _testInBd.Test_Name = SelectedTestt;
                    }

                    var flagSave = false; //проверка, можно ли сохранить в бд!
                    var countAnswer = 0; //проверка, есть ли все ответы!

                    if (!IsNullOrEmpty(themeInBD.Theme_Name) && !IsNullOrEmpty(_testInBd.Test_Name) &&
                        !IsNullOrEmpty(_questionInDb.Question) && _listAnswerInDb.Count == 4)
                    {
                        for (var i = 0; i < _listAnswerInDb.Count; i++)
                            if (!IsNullOrEmpty(_listAnswerInDb[i].Answer))
                                countAnswer++;
                        if (countAnswer == 4) flagSave = true;
                    }
                    else
                    {
                        MessageBox.Show("Enter all data!");
                        return;
                    }

                    //если всё классно, все объекты заполнены, то я начинаю их сохранять в бд
                    if (flagSave)
                    {
                        using var db = new BlueFoxContext();
                        try
                        {
                            using var transaction = db.Database.BeginTransaction();
                            {
                                try
                                {
                                    //Объект с которым буду работать и флаг(есть ли такая тема в бд)
                                    var theme = new Theme();
                                    var flagTheme = false;

                                    if (db.Themes_For_Tests.Any())
                                        foreach (var teme in db.Themes_For_Tests)
                                            if (SelectedThemee == teme.Theme_Name)
                                            {
                                                flagTheme = true;
                                                theme.Theme_Id = teme.Theme_Id;
                                                theme.Theme_Name = teme.Theme_Name;
                                                break;
                                            }


                                    if (flagTheme)
                                    {
                                        MessageBox.Show("Adding test to selected theme");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Choose a theme!");
                                        return;
                                    }

                                    var test = new Test();
                                    var flagTest = false;

                                    if (db.Tests.Any())
                                        foreach (var teste in db.Tests)
                                            if (SelectedTestt == teste.Test_Name)
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
                                        // Добавить команду добавления теста
                                        numberQuestion = 1;

                                        db.Tests.Add(new Test
                                        {
                                            Test_Name = _testInBd.Test_Name,
                                            Admin_Id = _testInBd.Admin_Id,
                                            Theme_Id = theme.Theme_Id
                                        });
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Adding question to selected test");
                                    }


                                    if (db.Tests.Any())
                                        foreach (var teste in db.Tests)
                                            if (SelectedTestt == teste.Test_Name)
                                            {
                                                test.Test_Id = teste.Test_Id;
                                                test.Admin_Id = teste.Admin_Id;
                                                test.Test_Name = teste.Test_Name;
                                                test.Theme_Id = teste.Theme_Id;
                                                break;
                                            }

                                    //--------------------------------------------------------------------------------------------------------------------------

                                    var question = new Questions_For_Tests();
                                    var flagQuestion = false; //проверка, есть ли вопросы в бд

                                    if (db.Questions_For_Tests.Any())
                                        foreach (var ques in db.Questions_For_Tests)
                                            if (SelectedQuestionn == ques.Question)
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
                                        db.Questions_For_Tests.Add(new Questions_For_Tests
                                        {
                                            Test_Id = test.Test_Id,
                                            Question_Number = numberQuestion,
                                            Question = _questionInDb.Question
                                        });
                                        db.SaveChanges();

                                        numberQuestion++;
                                    }
                                    else
                                    {
                                        MessageBox.Show("This question already exists!");
                                        return;
                                    }

                                    var isQuestion = false;

                                    if (db.Questions_For_Tests.Any())
                                        foreach (var ques in db.Questions_For_Tests)
                                            if (SelectedQuestionn == ques.Question)
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
                                        var answeradd = new Answers_For_Tests
                                        {
                                            Question_Id = question.Question_Id
                                        };
                                        var isRight = true;
                                        for (var i = 0; i < 4; i++)
                                        {
                                            answeradd.Is_Right = isRight;
                                            answeradd.Answer = _listAnswerInDb[i].Answer;
                                            db.Answers_For_Tests.Add(answeradd);
                                            db.SaveChanges();
                                            isRight = false;
                                        }

                                        await db.SaveChangesAsync();
                                        transaction.Commit();
                                    }

                                    MessageBox.Show("Question added successfully");
                                    dontSelectEventQuestion = true;
                                    dontSelectEventTest = true;
                                    var newItem = new ListViewItem { Content = question.Question };
                                    SelectedTestQuestions.Items.Add(newItem);
                                    CurrentlyAddedQuestions.Items.Remove(CurrentlyAddedQuestions.SelectedItems[0]);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    transaction.Rollback();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Enter all data!");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void EnableGrid(object sender, RoutedEventArgs e)
        {
            MainGrid.IsEnabled = true;
        }

        private void ShowDialog(string text)
        {
            DialogHost.IsOpen = true;
            MainGrid.IsEnabled = false;
            DialogText.Text = text;
        }
    }
}