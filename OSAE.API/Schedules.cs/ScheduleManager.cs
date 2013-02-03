namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MySql.Data.MySqlClient;

    class ScheduleManager
    {
        Logging logging = Logging.GetLogger();
        OSAE osae = new OSAE("");
        
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
        public void ScheduleQueueAdd(DateTime scheduleDate, string obj, string method, string parameter1, string parameter2, string pattern, int recurringID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_queue_add(@ScheduleDate, @Object, @Method, @Parameter1, @Parameter2, @Pattern, @RecurringID)";
                command.Parameters.AddWithValue("@ScheduleDate", scheduleDate);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Pattern", pattern);
                command.Parameters.AddWithValue("@RecurringID", recurringID);

                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("ScheduleQueueAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueID"></param>
        public void ScheduleQueueDelete(int queueID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_queue_delete(@QueueID)";
                command.Parameters.AddWithValue("@QueueID", queueID);

                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("ScheduleQueueDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
        public void ScheduleRecurringAdd(string scheduleName, string obj, string method, string parameter1, string parameter2, string pattern,
            string recurringTime, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday,
            string interval, int recurringMinutes, string recurringDay, string recurringDate)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_add(@ScheduleName, @Object, @Method, @Parameter1, @Parameter2, ";
                command.CommandText = command.CommandText + "@Pattern, @RecurringTime, @Sunday, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, ";
                command.CommandText = command.CommandText + "@Saturday, @Interval, @RecurringMinutes, @RecurringDay, @RecurringDate)";
                command.Parameters.AddWithValue("@ScheduleName", scheduleName);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Pattern", pattern);
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
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("ScheduleRecurringAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleName"></param>
        public void ScheduleRecurringDelete(string scheduleName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_delete(@ScheduleName)";
                command.Parameters.AddWithValue("@ScheduleName", scheduleName);

                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("ScheduleRecurringDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
        public void ScheduleRecurringUpdate(string oldScheduleName, string newScheduleName, string obj, string method, string parameter1, string parameter2, string pattern,
            string recurringTime, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday,
            string interval, int recurringMinutes, string recurringDay, string recurringDate)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_update(@OldScheduleName, @NewScheduleName, @Object, @Method, @Parameter1, @Parameter2, ";
                command.CommandText = command.CommandText + "@Pattern, @RecurringTime, @Sunday, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, ";
                command.CommandText = command.CommandText + "@Saturday, @Interval, @RecurringMinutes, @RecurringDay, @RecurringDate)";
                command.Parameters.AddWithValue("@OldScheduleName", oldScheduleName);
                command.Parameters.AddWithValue("@NewScheduleName", newScheduleName);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Pattern", pattern);
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
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("ScheduleRecurringUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RunScheduledMethods()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_run_scheduled_methods";
                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - RunScheduledMethods error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessRecurring()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_process_recurring";
                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - ProcessRecurring error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

    }
}
