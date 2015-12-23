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
            this.Log.Info("Checking for UWS HiPriv Services...");
            try
            {
                ServiceController hiService = new ServiceController();
                hiService.ServiceName = "UWS HiPriv Services";
                string svcStatus = hiService.Status.ToString();
                if (svcStatus == "Stopped")
                    hiService.Start();
                else if (svcStatus != "Running")
                    hiService.Start();
                this.Log.Info("Started UWS Priv Services");
            }
            catch (Exception ex)
            {
                Log.Error("Error checking for UWS Server!", ex);
            }
            try
            {
                ServiceController loService = new ServiceController();
                loService.ServiceName = "UWS LoPriv Services";
                string svcStatus = loService.Status.ToString();
                if (svcStatus == "Stopped")
                    loService.Start();
                else if (svcStatus != "Running")
                    loService.Start();
                this.Log.Info("Started UWS LoPriv Services");
            }
            catch (Exception ex)
            {
                Log.Error("Error checking for UWS Server!", ex);
            }
            try
            {
                ServiceController wsService = new ServiceController();
                wsService.ServiceName = "UltiDev Web Server Pro";
                string svcStatus = wsService.Status.ToString();
                if (svcStatus == "Stopped")
                    wsService.Start();
                else if (svcStatus != "Running")
                    wsService.Start();
                this.Log.Info("Started UWS LoPriv Services");
            }
            catch (Exception ex)
            {
                Log.Error("Error checking for UWS Server!", ex);
            }
            this.Log.Info("Web server is running");
        }

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void Shutdown()
        {
            this.Log.Info("Web server is stopping!");
            this.Log.Info("Checking for UWS HiPriv Services...");
            try
            {
                ServiceController hiService = new ServiceController();
                hiService.ServiceName = "UWS HiPriv Services";
                string svcStatus = hiService.Status.ToString();
                if (svcStatus == "Running")
                    hiService.Stop();
                else if (svcStatus != "Stopped")
                    hiService.Stop();
                this.Log.Info("Stopped UltiDev Web Server Pro Services");
            }
            catch (Exception ex)
            {
                Log.Error("Error checking for UWS Server!", ex);
            }
            this.Log.Info("Checking for UWS LoPriv Services...");
            try
            {
                ServiceController loService = new ServiceController();
                loService.ServiceName = "UWS LoPriv Services";
                string svcStatus = loService.Status.ToString();
                if (svcStatus == "Running")
                    loService.Stop();
                else if (svcStatus != "Stopped")
                    loService.Stop();
                this.Log.Info("Stopped UWS LoPriv Services");
            }
            catch (Exception ex)
            {
                Log.Error("Error checking for UWS Server!", ex);
            }
            try
            {
                ServiceController wsService = new ServiceController();
                wsService.ServiceName = "UltiDev Web Server Pro";
                string svcStatus = wsService.Status.ToString();
                if (svcStatus == "Running")
                    wsService.Stop();
                else if (svcStatus != "Stopped")
                    wsService.Stop();
                this.Log.Info("Stopped UltiDev Web Server Pro Services");
            }
            catch (Exception ex)
            {
                Log.Error("Error checking for UWS Server!", ex);
            }
        }
    }
}
