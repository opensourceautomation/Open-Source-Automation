namespace OSAE
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    /// <summary>
    /// Collection class for managing an OSAEMethod of an OSA object 
    /// </summary>
    public class OSAEMethodCollection : Collection<OSAEMethod>
    {
        public void AddRange(List<OSAEMethod> osaeObjects)
        {
            foreach (OSAEMethod obj in osaeObjects)
                this.Add(obj);
        }

        public OSAEMethod Find(string name)
        {
            foreach (OSAEMethod obj in this.Items)
            {
                if (obj.MethodName == name) return obj;
            }
            return null;
        }

        public OSAEMethod this[string key]
        {
            get { return this.Find(key); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (OSAEMethod obj in this.Items)
                sb.AppendLine(obj.MethodName);

            return sb.ToString();
        }
    }
}
