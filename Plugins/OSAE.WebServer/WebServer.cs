using System;
using System.Net;
using HttpServer.HttpModules;

namespace OSAE.WebServer
{
    public class WebServer : OSAEPluginBase
    {
        HttpServer.HttpServer server = new HttpServer.HttpServer();
        OSAE osae = new OSAE("Web Server");
        AdvancedFileModule afm = null; 
        string pName;

        public override void ProcessCommand(OSAEMethod method)
        {
            // No commands to process
        }

        public override void RunInterface(string pluginName)
        {
            try
            {
                pName = pluginName;
                afm = new AdvancedFileModule("/", osae.APIpath + @"\wwwroot");
                afm.ServeUnknownTypes(true, "php");
                afm.AddCgiApplication("php", @"C:\php\php-cgi.exe");

                //Need both as Image path is not consistant in DB.
                afm.AddVirtualDirectory("Images", osae.APIpath + @"\Images");
                afm.AddVirtualDirectory("/Images", osae.APIpath + @"\Images");
                
                server.Add(afm);
                
                osae.AddToLog("starting server...", true);
                server.Start(IPAddress.Any, Int32.Parse(osae.GetObjectPropertyValue(pName, "Port").Value));

            }
            catch (Exception ex)
            {
                osae.AddToLog("Error starting server: " + ex.Message, true);
            }
        }

        public override void Shutdown()
        {
            server.Stop();
        }
    }
}
