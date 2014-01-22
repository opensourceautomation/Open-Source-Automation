using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using NetworkCommsDotNet;

namespace OSAE.Service
{
    partial class OSAEService
    {
        private void StartNetworkListener()
        {
            this.Log.Info("Starting TCP Listener");

            try
            {
                string ip = Common.LocalIPAddress();
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Plugin", PluginMessageReceived);
                NetworkComms.AppendGlobalIncomingPacketHandler<string>("Method", MethodMessageReceived);
                //Start listening for incoming connections
                TCPConnection.StartListening(new IPEndPoint(IPAddress.Parse(ip), 10000));
                //TCPConnection.StartListening(true);

                foreach (System.Net.IPEndPoint localEndPoint in TCPConnection.ExistingLocalListenEndPoints()) 
                    this.Log.Info("Service listening for TCP connection on: " + localEndPoint.Address + ":" + localEndPoint.Port);
            }
            catch (Exception ex)
            {
                this.Log.Error("Error starting TCP Listener: " + ex.Message, ex);
            }
        }

        private void PluginMessageReceived(PacketHeader header, Connection connection, string message)
        {
            this.Log.Info("A message was recieved from " + connection.ToString() + " which said '" + message + "'.");

            string[] arguments = message.Split('|');
            bool local = false;
            if (arguments[1] == "ON")
            {
                OSAEObjectStateManager.ObjectStateSet(arguments[0], "ON", sourceName);
            }
            else if (arguments[1] == "OFF")
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
                        if (arguments[1] == "ON")
                        {
                            enablePlugin(p);
                            UDPConnection.SendObject("Plugin", p.PluginName + " | " + p.Enabled.ToString() + " | " + p.PluginVersion + " | Running | " + p.LatestAvailableVersion + " | " + p.PluginType + " | " + Common.ComputerName, new IPEndPoint(IPAddress.Broadcast, 10000));
                        }
                        else if (arguments[1] == "OFF")
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
            this.Log.Info("A message was recieved from " + connection.ToString() + " which said '" + message + "'.");
        }
    }
}
