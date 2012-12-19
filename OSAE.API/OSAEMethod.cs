using System;

namespace OSAE
{
    [Serializable]
    public class OSAEMethod
    {
        public string MethodName { get ; set; }
       
        public string ObjectName { get ; set; }
       
        public string Parameter1 { get ; set; }
       
        public string Parameter2 { get ; set; }
       
        public string Address { get ; set; }
       
        public string Owner { get ; set; }
       
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
