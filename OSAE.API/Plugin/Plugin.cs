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
        private string _pluginAuthor;

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
        private bool _running;

        /// <summary>
        /// 
        /// </summary>
        private bool _enabled;
        
        /// <summary>
        /// 
        /// </summary>
        private string _latestAvailableVersion;

        private OSAE.General.OSAELog Log;

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

        public string PluginAuthor
        {
            get { return _pluginAuthor; }
            set { _pluginAuthor = value; }
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

        public bool Running
        {
            get { return _running; }
            set { _running = value; }
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

        public Plugin(string assemblyName, string assemblyType, AppDomain domain, string location, string clientAppendage = "")
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
                    _pluginAuthor = desc.Author;
                }
            }

            _pluginType = desc.Type;
            //the following won't work, it is just whether it is a cient or not
            _pluginName = _pluginType;
            if (clientAppendage.Length > 0) _pluginName = _pluginName + "-" + clientAppendage;
            _assemblyType = assemblyType;
            _assemblyName = assemblyName;
            _domain = domain;
            _location = location;

            _latestAvailableVersion = "";
        }

        public Plugin()
        {
            _latestAvailableVersion = "";
        }

        public bool ActivatePlugin()
        {
            Log = new OSAE.General.OSAELog(PluginName);
            try
            {
                Log.Info(PluginName + ":  Starting Plugin...");
                _plugin = (OSAEPluginBase)_domain.CreateInstanceAndUnwrap(_assemblyName, _assemblyType);
                _plugin.InitializeLifetimeService();
                _running = true;

                _domain.UnhandledException += Domain_UnhandledException;
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error Starting Plugin (" + PluginName + ")", ex);
                _enabled = false;
                return false;
            }           
        }

        private void Domain_UnhandledException(object source, System.UnhandledExceptionEventArgs e)
        {
            Log.Fatal(PluginName + " plugin has fatally crashed. ERROR: \n" + e.ExceptionObject.ToString());
            AppDomain.Unload(_domain);            
        }

        public bool Shutdown()
        {
            try
            {
                Log.Info(PluginName + ":  Shutting down...");
                _plugin.Shutdown();
                _running = false;
                AppDomain.Unload(_domain);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(PluginName + " - Shutdown Error", ex);
                return false;
            }
        }

        public void RunInterface(string serviceName)
        {
            Log = new OSAE.General.OSAELog(serviceName);
            Log.Debug(PluginName + " - Running interface.");
            try
            {
                _plugin.RunInterface(PluginName);
                _running = true;
            }
            catch (Exception ex)
            { Log.Error(PluginName + " - Run Interface Error", ex); }

            Log.Debug(PluginName + " - Moving on...");
        }

        public void ExecuteCommand(OSAEMethod method)
        {
            try
            {
                _plugin.ProcessCommand(method);
            }
            catch (Exception ex)
            { Log.Error(PluginName + " - Process Command Error", ex); }
        }
    }
}
