namespace OSAE
{
    using System;
    using System.Data;
    using MySql.Data.MySqlClient;

    public class PluginManager
    {
        /// <summary>
        /// Returns a Dataset with all of the properties and their values for a plugin object
        /// </summary>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        public static DataSet GetPluginSettings(string ObjectName)
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
                    Logging.GetLogger().AddToLog("API - GetPluginSettings error: " + ex.Message, true);
                    return dataset;
                }
            }
        }

        /// <summary>
        /// Returns the name of the plugin object of the specified type on the scecified machine.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static string GetPluginName(string objectType, string machineName)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();

            try
            {
                command.CommandText = "SELECT * FROM osae_v_object_property WHERE object_type=@ObjectType AND property_name='Computer Name' AND (property_value IS NULL OR property_value='' OR property_value=@MachineName) LIMIT 1";
                command.Parameters.AddWithValue("@ObjectType", objectType);
                command.Parameters.AddWithValue("@MachineName", machineName);
                dataset = OSAESql.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                    return dataset.Tables[0].Rows[0]["object_name"].ToString();
                else
                {
                    command = new MySqlCommand();
                    command.CommandText = "SELECT * FROM osae_object WHERE object_name=@ObjectType2";
                    command.Parameters.AddWithValue("@ObjectType2", objectType);
                    dataset = OSAESql.RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                        return objectType;
                    else
                        return "";
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - GetPluginName error: " + ex.Message + " - objectType: " + objectType + " | machineName: " + machineName, true);
                return string.Empty;
            }
        }
    }
}
