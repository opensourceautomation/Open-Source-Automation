namespace OSAE
{
    using System;
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Security;
    using System.Security.Policy;
    ///using MySql.Data.MySqlClient;
    using MySql.Data.MySqlClient;
    using OSAE.General;

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
            get { return Dns.GetHostName(); }
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
        /// The address of the WCF server
        /// </summary>
        public static string WcfServer
        {
            get
            {
                ModifyRegistry registry = new ModifyRegistry();
                registry.SubKey = "SOFTWARE\\OSAE\\";
                return registry.Read("WCFSERVER");
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
                    "SslMode = none;" +
                    "PASSWORD=" + registry.Read("DBPASSWORD") + ";Allow User Variables=True;";

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
        public static DBConnectionStatus TestConnection()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                { connection.Open(); }
            }
            catch (Exception ex)
            { return new DBConnectionStatus(false, ex); }

            return new DBConnectionStatus(true, null);
        }

        /// <summary>
        /// CALL osae_sp_pattern_parse(pattern) and returns result
        /// This is parsing OUTPUT and not the OSA Patterns, it should be renamed...
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
                        return dataset.Tables[0].Rows[0]["vInput"].ToString();
                    else
                        return pattern;
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - PatternParse error: " + ex.Message, true);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get all object names that start with a single word
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static DataSet ObjectNamesStartingWith(string pattern)
        {
                using (MySqlCommand command = new MySqlCommand())
                {
                   // DataSet dataset = new DataSet();
                   // command.CommandText = "SELECT object_name FROM osae_object WHERE UPPER(object_name) LIKE '@Pattern%' ORDER BY Length(object_name) DESC";
                  //  command.Parameters.AddWithValue("@Pattern", pattern.ToUpper());
                  //  dataset = OSAESql.RunQuery(command);
                  //  return dataset;
                    DataSet dataset = new DataSet();
                    //command.CommandText = "SELECT object_name FROM osae_object WHERE UPPER(object_name) LIKE '@Pattern%' ORDER BY Length(object_name) DESC";
                    //command.Parameters.AddWithValue("@Pattern", pattern.ToUpper());
                    dataset = OSAESql.RunSQL("SELECT object_name FROM osae_object WHERE (UPPER(object_name) LIKE UPPER('" + pattern.Replace("'", "''") + "%') OR UPPER(object_alias) LIKE UPPER('" + pattern.Replace("'", "''") + "%')) ORDER BY Length(object_name) DESC");
                    return dataset;
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
                // Exception handling should be handled inside calling application
                throw new Exception("Error getting registry settings and/or deleting logs: " + ex.Message, ex);
            }
        }

        public static string GetComputerIP()
        {
            IPHostEntry ipEntry = Dns.GetHostByName(Common.ComputerName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[0].ToString();
        }

        public static void CheckComputerObject(string sourceName)
        {
            Logging.GetLogger().AddToLog("Checking for Computer object", true);
            string computerIp = Common.GetComputerIP();

            if (OSAEObjectManager.GetObjectByName(Common.ComputerName) == null)
            {                                  
                OSAEObject obj = OSAEObjectManager.GetObjectByAddress(computerIp);
                if (obj == null)
                {
                    Logging.GetLogger().AddToLog("Computer Object not found, creating it...", true);
                    OSAEObjectManager.ObjectAdd(Common.ComputerName,"", Common.ComputerName, "COMPUTER", computerIp, "", 30, true);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                    Logging.GetLogger().AddToLog("Computer Object created called: " + Common.ComputerName, true);
                }
                else if (obj.Type == "COMPUTER")
                {
                    Logging.GetLogger().AddToLog("Computer Object found under a different name, updating it...", true);
                    OSAEObjectManager.ObjectUpdate(obj.Name, Common.ComputerName, obj.Alias, obj.Description, "COMPUTER", computerIp, obj.Container, obj.MinTrustLevel, obj.Enabled);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else
                {
                    Logging.GetLogger().AddToLog("Computer Object found under a different Name and Object Type, updating it...", true);
                    OSAEObjectManager.ObjectAdd(Common.ComputerName, "", Common.ComputerName, "COMPUTER", computerIp, string.Empty, obj.MinTrustLevel, true);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName + "." + computerIp, "Host Name", Common.ComputerName, sourceName);
                }
            }
            else
            {
                Logging.GetLogger().AddToLog("Computer Object found, updating it...", true);
                OSAEObject obj = OSAEObjectManager.GetObjectByName(Common.ComputerName);
                OSAEObjectManager.ObjectUpdate(obj.Name, obj.Name, obj.Alias, obj.Description, "COMPUTER", computerIp, obj.Container, obj.MinTrustLevel, obj.Enabled);
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

        public static long GetJavascriptTimestamp(System.DateTime input)
        {
            return (long)input.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
