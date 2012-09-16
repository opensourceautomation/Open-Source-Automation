using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Data;
using System.Threading;
using OpenSourceAutomation;

namespace OSAE.Onkyo
{
    [AddIn("Onkyo", Version = "0.2.2")]
    public class Onkyo : IOpenSourceAutomationAddIn
    {
        public delegate void AddDeviceDelegate(Device ODevDele);
        
        OSAE osae = new OSAE("Onkyo");
        string pName;
        List<Receiver> receivers = new List<Receiver>();
        int _ctr = 1;
        UDPListen _UDPListen;
        UDPSend _UDPSend;

        public void ProcessCommand(System.Data.DataTable table)
        {
            System.Data.DataRow row = table.Rows[0];
            osae.AddToLog("Found Command: " + row["method_name"].ToString() + " | param1: " + row["parameter_1"].ToString() + " | param2: " + row["parameter_1"].ToString(), false);
            
            if(row["object_name"].ToString() == pName)
            {
                if(row["method_name"].ToString() == "SCAN")
                {
                    _UDPSend.Send();
                }
            }
            else
            {                
                Receiver r = getReceiver(row["object_name"].ToString());
                if(r != null)
                {
                    #region Network
                    if (r.Type == "Network")
                    {
                        switch (row["method_name"].ToString())
                        {
                            case "ON":
                                SendCommand_Network(r, "!1PWR01");
                                osae.ObjectStateSet(r.Name, "ON");
                                break;
                            case "OFF":
                                SendCommand_Network(r, "!1PWR00");
                                osae.ObjectStateSet(r.Name, "OFF");
                                break;
                            case "MUTE":
                                SendCommand_Network(r, "!1AMT01");
                                break;
                            case "UNMUTE":
                                SendCommand_Network(r, "!1AMT00");
                                break;
                            case "VOLUME UP":
                                SendCommand_Network(r, "!1MVLUP");
                                break;
                            case "VOLUME DOWN":
                                SendCommand_Network(r, "!1MVLDOWN");
                                break;
                            case "SET VOLUME":
                                SendCommand_Network(r, "!1MVL" + Int32.Parse(row["parameter_1"].ToString()).ToString("X"));
                                break;
                            case "VCR/DVR":
                                SendCommand_Network(r, "!1SLI00");
                                break;
                            case "CBL/SAT":
                                SendCommand_Network(r, "!1SLI01");
                                break;
                            case "GAME":
                                SendCommand_Network(r, "!1SLI02");
                                break;
                            case "AUX1":
                                SendCommand_Network(r, "!1SLI03");
                                break;
                            case "BD/DVD":
                                SendCommand_Network(r, "!1SLI10");
                                break;
                            case "TV/CD":
                                SendCommand_Network(r, "!1SLI23");
                                break;
                            case "TUNER":
                                SendCommand_Network(r, "!1SLI26");
                                break;
                            case "DLNA":
                                SendCommand_Network(r, "!1SLI2B");
                                SendCommand_Network(r, "!1NSV00");
                                break;
                            case "VTUNER":
                                SendCommand_Network(r, "!1SLI2B");
                                SendCommand_Network(r, "!1NSV02");
                                break;
                            case "PANDORA":
                                SendCommand_Network(r, "!1SLI2B");
                                SendCommand_Network(r, "!1NSV04");
                                break;
                            case "SIRIUS":
                                SendCommand_Network(r, "!1SLI2B");
                                SendCommand_Network(r, "!1NSV03");
                                break;
                            case "MEDIAFLY":
                                SendCommand_Network(r, "!1SLI2B");
                                SendCommand_Network(r, "!1NSV09");
                                break;
                            case "NAPSTER":
                                SendCommand_Network(r, "!1SLI2B");
                                SendCommand_Network(r, "!1NSV07");
                                break;
                            case "FAVORITES":
                                SendCommand_Network(r, "!1SLI2B");
                                SendCommand_Network(r, "!1NSV01");
                                break;
                            case "UP":
                                SendCommand_Network(r, "!1OSDUP");
                                break;
                            case "DOWN":
                                SendCommand_Network(r, "!1OSDDOWN");
                                break;
                            case "RIGHT":
                                SendCommand_Network(r, "!1OSDRIGHT");
                                break;
                            case "LEFT":
                                SendCommand_Network(r, "!1OSDLEFT");
                                break;
                            case "ENTER":
                                SendCommand_Network(r, "!1OSDENTER");
                                break;
                            case "NETUP":
                                SendCommand_Network(r, "!1NTCUP");
                                break;
                            case "NETDOWN":
                                SendCommand_Network(r, "!1NTCDOWN");
                                break;
                            case "NETRIGHT":
                                SendCommand_Network(r, "!1NTCRIGHT");
                                break;
                            case "NETLEFT":
                                SendCommand_Network(r, "!1NTCLEFT");
                                break;
                            case "NETENTER":
                                SendCommand_Network(r, "!1NTCSELECT");
                                break;
                        }
                    }
                    #endregion
                    else if (r.Type == "Serial")
                    {

                    }

                }
            }
        }

        public void RunInterface(string pluginName)
        {
            osae.AddToLog("Running interface", false);
            pName = pluginName;
            osae.ObjectTypeUpdate("ONKYO RECEIVER", "ONKYO RECEIVER", "Onkyo Receiver", pluginName, "ONKYO RECEIVER", 0, 0, 0, 1);

            _UDPListen = new UDPListen();
            _UDPSend = new UDPSend();
            _UDPListen.OnkyoDevice += new DelegateOnkyoReply(OnkyoMessageHandler);

            _UDPListen.Listen();
            _UDPSend.Send();

            List<OSAEObject> objects = osae.GetObjectsByType("ONKYO RECEIVER");

            foreach (OSAEObject obj in objects)
            {
                Receiver r = new Receiver(obj.Name);
                foreach (ObjectProperty prop in obj.Properties)
                {
                    switch (prop.Name)
                    {
                        case "Communication Type":
                            r.Type = prop.Value;
                            break;
                        case "IP":
                            r.IP = prop.Value;
                            break;
                        case "Network Port":
                            try
                            {
                                r.NetworkPort = Int32.Parse(prop.Value);
                            }
                            catch
                            {
                                r.NetworkPort = 0;
                            }
                            break;
                        case "COM Port":
                            try
                            {
                                r.ComPort = Int32.Parse(prop.Value);
                            }
                            catch
                            {
                                r.ComPort = 0;
                            }
                            break;
                    }
                }

                receivers.Add(r);
                osae.AddToLog("Added receiver to list: " + r.Name, false);

                try
                {
                    if (r.Type == "Network" && r.IP != "" && r.NetworkPort != 0)
                    {
                        osae.AddToLog("Creating TCP Client: ip-" + r.IP + " port-" + r.NetworkPort, false);
                        r.tcpClient = new TcpClient(r.IP, r.NetworkPort);

                        //get a network stream from server
                        r.clientSockStream = r.tcpClient.GetStream();

                        // create new writer and reader stream to send and receive
                        r.clientStreamWriter = new StreamWriter(r.clientSockStream);
                        r.clientStreamReader = new StreamReader(r.clientSockStream);

                        //Start listening
                        r.Connect();
                    }
                    else if (r.Type == "Serial" && r.ComPort != 0)
                    {
                        //not implemented
                    }
                    else
                    {
                        osae.AddToLog(r.Name + " - Properties not set", true);
                    }
                }
                catch (Exception ex)
                {
                    osae.AddToLog("Error creating connection to receiver: " + ex.Message, true);
                }
            }
            osae.AddToLog("Run Interface Complete", false);
        }

        public void Shutdown()
        {
            foreach(Receiver r in receivers)
            {
                r.clientStreamReader.Close();
                r.clientStreamWriter.Close();
                r.tcpClient.Close();
            }

            _UDPListen.OnkyoDevice -= new DelegateOnkyoReply(OnkyoMessageHandler);
            _UDPListen.Dispose();
        }

        public Receiver getReceiver(string name)
        {
            foreach(Receiver r in receivers)
            {
                if(r.Name == name)
                    return r;
            }
            return null;
        }

        public void SendCommand_Network(Receiver r, String command)
        {
            try
            {
                int length = command.Length;
                length++;
                int total = length + 16;
                char code = (char)length;

                // build up packet header and rest of command - followed by <CR> (chr(13))
                //
                string line = "ISCP\x00\x00\x00\x10\x00\x00\x00" + code + "\x01\x00\x00\x00" + command + "\x0D";

                if (r.tcpClient.Connected)
                {
                    // send command to receiver
                    //
                    r.clientStreamWriter.WriteLine(line);
                    r.clientStreamWriter.Flush();
                    osae.AddToLog("Sent command: " + line, true);
                }
                else
                {
                    try
                    {
                        if (r.Type == "Network" && r.IP != "" && r.NetworkPort != 0)
                        {
                            osae.AddToLog("Creating TCP Client: ip-" + r.IP + " port-" + r.NetworkPort, false);
                            r.tcpClient = new TcpClient(r.IP, r.NetworkPort);

                            //get a network stream from server
                            r.clientSockStream = r.tcpClient.GetStream();

                            // create new writer and reader stream to send and receive
                            r.clientStreamWriter = new StreamWriter(r.clientSockStream);
                            r.clientStreamReader = new StreamReader(r.clientSockStream);

                            //Start listening
                            r.Connect();

                            // send command to receiver
                            //
                            r.clientStreamWriter.WriteLine(line);
                            r.clientStreamWriter.Flush();
                            osae.AddToLog("Sent command: " + line, true);
                        }
                        else
                        {
                            osae.AddToLog(r.Name + " - Properties not set", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        osae.AddToLog("Error creating connection to receiver.  Command can not be sent: " + ex.Message, true);
                    }
                }
            }
            catch (Exception e)
            {
                osae.AddToLog("Error sending command: " + e.Message, true);
            }
        }

        private void OnkyoMessageHandler(Device oDevice)
        {
            try
            {
                if (osae.GetObjectByName(oDevice.ModelName) == null)
                {
                    osae.ObjectAdd(oDevice.ModelName, oDevice.ModelName, "ONKYO RECEIVER", "", "", true);
                    osae.ObjectPropertySet(oDevice.ModelName, "IP", oDevice.IP);
                    osae.ObjectPropertySet(oDevice.ModelName, "Network Port", oDevice.Port.ToString());
                    osae.ObjectPropertySet(oDevice.ModelName, "Communication Type", "Network");

                }
                 osae.AddToLog(_ctr.ToString() + " - " + oDevice.Region + Environment.NewLine +
                        _ctr.ToString() + " - " + oDevice.ModelName + Environment.NewLine +
                            _ctr.ToString() + " - " + oDevice.Mac + Environment.NewLine +
                                _ctr.ToString() + " - " + oDevice.IP + Environment.NewLine +
                                    _ctr.ToString() + " - " + oDevice.Port + Environment.NewLine, true);
                    _ctr++;
            }
            catch (Exception ex)
            {

                osae.AddToLog("Error receiver device info: " + ex.Message, true);
            }

        }
    }

    public class Receiver
    {
        private string _type;
        private string _name;
        private string _ip;
        private int _NetworkPort = 0;
        private int _comPort = 0;
        
        public StreamWriter clientStreamWriter = null;
        public StreamReader clientStreamReader = null;
        public TcpClient tcpClient = null;
        public NetworkStream clientSockStream = null;

        private OSAE osae = new OSAE("Onkyo");
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        public int NetworkPort
        {
            get { return _NetworkPort; }
            set { _NetworkPort = value; }
        }
        public int ComPort
        {
            get { return _comPort; }
            set { _comPort = value; }
        }

        public Receiver(string name)
        {
            _name = name;
        }

        public void Connect()
        {
            Thread listenThread = new Thread(new ThreadStart(Listen));
            listenThread.Start();
        }

        private void Listen()
        {
            while(tcpClient.Connected)
            {
                try
                {
                    string curLine = clientStreamReader.ReadLine();
                    osae.AddToLog("Received data from receiver: " + curLine, false);
                    if (curLine.IndexOf("!1PWR00") > -1)
                        osae.ObjectStateSet(_name, "OFF");
                    if (curLine.IndexOf("!1PWR01") > -1)
                        osae.ObjectStateSet(_name, "ON");
                }
                catch (Exception ex)
                {
                    osae.AddToLog("Error reading from stream: " + ex.Message, true);
                }
            }
        }
    }
}
