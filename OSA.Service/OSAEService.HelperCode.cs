namespace OSAE.Service
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Security.Policy;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using MySql.Data.MySqlClient;

    partial class OSAEService
    {
        private void InitialiseOSAInEventLog()
        {
            try
            {
                if (!EventLog.SourceExists("OSAE"))
                    EventLog.CreateEventSource("OSAE", "Application");
            }
            catch (Exception ex)
            {
                logging.AddToLog("CreateEventSource error: " + ex.Message, true);
            }
            this.ServiceName = "OSAE";
            this.EventLog.Source = "OSAE";
            this.EventLog.Log = "Application";
        }

        private static void InitialiseLogFolder()
        {
            FileInfo file = new FileInfo(Common.ApiPath + "/Logs/");
            file.Directory.Create();
            if (OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "Prune Logs").Value == "TRUE")
            {
                string[] files = Directory.GetFiles(Common.ApiPath + "/Logs/");
                foreach (string f in files)
                    File.Delete(f);
            }
        }

        private static void DeleteStoreFiles()
        {
            string[] stores = Directory.GetFiles(Common.ApiPath, "*.store", SearchOption.AllDirectories);
            foreach (string f in stores)
                File.Delete(f);
        }

        private string GetComputerIP()
        {
            IPHostEntry ipEntry = Dns.GetHostByName(Common.ComputerName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[0].ToString();
        }

        private void CreateServiceObject()
        {
            try
            {
                logging.AddToLog("Creating Service object", true);
                OSAEObject svcobj = OSAEObjectManager.GetObjectByName("SERVICE-" + Common.ComputerName);
                if (svcobj == null)
                {
                    OSAEObjectManager.ObjectAdd("SERVICE-" + Common.ComputerName, "SERVICE-" + Common.ComputerName, "SERVICE", "", "SYSTEM", true);
                }
                OSAEObjectStateManager.ObjectStateSet("SERVICE-" + Common.ComputerName, "ON", "OSAE Service");
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error creating service object - " + ex.Message, true);
            }
        }

        private void CreateComputerObject()
        {
            logging.AddToLog("Creating Computer object", true);
            if (OSAEObjectManager.GetObjectByName(Common.ComputerName) == null)
            {

                OSAEObject obj = OSAEObjectManager.GetObjectByAddress(computerIP);
                if (obj == null)
                {
                    OSAEObjectManager.ObjectAdd(Common.ComputerName, Common.ComputerName, "COMPUTER", computerIP, "", true);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else if (obj.Type == "COMPUTER")
                {
                    OSAEObjectManager.ObjectUpdate(obj.Name, Common.ComputerName, obj.Description, "COMPUTER", computerIP, obj.Container, obj.Enabled);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName, "Host Name", Common.ComputerName, sourceName);
                }
                else
                {
                    OSAEObjectManager.ObjectAdd(Common.ComputerName + "." + computerIP, Common.ComputerName, "COMPUTER", computerIP, "", true);
                    OSAEObjectPropertyManager.ObjectPropertySet(Common.ComputerName + "." + computerIP, "Host Name", Common.ComputerName, sourceName);
                }
            }
            else
            {
                OSAEObject obj = OSAEObjectManager.GetObjectByName(Common.ComputerName);
                OSAEObjectManager.ObjectUpdate(obj.Name, obj.Name, obj.Description, "COMPUTER", computerIP, obj.Container, obj.Enabled);
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Host Name", Common.ComputerName, sourceName);
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

        private void RemoveOrphanedMethods()
        {
            logging.AddToLog("Removing orphaned methods", true);

            try
            {
                OSAEMethodManager.ClearMethodQueue();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error clearing method queue details: \r\n" + ex.Message, true);
            }
        }

        private void StartThreads()
        {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void QueryCommandQueue()
        {
            while (running)
            {
                try
                {
                    DataSet dataset = new DataSet();
                    MySqlCommand command = new MySqlCommand();
                    command.CommandText = "SELECT method_queue_id, object_name, address, method_name, parameter_1, parameter_2, object_owner FROM osae_v_method_queue ORDER BY entry_time";
                    dataset = OSAESql.RunQuery(command);

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

                        if (method.ObjectName == "SERVICE-" + Common.ComputerName)
                        {
                            if (method.MethodName == "EXECUTE")
                            {
                                sendMessageToClients("command", method.Parameter1
                                    + " | " + method.Parameter2 + " | " + Common.ComputerName);
                            }
                            else if (method.MethodName == "START PLUGIN")
                            {
                                foreach (Plugin p in plugins)
                                {
                                    if (p.PluginName == method.Parameter1)
                                    {
                                        OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
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
                                        OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
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
                            OSAESql.RunQuery(command);
                        }
                        else
                        {
                            bool processed = false;
                            int methodQueueId = int.Parse(row["method_queue_id"].ToString());

                            foreach (Plugin plugin in plugins)
                            {
                                if (plugin.Enabled == true && (method.Owner.ToLower() == plugin.PluginName.ToLower() || method.ObjectName.ToLower() == plugin.PluginName.ToLower()))
                                {
                                    command.CommandText = "DELETE FROM osae_method_queue WHERE method_queue_id=" + row["method_queue_id"].ToString();
                                    logging.AddToLog("Removing method from queue with ID: " + methodQueueId, false);
                                    OSAEMethodManager.MethodQueueDelete(methodQueueId);
                                    processed = true;
                                    break;
                                }
                            }

                            if (!processed)
                            {
                                sendMessageToClients("method", method.ObjectName + " | " + method.Owner + " | "
                                    + method.MethodName + " | " + method.Parameter1 + " | " + method.Parameter2 + " | "
                                    + method.Address + " | " + row["method_queue_id"].ToString());

                                logging.AddToLog("Removing method from queue with ID: " + methodQueueId, false);
                                OSAEMethodManager.MethodQueueDelete(methodQueueId);
                                processed = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logging.AddToLog("Error in QueryCommandQueue: " + ex.Message, true);
                }
                System.Threading.Thread.Sleep(100);
            }
        }        
    }
}
