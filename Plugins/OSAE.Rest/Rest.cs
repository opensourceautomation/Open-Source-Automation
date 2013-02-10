namespace OSAE.Rest
{
    using System;
    using System.ServiceModel.Web;

    public class Rest : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("Rest");

        private WebServiceHost serviceHost = null;

        public override void ProcessCommand(OSAEMethod method)
        {
            //No commands to process
        }

        public override void RunInterface(string pluginName)
        {
            try
            {
                logging.AddToLog("Starting Rest Interface", true);

                serviceHost = new WebServiceHost(typeof(OSAERest.api), new Uri("http://localhost:8732/api"));
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
