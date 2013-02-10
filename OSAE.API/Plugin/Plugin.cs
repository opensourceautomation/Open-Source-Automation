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

        /// <summary>
        /// Provides access to logging
        /// </summary>
        private Logging logging = Logging.GetLogger();

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
                logging.AddToLog("Activating Plugin: " + PluginName, true);
                _plugin = (OSAEPluginBase)_domain.CreateInstanceAndUnwrap(_assemblyName, _assemblyType);
                _plugin.InitializeLifetimeService();

                _domain.UnhandledException += Domain_UnhandledException;
                return true;
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error activating plugin (" + PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
                _enabled = false;
                return false;
            }
            catch
            {
                logging.AddToLog("Error activating plugin", true);
                return false;
            }
        }

        private void Domain_UnhandledException(object source, System.UnhandledExceptionEventArgs e)
        {
            logging.AddToLog(PluginName + " plugin has fatally crashed. ERROR: \n" + e.ExceptionObject.ToString(), true);
            AppDomain.Unload(_domain);            
        }

        public bool Shutdown()
        {
            try
            {
                logging.AddToLog("Shutting down " + PluginName, true);
                _plugin.Shutdown();
                AppDomain.Unload(_domain);
                return true;
            }
            catch (Exception ex)
            {
                logging.AddToLog(PluginName + " - Shutdown Error: " + ex.Message, true);
                return false;
            }
        }

        public void RunInterface()
        {
            logging.AddToLog(PluginName + " - Running interface.", false);
            try
            {
                _plugin.RunInterface(PluginName);
            }
            catch (Exception ex)
            {
                logging.AddToLog(PluginName + " - Run Interface Error: " + ex.Message, true);
            }

            logging.AddToLog(PluginName + " - Moving on...", false);
        }

        public void ExecuteCommand(OSAEMethod method)
        {
            try
            {
                _plugin.ProcessCommand(method);
            }
            catch (Exception ex)
            {
                logging.AddToLog(PluginName + " - Process Command Error: " + ex.Message, true);
            }
        }
    }
}
