using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Win32;
using OpenSourceAutomation;
using OSAE;

namespace ClientService
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class ClientService : ServiceBase, WCFServiceReference.IWCFServiceCallback, IDisposable
    {
        private WCFServiceReference.WCFServiceClient wcfObj;
        private List<Plugin> plugins = new List<Plugin>();
        private string _computerIP;
        private ModifyRegistry myRegistry = new ModifyRegistry();
        private OSAE.OSAE osae = new OSAE.OSAE("Client Service");
        System.Timers.Timer Clock = new System.Timers.Timer();

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                OSAE.OSAE osacl = new OSAE.OSAE("OSACL");
                string pattern = osacl.MatchPattern(args[0]);
                osacl.AddToLog("Processing command: " + args[0] + ", Pattern: " + pattern, true);
                if (pattern != "")
                    osacl.MethodQueueAdd("Script Processor", "NAMED SCRIPT", pattern, "");
            }
            else
            {
                ServiceBase.Run(new ClientService());
            }
        }

        public ClientService()
        {
            osae.AddToLog("ClientService Starting", true);

            try
            {
                if (!EventLog.SourceExists("OSAEClient"))
                    EventLog.CreateEventSource("OSAEClient", "Application");
            }
            catch (Exception ex)
            {
                osae.AddToLog("CreateEventSource error: " + ex.Message, true);
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
                myRegistry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";
                osae.APIpath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
                IPHostEntry ipEntry = Dns.GetHostByName(osae.ComputerName);
                IPAddress[] addr = ipEntry.AddressList;
                _computerIP = addr[0].ToString();

                System.IO.FileInfo file = new System.IO.FileInfo(osae.APIpath + "/Logs/");
                file.Directory.Create();
                if (osae.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value == "TRUE")
                {
                    string[] files = Directory.GetFiles(osae.APIpath + "/Logs/");
                    foreach (string f in files)
                        File.Delete(f);
                }
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error getting registry settings and/or deleting logs: " + ex.Message, true);
            }

            osae.AddToLog("OnStart", true);

            osae.AddToLog("Creating Computer object: " + osae.ComputerName, true);
            if (osae.GetObjectByName(osae.ComputerName) == null)
            {
                OSAEObject obj = osae.GetObjectByAddress(_computerIP);
                if (obj == null)
                {
                    osae.ObjectAdd(osae.ComputerName, osae.ComputerName, "COMPUTER", _computerIP, "", true);
                    osae.ObjectPropertySet(osae.ComputerName, "Host Name", osae.ComputerName);
                }
                else if (obj.Type == "COMPUTER")
                {
                    osae.ObjectUpdate(obj.Name, osae.ComputerName, obj.Description, "COMPUTER", _computerIP, obj.Container, obj.Enabled);
                    osae.ObjectPropertySet(osae.ComputerName, "Host Name", osae.ComputerName);
                }
                else
                {
                    osae.ObjectAdd(osae.ComputerName + "." + _computerIP, osae.ComputerName, "COMPUTER", _computerIP, "", true);
                    osae.ObjectPropertySet(osae.ComputerName + "." + _computerIP, "Host Name", osae.ComputerName);
                }
            }
            else
            {
                OSAEObject obj = osae.GetObjectByName(osae.ComputerName);
                osae.ObjectUpdate(obj.Name, obj.Name, obj.Description, "COMPUTER", _computerIP, obj.Container, obj.Enabled);
                osae.ObjectPropertySet(obj.Name, "Host Name", osae.ComputerName);
            }

            try
            {
                osae.AddToLog("Creating Service object", true);
                OSAEObject svcobj = osae.GetObjectByName("SERVICE-" + osae.ComputerName);
                if (svcobj == null)
                    osae.ObjectAdd("SERVICE-" + osae.ComputerName, "SERVICE-" + osae.ComputerName, "SERVICE", "", "SYSTEM", true);
                osae.ObjectStateSet("SERVICE-" + osae.ComputerName, "ON");
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error creating service object - " + ex.Message, true);
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
                        osae.AddToLog("Shutting down " + p.PluginName, true);
                        p.addin.Shutdown();
                        p.addin = null;
                    }
                }
            }
            catch { }
        }

        public void LoadPlugins()
        {
            osae.AddToLog("Entered LoadPlugins", true);
            string path = osae.APIpath;

            AddInStore.Update(path);

            Collection<AddInToken> tokens = null;
            tokens = AddInStore.FindAddIns(typeof(IOpenSourceAutomationAddInv2), path);
            foreach (AddInToken token in tokens)
            {
                plugins.Add(new Plugin(token));
            }

            foreach (Plugin plugin in plugins)
            {
                try
                {
                    osae.AddToLog("---------------------------------------", true);
                    osae.AddToLog("plugin name: " + plugin.PluginName, true);
                    osae.AddToLog("plugin type: " + plugin.PluginType, true);

                    if (plugin.PluginName != "")
                    {
                        OSAE.OSAEObject obj = osae.GetObjectByName(plugin.PluginName);
                        osae.AddToLog("setting found: " + obj.Name + " - " + obj.Enabled.ToString(), true);
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
                        osae.AddToLog("isSystemPlugin?: " + isSystemPlugin.ToString(), true);
                        if (!isSystemPlugin)
                        {
                            if (obj.Enabled.ToString() == "1")
                            {
                                try
                                {
                                    if (plugin.ActivatePlugin())
                                        plugin.addin.RunInterface(plugin.PluginName);
                                    osae.ObjectStateSet(plugin.PluginName, "ON");
                                }
                                catch (Exception ex)
                                {
                                    osae.AddToLog("Error activating plugin (" + plugin.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
                                }
                                catch
                                {
                                    osae.AddToLog("Error activating plugin", true);
                                }
                            }
                            else
                                plugin.Enabled = false;

                            osae.AddToLog("status: " + plugin.Enabled.ToString(), true);
                            osae.AddToLog("PluginName: " + plugin.PluginName, true);
                            osae.AddToLog("PluginVersion: " + plugin.PluginVersion, true);
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
                        osae.AddToLog("Plugin object doesn't exist", true);
                        DataSet dataset = wcfObj.ExecuteSQL("SELECT * FROM osae_object_type_property p inner join osae_object_type t on p.object_type_id = t.object_type_id WHERE object_type='" + plugin.PluginType + "' AND property_name='Computer Name'");
                        osae.AddToLog("dataset count: " + dataset.Tables[0].Rows.Count.ToString(), true);

                        // if object type has a property called 'Computer Name' we know it is not a System Plugin
                        if (dataset.Tables[0].Rows.Count > 0)
                        {
                            plugin.PluginName = plugin.PluginType + "-" + osae.ComputerName;

                            osae.AddToLog("Plugin object does not exist in DB: " + plugin.PluginName, true);
                            osae.ObjectAdd(plugin.PluginName, plugin.PluginName, plugin.PluginType, "", "System", false);
                            osae.ObjectPropertySet(plugin.PluginName, "Computer Name", osae.ComputerName);

                            osae.AddToLog("Plugin added to DB: " + plugin.PluginName, true);
                            Thread thread = new Thread(() => messageHost("plugin", "plugin|" + plugin.PluginName + "|" + plugin.Status
                                + "|" + plugin.PluginVersion + "|" + plugin.Enabled));
                            thread.Start(); 
                        }

                    }
                    
                    
                }
                catch (Exception ex)
                {
                    osae.AddToLog("Error loading plugin: " + ex.Message, true);
                }
                catch
                {
                    osae.AddToLog("Error loading plugin", true);
                }
            }
            osae.AddToLog("Done loading plugins", true);
        }

        private bool connectToService()
        {
            try
            {
                EndpointAddress ep = new EndpointAddress("net.tcp://" + osae.DBConnection + ":8731/WCFService/");
                InstanceContext context = new InstanceContext(this);
                wcfObj = new WCFServiceReference.WCFServiceClient(context, "NetTcpBindingEndpoint", ep);
                wcfObj.Subscribe();
                osae.AddToLog("Connected to Service", true);

                return true;
            }
            catch (Exception ex)
            {
                osae.AddToLog("Unable to connect to service.  Is it running? - " + ex.Message, true);
                return false;
            }
        }

        public void OnMessageReceived(string msgType, string message, string from, DateTime timestamp)
        {
            osae.AddToLog("received message: " + msgType + " | " + message, false);
            switch (msgType)
            {
                case "plugin":
                    string[] arguments = message.Split('|');
                
                    if (arguments[1] == "True")
                        osae.ObjectStateSet(arguments[0], "ON");
                    else if (arguments[1] == "False")
                        osae.ObjectStateSet(arguments[0], "OFF");

                    foreach (Plugin p in plugins)
                    {
                        
                        if (p.PluginName == arguments[0])
                        {
                            OSAEObject obj = osae.GetObjectByName(p.PluginName);
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
                                    osae.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 1);
                                    try
                                    {
                                        if (p.ActivatePlugin())
                                            p.addin.RunInterface(p.PluginName);
                                        osae.AddToLog("Activated plugin: " + p.PluginName, false);
                                    }
                                    catch (Exception ex)
                                    {
                                        osae.AddToLog("Error activating plugin (" + p.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
                                    }
                                }
                                else if (arguments[1] == "False" && p.Enabled && !isSystemPlugin)
                                {
                                    osae.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 0);
                                    try
                                    {
                                        p.addin.Shutdown();
                                        p.addin = null;
                                        GC.Collect();
                                        p.Enabled = false;
                                        osae.AddToLog("Deactivated plugin: " + p.PluginName, false);
                                    }
                                    catch (Exception ex)
                                    {
                                        osae.AddToLog("Error stopping plugin (" + p.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
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

                    if (method.ObjectName == "SERVICE-" + osae.ComputerName)
                    {
                        if (method.MethodName == "RESTART PLUGIN")
                        {
                            foreach (Plugin p in plugins)
                            {
                                if (p.PluginName == method.Parameter1)
                                {
                                    OSAEObject obj = osae.GetObjectByName(p.PluginName);
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
                                plugin.addin.ProcessCommand(method);
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
                                OSAEObject obj = osae.GetObjectByName(p.PluginName);
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
                    wcfObj.messageHost(msgType, message, osae.ComputerName);
                else
                {
                    if (connectToService())
                        wcfObj.messageHost(msgType, message, osae.ComputerName);
                }
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error messaging host: " + ex.Message, true);
            }
        }

        public void enablePlugin(Plugin plugin)
        {
            OSAEObject obj = osae.GetObjectByName(plugin.PluginName);
            osae.ObjectUpdate(plugin.PluginName, plugin.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 1);
            try
            {
                if (plugin.ActivatePlugin())
                {
                    plugin.addin.RunInterface(plugin.PluginName);
                    osae.ObjectStateSet(plugin.PluginName, "ON");
                }
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error activating plugin (" + plugin.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
            }
            catch
            {
                osae.AddToLog("Error activating plugin", true);
            }
        }

        public void disablePlugin(Plugin p)
        {
            OSAEObject obj = osae.GetObjectByName(p.PluginName);
            osae.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 0);
            try
            {
                p.addin.Shutdown();
                p.addin = null;
                GC.Collect();
                p.Enabled = false;
                p.process.Shutdown();
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error stopping plugin (" + p.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
            }
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
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    //AddToLog("Registery Read error: " + e.Message);

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
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                //ShowErrorMessage(e, "Writing registry " + KeyName.ToUpper());
                return false;
            }
        }
    }

}
