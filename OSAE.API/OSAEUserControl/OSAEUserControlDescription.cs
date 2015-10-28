namespace OSAE
{

    using System.Collections.Generic;
    using System.IO;
    using System.Xml;


    public class OSAEUserControlDescription
    {
        /// <summary>
        /// The name of the plugin
        /// </summary>
        private string _userControlName;

        /// <summary>
        /// Gets or sets the name of the plugin
        /// </summary>
        public string Name
        {
            set
            {
                if (value != this._userControlName)
                {
                    this._userControlName = value;
                }
            }

            get
            {
                return _userControlName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _userControlType;

        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            set
            {
                if (value != this._userControlType)
                {
                    this._userControlType = value;
                }
            }

            get
            {
                return _userControlType;
            }
        }

        /// <summary>
        /// The version of the plgun
        /// </summary>
        private string _userControlVersion;

        /// <summary>
        /// Gets or sets the author of the plgin
        /// </summary>
        public string Version
        {
            set
            {
                if (value != this._userControlVersion)
                {
                    this._userControlVersion = value;
                }
            }

            get
            {
                return _userControlVersion;
            }
        }

        /// <summary>
        /// The author of the plugin
        /// </summary>
        private string _userControlAuthor;

        /// <summary>
        /// Gets or sets the author of the plugin
        /// </summary>
        public string Author
        {
            set
            {
                if (value != this._userControlAuthor)
                {
                    this._userControlAuthor = value;
                }
            }

            get
            {
                return _userControlAuthor;
            }
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

            get
            {
                return _wikiUrl;
            }
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
            get
            {
                return _description;
            }
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

            get
            {
                return _status;
            }
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

            get
            {
                return _enabled;
            }
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

            get
            {
                return _upgrade;
            }
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

            get
            {
                return _path;
            }
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

            get
            {
                return _id;
            }
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
            _userControlType = xml.SelectSingleNode("//plugin-name").InnerText;
            _id = xml.SelectSingleNode("//plugin-id").InnerText;
            _userControlVersion = xml.SelectSingleNode("//plugin-version").InnerText;
            _status = xml.SelectSingleNode("//plugin-state").InnerText;
            _userControlAuthor = xml.SelectSingleNode("//author").InnerText;
            _description = xml.SelectSingleNode("//short-description").InnerText;
            _path = xml.SelectSingleNode("//destination-folder").InnerText;
            _mainFile = xml.SelectSingleNode("//main-file").InnerText;
            _wikiUrl = xml.SelectSingleNode("//wiki-url").InnerText;

            XmlNode temp = xml.SelectSingleNode("//additional-assemblies");
            XmlNodeList childNodes = temp.ChildNodes;

            foreach (XmlNode t in childNodes)
            {
                _additionalAssemblies.Add(t.InnerText);
            }

            // Check if this plugin supports any x64 Specific Assemblies
            if ((temp = xml.SelectSingleNode("//x64-additional-assemblies")) != null)
            {
                childNodes = temp.ChildNodes;

                foreach (XmlNode t in childNodes)
                {
                    _x64Assemblies.Add(t.InnerText);
                }
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
