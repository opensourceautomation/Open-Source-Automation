using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace OSAE
{
    public partial class OSAE
	{        
        [System.Obsolete("Use ObjectPropertyArrayGetRandom")]
        public string GetListItem(string objName, string PropertyName)
        {
            DataSet dataset = new DataSet();

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "SELECT item_name FROM osae_v_object_property_array WHERE object_name=@ObjectName AND property_name=@PropertyName ORDER BY RAND() LIMIT 1;";
                    command.Parameters.AddWithValue("@ObjectName", objName);
                    command.Parameters.AddWithValue("@PropertyName", PropertyName);
                    dataset = RunQuery(command);
                }

                if (dataset.Tables[0].Rows.Count > 0)
                    return dataset.Tables[0].Rows[0]["item_name"].ToString();
                else
                    return "";
            }
            catch (Exception ex)
            {
                AddToLog("API - GetListItem error: " + ex.Message, true);
                return "";
            }
        }

        [System.Obsolete("use GetObjectPropertyValue")]
        public string GetObjectProperty(string ObjectName, string ObjectProperty)
        {
            DataSet dataset = new DataSet();

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "SELECT property_value, property_datatype FROM osae_v_object_property WHERE object_name=@ObjectName AND property_name=@ObjectProperty";
                    command.Parameters.AddWithValue("@ObjectName", ObjectName);
                    command.Parameters.AddWithValue("@ObjectProperty", ObjectProperty);
                    dataset = RunQuery(command);
                }

                if (dataset.Tables[0].Rows.Count > 0)
                    return dataset.Tables[0].Rows[0]["property_value"].ToString();
                else
                    return "";
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectProperty error: " + ex.Message, true);
                return "";
            }
        }

        [System.Obsolete("use GetObjectStateValue")]
        public string GetObjectState(string ObjectName)
        {
            DataSet dataset = new DataSet();

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "SELECT state_name FROM osae_v_object WHERE object_name=@ObjectName";
                    command.Parameters.AddWithValue("@ObjectName", ObjectName);
                    dataset = RunQuery(command);
                }

                return dataset.Tables[0].Rows[0]["state_name"].ToString();
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectState error: " + ex.Message, true);
                return "";
            }
        }
    }
}
