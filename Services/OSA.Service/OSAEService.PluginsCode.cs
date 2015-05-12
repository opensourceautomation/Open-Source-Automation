namespace OSAE.Service
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Threading;
    using System.Xml.Linq;
    using NetworkCommsDotNet;
    using MySql.Data.MySqlClient;

    partial class OSAEService
    {
        public void enablePlugin(Plugin plugin)
        {
            this.Log.Info(plugin.PluginName + ":  Enabling Plugin...");

            OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);

            OSAEObjectManager.ObjectUpdate(plugin.PluginName, plugin.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 1);
            try
            {
                if (plugin.ActivatePlugin())
                {
                    plugin.Enabled = true;
                    plugin.RunInterface();
                    OSAEObjectStateManager.ObjectStateSet(plugin.PluginName, "ON", sourceName);
                    this.Log.Debug(plugin.PluginName + ":  Plugin enabled.");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("Error activating plugin (" + plugin.PluginName + "): " + ex.Message, ex);
            }           
        }

        public void disablePlugin(Plugin p)
        {
            this.Log.Info(p.PluginName + ":  Disabling Plugin...");

            OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
            OSAEObjectManager.ObjectUpdate(p.PluginName, p.PluginName, obj.Description, obj.Type, obj.Address, obj.Container, 0);
            try
            {
                p.Shutdown();
                p.Enabled = false;
                p.Domain = Common.CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet, typeof(OSAEService));                
            }
            catch (Exception ex)
            {
                this.Log.Error("Error stopping plugin (" + p.PluginName + "): " + ex.Message, ex);
            }
        }

        public bool pluginExist(string name)
        {
            foreach (Plugin p in plugins)
            {
                if (p.PluginType == name)
                {
                    return true;
                }
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
        public void LoadPlugins()
        {
            this.Log.Info("Loading Plugins...");
            
            OSAEPluginCollection newPlugins = new OSAEPluginCollection();
            var pluginAssemblies = new List<OSAEPluginBase>();
            var types = PluginFinder.FindPlugins();

            foreach (var type in types)
            {
                this.Log.Debug("type.TypeName: " + type.TypeName);
                this.Log.Debug("type.AssemblyName: " + type.AssemblyName);

                var domain = Common.CreateSandboxDomain("Sandbox Domain", type.Location, SecurityZone.Internet, typeof(OSAEService));
                domain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledPluginExceptions);

                Plugin p = new Plugin(type.AssemblyName, type.TypeName, domain, type.Location);
                if (!pluginLoaded(p.PluginType))
                {
                    newPlugins.Add(p); 
                }
            }

            this.Log.Info("Found " + newPlugins.Count.ToString() + " Assemblies");
            MySqlConnection connection = new MySqlConnection(Common.ConnectionString);

            foreach (Plugin plugin in newPlugins)
            {
                try
                {
                    if (plugin.PluginName != "")
                    {
                        this.Log.Info("----------------------------------------------------");
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
                        this.Log.Info(plugin.PluginName + ":  Connection Passed (" + goodConnection + ")");
                        if (goodConnection)
                        {
                            if (plugin.PluginName != "")
                            {
                                OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);

                                if (obj == null)
                                {
                                    OSAEObjectManager.ObjectAdd(plugin.PluginName, plugin.PluginName + " plugin's Object", plugin.PluginType, "", "", true);
                                    obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);
                                }

                                if (obj != null)
                                {
                                    this.Log.Info(obj.Name + ":  Plugin Object found.  Plugin Object Enabled = " + obj.Enabled.ToString());
                                    if (obj.Enabled == 1)
                                    {
                                        enablePlugin(plugin);
                                    }
                                    else
                                        plugin.Enabled = false;

                                    this.Log.Info(obj.Name + ":  Plugin Enabled =  " + plugin.Enabled.ToString());
                                    this.Log.Info(obj.Name + ":  Plugin Version = " + plugin.PluginVersion);
                                }
                            }
                            else
                            {
                                //add code to create the object.  We need the plugin to specify the type though

                                MySqlDataAdapter adapter;
                                DataSet dataset = new DataSet();
                                DataSet dataset2 = new DataSet();
                                MySqlCommand command = new MySqlCommand();

                                command.Connection = connection;
                                command.CommandText = "SELECT * FROM osae_object_type_property p inner join osae_object_type t on p.object_type_id = t.object_type_id WHERE object_type=@type AND property_name='Computer Name'";
                                command.Parameters.AddWithValue("@type", plugin.PluginType);
                                adapter = new MySqlDataAdapter(command);
                                adapter.Fill(dataset);

                                command.CommandText = "SELECT * FROM osae_v_object WHERE object_type=@type";
                                command.Parameters.AddWithValue("@type", plugin.PluginType);
                                adapter = new MySqlDataAdapter(command);
                                adapter.Fill(dataset2);

                                if (dataset.Tables[0].Rows.Count > 0 && dataset2.Tables[0].Rows.Count > 0)
                                {
                                    plugin.PluginName = plugin.PluginType + "-" + Common.ComputerName;
                                }
                                else
                                {
                                    plugin.PluginName = plugin.PluginType;
                                }

                                this.Log.Info(plugin.PluginName + ":  Plugin object does not exist in DB!");
                                OSAEObjectManager.ObjectAdd(plugin.PluginName, plugin.PluginName, plugin.PluginType, "", "System", false);
                                OSAEObjectPropertyManager.ObjectPropertySet(plugin.PluginName, "Computer Name", Common.ComputerName, sourceName);
                                this.Log.Info(plugin.PluginName + ":  Plugin added to DB.");
                                UDPConnection.SendObject("Plugin", plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Stopped | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10051));
                            }
                            plugins.Add(plugin);
                            masterPlugins.Add(plugin);
                        }
                    }
                    else
                    {
                        this.Log.Info(plugin.PluginType + " Skipped! (Not Loaded due to missing Object or other issue)");
                    }
                }
                catch (Exception ex)
                {
                    this.Log.Error("Error loading plugin: " + ex.Message, ex);
                }
            }
        }

        private bool pluginLoaded(string type)
        {
            foreach (Plugin p in plugins)
            {
                if (p.PluginType == type)
                    return true;
            }
            return false;
        }

        void UnhandledPluginExceptions(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            this.Log.Error("Unhandled Plugin Exceptions : " + e.Message, e);
        }
    }
}
