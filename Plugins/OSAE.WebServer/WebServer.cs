namespace OSAE.WebServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;

    public class WebServer : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("Web Server");

        /// <summary>
        /// Holds the plugin name
        /// </summary>
        string pName;

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void ProcessCommand(OSAEMethod method)
        {
            // No commands to process
        }

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void RunInterface(string pluginName)
        {
            logging.AddToLog("Web server is running", true);
        }

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void Shutdown()
        {

        }
    }
}
