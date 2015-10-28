namespace OSAE
{

    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class OSAEUserControlCollection : Collection<OSAEUserControl>
    {
        public void AddRange(List<OSAEUserControl> osaeObjects)
        {
            foreach (OSAEUserControl obj in osaeObjects)
            {
                this.Add(obj);
            }
        }

        public OSAEUserControl Find(string name)
        {
            foreach (OSAEUserControl obj in this.Items)
            {
                if (obj.UserControlName == name)
                {
                    return obj;
                }
            }

            return null;
        }

        public OSAEUserControl Find(string name, bool enabled)
        {
            foreach (OSAEUserControl obj in this.Items)
            {
                if (obj.UserControlName == name && obj.Enabled == enabled)
                {
                    return obj;
                }
            }

            return null;
        }

        public OSAEUserControl this[string key]
        {
            get
            {
                return this.Find(key);
            }
        }
    }
}
