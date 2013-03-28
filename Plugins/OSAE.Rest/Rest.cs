namespace OSAE.Rest
{
    using OSAERest;
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;

    public class Rest : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("Rest");

        private WebServiceHost serviceHost = null;

        /// <summary>
        /// Plugin name
        /// </summary>
        string pName;

        public override void ProcessCommand(OSAEMethod method)
        {
            //No commands to process
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;

            try
            {
                logging.AddToLog("Starting Rest Interface", true);

                // int port = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value);
                // string uriPath = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "URI Path").Value;

                bool showHelp = bool.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Show Help").Value);
                
                serviceHost = new WebServiceHost(typeof(OSAERest.api), new Uri("http://localhost:8732/api"));

                serviceHost.AddServiceEndpoint(typeof(IRestService), new WebHttpBinding(), "http://localhost:8732/api");
                
                if (showHelp)
                {
                    serviceHost.Description.Endpoints[0].Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });
                }
                
                serviceHost.Open();

                logging.AddToLog("Rest Interface Started", true);                
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error starting RESTful web service: " + ex.Message, true);
            }
        }

        public override void Shutdown()
        {
            serviceHost.Close();
        }
    }
}
