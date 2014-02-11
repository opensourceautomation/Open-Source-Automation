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

        public static void AddScriptProcessor(string Name, string pluginName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_script_processor_add (@pname, @ppluginname)";
                command.Parameters.AddWithValue("@pname", Name);
                command.Parameters.AddWithValue("@ppluginname", pluginName);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - AddScriptProcessor error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
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
        /// <param name="scriptID">the id of the script to be done</param>
        /// <returns>the name of the plugin that should process the script</returns>
        public static string GetDestinationScriptProcessor(int scriptID)
        {
            // if we don't get a value back it's likely the default script processor
            string scriptProcessorName = "Script Processor";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                {
                    MySqlCommand command = new MySqlCommand("CALL osae_sp_script_processor_by_script_id (@ScriptId)", connection);
                    command.Parameters.AddWithValue("@ScriptId", scriptID);
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        scriptProcessorName = reader.GetString("script_processor_plugin_name");
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
        public static void ObjectEventScriptAdd(string name, string eventName, int scriptID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_event_script_add (@Name, @Event, @ScriptID)";
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Event", eventName);
                command.Parameters.AddWithValue("@ScriptID", scriptID);

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

        public static void ObjectEventScriptDelete(string eventScriptID)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = "CALL osae_sp_object_event_script_delete (@peventscriptid)";
            command.Parameters.AddWithValue("@peventscriptid", eventScriptID);

            try
            {
                OSAESql.RunQuery(command);
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - ObjectEventScriptDelete error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }

        public static void ObjectTypeEventScriptAdd(string name, string eventName, int scriptID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_event_script_add (@Name, @Event, @ScriptID)";
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Event", eventName);
                command.Parameters.AddWithValue("@ScriptID", scriptID);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeEventScriptAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void ObjectTypeEventScriptDelete(string eventScriptID)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = "CALL osae_sp_object_type_event_script_delete (@pobjtypeeventscriptid)";
            command.Parameters.AddWithValue("@pobjtypeeventscriptid", eventScriptID);

            try
            {
                OSAESql.RunQuery(command);
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - ObjectTypeEventScriptDelete error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }

        public static void PatternAdd(string name)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_pattern_add (@Name)";
                command.Parameters.AddWithValue("@Name", name);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - PatterAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static bool PatternDelete(string name)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_pattern_delete (@Name)";
                command.Parameters.AddWithValue("@Name", name);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - PatternDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                    return false;
                }
                return true;
            }
        }

        public static void PatternMatchDelete(string name)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_pattern_match_delete (@Name)";
                command.Parameters.AddWithValue("@Name", name);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - PatternMatchDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void PatternMatchAdd(string pattern, string match)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_pattern_match_add (@Pattern, @Match)";
                command.Parameters.AddWithValue("@Match", match);
                command.Parameters.AddWithValue("@Pattern", pattern);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - PatternMatchAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void PatternScriptAdd(string name, int scriptID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_pattern_script_add (@Name, @ScriptID)";
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@ScriptID", scriptID);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - PatternScriptAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void PatternScriptDelete(string patternScriptID)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = "CALL osae_sp_pattern_script_delete (@ppatternscriptid)";
            command.Parameters.AddWithValue("@ppatternscriptid", patternScriptID);

            try
            {
                OSAESql.RunQuery(command);
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - PatternScriptDelete error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }


        public static void ScriptAdd(string name, string scriptProcessor, string script)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_script_add (@pname, @pscriptprocessor, @pscript)";
                command.Parameters.AddWithValue("@pname", name);
                command.Parameters.AddWithValue("@pscriptprocessor", scriptProcessor);
                command.Parameters.AddWithValue("@pscript", script);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ScriptAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void ScriptUpdate(string oldName, string name, string scriptProcessor, string script)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_script_update (@poldname, @pname, @pscriptprocessor, @pscript)";
                command.Parameters.AddWithValue("@poldname", oldName);
                command.Parameters.AddWithValue("@pname", name);
                command.Parameters.AddWithValue("@pscriptprocessor", scriptProcessor);
                command.Parameters.AddWithValue("@pscript", script);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ScriptUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }
        
        public static void ScriptDelete(string name)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_script_delete (@pname)";
                command.Parameters.AddWithValue("@pname", name);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ScriptDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static string GetScript(string scriptID)
        {
            string script = "";

            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {
                MySqlCommand command = new MySqlCommand("SELECT script FROM osae_script WHERE script_id = " + scriptID.ToString(), connection);
                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    script = reader.GetString("script");
                }
            }

            return script;
        }

        public static string GetScriptByName(string scriptName)
        {
            string script = "";

            using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
            {
                MySqlCommand command = new MySqlCommand("SELECT script FROM osae_script WHERE script_name = @ScriptName", connection);
                command.Parameters.AddWithValue("@ScriptName", scriptName);

                connection.Open();

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    script = reader.GetString("script");
                }
            }

            return script;
        }


        public static void RunPatternScript(string pattern, string parameter, string from)
        {
            List<int> scriptIDs = GetScriptsForPattern(pattern);

            foreach (int scriptID in scriptIDs)
            {
                OSAEMethodManager.MethodQueueAdd(GetDestinationScriptProcessor(scriptID), "RUN SCRIPT", scriptID.ToString(), parameter, from);
            }
        }

        public static void RunScript(string scriptname, string scriptparameter, string from)
        {

            int iScriptID = OSAEScriptManager.GetScriptID(scriptname);

            OSAEMethodManager.MethodQueueAdd(GetDestinationScriptProcessor(iScriptID), "RUN SCRIPT", iScriptID.ToString(), scriptparameter, from);

        }

        public static List<int> GetScriptsForPattern(string pattern)
        {
            List<int> scriptIDs = new List<int>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                {
                    MySqlCommand command = new MySqlCommand("CALL osae_sp_pattern_scripts_get (@Pattern)", connection);
                    command.Parameters.AddWithValue("@Pattern", pattern);
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        scriptIDs.Add(Int32.Parse(reader.GetString("script_id")));
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.GetLogger().AddToLog("Failed to get pattern scripts, error detals: \r\n" + exc.Message, true);
            }

            return scriptIDs;        
        }

        public static int GetScriptID(string scriptname)
        {
// int scriptIDs = new List<int>();
            int iScriptID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                {
                    MySqlCommand command = new MySqlCommand("SELECT script_id FROM osae_script WHERE script_name=@pscriptname", connection);
                    command.Parameters.AddWithValue("@pscriptname", scriptname);
                    connection.Open();

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        iScriptID = Int32.Parse(reader.GetString("script_id"));
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.GetLogger().AddToLog("Failed to get script id, error detals: \r\n" + exc.Message, true);
            }

            return iScriptID;
        }
    }
}
