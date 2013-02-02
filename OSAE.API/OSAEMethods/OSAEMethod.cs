namespace OSAE
{
    using System;

    /// <summary>
    /// Method passed to plugins to allow them to process an event in the system
    /// </summary>
    [Serializable]
    public class OSAEMethod
    {
        /// <summary>
        /// The name of the method
        /// </summary>
        public string MethodName { get; set; }
       
        public string ObjectName { get; set; }

        /// <summary>
        /// The first parameter passed which may contain information that
        /// the plugin needs to process the request
        /// </summary>
        public string Parameter1 { get; set; }
       
        /// <summary>
        /// Second parameter passed which may contain information that 
        /// the plugin needs to process the request
        /// </summary>
        public string Parameter2 { get; set; }
       
        public string Address { get; set; }
       
        public string Owner { get; set; }
       
        public OSAEMethod(string methodName, string objName, string param1, string param2, string address, string owner)
        {
            MethodName = methodName;
            ObjectName = objName;
            Parameter1 = param1;
            Parameter2 = param2;
            Address = address;
            Owner = owner;
        }
    }
}
