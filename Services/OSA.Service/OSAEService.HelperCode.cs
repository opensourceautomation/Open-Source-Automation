namespace OSAE.Service
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Net;
    using NetworkCommsDotNet;

    partial class OSAEService
    {
        /// <summary>
        /// Check if there is a OSA message container in the Event log and create one if not.
        /// </summary>

        private void InitialiseOSAInEventLog()
        {
            Log.Debug("Initializing Event Log");
            
            try
            {
                if (!EventLog.SourceExists("OSAE"))  EventLog.CreateEventSource("OSAE", "Application");
                Log.Debug("EventLog Source Ensured.");
            }
            catch (Exception ex)
            { Log.Error("CreateEventSource error: " + ex.Message, ex); }

            ServiceName = "OSAE";
            EventLog.Source = "OSAE";
            EventLog.Log = "Application";
        }        

        private void DeleteStoreFiles()
        {
            Log.Debug("Deleting Store Files");

            string[] stores = Directory.GetFiles(Common.ApiPath, "*.store", SearchOption.AllDirectories);
            Log.Debug(stores.Length + " stores to delete.");

            foreach (string f in stores)
                File.Delete(f);
        }       

        /// <summary>
        /// Check if there is an object for the Service running on this machine in OSA, and create one if not.
        /// </summary>
        private string CheckServiceObject()
        {
            try
            {
                bool found = OSAEObjectManager.ObjectExists("SERVICE-" + Common.ComputerName);
                if (!found)
                {
                    OSAEObjectManager.ObjectAdd("SERVICE-" + Common.ComputerName, "", "SERVICE", "SERVICE", "", "SYSTEM", 50, true);
                    //Log.Debug("Created Service Object called " + "SERVICE-" + Common.ComputerName);
                }
                //else
                   //Log.Debug("Found Service Object called " + "SERVICE-" + Common.ComputerName);

                return "SERVICE-" + Common.ComputerName;

                //  OSAEObjectStateManager.ObjectStateSet("SERVICE-" + Common.ComputerName, "ON", "OSAE Service");   This is some kind of hack
            }
            catch (Exception ex)
            {
                //Log.Error("Error creating service object!", ex);
                return null;
            }
        }                     

        /// <summary>
        /// Starts the various OSA threads that monitors the command Queue, and monitors plugins
        /// </summary>
        private void StartThreads(string serviceName)
        {
            Log.Debug("Starting Threads");
            Thread QueryCommandQueueThread = new Thread(new ThreadStart(QueryCommandQueue));
            QueryCommandQueueThread.Start();

         //   Thread loadPluginsThread = new Thread(new ParameterizedThreadStart(LoadPlugins));
            Thread loadPluginsThread = new Thread(() => LoadPlugins(serviceObject));
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
            Log.Info("Stopping...");

            try
            {                          
                //checkPlugins.Enabled = false;
                running = false;
                
                Log.Debug("Shutting down plugins");
                foreach (Plugin p in plugins)
                {                           
                    if (p.Running)
                    {
                        Log.Debug("Shutting down plugin: " + p.PluginName);
                        p.Shutdown();
                        OSAEObjectStateManager.ObjectStateSet(p.PluginName, "OFF", serviceObject);
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
            while (running)
            {
                try
                {
                    foreach (OSAEMethod method in OSAEMethodManager.GetMethodsInQueue())
                    {
                        Log.Debug("Method in queue for: " + method.Owner + " Method: " + method.ObjectName + "." + method.MethodName + "," + method.Parameter1 + "," + method.Parameter2);

                        if (method.ObjectName == "SERVICE-" + Common.ComputerName) // This Service
                        {
                            switch (method.MethodName)
                            {
                                case "BROADCAST":
                                    Log.Info("-> UDP: " + method.Parameter1 + ", " + method.Parameter2);
                                    UDPConnection.SendObject(method.Parameter1, method.Parameter2, new IPEndPoint(IPAddress.Broadcast, 10051));
                                    break;
                                case "EXECUTE" : 
                                    Log.Info("Received Execute Method");
                                    UDPConnection.SendObject("Command", method.Parameter1 + " | " + method.Parameter2 + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10051));
                                    break;
                                case "START PLUGIN":
                                    StartPlugin(serviceObject, method);
                                    break;
                                case "STOP PLUGIN":
                                    StopPlugin(serviceObject, method);
                                    break;
                                case "RELOAD PLUGINS":
                                    LoadPlugins(serviceObject);
                                    break;
                            }                                                                            
                            OSAEMethodManager.MethodQueueDelete(method.Id);
                        }
                        else if (method.ObjectName.Split('-')[0] == "SERVICE") // Client Services
                        {
                            Log.Debug("Method for client service.  Sending Broadcast.");
                            switch (method.MethodName)
                            {
                                case "ON":
                                    Log.Info("-> UDP " + method.ObjectName + " | ON");
                                    UDPConnection.SendObject("Manager", method.ObjectName + " | ON", new IPEndPoint(IPAddress.Broadcast, 10052));
                                    break;
                                case "OFF":
                                    Log.Info("-> UDP " + method.ObjectName + " | OFF");
                                    UDPConnection.SendObject("Manager", method.ObjectName + " | OFF", new IPEndPoint(IPAddress.Broadcast, 10052));
                                    break;
                                case "EXECUTE":
                                    Log.Info("Recieved Execute Method Name");
                                    UDPConnection.SendObject("Command", method.Parameter1 + " | " + method.Parameter2 + " | " + method.ObjectName.Substring(8), new IPEndPoint(IPAddress.Broadcast, 10051));
                                    break;
                                case "START PLUGIN":
                                    UDPConnection.SendObject("Plugin", method.ObjectName + " | ON", new IPEndPoint(IPAddress.Broadcast, 10051));
                                    Log.Info("-> UDP: Plugin, " + method.ObjectName + " | ON");
                                    //object name | owner | method name | param1 | param 2 | address | from object 
                                    //StartPlugin(method);
                                    break;
                                case "STOP PLUGIN":
                                    UDPConnection.SendObject("Plugin", method.ObjectName + " | OFF", new IPEndPoint(IPAddress.Broadcast, 10051));
                                    Log.Info("-> UDP: Plugin, " + method.ObjectName + " | OFF");
                                    //StopPlugin(method);
                                    break;
                            }
                            OSAEMethodManager.MethodQueueDelete(method.Id);
                        }
                        else if (method.ObjectName.Split('-')[0] != "SERVICE")
                        {// THIS IS NOT GOOD ENOUGH.   it intercepts all plugins not just local service ones....
                            //Look up the basetype, if it is a plugin, THEN you can parse on and off for the intercept. 
                            //You must also look at the container and see if it is this service like above.

                            OSAEObject tempObj = OSAEObjectManager.GetObjectByName(method.ObjectName);
                            string isContainerService = tempObj.Container.Split('-')[0];
                            if (tempObj.BaseType == "PLUGIN" && tempObj.Container == ("SERVICE-" + Common.ComputerName))  // Plugins on the localhost
                            {
                                switch (method.MethodName)
                                {
                                    case "ON":
                                        OSAEMethodManager.MethodQueueDelete(method.Id);
                                        Log.Info("Recieved Start for: " + method.Owner);
                                        StartPlugin(serviceObject, method);
                                        break;
                                    case "OFF":
                                        OSAEMethodManager.MethodQueueDelete(method.Id);
                                        Log.Info("Recieved Stop for: " + method.Owner);
                                        StopPlugin(serviceObject, method);
                                        break;
                                    default:
                                        {
                                            foreach (Plugin plugin in plugins)
                                            {
                                                if (method.ObjectName == plugin.PluginName)
                                                {
                                                    plugin.ExecuteCommand(method);
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                }
                                OSAEMethodManager.MethodQueueDelete(method.Id);
                            }
                            else if (tempObj.BaseType == "PLUGIN" && isContainerService == "SERVICE")  // Plugins on a remote Client Service
                            {
                                //We can translate the the Object from the method to a parameter and just tell the client service to start/stop the plugin
                                switch (method.MethodName)
                                {
                                    case "ON":
                                        Log.Info("Sending Remote Start for: " + method.Owner);
                                        UDPConnection.SendObject("Plugin", method.ObjectName + " | ON", new IPEndPoint(IPAddress.Broadcast, 10051));
                                        Log.Info("-> UDP: Plugin, " + method.ObjectName + " | ON");
                                        break;
                                    case "OFF":
                                        Log.Info("Sending Remote Stop for: " + method.Owner);
                                        UDPConnection.SendObject("Plugin", method.ObjectName + " | OFF", new IPEndPoint(IPAddress.Broadcast, 10051));
                                        Log.Info("-> UDP: Plugin, " + method.ObjectName + " | OFF");
                                        break;
                                    default:
                                        {
                                            Log.Debug("-> UDP: Command, " + method.ObjectName + " | " + method.Owner + " | " + method.MethodName + " | " + method.Parameter1 + " | " + method.Parameter2 + " | " + method.Address + " | " + method.Id);
                                            UDPConnection.SendObject("Method", method.ObjectName + " | " + method.MethodName + " | " + method.Parameter1 + " | " + method.Parameter2 + " | " + method.Address + " | " + method.Owner + " | " + method.FromObject, new IPEndPoint(IPAddress.Broadcast, 10051));
                                            break;
                                        }
                                }
                                Log.Debug("Removing method from queue with ID: " + method.Id);
                                OSAEMethodManager.MethodQueueDelete(method.Id);
                            }
                            else
                            {
                                bool processed = false;
                                foreach (Plugin plugin in plugins)
                                {
                                    if (string.IsNullOrEmpty(method.Owner) || method.Owner.ToLower() == plugin.PluginName.ToLower() || method.ObjectName.ToLower() == plugin.PluginName.ToLower())
                                    {
                                        plugin.ExecuteCommand(method);
                                        processed = true;
                                        break;
                                    }
                                }

                                if (!processed)
                                {
                                    Log.Debug("Method found for client service plugin.  Sending Broadcast.");


                                    UDPConnection.SendObject("Plugin", method.ObjectName + " | ON", new IPEndPoint(IPAddress.Broadcast, 10051));



                                    Log.Debug("-> UDP: Command, " + method.ObjectName + " | " + method.Owner + " | " + method.MethodName + " | " + method.Parameter1 + " | " + method.Parameter2 + " | " + method.Address + " | " + method.Id);
                                    UDPConnection.SendObject("Command", method.ObjectName + " | " + method.Owner + " | "
                                        + method.MethodName + " | " + method.Parameter1 + " | " + method.Parameter2 + " | "
                                        + method.Address + " | " + method.Id, new IPEndPoint(IPAddress.Broadcast, 10051));
                                    UDPConnection.SendObject("Command", "Testing", new IPEndPoint(IPAddress.Broadcast, 10051));
                                    UDPConnection.SendObject("Method", "Testing", new IPEndPoint(IPAddress.Broadcast, 10051));
                                    UDPConnection.SendObject("Plugin", "Testing", new IPEndPoint(IPAddress.Broadcast, 10051));
                                    Log.Debug("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
                                }
                                Log.Debug("Removing method from queue with ID: " + method.Id);
                                OSAEMethodManager.MethodQueueDelete(method.Id);
                                break;
                            }
                       }
                    }
                }
                catch (Exception ex)
                { Log.Error("Error in QueryCommandQueue!", ex); }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Stops a plugin based on a method
        /// </summary>
        /// <param name="method">The method containing the information of the plugin to stop</param>
        private void StopPlugin(string serviceName, OSAEMethod method)
        {
            foreach (Plugin p in plugins)
            {
                if (p.PluginName == method.ObjectName)
                {
                    OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
                    if (obj != null)
                    {
                        stopPlugin(serviceName, p);
                        UDPConnection.SendObject("Plugin", p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | Stopped | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10051));
                    }
                }
            }
        }

        /// <summary>
        /// Starts a plugin based on a method
        /// </summary>
        /// <param name="method">The method containing the information of the plugin to stop</param>
        private void StartPlugin(string serviceName, OSAEMethod method)
        {
            foreach (Plugin p in plugins)
            {
                if (p.PluginName == method.ObjectName)
                {
                    OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
                    if (obj != null) startPlugin(serviceName, p);
                }
            }
        }        
    }
}
