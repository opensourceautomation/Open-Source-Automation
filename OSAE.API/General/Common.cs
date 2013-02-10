namespace OSAE
{
    using System;
    using System.Data;
    using System.Net;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Common helper class for common functionality
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Gets the name of the computer running the code
        /// </summary>
        public static string ComputerName
        {
            get
            {
                return Dns.GetHostName();
            }
        }

        /// <summary>
        /// Gets the installation directory of OSAE
        /// </summary>
        public static string ApiPath
        {
            get
            {
                ModifyRegistry registry = new ModifyRegistry();
                registry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

                return registry.Read("INSTALLDIR");
            }
        }

        public static string DBName
        {
            get
            {
                ModifyRegistry registry = new ModifyRegistry();
                registry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

                return registry.Read("DBNAME");
            }
        }

        public static string DBPort
        {
            get
            {
                ModifyRegistry registry = new ModifyRegistry();
                registry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

                return registry.Read("DBPORT");
            }
        }

        public static string DBPassword
        {
            get
            {
                ModifyRegistry registry = new ModifyRegistry();
                registry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

                return registry.Read("DBPASSWORD");
            }
        }

        public static string DBUsername
        {
            get
            {
                ModifyRegistry registry = new ModifyRegistry();
                registry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

                return registry.Read("DBUSERNAME");
            }
        }
             
        /// <summary>
        /// Gets the connection string used to connect to the OSA DB
        /// </summary>
        /// <remarks>reads from the registry each time so that if the settings change the
        /// service doesn't need to be restarted. If this creates performance issues then it
        /// can be changed over to  a static string that reads once.</remarks>
        public static string ConnectionString
        {
            get
            {
                string connectionString = string.Empty;
                
                ModifyRegistry registry = new ModifyRegistry();
                registry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

                connectionString = "SERVER=" + registry.Read("DBCONNECTION") + ";" +
                    "DATABASE=" + registry.Read("DBNAME") + ";" +
                    "PORT=" + registry.Read("DBPORT") + ";" +
                    "UID=" + registry.Read("DBUSERNAME") + ";" +
                    "PASSWORD=" + registry.Read("DBPASSWORD") + ";";

                return connectionString;
            }
        }

        public static string DBConnection
        {
            get
            {
                string databaseConnection = string.Empty;

                ModifyRegistry registry = new ModifyRegistry();
                registry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";

                databaseConnection = registry.Read("DBCONNECTION");

                return databaseConnection;
            }
        }

        /// <summary>
        /// Test to see if we can get a successful connection to the DB
        /// </summary>
        /// <returns>True if connect success false otherwise</returns>
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
                Logging.GetLogger().AddToLog("API - Cannot run query - bad connection: ", true);
            }             

            return connectionStatus;
        }

        /// <summary>
        /// CALL osae_sp_pattern_parse(pattern) and returns result
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string PatternParse(string pattern)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    DataSet dataset = new DataSet();
                    command.CommandText = "CALL osae_sp_pattern_parse(@Pattern)";
                    command.Parameters.AddWithValue("@Pattern", pattern);
                    dataset = OSAESql.RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        return dataset.Tables[0].Rows[0]["vInput"].ToString();
                    }
                    else
                    {
                        return pattern;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - PatternParse error: " + ex.Message, true);
                return string.Empty;
            }
        }

        public static string MatchPattern(string str)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    DataSet dataset = new DataSet();
                    command.CommandText = "SELECT pattern FROM osae_v_pattern WHERE `match`=@Name";
                    command.Parameters.AddWithValue("@Name", str);
                    dataset = OSAESql.RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        return dataset.Tables[0].Rows[0]["pattern"].ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - MatchPattern error: " + ex.Message, true);
                return string.Empty;
            }
        }
    }
}
