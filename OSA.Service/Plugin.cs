using System;
using System.AddIn.Hosting;
using System.Reflection;
using OpenSourceAutomation;

namespace OSAE.Service
{
    [Serializable]
    public class Plugin
    {
        private string _pluginName;
        private string _pluginType;
        private string _pluginVersion;
        private bool _enabled;
        private string _latestAvailableVersion;
        private AddInToken _token;
        private IOpenSourceAutomationAddInv2 _addin;
        //private AddInProcess _process;
        private OSAE osae = new OSAE("Plugin");
        private AppDomain _domain;

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
        //public AddInProcess process
        //{
        //    get { return _process; }
        //    set { _process = value; }
        //}
        public AppDomain Domain
        {
            get { return _domain; }
            set { _domain = value; }
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
                
                // Configure
                domaininfo.ApplicationName = PluginName;
                domaininfo.ApplicationBase = osae.APIpath;
                domaininfo.PrivateBinPath = osae.APIpath;
                domaininfo.LoaderOptimization = LoaderOptimization.MultiDomain;
                domaininfo.DisallowApplicationBaseProbing = false;
                domaininfo.DisallowBindingRedirects = false;
                domaininfo.DisallowCodeDownload = false;
                domaininfo.DisallowPublisherPolicy = false;

                //System.Security.Policy.Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                // Create the new application domain using setup information.

                Domain = AppDomain.CreateDomain(PluginName + "_Domain", null, domaininfo);
                Domain.UnhandledException += Domain_UnhandledException;

                // Write application domain information to the log.
                osae.AddToLog("Host domain: " + AppDomain.CurrentDomain.FriendlyName, true);
                osae.AddToLog("child domain: " + _domain.FriendlyName, true);
                osae.AddToLog("Application base is: " + _domain.SetupInformation.ApplicationBase, true);

                _addin = _token.Activate<IOpenSourceAutomationAddInv2>(Domain);

                //Domain.FirstChanceException += Domain_FirstChanceException;
                //    (object source, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e) =>
                //    {
                //        osae.AddToLog("FirstChanceException event raised in "+AppDomain.CurrentDomain.FriendlyName+": " + e.Exception.Message,true);
                //    };
                
                //_process = new AddInProcess(Platform.AnyCpu);
                //_addin = _token.Activate<IOpenSourceAutomationAddInv2>(_process,AddInSecurityLevel.FullTrust);
                

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

        private void Domain_UnhandledException(object source, System.UnhandledExceptionEventArgs e)
        {
            osae.AddToLog(PluginName + " plugin has caused OSA to stop.  It has been flagged as unstable and disabled.  It will not be loaded anymore when OSA starts.  To start plugin again remove the 'unstable.txt' file in the plugin directory.  ERROR: " + e.ExceptionObject.ToString(), true);
            osae.AddToLog("Creating unstable flag: " + osae.APIpath + "\\AddIns\\" + PluginType + "\\unstable.txt", true);
            System.IO.StreamWriter sw = System.IO.File.AppendText(osae.APIpath + "\\AddIns\\" + PluginType + "\\unstable.txt");
            sw.WriteLine(System.DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " - " + e.ExceptionObject.ToString());
            sw.Close();
        }

        public bool Shutdown()
        {
            try
            {
                osae.AddToLog("Shutting down " + PluginName, true);
                addin.Shutdown();
                //p.process.Shutdown();
                addin = null;
                return true;
            }
            catch(Exception ex)
            {

                return false;
            }
        }

        public void RunInterface()
        {
            osae.AddToLog(PluginName + " - Running interface.", false);
            try
            {
                addin.RunInterface(PluginName);
            }
            catch (Exception ex)
            {
                osae.AddToLog(PluginName + " - Run Interface Error: " + ex.Message, true);
            }
            osae.AddToLog(PluginName + " - Moving on...", false);
        }

        public void ExecuteCommand(OSAEMethod method)
        {
            try
            {
                addin.ProcessCommand(method);
            }
            catch (Exception ex)
            {
                osae.AddToLog(PluginName + " - Process Command Error: " + ex.Message, true);
            }
        }
    }
}
