namespace OSAE.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    [Serializable]
    public class Plugin
    {
        Logging2 log = Logging2.GetLogger("Plugin");

        private string _pluginName;
        private string _pluginType;
        private string _pluginVersion;
        private string _assemblyName;
        private string _assemblyType;
        private string _location;
        private bool _enabled;
        private string _latestAvailableVersion;
        private OSAE osae = new OSAE("Plugin");
        private AppDomain _domain;
        private OSAEPluginBase _plugin;

        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = new Logging("Plugin");
        
        #region Properties
        //public Assembly Assembly;

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
            get { return osae.GetObjectStateValue(_pluginName).Value; }
        }

        #endregion

        public Plugin(string assemblyName, string assemblyType,  AppDomain domain, string location)
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
            _pluginName = osae.GetPluginName(_pluginType, osae.ComputerName);
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
            try
            {
                logging.AddToLog("Activating Plugin: " + PluginName, true);
                _plugin = (OSAEPluginBase)_domain.CreateInstanceAndUnwrap(_assemblyName, _assemblyType);
                _plugin.InitializeLifetimeService();

                _domain.UnhandledException += Domain_UnhandledException;
                return true;
            }
            catch(Exception ex)
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
            catch(Exception ex)
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

    public class PluginDescription
    {
        /// <summary>
        /// The name of the plugin
        /// </summary>
        private string _pluginName;

        /// <summary>
        /// Gets or sets the name of the plugin
        /// </summary>
        public string Name
        {
            set
            {
                if (value != this._pluginName)
                {
                    this._pluginName = value;
                }
            }
            get { return _pluginName; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _pluginType;

        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            set
            {
                if (value != this._pluginType)
                {
                    this._pluginType = value;
                }
            }
            get { return _pluginType; }
        }

        /// <summary>
        /// The version of the plgun
        /// </summary>
        private string _pluginVersion;

        /// <summary>
        /// Gets or sets the author of the plgin
        /// </summary>
        public string Version
        {
            set
            {
                if (value != this._pluginVersion)
                {
                    this._pluginVersion = value;
                }
            }
            get { return _pluginVersion; }
        }

        /// <summary>
        /// The author of the plugin
        /// </summary>
        private string _pluginAuthor;

        /// <summary>
        /// Gets or sets the author of the plugin
        /// </summary>
        public string Author
        {
            set
            {
                if (value != this._pluginAuthor)
                {
                    this._pluginAuthor = value;
                }
            }
            get { return _pluginAuthor; }
        }

        /// <summary>
        /// The location of the help page on the wiki
        /// </summary>
        private string _wikiUrl;

        /// <summary>
        /// Gets or sets the location of the help page on the wiki
        /// </summary>
        public string WikiUrl
        {
            set
            {
                if (value != this._wikiUrl)
                {
                    this._wikiUrl = value;
                }
            }
            get { return _wikiUrl; }
        }

        /// <summary>
        /// A description of the plugin
        /// </summary>
        private string _description;

        /// <summary>
        /// Gets or sets a description of the plugin
        /// </summary>
        public string Description
        {
            set
            {
                if (value != this._description)
                {
                    this._description = value;
                }
            }
            get { return _description; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _status;

        /// <summary>
        /// 
        /// </summary>
        public string Status
        {
            set
            {
                if (value != this._status)
                {
                    this._status = value;
                }
            }
            get { return _status; }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _enabled;

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            set
            {
                if (value != this._enabled)
                {
                    this._enabled = value;
                }
            }
            get { return _enabled; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _upgrade;

        /// <summary>
        /// 
        /// </summary>
        public string Upgrade
        {
            set
            {
                if (value != this._upgrade)
                {
                    this._upgrade = value;
                }
            }
            get { return _upgrade; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _path;

        /// <summary>
        /// 
        /// </summary>
        public string Path
        {
            set
            {
                if (value != this._path)
                {
                    this._path = value;
                }
            }
            get { return _path; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _id;

        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            set
            {
                if (value != this._id)
                {
                    this._id = value;
                }
            }
            get { return _id; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _mainFile;

        /// <summary>
        /// 
        /// </summary>
        public string MainFile
        {
            set { _mainFile = value; }
            get { return _mainFile; }
        }

        /// <summary>
        /// A list of additional assemblies used by the plugin
        /// </summary>
        private List<string> _additionalAssemblies = new List<string>();

        /// <summary>
        /// Gets or sets a list of assemblies used by the plugin
        /// </summary>
        public List<string> AdditionalAssemblies
        {
            set { _additionalAssemblies = value; }
            get { return _additionalAssemblies; }
        }

        //jwelch -- added for the option of x64 bit specific assemblies
        private List<string> _x64Assemblies = new List<string>();
        public List<string> x64Assemblies
        {
            set { _x64Assemblies = value; }
            get { return _x64Assemblies; }
        }

        /// <summary>
        /// Loads a plugin from a file.
        /// </summary>
        /// <param name="file">The file to load the plugin from</param>
        public void Deserialize(string file)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(ReadFromFile(file));
            //_pluginName = xml.SelectSingleNode("//plugin-name").InnerText;
            _pluginType = xml.SelectSingleNode("//plugin-name").InnerText;
            _id = xml.SelectSingleNode("//plugin-id").InnerText;
            _pluginVersion = xml.SelectSingleNode("//plugin-version").InnerText;
            _status = xml.SelectSingleNode("//plugin-state").InnerText;
            _pluginAuthor = xml.SelectSingleNode("//author").InnerText;
            _description = xml.SelectSingleNode("//short-description").InnerText;
            _path = xml.SelectSingleNode("//destination-folder").InnerText;
            _mainFile = xml.SelectSingleNode("//main-file").InnerText;
            _wikiUrl = xml.SelectSingleNode("//wiki-url").InnerText;

            XmlNode temp = xml.SelectSingleNode("//additional-assemblies");
            XmlNodeList childNodes = temp.ChildNodes;

            foreach (XmlNode t in childNodes)
                _additionalAssemblies.Add(t.InnerText);

            //Check if this plugin supports any x64 Specific Assemblies
            if ((temp = xml.SelectSingleNode("//x64-additional-assemblies")) != null)
            {
                childNodes = temp.ChildNodes;

                foreach (XmlNode t in childNodes)
                    _x64Assemblies.Add(t.InnerText);
            }

        }

        /// <summary>
        /// Reads the content of a a file
        /// </summary>
        /// <param name="filename">The file to read the content from</param>
        /// <returns>The content that was read from the file</returns>
        static string ReadFromFile(string filename)
        {
            string S = string.Empty;

            using (StreamReader SR = File.OpenText(filename))
            {
                S = SR.ReadToEnd();
                SR.Close();
            }
            return S;
        }
    }
}
