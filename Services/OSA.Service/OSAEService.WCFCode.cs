namespace OSAE.Service
{
    using System;
    using System.ServiceModel;
    using System.Threading;

    partial class OSAEService
    {
        private void StartWCFService()
        {
            try
            {
                wcfService = new WCF.WCFService();
                sHost = new ServiceHost(wcfService);
                wcfService.MessageReceived += new EventHandler<WCF.CustomEventArgs>(wcfService_MessageReceived);
                sHost.Open();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error starting WCF service: " + ex.Message, true);
            }
        }       

        /// <summary>
        /// Event happens when a wcf client invokes it
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void wcfService_MessageReceived(object source, WCF.CustomEventArgs e)
        {
            try
            {
                logging.AddToLog("received message: " + e.Message.Type + " - " + e.Message.Message, false);
                if (e.Message.Type == WCF.OSAEWCFMessageType.CONNECT)
                {
                    try
                    {
                        logging.AddToLog("client connected", false);
                        foreach (Plugin p in masterPlugins)
                        {
                            string msg = p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | " + p.Status + " | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName;

                            sendMessageToClients(WCF.OSAEWCFMessageType.PLUGIN, msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        logging.AddToLog("Error sending plugin messages to clients: " + ex.Message, true);
                    }
                }
                else if (e.Message.Type == WCF.OSAEWCFMessageType.LOADPLUGINS)
                {
                    LoadPlugins();
                }
                else
                {
                    string[] arguments = e.Message.Message.Split('|');
                    if (arguments[0] == "ENABLEPLUGIN")
                    {
                        bool local = false;
                        if (arguments[2] == "True")
                        {
                            OSAEObjectStateManager.ObjectStateSet(arguments[1], "ON", sourceName);
                        }
                        else if (arguments[2] == "False")
                        {
                            OSAEObjectStateManager.ObjectStateSet(arguments[1], "OFF", sourceName);
                        }

                        foreach (Plugin p in plugins)
                        {
                            if (p.PluginName == arguments[1])
                            {
                                local = true;

                                OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
                                if (obj != null)
                                {
                                    if (arguments[2] == "True")
                                    {
                                        enablePlugin(p);
                                    }
                                    else if (arguments[2] == "False")
                                    {
                                        disablePlugin(p);
                                        sendMessageToClients(WCF.OSAEWCFMessageType.PLUGIN, p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | Stopped | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName);
                                    }
                                }
                            }
                        }
                        if (!local)
                        {
                            sendMessageToClients(WCF.OSAEWCFMessageType.PLUGIN, e.Message.Message);
                        }
                    }
                    else if (arguments[0] == "plugin")
                    {
                        bool found = false;
                        foreach (Plugin plugin in masterPlugins)
                        {
                            if (plugin.PluginName == arguments[1])
                            {
                                if (arguments[4].ToLower() == "true")
                                    plugin.Enabled = true;
                                else
                                    plugin.Enabled = false;
                                plugin.PluginVersion = arguments[3];
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            Plugin p = new Plugin();
                            p.PluginName = arguments[1];
                            p.PluginVersion = arguments[3];
                            if (arguments[4].ToLower() == "true")
                            {
                                p.Enabled = true;
                            }
                            else
                            {
                                p.Enabled = false;
                            }
                            masterPlugins.Add(p);
                        }
                    }
                    else if (arguments[0] == "updatePlugin")
                    {
                        foreach (Plugin plugin in masterPlugins)
                        {
                            if (plugin.PluginType == arguments[1])
                            {
                                if (plugin.Status == "ON")
                                {
                                    disablePlugin(plugin);
                                    sendMessageToClients(WCF.OSAEWCFMessageType.PLUGIN, plugin.PluginName + " | " + plugin.Enabled.ToString() + " | " + plugin.PluginVersion + " | Stopped | " + plugin.LatestAvailableVersion + " | " + plugin.PluginType + " | " + Common.ComputerName);
                                }

                                //code for downloading and installing plugin
                                break;
                            }
                        }
                    }
                }

                logging.AddToLog("-----------Master plugin list", false);
                foreach (Plugin p in masterPlugins)
                {
                    logging.AddToLog(" --- " + p.PluginName, false);
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error receiving message: " + ex.Message, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="message"></param>
        private void sendMessageToClients(WCF.OSAEWCFMessageType msgType, string message)
        {
            try
            {
                WCF.OSAEWCFMessage msg = new WCF.OSAEWCFMessage();
                msg.Type = msgType;
                msg.Message = message;
                msg.From = OSAE.Common.ComputerName;
                msg.TimeSent = DateTime.Now;
                Thread thread = new Thread(() => wcfService.SendMessageToClients(msg));
                thread.Start();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error sending message to clients: " + ex.Message, true);
            }
        }
    }
}
