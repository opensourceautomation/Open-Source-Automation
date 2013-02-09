namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MySql.Data.MySqlClient;

    public class ScriptManager
    {
        /// <summary>
        /// Add a new event script for an object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="event"></param>
        /// <param name="scriptText"></param>
        public static void ObjectEventScriptAdd(string name, string eventName, string scriptText)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_event_script_add (@Name, @Event, @ScriptText)";
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Event", eventName);
                command.Parameters.AddWithValue("@ScriptText", scriptText);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectEventScriptAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an event script
        /// </summary>
        /// <param name="name"></param>
        /// <param name="event"></param>
        /// <param name="scriptText"></param>
        public static void ObjectEventScriptUpdate(string name, string eventName, string scriptText)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = "CALL osae_sp_object_event_script_update (@Name, @Event, @ScriptText)";
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Event", eventName);
            command.Parameters.AddWithValue("@ScriptText", scriptText);

            try
            {
                OSAESql.RunQuery(command);
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - ObjectEventScriptUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }

        public static void NamedScriptUpdate(string name, string oldName, string scriptText)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = "CALL osae_sp_pattern_update (@poldpattern, @pnewpattern, @pscript)";
            command.Parameters.AddWithValue("@poldpattern", oldName);
            command.Parameters.AddWithValue("@pnewpattern", name);
            command.Parameters.AddWithValue("@pscript", scriptText);

            try
            {
                OSAESql.RunQuery(command);
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - NamedScriptUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }
    }
}
