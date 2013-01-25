using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using MySql.Data.MySqlClient;

namespace OSAE
{
    /// <summary>
    /// API used to interact with the various components of OSA
    /// </summary>
    [Serializable]
    public partial class OSAE
    {
        #region Properties

        private object locker = new object();                
        private object logLocker = new object();
        private string _parentProcess;
        private string connectionString = string.Empty;

        /// <summary>
        /// The installation folder of OSA
        /// </summary>                
        public string APIpath  { get; set; }
              
        /// <summary>
        /// The location of the DB server
        /// </summary>
        public string DBConnection  { get; set; }
            
        /// <summary>
        /// The port to connect to OSA on
        /// </summary>
        public string DBPort  { get; set; }
            
        /// <summary>
        /// The name of the OSA database
        /// </summary>
        public string DBName  { get; set; }
        
        /// <summary>
        /// The username to connect to the DB with
        /// </summary>
        public string DBUsername  { get; set; }
      
        /// <summary>
        /// The password to connect to the DB with
        /// </summary>
        public string DBPassword  { get; set; }
      
        public string ComputerName  { get; set; }
      
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="parentProcess">The parent process</param>
        public OSAE(string parentProcess)
        {
            ModifyRegistry myRegistry = new ModifyRegistry();
            myRegistry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";
            DBConnection = myRegistry.Read("DBCONNECTION");
            DBPort = myRegistry.Read("DBPORT");
            DBName = myRegistry.Read("DBNAME");
            DBUsername = myRegistry.Read("DBUSERNAME");
            DBPassword = myRegistry.Read("DBPASSWORD");
            ComputerName = Dns.GetHostName();
            _parentProcess = parentProcess;
            APIpath = myRegistry.Read("INSTALLDIR");
        }      

        /// <summary>
        /// Adds a message to the log
        /// </summary>
        /// <param name="audit"></param>
        /// <param name="alwaysLog"></param>
        public void AddToLog(string audit, bool alwaysLog)
        {
            try
            {
                if (GetObjectPropertyValue("SYSTEM", "Debug").Value == "TRUE" || alwaysLog)
                {
                    lock (logLocker)
                    {
                        string filePath = APIpath + "/Logs/" + _parentProcess + ".log";
                        System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                        file.Directory.Create();
                        StreamWriter sw = File.AppendText(filePath);
                        sw.WriteLine(System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + audit);
                        sw.Close();
                        if (GetObjectPropertyValue("SYSTEM", "Prune Logs").Value == "TRUE")
                        {
                            if (file.Length > 1000000)
                                file.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (logLocker)
                {
                    string filePath = APIpath + "/Logs/" + _parentProcess + ".log";
                    System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                    file.Directory.Create();
                    StreamWriter sw = File.AppendText(filePath);
                    sw.WriteLine(System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - LOGGING ERROR: " 
                        + ex.Message + " - " + ex.InnerException);
                    sw.Close();
                    if (file.Length > 1000000)
                        file.Delete();
                }
            }
        }

        /// <summary>
        /// Add an entry to the degug table
        /// </summary>
        /// <param name="entry">String to add to the debug table</param>
        public void DebugLogAdd(string entry)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "CALL osae_sp_debug_log_add (@Entry,@Process)";
                    command.Parameters.AddWithValue("@Entry", entry);
                    command.Parameters.AddWithValue("@Process", _parentProcess);
                    RunQuery(command);
                }
            }
            catch
            {
                // Not a lot we can do if it fails here
            }
        }

        /// <summary>
        /// Add an entry to the event log table
        /// </summary>
        /// <param name="objectName">Object Name</param>
        /// <param name="eventName">Event Name</param>
        public void EventLogAdd(string objectName, string eventName, string parameter1 = null, string parameter2 = null)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_event_log_add (@ObjectName, @EventName, @FromObject, @DebugInfo, @Param1, @Param2)";
                command.Parameters.AddWithValue("@ObjectName", objectName);
                command.Parameters.AddWithValue("@EventName", eventName);
                command.Parameters.AddWithValue("@FromObject", GetPluginName(_parentProcess, ComputerName));
                command.Parameters.AddWithValue("@DebugInfo", null);
                command.Parameters.AddWithValue("@Param1", parameter1);
                command.Parameters.AddWithValue("@Param2", parameter2);
                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - EventLogAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// Deletes everything from the event_log table
        /// </summary>
        public void EventLogClear()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_event_log_clear";
                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - EventLogClear error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - MethodQueueAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - MethodQueueDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    MySqlConnection connection = new MySqlConnection(connectionString = "SERVER=" + DBConnection + ";" +
                        "DATABASE=" + DBName + ";" +
                        "PORT=" + DBPort + ";" +
                        "UID=" + DBUsername + ";" +
                        "PASSWORD=" + DBPassword + ";");
                    command.Connection = connection;
                    command.Connection.Open();
                    command.ExecuteNonQuery();

                    if (command.Parameters["?results"].Value.ToString() == "1")
                        AddToLog("API - ObjectAdded successfully", true);
                    else if (command.Parameters["?results"].Value.ToString() == "2")
                        AddToLog("API - ObjectAdd failed.  Object type doesn't exist.", true);
                    else if (command.Parameters["?results"].Value.ToString() == "3")
                        AddToLog("API - ObjectAdd failed.  Object with same name or address already exists", true);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectDeleteByAddress error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectEventScriptAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                RunQuery(command);
            }
            catch (Exception ex)
            {
                AddToLog("API - ObjectEventScriptUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
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
                RunQuery(command);
            }
            catch (Exception ex)
            {
                AddToLog("API - ObjectPropertySet error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ObjectStateSet error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                ///AddToLog("@TypeOwner" + TypeOwner + "  @Container" + Container);
                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ObjectTypeEventAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeEventDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeEventUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeMethodAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeMethodDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeMethodUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                    AddToLog("osae_sp_object_type_method_update (" + oldName + "," + newName + "," + label + "," + objectType + "," + paramLabel1 + "," + paramLabel2 + ")", true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypePropertyAAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ObjectTypePropertyADelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypePropertyAUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypePropertyOptionAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypePropertyOptionDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypePropertyOptionUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeStateAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectTypeStateDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ObjectTypeStateUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    dataset = RunQuery(command);
                }

                return dataset;
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectTypePropertyOptions error: " + ex.Message, true);
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
                RunQuery(command);
            }
            catch (Exception ex)
            {
                AddToLog("API - ObjectPropertyArrayAdd error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    dataset = RunQuery(command);
                    string Result = dataset.Tables[0].Rows[0][0].ToString();
                    return Result;
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectPropertyArrayGetRandom error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    dataset = RunQuery(command);

                    return dataset;
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectPropertyArrayGetAll error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectPropertyArrayDelete error: " + command.CommandText + " - error: " + ex.Message, true);
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
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ObjectPropertyArrayDeleteAll error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        #endregion

        #region Scheduling Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleDate"></param>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="pattern"></param>
        /// <param name="recurringID"></param>
        public void ScheduleQueueAdd(DateTime scheduleDate, string obj, string method, string parameter1, string parameter2, string pattern, int recurringID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_queue_add(@ScheduleDate, @Object, @Method, @Parameter1, @Parameter2, @Pattern, @RecurringID)";
                command.Parameters.AddWithValue("@ScheduleDate", scheduleDate);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Pattern", pattern);
                command.Parameters.AddWithValue("@RecurringID", recurringID);

                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ScheduleQueueAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueID"></param>
        public void ScheduleQueueDelete(int queueID)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_queue_delete(@QueueID)";
                command.Parameters.AddWithValue("@QueueID", queueID);

                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ScheduleQueueDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleName"></param>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="pattern"></param>
        /// <param name="recurringTime"></param>
        /// <param name="sunday"></param>
        /// <param name="monday"></param>
        /// <param name="tuesday"></param>
        /// <param name="wednesday"></param>
        /// <param name="thursday"></param>
        /// <param name="friday"></param>
        /// <param name="saturday"></param>
        /// <param name="interval"></param>
        /// <param name="recurringDay"></param>
        /// <param name="recurringDate"></param>
        public void ScheduleRecurringAdd(string scheduleName, string obj, string method, string parameter1, string parameter2, string pattern,
            string recurringTime, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday,
            string interval, int recurringMinutes, string recurringDay, string recurringDate)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_add(@ScheduleName, @Object, @Method, @Parameter1, @Parameter2, ";
                command.CommandText = command.CommandText + "@Pattern, @RecurringTime, @Sunday, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, ";
                command.CommandText = command.CommandText + "@Saturday, @Interval, @RecurringMinutes, @RecurringDay, @RecurringDate)";
                command.Parameters.AddWithValue("@ScheduleName", scheduleName);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Pattern", pattern);
                command.Parameters.AddWithValue("@RecurringTime", recurringTime);
                command.Parameters.AddWithValue("@Sunday", sunday);
                command.Parameters.AddWithValue("@Monday", monday);
                command.Parameters.AddWithValue("@Tuesday", tuesday);
                command.Parameters.AddWithValue("@Wednesday", wednesday);
                command.Parameters.AddWithValue("@Thursday", thursday);
                command.Parameters.AddWithValue("@Friday", friday);
                command.Parameters.AddWithValue("@Saturday", saturday);
                command.Parameters.AddWithValue("@Interval", interval);
                command.Parameters.AddWithValue("@RecurringMinutes", recurringMinutes);
                command.Parameters.AddWithValue("@RecurringDay", recurringDay);
                command.Parameters.AddWithValue("@RecurringDate", recurringDate);

                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ScheduleRecurringAdd error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleName"></param>
        public void ScheduleRecurringDelete(string scheduleName)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_delete(@ScheduleName)";
                command.Parameters.AddWithValue("@ScheduleName", scheduleName);

                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ScheduleRecurringDelete error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldScheduleName"></param>
        /// <param name="newScheduleName"></param>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        /// <param name="pattern"></param>
        /// <param name="recurringTime"></param>
        /// <param name="sunday"></param>
        /// <param name="monday"></param>
        /// <param name="tuesday"></param>
        /// <param name="wednesday"></param>
        /// <param name="thursday"></param>
        /// <param name="friday"></param>
        /// <param name="saturday"></param>
        /// <param name="interval"></param>
        /// <param name="recurringDay"></param>
        /// <param name="recurringDate"></param>
        public void ScheduleRecurringUpdate(string oldScheduleName, string newScheduleName, string obj, string method, string parameter1, string parameter2, string pattern,
            string recurringTime, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday,
            string interval, int recurringMinutes, string recurringDay, string recurringDate)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_schedule_recurring_update(@OldScheduleName, @NewScheduleName, @Object, @Method, @Parameter1, @Parameter2, ";
                command.CommandText = command.CommandText + "@Pattern, @RecurringTime, @Sunday, @Monday, @Tuesday, @Wednesday, @Thursday, @Friday, ";
                command.CommandText = command.CommandText + "@Saturday, @Interval, @RecurringMinutes, @RecurringDay, @RecurringDate)";
                command.Parameters.AddWithValue("@OldScheduleName", oldScheduleName);
                command.Parameters.AddWithValue("@NewScheduleName", newScheduleName);
                command.Parameters.AddWithValue("@Object", obj);
                command.Parameters.AddWithValue("@Method", method);
                command.Parameters.AddWithValue("@Parameter1", parameter1);
                command.Parameters.AddWithValue("@Parameter2", parameter2);
                command.Parameters.AddWithValue("@Pattern", pattern);
                command.Parameters.AddWithValue("@RecurringTime", recurringTime);
                command.Parameters.AddWithValue("@Sunday", sunday);
                command.Parameters.AddWithValue("@Monday", monday);
                command.Parameters.AddWithValue("@Tuesday", tuesday);
                command.Parameters.AddWithValue("@Wednesday", wednesday);
                command.Parameters.AddWithValue("@Thursday", thursday);
                command.Parameters.AddWithValue("@Friday", friday);
                command.Parameters.AddWithValue("@Saturday", saturday);
                command.Parameters.AddWithValue("@Interval", interval);
                command.Parameters.AddWithValue("@RecurringMinutes", recurringMinutes);
                command.Parameters.AddWithValue("@RecurringDay", recurringDay);
                command.Parameters.AddWithValue("@RecurringDate", recurringDate);
                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("ScheduleRecurringUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RunScheduledMethods()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_run_scheduled_methods";
                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - RunScheduledMethods error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessRecurring()
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.CommandText = "CALL osae_sp_process_recurring";
                try
                {
                    RunQuery(command);
                }
                catch (Exception ex)
                {
                    AddToLog("API - ProcessRecurring error: " + command.CommandText + " - error: " + ex.Message, true);
                }
            }
        }

        #endregion

		#region Object Getters
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
                    command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_label, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE object_type=@ObjectType";
                    command.Parameters.AddWithValue("@ObjectType", ObjectType);
                    dataset = RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataset.Tables[0].Rows)
                        {
                            obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dataset.Tables[0].Rows[0]["object_type"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                            obj.State.Value = dr["state_label"].ToString();
                            obj.State.TimeInState = Convert.ToInt64(dr["time_in_state"]);
                            obj.BaseType = dr["base_type"].ToString();
                            
                            obj.Properties = GetObjectProperties(obj.Name);
                            obj.Methods = GetObjectMethods(obj.Name);
                            objects.Add(obj);
                        }
                        return objects;
                    }
                    return objects;
                }
                catch (Exception ex)
                {
                    AddToLog("API - GetObjectsByType error: " + ex.Message, true);
                    return objects;
                }
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
                    command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_label, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE base_type=@ObjectType";
                    command.Parameters.AddWithValue("@ObjectType", ObjectBaseType);
                    dataset = RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataset.Tables[0].Rows)
                        {
                            obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dr["object_type"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                            obj.State.Value = dr["state_label"].ToString();
                            obj.State.TimeInState = Convert.ToInt64(dr["time_in_state"]);
                            obj.BaseType = dr["base_type"].ToString();
                            
                            obj.Properties = GetObjectProperties(obj.Name);
                            obj.Methods = GetObjectMethods(obj.Name);
                            objects.Add(obj);
                        }
                        return objects;
                    }
                    return objects;
                }
                catch (Exception ex)
                {
                    AddToLog("API - GetObjectsByBaseType error: " + ex.Message, true);
                    return objects;
                }
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
                command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_label, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE owned_by=@ObjectOwner";
                command.Parameters.AddWithValue("@ObjectOwner", ObjectOwner);
                dataset = RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dataset.Tables[0].Rows)
                    {
                        obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dr["object_type"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                        obj.State.Value = dr["state_label"].ToString();
                        obj.State.TimeInState = Convert.ToInt64(dr["time_in_state"]);
                        obj.BaseType = dr["base_type"].ToString();
                        
                        obj.Properties = GetObjectProperties(obj.Name);
                        obj.Methods = GetObjectMethods(obj.Name);
                        objects.Add(obj);
                    }
                    return objects;
                }
                return objects;
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectsByBaseType error: " + ex.Message, true);
                return objects;
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
                    command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_label, base_type, coalesce(time_in_state, 0) as time_in_state, last_updated FROM osae_v_object WHERE container_name is null ORDER BY object_name ASC";
                else
                {
                    command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_label, base_type, coalesce(time_in_state, 0) as time_in_state, last_updated FROM osae_v_object WHERE container_name=@ContainerName ORDER BY object_name ASC";
                    command.Parameters.AddWithValue("@ContainerName", ContainerName);
                }
                dataset = RunQuery(command);
                if (dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dataset.Tables[0].Rows)
                    {
                        obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dr["object_type"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                        obj.State.Value = dr["state_label"].ToString();
                        obj.State.TimeInState = Convert.ToInt64(dr["time_in_state"]);
                        obj.BaseType = dr["base_type"].ToString();
                        obj.LastUpd = dr["last_updated"].ToString();

                        obj.Properties = GetObjectProperties(obj.Name);
                        obj.Methods = GetObjectMethods(obj.Name);
                        objects.Add(obj);
                    }
                    return objects;
                }
                return objects;
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectsByContainer error: " + ex.Message, true);
                return objects;
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
                command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_label, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE address=@Address";
                command.Parameters.AddWithValue("@Address", address);
                dataset = RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    obj = new OSAEObject(dataset.Tables[0].Rows[0]["object_name"].ToString(), dataset.Tables[0].Rows[0]["object_description"].ToString(), dataset.Tables[0].Rows[0]["object_type"].ToString(), dataset.Tables[0].Rows[0]["address"].ToString(), dataset.Tables[0].Rows[0]["container_name"].ToString(), Int32.Parse(dataset.Tables[0].Rows[0]["enabled"].ToString()));
                    obj.State.Value = dataset.Tables[0].Rows[0]["state_label"].ToString();
                    obj.State.TimeInState = Convert.ToInt64(dataset.Tables[0].Rows[0]["time_in_state"]);
                    obj.BaseType = dataset.Tables[0].Rows[0]["base_type"].ToString();

                    obj.Properties = GetObjectProperties(obj.Name);
                    obj.Methods = GetObjectMethods(obj.Name);
                    return obj;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectByAddress (" + address + ")error: " + ex.Message, true);
                return obj;
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
                command.CommandText = "SELECT object_name, object_description, object_type, address, container_name, enabled, state_label, base_type, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE object_name=@Name";
                command.Parameters.AddWithValue("@Name", name);
                dataset = RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    OSAEObject obj = new OSAEObject(dataset.Tables[0].Rows[0]["object_name"].ToString(), dataset.Tables[0].Rows[0]["object_description"].ToString(), dataset.Tables[0].Rows[0]["object_type"].ToString(), dataset.Tables[0].Rows[0]["address"].ToString(), dataset.Tables[0].Rows[0]["container_name"].ToString(), Int32.Parse(dataset.Tables[0].Rows[0]["enabled"].ToString()));
                    obj.State.Value = dataset.Tables[0].Rows[0]["state_label"].ToString();
                    obj.State.TimeInState = Convert.ToInt64(dataset.Tables[0].Rows[0]["time_in_state"]);
                    obj.BaseType = dataset.Tables[0].Rows[0]["base_type"].ToString();

                    obj.Properties = GetObjectProperties(obj.Name);

                    obj.Methods = GetObjectMethods(obj.Name);

                    return obj;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectByName (" + name + ")error: " + ex.Message, true);
                return null;
            }
        }

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
                dataset = RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dataset.Tables[0].Rows)
                    {
                        ctrl = new OSAEScreenControl();
                        ctrl.Object_State = dr["state_name"].ToString();
                        ctrl.Object_State_Time = Convert.ToInt64(dr["time_in_state"]).ToString();
                        ctrl.Control_Name = dr["control_name"].ToString();
                        ctrl.Control_Type = dr["control_type"].ToString();
                        ctrl.Object_Last_Updated = dr["last_updated"].ToString();
                        ctrl.Object_Name = dr["object_name"].ToString();

                        controls.Add(ctrl);
                    }
                    return controls;
                }
                return controls;
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectsByBaseType error: " + ex.Message, true);
                return controls;
            }
        }
		#endregion Object Getters

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
				dataset = RunQuery(command);

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
				AddToLog("API - GetObjectPropertyValue error: " + ex.Message, true);
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
					command.CommandText = "SELECT state_label, coalesce(time_in_state, 0) as time_in_state FROM osae_v_object WHERE object_name=@ObjectName";
					command.Parameters.AddWithValue("@ObjectName", ObjectName);
					dataset = RunQuery(command);
				}

				if (dataset.Tables[0].Rows.Count > 0)
				{
					state.Value = dataset.Tables[0].Rows[0]["state_label"].ToString();
					state.TimeInState = Convert.ToInt64(dataset.Tables[0].Rows[0]["time_in_state"]);

					return state;
				}
				else
				{
					AddToLog("API - GetObjectStateValue error: Object does not exist | ObjectName: " + ObjectName, true);
					return null;
				}
			}
			catch (Exception ex)
			{
				AddToLog("API - GetObjectStateValue error: " + ex.Message + " | ObjectName: " + ObjectName, true);
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
                dataset = RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                    return dataset.Tables[0].Rows[0]["object_name"].ToString();
                else
                {
                    command = new MySqlCommand();
                    command.CommandText = "SELECT * FROM osae_object WHERE object_name=@ObjectType2";
                    command.Parameters.AddWithValue("@ObjectType2", objectType);
                    dataset = RunQuery(command);

                    if (dataset.Tables[0].Rows.Count > 0)
                        return objectType;
                    else
                        return "";
                }
            }
            catch (Exception ex)
            {
                AddToLog("API - GetPluginName error: " + ex.Message + " - objectType: " + objectType + " | machineName: " + machineName, true);
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
					dataset = RunQuery(command);

					return dataset;
				}
				catch (Exception ex)
				{
					AddToLog("API - GetPluginSettings error: " + ex.Message, true);
					return dataset;
				}
			}
		}

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
                dataset = RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                AddToLog("API - ObjectExists Error: " + ex.Message, true);
                return false;
            }
        }

        /// <summary>
        /// Runs the passed in SQL query on the database and returns a dataset of the results
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public DataSet RunQuery(MySqlCommand command)
        {           
            DataSet dataset = new DataSet();
            
            if (API.Common.TestConnection())
            {
                lock (locker)
                {
                    using (MySqlConnection connection = new MySqlConnection(API.Common.ConnectionString))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        command.Connection = connection;                        
                        adapter.Fill(dataset);
                    }
                }
            }
            return dataset;
        }

        /// <summary>
        /// Runs the passed in SQL query on the database and returns a dataset of the results
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public DataSet RunSQL(string sql)
        {
            DataSet dataset = new DataSet();

            using (MySqlConnection connection = new MySqlConnection(API.Common.ConnectionString))
            {
                MySqlCommand command = new MySqlCommand(sql);
                MySqlDataAdapter adapter;  

                if (API.Common.TestConnection())
                {
                    lock (locker)
                    {                        
                        command.Connection = connection;
                        adapter = new MySqlDataAdapter(command);
                        adapter.Fill(dataset);
                    }
                }
            }
            return dataset;
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
                dataset = RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                    return dataset.Tables[0].Rows[0]["vInput"].ToString();
                else
                    return pattern;
            }
            catch (Exception ex)
            {
                AddToLog("API - PatternParse error: " + ex.Message, true);
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
                dataset = RunQuery(command);

                if (dataset.Tables[0].Rows.Count > 0)
                    return dataset.Tables[0].Rows[0]["pattern"].ToString();
                else
                    return "";
            }
            catch (Exception ex)
            {
                AddToLog("API - MatchPattern error: " + ex.Message, true);
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
                RunQuery(command);
            }
            catch (Exception ex)
            {
                AddToLog("API - NamedScriptUpdate error: " + command.CommandText + " - error: " + ex.Message, true);
            }
        }

        private List<ObjectProperty> GetObjectProperties(string ObjectName)
        {
            List<ObjectProperty> props = new List<ObjectProperty>();

            MySqlCommand command = new MySqlCommand();
            DataSet dataset = new DataSet();
            try
            {
                command.CommandText = "SELECT object_property_id, property_name, property_value, property_datatype, last_updated FROM osae_v_object_property WHERE object_name=@ObjectName ORDER BY property_name";
                command.Parameters.AddWithValue("@ObjectName", ObjectName);
                dataset = RunQuery(command);

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
                AddToLog("API - GetObjectProperty error: " + ex.Message, true);
                return props;
            }
        }

        private List<string> GetObjectMethods(string ObjectName)
        {
            DataSet dataset = new DataSet();
            List<string> methods = new List<string>();

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandText = "SELECT method_name FROM osae_v_object_method WHERE object_name=@ObjectName ORDER BY method_name";
                    command.Parameters.AddWithValue("@ObjectName", ObjectName);
                    dataset = RunQuery(command);
                }

                foreach (DataRow drp in dataset.Tables[0].Rows)
                {
                    methods.Add(drp["method_name"].ToString());
                }

                return methods;
            }
            catch (Exception ex)
            {
                AddToLog("API - GetObjectMethods error: " + ex.Message, true);
                return methods;
            }
        }

    }

   

    

    

  

  



}


