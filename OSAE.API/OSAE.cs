namespace OSAE
{
    using System;
    using System.Collections.Generic;
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
        /// Set the state of an object
        /// </summary>
        /// <param name="ObjectName"></param>
        /// <param name="State"></param>
        public void ObjectStateSet(string ObjectName, string State)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_state_set(@ObjectName, @State, @FromObject, @DebugInfo)";
                command.Parameters.AddWithValue("@ObjectName", ObjectName);
                command.Parameters.AddWithValue("@State", State);
                command.Parameters.AddWithValue("@FromObject", PluginManager.GetPluginName(_parentProcess, ComputerName));
                command.Parameters.AddWithValue("@DebugInfo", null);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("ObjectStateSet error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }                  
        
		/// <summary>
		/// Returns an ObjectProperty whcih contains the value, type, ID, last updated, and name
		/// </summary>
		/// <param name="ObjectName"></param>
		/// <param name="ObjectProperty"></param>
		/// <returns></returns>
		public ObjectProperty GetObjectPropertyValue(string ObjectName, string ObjectProperty)
		{
			MySqlCommand command = new MySqlCommand();
			DataSet dataset = new DataSet();
			try
			{
				command.CommandText = "SELECT object_property_id, property_name, property_value, property_datatype, last_updated FROM osae_v_object_property WHERE object_name=@ObjectName AND property_name=@ObjectProperty";
				command.Parameters.AddWithValue("@ObjectName", ObjectName);
				command.Parameters.AddWithValue("@ObjectProperty", ObjectProperty);
				dataset = OSAESql.RunQuery(command);

				if (dataset.Tables[0].Rows.Count > 0)
				{
					ObjectProperty p = new ObjectProperty();
					p.Id = dataset.Tables[0].Rows[0]["object_property_id"].ToString();
					p.DataType = dataset.Tables[0].Rows[0]["property_datatype"].ToString();
					p.LastUpdated = dataset.Tables[0].Rows[0]["last_updated"].ToString();
					p.Name = dataset.Tables[0].Rows[0]["property_name"].ToString();
					p.Value = dataset.Tables[0].Rows[0]["property_value"].ToString();

					return p;
				}
				else
				{
					ObjectProperty p = new ObjectProperty();
					p.Id = string.Empty;
					p.DataType = string.Empty;
					p.LastUpdated = string.Empty;
					p.Name = string.Empty;
					p.Value = string.Empty;

					return p;
				}
			}
			catch (Exception ex)
			{
				logging.AddToLog("API - GetObjectPropertyValue error: " + ex.Message, true);
				return null;
			}
		}

		/// <summary>
		/// Returns a ObjectState object
		/// </summary>
		/// <param name="ObjectName"></param>
		/// <returns></returns>
		public ObjectState GetObjectStateValue(string ObjectName)
		{
			DataSet dataset = new DataSet();
			ObjectState state = new ObjectState();
			try
			{
				using (MySqlCommand command = new MySqlCommand())
				{
					command.CommandText = "SELECT state_name, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE object_name=@ObjectName";
					command.Parameters.AddWithValue("@ObjectName", ObjectName);
					dataset = OSAESql.RunQuery(command);
				}

				if (dataset.Tables[0].Rows.Count > 0)
				{
					state.Value = dataset.Tables[0].Rows[0]["state_name"].ToString();
					state.TimeInState = Convert.ToInt64(dataset.Tables[0].Rows[0]["time_in_state"]);

					return state;
				}
				else
				{
					logging.AddToLog("API - GetObjectStateValue error: Object does not exist | ObjectName: " + ObjectName, true);
					return null;
				}
			}
			catch (Exception ex)
			{
				logging.AddToLog("API - GetObjectStateValue error: " + ex.Message + " | ObjectName: " + ObjectName, true);
				return null;
			}
			finally
			{
				dataset.Dispose();
			}
		}       
        
		/// <summary>
		/// Returns a Dataset with all of the properties and their values for a plugin object
		/// </summary>
		/// <param name="ObjectName"></param>
		/// <returns></returns>
		public DataSet GetPluginSettings(string ObjectName)
		{
			using (MySqlCommand command = new MySqlCommand())
			{
				DataSet dataset = new DataSet();
				try
				{
					command.CommandText = "SELECT * FROM osae_v_object_property WHERE object_name=@ObjectName";
					command.Parameters.AddWithValue("@ObjectName", ObjectName);
					dataset = OSAESql.RunQuery(command);

					return dataset;
				}
				catch (Exception ex)
				{
					logging.AddToLog("API - GetPluginSettings error: " + ex.Message, true);
					return dataset;
				}
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


