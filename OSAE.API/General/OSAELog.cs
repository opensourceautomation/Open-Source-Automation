using System;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using log4net;
using MySql.Data.MySqlClient;
using System.Threading;

namespace OSAE.General
{
    /// <summary>
    /// Public class used to hold an instance of the OSAELog system.
    /// </summary>
    [Serializable]
    public class OSAELog
    {
        private static ILog Log;
        private Type logSource;
        private bool bDebug = false;
        private bool bPrune = false;
        /// <summary>
        /// Used to create an instance of an OSAELog
        /// </summary>
        /// <param name="source">
        /// The name of the Log to create an instance of.</param>
        public OSAELog(string source)
        {
            StackFrame frame = new StackFrame(1);
            //MethodBase method = frame.GetMethod();
            //logSource = method.DeclaringType;
            Log = LogManager.GetLogger(source);

            DBConnectionStatus results = Common.TestConnection();
            //I will make this use a loop in the future if it fixes the problems with the service starting up
            if (!results.Success)
            {
                Thread.Sleep(5000);
                results = Common.TestConnection();
                if (!results.Success)
                {
                    Thread.Sleep(5000);
                    results = Common.TestConnection();
                    if (!results.Success)
                    {
                        Thread.Sleep(5000);
                        results = Common.TestConnection();
                        if (!results.Success) throw new Exception("DB Connection Test Failed!");
                    }
                }
            }

             try
            {
                bPrune = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value);
            }
            catch
            {
                bPrune = false;
            }

            bDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "Debug").Value);
            var root = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root;
            var attachable = root as log4net.Core.IAppenderAttachable;

            if (attachable != null)
            {
                log4net.Repository.Hierarchy.Hierarchy hier = log4net.LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;
                if (hier != null)
                {
                    var fileAppender =
                        (log4net.Appender.RollingFileAppender)hier.GetAppenders().Where(
                            appender => appender.Name.Equals("RollingLogFileAppender", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    var adoAppender =
                        (log4net.Appender.AdoNetAppender)hier.GetAppenders().Where(
                            appender => appender.Name.Equals("MySql_ADONetAppender", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (Common.TestConnection().Success)
                    {
                        if (adoAppender != null)
                        {
                            adoAppender.ConnectionString = Common.ConnectionString;
                            adoAppender.ActivateOptions();
                        }
                        root.RemoveAppender(fileAppender);
                    }
                }
            }
        }
        /// <summary>
        /// Appends information to log as "Standard Information"
        /// </summary>
        /// <param name="log">
        /// Sting text to append to log instance</param>
        public void Info(string log)
        {
            Log.Info(log);
        }

        /// <summary>
        /// Appends information to log as "Debug Information"
        /// </summary>
        /// <param name="log">
        /// Sting text to append to log instance</param>
        public void Debug(string log)
        {
            if (bDebug) Log.Debug(log);
        }

        /// <summary>
        /// Appends information to log as "Error Information"
        /// </summary>
        /// <param name="log">
        /// Sting text to append to log instance</param>
        public void Error(string log)
        {
            Log.Error(log);
        }

        /// <summary>
        /// Appends information to log as "Error Information" with the Exception text
        /// </summary>
        /// <param name="log">
        /// Sting text to append to log instance</param>
        /// <param name="ex">
        /// Exception text provided by MS .net</param>
        public void Error(string log, Exception ex)
        {
            Log.Error(log, ex);
        }

        /// <summary>
        /// Appends information to log as "Fatal Information" 
        /// </summary>
        /// <param name="log">
        /// Sting text to append to log instance</param>
        public void Fatal(string log)
        {
            Log.Fatal(log);
        }

        /// <summary>
        /// Appends information to log as "Fatal Information" with the Exception text
        /// </summary>
        /// <param name="log">
        /// Sting text to append to log instance</param>
        /// <param name="ex">
        /// Exception text provided by MS .net</param>
        public void Fatal(string log, Exception ex)
        {
            Log.Fatal(log, ex);
        }

        /// <summary>
        /// Used to flush the log4net buffers
        /// </summary>
        public static void FlushBuffers()
        {
            log4net.Repository.ILoggerRepository rep = LogManager.GetRepository();
            foreach (log4net.Appender.IAppender appender in rep.GetAppenders())
            {
                var buffered = appender as log4net.Appender.BufferingAppenderSkeleton;
                if (buffered != null)
                {
                    buffered.Flush();
                }
            }
        }

        public static DataSet Load(bool info, bool debug, bool error, string source)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_server_log_get (@pinfo, @pdebug, @perror, @psource)";
                command.Parameters.AddWithValue("@pinfo", info);
                command.Parameters.AddWithValue("@pdebug", debug);
                command.Parameters.AddWithValue("@perror", error);
                command.Parameters.AddWithValue("@psource", source);
                try
                { return OSAESql.RunQuery(command); }
                catch (Exception ex)
                { throw ex; }
            }
        }

        /// <summary>
        /// Clears ALL Server Log entries
        /// </summary>
        public static void Clear()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_server_log_clear";
                try
                { OSAESql.RunQuery(command); }
                catch (Exception ex)
                { throw ex; }
            }
        }

        /// <summary>
        /// Clears ALL Server log entries for the specified Server Log
        /// </summary>
        /// <param name="log">
        /// The name of the Server log to clear</param>
        public static void Clear_Log(string log)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_server_log_clear_logger(@log)";
                command.Parameters.AddWithValue("@log", log);
                try
                { OSAESql.RunQuery(command); }
                catch (Exception ex)
                { throw ex; }
            }
        }

        public static DataSet LoadSources()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "SELECT DISTINCT Logger FROM osae_log";
                try
                { return OSAESql.RunQuery(command); }
                catch (Exception ex)
                { throw ex; }
            }
        }

        public void PruneLogs()
        {
            if(bPrune)
            {
                if (GetTableSize("osae_log") > 10) Clear();
                if (GetTableSize("osae_event_log") > 10 || GetTableSize("osae_debug_log") > 10 || GetTableSize("osae_method_log") > 10) EventLogClear();
            }
        }

        private decimal GetTableSize(string tableName)
        {
            decimal size = 0;
            string sql;
            DataSet d;

            sql = "SELECT SUM( data_length + index_length) / 1024 / 1024 AS size FROM information_schema.TABLES WHERE table_schema = 'OSAE' AND TABLE_NAME = '" + tableName + "';";
            d = OSAESql.RunSQL(sql);
            size = (decimal)d.Tables[0].Rows[0]["size"];
            return size;
        }

        /// <summary>
        /// Used to add logging entries to the Debug Log
        /// </summary>
        /// <param name="entry">
        /// text to append to the Debug Log</param>
        public void DebugLogAdd(string entry)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "CALL osae_sp_debug_log_add (@Entry,@Process)";
                    command.Parameters.AddWithValue("@Entry", entry);
                    command.Parameters.AddWithValue("@Process", logSource.ToString());
                    OSAESql.RunQuery(command);
                }
            }
            catch
            {
                // Not a lot we can do if it fails here
            }
        }

        /// <summary>
        /// Deletes everything from the event_log table
        /// </summary>
        public static void EventLogClear()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_event_log_clear";
                try
                { OSAESql.RunQuery(command); }
                catch (Exception ex)
                { throw ex; }
            }
        }
    }
}
