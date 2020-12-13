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
using BlueFoxTests_Oracle.Models;
using BlueFoxTests_Oracle.Windows;

namespace BlueFoxTests_Oracle.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UserStatsControl.xaml
    /// </summary>
    public partial class UserStatsControl : UserControl
    {
        private static User_Stats _userStats;
        private static TextBlock _tbTotalAnswered;
        private static TextBlock _tbRightAnswered;
        private static TextBlock _tbAvgScore;
        private static TextBlock _tbTotalTests;
        private static TextBlock _tbPassedTests;

        public UserStatsControl()
        {
            InitializeComponent();
            _tbTotalAnswered = tbTotalAnswered;
            _tbRightAnswered = tbRightAnswered;
            _tbAvgScore = tbAvgScore;
            _tbTotalTests = tbTotalTests;
            _tbPassedTests = tbPassedTests;
            UpdateStats();
        }

        public static void UpdateStats()
        {
            _userStats = MainWindow.User.User_Stats;
            _tbTotalAnswered.Text = $"Total answered questions: {_userStats.Total_Answered}";
            _tbRightAnswered.Text = $"Correct answered questions: {_userStats.Right_Answered}";
            _tbAvgScore.Text = $"Average score: {_userStats.Avg_Score}%";
            _tbTotalTests.Text = $"Finished tests: {_userStats.Finished_Tests_Count}";
            _tbPassedTests.Text = $"Positive passed tests: {_userStats.Passed_Tests_Count}";
        }
    }
}
