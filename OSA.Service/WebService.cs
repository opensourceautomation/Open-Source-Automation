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
        Object GetObject(string name);
        
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
        List<Object> GetObjectsByType(string type);

        [OperationContract]
        [WebGet(UriTemplate = "objects/container/{container}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Object> GetObjectsByContainer(string container);

        [OperationContract]
        [WebGet(UriTemplate = "object/propertylist/{objName}/{propName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> getPropertyList(string objName, string propName);

        [OperationContract]
        [WebGet(UriTemplate = "plugins", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Object> GetPlugins();

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

        public Object GetObject(string name)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();

            // lookup object 
            OSAEObject OSAEobj = objectManager.GetObjectByName(name);
            Object obj = new Object();
            obj.Name = OSAEobj.Name;
            obj.Address = OSAEobj.Address;
            obj.Type = OSAEobj.Type;
            obj.BaseType = OSAEobj.BaseType;
            obj.Container = OSAEobj.Container;
            obj.Enabled = OSAEobj.Enabled;
            obj.Description = OSAEobj.Description;
            obj.Methods = OSAEobj.Methods;
            obj.State = OSAEobj.State;
            obj.Properties = getProperties(obj.Name);
            return obj;
        }

        public ObjectState GetObjectState(string name)
        {
            ObjectState state = osae.GetObjectStateValue(name);
            return state;
        }

        public List<Object> GetObjectsByType(string type)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            // lookup objects of the requested type 
            List<Object> objList = new List<Object>();
            List<OSAEObject> objects = objectManager.GetObjectsByType(type);

            foreach (OSAEObject oObj in objects)
            {
                Object obj = new Object();
                obj.Name = oObj.Name;
                obj.Address = oObj.Address;
                obj.Type = oObj.Type;
                obj.BaseType = oObj.BaseType;
                obj.Container = oObj.Container;
                obj.Enabled = oObj.Enabled;
                obj.Description = oObj.Description;
                obj.State = oObj.State;
                obj.Methods = oObj.Methods;

                obj.Properties = getProperties(obj.Name);
                
                objList.Add(obj);
            }

            return objList;
        }

        public List<Object> GetObjectsByContainer(string container)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            // lookup objects of the requested type 
            List<Object> objList = new List<Object>();
            List<OSAEObject> objects = objectManager.GetObjectsByContainer(container);

            foreach (OSAEObject oObj in objects)
            {
                Object obj = new Object();
                obj.Name = oObj.Name;
                obj.Address = oObj.Address;
                obj.Type = oObj.Type;
                obj.BaseType = oObj.BaseType;
                obj.Container = oObj.Container;
                obj.Enabled = oObj.Enabled;
                obj.Description = oObj.Description;
                obj.State = oObj.State;
                obj.Methods = oObj.Methods;

                obj.Properties = getProperties(obj.Name);

                objList.Add(obj);
            }

            return objList;
        }

        public Boolean ExecuteMethod(string name, string method, string param1, string param2)
        {
            // execute a method on an object 
            osae.MethodQueueAdd(name, method, param1, param2);
            return true;
        }

        public Boolean SendPattern(string match)
        {
            string patternName = osae.MatchPattern(match);
            if (patternName != "")
            {
                osae.MethodQueueAdd("Script Processor", "NAMED SCRIPT", patternName, "");
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

        public List<Object> GetPlugins()
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            // lookup objects of the requested type 
            List<Object> objList = new List<Object>();
            List<OSAEObject> objects = objectManager.GetObjectsByBaseType("plugin");

            foreach (OSAEObject oObj in objects)
            {
                Object obj = new Object();
                obj.Name = oObj.Name;
                obj.Address = oObj.Address;
                obj.Type = oObj.Type;
                obj.Container = oObj.Container;
                obj.Enabled = oObj.Enabled;
                obj.Description = oObj.Description;
                obj.State = oObj.State;
                obj.Methods = oObj.Methods;

                obj.Properties = getProperties(obj.Name);

                objList.Add(obj);
            }

            return objList;
        }

        public List<string> GetSystemStates()
        {
            List<string> states = new List<string>();

            DataSet ds = osae.RunSQL("select state_name from osae_v_object_state where object_name = 'SYSTEM'");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                states.Add(dr["state_name"].ToString());
            }
            return states;
        }

        public Boolean SetObjectProperty(string objName, string propName, string propVal)
        {
            osae.ObjectPropertySet(objName, propName, propVal);

            return true;
        }

        public List<string> getPropertyList(string objName, string propName)
        {
            List<string> list = new List<string>();
            DataSet ds = osae.ObjectPropertyArrayGetAll(objName, propName);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                list.Add(dr["item_name"].ToString());
            }

            return list;
        }
        
        private List<Property> getProperties(string objName)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            OSAEObject oObj = objectManager.GetObjectByName(objName);
            List<ObjectProperty> props = oObj.Properties;
            List<Property> properties = new List<Property>();

            foreach (ObjectProperty prop in props)
            {
                Property p = new Property();
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

    public class Object
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string BaseType { get; set; }
        public string Container { get; set; }
        public int Enabled { get; set; }
        public string Description { get; set; }
        public ObjectState State { get; set; }
        public List<Property> Properties { get; set; }
        public List<string> Methods { get; set; }
    }

    public class Property
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
        public string LastUpdated { get; set; }
    }

    public class ListItem
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}
