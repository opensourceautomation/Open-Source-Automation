namespace OSAE
{
    using System;
    using System.IO;
    using MySql.Data.MySqlClient;
    
    [Obsolete("Please use OSAELog class")]
    [Serializable]
    public class Logging
    {
        /// <summary>
        /// Private instance used as part of the Singleton pattern
        /// </summary>
         private static Logging privateInstance = null;

        /// <summary>
        /// Whether the system is in debug mode
        /// </summary>
         private static string debug;

        /// <summary>
        /// Whether the logs should be kept to a certain size
        /// </summary>
         private static string pruneLogs;

        /// <summary>
        /// Provides exlusive writes to the log file
        /// </summary>
        private static object logLocker = new object();

        /// <summary>
        /// The name of the log to write to
        /// </summary>
        private string logName = string.Empty;

        /// <summary>
        /// Memory lock to prevent access violation
        /// </summary>
        private static object memoryLock = new object();

        private Logging(string requestedLogName)
        {
            logName = requestedLogName;
        }

        /// <summary>
        /// Get the current logger, if no logger has been specified in a previous call then the 
        /// default logger will be returned
        /// </summary>
        /// <returns>provides access to the logging methods</returns>
        public static Logging GetLogger()
        {
            lock (memoryLock)
            {
                if (privateInstance == null) privateInstance = new Logging("Default");
            }
            Logging.GetConfiguration();
            return privateInstance;
        }

        public static Logging GetLogger(string requestedLogName)
        {
            lock (memoryLock)
            {
                if (privateInstance == null) privateInstance = new Logging(requestedLogName);
                else privateInstance.logName = requestedLogName;
            }
            Logging.GetConfiguration();
            return privateInstance;
        }

        public static void AddToLog(string audit, bool alwaysLog, string logFile)
        {
            try
            {
                if (debug != "FALSE" || alwaysLog)
                {
                    lock (logLocker)
                    {
                        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs\" + logFile + ".log";
                        System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                        file.Directory.Create();
                        StreamWriter sw = File.AppendText(filePath);

                        sw.WriteLine(System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + audit);
                        sw.Close();

                        if (pruneLogs == "TRUE")
                        {
                            if (file.Length > 1000000) file.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (logLocker)
                {
                    string filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/Logs/" + logFile + ".log";
                    System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                    file.Directory.Create();
                    StreamWriter sw = File.AppendText(filePath);
                    sw.WriteLine(System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - LOGGING ERROR: " + ex.Message + " - " + ex.InnerException);
                    sw.Close();
                    if (file.Length > 1000000) file.Delete();
                }
            }        
        }

        /// <summary>
        /// Adds a message to the log
        /// </summary>
        /// <param name="audit"></param>
        /// <param name="alwaysLog"></param>
        public void AddToLog(string audit, bool alwaysLog)
        {
            AddToLog(audit, alwaysLog, logName);
        }

        /// <summary>
        /// Add an entry to the degug table
        /// </summary>
        /// <param name="entry">String to add to the debug table</param>
        public void DebugLogAdd(string entry)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "CALL osae_sp_debug_log_add (@Entry,@Process)";
                    command.Parameters.AddWithValue("@Entry", entry);
                    command.Parameters.AddWithValue("@Process", logName);
                    OSAESql.RunQuery(command);
                }
            }
            catch { }
        }

        /// <summary>
        /// Add an entry to the event log table
        /// </summary>
        /// <param name="objectName">Object Name</param>
        /// <param name="eventName">Event Name</param>
        public void EventLogAdd(string objectName, string eventName, string parameter1 = null, string parameter2 = null)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_event_log_add (@ObjectName, @EventName, @FromObject, @DebugInfo, @Param1, @Param2)";
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@EventName", eventName);
                command.Parameters.AddWithValue("@FromObject", logName);
                command.Parameters.AddWithValue("@DebugInfo", null);
                command.Parameters.AddWithValue("@Param1", parameter1);
                command.Parameters.AddWithValue("@Param2", parameter2);
                try
                { OSAESql.RunQuery(command); }
                catch (Exception ex)
                { AddToLog("API - EventLogAdd error: " + command.CommandText + " - error: " + ex.Message, true); }
            }
        }

        /// <summary>
        /// Deletes everything from the event_log table
        /// </summary>
        public void EventLogClear()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_event_log_clear";
                try
                { OSAESql.RunQuery(command); }
                catch (Exception ex)
                { AddToLog("API - EventLogClear error: " + command.CommandText + " - error: " + ex.Message, true); }
            }
        }

        private static void GetConfiguration()
        {
            debug = OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "Debug").Value;
            pruneLogs = OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value;
        }
    }
}
