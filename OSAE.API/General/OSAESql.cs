namespace OSAE
{
    using System;
    using System.Data;
    using MySql.Data.MySqlClient;

    public class OSAESql
    {
        /// <summary>
        /// Runs the passed in SQL query on the database and returns a dataset of the results
        /// </summary>
        /// <param name="SQL">The SQL command to be run against the DB</param>
        /// <returns>The Dataset returned from the DB</returns>
        public static DataSet RunQuery(MySqlCommand command)
        {
            DataSet dataset = new DataSet();
            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {
                connection.Open();

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                command.Connection = connection;
                adapter.Fill(dataset);
            }
            

            return dataset;
        }

        /// <summary>
        /// Runs the passed in SQL query on the database and returns a dataset of the results
        /// </summary>
        /// <param name="SQL">The sql query to be run against the database</param>
        /// <returns>The dataset returned from the DB</returns>
        public static DataSet RunSQL(string sql)
        {
            DataSet dataset = new DataSet();

            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(sql);
                MySqlDataAdapter adapter;
                
                command.Connection = connection;
                adapter = new MySqlDataAdapter(command);
                adapter.Fill(dataset);
                
            }

            return dataset;
        }
    }
}
