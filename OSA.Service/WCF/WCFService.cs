namespace WCF
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;
    using MySql.Data.MySqlClient;
    using OSAE;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WCFService" in both code and config file together.
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class WCFService : IWCFService
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("WCF Service");
        
        OSAEObjectManager objectManager = new OSAEObjectManager();
        OSAE osae = new OSAE("WCF Service");

        public event EventHandler<CustomEventArgs> MessageReceived;

        #region iWCFService Members

        private static readonly List<IMessageCallback> subscribers = new List<IMessageCallback>();

        public void SendMessageToClients(string msgType, string message, string from)
        {
            try
            {
                subscribers.ForEach(delegate(IMessageCallback callback)
                {
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened)
                    {
                        try
                        {
                            callback.OnMessageReceived(msgType, message, from, DateTime.Now);
                            logging.AddToLog("Message sent to client: " + message, false);
                        }
                        catch (TimeoutException ex)
                        {
                            logging.AddToLog("Timeout error when sending message to client: " + ex.Message, true);
                            subscribers.Remove(callback);
                        }
                        catch (Exception ex)
                        {
                            logging.AddToLog("Error when sending message to client: " + ex.Message, true);
                        }
                    }
                    else
                    {
                        subscribers.Remove(callback);
                    }
                });
            }
            catch (TimeoutException ex)
            {
                logging.AddToLog("Timeout Exception Error in SendMessageToClients: " + ex.Message, true);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error in SendMessageToClients: " + ex.Message, true);
            }
        }

        public bool Subscribe()
        {
            logging.AddToLog("Attempting to add a subscriber", true);
            try
            {
                IMessageCallback callback = OperationContext.Current.GetCallbackChannel<IMessageCallback>();
                if (!subscribers.Contains(callback))
                {
                    subscribers.Add(callback);
                    logging.AddToLog("New subscriber: " + callback.ToString(), true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Unsubscribe()
        {
            try
            {
                IMessageCallback callback = OperationContext.Current.GetCallbackChannel<IMessageCallback>();
                if (!subscribers.Contains(callback))
                    subscribers.Remove(callback);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void messageHost(string msgType, string message, string from)
        {
            try
            {
                if (MessageReceived != null)
                    MessageReceived(null, new CustomEventArgs(msgType, message, from));
            }
            catch
            {
                
            }
        }

        public List<OSAEObject> GetAllObjects()
        {
            MySqlCommand command = new MySqlCommand("SELECT object_name, object_description, object_type_description, container_name, state_label, last_updated, address, enabled, time_in_state, base_type FROM osae_v_object");
            DataSet ds = OSAESql.RunQuery(command);

            List<OSAEObject> objs = new List<OSAEObject>();
            OSAEObject obj;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj = new OSAEObject(dr["object_name"].ToString(), dr["object_description"].ToString(), dr["object_type_description"].ToString(), dr["address"].ToString(), dr["container_name"].ToString(), Int32.Parse(dr["enabled"].ToString()));
                obj.LastUpd = dr["last_updated"].ToString();
                obj.State.Value = dr["state_label"].ToString();
                obj.State.TimeInState = long.Parse(dr["time_in_state"].ToString());
                obj.BaseType = dr["base_type"].ToString();
                objs.Add(obj);
            }
            return objs;
        }

        public OSAEObject GetObject(string name)
        {
            // lookup object 
            return objectManager.GetObjectByName(name);
        }

        public OSAEObject GetObjectByAddress(string address)
        {
            // lookup object 
            return objectManager.GetObjectByAddress(address);
        }

        public List<OSAEObject> GetObjectsByType(string type)
        {
            return objectManager.GetObjectsByType(type);
        }

        public List<OSAEObject> GetObjectsByBaseType(string type)
        {
            // lookup objects of the requested type             
            return objectManager.GetObjectsByBaseType(type);
        }

        public List<OSAEObject> GetObjectsByContainer(string container)
        {
            return objectManager.GetObjectsByContainer(container);
        }

        public Boolean ExecuteMethod(string name, string method, string param1, string param2)
        {
            // execute a method on an object 
            OSAEMethodManager.MethodQueueAdd(name, method, param1, param2, "WCF Service");
            return true;
        }

        public Boolean SendPattern(string pattern)
        {
            string patternName = osae.MatchPattern(pattern);
            if (!string.IsNullOrEmpty(patternName))
            {
                OSAEMethodManager.MethodQueueAdd("Script Processor", "NAMED SCRIPT", patternName, "", "WCF Service");
            }
            return true;
        }

        public Boolean AddObject(string name, string description, string type, string address, string container, string enabled)
        {
            objectManager.ObjectAdd(name, description, type, address, container, Convert.ToBoolean(enabled));

            return true;
        }

        public Boolean UpdateObject(string oldName, string newName, string description, string type, string address, string container, int enabled)
        {
            objectManager.ObjectUpdate(oldName, newName, description, type, address, container, enabled);

            return true;
        }

        public Boolean DeleteObject(string name)
        {
            objectManager.ObjectDelete(name);

            return true;
        }

        public Boolean AddScript(string objName, string objEvent, string script)
        {
            osae.ObjectEventScriptAdd(objName, objEvent, script);
            return true;
        }

        public Boolean UpdateScript(string objName, string objEvent, string script)
        {
            osae.ObjectEventScriptUpdate(objName, objEvent, script);
            return true;
        }

        public List<OSAEObject> GetPlugins()
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();

            // lookup objects of the requested type 
            List<OSAEObject> objects = objectManager.GetObjectsByBaseType("plugin");
            return objects;
        }

        public DataSet ExecuteSQL(string sql)
        {
            MySqlCommand command = new MySqlCommand(sql);
            return OSAESql.RunQuery(command);
        }
        #endregion

        public Boolean SetProperty(string objName, string propName, string propValue)
        {
            ObjectPopertiesManager.ObjectPropertySet(objName, propName, propValue, "WCF Service");

            return true;
        }

        public Boolean SetState(string objName, string state)
        {
            osae.ObjectStateSet(objName, state);

            return true;
        }

    }

    public class CustomEventArgs : EventArgs
    {
        public string Message;
        public string MsgType;
        public string From;

        public CustomEventArgs(string msgType, string message, string from)
        {
            Message = message;
            MsgType = msgType;
            From = from;
        }

    }
}
