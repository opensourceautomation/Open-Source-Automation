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
                logging.AddToLog("received message: " + e.Message, false);
                if (e.Message == "connected")
                {
                    try
                    {
                        logging.AddToLog("client connected", false);
                        foreach (Plugin p in masterPlugins)
                        {
                            string msg = p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | " + p.Status + " | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName;

                            sendMessageToClients("plugin", msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        logging.AddToLog("Error sending plugin messages to clients: " + ex.Message, true);
                    }
                }
                else
                {
                    string[] arguments = e.Message.Split('|');
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
                                    }
                                }
                            }
                        }
                        if (!local)
                        {
                            sendMessageToClients("enablePlugin", e.Message);
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
                            if (plugin.PluginName == arguments[1])
                            {
                                if (plugin.Status == "Running")
                                {
                                    disablePlugin(plugin);
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
        private void sendMessageToClients(string msgType, string message)
        {
            try
            {
                logging.AddToLog("Sending message to clients: " + msgType + " - " + message, false);
                Thread thread = new Thread(() => wcfService.SendMessageToClients(msgType, message, Common.ComputerName));
                thread.Start();
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error sending message to clients: " + ex.Message, true);
            }
        }
    }
}
