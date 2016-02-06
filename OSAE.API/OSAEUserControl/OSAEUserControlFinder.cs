namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Diagnostics;

    /// <summary>
    /// Safely identifies assemblies within the designated UserControl directory that contain qualifying plugin types.
    /// </summary>
    public class OSAEUserControlFinder : MarshalByRefObject
    {
        internal const string UserControlPath = "UserControls";
        private readonly Type _usercontrolBaseType;

        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("SYSTEM");

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginFinder"/> class.
        /// </summary>
        public OSAEUserControlFinder()
        {
            _usercontrolBaseType = typeof(OSAEUserControlBase);
        }

        /// <summary>
        /// Returns the name and assembly name of qualifying usercontrol classes found in assemblies within the designated plugin directory.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TypeLocator}"/> that represents the qualifying plugin types.</returns>
        public static IEnumerable<TypeLocator> FindUserControls()
        {
            AppDomain domain = null;
            try
            {
                domain = AppDomain.CreateDomain("Discovery Domain");
                var finder = (OSAEUserControlFinder)domain.CreateInstanceAndUnwrap(typeof(OSAEUserControlFinder).Assembly.FullName, typeof(OSAEUserControlFinder).FullName);
                return finder.Find();
            }
            finally
            { if (domain != null) AppDomain.Unload(domain); }
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
            foreach (var file in Directory.GetFiles(Common.ApiPath + "\\" + UserControlPath, "*.dll", SearchOption.AllDirectories))
            {
                Log.Debug("DLL Found while loading UserControls:" + file);
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    foreach (var type in assembly.GetExportedTypes())
                    {
                        Log.Debug("Exposed Type: " + type);
                        if (!type.Equals(_usercontrolBaseType) && _usercontrolBaseType.IsAssignableFrom(type))
                            result.Add(new TypeLocator(assembly.FullName, type.FullName, file));
                    }
                }
                catch (Exception ex)
                {
                    // This method is called in its own App Domain so will not have access to the calling logger
                    Log.Error("An assembly was not found for file:" + file, ex);
                }
            }
            return result;
        }
    }
}
