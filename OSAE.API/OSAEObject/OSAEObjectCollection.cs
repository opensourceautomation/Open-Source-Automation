namespace OSAE
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    /// <summary>
    /// Collection class for management of an OSAEObject of OSA 
    /// </summary>
    public class OSAEObjectCollection : Collection<OSAEObject>
    {
        public void AddRange(List<OSAEObject> osaeObjects)
        {
            foreach (OSAEObject obj in osaeObjects)
            {
                this.Add(obj);                   
            }
        }

        public OSAEObject Find(string name)
        {
            foreach (OSAEObject obj in this.Items)
            {
                if (obj.Name == name)
                {
                    return obj;
                }
            }

            return null;
        }

        public OSAEObject this[string key]
        {
            get
            {
                return this.Find(key);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach(OSAEObject obj in this.Items)
            {
                sb.AppendLine(obj.Name);
            }
            
            return sb.ToString();
        }
    }
}
