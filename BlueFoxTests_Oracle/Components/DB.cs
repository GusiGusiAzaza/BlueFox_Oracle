using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
using System.Windows.Documents;
using BlueFoxTests_Oracle.Models;
using Oracle.ManagedDataAccess.Client;

namespace BlueFoxTests_Oracle.Components
{
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
                    UserId = int.Parse(reader["user_id"].ToString()),
                    Username = reader["username"].ToString(),
                    PasswordHash = reader["password_hash"].ToString()
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
                        UserId = int.Parse(reader["user_id"].ToString()),
                        Username = reader["username"].ToString(),
                        PasswordHash = reader["password_hash"].ToString()
                    };

            return user;
        }

        public static bool IsAdmin(int id)
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
            return userIsAdminCmd.Parameters["return_value"].Value.ToString() == "1";
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
            addUserCmd.Parameters.Add("password_hash", OracleDbType.NVarchar2).Value = newUser.PasswordHash;
            await addUserCmd.ExecuteNonQueryAsync();
        }

        public static async void InitUserInfo(int id)
        {
            var initUserInfoCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "INITIALIZE_USER_INFO",
                CommandType = CommandType.StoredProcedure
            };
            initUserInfoCmd.Parameters.Add("u_id", OracleDbType.Decimal).Value = id;
            await initUserInfoCmd.ExecuteNonQueryAsync();
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
            {
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
            }

            return userInfo;
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
            List<Theme> themes = new List<Theme>();
            var getThemesCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_THEMES",
                CommandType = CommandType.StoredProcedure
            };
            var reader = getThemesCmd.ExecuteReader();
            while (reader.Read())
            {
                themes.Add(new Theme
                {
                    Theme_Id = int.Parse(reader["theme_id"].ToString()),
                    Theme_Name = reader["theme_name"].ToString()
                });
            }

            return themes;
        }

        public static async void AddTheme(Theme newTheme)
        {
            var addUserCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "ADD_THEME",
                CommandType = CommandType.StoredProcedure
            };
            addUserCmd.Parameters.Add("th_name", OracleDbType.NVarchar2).Value = newTheme.Theme_Name;
            await addUserCmd.ExecuteNonQueryAsync();
        }

        public static List<Test> GetTestsByThemeId(int theme_id)
        {
            List<Test> tests = new List<Test>();
            var getUserByUsernameCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_TESTS_BY_THEME_ID",
                CommandType = CommandType.StoredProcedure
            };
            getUserByUsernameCmd.Parameters.Add("th_id", OracleDbType.Decimal).Value = theme_id;
            var reader = getUserByUsernameCmd.ExecuteReader();
            while (reader.Read())
            {
                tests.Add(new Test
                {
                    Test_Id = int.Parse(reader["test_id"].ToString()),
                    Admin_Id = int.Parse(reader["admin_id"].ToString()),
                    Test_Name = reader["test_name"].ToString(),
                    Theme_Id = int.Parse(reader["theme_id"].ToString()),
                    Time_Limit_In_Minutes = int.Parse(reader["time_limit_in_minutes"].ToString()),
                    Passing_Score = int.Parse(reader["passing_score"].ToString())
                });
            }

            return tests;
        }


        public static List<Questions_For_Tests> GetQuestionsByTestId(int test_id)
        {
            List<Questions_For_Tests> questions = new List<Questions_For_Tests>();
            var getUserByUsernameCmd = new OracleCommand
            {
                Connection = Conn,
                CommandText = "GET_QUESTIONS_BY_TEST_ID",
                CommandType = CommandType.StoredProcedure
            };
            getUserByUsernameCmd.Parameters.Add("t_id", OracleDbType.Decimal).Value = test_id;
            var reader = getUserByUsernameCmd.ExecuteReader();
            while (reader.Read())
            {
                questions.Add(new Questions_For_Tests()
                {
                    Question_Id = int.Parse(reader["question_id"].ToString()),
                    Test_Id = int.Parse(reader["test_id"].ToString()),
                    Question_Number = int.Parse(reader["question_number"].ToString()),
                    Question = reader["question"].ToString()
                });
            }

            return questions;
        }
    }
}