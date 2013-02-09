namespace OSAE
{
    using System;
    using System.Data;
    using System.Net;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// API used to interact with the various components of OSA
    /// </summary>
    [Serializable]
    public partial class OSAE
    {
        #region Properties

        private string _parentProcess;

        /// <summary>
        /// Used to get access to the logging facility
        /// </summary>
        private Logging logging = Logging.GetLogger();
    
        public string ComputerName  { get; set; }
      
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parentProcess">The parent process</param>
        public OSAE(string parentProcess)
        {            
            ComputerName = Dns.GetHostName();
            _parentProcess = parentProcess;
        }        
        
        /// <summary>
        /// Add a new event script for an object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="event"></param>
        /// <param name="scriptText"></param>
        public void ObjectEventScriptAdd(string name, string eventName, string scriptText)
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
                    logging.AddToLog("API - ObjectEventScriptAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an event script
        /// </summary>
        /// <param name="name"></param>
        /// <param name="event"></param>
        /// <param name="scriptText"></param>
        public void ObjectEventScriptUpdate(string name, string eventName, string scriptText)
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
                logging.AddToLog("API - ObjectEventScriptUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }
                   
        /// <summary>
		/// CALL osae_sp_pattern_parse(pattern) and returns result
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string PatternParse(string pattern)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();
            try
            {
                command.CommandText = "CALL osae_sp_pattern_parse(@Pattern)";
                command.Parameters.AddWithValue("@Pattern", pattern);
                dataset = OSAESql.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    return dataset.Tables[0].Rows[0]["vInput"].ToString();
                }
                else
                {
                    return pattern;
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - PatternParse error: " + ex.Message, true);
                return string.Empty;
            }
        }

        public string MatchPattern(string str)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();

            try
            {
                command.CommandText = "SELECT pattern FROM osae_v_pattern WHERE `match`=@Name";
                command.Parameters.AddWithValue("@Name", str);
                dataset = OSAESql.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    return dataset.Tables[0].Rows[0]["pattern"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - MatchPattern error: " + ex.Message, true);
                return string.Empty;
            }
        }

        public void NamedScriptUpdate(string name, string oldName, string scriptText)
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
                logging.AddToLog("API - NamedScriptUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }
    }
}


