namespace OSAE
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    [Serializable]
    public class Plugin
    {
        /// <summary>
        /// 
        /// </summary>
        private string _pluginName;

        /// <summary>
        /// 
        /// </summary>
        private string _pluginType;

        /// <summary>
        /// 
        /// </summary>
        private string _pluginVersion;

        /// <summary>
        /// 
        /// </summary>
        private string _assemblyName;

        /// <summary>
        /// 
        /// </summary>
        private string _assemblyType;

        /// <summary>
        /// 
        /// </summary>
        private string _location;

        /// <summary>
        /// 
        /// </summary>
        private bool _enabled;
        
        /// <summary>
        /// 
        /// </summary>
        private string _latestAvailableVersion;

        //OSAELog
        private OSAE.General.OSAELog Log = new OSAE.General.OSAELog("Plugin Loader");

        private AppDomain _domain;
        private OSAEPluginBase _plugin;

        #region Properties

        public string PluginName
        {
            get { return _pluginName; }
            set { _pluginName = value; }
        }

        public string PluginType
        {
            get { return _pluginType; }
            set { _pluginType = value; }
        }

        public string PluginVersion
        {
            get { return _pluginVersion; }
            set { _pluginVersion = value; }
        }

        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public string LatestAvailableVersion
        {
            get { return _latestAvailableVersion; }
            set { _latestAvailableVersion = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public AppDomain Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public string Status
        {
            get { return OSAEObjectStateManager.GetObjectStateValue(_pluginName).Value; }
        }

        #endregion

        public Plugin(string assemblyName, string assemblyType, AppDomain domain, string location)
        {
            PluginDescription desc = new PluginDescription();
            List<string> osapdFiles = new List<string>();
            string[] pluginFile = Directory.GetFiles(location, "*.osapd", SearchOption.AllDirectories);
            osapdFiles.AddRange(pluginFile);

            foreach (string path in osapdFiles)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    desc = new PluginDescription();
                    desc.Deserialize(path);
                    _pluginVersion = desc.Version;
                }
            }

            _pluginType = desc.Type;
            _pluginName = PluginManager.GetPluginName(_pluginType, Common.ComputerName);
            _assemblyType = assemblyType;
            _assemblyName = assemblyName;
            _domain = domain;
            _location = location;

            _latestAvailableVersion = string.Empty;
        }

        public Plugin()
        {
            _latestAvailableVersion = string.Empty;
            
        }

        public bool ActivatePlugin()
        {
            try
            {
                this.Log.Info("Activating Plugin: " + PluginName);
                _plugin = (OSAEPluginBase)_domain.CreateInstanceAndUnwrap(_assemblyName, _assemblyType);
                _plugin.InitializeLifetimeService();

                _domain.UnhandledException += Domain_UnhandledException;
                return true;
            }
            catch (Exception ex)
            {
                this.Log.Error("Error activating plugin (" + PluginName + ")", ex);
                _enabled = false;
                return false;
            }           
        }

        private void Domain_UnhandledException(object source, System.UnhandledExceptionEventArgs e)
        {
            this.Log.Fatal(PluginName + " plugin has fatally crashed. ERROR: \n" + e.ExceptionObject.ToString());
            AppDomain.Unload(_domain);            
        }

        public bool Shutdown()
        {
            try
            {
                this.Log.Info("Shutting down " + PluginName);
                _plugin.Shutdown();
                AppDomain.Unload(_domain);
                return true;
            }
            catch (Exception ex)
            {
                this.Log.Error(PluginName + " - Shutdown Error", ex);
                return false;
            }
        }

        public void RunInterface()
        {
            this.Log.Debug(PluginName + " - Running interface.");
            try
            {
                _plugin.RunInterface(PluginName);
            }
            catch (Exception ex)
            {
                this.Log.Error(PluginName + " - Run Interface Error", ex);
            }

            this.Log.Debug(PluginName + " - Moving on...");
        }

        public void ExecuteCommand(OSAEMethod method)
        {
            try
            {
                _plugin.ProcessCommand(method);
            }
            catch (Exception ex)
            {
                this.Log.Error(PluginName + " - Process Command Error", ex);
            }
        }
    }
}
