namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using MySql.Data.MySqlClient;

    public class ObjectStateManager
    {
        /// <summary>
        /// Set the state of an object
        /// </summary>
        /// <param name="ObjectName"></param>
        /// <param name="State"></param>
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
        /// Returns a ObjectState object
        /// </summary>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        public static ObjectState GetObjectStateValue(string ObjectName)
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
    }
}
