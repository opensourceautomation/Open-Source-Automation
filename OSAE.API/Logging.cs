namespace OSAE.API
{
    using System;
    using MySql.Data.MySqlClient;
    using System.IO;
    
    [Serializable]
    public class Logging
    {
        /// <summary>
        /// The parent process calling the class
        /// </summary>
        private string callingProcess;

        /// <summary>
        /// Provides exlusive writes to the log file
        /// </summary>
        private object logLocker = new object();

        /// <summary>
        /// Creates a new instance of the Logging class
        /// </summary>
        /// <param name="parentProcess">The name of the process creating the class 
        /// e.g. the name of the plugin using the class</param>
        public Logging(string parentProcess)
        {
            callingProcess = parentProcess;
        }

        /// <summary>
        /// Adds a message to the log
        /// </summary>
        /// <param name="audit"></param>
        /// <param name="alwaysLog"></param>
        public void AddToLog(string audit, bool alwaysLog)
        {
            try
            {
                OSAE osae = new OSAE("");

                if (osae.GetObjectPropertyValue("SYSTEM", "Debug").Value == "TRUE" || alwaysLog)
                {
                    lock (logLocker)
                    {
                        string filePath = Common.ApiPath + "/Logs/" + callingProcess + ".log";
                        System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                        file.Directory.Create();
                        StreamWriter sw = File.AppendText(filePath);
                        sw.WriteLine(System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + audit);
                        sw.Close();
                        if (osae.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value == "TRUE")
                        {
                            if (file.Length > 1000000)
                                file.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (logLocker)
                {
                    string filePath = Common.ApiPath + "/Logs/" + callingProcess + ".log";
                    System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                    file.Directory.Create();
                    StreamWriter sw = File.AppendText(filePath);
                    sw.WriteLine(System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - LOGGING ERROR: "
                        + ex.Message + " - " + ex.InnerException);
                    sw.Close();
                    if (file.Length > 1000000)
                        file.Delete();
                }
            }
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
                    OSAE osae = new OSAE("");
                    command.CommandText = "CALL osae_sp_debug_log_add (@Entry,@Process)";
                    command.Parameters.AddWithValue("@Entry", entry);
                    command.Parameters.AddWithValue("@Process", callingProcess);
                    osae.RunQuery(command);
                }
            }
            catch
            {
                // Not a lot we can do if it fails here
            }
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
                OSAE osae = new OSAE("");
                command.CommandText = "CALL osae_sp_event_log_add (@ObjectName, @EventName, @FromObject, @DebugInfo, @Param1, @Param2)";
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@EventName", eventName);
                command.Parameters.AddWithValue("@FromObject", osae.GetPluginName(callingProcess, Common.ComputerName));
                command.Parameters.AddWithValue("@DebugInfo", null);
                command.Parameters.AddWithValue("@Param1", parameter1);
                command.Parameters.AddWithValue("@Param2", parameter2);
                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - EventLogAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
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
                {
                    OSAE osae = new OSAE("");
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - EventLogClear error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }
    }
}
