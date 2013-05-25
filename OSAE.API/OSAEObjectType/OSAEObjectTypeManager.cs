namespace OSAE
{
    using System;
    using System.Data;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// The class is used to manage OSA ObjectTypes
    /// </summary>
    public class OSAEObjectTypeManager
    {
        /// <summary>
        /// Loads the requested object Type
        /// </summary>
        /// <param name="name">The name of the object type to load</param>
        /// <returns>The requested object type</returns>
        public static OSAEObjectType ObjectTypeLoad(string name)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();

            try
            {
                command.CommandText = "SELECT object_type, object_type_description, object_type_owner, container, hide_redundant_events, base_type, object_name, system_hidden FROM osae_v_object_type WHERE object_type=@Name";
                command.Parameters.AddWithValue("@Name", name);
                dataset = OSAESql.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    OSAEObjectType type = new OSAEObjectType();
                    type.BaseType = dataset.Tables[0].Rows[0]["base_type"].ToString();
                    type.Description = dataset.Tables[0].Rows[0]["object_type_description"].ToString();
                    type.Name = dataset.Tables[0].Rows[0]["object_type"].ToString();
                    type.OwnedBy = dataset.Tables[0].Rows[0]["object_name"].ToString();
                    if(dataset.Tables[0].Rows[0]["object_type_owner"].ToString() == "1")
                        type.Owner = true;
                    else
                        type.Owner = false;

                    if (dataset.Tables[0].Rows[0]["system_hidden"].ToString() == "1")
                        type.SysType = true;
                    else
                        type.SysType = false;

                    if (dataset.Tables[0].Rows[0]["container"].ToString() == "1")
                        type.Container = true;
                    else
                        type.Container = false;

                    if (dataset.Tables[0].Rows[0]["hide_redundant_events"].ToString() == "1")
                        type.HideRedundant = true;
                    else
                        type.HideRedundant = false;

                    return type;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logging.GetLogger().AddToLog("API - GetObjectTypeLoad (" + name + ")error: " + ex.Message, true);
                return null;
            }
        }

        /// <summary>
        /// Create new object type
        /// </summary>
        /// <param name="Name">The name of the new object type</param>
        /// <param name="Description">The description of the new object type</param>
        /// <param name="OwnedBy">Who the new object type is owned by</param>
        /// <param name="BaseType">The base type of the new object</param>
        /// <param name="TypeOwner"></param>
        /// <param name="System"></param>
        /// <param name="Container">The container for the new object type</param>
        public static void ObjectTypeAdd(string Name, string Description, string OwnedBy, string BaseType, int TypeOwner, int System, int Container, int HideRedundantEvents)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_add (@Name, @Description, @OwnedBy, @BaseType, @TypeOwner, @System, @Container, @HideRedundantEvents)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Description", Description);
                command.Parameters.AddWithValue("@OwnedBy", OwnedBy);
                command.Parameters.AddWithValue("@BaseType", BaseType);
                command.Parameters.AddWithValue("@TypeOwner", TypeOwner);
                command.Parameters.AddWithValue("@System", System);
                command.Parameters.AddWithValue("@Container", Container);
                command.Parameters.AddWithValue("@HideRedundantEvents", HideRedundantEvents);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete an object type
        /// </summary>
        /// <param name="Name"></param>
        public static void ObjectTypeDelete(string Name)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_delete (@Name)";
                command.Parameters.AddWithValue("@Name", Name);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an existing object type
        /// </summary>
        /// <param name="oldName">The old name of the object type</param>
        /// <param name="newName">The new name to be given to the object type</param>
        /// <param name="Description">The description for the object type</param>
        /// <param name="OwnedBy">Who owns the object type</param>
        /// <param name="BaseType"></param>
        /// <param name="TypeOwner"></param>
        /// <param name="System"></param>
        /// <param name="Container">The container for the object type</param>
        public static void ObjectTypeUpdate(string oldName, string newName, string Description, string OwnedBy, string BaseType, int TypeOwner, int System, int Container, int HideRedundantEvents)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_update (@oldName, @newName, @Description, @OwnedBy, @BaseType, @TypeOwner, @System, @Container, @HideRedundantEvents)";
                command.Parameters.AddWithValue("@oldName", oldName);
                command.Parameters.AddWithValue("@newName", newName);
                command.Parameters.AddWithValue("@Description", Description);
                command.Parameters.AddWithValue("@OwnedBy", OwnedBy);
                command.Parameters.AddWithValue("@BaseType", BaseType);
                command.Parameters.AddWithValue("@TypeOwner", TypeOwner);
                command.Parameters.AddWithValue("@System", System);
                command.Parameters.AddWithValue("@Container", Container);
                command.Parameters.AddWithValue("@HideRedundantEvents", HideRedundantEvents);

                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Add an event top an existing object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Label"></param>
        /// <param name="ObjectType"></param>
        public static void ObjectTypeEventAdd(string Name, string Label, string ObjectType)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_event_add (@Name, @Label, @ObjectType)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Label", Label);
                command.Parameters.AddWithValue("@ObjectType", ObjectType);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ObjectTypeEventAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete an event from an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ObjectType"></param>
        public static void ObjectTypeEventDelete(string Name, string ObjectType)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_event_delete (@Name, @ObjectType)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@ObjectType", ObjectType);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeEventDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an existing event on an object type
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="label"></param>
        /// <param name="objectType"></param>
        public static void ObjectTypeEventUpdate(string oldName, string newName, string label, string objectType)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_event_update (@OldName, @NewName, @Label, @ObjectType)";
                command.Parameters.AddWithValue("@OldName", oldName);
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@Label", label);
                command.Parameters.AddWithValue("@ObjectType", objectType);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeEventUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Add a method to an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Label"></param>
        /// <param name="ObjectType"></param>
        public static void ObjectTypeMethodAdd(string Name, string Label, string ObjectType, string ParamLabel1, string ParamLabel2, string ParamDefault1, string ParamDefault2)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_method_add (@Name, @Label, @ObjectType, @ParamLabel1, @ParamLabel2, @ParamDefault1, @ParamDefault2)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Label", Label);
                command.Parameters.AddWithValue("@ObjectType", ObjectType);
                command.Parameters.AddWithValue("@ParamLabel1", ParamLabel1);
                command.Parameters.AddWithValue("@ParamLabel2", ParamLabel2);
                command.Parameters.AddWithValue("@ParamDefault1", ParamDefault1);
                command.Parameters.AddWithValue("@ParamDefault2", ParamDefault2);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeMethodAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete a method from an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ObjectType"></param>
        public static void ObjectTypeMethodDelete(string Name, string ObjectType)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_method_delete (@Name, @ObjectType)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@ObjectType", ObjectType);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeMethodDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an existing method on an object type
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="label"></param>
        /// <param name="objectType"></param>
        /// <param name="ParamLabel1"></param>
        /// <param name="ParamLabel2"></param>
        public static void ObjectTypeMethodUpdate(string oldName, string newName, string label, string objectType, string paramLabel1, string paramLabel2, string ParamDefault1, string ParamDefault2)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_method_update (@OldName, @NewName, @Label, @ObjectType, @ParamLabel1, @ParamLabel2, @ParamDefault1, @ParamDefault2)";
                command.Parameters.AddWithValue("@OldName", oldName);
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@Label", label);
                command.Parameters.AddWithValue("@ObjectType", objectType);
                command.Parameters.AddWithValue("@ParamLabel1", paramLabel1);
                command.Parameters.AddWithValue("@ParamLabel2", paramLabel2);
                command.Parameters.AddWithValue("@ParamDefault1", ParamDefault1);
                command.Parameters.AddWithValue("@ParamDefault2", ParamDefault2);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeMethodUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                    Logging.GetLogger().AddToLog("osae_sp_object_type_method_update (" + oldName + "," + newName + "," + label + "," + objectType + "," + paramLabel1 + "," + paramLabel2 + ")", true);
                }
            }
        }

        /// <summary>
        /// Add a property to an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ParameterType"></param>
        /// <param name="ObjectType"></param>
        public static void ObjectTypePropertyAdd(string Name, string ParameterType, string ParameterDefault, string ObjectType, bool TrackHistory)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_property_add (@Name, @ParameterType, @ParameterDefault, @ObjectType, @TrackHistory)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@ParameterType", ParameterType);
                command.Parameters.AddWithValue("@ParameterDefault", ParameterDefault);
                command.Parameters.AddWithValue("@ObjectType", ObjectType);
                command.Parameters.AddWithValue("@TrackHistory", TrackHistory);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypePropertyAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete a property from on object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ObjectType"></param>
        public static void ObjectTypePropertyDelete(string Name, string ObjectType)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_property_delete (@Name, @ObjectType)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@ObjectType", ObjectType);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ObjectTypePropertyADelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an existing property on an object type
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="ParameterType"></param>
        /// <param name="objectType"></param>
        public static void ObjectTypePropertyUpdate(string oldName, string newName, string ParameterType, string ParameterDefault, string objectType, bool TrackHistory)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_property_update (@OldName, @NewName, @ParameterType, @ParameterDefault, @ObjectType, @TrackHistory)";
                command.Parameters.AddWithValue("@OldName", oldName);
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@ParameterType", ParameterType);
                command.Parameters.AddWithValue("@ParameterDefault", ParameterDefault);
                command.Parameters.AddWithValue("@ObjectType", objectType);
                command.Parameters.AddWithValue("@TrackHistory", TrackHistory);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypePropertyAUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void ObjectTypePropertyOptionAdd(string objectType, string propertyName, string option)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_property_option_add (@objectName, @propertyName, @option)";
                command.Parameters.AddWithValue("@objectName", objectType);
                command.Parameters.AddWithValue("@propertyName", propertyName);
                command.Parameters.AddWithValue("@option", option);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypePropertyOptionAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void ObjectTypePropertyOptionDelete(string objectType, string propertyName, string option)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_property_option_delete (@objectName, @propertyName, @option)";
                command.Parameters.AddWithValue("@objectName", objectType);
                command.Parameters.AddWithValue("@propertyName", propertyName);
                command.Parameters.AddWithValue("@option", option);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypePropertyOptionDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public static void ObjectTypePropertyOptionUpdate(string objectType, string propertyName, string newoption, string oldoption)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_property_option_update (@objectName, @propertyName, @newoption, @oldoption)";
                command.Parameters.AddWithValue("@objectName", objectType);
                command.Parameters.AddWithValue("@propertyName", propertyName);
                command.Parameters.AddWithValue("@newoption", newoption);
                command.Parameters.AddWithValue("@oldoption", oldoption);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypePropertyOptionUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Add a state to an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Label"></param>
        /// <param name="ObjectType"></param>
        public static void ObjectTypeStateAdd(string Name, string Label, string ObjectType)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_state_add (@Name, @Label, @ObjectType)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Label", Label);
                command.Parameters.AddWithValue("@ObjectType", ObjectType);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeStateAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete a state from an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ObjectType"></param>
        public static void ObjectTypeStateDelete(string Name, string ObjectType)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_state_delete (@Name, @ObjectType)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@ObjectType", ObjectType);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("API - ObjectTypeStateDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an existing state from an object type
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="label"></param>
        /// <param name="objectType"></param>
        public static void ObjectTypeStateUpdate(string oldName, string newName, string newLabel, string objectType)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_state_update (@OldName, @NewName, @Label, @ObjectType)";
                command.Parameters.AddWithValue("@OldName", oldName);
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@Label", newLabel);
                command.Parameters.AddWithValue("@ObjectType", objectType);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    Logging.GetLogger().AddToLog("ObjectTypeStateUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }        
    }
}
