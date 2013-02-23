namespace OSAE
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class OSAEObjectPropertyCollection : Collection<OSAEObjectProperty>
    {
        public void AddRange(List<OSAEObjectProperty> osaeObjects)
        {
            foreach (OSAEObjectProperty obj in osaeObjects)
            {
                this.Add(obj);
            }
        }

        public OSAEObjectProperty Find(string name)
        {
            foreach (OSAEObjectProperty obj in this.Items)
            {
                if (obj.Name == name)
                {
                    return obj;
                }
            }

            return null;
        }
    }
}
