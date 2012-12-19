using System.Collections.Generic;

namespace OSAE
{
    /// <summary>
    /// 
    /// </summary>
    public class OSAEObject
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Name { get ; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string Type { get ; set; }      
       
        /// <summary>
        /// 
        /// </summary>
        public string BaseType { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string Address { get ; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public string Container  { get ; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public int Enabled  { get ; set; }      
       
        /// <summary>
        /// 
        /// </summary>
        public ObjectState State  { get ; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public string LastUpd { get ; set; }
      
        /// <summary>
        /// 
        /// </summary>
        public List<ObjectProperty> Properties  { get ; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public List<string> Methods  { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        private OSAE osae = new OSAE("API");

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="type"></param>
        /// <param name="address"></param>
        /// <param name="container"></param>
        /// <param name="enabled"></param>
        public OSAEObject(string name, string description, string type, string address, string container, int enabled)
        {
            Name = name;
            Type = type;
            Address = address;
            Container = container;
            Enabled = enabled;
            Description = description;
            State = new ObjectState();
        }

        /// <summary>
        /// 
        /// </summary>
        public OSAEObject()
        {
            State = new ObjectState();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Delete()
        {
            osae.ObjectDelete(Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetState(string state)
        {
            osae.ObjectStateSet(Name, state);
            State = osae.GetObjectStateValue(Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        public void SetProperty(string prop, string value)
        {
            osae.ObjectPropertySet(Name, prop, value);
            foreach (ObjectProperty p in Properties)
            {
                if (p.Name == prop)
                {
                    p.Value = value;
                }
            }
        }
    }
}
