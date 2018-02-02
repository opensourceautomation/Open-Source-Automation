namespace OSAERest
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Drawing;
    using System.Speech;
    using System.Speech.Recognition;
    using OSAE;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    using System.Linq;
    using System.Web;
    
    [ServiceContract]
    public interface IRestService
    {
        [OperationContract]
        [WebGet(UriTemplate = "object/{name}?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObject GetObject(string name, string authkey);
        
        [OperationContract]
        [WebGet(UriTemplate = "object/{name}/state?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectState GetObjectState(string name, string authkey);

        [OperationContract]
        [WebInvoke(UriTemplate = "object/{name}/setstate/{state}?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean SetObjectState(string name, string state, string authkey);

        [OperationContract]
        [WebInvoke(UriTemplate = "object/{name}/{method}?param1={param1}&param2={param2}&ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean ExecuteMethod(string name, string method, string param1, string param2, string authkey);

        [OperationContract]
        [WebInvoke(UriTemplate = "object/add?name={name}&alias={alias}&desc={description}&type={type}&address={address}&container={container}&mintrustlevel={mintrustlevel}&enabled={enabled}&ak={authkey}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean AddObject(string name, string alias, string description, string type, string address, string container, int mintrustlevel, string enabled, string authkey);

        [OperationContract]
        [WebInvoke(UriTemplate = "object/update?oldName={oldName}&newName={newName}&alias={alias}&desc={description}&type={type}&address={address}&container={container}&mintrustlevel={mintrustlevel}&enabled={enabled}&ak={authkey}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean UpdateObject(string oldName, string newName, string alias, string description, string type, string address, string container, int mintrustlevel, string enabled, string authkey);

        [OperationContract]
        [WebGet(UriTemplate = "objects/type/{type}?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectCollection GetObjectsByType(string type, string authkey);

        [OperationContract]
        [WebGet(UriTemplate = "objects/basetype/{type}?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectCollection GetObjectsByBaseType(string type, string authkey);

        [OperationContract]
        [WebGet(UriTemplate = "objects/container/{container}?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectCollection GetObjectsByContainer(string container, string authkey);

        [OperationContract]
        [WebGet(UriTemplate = "object/propertylist/{objName}/{propName}?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> getPropertyList(string objName, string propName, string authkey);

        [OperationContract]
        [WebGet(UriTemplate = "plugins?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectCollection GetPlugins(string authkey);

        [OperationContract]
        [WebInvoke(UriTemplate = "namedscript/{match}?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean SendPattern(string match, string authkey);

        //[OperationContract]
        //[WebGet(UriTemplate = "namedscript/update?name={name}&oldName={oldName}&script={script}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //Boolean UpdateNamedScript(string name, string oldName, string script);
        
        //[OperationContract]
        //[WebGet(UriTemplate = "script/add?obj={objName}&event={objEvent}&script={script}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //Boolean AddScript(string objName, string objEvent, string script);

        //[OperationContract]
        //[WebGet(UriTemplate = "script/update?obj={objName}&event={objEvent}&script={script}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //Boolean UpdateScript(string objName, string objEvent, string script);
    
        [OperationContract]
        [WebGet(UriTemplate = "system/states?ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> GetSystemStates(string authkey);

        [OperationContract]        
        [WebInvoke(UriTemplate = "property/update?objName={objName}&propName={propName}&propVal={propVal}&ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean SetObjectProperty(string objName, string propName, string propVal, string authkey);


        [OperationContract]
        [WebGet(UriTemplate = "analytics/{objName}/{propName}?f={from}&t={to}&ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare )]
        List<OSAEPropertyHistory> GetPropertyHistory(string objName, string propName, string from, string to, string authkey);

        [OperationContract]
        [WebGet(UriTemplate = "analytics/state/{objName}?f={from}&t={to}&ak={authkey}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        List<OSAEStateHistory> GetStateHistory(string objName, string from, string to, string authkey);
    }

    public class api : IRestService
    {
       // private Logging logging = Logging.GetLogger("Rest");
        private OSAE.General.OSAELog Log = new OSAE.General.OSAELog("Rest");
        SpeechRecognitionEngine oRecognizer = new SpeechRecognitionEngine();
     
        public OSAEObject GetObject(string name, string authkey)
        {
            // lookup object 
            if (OSAESecurity.Authorize(authkey, name))
            {
                OSAEObject OSAEobj = OSAEObjectManager.GetObjectByName(name);
                OSAEobj.Properties = getProperties(OSAEobj.Name);
                Log.Debug("Retrieving object:  " + name + ".");
                return OSAEobj;
            }
            else
            {
                return null;
            }
        }

        public OSAEObjectState GetObjectState(string name, string authkey)
        {
            if (OSAESecurity.Authorize(authkey, name))
            {
                OSAEObjectState state = OSAEObjectStateManager.GetObjectStateValue(name);
                Log.Debug("Looking up object state:  " + name + ".  I Found " + state.StateLabel + ".");
                return state;
            }
            else
            {
                return null;
            }
        }

        public Boolean SetObjectState(string name, string state, string authkey)
        {
            string uAuth = OSAESecurity.DecryptUser(authkey);
            if (uAuth != null)
            {
                OSAEObjectStateManager.ObjectStateSet(name, state, uAuth);
                Log.Debug("Setting object state:  " + name + " set to: " + state + ".");
                return true;
            }
            else
            {
                return false;
            }
        }

        public OSAEObjectCollection GetObjectsByType(string type, string authkey)
        {
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType(type);

            foreach (OSAEObject oObj in objects)
            {
                oObj.Properties = getProperties(oObj.Name);
            }
            Log.Debug("Looking up objects of the type:  " + type + ".  I Found " + objects.Count + ".");
            return objects;
        }

        public OSAEObjectCollection GetObjectsByBaseType(string type, string authkey)
        {
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByBaseType(type);
            foreach (OSAEObject oObj in objects)
            {
                oObj.Properties = getProperties(oObj.Name);
            }
            Log.Debug("Looking up objects of the base type:  " + type + ".  I Found " + objects.Count + ".");
            return objects;
        }

        public OSAEObjectCollection GetObjectsByContainer(string container, string authkey)
        {
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByContainer(container);

            foreach (OSAEObject oObj in objects)
            {
                oObj.Properties = getProperties(oObj.Name);
            }
            Log.Debug("Looking up objects in container:  " + container + ".  I Found " + objects.Count + ".");
            return objects;
        }

        public Boolean ExecuteMethod(string name, string method, string param1, string param2, string authkey)
        {
            string uAuth = OSAESecurity.DecryptUser(authkey);
            if (uAuth != null)
            {
                // execute a method on an object 
                OSAEMethodManager.MethodQueueAdd(name, method, param1, param2, uAuth);
                Log.Debug("Executing Method:  " + name + "." + method + "." + param1 + "." + param2);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean SendPattern(string match, string authkey)
        {

            //  try
            //  {
            //     oRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(oRecognizer_SpeechRecognized);
            //oRecognizer.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>(oRecognizer_StateChanged);
            //   }
            //   catch (Exception ex)
            //   {
            //      Log.Error("Unable to configure oRecognizer", ex);
            //   }

            //   oRecognizer = OSAEGrammar.Load_Direct_Grammar(oRecognizer);
            //   oRecognizer = OSAEGrammar.Load_Voice_Grammars(oRecognizer);
            //  oRecognizer = OSAEGrammar.Load_Text_Only_Grammars(oRecognizer);

            //REPLACE WITH GRAMMAR



            // string patternName = Common.MatchPattern(match,"");
            // if (patternName != "")
            //  {
            string uAuth = OSAESecurity.DecryptUser(authkey);
            if (uAuth != null)
            {
                OSAEScriptManager.RunScript(match, "", "Rest");
                Log.Debug("Executing Script:  " + match);
                return true;
            }
            else
            {
                return false;
            }
            //  }
            //  else
            //return false;
        }

        public Boolean AddObject(string name, string alias, string description, string type, string address, string container, int mintruestlevel, string enabled, string authkey)
        {
            if (OSAESecurity.Authorize(authkey, name))
            {
                OSAEObjectManager.ObjectAdd(name, alias, description, type, address, container, mintruestlevel, StringToBoolean(enabled));
                Log.Debug("Oject Add:  " + name + ", " + alias + ", " + description + ", " + type + ", " + address + ", " + container + ", " + mintruestlevel + ", " + enabled);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean UpdateObject(string oldName, string newName, string alias, string description, string type, string address, string container, int mintruestlevel, string enabled, string authkey)
        {
            if (OSAESecurity.Authorize(authkey, oldName))
            {
                OSAEObjectManager.ObjectUpdate(oldName, newName, alias, description, type, address, container, mintruestlevel, Convert.ToBoolean(StringToBoolean(enabled)));
                Log.Debug("Oject Update:  " + oldName + ", " + newName + ", " + alias + ", " + description + ", " + type + ", " + address + ", " + container + ", " + mintruestlevel + ", " + enabled);
                return true;
            }
            else
            {
                return false;
            }
        }

        //public Boolean AddScript(string objName, string objEvent, string script)
        //{
        //    OSAEScriptManager.ObjectEventScriptAdd(objName, objEvent, script);
        //    return true;
        //}

        //public Boolean UpdateScript(string objName, string objEvent, string script)
        //{
        //    OSAEScriptManager.ObjectEventScriptUpdate(objName, objEvent, script.Replace("\n", "\r\n"));
        //    return true;
        //}

        //public Boolean UpdateNamedScript(string Name, string oldName, string script)
        //{
        //    OSAEScriptManager.NamedScriptUpdate(Name, oldName, script.Replace("\n", "\r\n"));
        //    return true;
        //}

        public OSAEObjectCollection GetPlugins(string authkey)
        {
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByBaseType("plugin");

            foreach (OSAEObject oObj in objects)
                oObj.Properties = getProperties(oObj.Name);
            return objects;
        }

        public List<string> GetSystemStates(string authkey)
        {
            if (OSAESecurity.Authorize(authkey, "System"))
            {
                List<string> states = new List<string>();
                DataSet ds = OSAESql.RunSQL("select state_name from osae_v_object_state where object_name = 'SYSTEM'");
                foreach (DataRow dr in ds.Tables[0].Rows)
                    states.Add(dr["state_name"].ToString());
            
                return states;
            }
            else
            {
                return null;
            }
        }

        public Boolean SetObjectProperty(string objName, string propName, string propVal, string authkey)
        {
            string uAuth = OSAESecurity.DecryptUser(authkey);
            if (uAuth != null)
            {
                OSAEObjectPropertyManager.ObjectPropertySet(objName, propName, propVal, uAuth);
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<string> getPropertyList(string objName, string propName, string authkey)
        {
            if (OSAESecurity.Authorize(authkey, objName))
            {
                List<string> list = new List<string>();
                DataSet ds = OSAEObjectPropertyManager.ObjectPropertyArrayGetAll(objName, propName);
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(dr["item_name"].ToString());
            
                return list;
            }
            else
            {
                return null;
            }
        }

        private OSAEObjectPropertyCollection getProperties(string objName)
        {
            OSAEObject oObj = OSAEObjectManager.GetObjectByName(objName);
            OSAEObjectPropertyCollection props = oObj.Properties;
            OSAEObjectPropertyCollection properties = new OSAEObjectPropertyCollection();

            foreach (OSAEObjectProperty prop in props)
            {
                OSAEObjectProperty p = new OSAEObjectProperty();
                p.Name = prop.Name;
                p.Value = prop.Value;
                p.DataType = prop.DataType;
                p.LastUpdated = prop.LastUpdated; 
                p.Id = prop.Id;
                properties.Add(p);
            }
            return properties;
        }

        private bool StringToBoolean(string passedvalue)
        {
            bool booleanvalue = false;  //if we fail to convert we will just default to false

            if (!bool.TryParse(passedvalue, out booleanvalue)) //if they passed "true"/"false" this will work and booleanvalue will contain our converted value
            {
                // otherwise it is probably a "1" or "0" and we will try to convert that to boolean
                int intvalue;
                if (Int32.TryParse(passedvalue, out intvalue))
                    booleanvalue = Convert.ToBoolean(intvalue);
            }

            return booleanvalue;
        }

        public List<OSAEPropertyHistory> GetPropertyHistory(string objName, string propName, string from, string to, string authkey)
        {
            if (OSAESecurity.Authorize(authkey, objName))
            {
                List<OSAEPropertyHistory> list = new List<OSAEPropertyHistory>();
                DataSet ds = OSAEObjectPropertyManager.ObjectPropertyHistoryGet(objName, propName, from, to);
                OSAEPropertyHistory ph = new OSAEPropertyHistory();
                ph.label = objName + " - " + propName;
                List<List<double>> vals = new List<List<double>>();
                ph.data = vals;

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List<double> p = new List<double>();
                    p.Add((double)Common.GetJavascriptTimestamp(DateTime.Parse(dr["history_timestamp"].ToString())));
                    p.Add(double.Parse(dr["property_value"].ToString()));
                    vals.Add(p);
                }
                list.Add(ph);
                return list;
            }
            else
            {
                return null;
            }
        }

        public List<OSAEStateHistory> GetStateHistory(string objName, string from, string to, string authkey)
        {
            if (OSAESecurity.Authorize(authkey, objName))
            {
                List<OSAEStateHistory> list = new List<OSAEStateHistory>();
                DataSet ds = OSAEObjectStateManager.ObjectStateHistoryGet(objName, from, to);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    OSAEStateHistory sh = new OSAEStateHistory();
                    sh.obj = dr["object_name"].ToString();
                    sh.state = dr["state_label"].ToString();
                    sh.datetime = (double)Common.GetJavascriptTimestamp(DateTime.Parse(dr["history_timestamp"].ToString()));
                    list.Add(sh);
                }
                return list;
            }
            else
            {
                return null;
            }
        }

        private void oRecognizer_SpeechRecognized(object sender, System.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            string gCurrentUser = "Unidentified Person";
            try
            {
                RecognitionResult result = e.Result;
                SemanticValue semantics = e.Result.Semantics;
                string scriptParameter = "";
                if (e.Result.Semantics.ContainsKey("PARAM1"))
                {
                    string temp = e.Result.Semantics["PARAM1"].Value.ToString().Replace("'s", "").Replace("'S", "");
                    if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gCurrentUser;
                    if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                    scriptParameter = temp;
                    if (e.Result.Semantics.ContainsKey("PARAM2"))
                    {
                        temp = e.Result.Semantics["PARAM2"].Value.ToString().Replace("'s", "").Replace("'S", "");
                        if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gCurrentUser;
                        if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                        scriptParameter += "," + temp;
                        if (e.Result.Semantics.ContainsKey("PARAM3"))
                        {
                            temp = e.Result.Semantics["PARAM3"].Value.ToString().Replace("'s", "").Replace("'S", "");
                            if (temp.ToUpper() == "I" || temp.ToUpper() == "ME" || temp.ToUpper() == "MY") temp = gCurrentUser;
                            if (temp.ToUpper() == "YOU" || temp.ToUpper() == "YOUR") temp = "SYSTEM";
                            scriptParameter += "," + temp;
                        }
                    }
                }
                string sResults = "";
                    if (result.Grammar.Name.ToString() == "Direct Match")
                    {
                        Log.Debug("Searching for: " + sResults);
                        sResults = OSAEGrammar.SearchForMeaning(result.Text, scriptParameter, gCurrentUser);
                    }
                    else
                    {
                        Log.Debug("Searching for: " + sResults);
                        sResults = OSAEGrammar.SearchForMeaning(result.Grammar.Name.ToString(), scriptParameter, gCurrentUser);
                    }

                Log.Info("Search Results: " + sResults);
            }
            catch (Exception ex)
            { Log.Error("Error in _SpeechRecognized!", ex); }
        }
    }

    public class OSAEPropertyHistory
    {
        public string label;
        public List<List<double>> data;
    }

    public class OSAEStateHistory
    {
        public string obj;
        public string state;
        public double datetime;
    }
}
