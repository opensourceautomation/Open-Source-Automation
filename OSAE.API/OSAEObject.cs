using System.Collections.Generic;

namespace OSAE
{
    public class OSAEObject
    {
        #region Properties

        public string Name { get ; set; }

        public string Description { get ; set; }
       
        public string Type { get ; set; }      
       
        public string BaseType { get ; set; }
       
        public string Address { get ; set; }
               
        public string Container  { get ; set; }
              
        public int Enabled  { get ; set; }      
       
        public ObjectState State  { get ; set; }
            
        public string LastUpd { get ; set; }
      
        public List<ObjectProperty> Properties  { get ; set; }
            
        public List<string> Methods  { get ; set; }
       
        private OSAE osae = new OSAE("API");
        #endregion

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

        public OSAEObject()
        {
            State = new ObjectState();
        }

        public void Delete()
        {
            osae.ObjectDelete(Name);
        }

        public void SetState(string state)
        {
            osae.ObjectStateSet(Name, state);
            State = osae.GetObjectStateValue(Name);
        }

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
