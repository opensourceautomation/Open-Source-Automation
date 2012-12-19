using System;
using System.AddIn.Hosting;
using System.Reflection;
using OpenSourceAutomation;

namespace OSAE.Service
{
    public class Plugin
    {
        private OSAE osae = new OSAE("Plugin");

        #region Properties
        public Assembly Assembly;

        /// <summary>
        /// Gets or sets the plugin name
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>
        /// Gets or sets the plugin type
        /// </summary>
        public string PluginType { get; set; }

        /// <summary>
        /// Gets or sets the plugin version
        /// </summary>
        public string PluginVersion { get; set; }

        /// <summary>
        /// Gets or sets the latest available version
        /// </summary>
        public string LatestAvailableVersion { get; set; }

        /// <summary>
        /// Gets or sets whether the plugin is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the addin token
        /// </summary>
        public AddInToken Token { get; set; }

        /// <summary>
        /// Gets or sets the addin process
        /// </summary>
        public AddInProcess Process { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IOpenSourceAutomationAddInv2 Addin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Status
        {
            get { return osae.GetObjectStateValue(PluginName).Value; }
        }

        #endregion

        public Plugin(AddInToken token)
        {
            Token = token;
            PluginName = osae.GetPluginName(token.Name, osae.ComputerName);
            PluginType = token.Name;
            PluginVersion = token.Version;
            LatestAvailableVersion = "";
        }

        public Plugin()
        {
            LatestAvailableVersion = "";
        }

        /// <summary>
        /// Sets up the requested plugin and gets it running.
        /// </summary>
        /// <returns>true if the plugin successfully loaded false otherwise</returns>
        public bool ActivatePlugin()
        {
            try
            {
                osae.AddToLog("Activating Plugin: " + PluginName, true);
                // Create application domain setup information.
                AppDomainSetup domaininfo = new AppDomainSetup();
                domaininfo.PrivateBinPathProbe = osae.APIpath;
                domaininfo.ApplicationBase = osae.APIpath;
                //domaininfo.ApplicationTrust = AddInSecurityLevel.Host;

                // TODO Remove Log Message after testing
                osae.AddToLog("Running New Service Code", true);

                // Create the application domain.
                AppDomain domain = AppDomain.CreateDomain(PluginName + "_Domain", null, domaininfo);
                // Write application domain information to the console.
                osae.AddToLog("Host domain: " + AppDomain.CurrentDomain.FriendlyName, true);
                osae.AddToLog("child domain: " + domain.FriendlyName, true);
                osae.AddToLog("Application base is: " + domain.SetupInformation.ApplicationBase, true);
                
                //_addin = _token.Activate<IOpenSourceAutomationAddIn>(domain);
                Process = new AddInProcess(Platform.AnyCpu);
                Addin = Token.Activate<IOpenSourceAutomationAddInv2>(Process, AddInSecurityLevel.FullTrust);
                Enabled = true;
                return true;
            }
            catch(Exception ex)
            {
                osae.AddToLog("Error activating plugin (" + PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
                Enabled = false;
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
