namespace ClientService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using System.Security.Policy;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Threading;
    using OSAE;
    using NetworkCommsDotNet;

    public partial class ClientService : ServiceBase
    {
        private const string sourceName = "Client Service";
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger(sourceName);

        private OSAEPluginCollection plugins = new OSAEPluginCollection();
        System.Timers.Timer Clock = new System.Timers.Timer();

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string pattern = Common.MatchPattern(args[0]);
                Logging.AddToLog("Processing command: " + args[0] + ", Pattern: " + pattern, true, "OSACL");
                if (pattern != string.Empty)
                {
                    OSAEScriptManager.RunPatternScript(pattern, "", "OSACL");
                }
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
            Common.InitialiseLogFolder();

            logging.AddToLog("OnStart", true);

            try
            {
                logging.AddToLog("Starting UDP listener", false);
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Plugin", PluginMessageReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Commmand", MethodMessageReceived);
                //Start listening for incoming UDP data
                UDPConnection.StartListening(true);
                logging.AddToLog("UPD Listener started", false);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error starting listener:" + ex.Message, false);
            }
            
            Common.CreateComputerObject(sourceName);            

            try
            {
                logging.AddToLog("Creating Service object", true);
                OSAEObject svcobj = OSAEObjectManager.GetObjectByName("SERVICE-" + Common.ComputerName);
                if (svcobj == null)
                {
                    OSAEObjectManager.ObjectAdd("SERVICE-" + Common.ComputerName, "SERVICE-" + Common.ComputerName, "SERVICE", "", "SYSTEM", true);
                }
                OSAEObjectStateManager.ObjectStateSet("SERVICE-" + Common.ComputerName, "ON", sourceName);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error creating service object - " + ex.Message, true);
            }

            Thread loadPluginsThread = new Thread(new ThreadStart(LoadPlugins));
            loadPluginsThread.Start();
            
            //Clock.Interval = 5000;
            //Clock.Start();
            //Clock.Elapsed += new System.Timers.ElapsedEventHandler(checkConnection);
        }

        protected override void OnStop()
        {
            try
            {
                foreach (Plugin p in plugins)
                {
                    if (p.Enabled)
                    {
                        p.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error occured during stop, details: " + ex.Message, true);
            }
        }

        public void LoadPlugins()
        {
            logging.AddToLog("Entered LoadPlugins", true);

            var types = PluginFinder.FindPlugins();

            logging.AddToLog("Loading Plugins", true);

            foreach (var type in types)
            {
                logging.AddToLog("type.TypeName: " + type.TypeName, false);
                logging.AddToLog("type.AssemblyName: " + type.AssemblyName, false);

                var domain = Common.CreateSandboxDomain("Sandbox Domain", type.Location, SecurityZone.Internet, typeof(ClientService));

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
                        foreach (OSAEObjectProperty p in obj.Properties)
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
                            }
                            else
                            {
                                plugin.Enabled = false;
                            }

                            logging.AddToLog("status: " + plugin.Enabled.ToString(), true);
                            logging.AddToLog("PluginName: " + plugin.PluginName, true);
                            logging.AddToLog("PluginVersion: " + plugin.PluginVersion, true);
                            
                            NetworkComms.SendObject("Plugin", Common.WcfServer, 10000, plugin.PluginName + "|" + plugin.Status
                                + "|" + plugin.PluginVersion + "|" + plugin.Enabled);
                        }                       
                    }
                    else
                    {
                        //add code to create the object.  We need the plugin to specify the type though
                        logging.AddToLog("Plugin object doesn't exist", true);
                        DataSet dataset = OSAESql.RunSQL("SELECT * FROM osae_object_type_property p inner join osae_object_type t on p.object_type_id = t.object_type_id WHERE object_type='" + plugin.PluginType + "' AND property_name='Computer Name'");
                        logging.AddToLog("dataset count: " + dataset.Tables[0].Rows.Count.ToString(), true);

                        // if object type has a property called 'Computer Name' we know it is not a System Plugin
                        if (dataset.Tables[0].Rows.Count > 0)
                        {
                            plugin.PluginName = plugin.PluginType + "-" + Common.ComputerName;

                            logging.AddToLog("Plugin object does not exist in DB: " + plugin.PluginName, true);
                            OSAEObjectManager.ObjectAdd(plugin.PluginName, plugin.PluginName, plugin.PluginType, "", "System", false);
                            OSAEObjectPropertyManager.ObjectPropertySet(plugin.PluginName, "Computer Name", Common.ComputerName, "Client Service");

                            logging.AddToLog("Plugin added to DB: " + plugin.PluginName, true);
                            NetworkComms.SendObject("Plugin", Common.WcfServer, 10000, plugin.PluginName + "|" + plugin.Status
                                + "|" + plugin.PluginVersion + "|" + plugin.Enabled);
                        }

                    }                                        
                }
                catch (Exception ex)
                {
                    logging.AddToLog("Error loading plugin: " + ex.Message, true);
                }
            }
            logging.AddToLog("Done loading plugins", true);
        }

        private void PluginMessageReceived(PacketHeader header, Connection connection, string message)
        {
            string[] arguments = message.Split('|');

            if (arguments[1] == "True")
                OSAEObjectStateManager.ObjectStateSet(arguments[0], "ON", sourceName);
            else if (arguments[1] == "False")
                OSAEObjectStateManager.ObjectStateSet(arguments[0], "OFF", sourceName);

            foreach (Plugin p in plugins)
            {
                if (p.PluginName == arguments[0])
                {
                    OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);

                    if (obj != null)
                    {
                        bool isSystemPlugin = false;
                        foreach (OSAEObjectProperty p2 in obj.Properties)
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
        }

        private void MethodMessageReceived(PacketHeader header, Connection connection, string message)
        {
            string[] items = message.Split('|');

            OSAEMethod method = new OSAEMethod(items[2].Trim(), "", items[0].Trim(), items[3].Trim(), items[4].Trim(), items[5].Trim(), items[1].Trim());

            if (method.ObjectName == "SERVICE-" + Common.ComputerName)
            {
                if (method.MethodName == "RESTART PLUGIN")
                {
                    foreach (Plugin p in plugins)
                    {
                        if (p.PluginName == method.Parameter1)
                        {
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
                    if (plugin.Enabled == true && (method.Owner.ToLower() == plugin.PluginName.ToLower() || method.ObjectName.ToLower() == plugin.PluginName.ToLower()))
                    {
                        plugin.ExecuteCommand(method);
                    }
                }
            }
        }
        
        private void enablePlugin(Plugin plugin)
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);

            OSAEObjectManager.ObjectUpdate(plugin.PluginName, plugin.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 1);
            try
            {
                if (plugin.ActivatePlugin())
                {
                    plugin.RunInterface();
                    OSAEObjectStateManager.ObjectStateSet(plugin.PluginName, "ON", sourceName);
                    logging.AddToLog("Plugin enabled: " + plugin.PluginName, true);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error activating plugin (" + plugin.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
            }           
        }

        private void disablePlugin(Plugin p)
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);

            OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 0);
            try
            {
                p.Shutdown();
                p.Enabled = false;
                p.Domain = Common.CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet, typeof(ClientService));
                
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error stopping plugin (" + p.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
            }
        }

    }    
}
