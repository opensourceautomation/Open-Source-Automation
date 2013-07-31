namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Can be used to manager the properties of an object in OSA
    /// </summary>
    public class OSAEObjectPropertyManager
    {
        /// <summary>
        /// Returns an OSAEObjectProperty whcih contains the value, type, ID, last updated, and name
        /// </summary>
        /// <param name="ObjectName">The name of the object to get the property of</param>
        /// <param name="OSAEObjectProperty">The property to be retieved</param>
        /// <returns>the Property requested</returns>
        public static OSAEObjectProperty GetObjectPropertyValue(string ObjectName, string ObjectProperty)
        {           
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    DataSet dataset = new DataSet();

                    command.CommandText = "SELECT object_property_id, property_name, property_value, property_datatype, last_updated FROM osae_v_object_property WHERE object_name=@ObjectName AND property_name=@OSAEObjectProperty";
                    command.Parameters.AddWithValue("@ObjectName", ObjectName);
                    command.Parameters.AddWithValue("@OSAEObjectProperty", ObjectProperty);
                    dataset = OSAESql.RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        OSAEObjectProperty p = new OSAEObjectProperty();
                        p.Id = dataset.Tables[0].Rows[0]["object_property_id"].ToString();
                        p.DataType = dataset.Tables[0].Rows[0]["property_datatype"].ToString();
                        p.LastUpdated = dataset.Tables[0].Rows[0]["last_updated"].ToString();
                        p.Name = dataset.Tables[0].Rows[0]["property_name"].ToString();
                        p.Value = dataset.Tables[0].Rows[0]["property_value"].ToString();

                        return p;
                    }
                    else
                    {
                        OSAEObjectProperty p = new OSAEObjectProperty();
                        p.Id = string.Empty;
                        p.DataType = string.Empty;
                        p.LastUpdated = string.Empty;
                        p.Name = string.Empty;
                        p.Value = string.Empty;

                        return p;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - GetObjectPropertyValue error: " + ex.Message, true);
                return null;
            }
        }

        /// <summary>
        /// Set the value of a object's property
        /// </summary>
        /// <param name="objectName">The name of the object</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The value of the property</param>
        public static void ObjectPropertySet(string objectName, string propertyName, string propertyValue, string source)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_property_set (@ObjectName, @PropertyName, @PropertyValue, @FromObject, @DebugInfo)";
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@PropertyName", propertyName);
                command.Parameters.AddWithValue("@PropertyValue", propertyValue);
                command.Parameters.AddWithValue("@FromObject", PluginManager.GetPluginName(source, Common.ComputerName));
                command.Parameters.AddWithValue("@DebugInfo", null);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectPropertySet error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// The the properties of an object
        /// </summary>
        /// <param name="ObjectName">The object to get the proeprties of</param>
        /// <returns>The properties associated to the object</returns>
        public static OSAEObjectPropertyCollection GetObjectProperties(string ObjectName)
        {
            OSAEObjectPropertyCollection props = new OSAEObjectPropertyCollection();
                        
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    DataSet dataset = new DataSet();

                    command.CommandText = "SELECT object_property_id, property_name, property_value, property_datatype, last_updated FROM osae_v_object_property WHERE object_name=@ObjectName ORDER BY property_name";
                    command.Parameters.AddWithValue("@ObjectName", ObjectName);
                    dataset = OSAESql.RunQuery(command);

                    foreach (DataRow drp in dataset.Tables[0].Rows)
                    {
                        OSAEObjectProperty p = new OSAEObjectProperty();
                        p.Name = drp["property_name"].ToString();
                        p.Value = drp["property_value"].ToString();
                        p.DataType = drp["property_datatype"].ToString();
                        p.LastUpdated = drp["last_updated"].ToString();
                        p.Id = drp["object_property_id"].ToString();
                        props.Add(p);
                    }
                }

                return props;
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - GetObjectProperty error: " + ex.Message, true);
                return props;
            }
        }

        /// <summary>
        /// propertyLabel is usually left null
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="propertyLabel"></param>
        public static void ObjectPropertyArrayAdd(string objectName, string propertyName, string propertyValue, string propertyLabel)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_property_array_add (@ObjectName, @PropertyName, @PropertyValue, @PropertyLabel)";
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@PropertyName", propertyName);
                command.Parameters.AddWithValue("@PropertyValue", propertyValue);
                command.Parameters.AddWithValue("@PropertyLabel", propertyLabel);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectPropertyArrayAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Get one random value from a property array
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        public static string ObjectPropertyArrayGetRandom(string objectName, string propertyName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                DataSet dataset = new DataSet();
                command.CommandText = "CALL osae_sp_object_property_array_get_random (@ObjectName, @PropertyName)";
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@PropertyName", propertyName);
                try
                {
                    dataset = OSAESql.RunQuery(command);
                    string Result = dataset.Tables[0].Rows[0][0].ToString();
                    return Result;
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectPropertyArrayGetRandom error: " + command.CommandText + " - error: " + ex.Message, true);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Returns a Dataset of all the item in the property array
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static DataSet ObjectPropertyArrayGetAll(string objectName, string propertyName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                DataSet dataset = new DataSet();
                try
                {
                    command.CommandText = "CALL osae_sp_object_property_array_get_all (@ObjectName, @PropertyName)";
                    command.Parameters.AddWithValue("@ObjectName", objectName);
                    command.Parameters.AddWithValue("@PropertyName", propertyName);
                    dataset = OSAESql.RunQuery(command);

                    return dataset;
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectPropertyArrayGetAll error: " + command.CommandText + " - error: " + ex.Message, true);
                    return dataset;
                }
            }
        }

        /// <summary>
        /// Deletes an item from a property array
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public static void ObjectPropertyArrayDelete(string objectName, string propertyName, string propertyValue)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_property_array_delete (@ObjectName, @PropertyName, @PropertyValue)";
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@PropertyName", propertyName);
                command.Parameters.AddWithValue("@PropertyValue", propertyValue);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectPropertyArrayDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Deletes all items from a property array
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        public static void ObjectPropertyArrayDeleteAll(string objectName, string propertyName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_property_array_delete_all (@ObjectName, @PropertyName)";
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@PropertyName", propertyName);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectPropertyArrayDeleteAll error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static DataSet ObjectPropertyHistoryGet(string objectName, string propertyName, string from, string to)
        {
            DataSet ds = new DataSet();
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "SELECT history_timestamp, CASE property_datatype WHEN 'Boolean' THEN IF(property_value='TRUE', 1, 0) ELSE property_value END AS property_value FROM osae_v_object_property_history WHERE object_name = '" + objectName + "' and property_name = '" + propertyName + "' AND history_timestamp BETWEEN '" + from + "' AND '" + to + "' ORDER BY history_timestamp asc";
                try
                {
                    ds = OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectPropertyHistoryGet error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
            return ds;
        }
    }
}
