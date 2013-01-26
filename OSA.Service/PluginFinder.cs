using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using OSAE;


namespace OSAE.Service
{
    
    /// <summary>
    /// Safely identifies assemblies within a designated plugin directory that contain qualifying plugin types.
    /// </summary>
    internal class PluginFinder : MarshalByRefObject
    {
        Logging2 log = Logging2.GetLogger("PluginFinder");
        OSAE osae = new OSAE("Service");
        internal const string PluginPath = "Plugins";

        private readonly Type _pluginBaseType;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginFinder"/> class.
        /// </summary>
        public PluginFinder()
        {
            _pluginBaseType = typeof(OSAEPluginBase);
        }

        /// <summary>
        /// Returns the name and assembly name of qualifying plugin classes found in assemblies within the designated plugin directory.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TypeLocator}"/> that represents the qualifying plugin types.</returns>
        public static IEnumerable<TypeLocator> FindPlugins()
        {
            AppDomain domain = null;

            try
            {
                domain = AppDomain.CreateDomain("Discovery Domain");

                var finder = (PluginFinder)domain.CreateInstanceAndUnwrap(typeof(PluginFinder).Assembly.FullName, typeof(PluginFinder).FullName);
                return finder.Find();
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }
        }

        /// <summary>
        /// Surveys the configured plugin path and returns the the set of types that qualify as plugin classes.
        /// </summary>
        /// <remarks>
        /// Since this method loads assemblies, it must be called from within a dedicated application domain that is subsequently unloaded.
        /// </remarks>
        private IEnumerable<TypeLocator> Find()
        {
            var result = new List<TypeLocator>();

            foreach (var file in Directory.GetFiles(Common.ApiPath + "\\" + PluginPath, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);

                    foreach (var type in assembly.GetExportedTypes())
                    {
                        if (!type.Equals(_pluginBaseType) &&
                            _pluginBaseType.IsAssignableFrom(type))
                        {
                            result.Add(new TypeLocator(assembly.FullName, type.FullName, file));
                        }
                    }
                }
                catch (Exception e)
                {
                    // Ignore DLLs that are not .NET assemblies.
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Encapsulates the assembly name and type name for a <see cref="Type"/> in a serializable format.
    /// </summary>
    [Serializable]
    internal class TypeLocator
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
            if (string.IsNullOrEmpty(assemblyName)) throw new ArgumentNullException("assemblyName");
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentNullException("typeName");

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