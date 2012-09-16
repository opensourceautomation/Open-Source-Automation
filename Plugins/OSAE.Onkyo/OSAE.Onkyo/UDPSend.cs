using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace OSAE.Onkyo
{
    //----------------------------------------------------------------------//
    // Onkyo multicast address Device Search Address        239.255.255.250 //
    // Default Onkyo port                                   60128;          //
    // Onkyo autodetect command                             "!xECNQSTN"     //
    // using default receive port                           50000           //
    //----------------------------------------------------------------------//
    public class UDPSend : IDisposable
    {
        string _SEARCH_COMMAND = "!xECNQSTN";
        string _SEARCH_PACKET = "ISCP\x00\x00\x00\x10\x00\x00\x00{0}\x01\x00\x00\x00{1}\x0D";

        
        /// <summary>
        /// UDP Multicast Send (search request) to Onkyo devices
        /// </summary>
        /// <param name="MCGroup">The multicast IP subnet that we are broadcasting to/on</param>
        /// <param name="RemotePort">The default Onkyo port that we are sending the search request to</param>
        /// <param name="ReceivePort">
        /// This is the port we want the Onkyo to send the reply to 
        /// (we set our UDP listen server to listen on this port).
        /// </param>
        public void Send(string MCGroup = "239.255.255.250", int RemotePort = 60128, int ReceivePort = 50000)
        {
            try
            {
                using (Socket udpSend = new Socket(AddressFamily.InterNetwork,
                                     SocketType.Dgram, ProtocolType.Udp))
                {

                    IPEndPoint lip = new IPEndPoint(IPAddress.Parse(DnsUtils.GetLocalIP()), ReceivePort);
                    /* Bind to the local IP and ReceivePort, this tags the packet with a return port
                    if we don't the system will choose a random port for us (which we don't want). */
                    udpSend.Bind(lip);
                    IPEndPoint iep = new IPEndPoint(IPAddress.Parse(MCGroup), RemotePort);

                    int length = _SEARCH_COMMAND.Length + 1;

                    byte[] data = Encoding.ASCII.GetBytes(String.Format(_SEARCH_PACKET, (char)length, _SEARCH_COMMAND));
                    udpSend.SendTo(data, iep);
                    udpSend.Close();
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("Error: {0} {1}", ex.ErrorCode, ex.Message);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _SEARCH_COMMAND = null;
            _SEARCH_PACKET = null;
        }
    }
}
