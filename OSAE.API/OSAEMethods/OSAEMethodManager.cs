namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MySql.Data.MySqlClient;

    public class OSAEMethodManager
    {
        Logging logging = Logging.GetLogger();

        public void ClearMethodQueue()
        {
            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = "SET sql_safe_updates=0; DELETE FROM osae_method_queue;";
                OSAESql.RunQuery(command);
            }
        }
    }
}
