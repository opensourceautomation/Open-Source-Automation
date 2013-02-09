namespace OSAE
{
    using MySql.Data.MySqlClient;
    using System;
    using System.Data;

    public class ObjectTypeManager
    {
        private Logging logging = Logging.GetLogger();

        /// <summary>
        /// Create new object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="OwnedBy"></param>
        /// <param name="BaseType"></param>
        /// <param name="TypeOwner"></param>
        /// <param name="System"></param>
        /// <param name="Container"></param>
        public void ObjectTypeAdd(string Name, string Description, string OwnedBy, string BaseType, int TypeOwner, int System, int Container, int HideRedundantEvents)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_object_type_add (@Name, @Description, @OwnedBy, @BaseType, @TypeOwner, @System, @Container, @HideRedundantEvents)";
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Description", Description);
                command.Parameters.AddWithValue("@OwnedBy", OwnedBy);
                command.Parameters.AddWithValue("@BaseType", OwnedBy);
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
                    logging.AddToLog("API - ObjectTypeAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete an object type
        /// </summary>
        /// <param name="Name"></param>
        public void ObjectTypeDelete(string Name)
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
                    logging.AddToLog("API - ObjectTypeDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Update an existing object type
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="Description"></param>
        /// <param name="OwnedBy"></param>
        /// <param name="BaseType"></param>
        /// <param name="TypeOwner"></param>
        /// <param name="System"></param>
        /// <param name="Container"></param>
        public void ObjectTypeUpdate(string oldName, string newName, string Description, string OwnedBy, string BaseType, int TypeOwner, int System, int Container, int HideRedundantEvents)
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
                    logging.AddToLog("API - ObjectTypeUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Add an event top an existing object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Label"></param>
        /// <param name="ObjectType"></param>
        public void ObjectTypeEventAdd(string Name, string Label, string ObjectType)
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
                    logging.AddToLog("ObjectTypeEventAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete an event from an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ObjectType"></param>
        public void ObjectTypeEventDelete(string Name, string ObjectType)
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
                    logging.AddToLog("API - ObjectTypeEventDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
        public void ObjectTypeEventUpdate(string oldName, string newName, string label, string objectType)
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
                    logging.AddToLog("API - ObjectTypeEventUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Add a method to an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Label"></param>
        /// <param name="ObjectType"></param>
        public void ObjectTypeMethodAdd(string Name, string Label, string ObjectType, string ParamLabel1, string ParamLabel2, string ParamDefault1, string ParamDefault2)
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
                    logging.AddToLog("API - ObjectTypeMethodAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete a method from an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ObjectType"></param>
        public void ObjectTypeMethodDelete(string Name, string ObjectType)
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
                    logging.AddToLog("API - ObjectTypeMethodDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
        public void ObjectTypeMethodUpdate(string oldName, string newName, string label, string objectType, string paramLabel1, string paramLabel2, string ParamDefault1, string ParamDefault2)
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
                    logging.AddToLog("API - ObjectTypeMethodUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                    logging.AddToLog("osae_sp_object_type_method_update (" + oldName + "," + newName + "," + label + "," + objectType + "," + paramLabel1 + "," + paramLabel2 + ")", true);
                }
            }
        }

        /// <summary>
        /// Add a property to an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ParameterType"></param>
        /// <param name="ObjectType"></param>
        public void ObjectTypePropertyAdd(string Name, string ParameterType, string ParameterDefault, string ObjectType, bool TrackHistory)
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
                    logging.AddToLog("API - ObjectTypePropertyAAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete a property from on object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ObjectType"></param>
        public void ObjectTypePropertyDelete(string Name, string ObjectType)
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
                    logging.AddToLog("ObjectTypePropertyADelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
        public void ObjectTypePropertyUpdate(string oldName, string newName, string ParameterType, string ParameterDefault, string objectType, bool TrackHistory)
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
                    logging.AddToLog("API - ObjectTypePropertyAUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public void ObjectTypePropertyOptionAdd(string objectType, string propertyName, string option)
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
                    logging.AddToLog("API - ObjectTypePropertyOptionAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public void ObjectTypePropertyOptionDelete(string objectType, string propertyName, string option)
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
                    logging.AddToLog("API - ObjectTypePropertyOptionDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        public void ObjectTypePropertyOptionUpdate(string objectType, string propertyName, string newoption, string oldoption)
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
                    logging.AddToLog("API - ObjectTypePropertyOptionUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Add a state to an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Label"></param>
        /// <param name="ObjectType"></param>
        public void ObjectTypeStateAdd(string Name, string Label, string ObjectType)
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
                    logging.AddToLog("API - ObjectTypeStateAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete a state from an object type
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ObjectType"></param>
        public void ObjectTypeStateDelete(string Name, string ObjectType)
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
                    logging.AddToLog("API - ObjectTypeStateDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
        public void ObjectTypeStateUpdate(string oldName, string newName, string newLabel, string objectType)
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
                    logging.AddToLog("ObjectTypeStateUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public DataSet GetObjectTypePropertyOptions(string objType, string propName)
        {
            DataSet dataset = new DataSet();
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "SELECT option_name FROM osae_v_object_type_property_option WHERE object_type=@ObjectType AND property_name=@PropertyName";
                    command.Parameters.AddWithValue("@ObjectType", objType);
                    command.Parameters.AddWithValue("@PropertyName", propName);
                    dataset = OSAESql.RunQuery(command);
                }

                return dataset;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - GetObjectTypePropertyOptions error: " + ex.Message, true);
                return dataset;
            }
        }
    }
}
