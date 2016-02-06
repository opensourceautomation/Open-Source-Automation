namespace OSAE
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

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
            set { if (value != _pluginName) _pluginName = value; }
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
            set { if (value != _pluginType) _pluginType = value; }
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
            set { if (value != _pluginVersion) _pluginVersion = value; }
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
            set { if (value != _pluginAuthor) _pluginAuthor = value; }
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
            set { if (value != _wikiUrl) _wikiUrl = value; }
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
            set { if (value != _description) _description = value; }
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
            set { if (value != _status) _status = value; }
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
            set { if (value != _enabled) _enabled = value; }
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
            set { if (value != _upgrade) _upgrade = value; }
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
            set { if (value != _path) _path = value; }
            get { return _path; }
        }

        private string _computer;
        /// <summary>
        /// 
        /// </summary>
        public string Computer
        {
            set { if (value != _computer) _computer = value; }
            get { return _computer; }
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
            set { if (value != _id) _id = value; }
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

            // Check if this plugin supports any x64 Specific Assemblies
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
