using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Manager_WPF
{
    /// <summary>
    /// Holds the information about a plugin e.g. its name, type, author, version e.t.c
    /// </summary>
    public class PluginDescription : INotifyPropertyChanged 
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
                    NotifyPropertyChanged("Name");
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
                    NotifyPropertyChanged("Type");
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
                    NotifyPropertyChanged("Version");
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
                    NotifyPropertyChanged("Author");
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
                    NotifyPropertyChanged("WikiUrl");
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
                    NotifyPropertyChanged("Description");
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
                    NotifyPropertyChanged("Status");
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
                    NotifyPropertyChanged("Enabled");
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
                    NotifyPropertyChanged("Upgrade");
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
                    NotifyPropertyChanged("Path");
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
                    NotifyPropertyChanged("ID");
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
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
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
