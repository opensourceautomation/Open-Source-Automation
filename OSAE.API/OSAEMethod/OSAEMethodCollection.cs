namespace OSAE
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class OSAEMethodCollection : Collection<OSAEMethod>
    {
        public void AddRange(List<OSAEMethod> osaeObjects)
        {
            foreach (OSAEMethod obj in osaeObjects)
            {
                this.Add(obj);
            }
        }

        public OSAEMethod Find(string name)
        {
            foreach (OSAEMethod obj in this.Items)
            {
                if (obj.MethodName == name)
                {
                    return obj;
                }
            }

            return null;
        }

        public OSAEMethod this[string key]
        {
            get
            {
                return this.Find(key);
            }
        }
    }
}
