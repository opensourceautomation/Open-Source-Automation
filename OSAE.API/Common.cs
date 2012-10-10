namespace OSAE.API
{
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Common helper class for common functionality
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Gets the connection string used to connect to the OSA DB
        /// </summary>
        /// <remarks>reads from the registry each time so that if the settings change the
        /// service doesn't need to be restarted. If this creates performence issues then it
        /// can be changed over to  a static string that reads once.</remarks>
        public static string ConnectionString
        {
            get
            {
                string connectionString = string.Empty;
                
                ModifyRegistry myRegistry = new ModifyRegistry();
                myRegistry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

                connectionString = "SERVER=" + myRegistry.Read("DBCONNECTION") + ";" +
                    "DATABASE=" + myRegistry.Read("DBNAME") + ";" +
                    "PORT=" + myRegistry.Read("DBPORT") + ";" +
                    "UID=" + myRegistry.Read("DBUSERNAME") + ";" +
                    "PASSWORD=" + myRegistry.Read("DBPASSWORD") + ";";

                return connectionString;
            }
        }

        /// <summary>
        /// Test to see if we can get a successfull connection to the DB
        /// </summary>
        /// <returns></returns>
        public static bool TestConnection()
        {
            bool connectionStatus = true;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                {
                    connection.Open();                    
                }
            }
            catch
            {
                connectionStatus = false;

                OSAE osae = new OSAE("OSAE.API.Common.TestConnection");
                osae.AddToLog("API - Cannot run query - bad connection: ", true);
            }             

            return connectionStatus;
        }
    }
}
