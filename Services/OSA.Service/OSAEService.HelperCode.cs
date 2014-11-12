namespace OSAE.Service
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.ServiceModel;
    using System.Threading;
    using System.Timers;
    using System.Net;
    using NetworkCommsDotNet;

    partial class OSAEService
    {
        /// <summary>
        /// Check if there is a OSA message container in the Event log and create one if not.
        /// </summary>

        private void InitialiseOSAInEventLog()
        {
            this.Log.Debug("Initializing Event Log");
            
            try
            {
                if (!EventLog.SourceExists("OSAE"))
                    EventLog.CreateEventSource("OSAE", "Application");

                this.Log.Debug("EventLog Source Ensured.");
            }
            catch (Exception ex)
            {
                this.Log.Error("CreateEventSource error: " + ex.Message, ex);
            }
            this.ServiceName = "OSAE";
            this.EventLog.Source = "OSAE";
            this.EventLog.Log = "Application";
        }        

        private void DeleteStoreFiles()
        {
            this.Log.Debug("Deleting Store Files");

            string[] stores = Directory.GetFiles(Common.ApiPath, "*.store", SearchOption.AllDirectories);
            this.Log.Debug(stores.Length + " stores to delete.");

            foreach (string f in stores)
                File.Delete(f);

        }       

        /// <summary>
        /// Check if there is an object for the Service running on this machine in OSA, and create one if not.
        /// </summary>
        private void CreateServiceObject()
        {
            this.Log.Debug("Creating Service Object");

            try
            {
                OSAEObject svcobj = OSAEObjectManager.GetObjectByName("SERVICE-" + Common.ComputerName);
                if (svcobj == null)
                {
                    OSAEObjectManager.ObjectAdd("SERVICE-" + Common.ComputerName, "SERVICE-" + Common.ComputerName, "SERVICE", "", "SYSTEM", true);
                }
                OSAEObjectStateManager.ObjectStateSet("SERVICE-" + Common.ComputerName, "ON", "OSAE Service");
            }
            catch (Exception ex)
            {
                this.Log.Error("Error creating service object: " + ex.Message, ex);
            }
        }                     

        /// <summary>
        /// Starts the various OSA threads that monitors the command Queue, and monitors plugins
        /// </summary>
        private void StartThreads()
        {
            this.Log.Debug("Starting Threads");
            Thread QueryCommandQueueThread = new Thread(new ThreadStart(QueryCommandQueue));
            QueryCommandQueueThread.Start();

            Thread loadPluginsThread = new Thread(new ThreadStart(LoadPlugins));
            loadPluginsThread.Start();

            //checkPlugins.Interval = 60000;
            //checkPlugins.Enabled = true;
            //checkPlugins.Elapsed += new ElapsedEventHandler(checkPlugins_tick);
        }

        /// <summary>
        /// Stops all the plugins
        /// </summary>
        private void ShutDownSystems()
        {             
            this.Log.Info("Stopping...");

            try
            {                          
                //checkPlugins.Enabled = false;
                running = false;
                
                this.Log.Debug("Shutting down plugins");
                foreach (Plugin p in plugins)
                {                           
                    if (p.Enabled)
                    {
                        this.Log.Debug("Shutting down plugin: " + p.PluginName);
                        p.Shutdown();
                    }
                }

            }
            catch { }
        }

        /// <summary>
        /// periodically checks the command queue to see if there is any commands that need to be processed by plugins
        /// </summary>        
        private void QueryCommandQueue()
        {
            this.Log.Debug("QueryCommandQueue");


            while (running)
            {
                try
                {
                    foreach (OSAEMethod method in OSAEMethodManager.GetMethodsInQueue())
                    {
                        this.Log.Debug("Method in Queue, ObjectName: " + method.ObjectName + " MethodLabel: " + method.MethodLabel + " MethodName: " + method.MethodName);

                        LogMethodInformation(method);

                        if (method.ObjectName == "SERVICE-" + Common.ComputerName)
                        {
                            switch (method.MethodName)
                            {
                                case "EXECUTE" : 
                                    this.Log.Info("Recieved Execute Method Name");
                                    UDPConnection.SendObject("Command", method.Parameter1 + " | " + method.Parameter2 + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10051));
                                    break;
                                case "START PLUGIN":
                                    StartPlugin(method);
                                    break;
                                case "STOP PLUGIN":
                                    StopPlugin(method);
                                    break;
                                case "RELOAD PLUGINS":
                                    LoadPlugins();
                                    break;
                            }                                                                            

                            OSAEMethodManager.MethodQueueDelete(method.Id);
                        }
                        else if (method.ObjectName.Split('-')[0] == "SERVICE")
                        {
                            this.Log.Debug("Method for client service.  Sending Broadcast.");
                            if(method.MethodName == "EXECUTE")
                                UDPConnection.SendObject("Command", method.Parameter1 + " | " + method.Parameter2 + " | " + method.ObjectName.Substring(8), new IPEndPoint(IPAddress.Broadcast, 10051));

                            OSAEMethodManager.MethodQueueDelete(method.Id);
                        }
                        else
                        {
                            bool processed = false;                            

                            foreach (Plugin plugin in plugins)
                            {
                                if (plugin.Enabled == true && (method.Owner.ToLower() == plugin.PluginName.ToLower() || method.ObjectName.ToLower() == plugin.PluginName.ToLower()))
                                {
                                    this.Log.Debug("Removing method from queue with ID: " + method.Id);
                                    plugin.ExecuteCommand(method);
                                    processed = true;
                                    break;
                                }
                            }

                            if (!processed)
                            {
                                this.Log.Debug("Method found for client service plugin.  Sending Broadcast.");
                                UDPConnection.SendObject("Method", method.ObjectName + " | " + method.Owner + " | "
                                    + method.MethodName + " | " + method.Parameter1 + " | " + method.Parameter2 + " | "
                                    + method.Address + " | " + method.Id, new IPEndPoint(IPAddress.Broadcast, 10051));

                                this.Log.Debug("Removing method from queue with ID: " + method.Id);
                            }

                            OSAEMethodManager.MethodQueueDelete(method.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Log.Error("Error in QueryCommandQueue: " + ex.Message, ex);
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Logs information about a method found in the Queue
        /// </summary>
        /// <param name="method">The method to log</param>
        private void LogMethodInformation(OSAEMethod method)
        {
            this.Log.Debug("Found method in queue: " + method.MethodName);
            this.Log.Debug("-- object name: " + method.ObjectName);
            this.Log.Debug("-- param 1: " + method.Parameter1);
            this.Log.Debug("-- param 2: " + method.Parameter2);
            this.Log.Debug("-- object owner: " + method.Owner);
        }

        /// <summary>
        /// Stops a plugin based on a method
        /// </summary>
        /// <param name="method">The method containing the information of the plugin to stop</param>
        private void StopPlugin(OSAEMethod method)
        {
            foreach (Plugin p in plugins)
            {
                if (p.PluginName == method.Parameter1)
                {
                    OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
                    if (obj != null)
                    {
                        disablePlugin(p);
                        UDPConnection.SendObject("Plugin", p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | Stopped | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10051));
                    }
                }
            }
        }

        /// <summary>
        /// Starts a plugin based on a method
        /// </summary>
        /// <param name="method">The method containing the information of the plugin to stop</param>
        private void StartPlugin(OSAEMethod method)
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
    }
}
