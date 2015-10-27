namespace OSAE
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    [Serializable]
    public class OSAEUserControl: UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        private string _userControlName;

        /// <summary>
        /// 
        /// </summary>
        private string _userControlType;

        /// <summary>
        /// 
        /// </summary>
        private string _userControlVersion;

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
        private bool _enabled;
        
        /// <summary>
        /// 
        /// </summary>
        private string _latestAvailableVersion;

        //OSAELog
        private OSAE.General.OSAELog Log = new OSAE.General.OSAELog();

        private AppDomain _domain;

        #region Properties

        public string UserControlName
        {
            get { return _userControlName; }
            set { _userControlName = value; }
        }

        public string UserControlType
        {
            get { return _userControlType; }
            set { _userControlType = value; }
        }

        public string UserControlVersion
        {
            get { return _userControlVersion; }
            set { _userControlVersion = value; }
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
            get { return OSAEObjectStateManager.GetObjectStateValue(_userControlName).Value; }
        }

        #endregion

        public OSAEUserControl(string assemblyName, string assemblyType, AppDomain domain, string location)
        {
            PluginDescription desc = new PluginDescription();
            List<string> osaudFiles = new List<string>();
            string[] pluginFile = Directory.GetFiles(location, "*.osaud", SearchOption.AllDirectories);
            osaudFiles.AddRange(pluginFile);

            foreach (string path in osaudFiles)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    desc = new PluginDescription();
                    desc.Deserialize(path);
                    _userControlVersion = desc.Version;
                }
            }

            _userControlType = desc.Type;
            _userControlName = PluginManager.GetPluginName(_userControlType, Common.ComputerName);
            _assemblyType = assemblyType;
            _assemblyName = assemblyName;
            _domain = domain;

            _latestAvailableVersion = string.Empty;
        }

        public OSAEUserControl()
        {
            _latestAvailableVersion = string.Empty;
            
        }

        private void Domain_UnhandledException(object source, System.UnhandledExceptionEventArgs e)
        {
            this.Log.Fatal(UserControlName + " plugin has fatally crashed. ERROR: \n" + e.ExceptionObject.ToString());
            AppDomain.Unload(_domain);            
        }

        public bool Shutdown()
        {
            try
            {
                this.Log.Info("Shutting down " + UserControlName);
                AppDomain.Unload(_domain);
                return true;
            }
            catch (Exception ex)
            {
                this.Log.Error(UserControlName + " - Shutdown Error", ex);
                return false;
            }
        }

    }
}
