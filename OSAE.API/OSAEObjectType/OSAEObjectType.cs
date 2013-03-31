namespace OSAE
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class OSAEObjectType
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string BaseType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string OwnedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>  
        [DataMember]      
        public bool Owner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Container { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool SysType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool HideRedundant { get; set; }
    }
}
