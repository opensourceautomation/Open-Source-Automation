using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using NetworkCommsDotNet;

namespace OSAE.Service
{
    partial class OSAEService
    {
        private void StartNetworkListener()
        {
            try
            {
                logging.AddToLog("Starting TCP listener", false);
                string ip = Common.WcfServer;
                if (ip == "localhost")
                    ip = "127.0.0.1";
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Plugin", PluginMessageReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Method", MethodMessageReceived);
                //Start listening for incoming connections
                TCPConnection.StartListening(new IPEndPoint(IPAddress.Parse(ip), 10000));
                logging.AddToLog("TCP Listener started", false);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error starting listener:" + ex.Message, false);
            }
        }

        private void PluginMessageReceived(PacketHeader header, Connection connection, string message)
        {
            logging.AddToLog("\nA message was recieved from " + connection.ToString() + " which said '" + message + "'.", true);

            string[] arguments = message.Split('|');
            bool local = false;
            if (arguments[1] == "True")
            {
                OSAEObjectStateManager.ObjectStateSet(arguments[0], "ON", sourceName);
            }
            else if (arguments[1] == "False")
            {
                OSAEObjectStateManager.ObjectStateSet(arguments[0], "OFF", sourceName);
            }

            foreach (Plugin p in plugins)
            {
                if (p.PluginName == arguments[0])
                {
                    local = true;

                    OSAEObject obj = OSAEObjectManager.GetObjectByName(p.PluginName);
                    if (obj != null)
                    {
                        if (arguments[1] == "True")
                        {
                            enablePlugin(p);
                            UDPConnection.SendObject("Plugin", p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | Running | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10000));
                        }
                        else if (arguments[1] == "False")
                        {
                            disablePlugin(p);
                            UDPConnection.SendObject("Plugin", p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | Stopped | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10000));
                        }
                    }
                }
            }
            if (!local)
            {
                UDPConnection.SendObject("Plugin", message, new IPEndPoint(IPAddress.Broadcast, 10000));
            }
        }

        private void MethodMessageReceived(PacketHeader header, Connection connection, string message)
        {
            logging.AddToLog("\nA message was recieved from " + connection.ToString() + " which said '" + message + "'.", true);
        }
    }
}
