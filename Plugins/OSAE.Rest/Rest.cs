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
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

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
                this.Log.Info("Starting Rest Interface");

                bool showHelp = bool.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Show Help").Value);
                int restPort = 8732;

                if (!OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "REST Port").Id.Equals(String.Empty))
                {
                    try
                    {
                        restPort = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "REST Port").Value);
                        this.Log.Info("Rest Port read in as: " + restPort);
                    }
                    catch (FormatException ex)
                    { this.Log.Error("Error pulling REST port from property (not a valid number)", ex); }
                    catch (OverflowException ex)
                    { this.Log.Error("Error pulling REST port from property (too large)", ex); }
                    catch (ArgumentNullException ex)
                    { this.Log.Error("Error pulling REST port from property (null)", ex); }
                }

                String restUrl = "http://localhost:"+restPort.ToString()+"/api";
                serviceHost = new WebServiceHost(typeof(OSAERest.api), new Uri(restUrl));
                
               WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None); 
                binding.CrossDomainScriptAccessEnabled = true;
                var endpoint = serviceHost.AddServiceEndpoint(typeof(IRestService), binding, "");

                ServiceDebugBehavior sdb = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                sdb.HttpHelpPageEnabled = false;
                if (showHelp) serviceHost.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });

                this.Log.Info("Starting Rest Interface");
                serviceHost.Open();                                
            }
            catch (Exception ex)
            {
                this.Log.Error("Error starting RESTful web service", ex);
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
