using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Data;
using System.Threading;
using OpenSourceAutomation;
using System.AddIn.Hosting;

namespace OSAE.Service
{
    public class Plugin
    {
        private string _pluginName;
        private string _pluginType;
        private string _pluginVersion;
        private bool _enabled;
        private string _latestAvailableVersion;
        private AddInToken _token;
        private IOpenSourceAutomationAddInv2 _addin;
        private AddInProcess _process;
        private OSAE osae = new OSAE("Plugin");

        #region Properties
        public Assembly Assembly;

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
        public AddInToken token
        {
            get { return _token; }
            set { _token = value; }
        }
        public AddInProcess process
        {
            get { return _process; }
            set { _process = value; }
        }
        public IOpenSourceAutomationAddInv2 addin
        {
            get { return _addin; }
            set { _addin = value; }
        }
        public string Status
        {
            get { return osae.GetObjectStateValue(_pluginName).Value; }
        }

        #endregion

        public Plugin(AddInToken token)
        {
            _token = token;
            _pluginName = osae.GetPluginName(_token.Name, osae.ComputerName);
            _pluginType = _token.Name;
            _pluginVersion = _token.Version;
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
                osae.AddToLog("Activating Plugin: " + PluginName, true);
                // Create application domain setup information.
                AppDomainSetup domaininfo = new AppDomainSetup();
                domaininfo.ApplicationBase = osae.APIpath;
                //domaininfo.ApplicationTrust = AddInSecurityLevel.Host;

                // Create the application domain.
                AppDomain domain = AppDomain.CreateDomain(PluginName + "_Domain", null, domaininfo);
                // Write application domain information to the console.
                osae.AddToLog("Host domain: " + AppDomain.CurrentDomain.FriendlyName, true);
                osae.AddToLog("child domain: " + domain.FriendlyName, true);
                osae.AddToLog("Application base is: " + domain.SetupInformation.ApplicationBase, true);
                
                //_addin = _token.Activate<IOpenSourceAutomationAddIn>(domain);
                _process = new AddInProcess(Platform.AnyCpu);
                _addin = _token.Activate<IOpenSourceAutomationAddInv2>(process,AddInSecurityLevel.FullTrust);
                _enabled = true;
                return true;
            }
            catch(Exception ex)
            {
                osae.AddToLog("Error activating plugin (" + _pluginName + "): " + ex.Message + " - " + ex.InnerException, true);
                _enabled = false;
                return false;
            }
            catch
            {
                osae.AddToLog("Error activating plugin", true);
                return false;
            }
        }


    }
}
