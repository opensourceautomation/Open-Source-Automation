namespace OSAERest
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using OSAE;
    
    [ServiceContract]
    public interface IRestService
    {
        [OperationContract]
        [WebGet(UriTemplate = "object/{name}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObject GetObject(string name);
        
        [OperationContract]
        [WebGet(UriTemplate = "object/{name}/state", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectState GetObjectState(string name);

        [OperationContract]
        [WebInvoke(UriTemplate = "object/{name}/{method}?param1={param1}&param2={param2}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean ExecuteMethod(string name, string method, string param1, string param2);

        [OperationContract]
        [WebInvoke(UriTemplate = "object/add?name={name}&desc={description}&type={type}&address={address}&container={container}&enabled={enabled}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean AddObject(string name, string description, string type, string address, string container, string enabled);

        [OperationContract]
        [WebInvoke(UriTemplate = "object/update?oldName={oldName}&newName={newName}&desc={description}&type={type}&address={address}&container={container}&enabled={enabled}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean UpdateObject(string oldName, string newName, string description, string type, string address, string container, string enabled);

        [OperationContract]
        [WebGet(UriTemplate = "objects/type/{type}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectCollection GetObjectsByType(string type);

        [OperationContract]
        [WebGet(UriTemplate = "objects/container/{container}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectCollection GetObjectsByContainer(string container);

        [OperationContract]
        [WebGet(UriTemplate = "object/propertylist/{objName}/{propName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> getPropertyList(string objName, string propName);

        [OperationContract]
        [WebGet(UriTemplate = "plugins", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        OSAEObjectCollection GetPlugins();

        [OperationContract]
        [WebInvoke(UriTemplate = "namedscript/{match}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean SendPattern(string match);

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
        [WebGet(UriTemplate = "system/states", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> GetSystemStates();

        [OperationContract]        
        [WebInvoke(UriTemplate = "property/update?objName={objName}&propName={propName}&propVal={propVal}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean SetObjectProperty(string objName, string propName, string propVal);        
    }

    public class api : IRestService
    {
        public OSAEObject GetObject(string name)
        {
            // lookup object 
            OSAEObject OSAEobj = OSAEObjectManager.GetObjectByName(name);
            OSAEobj.Properties = getProperties(OSAEobj.Name);
            return OSAEobj;
        }

        public OSAEObjectState GetObjectState(string name)
        {
            OSAEObjectState state = OSAEObjectStateManager.GetObjectStateValue(name);
            return state;
        }

        public OSAEObjectCollection GetObjectsByType(string type)
        {
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType(type);

            foreach (OSAEObject oObj in objects)
            {
                oObj.Properties = getProperties(oObj.Name);
            }

            return objects;
        }

        public OSAEObjectCollection GetObjectsByContainer(string container)
        {
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByContainer(container);

            foreach (OSAEObject oObj in objects)
            {
                oObj.Properties = getProperties(oObj.Name);
            }

            return objects;
        }

        public Boolean ExecuteMethod(string name, string method, string param1, string param2)
        {
            // execute a method on an object 
            OSAEMethodManager.MethodQueueAdd(name, method, param1, param2, "REST Service");
            return true;
        }

        public Boolean SendPattern(string match)
        {
            string patternName = Common.MatchPattern(match);
            if (patternName != "")
            {
                OSAEScriptManager.RunPatternScript(patternName, "", "REST Service");
                return true;
            }
            else
                return false;
        }

        public Boolean AddObject(string name, string description, string type, string address, string container, string enabled)
        {

            OSAEObjectManager.ObjectAdd(name, description, type, address, container, StringToBoolean(enabled));

            return true;
        }

        public Boolean UpdateObject(string oldName, string newName, string description, string type, string address, string container, string enabled)
        {
            OSAEObjectManager.ObjectUpdate(oldName, newName, description, type, address, container, Convert.ToInt32(StringToBoolean(enabled)));

            return true;
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

        public OSAEObjectCollection GetPlugins()
        {
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByBaseType("plugin");

            foreach (OSAEObject oObj in objects)
            {
                oObj.Properties = getProperties(oObj.Name);
            }

            return objects;
        }

        public List<string> GetSystemStates()
        {
            List<string> states = new List<string>();

            DataSet ds = OSAESql.RunSQL("select state_name from osae_v_object_state where object_name = 'SYSTEM'");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                states.Add(dr["state_name"].ToString());
            }

            return states;
        }

        public Boolean SetObjectProperty(string objName, string propName, string propVal)
        {
            OSAEObjectPropertyManager.ObjectPropertySet(objName, propName, propVal, "Web ");

            return true;
        }

        public List<string> getPropertyList(string objName, string propName)
        {         
            List<string> list = new List<string>();
            DataSet ds = OSAEObjectPropertyManager.ObjectPropertyArrayGetAll(objName, propName);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(dr["item_name"].ToString());
            }

            return list;
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

        private Boolean StringToBoolean(string passedvalue)
        {

            Boolean booleanvalue = false;  //if we fail to convert we will just default to false

            if (!Boolean.TryParse(passedvalue, out booleanvalue)) //if they passed "true"/"false" this will work and booleanvalue will contain our converted value
            {
                // otherwise it is probably a "1" or "0" and we will try to convert that to boolean
                int intvalue;
                if (Int32.TryParse(passedvalue, out intvalue))
                {
                    booleanvalue = Convert.ToBoolean(intvalue);
                }
            }

            return booleanvalue;
        }

    }


}
