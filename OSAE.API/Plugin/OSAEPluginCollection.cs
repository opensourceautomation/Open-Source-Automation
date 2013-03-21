namespace OSAE
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class OSAEPluginCollection : Collection<Plugin>
    {
        public void AddRange(List<Plugin> osaeObjects)
        {
            foreach (Plugin obj in osaeObjects)
            {
                this.Add(obj);
            }
        }

        public Plugin Find(string name)
        {
            foreach (Plugin obj in this.Items)
            {
                if (obj.PluginName == name)
                {
                    return obj;
                }
            }

            return null;
        }

        public Plugin Find(string name, bool enabled)
        {
            foreach (Plugin obj in this.Items)
            {
                if (obj.PluginName == name && obj.Enabled == enabled)
                {
                    return obj;
                }
            }

            return null;
        }

        public Plugin this[string key]
        {
            get
            {
                return this.Find(key);
            }
        }
    }
}
