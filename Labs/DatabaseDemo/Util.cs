using System;
using System.Text;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace DatabaseDemo
{
    internal class Util
    {
        internal static string GetConsole()
        {
            StringBuilder console = new StringBuilder();
            console.Append("********************************************\n");
            console.Append("* Please enter one of the following choices:\n");
            console.Append("*\t0. To remove the users table and start over\n");
            console.Append("*\t1. To enter a new username/password pair\n");
            console.Append("*\t2. To authenticate a username\n");
            console.Append("*\t3. To change a user’s password\n");
            console.Append("*\t4. To delete a user\n");
            console.Append("*\t5. To display all users and passwords\n");
            console.Append("*\t9. To exit the program\n");
            console.Append("********************************************\n");
            console.Append("Please Enter your choice :");
            return console.ToString();
        }
        internal static void ReconstituteDatabase()
        {
            dropTable();
            buildTable();
            Console.WriteLine("Users table reconstituted.");
        }
        private static void dropTable()
        {
            Console.WriteLine("calling dropTable() ...");
            SQLiteConnection conn = CreateConnection();
            SQLiteCommand sqlite_cmd;
            string dropcommand = "drop table if exists Users";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = dropcommand;
            int result = sqlite_cmd.ExecuteNonQuery();
            //Console.WriteLine($"result of drop table is {result}");
            conn.Close();
        }
        private static void buildTable()
        {
            Console.WriteLine("calling buildTable() ...");
            SQLiteConnection conn = CreateConnection();
            SQLiteCommand sqlite_cmd;
            string createtable = "CREATE TABLE Users (" +
            "username nvarchar(50) primary key, " +
            "password nvarchar(64) not null" +
            ");";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = createtable; ;
            int result = sqlite_cmd.ExecuteNonQuery();
            //Console.WriteLine($"result of create table is {result}");
            conn.Close();
        }
        private static SQLiteConnection CreateConnection()
        {
            // Open the connection:
            //SQLiteConnection("Data Source= database.db; Version = 3; New = True; Compress = True;
            // ");
            // Open the connection:
            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source= database.db; New =True; Compress = True; ");
            try
            {
                sqlite_conn.Open();
                Console.WriteLine("SQLite connection created ...");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return sqlite_conn;
        }
        internal static void EnterNewUser()
        {
            Console.WriteLine("Please enter a username and password for the new user: ");
            (string username, string password) = getUserPass();
            password = hashString(password);
            insertData(username, password);
        }
        private static string hashString(string newPass)
        {
            SHA1Managed sha1 = new SHA1Managed();
            byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(newPass));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            string h = sb.ToString();
            return h;
        }
        private static void insertData(string u, string p)
        {
            Console.WriteLine($"insertData({u}, {p})");
            SQLiteConnection conn = CreateConnection();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            string query = $"INSERT INTO Users (username, password) VALUES(\"{u}\", \"{p}\");";
            sqlite_cmd.CommandText = query;
            int result = sqlite_cmd.ExecuteNonQuery();
            //Console.WriteLine($"result of insert is {result}");
            conn.Close();
            Console.WriteLine("Insert successfully executed ...");
        }
        internal static void DisplayUsers()
        {
            Console.WriteLine("calling DisplayUsers() ...");
            SQLiteConnection conn = CreateConnection();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Users";
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            Console.WriteLine("===============================================");
            while (sqlite_datareader.Read())
            {
                string user = sqlite_datareader.GetString(0);
                string cred = sqlite_datareader.GetString(1);
                Console.WriteLine($"{user}, {cred}");
            }
            Console.WriteLine("===============================================");
            conn.Close();
        }
        internal static void AuthenticateUser()
        {
            (string username, string password) = getUserPass();
            (string user, string pass) = getUser(username);
            password = hashString(password);
            if (username == user && password == pass)
                Console.WriteLine($"user {user} is authenticated");
            else
                Console.WriteLine($"user {user} is NOT authenticated");
        }
        private static (string, string) getUser(string u)
        {
            Console.WriteLine("calling getUser() ...");
            SQLiteConnection conn = CreateConnection();
            SQLiteCommand sqlite_cmd = conn.CreateCommand();
            string query = $"SELECT * FROM Users WHERE username like \"{u}\"";
            //Console.WriteLine(query);
            sqlite_cmd.CommandText = query;
            string user = "";
            string pass = "";
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                user = sqlite_datareader.GetString(0);
                pass = sqlite_datareader.GetString(1);
            }
            conn.Close();
            return (user, pass);
        }
        private static (string username, string password) getUserPass()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password for user: ");
            string password = Console.ReadLine();
            return (username, password);
        }
        internal static void ChangePassword()
        {
            (string username, string password) = getUserPass();
            password = hashString(password);
            (string user, string pass) = getUser(username);
            Console.WriteLine($"{username}/{password} ---- {user}/{pass}");
            if (username == user && password == pass)
                updatePassword(username);
            else
                Console.WriteLine($"Sorry, cannot update password for user {username}");
        }
        internal static void DeleteUser()
        {
            (string username, string password) = getUserPass();
            (string user, string pass) = getUser(username);
            password = hashString(password);
            if (username == user && password == pass)
                runDeleteQuery(username);
            else
                Console.WriteLine($"user {user} is NOT deleted");
        }
        private static void updatePassword(string username)
        {
            //Console.WriteLine($"updatePassword({username})");
            //Console.WriteLine($"Please enter a new password for {username}");
            //string pwd = Console.ReadLine();
            //pwd = hashString(pwd);
            //SQLiteConnection conn = CreateConnection();
            //SQLiteCommand sqlite_cmd = conn.CreateCommand();
            //string query = $"UPDATE Users set password = \"{pwd}\" where username like \"{username}\";";
            //Console.WriteLine(query);
            //sqlite_cmd.CommandText = query;
            //int result = sqlite_cmd.ExecuteNonQuery();
            //Console.WriteLine($"result of update is {result}");
            //conn.Close();
            //Console.WriteLine("Password update successfully executed ...");
        }
        private static void runDeleteQuery(string username)
        {
            //Console.WriteLine($"deleteData({username})");
            //SQLiteConnection conn = CreateConnection();
            //SQLiteCommand sqlite_cmd = conn.CreateCommand();
            //string query = $"DELETE FROM Users where username like \"{username}\";";
            //sqlite_cmd.CommandText = query;
            //int result = sqlite_cmd.ExecuteNonQuery();
            ////Console.WriteLine($"result of delete is {result}");
            //conn.Close();
            //Console.WriteLine("Delete successfully executed ...");
        }
        internal  static void RunQuery(string WhatoDo)
        {
            (string username, string password) = getUserPass();
            (string user, string pass) = getUser(username);
            password = hashString(password);
            bool match = (username==user&&password==pass) ? true : false;
            if (match && WhatoDo == "authenticate user")
                Console.WriteLine($"user {user} is authenticated");
            else if (match && WhatoDo == "change password")
                updatePassword(username);
            else if (match && WhatoDo == "delete user")
                runDeleteQuery(username);
            else
                Console.WriteLine($"Sorry, {WhatoDo} did NOT succeed");
        }


    }   }
