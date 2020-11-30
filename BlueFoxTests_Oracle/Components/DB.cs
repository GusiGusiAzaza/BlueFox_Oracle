using System;
using System.Data;
using System.IO.Packaging;
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
    }
}