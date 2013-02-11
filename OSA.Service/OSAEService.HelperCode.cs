namespace OSAE.Service
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security.Policy;
    using System.Security;
    using System.Threading;
    using System.Timers;

    partial class OSAEService
    {
        private void InitialiseOSAInEventLog()
        {
            try
            {
                if (!EventLog.SourceExists("OSAE"))
                    EventLog.CreateEventSource("OSAE", "Application");
            }
            catch (Exception ex)
            {
                logging.AddToLog("CreateEventSource error: " + ex.Message, true);
            }
            this.ServiceName = "OSAE";
            this.EventLog.Source = "OSAE";
            this.EventLog.Log = "Application";
        }

        private static void InitialiseLogFolder()
        {
            FileInfo file = new FileInfo(Common.ApiPath + "/Logs/");
            file.Directory.Create();
            if (OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value == "TRUE")
            {
                string[] files = Directory.GetFiles(Common.ApiPath + "/Logs/");
                foreach (string f in files)
                    File.Delete(f);
            }
        }

        private static void DeleteStoreFiles()
        {
            string[] stores = Directory.GetFiles(Common.ApiPath, "*.store", SearchOption.AllDirectories);
            foreach (string f in stores)
                File.Delete(f);
        }

        private void GetComputerIP()
        {
            IPHostEntry ipEntry = Dns.GetHostByName(Common.ComputerName);
            IPAddress[] addr = ipEntry.AddressList;
            computerIP = addr[0].ToString();
        }

        private void CreateServiceObject()
        {
            try
            {
                logging.AddToLog("Creating Service object", true);
                OSAEObject svcobj = OSAEObjectManager.GetObjectByName("SERVICE-" + Common.ComputerName);
                if (svcobj == null)
                {
                    OSAEObjectManager.ObjectAdd("SERVICE-" + Common.ComputerName, "SERVICE-" + Common.ComputerName, "SERVICE", "", "SYSTEM", true);
                }
                OSAEObjectStateManager.ObjectStateSet("SERVICE-" + Common.ComputerName, "ON", "OSAE Service");
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error creating service object - " + ex.Message, true);
            }
        }

        private void CreateComputerObject()
        {
            logging.AddToLog("Creating Computer object", true);
            if (OSAEObjectManager.GetObjectByName(Common.ComputerName) == null)
            {

                OSAEObject obj = OSAEObjectManager.GetObjectByAddress(computerIP);
                if (obj == null)
                {
                    OSAEObjectManager.ObjectAdd(Common.ComputerName, Common.ComputerName, "COMPUTER", computerIP, "", true);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else if (obj.Type == "COMPUTER")
                {
                    OSAEObjectManager.ObjectUpdate(obj.Name, Common.ComputerName, obj.Description, "COMPUTER", computerIP, obj.Container, obj.Enabled);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else
                {
                    OSAEObjectManager.ObjectAdd(Common.ComputerName + "." + computerIP, Common.ComputerName, "COMPUTER", computerIP, "", true);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName + "." + computerIP, "Host Name", Common.ComputerName, sourceName);
                }
            }
            else
            {
                OSAEObject obj = OSAEObjectManager.GetObjectByName(Common.ComputerName);
                OSAEObjectManager.ObjectUpdate(obj.Name, obj.Name, obj.Description, "COMPUTER", computerIP, obj.Container, obj.Enabled);
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Host Name", Common.ComputerName, sourceName);
            }
        }

        public void enablePlugin(Plugin plugin)
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);

            OSAEObjectManager.ObjectUpdate(plugin.PluginName, plugin.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 1);
            try
            {
                if (plugin.ActivatePlugin())
                {
                    plugin.Enabled = true;
                    plugin.RunInterface();
                    OSAEObjectStateManager.ObjectStateSet(plugin.PluginName, "ON", sourceName);
                    sendMessageToClients("plugin", plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Running | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName);
                    logging.AddToLog("Plugin enabled: " + plugin.PluginName, true);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error activating plugin (" + plugin.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
            }
            catch
            {
                logging.AddToLog("Error activating plugin", true);
            }
        }

        public void disablePlugin(Plugin p)
        {
            logging.AddToLog("Disabling Plugin: " + p.PluginName, true);

            OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
            OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 0);
            try
            {
                p.Shutdown();
                p.Enabled = false;
                p.Domain = CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet);
                sendMessageToClients("plugin", p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | Stopped | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error stopping plugin (" + p.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
            }
        }

        public AppDomain CreateSandboxDomain(string name, string path, SecurityZone zone)
        {
            var setup = new AppDomainSetup { ApplicationBase = Common.ApiPath, PrivateBinPath = Path.GetFullPath(path) };

            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(zone));
            var permissions = SecurityManager.GetStandardSandbox(evidence);

            var strongName = typeof(OSAEService).Assembly.Evidence.GetHostEvidence<StrongName>();

            return AppDomain.CreateDomain(name, null, setup);
        }

        public string TrimNulls(byte[] data)
        {
            int rOffset = data.Length - 1;

            for (int i = data.Length - 1; i >= 0; i--)
            {
                rOffset = i;

                if (data[i] != (byte)0) break;
            }

            return System.Text.Encoding.ASCII.GetString(data, 0, rOffset + 1);
        }

        public bool pluginExist(string name)
        {
            foreach (Plugin p in plugins)
            {
                if (p.PluginType == name)
                    return true;
            }
            return false;
        }

        private void RemoveOrphanedMethods()
        {
            logging.AddToLog("Removing orphaned methods", true);

            try
            {
                OSAEMethodManager.ClearMethodQueue();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error clearing method queue details: \r\n" + ex.Message, true);
            }
        }

        private void StartThreads()
        {
            Thread QueryCommandQueueThread = new Thread(new ThreadStart(QueryCommandQueue));
            QueryCommandQueueThread.Start();

            updates.Interval = 86400000;
            updates.Enabled = true;
            updates.Elapsed += new ElapsedEventHandler(getPluginUpdates_tick);

            Thread loadPluginsThread = new Thread(new ThreadStart(LoadPlugins));
            loadPluginsThread.Start();

            checkPlugins.Interval = 60000;
            checkPlugins.Enabled = true;
            checkPlugins.Elapsed += new ElapsedEventHandler(checkPlugins_tick);

            Thread updateThread = new Thread(() => getPluginUpdates());
            updateThread.Start();
        }        
    }
}
