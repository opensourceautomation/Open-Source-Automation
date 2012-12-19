using System;

namespace OSAE
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OSAEMethod
    {
        /// <summary>
        /// 
        /// </summary>
        public string MethodName { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string ObjectName { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string Parameter1 { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string Parameter2 { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string Address { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string Owner { get ; set; }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="objName"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="address"></param>
        /// <param name="owner"></param>
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
