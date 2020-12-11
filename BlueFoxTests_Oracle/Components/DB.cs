using System;
using System.Collections.Generic;
using System.Data;
using BlueFoxTests_Oracle.Models;
using Oracle.ManagedDataAccess.Client;

namespace BlueFoxTests_Oracle.Components
{
    // ReSharper disable once InconsistentNaming
    public static class DB
    {
        static DB()
        {
            Conn = new OracleConnection(Config.ConnectionString);
            Conn.OpenAsync();
        }

        public static OracleConnection Conn { get; }

        public static void Initialize()
        {
            Logger.Log.Info("DB has been initialized.");
        }

        //public static void CreateSynonymsForProcs()
        //{
        //    string sql = "";
        //    var getSyns = new OracleCommand
        //    {
        //        Connection = Conn,
        //        CommandText = "CREATE_SYNONS",
        //        CommandType = CommandType.StoredProcedure
        //    };

        //    OracleCommand cmd = new OracleCommand();
        //    cmd.Connection = Conn;

        //    var reader = getSyns.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        //sql = $"create synonym {reader["OBJECT_NAME"]} for system.{reader["OBJECT_NAME"]};";
        //        sql = $"create synonym GET_USERS for system.GET_USERS;";
        //        cmd.CommandText = sql;
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        public static User GetUserById(int id)
        {
            User user = null;
            var getUserByIdCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_USER_BY_ID",
                CommandType = CommandType.StoredProcedure
            };
            getUserByIdCmd.Parameters.Add("usrname", OracleDbType.Decimal).Value = id;
            var reader = getUserByIdCmd.ExecuteReader();
            while (reader.Read())
                user = new User
                {
                    User_Id = int.Parse(reader["user_id"].ToString()),
                    Username = reader["username"].ToString(),
                    Password_Hash = reader["password_hash"].ToString()
                };

            return user;
        }

        public static User GetUserByUsername(string username)
        {
            User user = null;
            var getUserByUsernameCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_USER_BY_USERNAME",
                CommandType = CommandType.StoredProcedure
            };
            getUserByUsernameCmd.Parameters.Add("usrname", OracleDbType.NVarchar2).Value = username;
            var reader = getUserByUsernameCmd.ExecuteReader();
            while (reader.Read())
                if (string.Equals(username, reader["username"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                    user = new User
                    {
                        User_Id = int.Parse(reader["user_id"].ToString()),
                        Username = reader["username"].ToString(),
                        Password_Hash = reader["password_hash"].ToString()
                    };

            return user;
        }

        public static User CheckUserLogin(string username, string pHash)
        {
            User user = null;
            var checkUserLoginCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "CHECK_USER_LOGIN",
                CommandType = CommandType.StoredProcedure
            };
            checkUserLoginCmd.Parameters.Add("usrname", OracleDbType.NVarchar2).Value = username;
            checkUserLoginCmd.Parameters.Add("p_hash", OracleDbType.NVarchar2).Value = pHash;
            var reader = checkUserLoginCmd.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                    user = new User
                    {
                        User_Id = int.Parse(reader["user_id"].ToString()),
                        Username = reader["username"].ToString()
                    };

            return user;
        }

        public static int IsAdmin(int id)
        {
            var userIsAdminCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "USER_IS_ADMIN",
                CommandType = CommandType.StoredProcedure
            };
            userIsAdminCmd.Parameters.Add("return_value", OracleDbType.Decimal).Direction =
                ParameterDirection.ReturnValue;
            userIsAdminCmd.Parameters.Add("u_id", OracleDbType.Decimal).Value = id;
            userIsAdminCmd.ExecuteNonQuery();
            return int.Parse(userIsAdminCmd.Parameters["return_value"].Value.ToString());
        }

        public static bool UserExists(int id)
        {
            var userExists = new OracleCommand
            {
                Connection = Conn,
                CommandText = "USER_EXISTS",
                CommandType = CommandType.StoredProcedure
            };
            userExists.Parameters.Add("return_value", OracleDbType.Decimal).Direction =
                ParameterDirection.ReturnValue;
            userExists.Parameters.Add("usrname", OracleDbType.Decimal).Value = id;
            userExists.ExecuteNonQuery();
            return userExists.Parameters["return_value"].Value.ToString() == "1";
        }

        public static bool UserExistsByUsername(string username)
        {
            var userExists = new OracleCommand
            {
                Connection = Conn,
                CommandText = "USER_EXISTS_USERNAME",
                CommandType = CommandType.StoredProcedure
            };
            userExists.Parameters.Add("return_value", OracleDbType.Decimal).Direction =
                ParameterDirection.ReturnValue;
            userExists.Parameters.Add("usrname", OracleDbType.NVarchar2).Value = username;
            userExists.ExecuteNonQuery();
            return userExists.Parameters["return_value"].Value.ToString() == "1";
        }

        public static async void AddUser(User newUser)
        {
            var addUserCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "ADD_USER",
                CommandType = CommandType.StoredProcedure
            };
            addUserCmd.Parameters.Add("username", OracleDbType.NVarchar2).Value = newUser.Username;
            addUserCmd.Parameters.Add("password_hash", OracleDbType.NVarchar2).Value = newUser.Password_Hash;
            await addUserCmd.ExecuteNonQueryAsync();
        }

        public static async void InitUserInfoAndStats(int id)
        {
            var initUserInfoAndStatsCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "INITIALIZE_USER_INFO_AND_STATS",
                CommandType = CommandType.StoredProcedure
            };
            initUserInfoAndStatsCmd.Parameters.Add("u_id", OracleDbType.Decimal).Value = id;
            await initUserInfoAndStatsCmd.ExecuteNonQueryAsync();
        }

        public static User_Info GetUserInfo(int id)
        {
            User_Info userInfo = null;
            var getUserInfo = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_USER_INFO",
                CommandType = CommandType.StoredProcedure
            };
            getUserInfo.Parameters.Add("u_id", OracleDbType.Decimal).Value = id;
            var reader = getUserInfo.ExecuteReader();
            while (reader.Read())
                userInfo = new User_Info
                {
                    Name = reader["name"].ToString(),
                    Gender = reader["gender"].ToString(),
                    Location = reader["location"].ToString(),
                    Birthday = reader["birthday"].ToString(),
                    Summary = reader["summary"].ToString(),
                    Education = reader["education"].ToString(),
                    Work = reader["work"].ToString()
                };

            return userInfo;
        }

        public static List<Test_Result> GetUserTestResults(int userId)
        {
            var results = new List<Test_Result>();
            var getUserTestResultsCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_TEST_RESULTS_BY_USER_ID",
                CommandType = CommandType.StoredProcedure
            };
            getUserTestResultsCmd.Parameters.Add("u_id", OracleDbType.Decimal).Value = userId;
            var reader = getUserTestResultsCmd.ExecuteReader();
            while (reader.Read())
                results.Add(new Test_Result
                {
                    Result_Id = int.Parse(reader["result_id"].ToString()),
                    User_Id = int.Parse(reader["user_id"].ToString()),
                    Test_Id = int.Parse(reader["test_id"].ToString()),
                    Try_Count = int.Parse(reader["try_count"].ToString()),
                    Is_Passed = reader["is_passed"].ToString() != "0",
                    Score = int.Parse(reader["score"].ToString()),
                    Right_Answers_Count = int.Parse(reader["right_answers_count"].ToString()),
                    Questions_Count = int.Parse(reader["questions_count"].ToString()),
                    Start_Date = DateTime.Parse(reader["start_date"].ToString()),
                    End_Date = DateTime.Parse(reader["end_date"].ToString())
                });

            return results;
        }

        public static async void UpdateUserInfo(User_Info userInfo)
        {
            var updateUserInfoCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "UPDATE_USER_INFO",
                CommandType = CommandType.StoredProcedure
            };
            updateUserInfoCmd.Parameters.Add("u_id", OracleDbType.Decimal).Value = userInfo.UserId;
            updateUserInfoCmd.Parameters.Add("u_name", OracleDbType.NVarchar2).Value = userInfo.Name;
            updateUserInfoCmd.Parameters.Add("u_gender", OracleDbType.NVarchar2).Value = userInfo.Gender;
            updateUserInfoCmd.Parameters.Add("u_location", OracleDbType.NVarchar2).Value = userInfo.Location;
            updateUserInfoCmd.Parameters.Add("u_birthday", OracleDbType.NVarchar2).Value = userInfo.Birthday;
            updateUserInfoCmd.Parameters.Add("u_summary", OracleDbType.NVarchar2).Value = userInfo.Summary;
            updateUserInfoCmd.Parameters.Add("u_education", OracleDbType.NVarchar2).Value = userInfo.Education;
            updateUserInfoCmd.Parameters.Add("u_work", OracleDbType.NVarchar2).Value = userInfo.Work;
            await updateUserInfoCmd.ExecuteNonQueryAsync();
        }

        public static List<Theme> GetThemes()
        {
            var themes = new List<Theme>();
            var getThemesCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_THEMES",
                CommandType = CommandType.StoredProcedure
            };
            var reader = getThemesCmd.ExecuteReader();
            while (reader.Read())
                themes.Add(new Theme
                {
                    Theme_Id = int.Parse(reader["theme_id"].ToString()),
                    Theme_Name = reader["theme_name"].ToString()
                });

            return themes;
        }

        public static Theme GetThemeByName(string themeName)
        {
            Theme theme = null;
            var getUserByUsernameCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_THEME_BY_NAME",
                CommandType = CommandType.StoredProcedure
            };
            getUserByUsernameCmd.Parameters.Add("th_name", OracleDbType.NVarchar2).Value = themeName;
            var reader = getUserByUsernameCmd.ExecuteReader();
            while (reader.Read())
                if (string.Equals(themeName, reader["theme_name"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                    theme = new Theme()
                    {
                        Theme_Id = int.Parse(reader["theme_id"].ToString()),
                        Theme_Name = reader["theme_name"].ToString(),
                    };

            return theme;
        }

        public static async void AddTheme(Theme newTheme)
        {
            var addThemeCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "ADD_THEME",
                CommandType = CommandType.StoredProcedure
            };
            addThemeCmd.Parameters.Add("th_name", OracleDbType.NVarchar2).Value = newTheme.Theme_Name;
            await addThemeCmd.ExecuteNonQueryAsync();
        }

        public static List<Test> GetTestsByThemeId(int themeId)
        {
            var tests = new List<Test>();
            var getTestsByThemeIdCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_TESTS_BY_THEME_ID",
                CommandType = CommandType.StoredProcedure
            };
            getTestsByThemeIdCmd.Parameters.Add("th_id", OracleDbType.Decimal).Value = themeId;
            var reader = getTestsByThemeIdCmd.ExecuteReader();
            while (reader.Read())
                tests.Add(new Test
                {
                    Test_Id = int.Parse(reader["test_id"].ToString()),
                    Admin_Id = int.Parse(reader["admin_id"].ToString()),
                    Test_Name = reader["test_name"].ToString(),
                    Theme_Id = int.Parse(reader["theme_id"].ToString()),
                    Time_Limit_In_Minutes = int.Parse(reader["time_limit_in_minutes"].ToString()),
                    Passing_Score = int.Parse(reader["passing_score"].ToString())
                });

            return tests;
        }

        public static Test GetTestByName(string testName)
        {
            Test test = null;
            var getUserByUsernameCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_TEST_BY_NAME",
                CommandType = CommandType.StoredProcedure
            };
            getUserByUsernameCmd.Parameters.Add("t_name", OracleDbType.NVarchar2).Value = testName;
            var reader = getUserByUsernameCmd.ExecuteReader();
            while (reader.Read())
                if (string.Equals(testName, reader["test_name"].ToString(), StringComparison.CurrentCultureIgnoreCase))
                    test = new Test()
                    {
                        Test_Id = int.Parse(reader["test_id"].ToString()),
                        Admin_Id = int.Parse(reader["admin_id"].ToString()),
                        Test_Name = reader["test_name"].ToString(),
                        Theme_Id = int.Parse(reader["theme_id"].ToString()),
                        Time_Limit_In_Minutes = int.Parse(reader["time_limit_in_minutes"].ToString()),
                        Passing_Score = int.Parse(reader["passing_score"].ToString())
                    };

            return test;
        }


        public static List<Questions_For_Tests> GetQuestionsByTestId(int testId)
        {
            var questions = new List<Questions_For_Tests>();
            var getQsByTestIdCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_QUESTIONS_BY_TEST_ID",
                CommandType = CommandType.StoredProcedure
            };
            getQsByTestIdCmd.Parameters.Add("t_id", OracleDbType.Decimal).Value = testId;
            var reader = getQsByTestIdCmd.ExecuteReader();
            while (reader.Read())
                questions.Add(new Questions_For_Tests
                {
                    Question_Id = int.Parse(reader["question_id"].ToString()),
                    Test_Id = int.Parse(reader["test_id"].ToString()),
                    Question_Number = int.Parse(reader["question_number"].ToString()),
                    Question = reader["question"].ToString()
                });

            return questions;
        }

        public static List<Answers_For_Tests> GetAnswersByQuestionId(int questionId)
        {
            var answers = new List<Answers_For_Tests>();
            var getAnswersByQidCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_ANSWERS_BY_QUESTION_ID",
                CommandType = CommandType.StoredProcedure
            };
            getAnswersByQidCmd.Parameters.Add("q_id", OracleDbType.Decimal).Value = questionId;
            var reader = getAnswersByQidCmd.ExecuteReader();
            while (reader.Read())
                answers.Add(new Answers_For_Tests
                {
                    Answer_Id = int.Parse(reader["answer_id"].ToString()),
                    Question_Id = int.Parse(reader["question_id"].ToString()),
                    Is_Right = int.Parse(reader["is_right"].ToString()) == 1,
                    Answer = reader["answer"].ToString()
                });

            return answers;
        }

        public static async void AddTest(Test newTest)
        {
            var addTestCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "ADD_TEST",
                CommandType = CommandType.StoredProcedure
            };
            addTestCmd.Parameters.Add("a_id", OracleDbType.Decimal).Value = newTest.Admin_Id;
            addTestCmd.Parameters.Add("t_name", OracleDbType.NVarchar2).Value = newTest.Test_Name;
            addTestCmd.Parameters.Add("th_id", OracleDbType.Decimal).Value = newTest.Theme_Id;
            addTestCmd.Parameters.Add("time_limit", OracleDbType.Decimal).Value = newTest.Time_Limit_In_Minutes;
            addTestCmd.Parameters.Add("pass_score", OracleDbType.Decimal).Value = newTest.Passing_Score;
            await addTestCmd.ExecuteNonQueryAsync();
        }

        public static async void AddQuestion(Questions_For_Tests newQuestion)
        {
            var addQuestionCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "ADD_QUESTION",
                CommandType = CommandType.StoredProcedure
            };
            addQuestionCmd.Parameters.Add("t_id", OracleDbType.Decimal).Value = newQuestion.Test_Id;
            addQuestionCmd.Parameters.Add("q_number", OracleDbType.Decimal).Value = newQuestion.Question_Number;
            addQuestionCmd.Parameters.Add("qn", OracleDbType.NVarchar2).Value = newQuestion.Question;
            await addQuestionCmd.ExecuteNonQueryAsync();
        }

        public static async void AddAnswer(Answers_For_Tests newAnswer)
        {
            var addAnswerCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "ADD_ANSWER",
                CommandType = CommandType.StoredProcedure
            };
            addAnswerCmd.Parameters.Add("answ", OracleDbType.NVarchar2).Value = newAnswer.Answer;
            addAnswerCmd.Parameters.Add("is_r", OracleDbType.Decimal).Value = newAnswer.Is_Right ? 1 : 0;
            addAnswerCmd.Parameters.Add("q_id", OracleDbType.NVarchar2).Value = newAnswer.Question_Id;
            await addAnswerCmd.ExecuteNonQueryAsync();
        }

        public static async void InitTestResult(int userId, int testId, int qCount, int tryCount, DateTime startDate)
        {
            var initTestResultCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "INITIALIZE_TEST_RESULT",
                CommandType = CommandType.StoredProcedure
            };
            initTestResultCmd.Parameters.Add("u_id", OracleDbType.Decimal).Value = userId;
            initTestResultCmd.Parameters.Add("t_id", OracleDbType.Decimal).Value = testId;
            initTestResultCmd.Parameters.Add("try_c", OracleDbType.Decimal).Value = testId;
            initTestResultCmd.Parameters.Add("q_count", OracleDbType.Decimal).Value = qCount;
            initTestResultCmd.Parameters.Add("st_date", OracleDbType.Date).Value = startDate;
            await initTestResultCmd.ExecuteNonQueryAsync();
        }

        public static async void UpdateTestResultOnFinish(Test_Result testResult)
        {
            var updateTestResultCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "UPDATE_TEST_RESULT",
                CommandType = CommandType.StoredProcedure
            };
            updateTestResultCmd.Parameters.Add("res_id", OracleDbType.Decimal).Value = testResult.Result_Id;
            updateTestResultCmd.Parameters.Add("scor", OracleDbType.Decimal).Value = testResult.Score;
            updateTestResultCmd.Parameters.Add("q_count", OracleDbType.Decimal).Value = testResult.Questions_Count;
            updateTestResultCmd.Parameters.Add("right_ans_count", OracleDbType.Decimal).Value = testResult.Right_Answers_Count;
            updateTestResultCmd.Parameters.Add("is_pass", OracleDbType.Decimal).Value = testResult.Is_Passed;
            updateTestResultCmd.Parameters.Add("end_d", OracleDbType.Date).Value = testResult.End_Date;
            await updateTestResultCmd.ExecuteNonQueryAsync();
        }
    }
}