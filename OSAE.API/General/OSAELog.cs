

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using log4net.Config;
using log4net;
using MySql.Data.MySqlClient;

namespace OSAE.General
{
    
    [Serializable]
    public class OSAELog
    {
        private static ILog Log;

        public OSAELog(string name)
        {
            Log = LogManager.GetLogger(name);
            
            
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
            Log.Debug(log);
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

        public static DataSet Load()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_server_log_get";
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
    }
}
