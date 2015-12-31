namespace OSAE.WebServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.ServiceProcess;

    public class WebServer : OSAEPluginBase
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

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
        }

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void Shutdown()
        {
            this.Log.Info("Web server is stopping!");
        }
    }
}
