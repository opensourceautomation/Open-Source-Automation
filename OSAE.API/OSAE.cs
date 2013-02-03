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
        /// Adds a method to the queue
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="method"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public void MethodQueueAdd(string objectName, string method, string param1, string param2)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_method_queue_add (@Name, @Method, @Param1, @Param2, @FromObject, @DebugInfo)";
                command.Parameters.AddWithValue("@Name", objectName);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Param1", param1);
                command.Parameters.AddWithValue("@Param2", param2);
                command.Parameters.AddWithValue("@FromObject", GetPluginName(_parentProcess, ComputerName));
                command.Parameters.AddWithValue("@DebugInfo", null);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - MethodQueueAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Delete method from the queue
        /// </summary>
        /// <param name="methodID"></param>
        public void MethodQueueDelete(int methodID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_method_queue_delete (@ID)";
                command.Parameters.AddWithValue("@ID", methodID);
                try
                {
                    OSAESql.RunQuery(command);
                }
                catch (Exception ex)
                {
                    logging.AddToLog("API - MethodQueueDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
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
        /// Set the value of a object's property
        /// </summary>
        /// <param name="objectName">The name of the object</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The value of the property</param>
        public void ObjectPropertySet(string objectName, string propertyName, string propertyValue)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = "CALL osae_sp_object_property_set (@ObjectName, @PropertyName, @PropertyValue, @FromObject, @DebugInfo)";
            command.Parameters.AddWithValue("@ObjectName", objectName);
            command.Parameters.AddWithValue("@PropertyName", propertyName);
            command.Parameters.AddWithValue("@PropertyValue", propertyValue);
            command.Parameters.AddWithValue("@FromObject", GetPluginName(_parentProcess, ComputerName));
            command.Parameters.AddWithValue("@DebugInfo", null);
            try
            {
                OSAESql.RunQuery(command);
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - ObjectPropertySet error: " + command.CommandText + " - error: " + ex.Message, true);
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
                command.Parameters.AddWithValue("@FromObject", GetPluginName(_parentProcess, ComputerName));
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

        #region Object Type Methods
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

        #endregion

        #region Property Array Methods

        /// <summary>
		/// propertyLabel is usually left null
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="propertyLabel"></param>
        public void ObjectPropertyArrayAdd(string objectName, string propertyName, string propertyValue, string propertyLabel)
        {
            MySqlCommand command = new MySqlCommand();
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
                logging.AddToLog("API - ObjectPropertyArrayAdd error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }

        /// <summary>
        /// Get one random value from a property array
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        public string ObjectPropertyArrayGetRandom(string objectName, string propertyName)
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
                    logging.AddToLog("API - ObjectPropertyArrayGetRandom error: " + command.CommandText + " - error: " + ex.Message, true);
                    return "";
                }
            }
        }

        /// <summary>
        /// Returns a Dataset of all the item in the property array
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public DataSet ObjectPropertyArrayGetAll(string objectName, string propertyName)
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
                    logging.AddToLog("API - ObjectPropertyArrayGetAll error: " + command.CommandText + " - error: " + ex.Message, true);
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
        public void ObjectPropertyArrayDelete(string objectName, string propertyName, string propertyValue)
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
                    logging.AddToLog("API - ObjectPropertyArrayDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Deletes all items from a property array
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="propertyName"></param>
        public void ObjectPropertyArrayDeleteAll(string objectName, string propertyName)
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
                    logging.AddToLog("API - ObjectPropertyArrayDeleteAll error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        #endregion
	                  
        public List<OSAEScreenControl> GetScreenControls(string screenName)
        {
            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();
            OSAEScreenControl ctrl = new OSAEScreenControl();
            List<OSAEScreenControl> controls = new List<OSAEScreenControl>();
            try
            {
                command.CommandText = "SELECT object_name, control_name, control_type, state_name, last_updated, coalesce(time_in_state, 0) as time_in_state FROM osae_v_screen_object WHERE screen_name=@ScreenName";
                command.Parameters.AddWithValue("@ScreenName", screenName);
                dataset = OSAESql.RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dataset.Tables[0].Rows)
                    {
                        ctrl = new OSAEScreenControl();
                        ctrl.ObjectState = dr["state_name"].ToString();


                        ctrl.TimeInState = Convert.ToInt64(dr["time_in_state"]).ToString();
                        ctrl.ControlName = dr["control_name"].ToString();
                        ctrl.ControlType = dr["control_type"].ToString();
                        ctrl.LastUpdated = DateTime.Parse(dr["last_updated"].ToString());
                        ctrl.ObjectName = dr["object_name"].ToString();

                        controls.Add(ctrl);
                    }
                    return controls;
                }
                return controls;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - GetObjectsByBaseType error: " + ex.Message, true);
                return controls;
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
        /// Returns the name of the plugin object of the specified type on the scecified machine.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public string GetPluginName(string objectType, string machineName)
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
                logging.AddToLog("API - GetPluginName error: " + ex.Message + " - objectType: " + objectType + " | machineName: " + machineName, true);
                return string.Empty;
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
                    return dataset.Tables[0].Rows[0]["vInput"].ToString();
                else
                    return pattern;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - PatternParse error: " + ex.Message, true);
                return "";
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
                    return dataset.Tables[0].Rows[0]["pattern"].ToString();
                else
                    return "";
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - MatchPattern error: " + ex.Message, true);
                return "";
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

        public List<ObjectProperty> GetObjectProperties(string ObjectName)
        {
            List<ObjectProperty> props = new List<ObjectProperty>();

            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();
            try
            {
                command.CommandText = "SELECT object_property_id, property_name, property_value, property_datatype, last_updated FROM osae_v_object_property WHERE object_name=@ObjectName ORDER BY property_name";
                command.Parameters.AddWithValue("@ObjectName", ObjectName);
                dataset = OSAESql.RunQuery(command);

                foreach (DataRow drp in dataset.Tables[0].Rows)
                {
                    ObjectProperty p = new ObjectProperty();
                    p.Name = drp["property_name"].ToString();
                    p.Value = drp["property_value"].ToString();
                    p.DataType = drp["property_datatype"].ToString();
                    p.LastUpdated = drp["last_updated"].ToString();
                    p.Id = drp["object_property_id"].ToString();
                    props.Add(p);
                }
                return props;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - GetObjectProperty error: " + ex.Message, true);
                return props;
            }
        }

        public List<string> GetObjectMethods(string ObjectName)
        {
            DataSet dataset = new DataSet();
            List<string> methods = new List<string>();

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "SELECT method_name FROM osae_v_object_method WHERE object_name=@ObjectName ORDER BY method_name";
                    command.Parameters.AddWithValue("@ObjectName", ObjectName);
                    dataset = OSAESql.RunQuery(command);
                }

                foreach (DataRow drp in dataset.Tables[0].Rows)
                {
                    methods.Add(drp["method_name"].ToString());
                }

                return methods;
            }
            catch (Exception ex)
            {
                logging.AddToLog("API - GetObjectMethods error: " + ex.Message, true);
                return methods;
            }
        }
    }
}


