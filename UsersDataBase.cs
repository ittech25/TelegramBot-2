using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot
{
    class UsersDataBase
    {
        readonly string db_table_name = "Users";
        readonly string db_table_chat_id = "chat_id";
        readonly string db_table_situation_id = "situation_id";
        readonly string db_table_situation_done = "situation_done";

        //OleDbConnection connection;

        public UsersDataBase()
        {
            OleDbConnection connection;
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";
            // Create a connection. 
            connection = new OleDbConnection(connectionString);
        }

        public bool user_exists_in_db(long chat_id)
        {
            bool exists = false;

            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";

            string strSQL = $"SELECT COUNT(*) FROM {db_table_name} WHERE {db_table_chat_id}={chat_id}";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(strSQL, conn))
                {
                    conn.Open();
                    exists = ((int)command.ExecuteScalar() > 0) ? true : false; 
                }
            }
            return exists;
        }

        public string recive_situation_id(long chat_id) // Завтра поменять SQL-запрос, чтобы он проверял, есть ли chat_id, указанный в запросе
        {
            OleDbConnection connection;
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";
            // Create a connection  
            connection = new OleDbConnection(connectionString);
            string recived_situation_id = string.Empty;

            string strSQL = $"SELECT {db_table_situation_id} FROM {db_table_name} WHERE {db_table_chat_id}={chat_id}";
            OleDbCommand command = new OleDbCommand(strSQL, connection);
            // Open the connection and execute the select command.  
            try
            {
                // Open connecton  
                connection.Open();
                // Execute command  
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("------------Original data----------------");
                    while (reader.Read())
                    {
                        var situation_id = reader[db_table_situation_id].ToString();
                        Console.WriteLine("{0}", situation_id);
                        recived_situation_id = situation_id;
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return recived_situation_id;
        }

        public void add_new_user(long user_id)
        {
            for (var i = 1; i <= 30; i++)
            {
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";

                string strSQL = $"INSERT INTO {db_table_name}({db_table_chat_id},{db_table_situation_id},{db_table_situation_done}) VALUES({user_id}, {i}, FALSE)";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    using (OleDbCommand command = new OleDbCommand(strSQL, connection))
                    {
                        int number = command.ExecuteNonQuery();
                        Console.WriteLine("Добавлено объектов: {0}", number);


                        connection.Close();
                    }
                }
            }
        }

        public bool is_situation_done(long chat_id, int id_situation)
        {
            OleDbConnection connection;

            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";
            // Create a connection  
            connection = new OleDbConnection(connectionString);
            bool is_situation_done = false;

            string strSQL = $"SELECT situation_done FROM {db_table_name} WHERE {db_table_chat_id}={chat_id} AND {db_table_situation_id}={id_situation}";
            OleDbCommand command = new OleDbCommand(strSQL, connection);
            // Open the connection and execute the select command.  
            try
            {
                // Open connecton  
                connection.Open();
                // Execute command
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("------------Original data----------------");
                    while (reader.Read())
                    {
                        bool description = Convert.ToBoolean(reader["situation_done"]);
                        Console.WriteLine("{0}", description);
                        is_situation_done = description;
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return is_situation_done;
        }

        public void situation_done(long chat_id, int id_situation)
        {
            OleDbConnection connection;

            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";
            // Create a connection  
            connection = new OleDbConnection(connectionString);

            string strSQL = $"UPDATE Users SET situation_done=True WHERE {db_table_chat_id}={chat_id} AND {db_table_situation_id}={id_situation}";
            OleDbCommand command = new OleDbCommand(strSQL, connection);
            // Open the connection and execute the select command.  
            try
            {
                // Open connecton  
                connection.Open();
                Console.WriteLine("------------Original data----------------");
                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
