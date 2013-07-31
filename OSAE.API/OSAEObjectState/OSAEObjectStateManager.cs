namespace OSAE
{
    using System;
    using System.Data;
    using MySql.Data.MySqlClient;

    public class OSAEObjectStateManager
    {
        /// <summary>
        /// Set the state of an object
        /// </summary>
        /// <param name="ObjectName">The name of the object to set the state of</param>
        /// <param name="State">The state to set the object too</param>
        /// <param name="source">Where the message was genreated from e.g. the plugin name (pName)</param>
        public static void ObjectStateSet(string ObjectName, string State, string source)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_state_set(@ObjectName, @State, @FromObject, @DebugInfo)";
                command.Parameters.AddWithValue("@ObjectName", ObjectName);
                command.Parameters.AddWithValue("@State", State);
                command.Parameters.AddWithValue("@FromObject", PluginManager.GetPluginName(source, Common.ComputerName));
                command.Parameters.AddWithValue("@DebugInfo", null);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ObjectStateSet error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }           

        /// <summary>
        /// Returns a OSAEObjectState object
        /// </summary>
        /// <param name="ObjectName">The object name to get the state of</param>
        /// <returns>The state of the requested object</returns>
        public static OSAEObjectState GetObjectStateValue(string ObjectName)
        {
            DataSet dataset = new DataSet();
            OSAEObjectState state = new OSAEObjectState();

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "SELECT state_name, coalesce(time_in_state, 0) as time_in_state, COALESCE(last_state_change,NOW()) as last_state_change FROM osae_v_object WHERE object_name=@ObjectName";
                    command.Parameters.AddWithValue("@ObjectName", ObjectName);
                    dataset = OSAESql.RunQuery(command);
                }

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    state.Value = dataset.Tables[0].Rows[0]["state_name"].ToString();
                    state.TimeInState = Convert.ToInt64(dataset.Tables[0].Rows[0]["time_in_state"]);
                    state.LastStateChange = Convert.ToDateTime(dataset.Tables[0].Rows[0]["last_state_change"]);
                    return state;
                }
                else
                {
                    Logging.GetLogger().AddToLog("API - GetObjectStateValue error: Object does not exist | ObjectName: " + ObjectName, true);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - GetObjectStateValue error: " + ex.Message + " | ObjectName: " + ObjectName, true);
                return null;
            }
            finally
            {
                dataset.Dispose();
            }
        }

        public static DataSet ObjectStateHistoryGet(string objectName, string from, string to)
        {
            DataSet ds = new DataSet();
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "SELECT history_timestamp, object_name, state_label FROM osae_v_object_state_change_history WHERE object_name = '" + objectName + "' AND history_timestamp BETWEEN '" + from + "' AND '" + to + "' ORDER BY history_timestamp asc";
                try
                {
                    ds = OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectStateHistoryGet error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
            return ds;
        }
    }
}
