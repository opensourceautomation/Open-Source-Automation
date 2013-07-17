namespace OSAE.SysLog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class SysLog : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("SysLog");

        CancellationTokenSource cts;
        CancellationToken token;
        Task task;
        bool logEventsToFile;
        int port = 514;
        List<SysLogObject> lookup = new List<SysLogObject>();

        /// <summary>
        /// Plugin name
        /// </summary>
        string pName;

        /// <summary>
        /// OSA Plugin Interface - Commands the be processed by the plugin
        /// </summary>
        /// <param name="method">Method containging the command to run</param>
        public override void ProcessCommand(OSAEMethod method)
        {
            // place the plugin command code here leave empty if unable to process commands
        }

        /// <summary>
        /// OSA Plugin Interface - called on start up to allow plugin to do any tasks it needs
        /// </summary>
        /// <param name="pluginName">The name of the plugin from the system</param>
        public override void RunInterface(string pluginName)
        {
            pName = pluginName;

            try
            {
                logging.AddToLog("Starting SysLog...", true);

                logEventsToFile = Boolean.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Log to file").Value);
                port = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value);                              

                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                UdpClient udpListener = new UdpClient(port);
                
                cts = new CancellationTokenSource();
                token = cts.Token;

                OSAEObjectCollection c = OSAEObjectManager.GetObjectsByType("SysLog-Trigger");
                if (c.Count > 0)
                {
                    logging.AddToLog("Found " + c.Count + " triggers to watch for", false);
                    GetMatches(c);
                }
                else
                {
                    logging.AddToLog("No SysLog Triggers found to look for creating example", true);
                    OSAEObjectManager.ObjectAdd("SysLog Tigger - Example", "This is an an example trigger created by the plugin which can be deleted", "SysLog-Trigger", "", "", true);
                    OSAEObjectPropertyManager.ObjectPropertySet("SysLog Tigger - Example", "Trigger String", "The string to look for", pName);
                    OSAEObjectPropertyManager.ObjectPropertySet("SysLog Tigger - Example", "Source IP", "192.168.0.1", pName);
                }

                task = Task.Factory.StartNew(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (udpListener.Available > 0)
                        {
                            try
                            {
                                byte[] bReceive = udpListener.Receive(ref anyIP);
                                string sReceive = Encoding.ASCII.GetString(bReceive);
                                string sourceIP = anyIP.Address.ToString();

                                Task messageProcessor = new Task(() => ProcessEvent(sourceIP, sReceive));
                                messageProcessor.Start();
                            }
                            catch (Exception ex)
                            {
                                logging.AddToLog("Exception occurred in SysLog details: " + ex.Message, true);
                            }
                        }
                        else
                        {
                            token.WaitHandle.WaitOne(300);
                        }
                    }

                    if (token.IsCancellationRequested)
                    {
                        logging.AddToLog("Cancellation Token Set", false);
                    }
                }, token);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error during RunInterface: " + ex.Message, true);
            }
        }

        private void GetMatches(OSAEObjectCollection collection)
        {
            logging.AddToLog("Watching for the following Messages:", false);
            foreach (OSAEObject obj in collection)
            {
                SysLogObject sysLogObj = new SysLogObject();

                sysLogObj.TriggerString = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Trigger String").Value;
                sysLogObj.Source = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Source IP").Value;
                sysLogObj.ExactMatch = bool.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Exact Match").Value);
                sysLogObj.OsaObjectName = obj.Name;

                lookup.Add(sysLogObj);
                logging.AddToLog("Source IP is: " + sysLogObj.Source , false);

                if (sysLogObj.ExactMatch)
                {
                    logging.AddToLog("Message exactly matches: " + sysLogObj.TriggerString, false);
                }
                else
                {
                    logging.AddToLog("Message Contains: " + sysLogObj.TriggerString, false);
                }
            }
        }

        private void ProcessEvent(string sourceIp, string message)
        {
            string totalMessage = sourceIp + " - " + message;

            if (logEventsToFile)
            {
                Logging.AddToLog("Source IP: " + sourceIp, true, "SysLogMessages");
                Logging.AddToLog("Sent the following message: " + message, true, "SysLogMessages");
            }
            
            bool foundMatch = false;

            foreach (SysLogObject obj in lookup)
            {
                if (sourceIp == obj.Source)
                {
                    if (obj.ExactMatch)
                    {
                        if (message == obj.TriggerString)
                        {
                            foundMatch = true;
                            logging.AddToLog("Adding to Event log that Event Occurred for object: " + obj.OsaObjectName, false);
                            logging.EventLogAdd(obj.OsaObjectName, "TRIGGERED");
                            break;
                        }                        
                    }
                    else
                    {
                        if (message.Contains(obj.TriggerString))
                        {
                            foundMatch = true;
                            logging.AddToLog("Adding to Event log that Event Occurred for object: " + obj.OsaObjectName, false);
                            logging.EventLogAdd(obj.OsaObjectName, "TRIGGERED");
                            break;
                        }                       
                    }
                }
            }

            if (foundMatch)
            {
                logging.AddToLog("Found a match for: " + totalMessage, false);                                
            }
            else
            {
                logging.AddToLog("No match found for: " + totalMessage, false);
            }
        }

        /// <summary>
        /// OSA Plugin Interface - The plugin has been asked to shut down
        /// </summary>        
        public override void Shutdown()
        {
            cts.Cancel();
            logging.AddToLog("Stopping SysLog...", true);            
        }
    }

    public class SysLogObject
    {
        public string OsaObjectName { get; set; }
        public string TriggerString { get; set; }
        public string Source { get; set; }
        public bool ExactMatch { get; set; }
    }
}
