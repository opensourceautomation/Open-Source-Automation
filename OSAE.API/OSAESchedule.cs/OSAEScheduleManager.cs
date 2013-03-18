namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MySql.Data.MySqlClient;

    public class OSAEScheduleManager
    {
        private Logging logging = Logging.GetLogger();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleDate"></param>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="pattern"></param>
        /// <param name="recurringID"></param>
        public static void ScheduleQueueAdd(DateTime scheduleDate, string obj, string method, string parameter1, string parameter2, string script, int recurringID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_queue_add(@ScheduleDate, @Object, @Method, @Parameter1, @Parameter2, @Script, @RecurringID)";
                command.Parameters.AddWithValue("@ScheduleDate", scheduleDate);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Script", script);
                command.Parameters.AddWithValue("@RecurringID", recurringID);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ScheduleQueueAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueID"></param>
        public static void ScheduleQueueDelete(int queueID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_queue_delete(@QueueID)";
                command.Parameters.AddWithValue("@QueueID", queueID);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ScheduleQueueDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleName"></param>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="pattern"></param>
        /// <param name="recurringTime"></param>
        /// <param name="sunday"></param>
        /// <param name="monday"></param>
        /// <param name="tuesday"></param>
        /// <param name="wednesday"></param>
        /// <param name="thursday"></param>
        /// <param name="friday"></param>
        /// <param name="saturday"></param>
        /// <param name="interval"></param>
        /// <param name="recurringDay"></param>
        /// <param name="recurringDate"></param>
        public static void ScheduleRecurringAdd(string scheduleName, string obj, string method, string parameter1, string parameter2, string script,
            string recurringTime, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday,
            string interval, int recurringMinutes, string recurringDay, string recurringDate)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_add(@ScheduleName, @Object, @Method, @Parameter1, @Parameter2, ";
                command.CommandText = command.CommandText + "@Script, @RecurringTime, @Sunday, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, ";
                command.CommandText = command.CommandText + "@Saturday, @Interval, @RecurringMinutes, @RecurringDay, @RecurringDate)";
                command.Parameters.AddWithValue("@ScheduleName", scheduleName);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Script", script);
                command.Parameters.AddWithValue("@RecurringTime", recurringTime);
                command.Parameters.AddWithValue("@Sunday", sunday);
                command.Parameters.AddWithValue("@Monday", monday);
                command.Parameters.AddWithValue("@Tuesday", tuesday);
                command.Parameters.AddWithValue("@Wednesday", wednesday);
                command.Parameters.AddWithValue("@Thursday", thursday);
                command.Parameters.AddWithValue("@Friday", friday);
                command.Parameters.AddWithValue("@Saturday", saturday);
                command.Parameters.AddWithValue("@Interval", interval);
                command.Parameters.AddWithValue("@RecurringMinutes", recurringMinutes);
                command.Parameters.AddWithValue("@RecurringDay", recurringDay);
                command.Parameters.AddWithValue("@RecurringDate", recurringDate);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ScheduleRecurringAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleName"></param>
        public static void ScheduleRecurringDelete(string scheduleName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_delete(@ScheduleName)";
                command.Parameters.AddWithValue("@ScheduleName", scheduleName);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ScheduleRecurringDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldScheduleName"></param>
        /// <param name="newScheduleName"></param>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="pattern"></param>
        /// <param name="recurringTime"></param>
        /// <param name="sunday"></param>
        /// <param name="monday"></param>
        /// <param name="tuesday"></param>
        /// <param name="wednesday"></param>
        /// <param name="thursday"></param>
        /// <param name="friday"></param>
        /// <param name="saturday"></param>
        /// <param name="interval"></param>
        /// <param name="recurringDay"></param>
        /// <param name="recurringDate"></param>
        public static void ScheduleRecurringUpdate(string oldScheduleName, string newScheduleName, string obj, string method, string parameter1, string parameter2, string script,
            string recurringTime, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday,
            string interval, int recurringMinutes, string recurringDay, string recurringDate)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_update(@OldScheduleName, @NewScheduleName, @Object, @Method, @Parameter1, @Parameter2, ";
                command.CommandText = command.CommandText + "@Script, @RecurringTime, @Sunday, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, ";
                command.CommandText = command.CommandText + "@Saturday, @Interval, @RecurringMinutes, @RecurringDay, @RecurringDate)";
                command.Parameters.AddWithValue("@OldScheduleName", oldScheduleName);
                command.Parameters.AddWithValue("@NewScheduleName", newScheduleName);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Script", script);
                command.Parameters.AddWithValue("@RecurringTime", recurringTime);
                command.Parameters.AddWithValue("@Sunday", sunday);
                command.Parameters.AddWithValue("@Monday", monday);
                command.Parameters.AddWithValue("@Tuesday", tuesday);
                command.Parameters.AddWithValue("@Wednesday", wednesday);
                command.Parameters.AddWithValue("@Thursday", thursday);
                command.Parameters.AddWithValue("@Friday", friday);
                command.Parameters.AddWithValue("@Saturday", saturday);
                command.Parameters.AddWithValue("@Interval", interval);
                command.Parameters.AddWithValue("@RecurringMinutes", recurringMinutes);
                command.Parameters.AddWithValue("@RecurringDay", recurringDay);
                command.Parameters.AddWithValue("@RecurringDate", recurringDate);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ScheduleRecurringUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void RunScheduledMethods()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_run_scheduled_methods";
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - RunScheduledMethods error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ProcessRecurring()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_process_recurring";
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ProcessRecurring error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }
    }
}
