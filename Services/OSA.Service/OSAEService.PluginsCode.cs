namespace OSAE.Service
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security;
    using NetworkCommsDotNet;
    using MySql.Data.MySqlClient;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Xml.Linq;

    partial class OSAEService
    {
        //private string serviceName;
        public void startPlugin(string serviceName,Plugin plugin)
        {
            Log = new General.OSAELog(serviceName);
            Log.Info(plugin.PluginName + ":  Starting Plugin...");

            OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);
            //Enabe and start seemed to be mixed, changed to start, enabe is handled at the Webui for now, maybe a method in the future
            //OSAEObjectManager.ObjectUpdate(plugin.PluginName, plugin.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.MinTrustLevel, true);
            try
            {
                if (plugin.ActivatePlugin())
                {
                   // plugin.Enabled = true;
                    plugin.RunInterface(serviceObject);
                    OSAEObjectStateManager.ObjectStateSet(plugin.PluginName, "ON", "SYSTEM");
                    Log.Debug(plugin.PluginName + ":  Plugin started.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error activating plugin (" + plugin.PluginName + "): " + ex.Message, ex);
                OSAEObjectStateManager.ObjectStateSet(plugin.PluginName, "OFF", "SYSTEM");
            }           
        }

        public void stopPlugin(string serviceName, Plugin p)
        {
            Log = new General.OSAELog("SERVICE");
            Log.Info(p.PluginName + ":  Disabling Plugin...");

            OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
          //  OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.MinTrustLevel, false);
            try
            {
                p.Shutdown();
                OSAEObjectStateManager.ObjectStateSet(p.PluginName, "OFF", "SYSTEM");
                p.Domain = Common.CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet, typeof(OSAEService));                
            }
            catch (Exception ex)
            { Log.Error("Error stopping plugin (" + p.PluginName + "): " + ex.Message, ex); }
        }

        public bool pluginExist(string name)
        {
            foreach (Plugin p in plugins)
            {
                if (p.PluginType == name) return true;
            }
            return false;
        }

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

        /// <summary>
        /// 
        /// </summary>
        public void LoadPlugins(string serviceName)
        {
            Log = new General.OSAELog(serviceName);
            Log.Info("Loading Plugins...");
            
            OSAEPluginCollection newPlugins = new OSAEPluginCollection();
            var pluginAssemblies = new List<OSAEPluginBase>();
            var types = PluginFinder.FindPlugins();

            foreach (var type in types)
            {
                Log.Debug("TypeName: " + type.TypeName + ", AssemblyName: " + type.AssemblyName);

                var domain = Common.CreateSandboxDomain("Sandbox Domain", type.Location, SecurityZone.Internet, typeof(OSAEService));
                domain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledPluginExceptions);

                Plugin p = new Plugin(type.AssemblyName, type.TypeName, domain, type.Location);
                if (!pluginLoaded(p.PluginType)) newPlugins.Add(p); 
            }

            Log.Info("Found " + newPlugins.Count.ToString() + " Assemblies");
            MySqlConnection connection = new MySqlConnection(Common.ConnectionString);

            foreach (Plugin plugin in newPlugins)
            {
                try
                {
                    Log.Info("----------------------------------------------------");
                    if (plugin.PluginName != "")
                    {
                        OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);
                        if (obj == null)
                        {
                            bool found = OSAEObjectTypeManager.ObjectTypeExists(plugin.PluginType);
                            if (found)
                            {
                                OSAEObjectManager.ObjectAdd(plugin.PluginName, "", plugin.PluginName + " plugin's Object", plugin.PluginType, "", serviceName, 50, false);
                                Log.Info(obj.Name + ":  Plugin Object Not found.  Plugin Object Created.");
                                obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);
                                if (obj == null) Log.Info(obj.Name + ":  I failed to create the Plugin Object!");
                            }
                            else
                                Log.Info(":  Plugin Object Type Not found for: " + plugin.PluginType + ".  Plugin Object Cannot be Created.");
                        }

                        if (obj != null)
                        {
                            Log.Info(obj.Name + ":  Plugin Object found.  Plugin Object Enabled = " + obj.Enabled.ToString());
                            //No idea why the following line would run
                            //OSAEObjectManager.ObjectUpdate(plugin.PluginName,plugin.PluginName, "", plugin.PluginName + " plugin's Object", plugin.PluginType, "", serviceName, 50, true);
                            if (obj.Enabled == true)
                            {
                                plugin.Enabled = true;
                                startPlugin(serviceObject, plugin);
                            }
                            else
                            {
                                plugin.Enabled = false;
                                OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", serviceObject);
                            }

                            Log.Info(obj.Name + ":  Plugin Enabled = " + plugin.Enabled.ToString());
                            Log.Info(obj.Name + ":  Plugin Version = " + plugin.PluginVersion);
                            OSAEObjectManager.ObjectUpdate(plugin.PluginName, plugin.PluginName, "", plugin.PluginName + " plugin's Object", plugin.PluginType, obj.Address, serviceName, 50, plugin.Enabled);
                            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Version", plugin.PluginVersion, serviceName);
                            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Author", plugin.PluginAuthor, serviceName);
                        }
                    }
                    else
                    {
                        bool found = OSAEObjectTypeManager.ObjectTypeExists(plugin.PluginType);
                        if (found)
                        {
                            plugin.PluginName = plugin.PluginType;
                            Log.Info(plugin.PluginName + ":  Plugin object does not exist in DB!");
                            OSAEObjectManager.ObjectAdd(plugin.PluginName, "", plugin.PluginName, plugin.PluginType, "", "System", 50, false);
                            OSAEObjectPropertyManager.ObjectPropertySet(plugin.PluginName, "Version", plugin.PluginVersion, serviceName);
                            OSAEObjectPropertyManager.ObjectPropertySet(plugin.PluginName, "Author", plugin.PluginAuthor, serviceName);
                            Log.Info(plugin.PluginName + ":  Plugin added to DB.");
                            //Uh, this still looks wrong below.   I don't think it is needed, besides, any new plugin is disabled...
                            //UDPConnection.SendObject("Plugin", plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Stopped | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10051));
                        }
                        else
                            Log.Info(":  Plugin Object Type Not found for: " + plugin.PluginType + ".  Plugin Object Cannot be Created!");
                    }
                    plugins.Add(plugin);
                    masterPlugins.Add(plugin);
                }
                catch (Exception ex)
                { Log.Error("Error loading plugin: " + ex.Message, ex); }
            }
            OSAEObjectStateManager.ObjectStateSet(serviceObject, "ON", serviceObject);
        }

        private bool pluginLoaded(string type)
        {
            foreach (Plugin p in plugins)
            {
                if (p.PluginType == type) return true;
            }
            return false;
        }

        void UnhandledPluginExceptions(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Log.Error("Unhandled Plugin Exceptions : " + e.Message, e);
        }
    }
}
