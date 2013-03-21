namespace OSAE
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

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
    }
}
