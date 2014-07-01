

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using log4net.Config;
using log4net;
using MySql.Data.MySqlClient;
using OSAE;

namespace OSAE.General
{
    
    [Serializable]
    public class OSAELog
    {
        private static ILog Log;
        private Type logSource;
        private Boolean bDebug = false;
        public OSAELog()
        {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();
            logSource = method.DeclaringType;
            Log = LogManager.GetLogger(logSource);


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

        public void Info(string log)
        {
            Log.Info(log);
        }

        public void Debug(string log)
        {
            if (bDebug) Log.Debug(log);
        }

        public void Error(string log)
        {
            Log.Error(log);
        }

        public void Error(string log, Exception ex)
        {
            Log.Error(log, ex);
        }

        public void Fatal(string log)
        {
            Log.Fatal(log);
        }

        public void Fatal(string log, Exception ex)
        {
            Log.Fatal(log, ex);
        }

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
                {
                    return OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void Clear()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_server_log_clear";
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static DataSet LoadSources()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "SELECT DISTINCT Logger FROM osae_log";
                try
                {
                    return OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

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
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
