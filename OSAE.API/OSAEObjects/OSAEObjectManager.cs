namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using MySql.Data.MySqlClient;

    public class OSAEObjectManager
    {
        Logging logging = Logging.GetLogger();
        OSAE osae = new OSAE("");
        /// <summary>
        /// Returns true or false whether the object with the specified address exists
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool ObjectExists(string address)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();

            try
            {
                command.CommandText = "SELECT * FROM osae_v_object WHERE address=@Address";
                command.Parameters.AddWithValue("@Address", address);
                dataset = osae.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - ObjectExists Error: " + ex.Message, true);
                return false;
            }
        }

        /// <summary>
        /// Returns an OSAEObject with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public OSAEObject GetObjectByName(string name)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();

            try
            {
                command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_name, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE object_name=@Name";
                command.Parameters.AddWithValue("@Name", name);
                dataset = osae.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    OSAEObject obj = new OSAEObject(dataset.Tables[0].Rows[0]["object_name"].ToString(), dataset.Tables[0].Rows[0]["object_description"].ToString(), dataset.Tables[0].Rows[0]["object_type"].ToString(), dataset.Tables[0].Rows[0]["address"].ToString(), dataset.Tables[0].Rows[0]["container_name"].ToString(), Int32.Parse(dataset.Tables[0].Rows[0]["enabled"].ToString()));
                    obj.State.Value = dataset.Tables[0].Rows[0]["state_name"].ToString();
                    obj.State.TimeInState = Convert.ToInt64(dataset.Tables[0].Rows[0]["time_in_state"]);
                    obj.BaseType = dataset.Tables[0].Rows[0]["base_type"].ToString();

                    obj.Properties = osae.GetObjectProperties(obj.Name);

                    obj.Methods = osae.GetObjectMethods(obj.Name);

                    return obj;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - GetObjectByName (" + name + ")error: " + ex.Message, true);
                return null;
            }
        }

        /// <summary>
        /// Returns an OSAEObject with the specified address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public OSAEObject GetObjectByAddress(string address)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();
            OSAEObject obj = null;

            try
            {
                command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_name, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE address=@Address";
                command.Parameters.AddWithValue("@Address", address);
                dataset = osae.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    obj = new OSAEObject(dataset.Tables[0].Rows[0]["object_name"].ToString(), dataset.Tables[0].Rows[0]["object_description"].ToString(), dataset.Tables[0].Rows[0]["object_type"].ToString(), dataset.Tables[0].Rows[0]["address"].ToString(), dataset.Tables[0].Rows[0]["container_name"].ToString(), Int32.Parse(dataset.Tables[0].Rows[0]["enabled"].ToString()));
                    obj.State.Value = dataset.Tables[0].Rows[0]["state_name"].ToString();
                    obj.State.TimeInState = Convert.ToInt64(dataset.Tables[0].Rows[0]["time_in_state"]);
                    obj.BaseType = dataset.Tables[0].Rows[0]["base_type"].ToString();

                    obj.Properties = osae.GetObjectProperties(obj.Name);
                    obj.Methods = osae.GetObjectMethods(obj.Name);
                    return obj;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - GetObjectByAddress (" + address + ")error: " + ex.Message, true);
                return obj;
            }
        }

        /// <summary>
        /// Returns a Dataset of all objects in a specified container
        /// </summary>
        /// <param name="ContainerName"></param>
        /// <returns></returns>
        public List<OSAEObject> GetObjectsByContainer(string ContainerName)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();
            OSAEObject obj = new OSAEObject();
            List<OSAEObject> objects = new List<OSAEObject>();
            try
            {
                if (ContainerName == "")
                    command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_name, base_type, coalesce(time_in_state, 0) as time_in_state, last_updated FROM osae_v_object WHERE container_name is null ORDER BY object_name ASC";
                else
                {
                    command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_name, base_type, coalesce(time_in_state, 0) as time_in_state, last_updated FROM osae_v_object WHERE container_name=@ContainerName ORDER BY object_name ASC";
                    command.Parameters.AddWithValue("@ContainerName", ContainerName);
                }
                dataset = osae.RunQuery(command);
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dataset.Tables[0].Rows)
                    {
                        obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dr["object_type"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                        obj.State.Value = dr["state_name"].ToString();
                        obj.State.TimeInState = Convert.ToInt64(dr["time_in_state"]);
                        obj.BaseType = dr["base_type"].ToString();
                        obj.LastUpd = dr["last_updated"].ToString();

                        obj.Properties = osae.GetObjectProperties(obj.Name);
                        obj.Methods = osae.GetObjectMethods(obj.Name);
                        objects.Add(obj);
                    }
                    return objects;
                }
                return objects;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - GetObjectsByContainer error: " + ex.Message, true);
                return objects;
            }
        }

        public List<OSAEObject> GetObjectsByOwner(string ObjectOwner)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();
            OSAEObject obj = new OSAEObject();
            List<OSAEObject> objects = new List<OSAEObject>();
            try
            {
                command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_name, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE owned_by=@ObjectOwner";
                command.Parameters.AddWithValue("@ObjectOwner", ObjectOwner);
                dataset = osae.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dataset.Tables[0].Rows)
                    {
                        obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dr["object_type"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                        obj.State.Value = dr["state_name"].ToString();
                        obj.State.TimeInState = Convert.ToInt64(dr["time_in_state"]);
                        obj.BaseType = dr["base_type"].ToString();

                        obj.Properties = osae.GetObjectProperties(obj.Name);
                        obj.Methods = osae.GetObjectMethods(obj.Name);
                        objects.Add(obj);
                    }
                    return objects;
                }
                return objects;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - GetObjectsByBaseType error: " + ex.Message, true);
                return objects;
            }
        }

        /// <summary>
        /// Returns all objects with the specified base type
        /// </summary>
        /// <param name="ObjectBaseType"></param>
        /// <returns></returns>
        public List<OSAEObject> GetObjectsByBaseType(string ObjectBaseType)
        {
            DataSet dataset = new DataSet();
            OSAEObject obj = new OSAEObject();
            List<OSAEObject> objects = new List<OSAEObject>();

            using (MySqlCommand command = new MySqlCommand())
            {
                try
                {
                    command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_name, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE base_type=@ObjectType";
                    command.Parameters.AddWithValue("@ObjectType", ObjectBaseType);
                    dataset = osae.RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataset.Tables[0].Rows)
                        {
                            obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dr["object_type"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                            obj.State.Value = dr["state_name"].ToString();
                            obj.State.TimeInState = Convert.ToInt64(dr["time_in_state"]);
                            obj.BaseType = dr["base_type"].ToString();

                            obj.Properties = osae.GetObjectProperties(obj.Name);
                            obj.Methods = osae.GetObjectMethods(obj.Name);
                            objects.Add(obj);
                        }
                        return objects;
                    }
                    return objects;
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - GetObjectsByBaseType error: " + ex.Message, true);
                    return objects;
                }
            }
        }

        /// <summary>
        /// Returns a Dataset of all objects of specified type
        /// </summary>
        /// <param name="ObjectType"></param>
        /// <returns></returns>
        public List<OSAEObject> GetObjectsByType(string ObjectType)
        {
            DataSet dataset = new DataSet();
            OSAEObject obj = new OSAEObject();
            List<OSAEObject> objects = new List<OSAEObject>();

            using (MySqlCommand command = new MySqlCommand())
            {
                try
                {
                    command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_name, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE object_type=@ObjectType";
                    command.Parameters.AddWithValue("@ObjectType", ObjectType);
                    dataset = osae.RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataset.Tables[0].Rows)
                        {
                            obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dataset.Tables[0].Rows[0]["object_type"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                            obj.State.Value = dr["state_name"].ToString();
                            obj.State.TimeInState = Convert.ToInt64(dr["time_in_state"]);
                            obj.BaseType = dr["base_type"].ToString();

                            obj.Properties = osae.GetObjectProperties(obj.Name);
                            obj.Methods = osae.GetObjectMethods(obj.Name);
                            objects.Add(obj);
                        }
                        return objects;
                    }
                    return objects;
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - GetObjectsByType error: " + ex.Message, true);
                    return objects;
                }
            }
        }

        /// <summary>
        /// Create a new object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="objectType"></param>
        /// <param name="address"></param>
        /// <param name="container"></param>
        public void ObjectAdd(string name, string description, string objectType, string address, string container, bool enabled)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                //command.CommandText = "CALL osae_sp_object_add (@Name, @Description, @ObjectType, @Address, @Container, @Enabled, @results)";
                command.CommandText = "osae_sp_object_add";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("?pname", name);
                command.Parameters.AddWithValue("?pdescription", description);
                command.Parameters.AddWithValue("?pobjecttype", objectType);
                command.Parameters.AddWithValue("?paddress", address);
                command.Parameters.AddWithValue("?pcontainer", container);
                command.Parameters.AddWithValue("?penabled", enabled);
                command.Parameters.Add(new MySqlParameter("?results", MySqlDbType.Int32));
                command.Parameters["?results"].Direction = ParameterDirection.Output;
                try
                {
                    //RunQuery(command);
                    MySqlConnection connection = new MySqlConnection(Common.ConnectionString);
                    command.Connection = connection;
                    command.Connection.Open();
                    command.ExecuteNonQuery();

                    if (command.Parameters["?results"].Value.ToString() == "1")
                        logging.AddToLog("API - ObjectAdded successfully", true);
                    else if (command.Parameters["?results"].Value.ToString() == "2")
                        logging.AddToLog("API - ObjectAdd failed.  Object type doesn't exist.", true);
                    else if (command.Parameters["?results"].Value.ToString() == "3")
                        logging.AddToLog("API - ObjectAdd failed.  Object with same name or address already exists", true);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - ObjectAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete an object
        /// </summary>
        /// <param name="Name"></param>
        public void ObjectDelete(string Name)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_delete (@Name)";
                command.Parameters.AddWithValue("@Name", Name);
                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - ObjectDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete an object
        /// </summary>
        /// <param name="Name"></param>
        public void ObjectDeleteByAddress(string address)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_delete_by_address (@Address)";
                command.Parameters.AddWithValue("@Address", address);
                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - ObjectDeleteByAddress error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an existing object
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="description"></param>
        /// <param name="objectType"></param>
        /// <param name="Address"></param>
        /// <param name="Container"></param>
        /// <param name="Enabled"></param>
        public void ObjectUpdate(string oldName, string newName, string description, string objectType, string address, string container, int enabled)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_update (@OldName, @NewName, @Description, @ObjectType, @Address, @Container, @Enabled)";
                command.Parameters.AddWithValue("@OldName", oldName);
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@ObjectType", objectType);
                command.Parameters.AddWithValue("@Address", address);
                command.Parameters.AddWithValue("@Container", container);
                command.Parameters.AddWithValue("@Enabled", enabled);

                try
                {
                    osae.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - ObjectUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }
    }
}
