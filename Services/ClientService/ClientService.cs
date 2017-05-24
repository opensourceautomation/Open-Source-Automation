//#define OSAESERVICECONTROLLER

namespace OSAE.ClientService
{
    #region Usings

    using OSAE;
    using System;
    using System.ServiceProcess;
    using System.Diagnostics;
    using System.Threading;
    using System.Security;
    using NetworkCommsDotNet;
    using System.Net;
    using log4net.Config;
    using log4net;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Data;

    #endregion

#if OSAESERVICECONTROLLER
    public partial class ClientService : OSAEServiceBase
#else
    public partial class ClientService : ServiceBase
#endif
    {
        private OSAEPluginCollection plugins = new OSAEPluginCollection();
        //System.Timers.Timer Clock = new System.Timers.Timer();
        private OSAE.General.OSAELog Log;
        private string serviceObject = "";

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
            serviceObject = "SERVICE-" + Common.ComputerName;
            Log = new OSAE.General.OSAELog(serviceObject);
            bool found = OSAEObjectManager.ObjectExists(serviceObject);
            if (!found)
                OSAEObjectManager.ObjectAdd(serviceObject, "", "SERVICE", "SERVICE", "", Common.ComputerName, 50, true);
            else
            {
                OSAEObject obj = OSAEObjectManager.GetObjectByName(serviceObject);
                OSAEObjectManager.ObjectUpdate(serviceObject, serviceObject, "", obj.Description, obj.Type, "", Common.ComputerName, obj.MinTrustLevel, obj.Enabled);
            }

            OSAE.OSAEObjectStateManager.ObjectStateSet(serviceObject, "ON", serviceObject);
          
            Log.Info("ClientService Starting");

            try
            {
                if (!EventLog.SourceExists("OSAEClient"))
                    EventLog.CreateEventSource("OSAEClient", "Application");
            }
            catch (Exception ex)
            { Log.Error("CreateEventSource error", ex); }
            ServiceName = "OSAEClient";
            EventLog.Source = "OSAEClient";
            EventLog.Log = "Application";

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            Common.InitialiseLogFolder();
            Log.Info("OnStart");

            try
            {
                Log.Debug("Starting UDP listener");
                string ip = Common.LocalIPAddress();
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Plugin", PluginMessageReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Commmand", MethodMessageReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Method", MethodMessageReceived);
                //Start listening for incoming UDP data
                //UDPConnection.StartListening(new IPEndPoint(IPAddress.Any, 10051));
                TCPConnection.StartListening(new IPEndPoint(IPAddress.Parse(ip), 10051));
                Log.Debug("UPD Listener started");
            }
            catch (Exception ex)
            { Log.Error("Error starting listener", ex); }

            Common.CheckComputerObject(serviceObject);

            Thread loadPluginsThread = new Thread(() => LoadPlugins(serviceObject));
            loadPluginsThread.Start();


          //  Thread loadPluginsThread = new Thread(new ThreadStart(LoadPlugins));
           // loadPluginsThread.Start();

            //Clock.Interval = 5000;
            //Clock.Start();
            //Clock.Elapsed += new System.Timers.ElapsedEventHandler(checkConnection);
            Log.Info("OnStart Completed");
            OSAEObjectStateManager.ObjectStateSet(serviceObject, "ON", serviceObject);
        }

        protected override void OnStop()
        {
            try
            {
                foreach (Plugin p in plugins)
                    if (p.Enabled) p.Shutdown();
            }
            catch (Exception ex)
            { Log.Error("Error occured during stop, details", ex); }

            NetworkComms.Shutdown();
            OSAEObjectStateManager.ObjectStateSet(serviceObject,"OFF", serviceObject);
            OSAE.General.OSAELog.FlushBuffers();
        }

        public void LoadPlugins(string name)
        {
            //Log.Info("Entered LoadPlugins");
            var types = PluginFinder.FindPlugins();
            Log.Info("Loading Plugins");

            foreach (var type in types)
            {
                Log.Debug("type.TypeName: " + type.TypeName);
                Log.Debug("type.AssemblyName: " + type.AssemblyName);

                var domain = Common.CreateSandboxDomain("Sandbox Domain", type.Location, SecurityZone.Internet, typeof(ClientService));

                plugins.Add(new Plugin(type.AssemblyName, type.TypeName, domain, type.Location, Common.ComputerName));
            }

            Log.Info("Found " + plugins.Count.ToString() + " plugins");

            foreach (Plugin plugin in plugins)
            {
                try
                {
                    Log.Info("---------------------------------------");
                    Log.Info("plugin name: " + plugin.PluginName);
                    Log.Info("plugin type: " + plugin.PluginType);

                    if (plugin.PluginName != "")
                    {
                        OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);

                        if (obj == null)
                        {
                            OSAEObjectManager.ObjectAdd(plugin.PluginName, "", plugin.PluginName + " plugin's Object", plugin.PluginType, "", name, 50, true);
                            Log.Info(obj.Name + ":  Plugin Object Not found.  Plugin Object Created.");
                            obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);
                            if (obj == null) Log.Info(obj.Name + ":  I failed to create the Plugin Object!");
                        }

                        if (obj != null)
                        {
                            Log.Info("Plugin Object found: " + plugin.PluginName);
                            bool isSystemPlugin = false;
                            foreach (OSAEObjectProperty p in obj.Properties)
                            {
                                if (p.Name == "System Plugin")
                                {
                                    if (p.Value == "TRUE") isSystemPlugin = true;
                                    break;
                                }
                            }
                            Log.Info("isSystemPlugin?: " + isSystemPlugin.ToString());
                            if (!isSystemPlugin)
                            {
                                if (obj.Enabled == true)
                                {
                                    try
                                    {
                                        startPlugin(plugin);
                                    }
                                    catch (Exception ex)
                                    { Log.Error("Error activating plugin (" + plugin.PluginName + ")", ex); }
                                }
                                else
                                    plugin.Enabled = false;

                                Log.Info("status: " + plugin.Enabled.ToString());
                                Log.Info("PluginName: " + plugin.PluginName);
                                Log.Info("PluginVersion: " + plugin.PluginVersion);
                                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Version", plugin.PluginVersion, name);
                                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Author", plugin.PluginAuthor, name);

                                //NetworkComms.SendObject("Plugin", Common.WcfServer, 10051, plugin.PluginName + "|" + plugin.Status + "|" + plugin.PluginVersion + "|" + plugin.Enabled);
                                UDPConnection.SendObject("Plugin", plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Stopped | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10051));
                            }
                        }
                    }
                    else
                    {
                        Log.Info("Plugin object does not exist in DB: " + plugin.PluginName);
                        OSAEObjectManager.ObjectAdd(plugin.PluginName, "", plugin.PluginName, plugin.PluginType, "", "System",50, true);
                        Log.Info("Plugin added to DB: " + plugin.PluginName);
                        UDPConnection.SendObject("Plugin", plugin.PluginName + " | ON | " + plugin.PluginVersion + " | Started | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10051));
                    }
                }
                catch (Exception ex)
                { Log.Error("Error loading plugin!", ex); }
            }
            Log.Info("Done loading plugins");
        }

        private void PluginMessageReceived(PacketHeader header, Connection connection, string message)
        {
            Log.Info("<- Plugin Message: " + message);
            string[] arguments = message.Split('|');

            foreach (Plugin p in plugins)
            {
                if (p.PluginName == arguments[0].Trim())
                {
                    Log.Info("Found Plugin: " + arguments[0].Trim() + " on " + serviceObject);
                    OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
                    if (obj != null)
                    {
                        Log.Info("Found Plugin Object for: " + arguments[0].Trim() + " on " + serviceObject);
                        bool isSystemPlugin = false;
                        foreach (OSAEObjectProperty p2 in obj.Properties)
                        {
                            if (p2.Name == "System Plugin")
                            {
                                if (p2.Value == "TRUE") isSystemPlugin = true;
                                break;
                            }
                        }
                        Log.Info("Plugin " + arguments[0].Trim() + " SystemPlugin = " + isSystemPlugin);
                        if (arguments[1].Trim() == "ON" && !isSystemPlugin)
                        {
                            //We are only stoppeng and starting, leave enabled alone
                            //OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.MinTrustLevel, true);
                            try
                            {
                                startPlugin(p);
                                Log.Debug("Started plugin: " + p.PluginName);
                            }
                            catch (Exception ex)
                            { Log.Error("Error Starting plugin (" + p.PluginName + ")", ex); }
                        }
                        else if (arguments[1].Trim() == "OFF" && !isSystemPlugin)
                        {
                            //OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.MinTrustLevel, false);
                            try
                            {
                                stopPlugin(p);
                                Log.Debug("Stopped plugin: " + p.PluginName);
                            }
                            catch (Exception ex)
                            { Log.Error("Error stopping plugin (" + p.PluginName + ")", ex); }
                        }
                    }
                }
            }
        }

        private void MethodMessageReceived(PacketHeader header, Connection connection, string message)
        {
            Log.Info("<- Command: " + message);
            string[] items = message.Split('|');
            //ObjectName + " | " + MethodName + " | " + Parameter1 + " | " + Parameter2 + " | " + Address + " | " + Owner + " | " +From, new IPEndPoint(IPAddress.Broadcast, 10051));

            OSAEMethod method = new OSAEMethod(items[0].Trim(), items[1].Trim(), "", items[2].Trim(), items[3].Trim(), items[4].Trim(), items[5].Trim(), items[6].Trim());
            Log.Info("Created Method object." + method.ObjectName);
            if (method.ObjectName == "SERVICE-" + Common.ComputerName)
            {
                if (method.MethodName == "RESTART PLUGIN")
                {
                    foreach (Plugin p in plugins)
                    {
                        if (p.PluginName == method.Parameter1)
                        {
                            bool found = OSAEObjectManager.ObjectExists(p.PluginName);
                            if (found)
                            {
                                stopPlugin(p);
                                startPlugin(p);
                            }
                        }
                    }
                }
                else if (method.MethodName == "START PLUGIN")
                {
                    foreach (Plugin p in plugins)
                    {
                        if (p.PluginName == method.Parameter1)
                        {
                            bool found = OSAEObjectManager.ObjectExists(p.PluginName);
                            if (found) startPlugin(p);
                        }
                    }
                }
                else if (method.MethodName == "STOP PLUGIN")
                {
                    foreach (Plugin p in plugins)
                    {
                        if (p.PluginName == method.Parameter1)
                        {
                            bool found = OSAEObjectManager.ObjectExists(p.PluginName);
                            if (found) stopPlugin(p);
                        }
                    }
                }
            }
            else
            {
                Log.Info("Passing Method to: " + method.ObjectName);
                foreach (Plugin plugin in plugins)
                {
                    //This exposes a flaw in the Ownership of distributed plugins.   
                    Log.Info("does " + method.ObjectName + " = " + plugin.PluginName);
                    if (plugin.Running && method.ObjectName == plugin.PluginName)
                    {
                        plugin.ExecuteCommand(method);
                        Log.Info("Passed Method to: " + method.ObjectName);
                    }
                }
            }
        }

        private void startPlugin(Plugin plugin)
        {
            //OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);
            //OSAEObjectManager.ObjectUpdate(plugin.PluginName, plugin.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.MinTrustLevel, true);
            try
            {
                if (plugin.ActivatePlugin())
                {
                    plugin.RunInterface(serviceObject);
                    OSAEObjectStateManager.ObjectStateSet(plugin.PluginName, "ON", serviceObject);
                    Log.Info("Plugin started: " + plugin.PluginName);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error starting plugin (" + plugin.PluginName + ")", ex);
                plugin.Running = false;
            }
        }

        private void stopPlugin(Plugin p)
        {
            //OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
            //OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.MinTrustLevel, false);
            try
            {
                p.Shutdown();
                p.Running = false;
               // p.Enabled = false;
                p.Domain = Common.CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet, typeof(ClientService));
                OSAEObjectStateManager.ObjectStateSet(p.PluginName, "OFF", serviceObject);

            }
            catch (Exception ex)
            { Log.Error("Error stopping plugin (" + p.PluginName + ")", ex); }
        }
    }
}
