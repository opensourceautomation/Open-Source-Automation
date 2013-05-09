namespace OSAE.Rest
{
    using OSAERest;
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Rest : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("Rest");

        /// <summary>
        /// Hosts the web service
        /// </summary>
        private WebServiceHost serviceHost = null;

        /// <summary>
        /// Plugin name
        /// </summary>
        string pName;

        /// <summary>
        /// OSA Plugin Interface - Commands the be processed by the plugin
        /// </summary>
        /// <param name="method">Method containging the command to run</param>
        public override void ProcessCommand(OSAEMethod method)
        {
            //No commands to process
        }

        /// <summary>
        /// OSA Plugin Interface - called on start up to allow plugin to do any tasks it needs
        /// </summary>
        /// <param name="pluginName">The name of the plugin from the system</param>
        public override void RunInterface(string pluginName)
        {
            pName = pluginName;

            try
            {
                logging.AddToLog("Starting Rest Interface", true);

                bool showHelp = bool.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Show Help").Value);
                
                serviceHost = new WebServiceHost(typeof(OSAERest.api), new Uri("http://localhost:8732/api"));
                WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None);
                binding.CrossDomainScriptAccessEnabled = true;

                var endpoint = serviceHost.AddServiceEndpoint(typeof(IRestService), binding, "");

                ServiceDebugBehavior sdb = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                sdb.HttpHelpPageEnabled = false;

                if (showHelp)
                {
                    serviceHost.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });
                }

                logging.AddToLog("Starting Rest Interface", true);
                serviceHost.Open();                                
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error starting RESTful web service: " + ex.Message, true);
            }
        }

        /// <summary>
        /// OSA Plugin Interface - The plugin has been asked to shut down
        /// </summary>        
        public override void Shutdown()
        {
            serviceHost.Close();
        }
    }
}
