namespace OSAE.WebServer
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using HttpServer.HttpModules;
    using ICSharpCode.SharpZipLib.Zip;

    public class WebServer : OSAEPluginBase
    {
        /// <summary>
        /// Server object used to host PHP site
        /// </summary>
        HttpServer.HttpServer server = new HttpServer.HttpServer();

        /// <summary>
        /// Gives access OSA API functionality
        /// </summary>
        OSAE osae = new OSAE("Web Server");

        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = new Logging("Web Server");

        /// <summary>
        /// Used as part of the host site
        /// </summary>
        AdvancedFileModule afm = null; 

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
            try
            {
                PerformUpgradeIfRequired();

                pName = pluginName;
                afm = new AdvancedFileModule("/", Common.ApiPath + @"\wwwroot");
                afm.ServeUnknownTypes(true, "php");
                afm.AddCgiApplication("php", @"C:\php\php-cgi.exe");

                //Need both as Image path is not consistant in DB.
                afm.AddVirtualDirectory("Images", Common.ApiPath + @"\Images");
                afm.AddVirtualDirectory("/Images", Common.ApiPath + @"\Images");
                
                server.Add(afm);                
                logging.AddToLog("starting server...", true);
                server.Start(IPAddress.Any, Int32.Parse(osae.GetObjectPropertyValue(pName, "Port").Value));

            }
            catch (Exception ex)
            {
                logging.AddToLog("Error starting server: " + ex.Message, true);
            }
        }

        /// <summary>
        /// Standard OSA Plugin architecture see Wiki Docs
        /// </summary>
        /// <param name="method">Standard Architecture parameter see Wiki Docs</param>
        public override void Shutdown()
        {
            server.Stop();
        }

        /// <summary>
        /// Upgrades the site using with any new components
        /// </summary>
        private void PerformUpgradeIfRequired()
        {
            try
            {
                // TODO might want to consider clearing out folder before starting

                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyName assemName = assembly.GetName();
                string zipFileName = System.IO.Path.GetTempPath() + @"OSA_Web_" + assemName.Version.ToString() + ".zip";
                string outputFolder = Common.ApiPath + @"\wwwroot\";
                               

                // TODO consider using OSAP file to determine version
                //if (!File.Exists(zipFileName))
                {
                    //logging.AddToLog("Did not find a Zip with Version: " + assemName.Version.ToString() + " Performing upgrade", true);

                    ExtractZipFromResource(zipFileName);

                    FastZipEvents events = new FastZipEvents();
                    FastZip fastZip = new FastZip(events);

                    fastZip.ExtractZip(zipFileName,
                        outputFolder,
                        FastZip.Overwrite.Always,
                        null,
                        null,
                        null,
                        true);
                    logging.AddToLog("Extracting file to : " + outputFolder, true);
                    logging.AddToLog("Upgrade Complete", true);

                }
                // else
                {
                   // logging.AddToLog("File with verison: " + zipFileName + " upgrade not required", true);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Upgrade failed: " + ex.Message, true);
            }
        }

        /// <summary>
        /// Extracts the embedded zip file containing the website to the specified path
        /// </summary>
        /// <param name="destinationPath">The path where the embeded resource should be extracted</param>
        private static void ExtractZipFromResource(string destinationPath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemName = assembly.GetName();

            StreamReader stream = new StreamReader(assembly.GetManifestResourceStream("OSAE.WebServer.WebUI.zip"));
            using (FileStream fs = new FileStream(destinationPath, FileMode.Create))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.BaseStream.Length];
                stream.BaseStream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fs.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
    }
}
