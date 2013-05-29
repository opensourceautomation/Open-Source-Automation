using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginDescriptionEditor
{
    class PluginDescription
    {
        private string _pluginName;
        public string PluginName
        {
            set { _pluginName = value; }
            get { return _pluginName; }
        }

        private string _pluginID;
        public string PluginID
        {
            set { _pluginID = value; }
            get { return _pluginID; }
        }

        private string _pluginVersion;
        public string PluginVersion
        {
            set { _pluginVersion = value; }
            get { return _pluginVersion; }
        }

        private string _pluginState;
        public string PluginState
        {
            set { _pluginState = value; }
            get { return _pluginState; }
        }

        private string _pluginAuthor;
        public string PluginAuthor
        {
            set { _pluginAuthor = value; }
            get { return _pluginAuthor; }
        }

        private string _wikiUrl;
        public string WikiUrl
        {
            set { _wikiUrl = value; }
            get { return _wikiUrl; }
        }

        private string _description;
        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }

        private string _destFolder;
        public string DestFolder
        {
            set { _destFolder = value; }
            get { return _destFolder; }
        }

        private string _mainFile;
        public string MainFile
        {
            set { _mainFile = value; }
            get { return _mainFile; }
        }

        private List<string> _additionalAssemblies;
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

    }


}
