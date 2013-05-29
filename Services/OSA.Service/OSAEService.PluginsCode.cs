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
    using MySql.Data.MySqlClient;

    partial class OSAEService
    {
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
                    sendMessageToClients(WCF.OSAEWCFMessageType.PLUGIN, plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Running | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName);
                    logging.AddToLog("Plugin enabled: " + plugin.PluginName, true);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error activating plugin (" + plugin.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
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
                p.Domain = Common.CreateSandboxDomain("Sandbox Domain", p.Location, SecurityZone.Internet, typeof(OSAEService));                
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error stopping plugin (" + p.PluginName + "): " + ex.Message + " - " + ex.InnerException, true);
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
            OSAEPluginCollection newPlugins = new OSAEPluginCollection();
            var pluginAssemblies = new List<OSAEPluginBase>();
            var types = PluginFinder.FindPlugins();

            logging.AddToLog("Loading Plugins", true);

            foreach (var type in types)
            {
                logging.AddToLog("type.TypeName: " + type.TypeName, false);
                logging.AddToLog("type.AssemblyName: " + type.AssemblyName, false);

                var domain = Common.CreateSandboxDomain("Sandbox Domain", type.Location, SecurityZone.Internet, typeof(OSAEService));
                domain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledPluginExceptions);

                Plugin p = new Plugin(type.AssemblyName, type.TypeName, domain, type.Location);
                if (!pluginLoaded(p.PluginType))
                {
                    newPlugins.Add(p);
                }
            }

            logging.AddToLog("Found " + plugins.Count.ToString() + " plugins", true);
            MySqlConnection connection = new MySqlConnection(Common.ConnectionString);

            foreach (Plugin plugin in newPlugins)
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
                            OSAEObject obj = OSAEObjectManager.GetObjectByName(plugin.PluginName);

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

                            command.Connection = connection;
                            command.CommandText = "SELECT * FROM osae_object_type_property p inner join osae_object_type t on p.object_type_id = t.object_type_id WHERE object_type=@type AND property_name='Computer Name'";
                            command.Parameters.AddWithValue("@type", plugin.PluginType);
                            adapter = new MySqlDataAdapter(command);
                            adapter.Fill(dataset);

                            if (dataset.Tables[0].Rows.Count > 0)
                            {
                                plugin.PluginName = plugin.PluginType + "-" + Common.ComputerName;
                            }
                            else
                            {
                                plugin.PluginName = plugin.PluginType;
                            }

                            logging.AddToLog("Plugin object does not exist in DB: " + plugin.PluginName, true);
                            OSAEObjectManager.ObjectAdd(plugin.PluginName, plugin.PluginName, plugin.PluginType, "", "System", false);
                            OSAEObjectPropertyManager.ObjectPropertySet(plugin.PluginName, "Computer Name", Common.ComputerName, sourceName);

                            logging.AddToLog("Plugin added to DB: " + plugin.PluginName, true);
                            sendMessageToClients(WCF.OSAEWCFMessageType.PLUGIN, plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Stopped | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName);

                        }
                        plugins.Add(plugin);
                        masterPlugins.Add(plugin);
                    }
                }
                catch (Exception ex)
                {
                    logging.AddToLog("Error loading plugin: " + ex.Message, true);
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
            logging.AddToLog("Unhandled Plugin Exceptions : " + e.Message + " - InnerException: " + e.InnerException.Message, true);
        }
    }
}
