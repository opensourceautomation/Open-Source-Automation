namespace OSAE.Service
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Security.Policy;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Xml.Linq;
    using MySql.Data.MySqlClient;
    using API;

    class OSAEService : ServiceBase
    {
        private ServiceHost sHost;
        private WCF.WCFService wcfService;
        private List<Plugin> plugins = new List<Plugin>();
        private List<Plugin> masterPlugins = new List<Plugin>();
        private string _computerIP;
        private bool goodConnection = false;
        private WebServiceHost serviceHost = new WebServiceHost(typeof(OSAERest.api));
        private OSAE osae = new OSAE("OSAE Service");

        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("OSAE Service");

        private bool running = true;
        
        private System.Timers.Timer timer = new System.Timers.Timer();
        private System.Timers.Timer updates = new System.Timers.Timer();
        private System.Timers.Timer checkPlugins = new System.Timers.Timer();

        /// <summary>
        /// The Main Thread: This is where your Service is Run.
        /// </summary>
        static void Main(string[] args) 
        {          
            if (args.Length > 0)
            {
                OSAE osacl = new OSAE("OSACL");
               
                string pattern = osacl.MatchPattern(args[0]);
                Logging.AddToLog("Processing command: " + args[0] + ", Named Script: " + pattern, true, "OSACL");
                if (pattern != "")
                    osacl.MethodQueueAdd("Script Processor", "NAMED SCRIPT", pattern, "");
            }
            else
            {
                ServiceBase.Run(new OSAEService());
            }
            
        }
        
        /// <summary>
        /// Public Constructor for WindowsService.
        /// - Put all of your Initialization code here.
        /// </summary>
        public OSAEService()
        {
            logging.AddToLog("Service Starting", true);
           

            try
            {
                if (!EventLog.SourceExists("OSAE"))
                    EventLog.CreateEventSource("OSAE", "Application");
            }
            catch(Exception ex)
            {
                logging.AddToLog("CreateEventSource error: " + ex.Message, true);
            }
            this.ServiceName = "OSAE";
            this.EventLog.Source = "OSAE";
            this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            
            this.CanStop = true;
            this.CanShutdown = true;
        }

        #region Service Start/Stop Processing
        /// <summary>
        /// OnStart: Put startup code here
        ///  - Start threads, get inital data, etc.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
//#if (DEBUG)
//            Debugger.Launch(); //<-- Simple form to debug a web services 
//#endif

            try
            {
                

                IPHostEntry ipEntry = Dns.GetHostByName(Common.ComputerName);
                IPAddress[] addr = ipEntry.AddressList;
                _computerIP = addr[0].ToString();

                System.IO.FileInfo file = new System.IO.FileInfo(Common.ApiPath + "/Logs/");
                file.Directory.Create();
                if (osae.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value == "TRUE")
                {
                    string[] files = Directory.GetFiles(Common.ApiPath + "/Logs/");
                    foreach (string f in files)
                        File.Delete(f);
                }
                string[] stores = Directory.GetFiles(Common.ApiPath, "*.store", SearchOption.AllDirectories);
                foreach (string f in stores)
                    File.Delete(f);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error getting registry settings and/or deleting logs: " + ex.Message, true);
            }

            logging.AddToLog("OnStart", true);
            logging.AddToLog("Removing orphaned methods", true);

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Common.ConnectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = connection;
                    command.CommandText = "SET sql_safe_updates=0; DELETE FROM osae_method_queue;";
                    osae.RunQuery(command);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error clearing method queue details: \r\n" + ex.Message, true);
            }

            OSAEObjectManager objectManager = new OSAEObjectManager();

            logging.AddToLog("Creating Computer object", true);
            if (objectManager.GetObjectByName(osae.ComputerName) == null)
            {
                
                OSAEObject obj = objectManager.GetObjectByAddress(_computerIP);
                if (obj == null)
                {
                    objectManager.ObjectAdd(osae.ComputerName, osae.ComputerName, "COMPUTER", _computerIP, "", true);
                    osae.ObjectPropertySet(osae.ComputerName, "Host Name", osae.ComputerName);
                }
                else if (obj.Type == "COMPUTER")
                {
                    objectManager.ObjectUpdate(obj.Name, osae.ComputerName, obj.Description, "COMPUTER", _computerIP, obj.Container, obj.Enabled);
                    osae.ObjectPropertySet(osae.ComputerName, "Host Name", osae.ComputerName);
                }
                else
                {
                    objectManager.ObjectAdd(osae.ComputerName + "." + _computerIP, osae.ComputerName, "COMPUTER", _computerIP, "", true);
                    osae.ObjectPropertySet(osae.ComputerName + "." + _computerIP, "Host Name", osae.ComputerName);
                }
            }
            else
            {
                OSAEObject obj = objectManager.GetObjectByName(osae.ComputerName);
                objectManager.ObjectUpdate(obj.Name, obj.Name, obj.Description, "COMPUTER", _computerIP, obj.Container, obj.Enabled);
                osae.ObjectPropertySet(obj.Name, "Host Name", osae.ComputerName);
            }

            try
            {
                logging.AddToLog("Creating Service object", true);
                OSAEObject svcobj = objectManager.GetObjectByName("SERVICE-" + osae.ComputerName);
                if (svcobj == null)
                    objectManager.ObjectAdd("SERVICE-" + osae.ComputerName, "SERVICE-" + osae.ComputerName, "SERVICE", "", "SYSTEM", true);
                osae.ObjectStateSet("SERVICE-" + osae.ComputerName, "ON");
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error creating service object - " + ex.Message, true);
            }

            try
            {
                serviceHost.Open();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error starting RESTful web service: " + ex.Message, true);
            }
            
            wcfService = new WCF.WCFService();
            sHost = new ServiceHost(wcfService);
            wcfService.MessageReceived += new EventHandler<WCF.CustomEventArgs>(wcfService_MessageReceived);
            try
            {
                sHost.Open();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error starting WCF service: " + ex.Message, true);
            }

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

        /// <summary>
        /// OnStop: Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            ShutDownSystems();
        }

        private void ShutDownSystems()
        {
            int waitPeriod = 15000; // Milliseconds
            logging.AddToLog("stopping...", true);
            try
            {
                // run the shutdown as a task so that the plugins don't prevent us from closing
                // as we can't guarantee the quality of a third party plugin
                var taskA = new Task(() =>
                {
                    checkPlugins.Enabled = false;
                    running = false;
                    if (sHost.State == CommunicationState.Opened)
                        sHost.Close();
                    serviceHost.Close();
                    logging.AddToLog("shutting down plugins", true);
                    foreach (Plugin p in plugins)
                    {
                        if (p.Enabled)
                        {
                            p.Shutdown();
                        }
                    }
                });

                if (!taskA.Wait(15000))
                {
                    logging.AddToLog("Failed to shutdown plugins after: " + (waitPeriod / 1000) + " seconds shutting down anyway", true);
                }
            }
            catch { }
        }

        protected override void OnShutdown() 
        {
            ShutDownSystems();
        }
        #endregion

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void QueryCommandQueue()
        {
            //timer.Enabled = false;
            while (running)
            {
                try
                {
                    DataSet dataset = new DataSet();
                    MySqlCommand command = new MySqlCommand();
                    command.CommandText = "SELECT method_queue_id, object_name, address, method_name, parameter_1, parameter_2, object_owner FROM osae_v_method_queue ORDER BY entry_time";
                    dataset = osae.RunQuery(command);

                    foreach (DataRow row in dataset.Tables[0].Rows)
                    {
                        OSAEMethod method = new OSAEMethod(row["method_name"].ToString(), row["object_name"].ToString(), row["parameter_1"].ToString(), row["parameter_2"].ToString(), row["address"].ToString(), row["object_owner"].ToString());
                        
                        sendMessageToClients("log", "found method in queue: " + method.ObjectName +
                            "(" + method.MethodName + ")   p1: " + method.Parameter1 +
                            "  p2: " + method.Parameter2);
                        logging.AddToLog("Found method in queue: " + method.MethodName, false);
                        logging.AddToLog("-- object name: " + method.ObjectName, false);
                        logging.AddToLog("-- param 1: " + method.Parameter1, false);
                        logging.AddToLog("-- param 2: " + method.Parameter2, false);
                        logging.AddToLog("-- object owner: " + method.Owner, false);

                        if (method.ObjectName == "SERVICE-" + osae.ComputerName)
                        {
                            if (method.MethodName == "EXECUTE")
                            {
                                sendMessageToClients("command", method.Parameter1
                                    + " | " + method.Parameter2 + " | " + osae.ComputerName);
                            }
                            else if (method.MethodName == "START PLUGIN")
                            {
                                foreach (Plugin p in plugins)
                                {
                                    if (p.PluginName == method.Parameter1)
                                    {
                                        OSAEObjectManager objectManager = new OSAEObjectManager();

                                        OSAEObject obj = objectManager.GetObjectByName(p.PluginName);
                                        if (obj != null)
                                        {
                                            enablePlugin(p);
                                        }
                                    }
                                }
                            }
                            else if (method.MethodName == "STOP PLUGIN")
                            {
                                foreach (Plugin p in plugins)
                                {
                                    if (p.PluginName == method.Parameter1)
                                    {
                                        OSAEObjectManager objectManager = new OSAEObjectManager();

                                        OSAEObject obj = objectManager.GetObjectByName(p.PluginName);
                                        if (obj != null)
                                        {
                                            disablePlugin(p);
                                        }
                                    }
                                }
                            }
                            else if (method.MethodName == "LOAD PLUGIN")
                            {
                                LoadPlugins();
                            }
                            command.CommandText = "DELETE FROM osae_method_queue WHERE method_queue_id=" + row["method_queue_id"].ToString();
                            logging.AddToLog("Removing method from queue: " + command.CommandText, false);
                            osae.RunQuery(command);
                        }
                        else
                        {
                            bool processed = false;
                            foreach (Plugin plugin in plugins)
                            {
                                if (plugin.Enabled == true && (method.Owner.ToLower() == plugin.PluginName.ToLower() || method.ObjectName.ToLower() == plugin.PluginName.ToLower()))
                                {
                                    command.CommandText = "DELETE FROM osae_method_queue WHERE method_queue_id=" + row["method_queue_id"].ToString();
                                    logging.AddToLog("Removing method from queue: " + command.CommandText, false);
                                    osae.RunQuery(command);
                                   
                                    plugin.ExecuteCommand(method);
                                    processed = true;
                                    break;
                                }
                            }

                            if (!processed)
                            {
                                sendMessageToClients("method", method.ObjectName + " | " + method.Owner + " | "
                                    + method.MethodName + " | " + method.Parameter1 + " | " + method.Parameter2 + " | " 
                                    + method.Address + " | " + row["method_queue_id"].ToString());

                                
                                command.CommandText = "DELETE FROM osae_method_queue WHERE method_queue_id=" + row["method_queue_id"].ToString();
                                logging.AddToLog("Removing method from queue: " + command.CommandText, false);
                                osae.RunQuery(command);
                                processed = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logging.AddToLog("Error in QueryCommandQueue: " + ex.Message, true);
                    //timer.Enabled = true;
                }
                System.Threading.Thread.Sleep(100);
            }
            //timer.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadPlugins()
        {
            var pluginAssemblies = new List<OSAEPluginBase>();
            var types = PluginFinder.FindPlugins();

            logging.AddToLog("Loading Plugins", true);

            foreach (var type in types)
            {
                logging.AddToLog("type.TypeName: " + type.TypeName, false);
                logging.AddToLog("type.AssemblyName: " + type.AssemblyName, false);

                var domain = CreateSandboxDomain("Sandbox Domain", type.Location, SecurityZone.Internet);

                plugins.Add(new Plugin(type.AssemblyName, type.TypeName, domain, type.Location));
            }

            logging.AddToLog("Found " + plugins.Count.ToString() + " plugins", true);
            MySqlConnection connection = new MySqlConnection(Common.ConnectionString);

            foreach (Plugin plugin in plugins)
            {
                try
                {
                    logging.AddToLog("---------------------------------------", true);
                    logging.AddToLog("Plugin name: " + plugin.PluginName, true);
                    logging.AddToLog("Testing connection", true);
                    if (!goodConnection)
                    {
                        try
                        {
                            connection.Open();
                            goodConnection = true;
                        }
                        catch
                        {
                        }
                    }

                    if (goodConnection)
                    {
                        if (plugin.PluginName != "")
                        {
                            OSAEObjectManager objectManager = new OSAEObjectManager();
                            OSAEObject obj = objectManager.GetObjectByName(plugin.PluginName);

                            if (obj != null)
                            {
                                logging.AddToLog("Plugin Object found: " + obj.Name + " - Enabled: " + obj.Enabled.ToString(), true);
                                if (obj.Enabled == 1)
                                {
                                    enablePlugin(plugin);
                                }
                                else
                                    plugin.Enabled = false;

                                logging.AddToLog("Status: " + plugin.Enabled.ToString(), true);
                                logging.AddToLog("PluginVersion: " + plugin.PluginVersion, true);
                            }
                        }
                        else
                        {
                            //add code to create the object.  We need the plugin to specify the type though

                            MySqlDataAdapter adapter;
                            DataSet dataset = new DataSet();
                            MySqlCommand command = new MySqlCommand();
                            OSAEObjectManager objectManager = new OSAEObjectManager();

                            command.Connection = connection;
                            command.CommandText = "SELECT * FROM osae_object_type_property p inner join osae_object_type t on p.object_type_id = t.object_type_id WHERE object_type=@type AND property_name='Computer Name'";
                            command.Parameters.AddWithValue("@type", plugin.PluginType);
                            adapter = new MySqlDataAdapter(command);
                            adapter.Fill(dataset);

                            if (dataset.Tables[0].Rows.Count > 0)
                                plugin.PluginName = plugin.PluginType + "-" + osae.ComputerName;
                            else
                                plugin.PluginName = plugin.PluginType;
                            logging.AddToLog("Plugin object does not exist in DB: " + plugin.PluginName, true);
                            objectManager.ObjectAdd(plugin.PluginName, plugin.PluginName, plugin.PluginType, "", "System", false);
                            osae.ObjectPropertySet(plugin.PluginName, "Computer Name", osae.ComputerName);

                            logging.AddToLog("Plugin added to DB: " + plugin.PluginName, true);
                            sendMessageToClients("plugin", plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Stopped | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + osae.ComputerName);

                        }
                        masterPlugins.Add(plugin);
                    }


                }
                catch (Exception ex)
                {
                    logging.AddToLog("Error loading plugin: " + ex.Message, true);
                }
                catch
                {
                    logging.AddToLog("Error loading plugin", true);
                }
            }

        }

        #region WCF Events and Methods

        /// <summary>
        /// Event happens when a wcf client invokes it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void wcfService_MessageReceived(object source, WCF.CustomEventArgs e)
        {
            try
            {
                logging.AddToLog("received message: " + e.Message, false);
                if (e.Message == "connected")
                {
                    try
                    {
                        logging.AddToLog("client connected", false);
                        foreach (Plugin p in masterPlugins)
                        {
                            string msg = p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | " + p.Status + " | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + osae.ComputerName;

                            sendMessageToClients("plugin", msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        logging.AddToLog("Error sending plugin messages to clients: " + ex.Message, true);
                    }
                }
                else
                {
                    string[] arguments = e.Message.Split('|');
                    if (arguments[0] == "ENABLEPLUGIN")
                    {
                        bool local = false;
                        if (arguments[2] == "True")
                            osae.ObjectStateSet(arguments[1], "ON");
                        else if (arguments[2] == "False")
                            osae.ObjectStateSet(arguments[1], "OFF");
                        foreach (Plugin p in plugins)
                        {
                            if (p.PluginName == arguments[1])
                            {
                                local = true;
                                OSAEObjectManager objectManager = new OSAEObjectManager();

                                OSAEObject obj = objectManager.GetObjectByName(p.PluginName);
                                if (obj != null)
                                {
                                    if (arguments[2] == "True")
                                    {                                        
                                        enablePlugin(p);
                                    }
                                    else if (arguments[2] == "False")
                                    {
                                        disablePlugin(p);
                                    }
                                }
                            }
                        }
                        if (!local)
                        {
                            sendMessageToClients("enablePlugin", e.Message);
                        }
                    }
                    else if (arguments[0] == "plugin")
                    {
                        bool found = false;
                        foreach (Plugin plugin in masterPlugins)
                        {

                            if (plugin.PluginName == arguments[1])
                            {
                                if (arguments[4].ToLower() == "true")
                                    plugin.Enabled = true;
                                else
                                    plugin.Enabled = false;
                                plugin.PluginVersion = arguments[3];
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            Plugin p = new Plugin();
                            p.PluginName = arguments[1];
                            p.PluginVersion = arguments[3];
                            if (arguments[4].ToLower() == "true")
                                p.Enabled = true;
                            else
                                p.Enabled = false;
                            masterPlugins.Add(p);
                        }
                    }
                    else if (arguments[0] == "updatePlugin")
                    {
                        foreach (Plugin plugin in masterPlugins)
                        {
                            if (plugin.PluginName == arguments[1])
                            {
                                if (plugin.Status == "Running")
                                    disablePlugin(plugin);

                                //code for downloading and installing plugin
                                break;
                            }
                        }
                    }
                }

                logging.AddToLog("-----------Master plugin list", false);
                foreach (Plugin p in masterPlugins)
                    logging.AddToLog(" --- " + p.PluginName, false);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error receiving message: " + ex.Message, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="message"></param>
        private void sendMessageToClients(string msgType, string message)
        {
            try
            {
                logging.AddToLog("Sending message to clients: " + msgType + " - " + message, false);
                Thread thread = new Thread(() => wcfService.SendMessageToClients(msgType, message, osae.ComputerName));
                thread.Start();
            }
            catch(Exception ex)
            {
                logging.AddToLog("Error sending message to clients: " + ex.Message, true);
            }
        }

        #endregion

        #region Check For Plugin Updates

        private void getPluginUpdates_tick(object source, EventArgs e)
        {
            //getPluginUpdates();
            Thread usageThread = new Thread(() => getPluginUpdates());
            usageThread.Start();
        }

        private void checkForUpdates(string name, string version)
        {
            try
            {
                Plugin p = new Plugin();
                bool plug = false;
                foreach (Plugin plugin in plugins)
                {
                    if (plugin.PluginType == name)
                    {
                        p = plugin;
                        plug = true;
                    }
                }
                int curMajor, curMinor, curRevion, latestMajor = 0, latestMinor = 0, latestRevision = 0;
                string[] split = version.Split('.');
                curMajor = Int32.Parse(split[0]);
                curMinor = Int32.Parse(split[1]);
                curRevion = Int32.Parse(split[2]);

                string url = "http://www.opensourceautomation.com/pluginUpdates.php?app=" + name + "&ver=" + version;
                logging.AddToLog("Checking for plugin updates: " + url, false);
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                //XmlReader rdr = XmlReader.Create(responseStream);
                
                XElement xml  = XElement.Load(responseStream);
                logging.AddToLog("XML retreived", false);

                var query = from e in xml.Elements("plugin")
                            select new { B = e.Element("major").Value, C = e.Element("minor").Value, D = e.Element("revision").Value };

                foreach (var e in query)
                {
                    latestMajor = Int32.Parse(e.B);
                    latestMinor = Int32.Parse(e.C);
                    latestRevision = Int32.Parse(e.D);
                }
                
                
                if (latestMajor >= curMajor)
                {
                    if (latestMinor >= curMinor)
                    {
                        if (latestRevision > curRevion)
                        {
                            p.LatestAvailableVersion = latestMajor + "." + latestMinor + "." + latestRevision;
                            logging.AddToLog("current version: " + curMajor + "." + curMinor + "." + curRevion, false);
                            logging.AddToLog("latest version: " + p.LatestAvailableVersion, false);
                            string msg;

                            if (!plug)
                            {
                                msg = version + "|" + latestMajor + "." + latestMinor + "." + latestRevision;
                                sendMessageToClients("service", msg);
                            }
                        }
                    }
                }
                
                response.Close();

                if (plug)
                {
                    string msg = p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | " + p.Status + " | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + osae.ComputerName;
                    sendMessageToClients("plugin", msg);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("plugin update error: " + ex.Message, true);
            }
        }

        private void getPluginUpdates()
        {
            foreach (Plugin plugin in plugins)
            {
                logging.AddToLog("Checking for update: " + plugin.PluginName, false);
                try
                {
                    //Thread usageThread = new Thread(() => checkForUpdates(plugin.PluginType, plugin.PluginVersion));
                    //usageThread.Start();
                    checkForUpdates(plugin.PluginType, plugin.PluginVersion);
                }
                catch { }
            }
            try
            {
                checkForUpdates("Service", osae.GetObjectPropertyValue("SYSTEM", "DB Version").Value);
            }
            catch { }
            
        }

        #endregion

        #region Monitor Plugins

        private void checkPlugins_tick(object source, EventArgs e)
        {
            //foreach (Plugin plugin in plugins)
            //{
            //    try
            //    {
            //        if (plugin.Enabled)
            //        {
            //            Process process = Process.GetProcessById(plugin.process.ProcessId);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        logging.AddToLog(plugin.PluginName + " - Plugin has crashed. Attempting to restart.", true);
            //        enablePlugin(plugin);
            //        logging.AddToLog("New Process ID: " + plugin.process.ProcessId, true);
            //    }
            
            //}

        }

        #endregion

        #region Helper functions
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

        public void enablePlugin(Plugin plugin)
        {
            OSAEObjectManager objectManager = new OSAEObjectManager();
            OSAEObject obj = objectManager.GetObjectByName(plugin.PluginName);

            objectManager.ObjectUpdate(plugin.PluginName, plugin.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 1);
            try
            {
                if (plugin.ActivatePlugin())
                {
                    plugin.Enabled = true;
                    plugin.RunInterface();
                    osae.ObjectStateSet(plugin.PluginName, "ON");
                    sendMessageToClients("plugin", plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Running | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + osae.ComputerName);
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
            logging.AddToLog("Disabling Plugin: " + p.PluginName,true);
           
            OSAEObjectManager objectManager = new OSAEObjectManager();

            OSAEObject obj = objectManager.GetObjectByName(p.PluginName);
            objectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 0);
            try
            {
                p.Shutdown();
                p.Enabled = false;
                p.Domain = CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet);
                sendMessageToClients("plugin", p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | Stopped | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + osae.ComputerName);
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

        #endregion

    }

}
