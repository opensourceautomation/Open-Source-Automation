namespace OSAE
{
    using System;
    using System.IO;

    /// <summary>
    /// Encapsulates the assembly name and type name for a <see cref="Type"/> in a serializable format.
    /// </summary>
    [Serializable]
    public class TypeLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeLocator"/> class.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly containing the target type.</param>
        /// <param name="typeName">The name of the target type.</param>
        public TypeLocator(
            string assemblyName,
            string typeName,
            string path)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullException("assemblyName");
            }

            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName");
            }

            AssemblyName = assemblyName;
            TypeName = typeName;
            Location = Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Gets the name of the assembly containing the target type.
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Gets the name of the target type.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets the Location of assembly
        /// </summary>
        public string Location { get; private set; }
    }
}
