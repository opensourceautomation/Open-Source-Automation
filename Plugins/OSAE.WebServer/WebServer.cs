using System;
using System.AddIn;
using System.Net;
using HttpServer.HttpModules;
using OpenSourceAutomation;

namespace OSAE.WebServer
{
    [AddIn("Web Server", Version = "0.3.7")]
    public class WebServer : IOpenSourceAutomationAddInv2
    {
        HttpServer.HttpServer server = new HttpServer.HttpServer();
        OSAE osae = new OSAE("Web Server");
        AdvancedFileModule afm = null; 
        string pName;

        public void ProcessCommand(OSAEMethod method)
        {
            // No commands to process
        }

        public void RunInterface(string pluginName)
        {
            try
            {
                pName = pluginName;
                afm = new AdvancedFileModule("/", osae.APIpath + @"\wwwroot");
                afm.ServeUnknownTypes(true, "php");
                afm.AddCgiApplication("php", @"C:\php\php-cgi.exe");
                server.Add(afm);

                osae.AddToLog("starting server...", true);
                server.Start(IPAddress.Any, Int32.Parse(osae.GetObjectPropertyValue(pName, "Port").Value));

            }
            catch (Exception ex)
            {
                osae.AddToLog("Error starting server: " + ex.Message, true);
            }
        }
        
        public void Shutdown()
        {
            server.Stop();
        }
    }
}
