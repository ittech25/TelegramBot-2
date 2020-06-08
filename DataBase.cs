using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace TelegramBot
{
    class DataBase
    { 

        readonly string db_table_name = "Situations";
        readonly string db_table_id_situations = "id_situations";
        readonly string db_table_id_choice = "id_choice";
        readonly string db_table_description = "description";

        //OleDbConnection connection;


        public DataBase()
        {
            OleDbConnection connection;
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";            
            // Create a connection  
            connection = new OleDbConnection(connectionString);
            // Create a command and set its connection  
            
            // The connection is automatically closed becasuse of using block.  
            

            //OleDbDataAdapter adapter = new OleDbDataAdapter($"SELECT {db_table_description} FROM {db_table_name} WHERE {db_table_id_situations}=1", connection);
            //OleDbCommand cmd = connection.CreateCommand();
            //connection.Open();
            //cmd.CommandText = $"SELECT {db_table_description} FROM {db_table_name} WHERE {db_table_id_situations}=1";
            //cmd.Connection = connection;
            //cmd.ExecuteNonQuery();
            //connection.Close();
            //command.ExecuteNonQuery();
        }

        public string recive_text(string id_situation, string id_choice = "1")
        {
            OleDbConnection connection;
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";
            // Create a connection  
            connection = new OleDbConnection(connectionString);
            string recived_desription = "None";

            string strSQL = $"SELECT {db_table_description} FROM {db_table_name} WHERE {db_table_id_situations}={id_situation} AND {db_table_id_choice}=\"{id_choice}\"";
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
                        var description = reader[db_table_description].ToString();
                        Console.WriteLine("{0}", description);
                        recived_desription = description;
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return recived_desription;
        }

        public string[] get_childs(string id, string choice = "1")
        {
            OleDbConnection connection;
            string[] childs = new string[0];

            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Emergencies.accdb;Persist Security Info=True";
            // Create a connection  
            connection = new OleDbConnection(connectionString);

            string strSQL = $"SELECT id_child FROM {db_table_name} WHERE {db_table_id_situations}={id} AND {db_table_id_choice}=\"{choice}\"";
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
                        var description = reader["id_child"].ToString();
                        Console.WriteLine("{0}", description);
                        childs = description.Split(',');
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return childs;
        }

    }
}
