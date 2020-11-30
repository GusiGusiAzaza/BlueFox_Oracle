using System;
using System.Windows;
using System.Windows.Controls;
using BlueFoxTests_Oracle.Components;
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;
using MaterialDesignThemes.Wpf;
using static System.String;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    ///     Логика взаимодействия для UserProfileUserControl.xaml
    /// </summary>
    public partial class UserProfile : UserControl
    {
        public UserProfile()
        {
            InitializeComponent();
            LoadUserInfo();
        }

        private void EditNameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string bday = "";
                if (Birthday.SelectedDate != null) bday = Birthday.SelectedDate.Value.ToShortDateString();

                var userInfo = new User_Info
                {
                    UserId = MainWindow.User.UserId,
                    Name = Namee.Text,
                    Gender = Gender.Text,
                    Location = Location.Text,
                    Birthday = bday,
                    Summary = Summary.Text,
                    Education = Education.Text,
                    Work = Work.Text
                };
                DB.UpdateUserInfo(userInfo);
                //using BlueFoxContext db = new BlueFoxContext();
                //var user = db.Users.FirstOrDefault(u => u.User_Id == MainWindow.User.User_Id);

                //if (user != null && user.User_Info.Name != Namee.Text) user.User_Info.Name = Namee.Text;
                //if (user != null && user.User_Info.Gender != Gender.Text) user.User_Info.Gender = Gender.Text;
                //if (user != null && user.User_Info.Location != Location.Text) user.User_Info.Location = Location.Text;
                ////if (user != null && user.User_Info.Birthday != Birthday.SelectedDate) user.User_Info.Birthday = Birthday.SelectedDate;
                //if (user != null && user.User_Info.Summary != Summary.Text) user.User_Info.Summary = Summary.Text;
                //if (user != null && user.User_Info.Education != Education.Text) user.User_Info.Education = Education.Text;
                //if (user != null && user.User_Info.Work != Work.Text) user.User_Info.Work = Work.Text;
                //db.SaveChanges();
                MessageBox.Show("Information successfully changed", "Info");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

        private void LoadUserInfo()
        {
            try
            {
                var userInfo = DB.GetUserInfo(MainWindow.User.UserId);

                if (MainWindow.IsAdmin) ProfileIcon.Kind = PackIconKind.AccountCog;
                Username.Text = MainWindow.User.Username;
                if (!IsNullOrEmpty(userInfo.Name)) Namee.Text = userInfo.Name;
                if (!IsNullOrEmpty(userInfo.Gender)) Gender.Text = userInfo.Gender;
                if (!IsNullOrEmpty(userInfo.Location)) Location.Text = userInfo.Location;
                if (!IsNullOrEmpty(userInfo.Birthday)) Birthday.SelectedDate = DateTime.Parse(userInfo.Birthday);
                if (!IsNullOrEmpty(userInfo.Summary)) Summary.Text = userInfo.Summary;
                if (!IsNullOrEmpty(userInfo.Education)) Education.Text = userInfo.Education;
                if (!IsNullOrEmpty(userInfo.Work)) Work.Text = userInfo.Work;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }
    }
}