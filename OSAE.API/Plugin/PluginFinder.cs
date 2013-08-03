namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Safely identifies assemblies within a designated plugin directory that contain qualifying plugin types.
    /// </summary>
    public class PluginFinder : MarshalByRefObject
    {        
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
        public IEnumerable<TypeLocator> Find()
        {
            var result = new List<TypeLocator>();

            foreach (var file in Directory.GetFiles(Common.ApiPath + "\\" + PluginPath, "*.dll", SearchOption.AllDirectories))
            {
                Logging.GetLogger("Plugin Loader").AddToLog("DLL Found while loading Plugins:" + file, false);

                try
                {
                    var assembly = Assembly.LoadFrom(file);

                    foreach (var type in assembly.GetExportedTypes())
                    {
                        Logging.GetLogger("Plugin Loader").AddToLog("Exposed Type: " + type, false);

                        if (!type.Equals(_pluginBaseType) &&
                            _pluginBaseType.IsAssignableFrom(type))
                        {
                            result.Add(new TypeLocator(assembly.FullName, type.FullName, file));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // This method is called in its own App Domain so will not have access to the calling logger
                    Logging.GetLogger("Plugin Loader").AddToLog("An assembly was not found for file:" + file, true);
                    Logging.GetLogger("Plugin Loader").AddToLog( ex.Message, true);
                }
            }

            return result;
        }
    }    
}
