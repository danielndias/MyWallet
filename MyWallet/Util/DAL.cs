using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using MyWallet.Models;
using System.Data.SqlClient;

namespace MyWallet.Util
{
    public class DAL
    {
        private static string server = "localhost";
        private static string database = "mywallet";
        private static string user = "root";
        private static string password = "";
        private string connectionString = $"Server={server};Database={database};Uid={user};Pwd={password};SslMode=none;convert zero datetime=True";
        private MySqlConnection connection;

        public DAL()
        {
            connection = new MySqlConnection(connectionString);
            //connection.Open();
        }

        //DQL Operations
        public DataTable RetrieveDataTable(string sql)
        {

            connection.Open();
            DataTable dt = new DataTable();
            MySqlCommand cmd = new MySqlCommand(sql, connection);

            //Adapt the query result and insert into the DataTable
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);

            connection.Close();

            return dt;
        }

        //DML Operations
        public void ExecuteSQLCommand(string sql)
        {
            connection.Open();

            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public UserModel ValidateLogin(UserModel user)
        {
            UserModel newUser = new UserModel();

            String query = "SELECT ID, NAME FROM USER WHERE Email = @email AND PASSWORD = @password";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@password", user.Password);

                connection.Open();

                MySqlDataReader reader = cmd.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        newUser.Id = int.Parse(reader["ID"].ToString());
                        newUser.Name = reader["NAME"].ToString();
                    }
                    connection.Close();
                    return newUser;
                }
                else
                {
                    connection.Close();
                    return null;
                }

            }
        }

        public void CreateUser(UserModel user)
        {
            connection.Open();

            string query = $"INSERT INTO USER (NAME, EMAIL, PASSWORD) VALUES (@name, @email, @password)";

            MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", user.Password);

            cmd.ExecuteNonQuery();

            connection.Close();

        }

        public void CreateAccount(AccountModel acc, string userid)
        {

            connection.Open();

            String query = "INSERT INTO ACCOUNT (NAME, BALANCE, USER_ID) VALUES(@name, @balance, @userid)";
            
            MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@name", acc.Name);
            cmd.Parameters.AddWithValue("@balance", acc.Balance);
            cmd.Parameters.AddWithValue("@userid", userid);

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void InsertCategory(CategoryModel cat, string userid)
        {
            connection.Open();

            String query = "INSERT INTO CATEGORY (Description, Type, User_Id) VALUES(@description, @type, @userid)";

            MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@description", cat.Description);
            cmd.Parameters.AddWithValue("@type", Convert.ToInt32(cat.Type));
            cmd.Parameters.AddWithValue("@userid", userid);

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void UpdateCategory(CategoryModel cat)
        {
            connection.Open();

            String query = "UPDATE CATEGORY SET Description = @description, Type = @type WHERE Id = @id";

            MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@description", cat.Description);
            cmd.Parameters.AddWithValue("@type", Convert.ToInt32(cat.Type));
            cmd.Parameters.AddWithValue("@id", cat.Id);

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void InsertTransaction(TransactionModel tr, string userid)
        {
            connection.Open();

            String query = "INSERT INTO TRANSACTION (Date, Type, Amount, Description, Account_Id, Category_Id, User_Id) VALUES(@date, @type, @amount, @description, @accountid, @categoryid" +
                ", @userid)";

            MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@date", DateTime.Parse(tr.Date).ToString("yyyy/MM/dd"));
            cmd.Parameters.AddWithValue("@type", Convert.ToInt32(tr.Type));
            cmd.Parameters.AddWithValue("@amount", tr.Amount);
            cmd.Parameters.AddWithValue("@description", tr.Description);
            cmd.Parameters.AddWithValue("@accountid", tr.Account_Id);
            cmd.Parameters.AddWithValue("@categoryid", tr.Category_Id);
            cmd.Parameters.AddWithValue("@userid", userid);

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void UpdateTransaction(TransactionModel tr)
        {
            connection.Open();

            String query = $"UPDATE TRANSACTION SET Date = @date, Type = @type, Amount = @amount, Description = @description, Account_Id = @accountid, Category_Id = @categoryid WHERE Id = @id";

            MySqlCommand cmd = new MySqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@date", DateTime.Parse(tr.Date).ToString("yyyy/MM/dd"));
            cmd.Parameters.AddWithValue("@type", Convert.ToInt32(tr.Type));
            cmd.Parameters.AddWithValue("@amount", tr.Amount);
            cmd.Parameters.AddWithValue("@description", tr.Description);
            cmd.Parameters.AddWithValue("@accountid", tr.Account_Id);
            cmd.Parameters.AddWithValue("@categoryid", tr.Category_Id);
            cmd.Parameters.AddWithValue("@id", tr.Id);

            cmd.ExecuteNonQuery();

            connection.Close();
        }
    }
}
