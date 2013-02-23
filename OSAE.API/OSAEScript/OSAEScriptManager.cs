namespace OSAE
{
    using System;
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;

    public class OSAEScriptManager
    {
        public static List<OSAEScriptProcessor> GetScriptProcessors()
        {            
            List<OSAEScriptProcessor> scriptProcessors = new List<OSAEScriptProcessor>();

            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {                
                MySqlCommand command = new MySqlCommand("SELECT script_processor_id, script_processor_name, script_processor_plugin_name FROM osae_script_processors", connection);
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OSAEScriptProcessor scriptProcessor = new OSAEScriptProcessor();

                    scriptProcessor.ID = reader.GetUInt32("script_processor_id");
                    scriptProcessor.Name = reader.GetString("script_processor_name");
                    scriptProcessor.PluginName = reader.GetString("script_processor_plugin_name");

                    scriptProcessors.Add(scriptProcessor);
                }
            }

            return scriptProcessors;
        }

        public static void AddScriptProcessor()
        {
            throw new NotImplementedException();
        }

        public static void DeleteScriptProcessor()
        {
            throw new NotImplementedException();
        }

        public static void UpdateScriptProcessor()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the name of the plugin that was meant to process the script
        /// </summary>
        /// <param name="osaeMethod">the method to be done</param>
        /// <returns>the name of the plugin that should process the script</returns>
        public static string GetDestinationScriptProcessor(OSAEMethod method)
        {
            // if we don't get a value back it's likely the default script processor
            string scriptProcessorName = "Script Processor";

            try
            {
                if (method.MethodName == "EVENT SCRIPT")
                {
                    using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                    {
                        MySqlCommand command = new MySqlCommand("CALL osae_sp_script_processor_by_pattern (@EventScriptId)", connection);
                        command.Parameters.AddWithValue("@EventScriptId", method.Parameter1);
                        connection.Open();

                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            scriptProcessorName = reader.GetString("script_processor_plugin_name");
                        }
                    }
                }
                else
                {
                    using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                    {
                        MySqlCommand command = new MySqlCommand("CALL osae_sp_script_processor_by_pattern (@Pattern)", connection);
                        command.Parameters.AddWithValue("@Pattern", method.Parameter1);
                        connection.Open();

                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            scriptProcessorName = reader.GetString("script_processor_plugin_name");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.GetLogger().AddToLog("Failed to get script processor, error detals: \r\n" + exc.Message, true);
            }            

            return scriptProcessorName;        
        }

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
