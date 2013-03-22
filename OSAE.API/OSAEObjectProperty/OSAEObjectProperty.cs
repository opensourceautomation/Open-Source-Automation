namespace OSAE
{
    /// <summary>
    /// This class represents and OSA object Property
    /// </summary>
    public class OSAEObjectProperty
    {
        /// <summary>
        /// The db Id of the property
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Value of the property
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The data type associated to the property
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// When the peroperty was last updated.
        /// </summary>
        public string LastUpdated { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
