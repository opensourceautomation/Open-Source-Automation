namespace ClientService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Security.Policy;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Threading;
    using Microsoft.Win32;
    using OSAE;    

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class ClientService : ServiceBase, WCFServiceReference.IWCFServiceCallback, IDisposable
    {
        private const string sourceName = "Client Service";
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger(sourceName);

        private WCFServiceReference.WCFServiceClient wcfObj;
        private List<Plugin> plugins = new List<Plugin>();
        private string _computerIP;
        private ModifyRegistry myRegistry = new ModifyRegistry();
        System.Timers.Timer Clock = new System.Timers.Timer();

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string pattern = Common.MatchPattern(args[0]);
                Logging.AddToLog("Processing command: " + args[0] + ", Pattern: " + pattern, true, "OSACL");
                if (pattern != "")
                    OSAEMethodManager.MethodQueueAdd("Script Processor", "NAMED SCRIPT", pattern, "", "OSACL");
            }
            else
            {
                ServiceBase.Run(new ClientService());
            }
        }

        public ClientService()
        {
            
            logging.AddToLog("ClientService Starting", true);

            try
            {
                if (!EventLog.SourceExists("OSAEClient"))
                    EventLog.CreateEventSource("OSAEClient", "Application");
            }
            catch (Exception ex)
            {
                logging.AddToLog("CreateEventSource error: " + ex.Message, true);
            }
            this.ServiceName = "OSAEClient";
            this.EventLog.Source = "OSAEClient";
            this.EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.

            this.CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            try
            {               
                IPHostEntry ipEntry = Dns.GetHostByName(Common.ComputerName);
                IPAddress[] addr = ipEntry.AddressList;
                _computerIP = addr[0].ToString();

                System.IO.FileInfo file = new System.IO.FileInfo(Common.ApiPath + "/Logs/");
                file.Directory.Create();
                if (ObjectPopertiesManager.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value == "TRUE")
                {
                    string[] files = Directory.GetFiles(Common.ApiPath + "/Logs/");
                    foreach (string f in files)
                        File.Delete(f);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error getting registry settings and/or deleting logs: " + ex.Message, true);
            }

            logging.AddToLog("OnStart", true);
            logging.AddToLog("Creating Computer object: " + Common.ComputerName, true);

            if (OSAEObjectManager.GetObjectByName(Common.ComputerName) == null)
            {
                OSAEObject obj = OSAEObjectManager.GetObjectByAddress(_computerIP);
                if (obj == null)
                {
                    OSAEObjectManager.ObjectAdd(Common.ComputerName, Common.ComputerName, "COMPUTER", _computerIP, "", true);
                    ObjectPopertiesManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else if (obj.Type == "COMPUTER")
                {
                    OSAEObjectManager.ObjectUpdate(obj.Name, Common.ComputerName, obj.Description, "COMPUTER", _computerIP, obj.Container, obj.Enabled);
                    ObjectPopertiesManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else
                {
                    OSAEObjectManager.ObjectAdd(Common.ComputerName + "." + _computerIP, Common.ComputerName, "COMPUTER", _computerIP, "", true);
                    ObjectPopertiesManager.ObjectPropertySet(Common.ComputerName + "." + _computerIP, "Host Name", Common.ComputerName, sourceName);
                }
            }
            else
            {
                OSAEObject obj = OSAEObjectManager.GetObjectByName(Common.ComputerName);
                OSAEObjectManager.ObjectUpdate(obj.Name, obj.Name, obj.Description, "COMPUTER", _computerIP, obj.Container, obj.Enabled);
                ObjectPopertiesManager.ObjectPropertySet(obj.Name, "Host Name", Common.ComputerName, sourceName);
            }

            try
            {
                logging.AddToLog("Creating Service object", true);
                OSAEObject svcobj = OSAEObjectManager.GetObjectByName("SERVICE-" + Common.ComputerName);
                if (svcobj == null)
                    OSAEObjectManager.ObjectAdd("SERVICE-" + Common.ComputerName, "SERVICE-" + Common.ComputerName, "SERVICE", "", "SYSTEM", true);
                ObjectStateManager.ObjectStateSet("SERVICE-" + Common.ComputerName, "ON", sourceName);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error creating service object - " + ex.Message, true);
            }

            if (connectToService())
            {

                //LoadPlugins();
                Thread loadPluginsThread = new Thread(new ThreadStart(LoadPlugins));
                loadPluginsThread.Start();
            }

            Clock.Interval = 5000;
            Clock.Start();
            Clock.Elapsed += new System.Timers.ElapsedEventHandler(checkConnection);
        }

        protected override void OnStop()
        {
            try
            {
                if (wcfObj.State == CommunicationState.Opened)
                {
                    wcfObj.Unsubscribe();
                    wcfObj.Close();
                }
                foreach (Plugin p in plugins)
                {
                    if (p.Enabled)
                    {
                        p.Shutdown();
                    }
                }
            }
            catch { }
        }

        public void LoadPlugins()
        {
            logging.AddToLog("Entered LoadPlugins", true);
            string path = Common.ApiPath;

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

            foreach (Plugin plugin in plugins)
            {
                try
                {
                    logging.AddToLog("---------------------------------------", true);
                    logging.AddToLog("plugin name: " + plugin.PluginName, true);
                    logging.AddToLog("plugin type: " + plugin.PluginType, true);

                    if (plugin.PluginName != string.Empty)
                    {                       
                        OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);

                        logging.AddToLog("setting found: " + obj.Name + " - " + obj.Enabled.ToString(), true);
                        bool isSystemPlugin = false;
                        foreach (ObjectProperty p in obj.Properties)
                        {
                            if (p.Name == "System Plugin")
                            {
                                if (p.Value == "TRUE")
                                    isSystemPlugin = true;
                                break;
                            }
                        }
                        logging.AddToLog("isSystemPlugin?: " + isSystemPlugin.ToString(), true);
                        if (!isSystemPlugin)
                        {
                            if (obj.Enabled == 1)
                            {
                                try
                                {
                                    enablePlugin(plugin);
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
                            else
                                plugin.Enabled = false;

                            logging.AddToLog("status: " + plugin.Enabled.ToString(), true);
                            logging.AddToLog("PluginName: " + plugin.PluginName, true);
                            logging.AddToLog("PluginVersion: " + plugin.PluginVersion, true);
                            Thread thread = new Thread(() => messageHost("plugin", "plugin|" + plugin.PluginName + "|" + plugin.Status
                                + "|" + plugin.PluginVersion + "|" + plugin.Enabled));
                            thread.Start(); 
                        }
                        //else
                        //    plugins.Remove(plugin);
                    }
                    else
                    {
                        //add code to create the object.  We need the plugin to specify the type though
                        logging.AddToLog("Plugin object doesn't exist", true);
                        DataSet dataset = wcfObj.ExecuteSQL("SELECT * FROM osae_object_type_property p inner join osae_object_type t on p.object_type_id = t.object_type_id WHERE object_type='" + plugin.PluginType + "' AND property_name='Computer Name'");
                        logging.AddToLog("dataset count: " + dataset.Tables[0].Rows.Count.ToString(), true);

                        // if object type has a property called 'Computer Name' we know it is not a System Plugin
                        if (dataset.Tables[0].Rows.Count > 0)
                        {
                            plugin.PluginName = plugin.PluginType + "-" + Common.ComputerName;

                            logging.AddToLog("Plugin object does not exist in DB: " + plugin.PluginName, true);
                            OSAEObjectManager.ObjectAdd(plugin.PluginName, plugin.PluginName, plugin.PluginType, "", "System", false);
                            ObjectPopertiesManager.ObjectPropertySet(plugin.PluginName, "Computer Name", Common.ComputerName, "Client Service");

                            logging.AddToLog("Plugin added to DB: " + plugin.PluginName, true);
                            Thread thread = new Thread(() => messageHost("plugin", "plugin|" + plugin.PluginName + "|" + plugin.Status
                                + "|" + plugin.PluginVersion + "|" + plugin.Enabled));
                            thread.Start(); 
                        }

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
            logging.AddToLog("Done loading plugins", true);
        }

        private bool connectToService()
        {
            try
            {
                EndpointAddress ep = new EndpointAddress("net.tcp://" + Common.ConnectionString + ":8731/WCFService/");
                InstanceContext context = new InstanceContext(this);
                wcfObj = new WCFServiceReference.WCFServiceClient(context, "NetTcpBindingEndpoint", ep);
                wcfObj.Subscribe();
                logging.AddToLog("Connected to Service", true);

                return true;
            }
            catch (Exception ex)
            {
                logging.AddToLog("Unable to connect to service.  Is it running? - " + ex.Message, true);
                return false;
            }
        }

        public void OnMessageReceived(string msgType, string message, string from, DateTime timestamp)
        {
            logging.AddToLog("received message: " + msgType + " | " + message, false);
            switch (msgType)
            {
                case "plugin":
                    string[] arguments = message.Split('|');
                
                    if (arguments[1] == "True")
                        ObjectStateManager.ObjectStateSet(arguments[0], "ON", sourceName);
                    else if (arguments[1] == "False")
                        ObjectStateManager.ObjectStateSet(arguments[0], "OFF", sourceName);

                    foreach (Plugin p in plugins)
                    {                        
                        if (p.PluginName == arguments[0])
                        {
                            OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);

                            if (obj != null)
                            {
                                bool isSystemPlugin = false;
                                foreach (ObjectProperty p2 in obj.Properties)
                                {
                                    if (p2.Name == "System Plugin")
                                    {
                                        if (p2.Value == "TRUE")
                                            isSystemPlugin = true;
                                        break;
                                    }
                                }
                                if (arguments[1] == "True" && !p.Enabled && !isSystemPlugin)
                                {
                                    OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 1);
                                    try
                                    {
                                        enablePlugin(p);
                                        logging.AddToLog("Activated plugin: " + p.PluginName, false);
                                    }
                                    catch (Exception ex)
                                    {
                                        logging.AddToLog("Error activating plugin (" + p.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
                                    }
                                }
                                else if (arguments[1] == "False" && p.Enabled && !isSystemPlugin)
                                {
                                    OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 0);
                                    try
                                    {
                                        disablePlugin(p);
                                        logging.AddToLog("Deactivated plugin: " + p.PluginName, false);
                                    }
                                    catch (Exception ex)
                                    {
                                        logging.AddToLog("Error stopping plugin (" + p.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "method":
                    string[] items = message.Split('|');
                    DataTable dt = new DataTable();
                    DataColumn col1 = new DataColumn("object_name");
                    DataColumn col2 = new DataColumn("object_owner");
                    DataColumn col3 = new DataColumn("method_name");
                    DataColumn col4 = new DataColumn("parameter_1");
                    DataColumn col5 = new DataColumn("parameter_2");
                    DataColumn col6 = new DataColumn("address");
                    col1.DataType = System.Type.GetType("System.String");
                    col2.DataType = System.Type.GetType("System.String");
                    col3.DataType = System.Type.GetType("System.String");
                    col4.DataType = System.Type.GetType("System.String");
                    col5.DataType = System.Type.GetType("System.String");
                    col6.DataType = System.Type.GetType("System.String");
                    dt.Columns.Add(col1);
                    dt.Columns.Add(col2);
                    dt.Columns.Add(col3);
                    dt.Columns.Add(col4);
                    dt.Columns.Add(col5);
                    dt.Columns.Add(col6);
                    DataRow row = dt.NewRow();
                    row[col1] = items[0].Trim();
                    row[col2] = items[1].Trim();
                    row[col3] = items[2].Trim();
                    row[col4] = items[3].Trim();
                    row[col5] = items[4].Trim();
                    row[col6] = items[5].Trim();
                    dt.Rows.Add(row);
                    
                    OSAEMethod method = new OSAEMethod(row["method_name"].ToString(), row["object_name"].ToString(), row["parameter_1"].ToString(), row["parameter_2"].ToString(), row["address"].ToString(), row["object_owner"].ToString());
                    dt = null;

                    if (method.ObjectName == "SERVICE-" + Common.ComputerName)
                    {
                        if (method.MethodName == "RESTART PLUGIN")
                        {
                            foreach (Plugin p in plugins)
                            {
                                if (p.PluginName == method.Parameter1)
                                {
                                    OSAEObjectManager objectManager = new OSAEObjectManager();
                                    OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);

                                    if (obj != null)
                                    {
                                        disablePlugin(p);
                                        enablePlugin(p);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Plugin plugin in plugins)
                        {
                            string x = row["object_owner"].ToString().ToLower();
                            string y = row["object_name"].ToString().ToLower();

                            if (plugin.Enabled == true && (row["object_owner"].ToString().ToLower() == plugin.PluginName.ToLower() || row["object_name"].ToString().ToLower() == plugin.PluginName.ToLower()))
                            {
                                plugin.ExecuteCommand(method);
                            }
                        }
                    }
                    break;
                case "enablePlugin":
                    string[] plug = message.Split('|');

                    if (plug[0] == "ENABLEPLUGIN")
                    {
                        foreach (Plugin p in plugins)
                        {
                            if (p.PluginName == plug[1])
                            {
                                OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);

                                if (obj != null)
                                {
                                    if (plug[2] == "True")
                                    {
                                        enablePlugin(p);
                                    }
                                    else if (plug[2] == "False")
                                    {
                                        disablePlugin(p);
                                    }
                                }
                            }
                        }
                    }

                    break;
            }
        }

        private void checkConnection(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (wcfObj == null || wcfObj.State == CommunicationState.Closed || wcfObj.State == CommunicationState.Faulted)
            {
                connectToService();
            }
        }

        private void messageHost(string msgType, string message)
        {
            try
            {
                if (wcfObj.State == CommunicationState.Opened)
                    wcfObj.messageHost(msgType, message, Common.ComputerName);
                else
                {
                    if (connectToService())
                        wcfObj.messageHost(msgType, message, Common.ComputerName);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error messaging host: " + ex.Message, true);
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
                    plugin.RunInterface();
                    ObjectStateManager.ObjectStateSet(plugin.PluginName, "ON", sourceName);
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
            OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);

            OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 0);
            try
            {
                p.Shutdown();
                p.Enabled = false;
                p.Domain = CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet);
                
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

            var strongName = typeof(ClientService).Assembly.Evidence.GetHostEvidence<StrongName>();

            return AppDomain.CreateDomain(name, null, setup);
        }

    }

    public class ModifyRegistry
    {
        private string subKey;

        public string SubKey
        {
            get { return subKey; }
            set { subKey = value; }
        }

        private RegistryKey baseRegistryKey = Registry.LocalMachine;
        /// <summary>
        /// A property to set the BaseRegistryKey value.
        /// (default = Registry.LocalMachine)
        /// </summary>
        public RegistryKey BaseRegistryKey
        {
            get { return baseRegistryKey; }
            set { baseRegistryKey = value; }
        }

        /* **************************************************************************
         * **************************************************************************/

        /// <summary>
        /// To read a registry key.
        /// input: KeyName (string)
        /// output: value (string) 
        /// </summary>
        public string Read(string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = baseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /* **************************************************************************
         * **************************************************************************/

        /// <summary>
        /// To write into a registry key.
        /// input: KeyName (string) , Value (object)
        /// output: true or false 
        /// </summary>
        public bool Write(string KeyName, object Value)
        {
            try
            {
                // Setting
                RegistryKey rk = baseRegistryKey;
                // I have to use CreateSubKey 
                // (create or open it if already exits), 
                // 'cause OpenSubKey open a subKey as read-only
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                // Save the value
                sk1.SetValue(KeyName.ToUpper(), Value);

                return true;
            }
            catch (Exception)
            {              
                return false;
            }
        }
    }

}
