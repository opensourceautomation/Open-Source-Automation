namespace OSAE
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Collection class for managing properties of an OSA object
    /// </summary>
    public class OSAEObjectPropertyCollection : Collection<OSAEObjectProperty>
    {
        public void AddRange(List<OSAEObjectProperty> osaeObjects)
        {
            foreach (OSAEObjectProperty obj in osaeObjects)
            {
                this.Add(obj);
            }
        }

        /// <summary>
        /// Finds a property based on its name
        /// </summary>
        /// <param name="name">The name of the property to find</param>
        /// <returns>The requested property</returns>
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

        public OSAEObjectProperty this[string key]
        {
            get
            {
                return this.Find(key);
            }
        }
    }
}
