using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using MySql.Data.MySqlClient;
using OSAE;

namespace WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WCFService" in both code and config file together.
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class WCFService : IWCFService
    {
        OSAE.OSAE osae = new OSAE.OSAE("WCF Service");

        public event EventHandler<CustomEventArgs> MessageReceived;

        #region iWCFService Members

        private static readonly List<IMessageCallback> subscribers = new List<IMessageCallback>();

        public void SendMessageToClients(string msgType, string message)
        {
            try
            {
                subscribers.ForEach(delegate(IMessageCallback callback)
                {
                    if (((ICommunicationObject)callback).State == CommunicationState.Opened)
                    {
                        try
                        {
                            callback.OnMessageReceived(msgType, message, DateTime.Now);
                            osae.AddToLog("Message sent to client: " + message, false);
                        }
                        catch (TimeoutException ex)
                        {
                            osae.AddToLog("Timeout error when sending message to client: " + ex.Message, true);
                            subscribers.Remove(callback);
                        }
                        catch (Exception ex)
                        {
                            osae.AddToLog("Error when sending message to client: " + ex.Message, true);
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
                osae.AddToLog("Timeout Exception Error in SendMessageToClients: " + ex.Message, true);
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error in SendMessageToClients: " + ex.Message, true);
            }
        }

        public bool Subscribe()
        {
            osae.AddToLog("Attempting to add a subscriber", true);
            try
            {
                IMessageCallback callback = OperationContext.Current.GetCallbackChannel<IMessageCallback>();
                if (!subscribers.Contains(callback))
                {
                    subscribers.Add(callback);
                    osae.AddToLog("New subscriber: " + callback.ToString(), true);
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

        public void messageHost(string message)
        {
            try
            {
                if (MessageReceived != null)
                    MessageReceived(null, new CustomEventArgs(message));
            }
            catch
            {
                
            }
        }

        public List<OSAEObject> GetAllObjects()
        {
            MySqlCommand command = new MySqlCommand("SELECT object_name, object_description, object_type_description, container_name, state_label, last_updated, address, enabled, time_in_state, base_type FROM osae_v_object");
            DataSet ds = osae.RunQuery(command);

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
            return osae.GetObjectByName(name);
        }

        public OSAEObject GetObjectByAddress(string address)
        {
            // lookup object 
            return osae.GetObjectByAddress(address);
        }

        public List<OSAEObject> GetObjectsByType(string type)
        {
            
            return osae.GetObjectsByType(type);
        }

        public List<OSAEObject> GetObjectsByBaseType(string type)
        {
            // lookup objects of the requested type 
            
            return osae.GetObjectsByBaseType(type);
        }

        public List<OSAEObject> GetObjectsByContainer(string container)
        {
            
            return osae.GetObjectsByContainer(container);
        }

        public Boolean ExecuteMethod(string name, string method, string param1, string param2)
        {
            // execute a method on an object 
            osae.MethodQueueAdd(name, method, param1, param2);
            return true;
        }

        public Boolean SendPattern(string pattern)
        {
            string patternName = osae.MatchPattern(pattern);
            if (patternName != "")
                osae.MethodQueueAdd("Script Processor", "NAMED SCRIPT", patternName, "");
            return true;
        }

        public Boolean AddObject(string name, string description, string type, string address, string container, string enabled)
        {
            osae.ObjectAdd(name, description, type, address, container, Convert.ToBoolean(enabled));

            return true;
        }

        public Boolean UpdateObject(string oldName, string newName, string description, string type, string address, string container, int enabled)
        {
            osae.ObjectUpdate(oldName, newName, description, type, address, container, enabled);

            return true;
        }

        public Boolean DeleteObject(string name)
        {
            osae.ObjectDelete(name);

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
            // lookup objects of the requested type 
            List<OSAEObject> objects = osae.GetObjectsByBaseType("plugin");
            return objects;
        }

        public DataSet ExecuteSQL(string sql)
        {
            MySqlCommand command = new MySqlCommand(sql);
            return osae.RunQuery(command);
        }
        #endregion

        public Boolean SetProperty(string objName, string propName, string propValue)
        {
            osae.ObjectPropertySet(objName, propName, propValue);

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

        public CustomEventArgs(string message)
        {
            Message = message;
        }

    }
}
