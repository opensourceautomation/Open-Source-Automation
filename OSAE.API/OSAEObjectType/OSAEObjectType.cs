namespace OSAE
{
    public class OSAEObjectType
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BaseType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OwnedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>        
        public bool Owner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Container { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool SysType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HideRedundant { get; set; }
    }
}
