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
                    sendMessageToClients("plugin", plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Running | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName);
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

                XElement xml = XElement.Load(responseStream);
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
                    string msg = p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | " + p.Status + " | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName;
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
                catch(Exception ex)
                {
                    logging.AddToLog("Error occurred checking for update, details: " + ex.Message, true);
                }
            }

            try
            {
                checkForUpdates("Service", OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "DB Version").Value);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error occurred checking for update, details: " + ex.Message, true);
            }      
        }

        #endregion    
   
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

                var domain = Common.CreateSandboxDomain("Sandbox Domain", type.Location, SecurityZone.Internet, typeof(OSAEService));

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
                            sendMessageToClients("plugin", plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Stopped | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName);

                        }
                        masterPlugins.Add(plugin);
                    }
                }
                catch (Exception ex)
                {
                    logging.AddToLog("Error loading plugin: " + ex.Message, true);
                }
            }
        }       
    }
}
