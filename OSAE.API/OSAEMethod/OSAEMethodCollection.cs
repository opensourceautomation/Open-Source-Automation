namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

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
    }
}
