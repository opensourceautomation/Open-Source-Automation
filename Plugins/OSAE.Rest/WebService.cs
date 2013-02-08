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
        ObjectState GetObjectState(string name);

        [OperationContract]
        [WebGet(UriTemplate = "object/{name}/{method}?param1={param1}&param2={param2}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean ExecuteMethod(string name, string method, string param1, string param2);

        [OperationContract]
        [WebGet(UriTemplate = "object/add?name={name}&desc={description}&type={type}&address={address}&container={container}&enabled={enabled}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean AddObject(string name, string description, string type, string address, string container, string enabled);

        [OperationContract]
        [WebGet(UriTemplate = "object/update?oldName={oldName}&newName={newName}&desc={description}&type={type}&address={address}&container={container}&enabled={enabled}",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean UpdateObject(string oldName, string newName, string description, string type, string address, string container, string enabled);

        [OperationContract]
        [WebGet(UriTemplate = "objects/type/{type}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<OSAEObject> GetObjectsByType(string type);

        [OperationContract]
        [WebGet(UriTemplate = "objects/container/{container}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<OSAEObject> GetObjectsByContainer(string container);

        [OperationContract]
        [WebGet(UriTemplate = "object/propertylist/{objName}/{propName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> getPropertyList(string objName, string propName);

        [OperationContract]
        [WebGet(UriTemplate = "plugins", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<OSAEObject> GetPlugins();

        [OperationContract]
        [WebGet(UriTemplate = "namedscript/{match}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean SendPattern(string match);

        [OperationContract]
        [WebGet(UriTemplate = "namedscript/update?name={name}&oldName={oldName}&script={script}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean UpdateNamedScript(string name, string oldName, string script);
        
        [OperationContract]
        [WebGet(UriTemplate = "script/add?obj={objName}&event={objEvent}&script={script}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean AddScript(string objName, string objEvent, string script);

        [OperationContract]
        [WebGet(UriTemplate = "script/update?obj={objName}&event={objEvent}&script={script}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean UpdateScript(string objName, string objEvent, string script);

        [OperationContract]
        [WebGet(UriTemplate = "system/states", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> GetSystemStates();

        [OperationContract]
        [WebGet(UriTemplate = "property/update?objName={objName}&propName={propName}&propVal={propVal}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Boolean SetObjectProperty(string objName, string propName, string propVal);        
    }

    public class api : IRestService
    {
        OSAE osae = new OSAE("WebService");

        public OSAEObject GetObject(string name)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            // lookup object 
            OSAEObject OSAEobj = objectManager.GetObjectByName(name);
            OSAEobj.Properties = getProperties(OSAEobj.Name);
            return OSAEobj;
        }

        public ObjectState GetObjectState(string name)
        {
            ObjectState state = osae.GetObjectStateValue(name);
            return state;
        }

        public List<OSAEObject> GetObjectsByType(string type)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();

            List<OSAEObject> objects = objectManager.GetObjectsByType(type);

            foreach (OSAEObject oObj in objects)
            {
                oObj.Properties = getProperties(oObj.Name);
            }

            return objects;
        }

        public List<OSAEObject> GetObjectsByContainer(string container)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            List<OSAEObject> objects = objectManager.GetObjectsByContainer(container);

            foreach (OSAEObject oObj in objects)
            {
                oObj.Properties = getProperties(oObj.Name);
            }

            return objects;
        }

        public Boolean ExecuteMethod(string name, string method, string param1, string param2)
        {
            // execute a method on an object 
            OSAEMethodManager.MethodQueueAdd(name, method, param1, param2, "WCF Service");
            return true;
        }

        public Boolean SendPattern(string match)
        {
            string patternName = osae.MatchPattern(match);
            if (patternName != "")
            {
                OSAEMethodManager.MethodQueueAdd("Script Processor", "NAMED SCRIPT", patternName, "", "WCF Service");
                return true;
            }
            else
                return false;
        }

        public Boolean AddObject(string name, string description, string type, string address, string container, string enabled)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            objectManager.ObjectAdd(name, description, type, address, container, Convert.ToBoolean(enabled));

            return true;
        }

        public Boolean UpdateObject(string oldName, string newName, string description, string type, string address, string container, string enabled)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            objectManager.ObjectUpdate(oldName, newName, description, type, address, container, Int32.Parse(enabled));

            return true;
        }

        public Boolean AddScript(string objName, string objEvent, string script)
        {
            osae.ObjectEventScriptAdd(objName, objEvent, script);
            return true;
        }

        public Boolean UpdateScript(string objName, string objEvent, string script)
        {
            osae.ObjectEventScriptUpdate(objName, objEvent, script.Replace("\n", "\r\n"));
            return true;
        }

        public Boolean UpdateNamedScript(string Name, string oldName, string script)
        {
            osae.NamedScriptUpdate(Name, oldName, script.Replace("\n", "\r\n"));
            return true;
        }

        public List<OSAEObject> GetPlugins()
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            List<OSAEObject> objects = objectManager.GetObjectsByBaseType("plugin");

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
            ObjectPopertiesManager.ObjectPropertySet(objName, propName, propVal, "Web ");

            return true;
        }

        public List<string> getPropertyList(string objName, string propName)
        {         
            List<string> list = new List<string>();
            DataSet ds = ObjectPopertiesManager.ObjectPropertyArrayGetAll(objName, propName);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(dr["item_name"].ToString());
            }

            return list;
        }

        private List<ObjectProperty> getProperties(string objName)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            OSAEObject oObj = objectManager.GetObjectByName(objName);
            List<ObjectProperty> props = oObj.Properties;
            List<ObjectProperty> properties = new List<ObjectProperty>();

            foreach (ObjectProperty prop in props)
            {
                ObjectProperty p = new ObjectProperty();
                p.Name = prop.Name;
                p.Value = prop.Value;
                p.DataType = prop.DataType;
                p.LastUpdated = prop.LastUpdated; 
                p.Id = prop.Id;
                properties.Add(p);
            }
            return properties;
        }
    }
}
