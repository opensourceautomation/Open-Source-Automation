namespace OSAE
{

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class used to hold an instance of an OSAObject Property
    /// </summary>
    [Serializable, DataContract]
    public class OSAEObjectProperty
    {
        /// <summary>
        /// The db Id of the property
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// The name of the property
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The Value of the property
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// The data type associated to the property
        /// </summary>
        [DataMember]
        public string DataType { get; set; }

        /// <summary>
        /// When the peroperty was last updated.
        /// </summary>
        [DataMember]
        public string LastUpdated { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
