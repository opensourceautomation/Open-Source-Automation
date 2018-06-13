namespace OSAE
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    /// <summary>
    /// Collection class for management of an OSAEObjectProperty
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (OSAEObjectProperty obj in this.Items)
            {
                sb.AppendLine(obj.Name);
            }

            return sb.ToString();
        }
    }
}
