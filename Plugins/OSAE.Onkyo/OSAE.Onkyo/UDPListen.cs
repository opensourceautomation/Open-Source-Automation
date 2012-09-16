using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace OSAE.Onkyo
{
    public delegate void DelegateOnkyoReply(Device Reply);

    public class UDPListen :IDisposable
    {
        public event DelegateOnkyoReply OnkyoDevice;
       
        private Socket _udpSocket;
        private byte[] _buffer;
        private int _port;
        private int _bufferSize;

        /// <summary>
        /// Async Listen Server for UDP reply from Onkyo Devices
        /// </summary>
        /// <param name="port">Listen Port</param>
        /// <param name="bufferSize">Receive message buffersize</param>
        public void Listen(int port = 50000, int bufferSize = 512)
        {
            try
            {
                _port = port;
                _bufferSize = bufferSize;
                _buffer = new byte[_bufferSize];

                _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint iEP = new IPEndPoint(IPAddress.Any, _port);
                EndPoint ep = iEP;
                _udpSocket.Bind(iEP);

                // Listen for data
                _udpSocket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref ep, ReceiveData, _udpSocket);
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("Error: {0} {1}", ex.ErrorCode, ex.Message);
            }
        }

        private void ReceiveData(IAsyncResult iar)
        {
            try
            {
                //Get the received message.
                Socket recvSock = (Socket)iar.AsyncState;
                EndPoint rEP = new IPEndPoint(IPAddress.Any, 0);
                int dataLen = recvSock.EndReceiveFrom(iar, ref rEP);

                // Copy data to a local buffer so we can start listening again asap
                byte[]  localData = new byte[dataLen];
                Array.Copy(_buffer, localData, dataLen);
                      
                //Start listening for a new message.
                IPEndPoint iEP = new IPEndPoint(IPAddress.Any, _port);
                EndPoint ep = iEP;
                _udpSocket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref ep, ReceiveData, _udpSocket);
                
                // Handle the local data buffer now (after we restarted the listen server)
                string stringData = Encoding.UTF8.GetString(localData, 0, localData.Length);
                stringData = CleanString(stringData, true);
                
                // ------------------------------------------------------------------------------
                // Debug.WriteLine(stringData + "/" + ((System.Net.IPEndPoint)rEP).Address);
                // Result will look something like below: /192.168.1.36 IS TAGGED ON FROM REMOTEIP
                // ISCP&!1ECNTX-NR609/60128/XX/0009F013A3B6/192.168.1.36
                // ------------------------------------------------------------------------------

                                
                Device oDev = new Device();
                oDev.LoadDevice(stringData + "/" + ((System.Net.IPEndPoint)rEP).Address);

                //// Invoke Delegate Event here to return ct_Onkyo.Device object
                OnkyoDevice.Invoke(oDev);

                oDev = null;              
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("Error: {0} {1}", ex.ErrorCode, ex.Message);
            }
        }

        /// <summary>
        /// Cleans the Onkyo reply string of control chars and spaces
        /// </summary>
        /// <param name="str">String to be cleaned</param>
        /// <param name="RemoveSpaces">Optional: if set to true will removes whitespace from the string</param>
        /// <returns>Cleaned Onkyo reply string</returns>
        static private string CleanString(string str, Boolean RemoveSpaces = false)
        {
            if (str != null && str.Length > 0)
            {   
                StringBuilder sb = new StringBuilder(str.Length);
                foreach (char c in str)
                {
                    if (!char.IsControl(c)) 
                    {
                        if (c.Equals(' ') && RemoveSpaces)
                            continue;
                        else
                            sb.Append(c);
                    }
                }
                str = sb.ToString();
            }
            return str;
        }

        /// <summary>
        /// Cleanup: shutdown/close the socket, dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                _udpSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                if (_udpSocket != null)
                    _udpSocket.Close();

                //Clean up  
                _udpSocket = null;
                _buffer = null;
            }
        }

        /// <summary>
        /// ReadOnly: Returns the sockets receive buffersize
        /// </summary>
        public int BufferSize
        {
            get { return _bufferSize; }
        }

        /// <summary>
        /// ReadOnly: Returns the sockets listen Port
        /// </summary>
        public int Port
        {
            get { return _port; }
        }

    }
}
