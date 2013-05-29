namespace OSAE
{
    using System;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Security.Policy;
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

        public static void InitialiseLogFolder()
        {
            try
            {
                FileInfo file = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs\");
                file.Directory.Create();
                if (OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value == "TRUE")
                {
                    string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs\");
                    foreach (string f in files)
                        File.Delete(f);
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("Error getting registry settings and/or deleting logs: " + ex.Message, true);
            }
        }

        public static string GetComputerIP()
        {
            IPHostEntry ipEntry = Dns.GetHostByName(Common.ComputerName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[0].ToString();
        }

        public static void CreateComputerObject(string sourceName)
        {
            Logging.GetLogger().AddToLog("Creating Computer object", true);
            string computerIp = Common.GetComputerIP();

            if (OSAEObjectManager.GetObjectByName(Common.ComputerName) == null)
            {                                  
                OSAEObject obj = OSAEObjectManager.GetObjectByAddress(computerIp);
                if (obj == null)
                {
                    OSAEObjectManager.ObjectAdd(Common.ComputerName, Common.ComputerName, "COMPUTER", computerIp, string.Empty, true);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else if (obj.Type == "COMPUTER")
                {
                    OSAEObjectManager.ObjectUpdate(obj.Name, Common.ComputerName, obj.Description, "COMPUTER", computerIp, obj.Container, obj.Enabled);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else
                {
                    OSAEObjectManager.ObjectAdd(Common.ComputerName + "." + computerIp, Common.ComputerName, "COMPUTER", computerIp, string.Empty, true);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName + "." + computerIp, "Host Name", Common.ComputerName, sourceName);
                }
            }
            else
            {
                OSAEObject obj = OSAEObjectManager.GetObjectByName(Common.ComputerName);
                OSAEObjectManager.ObjectUpdate(obj.Name, obj.Name, obj.Description, "COMPUTER", computerIp, obj.Container, obj.Enabled);
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Host Name", Common.ComputerName, sourceName);
            }
        }

        public static AppDomain CreateSandboxDomain(string name, string path, SecurityZone zone, Type item)
        {
            var setup = new AppDomainSetup { ApplicationBase = Common.ApiPath, PrivateBinPath = Path.GetFullPath(path) };

            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(zone));
            var permissions = SecurityManager.GetStandardSandbox(evidence);

            StrongName strongName = item.Assembly.Evidence.GetHostEvidence<StrongName>();

            return AppDomain.CreateDomain(name, null, setup);
        }
    }
}
