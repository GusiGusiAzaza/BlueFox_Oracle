using System;
using System.Linq;
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
    /// Логика взаимодействия для UserProfileUserControl.xaml
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
                using BlueFoxContext db = new BlueFoxContext();
                var user = db.Users.FirstOrDefault(u => u.User_Id == MainWindow.User.User_Id);

                if (user != null && user.User_Info.Name != Namee.Text) user.User_Info.Name = Namee.Text;
                if (user != null && user.User_Info.Gender != Gender.Text) user.User_Info.Gender = Gender.Text;
                if (user != null && user.User_Info.Location != Location.Text) user.User_Info.Location = Location.Text;
                if (user != null && user.User_Info.Birthday != Birthday.SelectedDate) user.User_Info.Birthday = Birthday.SelectedDate;
                if (user != null && user.User_Info.Summary != Summary.Text) user.User_Info.Summary = Summary.Text;
                if (user != null && user.User_Info.Education != Education.Text) user.User_Info.Education = Education.Text;
                if (user != null && user.User_Info.Work != Work.Text) user.User_Info.Work = Work.Text;
                db.SaveChanges();
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
                using BlueFoxContext db = new BlueFoxContext();
                var user = db.Users.FirstOrDefault(u => u.User_Id == MainWindow.User.User_Id);
                if (user != null && user.User_Info == null) db.User_Info.Add(new User_Info()
                {
                    User_Id = user.User_Id
                });
                db.SaveChanges();
                if (MainWindow.IsAdmin) ProfileIcon.Kind = PackIconKind.AccountCog;
                if (user != null && !IsNullOrEmpty(user.Username)) Username.Text = user.Username;
                if (user != null && !IsNullOrEmpty(user.User_Info.Name)) Namee.Text = user.User_Info.Name;
                if (user != null && !IsNullOrEmpty(user.User_Info.Gender)) Gender.Text = user.User_Info.Gender;
                if (user != null && !IsNullOrEmpty(user.User_Info.Location)) Location.Text = user.User_Info.Location;
                if (user?.User_Info.Birthday != null) Birthday.SelectedDate = user.User_Info.Birthday;
                if (user != null && !IsNullOrEmpty(user.User_Info.Summary)) Summary.Text = user.User_Info.Summary;
                if (user != null && !IsNullOrEmpty(user.User_Info.Education)) Education.Text = user.User_Info.Education;
                if (user != null && !IsNullOrEmpty(user.User_Info.Work)) Work.Text = user.User_Info.Work;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                Logger.Log.Error(exception);
            }
        }

    }
}
