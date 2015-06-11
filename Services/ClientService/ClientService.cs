//#define OSAESERVICECONTROLLER

namespace OSAE.ClientService
{
    #region Usings

    using OSAE;
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Diagnostics;
    using System.Threading;
    using System.Security;
    using System.Data;
    using NetworkCommsDotNet;
    using log4net.Config;
    using log4net;
    using System.Reflection;
    using System.Windows.Forms;

    #endregion

#if OSAESERVICECONTROLLER
    public partial class ClientService : OSAEServiceBase
#else
    public partial class ClientService : ServiceBase
#endif
    {
        private const string sourceName = "Client Service";
        private OSAEPluginCollection plugins = new OSAEPluginCollection();
        //System.Timers.Timer Clock = new System.Timers.Timer();

        //OSAELog
        private OSAE.General.OSAELog Log;

        static void Main(string[] args)
        {
          //  if (args.Length > 0)
          //  {
                //REPLACE WITH GRAMMAR
















                //string pattern = Common.MatchPattern(args[0],"");
                //this.Log.Info("Processing command: " + args[0] + ", Named Script: " + pattern);
               // if (pattern != string.Empty)
             //   {
             //       OSAEScriptManager.RunPatternScript(pattern, "", "OSACL");
             //   }
          //  }
          //  else
          //  {
                //Debugger.Launch();
#if OSAESERVICECONTROLLER
                if (Environment.UserInteractive)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new OSAEServiceController(new ClientService(), "Client Service Controller"));
                }
                else
                    OSAEServiceBase.Run(new ClientService());
#else
                ServiceBase.Run(new ClientService());
#endif
          //  }
        }

        public ClientService()
        {
            Log = new OSAE.General.OSAELog();

            this.Log.Info("ClientService Starting");

            try
            {
                if (!EventLog.SourceExists("OSAEClient"))
                    EventLog.CreateEventSource("OSAEClient", "Application");
            }
            catch (Exception ex)
            {
                this.Log.Error("CreateEventSource error", ex);
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

            this.Log.Info("OnStart");

            try
            {
                this.Log.Debug("Starting UDP listener");
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Plugin", PluginMessageReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Commmand", MethodMessageReceived);
                //Start listening for incoming UDP data
                UDPConnection.StartListening(true);
                this.Log.Debug("UPD Listener started");
            }
            catch (Exception ex)
            {
                this.Log.Error("Error starting listener", ex);
            }

            Common.CreateComputerObject(sourceName);

            try
            {
                this.Log.Info("Creating Service object");
                OSAEObject svcobj = OSAEObjectManager.GetObjectByName("SERVICE-" + Common.ComputerName);
                if (svcobj == null)
                {
                    OSAEObjectManager.ObjectAdd("SERVICE-" + Common.ComputerName, "SERVICE-" + Common.ComputerName, "SERVICE-" + Common.ComputerName, "SERVICE", "", "SYSTEM", true);
                }
                OSAEObjectStateManager.ObjectStateSet("SERVICE-" + Common.ComputerName, "ON", sourceName);
            }
            catch (Exception ex)
            {
                this.Log.Error("Error creating service object", ex);
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
                this.Log.Error("Error occured during stop, details", ex);
            }

            NetworkComms.Shutdown();
            OSAE.General.OSAELog.FlushBuffers();
        }

        public void LoadPlugins()
        {
            this.Log.Info("Entered LoadPlugins");

            var types = PluginFinder.FindPlugins();

            this.Log.Info("Loading Plugins");

            foreach (var type in types)
            {
                this.Log.Debug("type.TypeName: " + type.TypeName);
                this.Log.Debug("type.AssemblyName: " + type.AssemblyName);

                var domain = Common.CreateSandboxDomain("Sandbox Domain", type.Location, SecurityZone.Internet, typeof(ClientService));

                plugins.Add(new Plugin(type.AssemblyName, type.TypeName, domain, type.Location));
            }

            this.Log.Info("Found " + plugins.Count.ToString() + " plugins");

            foreach (Plugin plugin in plugins)
            {
                try
                {
                    this.Log.Info("---------------------------------------");
                    this.Log.Info("plugin name: " + plugin.PluginName);
                    this.Log.Info("plugin type: " + plugin.PluginType);

                    if (plugin.PluginName != string.Empty)
                    {
                        OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);

                        this.Log.Info("setting found: " + obj.Name + " - " + obj.Enabled.ToString());
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
                        this.Log.Info("isSystemPlugin?: " + isSystemPlugin.ToString());
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
                                    this.Log.Error("Error activating plugin (" + plugin.PluginName + ")", ex);
                                }
                            }
                            else
                            {
                                plugin.Enabled = false;
                            }

                            this.Log.Info("status: " + plugin.Enabled.ToString());
                            this.Log.Info("PluginName: " + plugin.PluginName);
                            this.Log.Info("PluginVersion: " + plugin.PluginVersion);

                            NetworkComms.SendObject("Plugin", Common.WcfServer, 10051, plugin.PluginName + "|" + plugin.Status + "|" + plugin.PluginVersion + "|" + plugin.Enabled);
                        }
                    }
                    else
                    {
                        //add code to create the object.  We need the plugin to specify the type though
                        this.Log.Info("Plugin object doesn't exist");
                        DataSet dataset = OSAESql.RunSQL("SELECT * FROM osae_object_type_property p inner join osae_object_type t on p.object_type_id = t.object_type_id WHERE object_type='" + plugin.PluginType + "' AND property_name='Computer Name'");
                        this.Log.Info("dataset count: " + dataset.Tables[0].Rows.Count.ToString());

                        // if object type has a property called 'Computer Name' we know it is not a System Plugin
                        if (dataset.Tables[0].Rows.Count > 0)
                        {
                            plugin.PluginName = plugin.PluginType + "-" + Common.ComputerName;

                            this.Log.Info("Plugin object does not exist in DB: " + plugin.PluginName);
                            OSAEObjectManager.ObjectAdd(plugin.PluginName, plugin.PluginName, plugin.PluginName, plugin.PluginType, "", "System", false);
                            OSAEObjectPropertyManager.ObjectPropertySet(plugin.PluginName, "Computer Name", Common.ComputerName, "Client Service");

                            this.Log.Info("Plugin added to DB: " + plugin.PluginName);
                            NetworkComms.SendObject("Plugin", Common.WcfServer, 10051, plugin.PluginName + "|" + plugin.Status
                                + "|" + plugin.PluginVersion + "|" + plugin.Enabled);
                        }

                    }
                }
                catch (Exception ex)
                {
                    this.Log.Error("Error loading plugin", ex);
                }
            }
            this.Log.Info("Done loading plugins");
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
                            OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, 1);
                            try
                            {
                                enablePlugin(p);
                                this.Log.Debug("Activated plugin: " + p.PluginName);
                            }
                            catch (Exception ex)
                            {
                                this.Log.Error("Error activating plugin (" + p.PluginName + ")", ex);
                            }
                        }
                        else if (arguments[1] == "False" && p.Enabled && !isSystemPlugin)
                        {
                            OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, 0);
                            try
                            {
                                disablePlugin(p);
                                this.Log.Debug("Deactivated plugin: " + p.PluginName);
                            }
                            catch (Exception ex)
                            {
                                this.Log.Error("Error stopping plugin (" + p.PluginName + ")", ex);
                            }
                        }
                    }
                }
            }
        }

        private void MethodMessageReceived(PacketHeader header, Connection connection, string message)
        {
            string[] items = message.Split('|');

            OSAEMethod method = new OSAEMethod(items[2].Trim(), "", items[0].Trim(), items[3].Trim(), items[4].Trim(), items[5].Trim(), items[1].Trim(), items[6].Trim());

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

            OSAEObjectManager.ObjectUpdate(plugin.PluginName, plugin.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, 1);
            try
            {
                if (plugin.ActivatePlugin())
                {
                    plugin.RunInterface();
                    OSAEObjectStateManager.ObjectStateSet(plugin.PluginName, "ON", sourceName);
                    this.Log.Info("Plugin enabled: " + plugin.PluginName);
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("Error activating plugin (" + plugin.PluginName + ")", ex);
            }
        }

        private void disablePlugin(Plugin p)
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);

            OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, 0);
            try
            {
                p.Shutdown();
                p.Enabled = false;
                p.Domain = Common.CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet, typeof(ClientService));

            }
            catch (Exception ex)
            {
                this.Log.Error("Error stopping plugin (" + p.PluginName + ")", ex);
            }
        }

    }
}
