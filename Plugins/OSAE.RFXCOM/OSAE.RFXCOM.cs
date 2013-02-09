using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Globalization;

namespace OSAE.RFXCOM
{
    public class RFXCOM : OSAEPluginBase
    {
        OSAE osae = new OSAE("RFXCOM");
        Logging logging = new Logging("RFXCOM");

        private System.Timers.Timer tmrRead = new System.Timers.Timer(100);
        private string rcvdStr = "";
        private string pName;
        private SerialPort RS232Port = new SerialPort();
        private bool gRecComPortEnabled = false;
        private int Resettimer = 0;

        private int trxType = 0;
        private byte[] recbuf = new byte[40];
        private byte recbytes;
        private int bytecnt = 0;
        private string message;
        private byte bytSeqNbr = 0;
        private byte bytRemoteToggle = 0;

        private byte bytFWversion;
        private bool tcp;
        private byte maxticks = 0;
        private bool LogActive = false;
        private byte[] TCPData = new byte[1025];

        public override void ProcessCommand(OSAEMethod method)
        {
            logging.AddToLog("--------------Processing Command---------------", false);
            logging.AddToLog("Command: " + method.MethodName, false);

            OSAEObject obj = osae.GetObjectByName(method.ObjectName);
            logging.AddToLog("Object Name: " + obj.Name, false);
            logging.AddToLog("Object Type: " + obj.Type, false);
            logging.AddToLog("Object Adress: " + obj.Address, false);

            try
            {
                byte[] kar;

                switch (obj.Type)
                {
                    #region Lighting 1

                    case "X10 RELAY":
                    case "ARC BINARY SWITCH":
                    case "ELRO BINARY SWITCH":
                    case "WAVEMAN BINARY SWITCH":
                    case "EMW200 BINARY SWITCH":
                    case "RISING SUN BINARY SWITCH":
                    case "IMPULS BINARY SWITCH":
                        logging.AddToLog("Executing Lighting1 command", false);

                        kar = new byte[(byte)LIGHTING1.size + 1];
                        byte type_l1 = 0;

                        switch (obj.Type)
                        {
                            case "X10 RELAY":
                                type_l1 = (byte)LIGHTING1.sTypeX10;
                                break;
                            case "ARC BINARY SWITCH":
                                type_l1 = (byte)LIGHTING1.sTypeARC;
                                break;
                            case "ELRO BINARY SWITCH":
                                type_l1 = (byte)LIGHTING1.sTypeAB400D;
                                break;
                            case "WAVEMAN BINARY SWITCH":
                                type_l1 = (byte)LIGHTING1.sTypeWaveman;
                                break;
                            case "EMW200 BINARY SWITCH":
                                type_l1 = (byte)LIGHTING1.sTypeEMW200;
                                break;
                            case "IMPULS BINARY SWITCH":
                                type_l1 = (byte)LIGHTING1.sTypeIMPULS;
                                break;
                            case "RISING SUN BINARY SWITCH":
                                type_l1 = (byte)LIGHTING1.sTypeRisingSun;
                                break;
                        }

                        kar[(byte)LIGHTING1.packetlength] = (byte)LIGHTING1.size;
                        kar[(byte)LIGHTING1.packettype] = (byte)LIGHTING1.pType;
                        kar[(byte)LIGHTING1.subtype] = type_l1;
                        kar[(byte)LIGHTING1.seqnbr] = bytSeqNbr;
                        kar[(byte)LIGHTING1.housecode] = (byte)Convert.ToInt32(obj.Address.Substring(0, 1));
                        kar[(byte)LIGHTING1.unitcode] = (byte)Convert.ToInt32(obj.Address.Substring(1));
                        switch (method.MethodName)
                        {
                            case "OFF":
                                kar[(byte)LIGHTING1.cmnd] = (byte)LIGHTING1.sOff;
                                osae.ObjectStateSet(obj.Name, "OFF");
                                break;
                            case "ON":
                                kar[(byte)LIGHTING1.cmnd] = (byte)LIGHTING1.sOn;
                                osae.ObjectStateSet(obj.Name, "ON");
                                break;
                            case "ALL OFF":
                                kar[(byte)LIGHTING1.cmnd] = (byte)LIGHTING1.sAllOff;
                                osae.ObjectStateSet(obj.Name, "OFF");
                                break;
                            case "ALL ON":
                                kar[(byte)LIGHTING1.cmnd] = (byte)LIGHTING1.sAllOn;
                                osae.ObjectStateSet(obj.Name, "ON");
                                break;
                            case "CHIME":
                                kar[(byte)LIGHTING1.cmnd] = (byte)LIGHTING1.sChime;
                                kar[(byte)LIGHTING1.unitcode] = 8;
                                break;
                        }
                        kar[(byte)LIGHTING1.filler] = 0;

                        WriteCom(kar);
                        string command = "";
                        foreach (byte bt in kar)
                        {
                            command += ("0" + bt.ToString()).Substring(("0" + bt.ToString()).Length - 2) + " ";
                        }
                        logging.AddToLog("Lighting1 command:" + command, false);

                        break;
                    #endregion

                    #region Lighting 2

                    case "AC DIMMER SWITCH":
                    case "HEU DIMMER SWITCH":
                    case "ANSLUT DIMMER SWITCH":
                        logging.AddToLog("Executing Lighting2 command", false);

                        kar = new byte[(byte)LIGHTING2.size + 1];
                        string[] addr = obj.Address.Split('-');

                        byte type_l2 = 0;

                        switch (obj.Type)
                        {
                            case "AC DIMMER SWITCH":
                                type_l2 = (byte)LIGHTING2.sTypeAC;
                                break;
                            case "HEU DIMMER SWITCH":
                                type_l2 = (byte)LIGHTING2.sTypeHEU;
                                break;
                            case "ANSLUT DIMMER SWITCH":
                                type_l2 = (byte)LIGHTING2.sTypeANSLUT;
                                break;
                        }
                        
                        kar[(byte)LIGHTING2.packetlength] = (byte)LIGHTING2.size;
                        kar[(byte)LIGHTING2.packettype] = (byte)LIGHTING2.pType;
                        kar[(byte)LIGHTING2.subtype] = type_l2;
                        kar[(byte)LIGHTING2.seqnbr] = bytSeqNbr;
                        kar[(byte)LIGHTING2.id1] = (byte)Int32.Parse(addr[0], System.Globalization.NumberStyles.HexNumber);
                        kar[(byte)LIGHTING2.id2] = (byte)Int32.Parse(addr[1], System.Globalization.NumberStyles.HexNumber);
                        kar[(byte)LIGHTING2.id3] = (byte)Int32.Parse(addr[2], System.Globalization.NumberStyles.HexNumber);
                        kar[(byte)LIGHTING2.id4] = (byte)Int32.Parse(addr[3], System.Globalization.NumberStyles.HexNumber);
                        kar[(byte)LIGHTING2.unitcode] = (byte)Int32.Parse(addr[4]);


                        switch (method.MethodName)
                        {
                            case "OFF":
                                kar[(byte)LIGHTING2.cmnd] = (byte)LIGHTING2.sOff;
                                osae.ObjectStateSet(obj.Name, "OFF");
                                break;
                            case "ON":
                                if (method.Parameter1 != "")
                                {
                                    kar[(byte)LIGHTING2.cmnd] = (byte)LIGHTING2.sOn;
                                    kar[(byte)LIGHTING2.level] = (byte)0;
                                }
                                else
                                {
                                    kar[(byte)LIGHTING2.cmnd] = (byte)LIGHTING2.sOn;
                                    //kar[(byte)LIGHTING2.level] = (byte)Math.Round((double)Int32.Parse(method.Parameter1) / 7, 0);
                                }
                                osae.ObjectStateSet(obj.Name, "ON");

                                break;
                        }

                        kar[(byte)LIGHTING2.filler] = 0;

                        logging.AddToLog("Writing command. len: " + kar.Length.ToString(), false);
                        WriteCom(kar);
                        string command_l2 = "";
                        foreach (byte bt in kar)
                        {
                            command_l2 += ("0" + bt.ToString("X")).Substring(("0" + bt.ToString("X")).Length - 2) + " ";
                        }
                        logging.AddToLog("Lighting2 command:" + command_l2, false);
                        break;

                    #endregion

                    #region Lighting 5

                    case "LIGHTWAVERF DIMMER SWITCH":
                    case "EMW100 BINARY SWITCH":
                        logging.AddToLog("Executing Lighting5 command", false);

                        kar = new byte[(byte)LIGHTING5.size + 1];
                        logging.AddToLog("Lighting 5 device", false);

                        if (bytFWversion < 29)
                        {
                            logging.AddToLog("RFXtrx433 firmware version must be > 28, flash your RFXtrx433 with the latest firmware", true);
                            return;
                        }

                        string[] l5_addr = obj.Address.Split('-');
                        if (l5_addr.Length != 4)
                        {
                            logging.AddToLog("invalid unit address", true);
                            break;
                        }
                        else
                        {
                            byte subtype = (byte)0;
                            if (obj.Type == "LIGHTWAVERF DIMMER SWITCH")
                            {
                                subtype = (byte)0;
                            }
                            else if (obj.Type == "EMW100 BINARY SWITCH")
                            {
                                subtype = (byte)1;
                            }
                            kar[(byte)LIGHTING5.packetlength] = (byte)LIGHTING5.size;
                            logging.AddToLog("kar[(byte)LIGHTING5.packetlength]: " + kar[(byte)LIGHTING5.packetlength].ToString(), false);
                            kar[(byte)LIGHTING5.packettype] = (byte)LIGHTING5.pType;
                            logging.AddToLog("kar[(byte)LIGHTING5.packettype]: " + kar[(byte)LIGHTING5.packettype].ToString(), false);
                            kar[(byte)LIGHTING5.subtype] = subtype;
                            logging.AddToLog("kar[(byte)LIGHTING5.subtype]: " + subtype.ToString(), false);
                            kar[(byte)LIGHTING5.seqnbr] = bytSeqNbr;
                            logging.AddToLog("kar[(byte)LIGHTING5.seqnbr]: " + bytSeqNbr.ToString(), false);
                            kar[(byte)LIGHTING5.id1] = (byte)Int32.Parse(l5_addr[0]);
                            logging.AddToLog("kar[(byte)LIGHTING5.id1]: " + l5_addr[0], false);
                            kar[(byte)LIGHTING5.id2] = (byte)Int32.Parse(l5_addr[1]);
                            logging.AddToLog("kar[(byte)LIGHTING5.id2]: " + l5_addr[1], false);
                            kar[(byte)LIGHTING5.id3] = (byte)Int32.Parse(l5_addr[2]);
                            logging.AddToLog("kar[(byte)LIGHTING5.id3]: " + l5_addr[2], false);
                            kar[(byte)LIGHTING5.unitcode] = (byte)Int32.Parse(l5_addr[3]);
                            logging.AddToLog("kar[(byte)LIGHTING5.unitcode]: " + l5_addr[3], false);

                            switch (method.MethodName)
                            {
                                case "OFF":
                                    kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sOff;
                                    logging.AddToLog("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString(), false);
                                    osae.ObjectStateSet(obj.Name, "OFF");
                                    break;
                                case "ON":
                                    if (method.Parameter1 == "")
                                    {
                                        kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sOn;
                                        logging.AddToLog("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString(), false);
                                        kar[(byte)LIGHTING5.level] = 0;
                                        logging.AddToLog("kar[(byte)LIGHTING5.level]: " + kar[(byte)LIGHTING5.level].ToString(), false);
                                    }
                                    else
                                    {
                                        kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sSetLevel;
                                        logging.AddToLog("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString(), false);
                                        kar[(byte)LIGHTING5.level] = (byte)Math.Round((double)Int32.Parse(method.Parameter1) / 3, 0);
                                        logging.AddToLog("kar[(byte)LIGHTING5.level]: " + kar[(byte)LIGHTING5.level].ToString(), false);
                                    }
                                    osae.ObjectStateSet(obj.Name, "ON");

                                    break;
                            }

                            kar[(byte)LIGHTING5.filler] = 0;
                            logging.AddToLog("kar[(byte)LIGHTING5.filler]: " + kar[(byte)LIGHTING5.filler].ToString(), false);

                            //not used commands
                            if (kar[(byte)LIGHTING5.cmnd] == 8 | kar[(byte)LIGHTING5.cmnd] == 9)
                            {
                                logging.AddToLog("not used command", true);
                                return;
                            }

                            if (kar[(byte)LIGHTING5.id1] == 0 & kar[(byte)LIGHTING5.id2] == 0 & kar[(byte)LIGHTING5.id3] == 0)
                            {
                                logging.AddToLog("invalid unit address", true);
                                return;
                            }
                            logging.AddToLog("Writing command to port", true);
                            WriteCom(kar);
                            logging.AddToLog("Lighting5 command:", true);
                            string command_l5 = "";
                            foreach (byte bt in kar)
                            {
                                command_l5 += ("0" + bt.ToString()).Substring(("0" + bt.ToString()).Length - 2) + " ";
                            }
                            logging.AddToLog("Lighting5 command:" + command_l5, true);

                        }
                        break;

                    #endregion

                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error processing command: " + ex.Message, true);
            }
            logging.AddToLog("-----------------------------------------------", false);
        }

        public override void RunInterface(string pluginName)
        {
            logging.AddToLog("Plugin version: 0.2.8", true);
            pName = pluginName;
            RSInit("COM" + osae.GetObjectPropertyValue(pluginName,"Port").Value, 38400);
            if(RSOpen())
                gRecComPortEnabled = true;

            recbuf[0] = 0;
            maxticks = 0;

            tmrRead.Elapsed += new System.Timers.ElapsedEventHandler(tmrRead_Tick);
            tmrRead.Enabled = true;

            RESETtrx();

        }

        public override void Shutdown()
        {

        }

        public bool RSOpen()
        {
            try
            {
                RS232Port.Open();
                if (RS232Port.IsOpen)
                {
                    GC.SuppressFinalize(RS232Port.BaseStream);
                    RS232Port.DtrEnable = true;
                    RS232Port.RtsEnable = true;
                    RS232Port.DiscardInBuffer();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RSInit(string comport, int baudrate)
        {
            try
            {
                if (RS232Port.IsOpen)
                {
                    RS232Port.Close();
                }

                RS232Port.PortName = comport;
                RS232Port.BaudRate = baudrate;
                RS232Port.Parity = Parity.None;
                RS232Port.DataBits = 8;
                //.Encoding = Encoding.GetEncoding(1252)  'Extended ASCII (8-bits)
                RS232Port.StopBits = StopBits.One;
                RS232Port.Handshake = Handshake.None;
                RS232Port.ReadBufferSize = Convert.ToInt32(4096);
                //.ReceivedBytesThreshold = 1
                //.ReadTimeout = 100
                RS232Port.WriteTimeout = 500;
                return true;

            }
            catch (Exception pEx)
            {
                // handle exception
                return false;
            }

        }

        public void RESETtrx()
        {
            SendCommand((byte)ICMD.RESET, "Reset receiver/transceiver:");
            Resettimer = 5;
            //5 * 100ms = ignore data for 0.5 seconds
        }

        public void SendCommand(byte command, string message)
        {
            string msgStr = "";
            byte[] kar = new byte[(byte)ICMD.size + 1];

            kar[(byte)ICMD.packetlength] = (byte)ICMD.size;
            kar[(byte)ICMD.packettype] = (byte)ICMD.pType;
            kar[(byte)ICMD.subtype] = (byte)ICMD.sType;
            kar[(byte)ICMD.seqnbr] = bytSeqNbr;
            kar[(byte)ICMD.cmnd] = command;
            kar[(byte)ICMD.msg1] = 0;
            kar[(byte)ICMD.msg2] = 0;
            kar[(byte)ICMD.msg3] = 0;
            kar[(byte)ICMD.msg4] = 0;
            kar[(byte)ICMD.msg5] = 0;
            kar[(byte)ICMD.msg6] = 0;
            kar[(byte)ICMD.msg7] = 0;
            kar[(byte)ICMD.msg8] = 0;
            kar[(byte)ICMD.msg9] = 0;

            logging.AddToLog("================================================", false);
            logging.AddToLog(message, false);
            foreach (byte bt in kar)
            {
                msgStr += ("0" + bt.ToString()).Substring(("0" + bt.ToString()).Length - 2, 2) + " ";
            }
            logging.AddToLog(msgStr, false);
            WriteCom(kar);
        }

        private void WriteCom(byte[] kar)
        {
            if (tcp == true)
            {
                //try
                //{
                //    // Send the message to the connected TcpServer. 
                //    stream.Write(kar, 0, kar.Length);
                //    bytSeqNbr = bytSeqNbr + 1;
                //}
                //catch (Exception ex)
                //{
                //    // Warn the user.
                //    MessageBox.Show("Unable to write to TCP port");
                //}
            }
            else
            {
                try
                {
                    // Enable the timer.
                    // Write an user specified Command to the Port.
                    RSTxData(kar, kar.Length);
                    bytSeqNbr += 1;
                }
                catch (Exception ex)
                {
                    // Warn the user.
                    logging.AddToLog("Unable to write to COM port", true);
                }
            }
        }

        public bool RSTxData(byte[] buffer, int intLength)
        {
            try
            {
                RS232Port.Write(buffer, 0, intLength);
            }
            catch (Exception pEx)
            {
                return false;
            }
            return true;
        }

        private void tmrRead_Tick(System.Object sender, System.EventArgs e)
        {
            //COM port receive polling added instead of receive interrupt
            while (gRecComPortEnabled & (RSBytesToRead() > 0))
            {
                ProcessReceivedChar((byte)RSReadByte());
            }
            //end of receive polling

            if (Resettimer == 0)
            {
                //one or more bytes received
                if (recbytes != 0)
                {
                    maxticks += 1;
                    //flush buffer due to 400ms timeout
                    if (maxticks > 3)
                    {
                        maxticks = 0;
                        recbytes = 0;
                        logging.AddToLog(" Buffer flushed due to timeout", true);
                    }
                }
            }
            else
            {
                Resettimer = Resettimer - 1;
                // decrement resettimer
                if (Resettimer == 0)
                {
                    if (gRecComPortEnabled)
                    {
                        RSDiscardInBuffer();
                    }
                    else
                    {
                        //stream.Flush() 'flush not yet supported
                    }
                    GetStatus();
                }
            }

        }

        public void GetStatus()
        {
            SendCommand((byte)ICMD.STATUS, "Get Status:");
            maxticks = 0;
        }

        public bool RSDiscardInBuffer()
        {
            try
            {
                RS232Port.DiscardInBuffer();
                return true;
            }
            catch (Exception pEx)
            {
                return false;
            }
        }


        public int RSBytesToRead()
        {
            try
            {
                if (RS232Port.IsOpen)
                {
                    return RS232Port.BytesToRead;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception pEx)
            {
                return -1;
            }
        }

        public int RSReadByte()
        {
            try
            {
                return RS232Port.ReadByte();
            }
            catch (TimeoutException pEx)
            {
                return -2;
            }
            catch (Exception pEx)
            {
                return -1;
            }
        }


        public void ProcessReceivedChar(byte sComChar)
        {
            
            if (Resettimer != 0)
            {
                return;
                //ignore received characters after a reset cmd until resettimer = 0
            }

            maxticks = 0;
            //reset receive timeout

            //1st char of a packet received
            if (recbytes == 0)
            {
                logging.AddToLog("------------------------------------------------", false);
                if (sComChar == 0)
                {
                    return;
                    //ignore 1st char if 00
                }
            }

            recbuf[recbytes] = sComChar;
            //store received char
            recbytes += 1;
            //increment char counter

            //all bytes of the packet received?
            if (recbytes > recbuf[0])
            {
                rcvdStr += " " + ("0" + sComChar.ToString()).Substring(("0" + sComChar.ToString()).Length - 2, 2);
                logging.AddToLog(rcvdStr, false);
                rcvdStr = "";
                // Write the output to the screen.
                decode_messages();
                //decode message
                recbytes = 0;
                //set to zero to receive next message
            }
            else
            {
                rcvdStr += " " + ("0" + sComChar.ToString()).Substring(("0" + sComChar.ToString()).Length - 2, 2);
                // Write the output to the screen.
            }
        }

        #region "Decode messages"
        public void decode_messages()
        {
            logging.AddToLog("---------------Received Message----------------", false);
            switch (recbuf[1])
            {
                case (byte)IRESPONSE.pType:
                    logging.AddToLog("Packettype        = Interface Message", false);
                    decode_InterfaceMessage();

                    break;
                case (byte)RXRESPONSE.pType:
                    logging.AddToLog("Packettype        = Receiver/Transmitter Message", false);
                    decode_RecXmitMessage();

                    break;
                case (byte)UNDECODED.pType:
                    logging.AddToLog("Packettype        = UNDECODED RF Message", false);
                    decode_UNDECODED();

                    break;
                //case (byte)LIGHTING1.pType:
                //    logging.AddToLog("Packettype    = Lighting1", false);
                //    decode_Lighting1();

                //    break;
                case (byte)LIGHTING2.pType:
                    logging.AddToLog("Packettype    = Lighting2", false);
                    decode_Lighting2();

                    break;
                //case (byte)LIGHTING3.pType:
                //    logging.AddToLog("Packettype    = Lighting3", false);
                //    decode_Lighting3();

                //    break;
                //case (byte)LIGHTING4.pType:
                //    logging.AddToLog("Packettype    = Lighting4", false);
                //    decode_Lighting4();

                //    break;
                case (byte)LIGHTING5.pType:
                    logging.AddToLog("Packettype    = Lighting5", false);
                    decode_Lighting5();

                    break;
                //case (byte)SECURITY1.pType:
                //    logging.AddToLog("Packettype    = Security1", false);
                //    decode_Security1();

                //    break;
                //case (byte)CAMERA1.pType:
                //    logging.AddToLog("Packettype    = Camera1", false);
                //    decode_Camera1();

                //    break;
                //case (byte)REMOTE.pType:
                //    logging.AddToLog("Packettype    = Remote control & IR", false);
                //    decode_Remote();

                //    break;
                //case (byte)THERMOSTAT1.pType:
                //    logging.AddToLog("Packettype    = Thermostat1", false);
                //    decode_Thermostat1();

                //    break;
                //case (byte)THERMOSTAT2.pType:
                //    logging.AddToLog("Packettype    = Thermostat2", false);
                //    decode_Thermostat2();

                //    break;
                //case (byte)THERMOSTAT3.pType:
                //    logging.AddToLog("Packettype    = Thermostat3", false);
                //    decode_Thermostat3();

                //    break;
                case (byte)TEMP.pType:
                    logging.AddToLog("Packettype    = TEMP", false);
                    decode_Temp();

                    break;
                case (byte)HUM.pType:
                    logging.AddToLog("Packettype    = HUM", false);
                    decode_Hum();

                    break;
                case (byte)TEMP_HUM.pType:
                    logging.AddToLog("Packettype    = TEMP_HUM", false);
                    decode_TempHum();

                    break;
                case (byte)BARO.pType:
                    logging.AddToLog("Packettype    = BARO", false);
                    decode_Baro();

                    break;
                case (byte)TEMP_HUM_BARO.pType:
                    logging.AddToLog("Packettype    = TEMP_HUM_BARO", false);
                    decode_TempHumBaro();

                    break;
                case (byte)RAIN.pType:
                    logging.AddToLog("Packettype    = RAIN", false);
                    decode_Rain();

                    break;
                case (byte)WIND.pType:
                    logging.AddToLog("Packettype    = WIND", false);
                    decode_Wind();

                    break;
                case (byte)UV.pType:
                    logging.AddToLog("Packettype    = UV", false);
                    decode_UV();

                    break;
                case (byte)DT.pType:
                    logging.AddToLog("Packettype    = DT", false);
                    decode_DateTime();

                    break;
                case (byte)CURRENT.pType:
                    logging.AddToLog("Packettype    = CURRENT", false);
                    decode_Current();

                    break;
                case (byte)ENERGY.pType:
                    logging.AddToLog("Packettype    = ENERGY", false);
                    decode_Energy();

                    break;
                case (byte)GAS.pType:
                    logging.AddToLog("Packettype    = GAS", false);
                    decode_Gas();

                    break;
                case (byte)WATER.pType:
                    logging.AddToLog("Packettype    = WATER", false);
                    decode_Water();

                    break;
                case (byte)WEIGHT.pType:
                    logging.AddToLog("Packettype    = WEIGHT", false);
                    decode_Weight();

                    break;
                //case (byte)RFXSENSOR.pType:
                //    logging.AddToLog("Packettype    = RFXSensor", false);
                //    decode_RFXSensor();

                //    break;
                //case (byte)RFXMETER.pType:
                //    logging.AddToLog("Packettype    = RFXMeter", false);
                //    decode_RFXMeter();

                //    break;
                //case (byte)FS20.pType:
                //    logging.AddToLog("Packettype    = FS20", false);
                //    decode_FS20();

                //    break;
                default:
                    logging.AddToLog("ERROR: Unknown Packet type:" + recbuf[1].ToString(), true);
                    break;
            }
            logging.AddToLog("-----------------------------------------------", false);
        }

        public void decode_InterfaceMessage()
        {
            switch (recbuf[(byte)IRESPONSE.subtype])
            {
                case (byte)IRESPONSE.sType:
                    logging.AddToLog("subtype           = Interface Response", false);
                    logging.AddToLog("Sequence nbr      = " + recbuf[(byte)IRESPONSE.seqnbr].ToString(), false);
                    switch (recbuf[(byte)IRESPONSE.cmnd])
                    {
                        case (byte)ICMD.STATUS:
                        case (byte)ICMD.SETMODE:
                        case (byte)ICMD.sel310:
                        case (byte)ICMD.sel315:
                        case (byte)ICMD.sel800:
                        case (byte)ICMD.sel800F:
                        case (byte)ICMD.sel830:
                        case (byte)ICMD.sel830F:
                        case (byte)ICMD.sel835:
                        case (byte)ICMD.sel835F:
                        case (byte)ICMD.sel895:
                            logging.AddToLog("response on cmnd  = ", false);
                            switch (recbuf[(byte)IRESPONSE.cmnd])
                            {
                                case (byte)ICMD.STATUS:
                                    logging.AddToLog("Get Status", false);
                                    break;
                                case (byte)ICMD.SETMODE:
                                    logging.AddToLog("Set Mode", false);
                                    break;
                                case (byte)ICMD.sel310:
                                    logging.AddToLog("Select 310MHz", false);
                                    break;
                                case (byte)ICMD.sel315:
                                    logging.AddToLog("Select 315MHz", false);
                                    break;
                                case (byte)ICMD.sel800:
                                    logging.AddToLog("Select 868.00MHz", false);
                                    break;
                                case (byte)ICMD.sel800F:
                                    logging.AddToLog("Select 868.00MHz FSK", false);
                                    break;
                                case (byte)ICMD.sel830:
                                    logging.AddToLog("Select 868.30MHz", false);
                                    break;
                                case (byte)ICMD.sel830F:
                                    logging.AddToLog("Select 868.30MHz FSK", false);
                                    break;
                                case (byte)ICMD.sel835:
                                    logging.AddToLog("Select 868.35MHz", false);
                                    break;
                                case (byte)ICMD.sel835F:
                                    logging.AddToLog("Select 868.35MHz FSK", false);
                                    break;
                                case (byte)ICMD.sel895:
                                    logging.AddToLog("Select 868.95MHz", false);
                                    break;
                                default:
                                    logging.AddToLog("Error: unknown response", false);
                                    break;
                            }
                            switch (recbuf[(byte)IRESPONSE.msg1])
                            {
                                case (byte)IRESPONSE.recType310:
                                    logging.AddToLog("Transceiver type  = 310MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType315:
                                    logging.AddToLog("Receiver type     = 315MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType43392:
                                    logging.AddToLog("Receiver type     = 433.92MHz (receive only)", false);
                                    break;
                                case (byte)IRESPONSE.trxType43392:
                                    logging.AddToLog("Transceiver type  = 433.92MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType86800:
                                    logging.AddToLog("Receiver type     = 868.00MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType86800FSK:
                                    logging.AddToLog("Receiver type     = 868.00MHz FSK", false);
                                    break;
                                case (byte)IRESPONSE.recType86830:
                                    logging.AddToLog("Receiver type     = 868.30MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType86830FSK:
                                    logging.AddToLog("Receiver type     = 868.30MHz FSK", false);
                                    break;
                                case (byte)IRESPONSE.recType86835:
                                    logging.AddToLog("Receiver type     = 868.35MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType86835FSK:
                                    logging.AddToLog("Receiver type     = 868.35MHz FSK", false);
                                    break;
                                case (byte)IRESPONSE.recType86895:
                                    logging.AddToLog("Receiver type     = 868.95MHz", false);
                                    break;
                                default:
                                    logging.AddToLog("Receiver type     = unknown", false);
                                    break;
                            }
                            trxType = recbuf[(byte)IRESPONSE.msg1];
                            logging.AddToLog("Firmware version  = " + recbuf[(byte)IRESPONSE.msg2].ToString(), false);
                            bytFWversion = recbuf[(byte)IRESPONSE.msg2];

                            if ((recbuf[(byte)IRESPONSE.msg3] & (byte)IRESPONSE.msg3_undec) != 0)
                            {
                                logging.AddToLog("Undec             on", false);
                            }
                            else
                            {
                                logging.AddToLog("Undec             off", false);
                            }

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_X10) != 0)
                            {
                                logging.AddToLog("X10               enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("X10               disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_ARC) != 0)
                            {
                                logging.AddToLog("ARC               enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("ARC               disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_AC) != 0)
                            {
                                logging.AddToLog("AC                enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("AC                disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_HEU) != 0)
                            {
                                logging.AddToLog("HomeEasy EU       enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("HomeEasy EU       disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_KOP) != 0)
                            {
                                logging.AddToLog("Ikea Koppla       enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("Ikea Koppla       disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_OREGON) != 0)
                            {
                                logging.AddToLog("Oregon Scientific enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("Oregon Scientific disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_ATI) != 0)
                            {
                                logging.AddToLog("ATI               enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("ATI               disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_VISONIC) != 0)
                            {
                                logging.AddToLog("Visonic           enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("Visonic           disabled", false);
                            }

                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_MERTIK) != 0)
                            {
                                logging.AddToLog("Mertik            enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("Mertik            disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_AD) != 0)
                            {
                                logging.AddToLog("AD                enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("AD                disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_HID) != 0)
                            {
                                logging.AddToLog("Hideki            enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("Hideki            disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_LCROS) != 0)
                            {
                                logging.AddToLog("La Crosse         enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("La Crosse         disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_FS20) != 0)
                            {
                                logging.AddToLog("FS20              enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("FS20              disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_PROGUARD) != 0)
                            {
                                logging.AddToLog("ProGuard          enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("ProGuard          disabled", false);
                            }

                            if ((recbuf[(byte)IRESPONSE.msg4] & 0x80) != 0)
                            {
                                logging.AddToLog("RFU protocol 7    enabled", false);
                            }
                            else
                            {
                                logging.AddToLog("RFU protocol 7    disabled", false);
                            }

                            break;
                        case (byte)ICMD.ENABLEALL:
                            logging.AddToLog("response on cmnd  = Enable All RF", false);
                            break;
                        case (byte)ICMD.UNDECODED:
                            logging.AddToLog("response on cmnd  = UNDECODED on", false);
                            break;
                        case (byte)ICMD.SAVE:
                            logging.AddToLog("response on cmnd  = Save", false);
                            break;
                        case (byte)ICMD.DISX10:
                            logging.AddToLog("response on cmnd  = Disable X10 RF", false);
                            break;
                        case (byte)ICMD.DISARC:
                            logging.AddToLog("response on cmnd  = Disable ARC RF", false);
                            break;
                        case (byte)ICMD.DISAC:
                            logging.AddToLog("response on cmnd  = Disable AC RF", false);
                            break;
                        case (byte)ICMD.DISHEU:
                            logging.AddToLog("response on cmnd  = Disable HomeEasy EU RF", false);
                            break;
                        case (byte)ICMD.DISKOP:
                            logging.AddToLog("response on cmnd  = Disable Ikea Koppla RF", false);
                            break;
                        case (byte)ICMD.DISOREGON:
                            logging.AddToLog("response on cmnd  = Disable Oregon Scientific RF", false);
                            break;
                        case (byte)ICMD.DISATI:
                            logging.AddToLog("response on cmnd  = Disable ATI remote RF", false);
                            break;
                        case (byte)ICMD.DISVISONIC:
                            logging.AddToLog("response on cmnd  = Disable Visonic RF", false);
                            break;
                        case (byte)ICMD.DISMERTIK:
                            logging.AddToLog("response on cmnd  = Disable Mertik RF", false);
                            break;
                        case (byte)ICMD.DISAD:
                            logging.AddToLog("response on cmnd  = Disable AD RF", false);
                            break;
                        case (byte)ICMD.DISHID:
                            logging.AddToLog("response on cmnd  = Disable Hideki RF", false);
                            break;
                        case (byte)ICMD.DISLCROS:
                            logging.AddToLog("response on cmnd  = Disable La Crosse RF", false);

                            break;
                        //For internal use by RFXCOM only, do not use this coding.
                        //=========================================================
                        case 0x8:
                            logging.AddToLog("response on cmnd  = T1", false);
                            if (recbuf[(byte)IRESPONSE.msg9] == 0)
                            {
                                logging.AddToLog("Not OK!", false);
                            }
                            else
                            {
                                logging.AddToLog("On", false);
                            }

                            break;
                        case 0x9:
                            logging.AddToLog("response on cmnd  = T2", false);
                            if (recbuf[(byte)IRESPONSE.msg9] == 0)
                            {
                                logging.AddToLog("Not OK!", false);
                            }
                            else
                            {
                                logging.AddToLog("Blk On", false);
                            }
                            break;
                        //=========================================================

                        default:
                            logging.AddToLog("ERROR: Unexpected response for Packet type=" + recbuf[(byte)IRESPONSE.packettype].ToString() + ", Sub type=" + recbuf[(byte)IRESPONSE.subtype].ToString() + " cmnd=" + recbuf[(byte)IRESPONSE.cmnd].ToString(), false);
                            break;
                    }
                    break;
            }
        }

        public void decode_RecXmitMessage()
        {
            switch (recbuf[(byte)RXRESPONSE.subtype])
            {
                case (byte)RXRESPONSE.sTypeReceiverLockError:
                    logging.AddToLog("subtype           = Receiver lock error", false);
                    //SystemSounds.Asterisk.Play();
                    logging.AddToLog("Sequence nbr      = " + recbuf[(byte)RXRESPONSE.seqnbr].ToString(), false);

                    break;
                case (byte)RXRESPONSE.sTypeTransmitterResponse:
                    logging.AddToLog("subtype           = Transmitter Response", false);
                    logging.AddToLog("Sequence nbr      = " + recbuf[(byte)RXRESPONSE.seqnbr].ToString(), false);
                    switch (recbuf[(byte)RXRESPONSE.msg])
                    {
                        case 0x0:
                            logging.AddToLog("response          = ACK, data correct transmitted", false);
                            break;
                        case 0x1:
                            logging.AddToLog("response          = ACK, but transmit started after 6 seconds delay anyway with RF receive data detected", false);
                            break;
                        case 0x2:
                            logging.AddToLog("response          = NAK, transmitter did not lock on the requested transmit frequency", false);
                            //SystemSounds.Asterisk.Play();
                            break;
                        case 0x3:
                            logging.AddToLog("response          = NAK, AC address zero in id1-id4 not allowed", false);
                            //SystemSounds.Asterisk.Play();
                            break;
                        default:
                            logging.AddToLog("ERROR: Unexpected message type=" + recbuf[(byte)RXRESPONSE.msg].ToString(), false);
                            break;
                    }
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)RXRESPONSE.packettype].ToString() + ": " + recbuf[(byte)RXRESPONSE.subtype].ToString(), false);
                    break;
            }
        }

        public void decode_UNDECODED()
        {
            logging.AddToLog("UNDECODED ", false);
            switch (recbuf[(byte)UNDECODED.subtype])
            {
                case (byte)UNDECODED.sTypeUac:
                    logging.AddToLog("AC:", false);
                    break;
                case (byte)UNDECODED.sTypeUarc:
                    logging.AddToLog("ARC:", false);
                    break;
                case (byte)UNDECODED.sTypeUati:
                    logging.AddToLog("ATI:", false);
                    break;
                case (byte)UNDECODED.sTypeUhideki:
                    logging.AddToLog("HIDEKI:", false);
                    break;
                case (byte)UNDECODED.sTypeUlacrosse:
                    logging.AddToLog("LACROSSE:", false);
                    break;
                case (byte)UNDECODED.sTypeUlwrf:
                    logging.AddToLog("LWRF:", false);
                    break;
                case (byte)UNDECODED.sTypeUmertik:
                    logging.AddToLog("MERTIK:", false);
                    break;
                case (byte)UNDECODED.sTypeUoregon1:
                    logging.AddToLog("OREGON1:", false);
                    break;
                case (byte)UNDECODED.sTypeUoregon2:
                    logging.AddToLog("OREGON2:", false);
                    break;
                case (byte)UNDECODED.sTypeUoregon3:
                    logging.AddToLog("OREGON3:", false);
                    break;
                case (byte)UNDECODED.sTypeUproguard:
                    logging.AddToLog("PROGUARD:", false);
                    break;
                case (byte)UNDECODED.sTypeUvisonic:
                    logging.AddToLog("VISONIC:", false);
                    break;
                case (byte)UNDECODED.sTypeUnec:
                    logging.AddToLog("NEC:", false);
                    break;
                case (byte)UNDECODED.sTypeUfs20:
                    logging.AddToLog("FS20:", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)UNDECODED.packettype] + ": " + recbuf[(byte)UNDECODED.subtype], false);
                    break;
            }
            for (int i = 0; i <= recbuf[(byte)UNDECODED.packetlength] - (byte)UNDECODED.msg1; i++)
            {
                logging.AddToLog("0" + recbuf[(byte)UNDECODED.msg1 + i], false);
            }

        }

        //public void decode_Lighting1()
        //{
        //    switch (recbuf(LIGHTING1.subtype))
        //    {
        //        case LIGHTING1.sTypeX10:
        //            logging.AddToLog("subtype       = X10");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            logging.AddToLog("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            logging.AddToLog("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            logging.AddToLog("Command       = ", false);
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    logging.AddToLog("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    logging.AddToLog("On");
        //                    break;
        //                case LIGHTING1.sDim:
        //                    logging.AddToLog("Dim");
        //                    break;
        //                case LIGHTING1.sBright:
        //                    logging.AddToLog("Bright");
        //                    break;
        //                case LIGHTING1.sAllOn:
        //                    logging.AddToLog("All On");
        //                    break;
        //                case LIGHTING1.sAllOff:
        //                    logging.AddToLog("All Off");
        //                    break;
        //                case LIGHTING1.sChime:
        //                    logging.AddToLog("Chime");
        //                    break;
        //                default:
        //                    logging.AddToLog("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        case LIGHTING1.sTypeARC:
        //            logging.AddToLog("subtype       = ARC");
        //            logging.AddToLog("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            logging.AddToLog("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            logging.AddToLog("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            logging.AddToLog("Command       = ", false);
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    logging.AddToLog("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    logging.AddToLog("On");
        //                    break;
        //                case LIGHTING1.sAllOn:
        //                    logging.AddToLog("All On");
        //                    break;
        //                case LIGHTING1.sAllOff:
        //                    logging.AddToLog("All Off");
        //                    break;
        //                default:
        //                    logging.AddToLog("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        case LIGHTING1.sTypeAB400D:
        //            logging.AddToLog("subtype       = ELRO AB400");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            logging.AddToLog("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            logging.AddToLog("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            logging.AddToLog("Command       = ", false);
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    logging.AddToLog("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    logging.AddToLog("On");
        //                    break;
        //                default:
        //                    logging.AddToLog("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(LIGHTING1.packettype)) + ": " + Conversion.Hex(recbuf(LIGHTING1.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Signal level  = " + (recbuf(LIGHTING1.rssi) >> 4).ToString());
        //}

        public void decode_Lighting2()
        {
            logging.AddToLog("Recieved Lighting2 Message.  Type: " + recbuf[(byte)LIGHTING2.subtype].ToString(), false);
            OSAEObject obj = new OSAEObject(); 
            
            switch (recbuf[(byte)LIGHTING2.subtype])
            {
                case (byte)LIGHTING2.sTypeAC:
                case (byte)LIGHTING2.sTypeHEU:
                case (byte)LIGHTING2.sTypeANSLUT:
                    //logging.AddToLog("id1: " + recbuf[(byte)LIGHTING2.id1].ToString(), true);
                    //logging.AddToLog("id2: " + recbuf[(byte)LIGHTING2.id2].ToString(), true);
                    //logging.AddToLog("id3: " + recbuf[(byte)LIGHTING2.id3].ToString(), true);
                    //logging.AddToLog("id4: " + recbuf[(byte)LIGHTING2.id4].ToString(), true);
                    //logging.AddToLog("uc: " + recbuf[(byte)LIGHTING2.unitcode].ToString(), true);



                    obj = osae.GetObjectByAddress((recbuf[(byte)LIGHTING2.id1].ToString("X") +
                        "-" + recbuf[(byte)LIGHTING2.id2].ToString("X") +
                        "-" + recbuf[(byte)LIGHTING2.id3].ToString("X") +
                        "-" + recbuf[(byte)LIGHTING2.id4].ToString("X") +
                        "-" + recbuf[(byte)LIGHTING2.unitcode].ToString()));
                    

                    
                        
                    switch (recbuf[(byte)LIGHTING2.subtype])
                    {
                        case (byte)LIGHTING2.sTypeAC:
                            logging.AddToLog("subtype       = AC", false);
                            break;
                        case (byte)LIGHTING2.sTypeHEU:
                            logging.AddToLog("subtype       = HomeEasy EU", false);
                            break;
                        case (byte)LIGHTING2.sTypeANSLUT:
                            logging.AddToLog("subtype       = ANSLUT", false);
                            break;
                    }
                    logging.AddToLog("Sequence nbr  = " + recbuf[(byte)LIGHTING2.seqnbr].ToString(), false);
                    //logging.AddToLog("ID - Unit            = " + address, false);
                    logging.AddToLog("Unit          = " + recbuf[(byte)LIGHTING2.unitcode].ToString(), false);
                    switch (recbuf[(byte)LIGHTING2.cmnd])
                    {
                        case (byte)LIGHTING2.sOff:
                            logging.AddToLog("Command       = Off", false);
                            osae.ObjectStateSet(obj.Name, "OFF");
                            break;
                        case (byte)LIGHTING2.sOn:
                            logging.AddToLog("Command       = On", false);
                            osae.ObjectStateSet(obj.Name, "ON");
                            break;
                        case (byte)LIGHTING2.sSetLevel:
                            logging.AddToLog("Set Level:" + recbuf[(byte)LIGHTING2.level].ToString(), false);
                            break;
                        case (byte)LIGHTING2.sGroupOff:
                            logging.AddToLog("Group Off", false);
                            break;
                        case (byte)LIGHTING2.sGroupOn:
                            logging.AddToLog("Group On", false);
                            break;
                        case (byte)LIGHTING2.sSetGroupLevel:
                            logging.AddToLog("Set Group Level:" + recbuf[(byte)LIGHTING2.level].ToString(), false);
                            break;
                        default:
                            logging.AddToLog("UNKNOWN", false);
                            break;
                    }

                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Convert.ToInt32(recbuf[(byte)LIGHTING2.packettype]) + ": " + Convert.ToInt32(recbuf[(byte)LIGHTING2.subtype]), false);
                    break;
            }
            logging.AddToLog("Signal level  = " + (recbuf[(byte)LIGHTING2.rssi] >> 4).ToString(), false);
        }

        //public void decode_Lighting3()
        //{
        //    switch (recbuf(LIGHTING3.subtype))
        //    {
        //        case LIGHTING3.sTypeKoppla:
        //            logging.AddToLog("subtype       = Ikea Koppla");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(LIGHTING3.seqnbr).ToString);
        //            logging.AddToLog("Command       = ", false);
        //            switch (recbuf(LIGHTING3.cmnd))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("Off");
        //                    break;
        //                case 0x1:
        //                    logging.AddToLog("On");
        //                    break;
        //                case 0x20:
        //                    logging.AddToLog("Set Level:" + recbuf(6).ToString);
        //                    break;
        //                case 0x21:
        //                    logging.AddToLog("Program");
        //                    break;
        //                default:
        //                    if (recbuf(LIGHTING3.cmnd) >= 0x10 & recbuf(LIGHTING3.cmnd) < 0x18)
        //                    {
        //                        logging.AddToLog("Dim");
        //                    }
        //                    else if (recbuf(LIGHTING3.cmnd) >= 0x18 & recbuf(LIGHTING3.cmnd) < 0x20)
        //                    {
        //                        logging.AddToLog("Bright");
        //                    }
        //                    else
        //                    {
        //                        logging.AddToLog("UNKNOWN");
        //                    }
        //                    break;
        //            }
        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(LIGHTING3.packettype)) + ": " + Conversion.Hex(recbuf(LIGHTING3.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Signal level  = " + (recbuf(LIGHTING3.rssi) >> 4).ToString());

        //}

        //public void decode_Lighting4()
        //{
        //    logging.AddToLog("Not implemented");
        //}

        public void decode_Lighting5()
        {
            logging.AddToLog("Recieved Lighting5 Message", false);
            OSAEObject obj = new OSAEObject();
            switch (recbuf[(byte)LIGHTING5.subtype])
            {
                case (byte)LIGHTING5.sTypeLightwaveRF:
                    obj = osae.GetObjectByAddress("0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString() + "-0" + recbuf[(byte)LIGHTING5.id3].ToString() + "-" + recbuf[(byte)LIGHTING5.unitcode].ToString()); 
                    logging.AddToLog("subtype       = LightwaveRF", false);
                    logging.AddToLog("Sequence nbr  = " + recbuf[(byte)LIGHTING5.seqnbr].ToString(), false);
                    logging.AddToLog("ID            = " + "0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2] + "-0" + recbuf[(byte)LIGHTING5.id3].ToString(), false);
                    logging.AddToLog("Unit          = " + recbuf[(byte)LIGHTING5.unitcode].ToString(), false);
                    switch (recbuf[(byte)LIGHTING5.cmnd])
                    {
                        case (byte)LIGHTING5.sOff:
                            osae.ObjectStateSet(obj.Name, "OFF");
                            logging.AddToLog("Command       = Off", false);
                            break;
                        case (byte)LIGHTING5.sOn:
                            osae.ObjectStateSet(obj.Name, "ON");
                            logging.AddToLog("Command       = On", false);
                            break;
                        case (byte)LIGHTING5.sGroupOff:
                            logging.AddToLog("Command       = Group Off", false);
                            break;
                        case (byte)LIGHTING5.sMood1:
                            logging.AddToLog("Command       = Group Mood 1", false);
                            break;
                        case (byte)LIGHTING5.sMood2:
                            logging.AddToLog("Command       = Group Mood 2", false);
                            break;
                        case (byte)LIGHTING5.sMood3:
                            logging.AddToLog("Command       = Group Mood 3", false);
                            break;
                        case (byte)LIGHTING5.sMood4:
                            logging.AddToLog("Command       = Group Mood 4", false);
                            break;
                        case (byte)LIGHTING5.sMood5:
                            logging.AddToLog("Command       = Group Mood 5", false);
                            break;
                        case (byte)LIGHTING5.sUnlock:
                            logging.AddToLog("Command       = Unlock", false);
                            break;
                        case (byte)LIGHTING5.sLock:
                            logging.AddToLog("Command       = Lock", false);
                            break;
                        case (byte)LIGHTING5.sAllLock:
                            logging.AddToLog("Command       = All lock", false);
                            break;
                        case (byte)LIGHTING5.sClose:
                            logging.AddToLog("Command       = Close inline relay", false);
                            break;
                        case (byte)LIGHTING5.sStop:
                            logging.AddToLog("Command       = Stop inline relay", false);
                            break;
                        case (byte)LIGHTING5.sOpen:
                            logging.AddToLog("Command       = Open inline relay", false);
                            break;
                        case (byte)LIGHTING5.sSetLevel:
                            logging.AddToLog("Command       = Set dim level to: " + Convert.ToInt32((recbuf[(byte)LIGHTING5.level] * 3.2)).ToString() + "%", false);
                            break;
                        default:
                            logging.AddToLog("UNKNOWN", false);
                            break;
                    }

                    break;
                case (byte)LIGHTING5.sTypeEMW100:
                    obj = osae.GetObjectByAddress("0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString() + "-" + recbuf[(byte)LIGHTING5.unitcode].ToString()); 
                    logging.AddToLog("subtype       = EMW100", false);
                    logging.AddToLog("Sequence nbr  = " + recbuf[(byte)LIGHTING5.seqnbr].ToString(), false);
                    logging.AddToLog("ID            = " + "0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString(), false);
                    logging.AddToLog("Unit          = " + recbuf[(byte)LIGHTING5.unitcode].ToString(),false);
                    switch (recbuf[(byte)LIGHTING5.cmnd])
                    {
                        case (byte)LIGHTING5.sOff:
                            osae.ObjectStateSet(obj.Name, "OFF");
                            logging.AddToLog("Command       = Off", false);
                            break;
                        case (byte)LIGHTING5.sOn:
                            osae.ObjectStateSet(obj.Name, "ON");
                            logging.AddToLog("Command       = On", false);
                            break;
                        case (byte)LIGHTING5.sLearn:
                            logging.AddToLog("Command       = Learn", false);
                            break;
                        default:
                            logging.AddToLog("Command       = UNKNOWN", false);
                            break;
                    }

                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)LIGHTING5.packettype].ToString() + ": " + recbuf[(byte)LIGHTING5.subtype].ToString(), false);
                    break;
            }
            logging.AddToLog("Signal level  = " + (recbuf[(byte)LIGHTING5.rssi] >> 4).ToString(), false);

        }

        //public void decode_Security1()
        //{
        //    switch (recbuf(SECURITY1.subtype))
        //    {
        //        case SECURITY1.SecX10:
        //            logging.AddToLog("subtype       = X10 security");
        //            break;
        //        case SECURITY1.SecX10M:
        //            logging.AddToLog("subtype       = X10 security motion");
        //            break;
        //        case SECURITY1.SecX10R:
        //            logging.AddToLog("subtype       = X10 security remote");
        //            break;
        //        case SECURITY1.KD101:
        //            logging.AddToLog("subtype       = KD101 smoke detector");
        //            break;
        //        case SECURITY1.PowercodeSensor:
        //            logging.AddToLog("subtype       = Visonic PowerCode sensor - primary contact");
        //            break;
        //        case SECURITY1.PowercodeMotion:
        //            logging.AddToLog("subtype       = Visonic PowerCode motion");
        //            break;
        //        case SECURITY1.Codesecure:
        //            logging.AddToLog("subtype       = Visonic CodeSecure");
        //            break;
        //        case SECURITY1.PowercodeAux:
        //            logging.AddToLog("subtype       = Visonic PowerCode sensor - auxiliary contact");
        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(SECURITY1.packettype)) + ": " + Conversion.Hex(recbuf(SECURITY1.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Sequence nbr  = " + recbuf(SECURITY1.seqnbr).ToString);
        //    logging.AddToLog("id1-3         = " + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id2)), 2) + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id3)), 2));
        //    logging.AddToLog("status        = ", false);
        //    switch (recbuf(SECURITY1.status))
        //    {
        //        case SECURITY1.sStatusNormal:
        //            logging.AddToLog("Normal");
        //            break;
        //        case SECURITY1.sStatusNormalDelayed:
        //            logging.AddToLog("Normal Delayed");
        //            break;
        //        case SECURITY1.sStatusAlarm:
        //            logging.AddToLog("Alarm");
        //            break;
        //        case SECURITY1.sStatusAlarmDelayed:
        //            logging.AddToLog("Alarm Delayed");
        //            break;
        //        case SECURITY1.sStatusMotion:
        //            logging.AddToLog("Motion");
        //            break;
        //        case SECURITY1.sStatusNoMotion:
        //            logging.AddToLog("No Motion");
        //            break;
        //        case SECURITY1.sStatusPanic:
        //            logging.AddToLog("Panic");
        //            break;
        //        case SECURITY1.sStatusPanicOff:
        //            logging.AddToLog("Panic End");
        //            break;
        //        case SECURITY1.sStatusTamper:
        //            logging.AddToLog("Tamper");
        //            break;
        //        case SECURITY1.sStatusArmAway:
        //            logging.AddToLog("Arm Away");
        //            break;
        //        case SECURITY1.sStatusArmAwayDelayed:
        //            logging.AddToLog("Arm Away Delayed");
        //            break;
        //        case SECURITY1.sStatusArmHome:
        //            logging.AddToLog("Arm Home");
        //            break;
        //        case SECURITY1.sStatusArmHomeDelayed:
        //            logging.AddToLog("Arm Home Delayed");
        //            break;
        //        case SECURITY1.sStatusDisarm:
        //            logging.AddToLog("Disarm");
        //            break;
        //        case SECURITY1.sStatusLightOff:
        //            logging.AddToLog("Light Off");
        //            break;
        //        case SECURITY1.sStatusLightOn:
        //            logging.AddToLog("Light On");
        //            break;
        //        case SECURITY1.sStatusLIGHTING2Off:
        //            logging.AddToLog("Light 2 Off");
        //            break;
        //        case SECURITY1.sStatusLIGHTING2On:
        //            logging.AddToLog("Light 2 On");
        //            break;
        //        case SECURITY1.sStatusDark:
        //            logging.AddToLog("Dark detected");
        //            break;
        //        case SECURITY1.sStatusLight:
        //            logging.AddToLog("Light Detected");
        //            break;
        //        case SECURITY1.sStatusBatLow:
        //            logging.AddToLog("Battery low MS10 or XX18 sensor");
        //            break;
        //        case SECURITY1.sStatusPairKD101:
        //            logging.AddToLog("Pair KD101");
        //            break;
        //    }
        //    if ((recbuf(SECURITY1.battery_level) & 0xf) == 0)
        //    {
        //        logging.AddToLog("battery level = Low");
        //    }
        //    else
        //    {
        //        logging.AddToLog("battery level = OK");
        //    }
        //    logging.AddToLog("Signal level  = " + (recbuf(SECURITY1.rssi) >> 4).ToString());
        //}

        //public void decode_Camera1()
        //{
        //    switch (recbuf(CAMERA1.subtype))
        //    {
        //        case CAMERA1.Ninja:
        //            logging.AddToLog("subtype       = X10 Ninja/Robocam");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(CAMERA1.seqnbr).ToString);
        //            logging.AddToLog("Command       = ", false);
        //            switch (recbuf(CAMERA1.cmnd))
        //            {
        //                case CAMERA1.sLeft:
        //                    logging.AddToLog("Left");
        //                    break;
        //                case CAMERA1.sRight:
        //                    logging.AddToLog("Right");
        //                    break;
        //                case CAMERA1.sUp:
        //                    logging.AddToLog("Up");
        //                    break;
        //                case CAMERA1.sDown:
        //                    logging.AddToLog("Down");
        //                    break;
        //                case CAMERA1.sPosition1:
        //                    logging.AddToLog("Position 1");
        //                    break;
        //                case CAMERA1.sProgramPosition1:
        //                    logging.AddToLog("Position 1 program");
        //                    break;
        //                case CAMERA1.sPosition2:
        //                    logging.AddToLog("Position 2");
        //                    break;
        //                case CAMERA1.sProgramPosition2:
        //                    logging.AddToLog("Position 2 program");
        //                    break;
        //                case CAMERA1.sPosition3:
        //                    logging.AddToLog("Position 3");
        //                    break;
        //                case CAMERA1.sProgramPosition3:
        //                    logging.AddToLog("Position 3 program");
        //                    break;
        //                case CAMERA1.sPosition4:
        //                    logging.AddToLog("Position 4");
        //                    break;
        //                case CAMERA1.sProgramPosition4:
        //                    logging.AddToLog("Position 4 program");
        //                    break;
        //                case CAMERA1.sCenter:
        //                    logging.AddToLog("Center");
        //                    break;
        //                case CAMERA1.sProgramCenterPosition:
        //                    logging.AddToLog("Center program");
        //                    break;
        //                case CAMERA1.sSweep:
        //                    logging.AddToLog("Sweep");
        //                    break;
        //                case CAMERA1.sProgramSweep:
        //                    logging.AddToLog("Sweep program");
        //                    break;
        //                default:
        //                    logging.AddToLog("UNKNOWN");
        //                    break;
        //            }
        //            logging.AddToLog("Housecode     = " + Strings.Chr(recbuf(CAMERA1.housecode)));
        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(CAMERA1.packettype)) + ": " + Conversion.Hex(recbuf(CAMERA1.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Signal level  = " + (recbuf(CAMERA1.rssi) >> 4).ToString());
        //}

        //public void decode_Remote()
        //{
        //    switch (recbuf(REMOTE.subtype))
        //    {
        //        case REMOTE.ATI:
        //            logging.AddToLog("subtype       = ATI Remote Wonder");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + recbuf(REMOTE.id).ToString);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("Command       = A", false);
        //                    break;
        //                case 0x1:
        //                    logging.AddToLog("Command       = B", false);
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("Command       = power", false);
        //                    break;
        //                case 0x3:
        //                    logging.AddToLog("Command       = TV", false);
        //                    break;
        //                case 0x4:
        //                    logging.AddToLog("Command       = DVD", false);
        //                    break;
        //                case 0x5:
        //                    logging.AddToLog("Command       = ?", false);
        //                    break;
        //                case 0x6:
        //                    logging.AddToLog("Command       = Guide", false);
        //                    break;
        //                case 0x7:
        //                    logging.AddToLog("Command       = Drag", false);
        //                    break;
        //                case 0x8:
        //                    logging.AddToLog("Command       = VOL+", false);
        //                    break;
        //                case 0x9:
        //                    logging.AddToLog("Command       = VOL-", false);
        //                    break;
        //                case 0xa:
        //                    logging.AddToLog("Command       = MUTE", false);
        //                    break;
        //                case 0xb:
        //                    logging.AddToLog("Command       = CHAN+", false);
        //                    break;
        //                case 0xc:
        //                    logging.AddToLog("Command       = CHAN-", false);
        //                    break;
        //                case 0xd:
        //                    logging.AddToLog("Command       = 1", false);
        //                    break;
        //                case 0xe:
        //                    logging.AddToLog("Command       = 2", false);
        //                    break;
        //                case 0xf:
        //                    logging.AddToLog("Command       = 3", false);
        //                    break;
        //                case 0x10:
        //                    logging.AddToLog("Command       = 4", false);
        //                    break;
        //                case 0x11:
        //                    logging.AddToLog("Command       = 5", false);
        //                    break;
        //                case 0x12:
        //                    logging.AddToLog("Command       = 6", false);
        //                    break;
        //                case 0x13:
        //                    logging.AddToLog("Command       = 7", false);
        //                    break;
        //                case 0x14:
        //                    logging.AddToLog("Command       = 8", false);
        //                    break;
        //                case 0x15:
        //                    logging.AddToLog("Command       = 9", false);
        //                    break;
        //                case 0x16:
        //                    logging.AddToLog("Command       = txt", false);
        //                    break;
        //                case 0x17:
        //                    logging.AddToLog("Command       = 0", false);
        //                    break;
        //                case 0x18:
        //                    logging.AddToLog("Command       = snapshot ESC", false);
        //                    break;
        //                case 0x19:
        //                    logging.AddToLog("Command       = C", false);
        //                    break;
        //                case 0x1a:
        //                    logging.AddToLog("Command       = ^", false);
        //                    break;
        //                case 0x1b:
        //                    logging.AddToLog("Command       = D", false);
        //                    break;
        //                case 0x1c:
        //                    logging.AddToLog("Command       = TV/RADIO", false);
        //                    break;
        //                case 0x1d:
        //                    logging.AddToLog("Command       = <", false);
        //                    break;
        //                case 0x1e:
        //                    logging.AddToLog("Command       = OK", false);
        //                    break;
        //                case 0x1f:
        //                    logging.AddToLog("Command       = >", false);
        //                    break;
        //                case 0x20:
        //                    logging.AddToLog("Command       = <-", false);
        //                    break;
        //                case 0x21:
        //                    logging.AddToLog("Command       = E", false);
        //                    break;
        //                case 0x22:
        //                    logging.AddToLog("Command       = v", false);
        //                    break;
        //                case 0x23:
        //                    logging.AddToLog("Command       = F", false);
        //                    break;
        //                case 0x24:
        //                    logging.AddToLog("Command       = Rewind", false);
        //                    break;
        //                case 0x25:
        //                    logging.AddToLog("Command       = Play", false);
        //                    break;
        //                case 0x26:
        //                    logging.AddToLog("Command       = Fast forward", false);
        //                    break;
        //                case 0x27:
        //                    logging.AddToLog("Command       = Record", false);
        //                    break;
        //                case 0x28:
        //                    logging.AddToLog("Command       = Stop", false);
        //                    break;
        //                case 0x29:
        //                    logging.AddToLog("Command       = Pause", false);

        //                    break;
        //                case 0x2c:
        //                    logging.AddToLog("Command       = TV", false);
        //                    break;
        //                case 0x2d:
        //                    logging.AddToLog("Command       = VCR", false);
        //                    break;
        //                case 0x2e:
        //                    logging.AddToLog("Command       = RADIO", false);
        //                    break;
        //                case 0x2f:
        //                    logging.AddToLog("Command       = TV Preview", false);
        //                    break;
        //                case 0x30:
        //                    logging.AddToLog("Command       = Channel list", false);
        //                    break;
        //                case 0x31:
        //                    logging.AddToLog("Command       = Video Desktop", false);
        //                    break;
        //                case 0x32:
        //                    logging.AddToLog("Command       = red", false);
        //                    break;
        //                case 0x33:
        //                    logging.AddToLog("Command       = green", false);
        //                    break;
        //                case 0x34:
        //                    logging.AddToLog("Command       = yellow", false);
        //                    break;
        //                case 0x35:
        //                    logging.AddToLog("Command       = blue", false);
        //                    break;
        //                case 0x36:
        //                    logging.AddToLog("Command       = rename TAB", false);
        //                    break;
        //                case 0x37:
        //                    logging.AddToLog("Command       = Acquire image", false);
        //                    break;
        //                case 0x38:
        //                    logging.AddToLog("Command       = edit image", false);
        //                    break;
        //                case 0x39:
        //                    logging.AddToLog("Command       = Full screen", false);
        //                    break;
        //                case 0x3a:
        //                    logging.AddToLog("Command       = DVD Audio", false);
        //                    break;
        //                case 0x70:
        //                    logging.AddToLog("Command       = Cursor-left", false);
        //                    break;
        //                case 0x71:
        //                    logging.AddToLog("Command       = Cursor-right", false);
        //                    break;
        //                case 0x72:
        //                    logging.AddToLog("Command       = Cursor-up", false);
        //                    break;
        //                case 0x73:
        //                    logging.AddToLog("Command       = Cursor-down", false);
        //                    break;
        //                case 0x74:
        //                    logging.AddToLog("Command       = Cursor-up-left", false);
        //                    break;
        //                case 0x75:
        //                    logging.AddToLog("Command       = Cursor-up-right", false);
        //                    break;
        //                case 0x76:
        //                    logging.AddToLog("Command       = Cursor-down-right", false);
        //                    break;
        //                case 0x77:
        //                    logging.AddToLog("Command       = Cursor-down-left", false);
        //                    break;
        //                case 0x78:
        //                    logging.AddToLog("Command       = V", false);
        //                    break;
        //                case 0x79:
        //                    logging.AddToLog("Command       = V-End", false);
        //                    break;
        //                case 0x7c:
        //                    logging.AddToLog("Command       = X", false);
        //                    break;
        //                case 0x7d:
        //                    logging.AddToLog("Command       = X-End", false);
        //                    break;
        //                default:
        //                    logging.AddToLog("Command       = unknown", false);
        //                    break;
        //            }

        //            break;
        //        case REMOTE.ATI2:
        //            logging.AddToLog("subtype       = ATI Remote Wonder II");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + recbuf(REMOTE.id).ToString);
        //            logging.AddToLog("Command       = ", false);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("A", false);
        //                    break;
        //                case 0x1:
        //                    logging.AddToLog("B", false);
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("power", false);
        //                    break;
        //                case 0x3:
        //                    logging.AddToLog("TV", false);
        //                    break;
        //                case 0x4:
        //                    logging.AddToLog("DVD", false);
        //                    break;
        //                case 0x5:
        //                    logging.AddToLog("?", false);
        //                    break;
        //                case 0x6:
        //                    logging.AddToLog("Guide", false);
        //                    break;
        //                case 0x7:
        //                    logging.AddToLog("Drag", false);
        //                    break;
        //                case 0x8:
        //                    logging.AddToLog("VOL+", false);
        //                    break;
        //                case 0x9:
        //                    logging.AddToLog("VOL-", false);
        //                    break;
        //                case 0xa:
        //                    logging.AddToLog("MUTE", false);
        //                    break;
        //                case 0xb:
        //                    logging.AddToLog("CHAN+", false);
        //                    break;
        //                case 0xc:
        //                    logging.AddToLog("CHAN-", false);
        //                    break;
        //                case 0xd:
        //                    logging.AddToLog("1", false);
        //                    break;
        //                case 0xe:
        //                    logging.AddToLog("2", false);
        //                    break;
        //                case 0xf:
        //                    logging.AddToLog("3", false);
        //                    break;
        //                case 0x10:
        //                    logging.AddToLog("4", false);
        //                    break;
        //                case 0x11:
        //                    logging.AddToLog("5", false);
        //                    break;
        //                case 0x12:
        //                    logging.AddToLog("6", false);
        //                    break;
        //                case 0x13:
        //                    logging.AddToLog("7", false);
        //                    break;
        //                case 0x14:
        //                    logging.AddToLog("8", false);
        //                    break;
        //                case 0x15:
        //                    logging.AddToLog("9", false);
        //                    break;
        //                case 0x16:
        //                    logging.AddToLog("txt", false);
        //                    break;
        //                case 0x17:
        //                    logging.AddToLog("0", false);
        //                    break;
        //                case 0x18:
        //                    logging.AddToLog("Open Setup Menu", false);
        //                    break;
        //                case 0x19:
        //                    logging.AddToLog("C", false);
        //                    break;
        //                case 0x1a:
        //                    logging.AddToLog("^", false);
        //                    break;
        //                case 0x1b:
        //                    logging.AddToLog("D", false);
        //                    break;
        //                case 0x1c:
        //                    logging.AddToLog("FM", false);
        //                    break;
        //                case 0x1d:
        //                    logging.AddToLog("<", false);
        //                    break;
        //                case 0x1e:
        //                    logging.AddToLog("OK", false);
        //                    break;
        //                case 0x1f:
        //                    logging.AddToLog(">", false);
        //                    break;
        //                case 0x20:
        //                    logging.AddToLog("Max/Restore window", false);
        //                    break;
        //                case 0x21:
        //                    logging.AddToLog("E", false);
        //                    break;
        //                case 0x22:
        //                    logging.AddToLog("v", false);
        //                    break;
        //                case 0x23:
        //                    logging.AddToLog("F", false);
        //                    break;
        //                case 0x24:
        //                    logging.AddToLog("Rewind", false);
        //                    break;
        //                case 0x25:
        //                    logging.AddToLog("Play", false);
        //                    break;
        //                case 0x26:
        //                    logging.AddToLog("Fast forward", false);
        //                    break;
        //                case 0x27:
        //                    logging.AddToLog("Record", false);
        //                    break;
        //                case 0x28:
        //                    logging.AddToLog("Stop", false);
        //                    break;
        //                case 0x29:
        //                    logging.AddToLog("Pause", false);
        //                    break;
        //                case 0x2a:
        //                    logging.AddToLog("TV2", false);
        //                    break;
        //                case 0x2b:
        //                    logging.AddToLog("Clock", false);
        //                    break;
        //                case 0x2c:
        //                    logging.AddToLog("i", false);
        //                    break;
        //                case 0x2d:
        //                    logging.AddToLog("ATI", false);
        //                    break;
        //                case 0x2e:
        //                    logging.AddToLog("RADIO", false);
        //                    break;
        //                case 0x2f:
        //                    logging.AddToLog("TV Preview", false);
        //                    break;
        //                case 0x30:
        //                    logging.AddToLog("Channel list", false);
        //                    break;
        //                case 0x31:
        //                    logging.AddToLog("Video Desktop", false);
        //                    break;
        //                case 0x32:
        //                    logging.AddToLog("red", false);
        //                    break;
        //                case 0x33:
        //                    logging.AddToLog("green", false);
        //                    break;
        //                case 0x34:
        //                    logging.AddToLog("yellow", false);
        //                    break;
        //                case 0x35:
        //                    logging.AddToLog("blue", false);
        //                    break;
        //                case 0x36:
        //                    logging.AddToLog("rename TAB", false);
        //                    break;
        //                case 0x37:
        //                    logging.AddToLog("Acquire image", false);
        //                    break;
        //                case 0x38:
        //                    logging.AddToLog("edit image", false);
        //                    break;
        //                case 0x39:
        //                    logging.AddToLog("Full screen", false);
        //                    break;
        //                case 0x3a:
        //                    logging.AddToLog("DVD Audio", false);
        //                    break;
        //                case 0x70:
        //                    logging.AddToLog("Cursor-left", false);
        //                    break;
        //                case 0x71:
        //                    logging.AddToLog("Cursor-right", false);
        //                    break;
        //                case 0x72:
        //                    logging.AddToLog("Cursor-up", false);
        //                    break;
        //                case 0x73:
        //                    logging.AddToLog("Cursor-down", false);
        //                    break;
        //                case 0x74:
        //                    logging.AddToLog("Cursor-up-left", false);
        //                    break;
        //                case 0x75:
        //                    logging.AddToLog("Cursor-up-right", false);
        //                    break;
        //                case 0x76:
        //                    logging.AddToLog("Cursor-down-right", false);
        //                    break;
        //                case 0x77:
        //                    logging.AddToLog("Cursor-down-left", false);
        //                    break;
        //                case 0x78:
        //                    logging.AddToLog("Left Mouse Button", false);
        //                    break;
        //                case 0x79:
        //                    logging.AddToLog("V-End", false);
        //                    break;
        //                case 0x7c:
        //                    logging.AddToLog("Right Mouse Button", false);
        //                    break;
        //                case 0x7d:
        //                    logging.AddToLog("X-End", false);
        //                    break;
        //                default:
        //                    logging.AddToLog("unknown", false);
        //                    break;
        //            }
        //            if ((recbuf(REMOTE.toggle) & 0x1) == 0x1)
        //            {
        //                logging.AddToLog("  (button press = odd)");
        //            }
        //            else
        //            {
        //                logging.AddToLog("  (button press = even)");
        //            }

        //            break;
        //        case REMOTE.Medion:
        //            logging.AddToLog("subtype       = Medion Remote");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + recbuf(REMOTE.id).ToString);
        //            logging.AddToLog("Command       = ", false);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("Mute");
        //                    break;
        //                case 0x1:
        //                    logging.AddToLog("B");
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("power");
        //                    break;
        //                case 0x3:
        //                    logging.AddToLog("TV");
        //                    break;
        //                case 0x4:
        //                    logging.AddToLog("DVD");
        //                    break;
        //                case 0x5:
        //                    logging.AddToLog("Photo");
        //                    break;
        //                case 0x6:
        //                    logging.AddToLog("Music");
        //                    break;
        //                case 0x7:
        //                    logging.AddToLog("Drag");
        //                    break;
        //                case 0x8:
        //                    logging.AddToLog("VOL-");
        //                    break;
        //                case 0x9:
        //                    logging.AddToLog("VOL+");
        //                    break;
        //                case 0xa:
        //                    logging.AddToLog("MUTE");
        //                    break;
        //                case 0xb:
        //                    logging.AddToLog("CHAN+");
        //                    break;
        //                case 0xc:
        //                    logging.AddToLog("CHAN-");
        //                    break;
        //                case 0xd:
        //                    logging.AddToLog("1");
        //                    break;
        //                case 0xe:
        //                    logging.AddToLog("2");
        //                    break;
        //                case 0xf:
        //                    logging.AddToLog("3");
        //                    break;
        //                case 0x10:
        //                    logging.AddToLog("4");
        //                    break;
        //                case 0x11:
        //                    logging.AddToLog("5");
        //                    break;
        //                case 0x12:
        //                    logging.AddToLog("6");
        //                    break;
        //                case 0x13:
        //                    logging.AddToLog("7");
        //                    break;
        //                case 0x14:
        //                    logging.AddToLog("8");
        //                    break;
        //                case 0x15:
        //                    logging.AddToLog("9");
        //                    break;
        //                case 0x16:
        //                    logging.AddToLog("txt");
        //                    break;
        //                case 0x17:
        //                    logging.AddToLog("0");
        //                    break;
        //                case 0x18:
        //                    logging.AddToLog("snapshot ESC");
        //                    break;
        //                case 0x19:
        //                    logging.AddToLog("DVD MENU");
        //                    break;
        //                case 0x1a:
        //                    logging.AddToLog("^");
        //                    break;
        //                case 0x1b:
        //                    logging.AddToLog("Setup");
        //                    break;
        //                case 0x1c:
        //                    logging.AddToLog("TV/RADIO");
        //                    break;
        //                case 0x1d:
        //                    logging.AddToLog("<");
        //                    break;
        //                case 0x1e:
        //                    logging.AddToLog("OK");
        //                    break;
        //                case 0x1f:
        //                    logging.AddToLog(">");
        //                    break;
        //                case 0x20:
        //                    logging.AddToLog("<-");
        //                    break;
        //                case 0x21:
        //                    logging.AddToLog("E");
        //                    break;
        //                case 0x22:
        //                    logging.AddToLog("v");
        //                    break;
        //                case 0x23:
        //                    logging.AddToLog("F");
        //                    break;
        //                case 0x24:
        //                    logging.AddToLog("Rewind");
        //                    break;
        //                case 0x25:
        //                    logging.AddToLog("Play");
        //                    break;
        //                case 0x26:
        //                    logging.AddToLog("Fast forward");
        //                    break;
        //                case 0x27:
        //                    logging.AddToLog("Record");
        //                    break;
        //                case 0x28:
        //                    logging.AddToLog("Stop");
        //                    break;
        //                case 0x29:
        //                    logging.AddToLog("Pause");

        //                    break;
        //                case 0x2c:
        //                    logging.AddToLog("TV");
        //                    break;
        //                case 0x2d:
        //                    logging.AddToLog("VCR");
        //                    break;
        //                case 0x2e:
        //                    logging.AddToLog("RADIO");
        //                    break;
        //                case 0x2f:
        //                    logging.AddToLog("TV Preview");
        //                    break;
        //                case 0x30:
        //                    logging.AddToLog("Channel list");
        //                    break;
        //                case 0x31:
        //                    logging.AddToLog("Video Desktop");
        //                    break;
        //                case 0x32:
        //                    logging.AddToLog("red");
        //                    break;
        //                case 0x33:
        //                    logging.AddToLog("green");
        //                    break;
        //                case 0x34:
        //                    logging.AddToLog("yellow");
        //                    break;
        //                case 0x35:
        //                    logging.AddToLog("blue");
        //                    break;
        //                case 0x36:
        //                    logging.AddToLog("rename TAB");
        //                    break;
        //                case 0x37:
        //                    logging.AddToLog("Acquire image");
        //                    break;
        //                case 0x38:
        //                    logging.AddToLog("edit image");
        //                    break;
        //                case 0x39:
        //                    logging.AddToLog("Full screen");
        //                    break;
        //                case 0x3a:
        //                    logging.AddToLog("DVD Audio");
        //                    break;
        //                case 0x70:
        //                    logging.AddToLog("Cursor-left");
        //                    break;
        //                case 0x71:
        //                    logging.AddToLog("Cursor-right");
        //                    break;
        //                case 0x72:
        //                    logging.AddToLog("Cursor-up");
        //                    break;
        //                case 0x73:
        //                    logging.AddToLog("Cursor-down");
        //                    break;
        //                case 0x74:
        //                    logging.AddToLog("Cursor-up-left");
        //                    break;
        //                case 0x75:
        //                    logging.AddToLog("Cursor-up-right");
        //                    break;
        //                case 0x76:
        //                    logging.AddToLog("Cursor-down-right");
        //                    break;
        //                case 0x77:
        //                    logging.AddToLog("Cursor-down-left");
        //                    break;
        //                case 0x78:
        //                    logging.AddToLog("V");
        //                    break;
        //                case 0x79:
        //                    logging.AddToLog("V-End");
        //                    break;
        //                case 0x7c:
        //                    logging.AddToLog("X");
        //                    break;
        //                case 0x7d:
        //                    logging.AddToLog("X-End");
        //                    break;
        //                default:
        //                    logging.AddToLog("unknown");
        //                    break;
        //            }

        //            break;
        //        case REMOTE.PCremote:
        //            logging.AddToLog("subtype       = PC Remote");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + recbuf(REMOTE.id).ToString);
        //            logging.AddToLog("Command       = unknown", false);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x2:
        //                    logging.AddToLog("0");
        //                    break;
        //                case 0x82:
        //                    logging.AddToLog("1");
        //                    break;
        //                case 0xd1:
        //                    logging.AddToLog("MP3");
        //                    break;
        //                case 0x42:
        //                    logging.AddToLog("2");
        //                    break;
        //                case 0xd2:
        //                    logging.AddToLog("DVD");
        //                    break;
        //                case 0xc2:
        //                    logging.AddToLog("3");
        //                    break;
        //                case 0xd3:
        //                    logging.AddToLog("CD");
        //                    break;
        //                case 0x22:
        //                    logging.AddToLog("4");
        //                    break;
        //                case 0xd4:
        //                    logging.AddToLog("PC or SHIFT-4");
        //                    break;
        //                case 0xa2:
        //                    logging.AddToLog("5");
        //                    break;
        //                case 0xd5:
        //                    logging.AddToLog("SHIFT-5");
        //                    break;
        //                case 0x62:
        //                    logging.AddToLog("6");
        //                    break;
        //                case 0xe2:
        //                    logging.AddToLog("7");
        //                    break;
        //                case 0x12:
        //                    logging.AddToLog("8");
        //                    break;
        //                case 0x92:
        //                    logging.AddToLog("9");
        //                    break;
        //                case 0xc0:
        //                    logging.AddToLog("CH-");
        //                    break;
        //                case 0x40:
        //                    logging.AddToLog("CH+");
        //                    break;
        //                case 0xe0:
        //                    logging.AddToLog("VOL-");
        //                    break;
        //                case 0x60:
        //                    logging.AddToLog("VOL+");
        //                    break;
        //                case 0xa0:
        //                    logging.AddToLog("MUTE");
        //                    break;
        //                case 0x3a:
        //                    logging.AddToLog("INFO");
        //                    break;
        //                case 0x38:
        //                    logging.AddToLog("REW");
        //                    break;
        //                case 0xb8:
        //                    logging.AddToLog("FF");
        //                    break;
        //                case 0xb0:
        //                    logging.AddToLog("PLAY");
        //                    break;
        //                case 0x64:
        //                    logging.AddToLog("PAUSE");
        //                    break;
        //                case 0x63:
        //                    logging.AddToLog("STOP");
        //                    break;
        //                case 0xb6:
        //                    logging.AddToLog("MENU");
        //                    break;
        //                case 0xff:
        //                    logging.AddToLog("REC");
        //                    break;
        //                case 0xc9:
        //                    logging.AddToLog("EXIT");
        //                    break;
        //                case 0xd8:
        //                    logging.AddToLog("TEXT");
        //                    break;
        //                case 0xd9:
        //                    logging.AddToLog("SHIFT-TEXT");
        //                    break;
        //                case 0xf2:
        //                    logging.AddToLog("TELETEXT");
        //                    break;
        //                case 0xd7:
        //                    logging.AddToLog("SHIFT-TELETEXT");
        //                    break;
        //                case 0xba:
        //                    logging.AddToLog("A+B");
        //                    break;
        //                case 0x52:
        //                    logging.AddToLog("ENT");
        //                    break;
        //                case 0xd6:
        //                    logging.AddToLog("SHIFT-ENT");
        //                    break;
        //                case 0x70:
        //                    logging.AddToLog("Cursor-left");
        //                    break;
        //                case 0x71:
        //                    logging.AddToLog("Cursor-right");
        //                    break;
        //                case 0x72:
        //                    logging.AddToLog("Cursor-up");
        //                    break;
        //                case 0x73:
        //                    logging.AddToLog("Cursor-down");
        //                    break;
        //                case 0x74:
        //                    logging.AddToLog("Cursor-up-left");
        //                    break;
        //                case 0x75:
        //                    logging.AddToLog("Cursor-up-right");
        //                    break;
        //                case 0x76:
        //                    logging.AddToLog("Cursor-down-right");
        //                    break;
        //                case 0x77:
        //                    logging.AddToLog("Cursor-down-left");
        //                    break;
        //                case 0x78:
        //                    logging.AddToLog("Left mouse");
        //                    break;
        //                case 0x79:
        //                    logging.AddToLog("Left mouse-End");
        //                    break;
        //                case 0x7b:
        //                    logging.AddToLog("Drag");
        //                    break;
        //                case 0x7c:
        //                    logging.AddToLog("Right mouse");
        //                    break;
        //                case 0x7d:
        //                    logging.AddToLog("Right mouse-End");
        //                    break;
        //                default:
        //                    logging.AddToLog("unknown");
        //                    break;
        //            }

        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(REMOTE.packettype)) + ":" + Conversion.Hex(recbuf(REMOTE.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Signal level  = " + (recbuf(REMOTE.rssi) >> 4).ToString());

        //}

        //public void decode_Thermostat1()
        //{
        //    switch (recbuf(THERMOSTAT1.subtype))
        //    {
        //        case THERMOSTAT1.Digimax:
        //            logging.AddToLog("subtype       = Digimax");
        //            break;
        //        case THERMOSTAT1.DigimaxShort:
        //            logging.AddToLog("subtype       = Digimax with short format");
        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(THERMOSTAT1.packettype)) + ":" + Conversion.Hex(recbuf(THERMOSTAT1.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Sequence nbr  = " + recbuf(THERMOSTAT1.seqnbr).ToString);
        //    logging.AddToLog("ID            = " + ((recbuf(THERMOSTAT1.id1) * 256 + recbuf(THERMOSTAT1.id2))).ToString());
        //    logging.AddToLog("Temperature   = " + recbuf(THERMOSTAT1.temperature).ToString + " °C");
        //    if (recbuf(THERMOSTAT1.subtype) == THERMOSTAT1.Digimax)
        //    {
        //        logging.AddToLog("Set           = " + recbuf(THERMOSTAT1.set_point).ToString + " °C");
        //        if ((recbuf(THERMOSTAT1.mode) & 0x80) == 0)
        //        {
        //            logging.AddToLog("Mode          = heating");
        //        }
        //        else
        //        {
        //            logging.AddToLog("Mode          = Cooling");
        //        }
        //        switch ((recbuf(THERMOSTAT1.status) & 0x3))
        //        {
        //            case 0:
        //                logging.AddToLog("Status        = no status available");
        //                break;
        //            case 1:
        //                logging.AddToLog("Status        = demand");
        //                break;
        //            case 2:
        //                logging.AddToLog("Status        = no demand");
        //                break;
        //            case 3:
        //                logging.AddToLog("Status        = initializing");
        //                break;
        //        }
        //    }

        //    logging.AddToLog("Signal level  = " + (recbuf(THERMOSTAT1.rssi) >> 4).ToString());
        //}

        //public void decode_Thermostat2()
        //{
        //    logging.AddToLog("Not implemented");
        //}

        //public void decode_Thermostat3()
        //{
        //    switch (recbuf(THERMOSTAT3.subtype))
        //    {
        //        case THERMOSTAT3.MertikG6RH4T1:
        //            logging.AddToLog("subtype       = Mertik G6R-H4T1");
        //            break;
        //        case THERMOSTAT3.MertikG6RH4TB:
        //            logging.AddToLog("subtype       = Mertik G6R-H4TB");
        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(THERMOSTAT3.packettype)) + ":" + Conversion.Hex(recbuf(THERMOSTAT3.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Sequence nbr  = " + recbuf(THERMOSTAT3.seqnbr).ToString);

        //    logging.AddToLog("ID            = 0x" + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode2)), 2) + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode3)), 2));

        //    switch (recbuf(THERMOSTAT3.cmnd))
        //    {
        //        case 0:
        //            logging.AddToLog("Command       = Off");
        //            break;
        //        case 1:
        //            logging.AddToLog("Command       = On");
        //            break;
        //        case 2:
        //            logging.AddToLog("Command       = Up");
        //            break;
        //        case 3:
        //            logging.AddToLog("Command       = Down");
        //            break;
        //        case 4:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                logging.AddToLog("Command       = Run Up");
        //            }
        //            else
        //            {
        //                logging.AddToLog("Command       = 2nd Off");
        //            }
        //            break;
        //        case 5:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                logging.AddToLog("Command       = Run Down");
        //            }
        //            else
        //            {
        //                logging.AddToLog("Command       = 2nd On");
        //            }
        //            break;
        //        case 6:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                logging.AddToLog("Command       = Stop");
        //            }
        //            else
        //            {
        //                logging.AddToLog("Command       = unknown");
        //            }
        //            break;
        //        default:
        //            logging.AddToLog("Command       = unknown");
        //            break;
        //    }

        //    logging.AddToLog("Signal level  = " + (recbuf(THERMOSTAT3.rssi) >> 4).ToString());
        //}

        public void decode_Temp()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName,"Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New temperature sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Temperature Sensor - " + (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString(), "Temperature Sensor", "OS TEMP SENSOR", (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString());
            }

            switch (recbuf[(byte)TEMP.subtype])
            {
                case (byte)TEMP.TEMP1:
                    logging.AddToLog("subtype       = TEMP1 - THR128/138, THC138", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP.id2].ToString(), false);
                    break;
                case (byte)TEMP.TEMP2:
                    logging.AddToLog("subtype       = TEMP2 - THC238/268,THN132,THWR288,THRN122,THN122,AW129/131", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP.id2].ToString(), false);
                    break;
                case (byte)TEMP.TEMP3:
                    logging.AddToLog("subtype       = TEMP3 - THWR800", false);
                    break;
                case (byte)TEMP.TEMP4:
                    logging.AddToLog("subtype       = TEMP4 - RTHN318", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP.id2].ToString(), false);
                    break;
                case (byte)TEMP.TEMP5:
                    logging.AddToLog("subtype       = TEMP5 - LaCrosse TX3, TX4, TX17", false);
                    break;
                case (byte)TEMP.TEMP6:
                    logging.AddToLog("subtype       = TEMP6 - TS15C", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP.packettype].ToString() + ":" + recbuf[(byte)TEMP.subtype].ToString(), false);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)TEMP.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString(), false);

            
            double temp = Math.Round((double)(recbuf[(byte)TEMP.temperatureh] * 256 + recbuf[(byte)TEMP.temperaturel]) / 10, 2);
            string strTemp = "";

            if (osae.GetObjectPropertyValue(pName, "Temp Units").Value.Trim() == "Farenheit")
            {
                temp = (temp * 9 / 5) + 32;
                strTemp = temp.ToString() + " °F";
            }
            else
                strTemp = temp.ToString() + " °C";

            if ((recbuf[(byte)TEMP.tempsign] & 0x80) == 0)
            {
                logging.AddToLog("Temperature   = " + strTemp, false);
                osae.ObjectPropertySet(obj.Name, "Temperature", temp.ToString());
            }
            else
            {
                logging.AddToLog("Temperature   = -" + strTemp, false);
                osae.ObjectPropertySet(obj.Name, "Temperature", "-" + temp.ToString());
            }
            logging.AddToLog("Signal level  = " + (recbuf[(byte)TEMP.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)TEMP.battery_level] & 0xf) == 0)
            {
                logging.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                logging.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Hum()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New humidity sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Humidity Sensor - " + (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString(), "Humidity Sensor", "HUMIDITY METER", (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString());
            }

            switch (recbuf[(byte)HUM.subtype])
            {
                case (byte)HUM.HUM1:
                    logging.AddToLog("subtype       = HUM1 - LaCrosse TX3", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)HUM.packettype] + ":" + recbuf[(byte)HUM.subtype], true);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)HUM.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString(), false);
            logging.AddToLog("Humidity      = " + recbuf[(byte)HUM.humidity].ToString(), false);
            osae.ObjectPropertySet(obj.Name, "Humidity", recbuf[(byte)HUM.humidity].ToString());

            switch (recbuf[(byte)HUM.humidity_status])
            {
                case 0x0:
                    logging.AddToLog("Status        = Dry", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Dry");
                    break;
                case 0x1:
                    logging.AddToLog("Status        = Comfortable", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Comfortable");
                    break;
                case 0x2:
                    logging.AddToLog("Status        = Normal", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Normal");
                    break;
                case 0x3:
                    logging.AddToLog("Status        = Wet", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Wet");
                    break;
            }
            logging.AddToLog("Signal level  = " + (recbuf[(byte)HUM.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)HUM.battery_level] & 0xf) == 0)
            {
                logging.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                logging.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_TempHum()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New temperature and humidity sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Temp and Humidity Sensor - " + (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString(), "Temp and Humidity Sensor", "TEMP HUM METER", (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString());
            }
            
            switch (recbuf[(byte)TEMP_HUM.subtype])
            {
                case (byte)TEMP_HUM.TH1:
                    logging.AddToLog("subtype       = TH1 - THGN122/123,/THGN132,THGR122/228/238/268", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH2:
                    logging.AddToLog("subtype       = TH2 - THGR810,THGN800", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH3:
                    logging.AddToLog("subtype       = TH3 - RTGR328", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH4:
                    logging.AddToLog("subtype       = TH4 - THGR328", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH5:
                    logging.AddToLog("subtype       = TH5 - WTGR800", false);
                    break;
                case (byte)TEMP_HUM.TH6:
                    logging.AddToLog("subtype       = TH6 - THGR918,THGRN228,THGN500", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH7:
                    logging.AddToLog("subtype       = TH7 - Cresta, TFA TS34C", false);
                    if (recbuf[(byte)TEMP_HUM.id1] < 0x40)
                    {
                        logging.AddToLog("                channel 1", false);
                    }
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0x60)
                    {
                        logging.AddToLog("                channel 2", false);
                    }
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0x80)
                    {
                        logging.AddToLog("                channel 3", false);
                    }
                    else if (recbuf[(byte)TEMP_HUM.id1] > 0x9f & (byte)TEMP_HUM.id1 < 0xc0)
                    {
                        logging.AddToLog("                channel 4", false);
                    }
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0xe0)
                    {
                        logging.AddToLog("                channel 5", false);
                    }
                    else
                    {
                        logging.AddToLog("                channel ??", false);
                    }
                    break;
                case (byte)TEMP_HUM.TH8:
                    logging.AddToLog("subtype       = TH8 - WT440H,WT450H", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP_HUM.packettype] + ":" + recbuf[(byte)TEMP_HUM.subtype], false);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)TEMP_HUM.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString(), false);
            if ((recbuf[(byte)TEMP_HUM.tempsign] & 0x80) == 0)
            {
                logging.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            else
            {
                logging.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            logging.AddToLog("Humidity      = " + recbuf[(byte)TEMP_HUM.humidity].ToString(), false);
            switch (recbuf[(byte)TEMP_HUM.humidity_status])
            {
                case 0x0:
                    logging.AddToLog("Status        = Dry", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Dry");
                    break;
                case 0x1:
                    logging.AddToLog("Status        = Comfortable", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Comfortable");
                    break;
                case 0x2:
                    logging.AddToLog("Status        = Normal", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Normal");
                    break;
                case 0x3:
                    logging.AddToLog("Status        = Wet", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Wet");
                    break;
            }
            logging.AddToLog("Signal level  = " + (recbuf[(byte)TEMP_HUM.rssi] >> 4).ToString(), false);
            if (recbuf[(byte)TEMP_HUM.subtype] == (byte)TEMP_HUM.TH6)
            {
                switch (recbuf[(byte)TEMP_HUM.battery_level])
                {
                    case 0:
                        logging.AddToLog("Battery       = 10%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "10%");
                        break;
                    case 1:
                        logging.AddToLog("Battery       = 20%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "20%");
                        break;
                    case 2:
                        logging.AddToLog("Battery       = 30%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "30%");
                        break;
                    case 3:
                        logging.AddToLog("Battery       = 40%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "40%");
                        break;
                    case 4:
                        logging.AddToLog("Battery       = 50%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "50%");
                        break;
                    case 5:
                        logging.AddToLog("Battery       = 60%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "60%");
                        break;
                    case 6:
                        logging.AddToLog("Battery       = 70%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "70%");
                        break;
                    case 7:
                        logging.AddToLog("Battery       = 80%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "80%");
                        break;
                    case 8:
                        logging.AddToLog("Battery       = 90%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "90%");
                        break;
                    case 9:
                        logging.AddToLog("Battery       = 100%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "100%");
                        break;
                }
            }
            else
            {
                if ((recbuf[(byte)TEMP_HUM.battery_level] & 0xf) == 0)
                {
                    logging.AddToLog("Battery       = Low", false);
                    osae.ObjectPropertySet(obj.Name, "Battery", "Low");
                }
                else
                {
                    logging.AddToLog("Battery       = OK", false);
                    osae.ObjectPropertySet(obj.Name, "Battery", "OK");
                }
            }
        }

        public void decode_Baro()
        {
            logging.AddToLog("Baro Not implemented", true);
        }

        public void decode_TempHumBaro()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New temperature, humidity and barometric sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Temp, Humidity and Baro Sensor - " + (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString(), "Temp, Humidity and Baro Sensor", "TEMP HUM BARO METER", (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString());
            }
            
            switch (recbuf[(byte)TEMP_HUM_BARO.subtype])
            {
                case (byte)TEMP_HUM_BARO.THB1:
                    logging.AddToLog("subtype       = THB1 - BTHR918", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP_HUM_BARO.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM_BARO.THB2:
                    logging.AddToLog("subtype       = THB2 - BTHR918N, BTHR968", false);
                    logging.AddToLog("                channel " + recbuf[(byte)TEMP_HUM_BARO.id2].ToString(), false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP_HUM_BARO.packettype] + ":" + recbuf[(byte)TEMP_HUM_BARO.subtype], false);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)TEMP_HUM_BARO.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString(), false);
            if ((recbuf[(byte)TEMP_HUM_BARO.tempsign] & 0x80) == 0)
            {
                logging.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            else
            {
                logging.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            logging.AddToLog("Humidity      = " + recbuf[(byte)TEMP_HUM_BARO.humidity].ToString(), false);
            osae.ObjectPropertySet(obj.Name, "Humidity", recbuf[(byte)TEMP_HUM_BARO.humidity].ToString());
            
            switch (recbuf[(byte)TEMP_HUM_BARO.humidity_status])
            {
                case 0x0:
                    logging.AddToLog("Status        = Dry", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Dry");
                    break;
                case 0x1:
                    logging.AddToLog("Status        = Comfortable", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Comfortable");
                    break;
                case 0x2:
                    logging.AddToLog("Status        = Normal", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Normal");
                    break;
                case 0x3:
                    logging.AddToLog("Status        = Wet", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Wet");
                    break;
            }
            logging.AddToLog("Barometer     = " + recbuf[(byte)TEMP_HUM_BARO.baroh] * 256 + recbuf[(byte)TEMP_HUM_BARO.barol].ToString(), false);
            osae.ObjectPropertySet(obj.Name, "Barometer", recbuf[(byte)TEMP_HUM_BARO.baroh] * 256 + recbuf[(byte)TEMP_HUM_BARO.barol].ToString());

            switch (recbuf[(byte)TEMP_HUM_BARO.forecast])
            {
                case 0x0:
                    logging.AddToLog("Forecast      = No information available", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "No information available");
                    break;
                case 0x1:
                    logging.AddToLog("Forecast      = Sunny", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "Sunny");
                    break;
                case 0x2:
                    logging.AddToLog("Forecast      = Partly Cloudy", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "Partly Cloudy");
                    break;
                case 0x3:
                    logging.AddToLog("Forecast      = Cloudy", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "Cloudy");
                    break;
                case 0x4:
                    logging.AddToLog("Forecast      = Rain", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "Rain");
                    break;
            }

            logging.AddToLog("Signal level  = " + (recbuf[(byte)TEMP_HUM_BARO.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)TEMP_HUM_BARO.battery_level] & 0xf) == 0)
            {
                logging.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                logging.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Rain()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New temperature sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Rain Meter - " + (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString(), "Rain Meter", "OS RAIN METER", (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString());
            }

            switch (recbuf[(byte)RAIN.subtype])
            {
                case (byte)RAIN.RAIN1:
                    logging.AddToLog("subtype       = RAIN1 - RGR126/682/918", false);
                    break;
                case (byte)RAIN.RAIN2:
                    logging.AddToLog("subtype       = RAIN2 - PCR800", false);
                    break;
                case (byte)RAIN.RAIN3:
                    logging.AddToLog("subtype       = RAIN3 - TFA", false);
                    break;
                case (byte)RAIN.RAIN4:
                    logging.AddToLog("subtype       = RAIN4 - UPM RG700", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)RAIN.packettype].ToString() + ":" + recbuf[(byte)RAIN.subtype].ToString(), true);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)RAIN.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString(), false);

            if (recbuf[(byte)RAIN.subtype] == (byte)RAIN.RAIN1)
            {
                logging.AddToLog("Rain rate     = " + ((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]).ToString() + " mm/h", false);
                osae.ObjectPropertySet(obj.Name, "Rain Rate", ((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]).ToString());
            }
            else if (recbuf[(byte)RAIN.subtype] == (byte)RAIN.RAIN2)
            {
                logging.AddToLog("Rain rate     = " + (((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]) / 100).ToString() + " mm/h", false);
                osae.ObjectPropertySet(obj.Name, "Rain Rate", (((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]) / 100).ToString());
            }

            logging.AddToLog("Total rain    = " + Math.Round((double)((recbuf[(byte)RAIN.raintotal1] * 65535) + recbuf[(byte)RAIN.raintotal2] * 256 + recbuf[(byte)RAIN.raintotal3]) / 10, 2).ToString() + " mm", false);
            osae.ObjectPropertySet(obj.Name, "Total Rain", Math.Round((double)((recbuf[(byte)RAIN.raintotal1] * 65535) + recbuf[(byte)RAIN.raintotal2] * 256 + recbuf[(byte)RAIN.raintotal3]) / 10, 2).ToString());

            logging.AddToLog("Signal level  = " + (recbuf[(byte)RAIN.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)RAIN.battery_level] & 0xf) == 0)
            {
                logging.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                logging.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Wind()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New wind sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Wind Sensor - " + (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString(), "Wind Sensor", "WIND SENSOR", (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString());
            }

            int intDirection = 0;
            int intSpeed = 0;
            string strDirection = null;

            switch (recbuf[(byte)WIND.subtype])
            {
                case (byte)WIND.WIND1:
                    logging.AddToLog("subtype       = WIND1 - WTGR800", false);
                    break;
                case (byte)WIND.WIND2:
                    logging.AddToLog("subtype       = WIND2 - WGR800", false);
                    break;
                case (byte)WIND.WIND3:
                    logging.AddToLog("subtype       = WIND3 - STR918, WGR918", false);
                    break;
                case (byte)WIND.WIND4:
                    logging.AddToLog("subtype       = WIND4 - TFA", false);
                    break;
                case (byte)WIND.WIND5:
                    logging.AddToLog("subtype       = WIND5 - UPM WDS500", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)WIND.packettype] + ":" + recbuf[(byte)WIND.subtype], false);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)WIND.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString(), false);
            intDirection = (recbuf[(byte)WIND.directionh] * 256) + recbuf[(byte)WIND.directionl];
            if (intDirection > 348.75 | intDirection < 11.26)
            {
                strDirection = "N";
            }
            else if (intDirection < 33.76)
            {
                strDirection = "NNE";
            }
            else if (intDirection < 56.26)
            {
                strDirection = "NE";
            }
            else if (intDirection < 78.76)
            {
                strDirection = "ENE";
            }
            else if (intDirection < 101.26)
            {
                strDirection = "E";
            }
            else if (intDirection < 123.76)
            {
                strDirection = "ESE";
            }
            else if (intDirection < 146.26)
            {
                strDirection = "SE";
            }
            else if (intDirection < 168.76)
            {
                strDirection = "SSE";
            }
            else if (intDirection < 191.26)
            {
                strDirection = "S";
            }
            else if (intDirection < 213.76)
            {
                strDirection = "SSW";
            }
            else if (intDirection < 236.26)
            {
                strDirection = "SW";
            }
            else if (intDirection < 258.76)
            {
                strDirection = "WSW";
            }
            else if (intDirection < 281.26)
            {
                strDirection = "W";
            }
            else if (intDirection < 303.76)
            {
                strDirection = "WNW";
            }
            else if (intDirection < 326.26)
            {
                strDirection = "NW";
            }
            else if (intDirection < 348.76)
            {
                strDirection = "NNW";
            }
            else
            {
                strDirection = "---";
            }
            logging.AddToLog("Direction     = " + intDirection.ToString() + " degrees  " + strDirection, false);
            osae.ObjectPropertySet(obj.Name, "Direction", intDirection.ToString() + " degrees  " + strDirection);

            intSpeed = (recbuf[(byte)WIND.av_speedh] * 256) + recbuf[(byte)WIND.av_speedl];
            if (recbuf[(byte)WIND.subtype] != (byte)WIND.WIND5)
            {
                logging.AddToLog("Average speed = " + (intSpeed / 10).ToString() + " mtr/sec = " + Math.Round((intSpeed * 0.36), 2).ToString() + " km/hr = " + Math.Round((intSpeed * 0.223693629) / 10, 2).ToString() + " mph", false);
                osae.ObjectPropertySet(obj.Name, "Average Speed", Math.Round((intSpeed * 0.223693629) / 10, 2).ToString());
            }

            intSpeed = (recbuf[(byte)WIND.gusth] * 256) + recbuf[(byte)WIND.gustl];
            logging.AddToLog("Wind gust     = " + (intSpeed / 10).ToString() + " mtr/sec = " + Math.Round((intSpeed * 0.36), 2).ToString() + " km/hr = " + Math.Round((intSpeed * 0.223693629) / 10, 2).ToString() + " mph", false);
            osae.ObjectPropertySet(obj.Name, "Wind Gust", Math.Round((intSpeed * 0.223693629) / 10, 2).ToString()); 
            
            if (recbuf[(byte)WIND.subtype] == (byte)WIND.WIND4)
            {
                if (((byte)WIND.tempsign & 0x80) == 0)
                {
                    logging.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
                else
                {
                    logging.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
                }

                if (((byte)WIND.chillsign & 0x80) == 0)
                {
                    logging.AddToLog("Chill         = " + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Windchill", (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
                else
                {
                    logging.AddToLog("Chill         = -" + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Windchill", "-" + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
            }

            logging.AddToLog("Signal level  = " + (recbuf[(byte)WIND.rssi] >> 4).ToString(), false);
            if (recbuf[(byte)WIND.subtype] == (byte)WIND.WIND3)
            {
                switch (recbuf[(byte)WIND.battery_level])
                {
                    case 0:
                        logging.AddToLog("Battery       = 10%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "10%");
                        break;
                    case 1:
                        logging.AddToLog("Battery       = 20%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "20%");
                        break;
                    case 2:
                        logging.AddToLog("Battery       = 30%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "30%");
                        break;
                    case 3:
                        logging.AddToLog("Battery       = 40%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "40%");
                        break;
                    case 4:
                        logging.AddToLog("Battery       = 50%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "50%");
                        break;
                    case 5:
                        logging.AddToLog("Battery       = 60%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "60%");
                        break;
                    case 6:
                        logging.AddToLog("Battery       = 70%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "70%");
                        break;
                    case 7:
                        logging.AddToLog("Battery       = 80%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "80%");
                        break;
                    case 8:
                        logging.AddToLog("Battery       = 90%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "90%");
                        break;
                    case 9:
                        logging.AddToLog("Battery       = 100%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "100%");
                        break;
                }
            }
            else
            {
                if ((recbuf[(byte)WIND.battery_level] & 0xf) == 0)
                {
                    logging.AddToLog("Battery       = Low", false);
                    osae.ObjectPropertySet(obj.Name, "Battery", "Low");
                }
                else
                {
                    logging.AddToLog("Battery       = OK", false);
                    osae.ObjectPropertySet(obj.Name, "Battery", "OK");
                }
            }
        }

        public void decode_UV()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New UV sensor found.  Adding to OSA", true);
                osae.ObjectAdd("UV Sensor - " + (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString(), "UV Sensor", "UV SENSOR", (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString());
            }

            switch (recbuf[(byte)UV.subtype])
            {
                case (byte)UV.UV1:
                    logging.AddToLog("Subtype       = UV1 - UVN128, UV138", false);
                    break;
                case (byte)UV.UV2:
                    logging.AddToLog("Subtype       = UV2 - UVN800", false);
                    break;
                case (byte)UV.UV3:
                    logging.AddToLog("Subtype       = UV3 - TFA", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + (byte)UV.packettype + ":" + recbuf[(byte)UV.subtype], false);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)UV.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString(), false);
            logging.AddToLog("Level         = " + (recbuf[(byte)UV.uv] / 10).ToString(), false);
            osae.ObjectPropertySet(obj.Name, "Level", (recbuf[(byte)UV.uv] / 10).ToString());

            if (recbuf[(byte)UV.subtype] == (byte)UV.UV3)
            {
                if (((byte)UV.tempsign & 0x80) == 0)
                {
                    logging.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Level", (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
                else
                {
                    logging.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Level", (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
            }
            if (recbuf[(byte)UV.uv] < 3)
            {
                logging.AddToLog("Description = Low", false);
                osae.ObjectPropertySet(obj.Name, "Description", "Low");
            }
            else if (recbuf[(byte)UV.uv] < 6)
            {
                logging.AddToLog("Description = Medium", false);
                osae.ObjectPropertySet(obj.Name, "Description", "Medium");
            }
            else if (recbuf[(byte)UV.uv] < 8)
            {
                logging.AddToLog("Description = High", false);
                osae.ObjectPropertySet(obj.Name, "Description", "High");
            }
            else if (recbuf[(byte)UV.uv] < 11)
            {
                logging.AddToLog("Description = Very high", false);
                osae.ObjectPropertySet(obj.Name, "Description", "Very high");
            }
            else
            {
                logging.AddToLog("Description = Dangerous", false);
                osae.ObjectPropertySet(obj.Name, "Description", "Dangerous");
            }
            logging.AddToLog("Signal level  = " + (recbuf[(byte)UV.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)UV.battery_level] & 0xf) == 0)
            {
                logging.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                logging.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }


        public void decode_DateTime()
        {
            logging.AddToLog("DateTime Not implemented", true);
        }

        public void decode_Current()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New Current meter found.  Adding to OSA", true);
                osae.ObjectAdd("Current Meter - " + (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString(), "Current Meter", "CURRENT METER", (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString());
            }

            switch (recbuf[(byte)CURRENT.subtype])
            {
                case (byte)CURRENT.ELEC1:
                    logging.AddToLog("subtype       = ELEC1 - OWL CM113, Electrisave, cent-a-meter", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)CURRENT.packettype] + ":" + recbuf[(byte)CURRENT.subtype], false);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)CURRENT.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString(), false);
            logging.AddToLog("Count         = " + recbuf[5].ToString(), false);
            logging.AddToLog("Channel 1     = " + ((recbuf[(byte)CURRENT.ch1h] * 256 + recbuf[(byte)CURRENT.ch1l]) / 10).ToString() + " ampere", false);
            logging.AddToLog("Channel 2     = " + ((recbuf[(byte)CURRENT.ch2h] * 256 + recbuf[(byte)CURRENT.ch2l]) / 10).ToString() + " ampere", false);
            logging.AddToLog("Channel 3     = " + ((recbuf[(byte)CURRENT.ch3h] * 256 + recbuf[(byte)CURRENT.ch3l]) / 10).ToString() + " ampere", false);
            osae.ObjectPropertySet(obj.Name, "Count", recbuf[5].ToString());
            osae.ObjectPropertySet(obj.Name, "Channel 1", ((recbuf[(byte)CURRENT.ch1h] * 256 + recbuf[(byte)CURRENT.ch1l]) / 10).ToString());
            osae.ObjectPropertySet(obj.Name, "Channel 2", ((recbuf[(byte)CURRENT.ch2h] * 256 + recbuf[(byte)CURRENT.ch2l]) / 10).ToString());
            osae.ObjectPropertySet(obj.Name, "Channel 3", ((recbuf[(byte)CURRENT.ch3h] * 256 + recbuf[(byte)CURRENT.ch3l]) / 10).ToString());

            logging.AddToLog("Signal level  = " + (recbuf[(byte)CURRENT.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)CURRENT.battery_level] & 0xf) == 0)
            {
                logging.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                logging.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Energy()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New Energy meter found.  Adding to OSA", true);
                osae.ObjectAdd("Energy Meter - " + (recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString(), "Energy Meter", "ENERGY METER", (recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString());
            }

            long instant = 0;
            double usage = 0;

            instant = Convert.ToInt64(recbuf[(byte)ENERGY.instant1]) * 0x1000000 + recbuf[(byte)ENERGY.instant2] * 0x10000 + recbuf[(byte)ENERGY.instant3] * 0x100 + recbuf[(byte)ENERGY.instant4];
            usage = (Convert.ToDouble(recbuf[(byte)ENERGY.total1]) * 0x10000000000L + Convert.ToDouble(recbuf[(byte)ENERGY.total2]) * 0x100000000L + Convert.ToDouble(recbuf[(byte)ENERGY.total3]) * 0x1000000 + recbuf[(byte)ENERGY.total4] * 0x10000 + recbuf[(byte)ENERGY.total5] * 0x100 + recbuf[(byte)ENERGY.total6]) / 223.666;

            switch (recbuf[(byte)ENERGY.subtype])
            {
                case (byte)ENERGY.ELEC2:
                    logging.AddToLog("subtype       = ELEC2 - OWL CM119, CM160", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)ENERGY.packettype] + ":" + recbuf[(byte)ENERGY.subtype], false);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)ENERGY.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString(), false);
            logging.AddToLog("Count         = " + recbuf[(byte)ENERGY.count].ToString(), false);
            logging.AddToLog("Instant usage = " + instant.ToString() + " Watt", false);
            logging.AddToLog("total usage   = " + usage.ToString() + " Wh", false);
            osae.ObjectPropertySet(obj.Name, "Count", recbuf[5].ToString());
            osae.ObjectPropertySet(obj.Name, "Instant usage", instant.ToString());
            osae.ObjectPropertySet(obj.Name, "Total usage", usage.ToString());

            logging.AddToLog("Signal level  = " + (recbuf[(byte)ENERGY.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)ENERGY.battery_level] & 0xf) == 0)
            {
                logging.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                logging.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Gas()
        {
            logging.AddToLog("Gas Not implemented", false);
        }

        public void decode_Water()
        {
            logging.AddToLog("Water Not implemented", false);
        }

        public void decode_Weight()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                logging.AddToLog("New Scale found.  Adding to OSA", true);
                osae.ObjectAdd("Scale Meter - " + (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString(), "Scale Meter", "SCALE", (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString());
            }
            
            switch (recbuf[(byte)WEIGHT.subtype])
            {
                case (byte)WEIGHT.WEIGHT1:
                    logging.AddToLog("subtype       = BWR102", false);
                    break;
                case (byte)WEIGHT.WEIGHT2:
                    logging.AddToLog("subtype       = GR101", false);
                    break;
                default:
                    logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)WEIGHT.packettype] + ":" + recbuf[(byte)WEIGHT.subtype], false);
                    break;
            }
            logging.AddToLog("Sequence nbr  = " + recbuf[(byte)WEIGHT.seqnbr].ToString(), false);
            logging.AddToLog("ID            = " + (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString(), false);
            logging.AddToLog("Weight        = " + (((recbuf[(byte)WEIGHT.weighthigh] * 25.6) + recbuf[(byte)WEIGHT.weightlow] / 10).ToString() + 2.2).ToString() + " lb", false);
            logging.AddToLog("Signal level  = " + (recbuf[(byte)WEIGHT.rssi] >> 4).ToString(), false);

            osae.ObjectPropertySet(obj.Name, "Weight", (((recbuf[(byte)WEIGHT.weighthigh] * 25.6) + recbuf[(byte)WEIGHT.weightlow] / 10).ToString() + 2.2).ToString());
        }

        //public void decode_RFXSensor()
        //{
        //    switch (recbuf(RFXSENSOR.subtype))
        //    {
        //        case RFXSENSOR.Temp:
        //            logging.AddToLog("subtype       = Temperature");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            //positive temperature?
        //            if ((recbuf(RFXSENSOR.msg1) & 0x80) == 0)
        //            {
        //                logging.AddToLog("msg           = " + Math.Round(((recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)) / 100), 2).ToString() + " °C");
        //            }
        //            else
        //            {
        //                logging.AddToLog("msg           = " + Math.Round((0 - ((recbuf(RFXSENSOR.msg1) & 0x7f) * 256 + recbuf(RFXSENSOR.msg2)) / 100), 2).ToString() + " °C");
        //            }
        //            break;
        //        case RFXSENSOR.AD:
        //            logging.AddToLog("subtype       = A/D");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            logging.AddToLog("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString() + " mV");
        //            break;
        //        case RFXSENSOR.Volt:
        //            logging.AddToLog("subtype       = Voltage");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            logging.AddToLog("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString() + " mV");
        //            break;
        //        case RFXSENSOR.Message:
        //            logging.AddToLog("subtype       = Message");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            switch (recbuf(RFXSENSOR.msg2))
        //            {
        //                case 0x1:
        //                    logging.AddToLog("msg           = sensor addresses incremented");
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("msg           = battery low detected");
        //                    break;
        //                case 0x81:
        //                    logging.AddToLog("msg           = no 1-wire device connected");
        //                    break;
        //                case 0x82:
        //                    logging.AddToLog("msg           = 1-Wire ROM CRC error");
        //                    break;
        //                case 0x83:
        //                    logging.AddToLog("msg           = 1-Wire device connected is not a DS18B20 or DS2438");
        //                    break;
        //                case 0x84:
        //                    logging.AddToLog("msg           = no end of read signal received from 1-Wire device");
        //                    break;
        //                case 0x85:
        //                    logging.AddToLog("msg           = 1-Wire scratchpad CRC error");
        //                    break;
        //                default:
        //                    logging.AddToLog("ERROR: unknown message");
        //                    break;
        //            }

        //            logging.AddToLog("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString());
        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(RFXSENSOR.packettype)) + ":" + Conversion.Hex(recbuf(RFXSENSOR.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Signal level  = " + (recbuf(RFXSENSOR.rssi) >> 4).ToString());

        //}

        //public void decode_RFXMeter()
        //{
        //    long counter = 0;

        //    switch (recbuf(RFXMETER.subtype))
        //    {
        //        case RFXMETER.Count:
        //            logging.AddToLog("subtype       = RFXMeter counter");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            counter = (Convert.ToInt64(recbuf(RFXMETER.count1)) << 24) + (Convert.ToInt64(recbuf(RFXMETER.count2)) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4);
        //            logging.AddToLog("Counter       = " + counter.ToString());
        //            logging.AddToLog("if RFXPwr     = " + (counter / 1000).ToString() + " kWh");
        //            break;
        //        case RFXMETER.Interval:
        //            logging.AddToLog("subtype       = RFXMeter new interval time set");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            logging.AddToLog("Interval time = ", false);
        //            switch (recbuf(RFXMETER.count3))
        //            {
        //                case 0x1:
        //                    logging.AddToLog("30 seconds");
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("1 minute");
        //                    break;
        //                case 0x4:
        //                    logging.AddToLog("6 minutes");
        //                    break;
        //                case 0x8:
        //                    logging.AddToLog("12 minutes");
        //                    break;
        //                case 0x10:
        //                    logging.AddToLog("15 minutes");
        //                    break;
        //                case 0x20:
        //                    logging.AddToLog("30 minutes");
        //                    break;
        //                case 0x40:
        //                    logging.AddToLog("45 minutes");
        //                    break;
        //                case 0x80:
        //                    logging.AddToLog("1 hour");
        //                    break;
        //            }

        //            break;
        //        case RFXMETER.Calib:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("subtype       = Calibrate mode for channel 1");
        //                    break;
        //                case 0x40:
        //                    logging.AddToLog("subtype       = Calibrate mode for channel 2");
        //                    break;
        //                case 0x80:
        //                    logging.AddToLog("subtype       = Calibrate mode for channel 3");
        //                    break;
        //            }
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            counter = ((Convert.ToInt64(recbuf(RFXMETER.count2) & 0x3f) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)) / 1000;
        //            logging.AddToLog("Calibrate cnt = " + counter.ToString() + " msec");
        //            logging.AddToLog("RFXPwr        = " + Convert.ToString(Round(1 / ((16 * counter) / (3600000 / 62.5)), 3)) + " kW", false);
        //            break;
        //        case RFXMETER.Addr:
        //            logging.AddToLog("subtype       = New address set, push button for next address");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.CounterReset:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 1 will be executed");
        //                    break;
        //                case 0x40:
        //                    logging.AddToLog("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 2 will be executed");
        //                    break;
        //                case 0x80:
        //                    logging.AddToLog("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 3 will be executed");
        //                    break;
        //            }
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.CounterSet:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("subtype       = Counter channel 1 is reset to zero");
        //                    break;
        //                case 0x40:
        //                    logging.AddToLog("subtype       = Counter channel 2 is reset to zero");
        //                    break;
        //                case 0x80:
        //                    logging.AddToLog("subtype       = Counter channel 3 is reset to zero");
        //                    break;
        //            }
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            logging.AddToLog("Counter       = " + ((Convert.ToInt64(recbuf(RFXMETER.count1)) << 24) + (Convert.ToInt64(recbuf(RFXMETER.count2)) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)).ToString());

        //            break;
        //        case RFXMETER.SetInterval:
        //            logging.AddToLog("subtype       = Push the button for next mode within 5 seconds or else SET INTERVAL MODE will be entered");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.SetCalib:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 1 will be executed");
        //                    break;
        //                case 0x40:
        //                    logging.AddToLog("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 2 will be executed");
        //                    break;
        //                case 0x80:
        //                    logging.AddToLog("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 3 will be executed");
        //                    break;
        //            }
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.SetAddr:
        //            logging.AddToLog("subtype       = Push the button for next mode within 5 seconds or else SET ADDRESS MODE will be entered");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.Ident:
        //            logging.AddToLog("subtype       = RFXMeter identification");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            logging.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            logging.AddToLog("FW version    = " + Conversion.Hex(recbuf(RFXMETER.count3)));
        //            logging.AddToLog("Interval time = ", false);
        //            switch (recbuf(RFXMETER.count4))
        //            {
        //                case 0x1:
        //                    logging.AddToLog("30 seconds");
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("1 minute");
        //                    break;
        //                case 0x4:
        //                    logging.AddToLog("6 minutes");
        //                    break;
        //                case 0x8:
        //                    logging.AddToLog("12 minutes");
        //                    break;
        //                case 0x10:
        //                    logging.AddToLog("15 minutes");
        //                    break;
        //                case 0x20:
        //                    logging.AddToLog("30 minutes");
        //                    break;
        //                case 0x40:
        //                    logging.AddToLog("45 minutes");
        //                    break;
        //                case 0x80:
        //                    logging.AddToLog("1 hour");
        //                    break;
        //            }
        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(RFXMETER.packettype)) + ":" + Conversion.Hex(recbuf(RFXMETER.subtype)));
        //            break;
        //    }

        //    logging.AddToLog("Signal level  = " + (recbuf(RFXMETER.rssi) >> 4).ToString());
        //}

        //public void decode_FS20()
        //{
        //    switch (recbuf(FS20.subtype))
        //    {
        //        case FS20.sTypeFS20:
        //            logging.AddToLog("subtype       = FS20");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            logging.AddToLog("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            logging.AddToLog("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            logging.AddToLog("Cmd1          = ", false);
        //            switch ((recbuf(FS20.cmd1) & 0x1f))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("Off");
        //                    break;
        //                case 0x1:
        //                    logging.AddToLog("dim level 1 = 6.25%");
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("dim level 2 = 12.5%");
        //                    break;
        //                case 0x3:
        //                    logging.AddToLog("dim level 3 = 18.75%");
        //                    break;
        //                case 0x4:
        //                    logging.AddToLog("dim level 4 = 25%");
        //                    break;
        //                case 0x5:
        //                    logging.AddToLog("dim level 5 = 31.25%");
        //                    break;
        //                case 0x6:
        //                    logging.AddToLog("dim level 6 = 37.5%");
        //                    break;
        //                case 0x7:
        //                    logging.AddToLog("dim level 7 = 43.75%");
        //                    break;
        //                case 0x8:
        //                    logging.AddToLog("dim level 8 = 50%");
        //                    break;
        //                case 0x9:
        //                    logging.AddToLog("dim level 9 = 56.25%");
        //                    break;
        //                case 0xa:
        //                    logging.AddToLog("dim level 10 = 62.5%");
        //                    break;
        //                case 0xb:
        //                    logging.AddToLog("dim level 11 = 68.75%");
        //                    break;
        //                case 0xc:
        //                    logging.AddToLog("dim level 12 = 75%");
        //                    break;
        //                case 0xd:
        //                    logging.AddToLog("dim level 13 = 81.25%");
        //                    break;
        //                case 0xe:
        //                    logging.AddToLog("dim level 14 = 87.5%");
        //                    break;
        //                case 0xf:
        //                    logging.AddToLog("dim level 15 = 93.75%");
        //                    break;
        //                case 0x10:
        //                    logging.AddToLog("On (100%)");
        //                    break;
        //                case 0x11:
        //                    logging.AddToLog("On ( at last dim level set)");
        //                    break;
        //                case 0x12:
        //                    logging.AddToLog("Toggle between Off and On (last dim level set)");
        //                    break;
        //                case 0x13:
        //                    logging.AddToLog("Bright one step");
        //                    break;
        //                case 0x14:
        //                    logging.AddToLog("Dim one step");
        //                    break;
        //                case 0x15:
        //                    logging.AddToLog("Start dim cycle");
        //                    break;
        //                case 0x16:
        //                    logging.AddToLog("Program(Timer)");
        //                    break;
        //                case 0x17:
        //                    logging.AddToLog("Request status from a bidirectional device");
        //                    break;
        //                case 0x18:
        //                    logging.AddToLog("Off for timer period");
        //                    break;
        //                case 0x19:
        //                    logging.AddToLog("On (100%) for timer period");
        //                    break;
        //                case 0x1a:
        //                    logging.AddToLog("On ( at last dim level set) for timer period");
        //                    break;
        //                case 0x1b:
        //                    logging.AddToLog("Reset");
        //                    break;
        //                default:
        //                    logging.AddToLog("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                logging.AddToLog("                command to receiver");
        //            }
        //            else
        //            {
        //                logging.AddToLog("                response from receiver");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x40) == 0)
        //            {
        //                logging.AddToLog("                unidirectional command");
        //            }
        //            else
        //            {
        //                logging.AddToLog("                bidirectional command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x20) == 0)
        //            {
        //                logging.AddToLog("                additional cmd2 byte not present");
        //            }
        //            else
        //            {
        //                logging.AddToLog("                additional cmd2 byte present");
        //            }

        //            if ((recbuf(FS20.cmd1) & 0x20) != 0)
        //            {
        //                logging.AddToLog("Cmd2          = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2));
        //            }

        //            break;
        //        case FS20.sTypeFHT8V:
        //            logging.AddToLog("subtype       = FHT 8V valve");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            logging.AddToLog("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            logging.AddToLog("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            logging.AddToLog("Cmd1          = ", false);
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                logging.AddToLog("new command");
        //            }
        //            else
        //            {
        //                logging.AddToLog("repeated command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x40) == 0)
        //            {
        //                logging.AddToLog("                unidirectional command");
        //            }
        //            else
        //            {
        //                logging.AddToLog("                bidirectional command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x20) == 0)
        //            {
        //                logging.AddToLog("                additional cmd2 byte not present");
        //            }
        //            else
        //            {
        //                logging.AddToLog("                additional cmd2 byte present");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x10) == 0)
        //            {
        //                logging.AddToLog("                battery empty beep not enabled");
        //            }
        //            else
        //            {
        //                logging.AddToLog("                enable battery empty beep");
        //            }
        //            switch ((recbuf(FS20.cmd1) & 0xf))
        //            {
        //                case 0x0:
        //                    logging.AddToLog("                Synchronize now");
        //                    logging.AddToLog("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0x1:
        //                    logging.AddToLog("                open valve");
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("                close valve");
        //                    break;
        //                case 0x6:
        //                    logging.AddToLog("                open valve at percentage level");
        //                    logging.AddToLog("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0x8:
        //                    logging.AddToLog("                relative offset (cmd2 bit 7=direction, bit 5-0 offset value)");
        //                    break;
        //                case 0xa:
        //                    logging.AddToLog("                decalcification cycle");
        //                    logging.AddToLog("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0xc:
        //                    logging.AddToLog("                synchronization active");
        //                    logging.AddToLog("Cmd2          = count down is " + (recbuf(FS20.cmd2) >> 1).ToString() + " seconds");
        //                    break;
        //                case 0xe:
        //                    logging.AddToLog("                test, drive valve and produce an audible signal");
        //                    break;
        //                case 0xf:
        //                    logging.AddToLog("                pair valve (cmd2 bit 7-1 is count down in seconds, bit 0=1)");
        //                    logging.AddToLog("Cmd2          = count down is " + recbuf(FS20.cmd2) >> 1 + " seconds");
        //                    break;
        //                default:
        //                    logging.AddToLog("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }

        //            break;
        //        case FS20.sTypeFHT80:
        //            logging.AddToLog("subtype       = FHT80 door/window sensor");
        //            logging.AddToLog("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            logging.AddToLog("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            logging.AddToLog("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            logging.AddToLog("Cmd1          = ", false);
        //            switch ((recbuf(FS20.cmd1) & 0xf))
        //            {
        //                case 0x1:
        //                    logging.AddToLog("sensor opened");
        //                    break;
        //                case 0x2:
        //                    logging.AddToLog("sensor closed");
        //                    break;
        //                case 0xc:
        //                    logging.AddToLog("synchronization active");
        //                    break;
        //                default:
        //                    logging.AddToLog("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                logging.AddToLog("                new command");
        //            }
        //            else
        //            {
        //                logging.AddToLog("                repeated command");
        //            }

        //            break;
        //        default:
        //            logging.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(FS20.packettype)) + ":" + Conversion.Hex(recbuf(FS20.subtype)));
        //            break;
        //    }
        //    logging.AddToLog("Signal level  = " + (recbuf(FS20.rssi) >> 4).ToString());
        //}

        #endregion


        # region Enumerations

        enum ICMD : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            cmnd = 4,
            msg1 = 5,
            msg2 = 6,
            msg3 = 7,
            msg4 = 8,
            msg5 = 9,
            msg6 = 10,
            msg7 = 11,
            msg8 = 12,
            msg9 = 13,
            size = 13,

            //Interface Control
            pType = 0x0,
            sType = 0x0,

            //Interface commands
            RESET = 0x0,
            // reset the receiver/transceiver
            STATUS = 0x2,
            // request firmware versions and configuration of the interface
            SETMODE = 0x3,
            // set the configuration of the interface
            ENABLEALL = 0x4,
            // enable all receiving modes of the receiver/transceiver
            UNDECODED = 0x5,
            // display UNDECODEDoded messages
            SAVE = 0x6,
            // save receiving modes of the receiver/transceiver in non-volatile memory
            DISX10 = 0x10,
            // disable receiving of X10
            DISARC = 0x11,
            // disable receiving of ARC
            DISAC = 0x12,
            // disable receiving of AC
            DISHEU = 0x13,
            // disable receiving of HomeEasy EU
            DISKOP = 0x14,
            // disable receiving of Ikea-Koppla
            DISOREGON = 0x15,
            // disable receiving of Oregon Scientific
            DISATI = 0x16,
            // disable receiving of ATI Remote Wonder
            DISVISONIC = 0x17,
            // disable receiving of Visonic
            DISMERTIK = 0x18,
            // disable receiving of Mertik
            DISAD = 0x19,
            // disable receiving of AD
            DISHID = 0x1a,
            // disable receiving of Hideki
            DISLCROS = 0x1b,
            // disable receiving of La Crosse
            DISFS20 = 0x1c,
            // disable receiving of FS20

            sel310 = 0x50,
            // select 310MHz in the 310/315 transceiver
            sel315 = 0x51,
            // select 315MHz in the 310/315 transceiver
            sel800 = 0x55,
            // select 868.00MHz ASK in the 868 transceiver
            sel800F = 0x56,
            // select 868.00MHz FSK in the 868 transceiver
            sel830 = 0x57,
            // select 868.30MHz ASK in the 868 transceiver
            sel830F = 0x58,
            // select 868.30MHz FSK in the 868 transceiver
            sel835 = 0x59,
            // select 868.35MHz ASK in the 868 transceiver
            sel835F = 0x5a,
            // select 868.35MHz FSK in the 868 transceiver
            sel895 = 0x5b
            // select 868.95MHz in the 868 transceiver
        }

        enum IRESPONSE : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            cmnd = 4,
            msg1 = 5,
            msg2 = 6,
            msg3 = 7,
            msg4 = 8,
            msg5 = 9,
            msg6 = 10,
            msg7 = 11,
            msg8 = 12,
            msg9 = 13,
            size = 13,

            pType = 0x1,
            sType = 0x0,
            recType310 = 0x50,
            recType315 = 0x51,
            recType43392 = 0x52,
            trxType43392 = 0x53,
            recType86800 = 0x55,
            recType86800FSK = 0x56,
            recType86830 = 0x57,
            recType86830FSK = 0x58,
            recType86835 = 0x59,
            recType86835FSK = 0x5a,
            recType86895 = 0x5b,

            msg3_undec = 0x80,

            msg4_PROGUARD = 0x20,
            msg4_FS20 = 0x10,
            msg4_LCROS = 0x8,
            msg4_HID = 0x4,
            msg4_AD = 0x2,
            msg4_MERTIK = 0x1,

            msg5_VISONIC = 0x80,
            msg5_ATI = 0x40,
            msg5_OREGON = 0x20,
            msg5_KOP = 0x10,
            msg5_HEU = 0x8,
            msg5_AC = 0x4,
            msg5_ARC = 0x2,
            msg5_X10 = 0x1
        }

        enum RXRESPONSE : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            msg = 4,
            size = 4,

            pType = 0x2,
            sTypeReceiverLockError = 0x0,
            sTypeTransmitterResponse = 0x1
        }

        enum UNDECODED : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            msg1 = 4,
            //msg2 to msg32 depending on RF packet length
            size = 36,
            //maximum size

            pType = 0x3,
            sTypeUac = 0x0,
            sTypeUarc = 0x1,
            sTypeUati = 0x2,
            sTypeUhideki = 0x3,
            sTypeUlacrosse = 0x4,
            sTypeUlwrf = 0x5,
            sTypeUmertik = 0x6,
            sTypeUoregon1 = 0x7,
            sTypeUoregon2 = 0x8,
            sTypeUoregon3 = 0x9,
            sTypeUproguard = 0xa,
            sTypeUvisonic = 0xb,
            sTypeUnec = 0xc,
            sTypeUfs20 = 0xd
        }

        enum LIGHTING1 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            housecode = 4,
            unitcode = 5,
            cmnd = 6,
            filler = 7,
            //bits 3-0
            rssi = 7,
            //bits 7-4
            size = 7,

            pType = 0x10,
            sTypeX10 = 0x0,
            sTypeARC = 0x1,
            sTypeAB400D = 0x2,
            sTypeWaveman = 0x3,
            sTypeEMW200 = 0x4,
            sTypeIMPULS = 0x5,
            sTypeRisingSun = 0x6,

            sOff = 0,
            sOn = 1,
            sDim = 2,
            sBright = 3,
            sAllOff = 5,
            sAllOn = 6,
            sChime = 7
        }

        enum LIGHTING2 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            id3 = 6,
            id4 = 7,
            unitcode = 8,
            cmnd = 9,
            level = 10,
            filler = 11,
            //bits 3-0
            rssi = 11,
            //bits 7-4
            size = 11,

            pType = 0x11,
            sTypeAC = 0x0,
            sTypeHEU = 0x1,
            sTypeANSLUT = 0x2,

            sOff = 0,
            sOn = 1,
            sSetLevel = 2,
            sGroupOff = 3,
            sGroupOn = 4,
            sSetGroupLevel = 5
        }

        enum LIGHTING3 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            system = 4,
            channel8_1 = 5,
            channel10_9 = 6,
            cmnd = 7,
            filler = 8,
            //bits 3-0
            rssi = 8,
            //bits 7-4
            size = 8,

            pType = 0x12,
            sTypeKoppla = 0x0,

            sBright = 0x0,
            sDim = 0x8,
            sOn = 0x10,
            sLevel1 = 0x11,
            sLevel2 = 0x12,
            sLevel3 = 0x13,
            sLevel4 = 0x14,
            sLevel5 = 0x15,
            sLevel6 = 0x16,
            sLevel7 = 0x17,
            sLevel8 = 0x18,
            sLevel9 = 0x19,
            sOff = 0x1a,
            sProgram = 0x1c
        }

        enum LIGHTING4 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            cmd1 = 4,
            cmd2 = 5,
            cmd3 = 6,
            pulsehigh = 7,
            pulselow = 8,
            filler = 9,
            size = 9,

            pType = 0x13,
            sTypePT2262 = 0x0
        }

        enum LIGHTING5 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            id3 = 6,
            unitcode = 7,
            cmnd = 8,
            level = 9,
            filler = 10,
            //bits 3-0
            rssi = 10,
            //bits 7-4
            size = 10,

            pType = 0x14,
            sTypeLightwaveRF = 0x0,
            sTypeEMW100 = 0x1,

            sOff = 0,
            sOn = 1,
            sGroupOff = 2,
            sLearn = 2,
            sMood1 = 3,
            sMood2 = 4,
            sMood3 = 5,
            sMood4 = 6,
            sMood5 = 7,
            sUnlock = 10,
            sLock = 11,
            sAllLock = 12,
            sClose = 13,
            sStop = 14,
            sOpen = 15,
            sSetLevel = 16
        }

        enum CURTAIN1 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            housecode = 4,
            unitcode = 5,
            cmnd = 6,
            filler = 7,
            size = 7,

            //types for Curtain
            pType = 0x18,
            Harrison = 0x0,

            sOpen = 0,
            sClose = 1,
            sStop = 2,
            sProgram = 3
        }

        enum SECURITY1 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            id3 = 6,
            status = 7,
            battery_level = 8,
            //bits 3-0
            rssi = 8,
            //bits 7-4
            filler = 8,
            size = 8,

            //Security
            pType = 0x20,
            SecX10 = 0x0,
            SecX10M = 0x1,
            SecX10R = 0x2,
            KD101 = 0x3,
            PowercodeSensor = 0x4,
            PowercodeMotion = 0x5,
            Codesecure = 0x6,
            PowercodeAux = 0x7,

            //status security
            sStatusNormal = 0x0,
            sStatusNormalDelayed = 0x1,
            sStatusAlarm = 0x2,
            sStatusAlarmDelayed = 0x3,
            sStatusMotion = 0x4,
            sStatusNoMotion = 0x5,
            sStatusPanic = 0x6,
            sStatusPanicOff = 0x7,
            sStatusTamper = 0x8,
            sStatusArmAway = 0x9,
            sStatusArmAwayDelayed = 0xa,
            sStatusArmHome = 0xb,
            sStatusArmHomeDelayed = 0xc,
            sStatusDisarm = 0xd,
            sStatusLightOff = 0x10,
            sStatusLightOn = 0x11,
            sStatusLIGHTING2Off = 0x12,
            sStatusLIGHTING2On = 0x13,
            sStatusDark = 0x14,
            sStatusLight = 0x15,
            sStatusBatLow = 0x16,
            sStatusPairKD101 = 0x17
        }

        enum CAMERA1 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            housecode = 4,
            cmnd = 5,
            filler = 6,
            //bits 3-0
            rssi = 6,
            //bits 7-4
            size = 6,

            //Camera1
            pType = 0x28,
            Ninja = 0x0,

            sLeft = 0,
            sRight = 1,
            sUp = 2,
            sDown = 3,
            sPosition1 = 4,
            sProgramPosition1 = 5,
            sPosition2 = 6,
            sProgramPosition2 = 7,
            sPosition3 = 8,
            sProgramPosition3 = 9,
            sPosition4 = 10,
            sProgramPosition4 = 11,
            sCenter = 12,
            sProgramCenterPosition = 13,
            sSweep = 14,
            sProgramSweep = 15
        }

        enum REMOTE : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id = 4,
            cmnd = 5,
            toggle = 6,
            //bit 0
            filler = 6,
            //bits 3-1
            rssi = 6,
            //bits 7-4
            size = 6,

            //Remotes
            pType = 0x30,
            ATI = 0x0,
            ATI2 = 0x1,
            Medion = 0x2,
            PCremote = 0x3
        }

        enum THERMOSTAT1 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            temperature = 6,
            set_point = 7,
            status = 8,
            //bits 1-0
            filler = 8,
            //bits 6-2
            mode = 8,
            //bit 7
            battery_level = 9,
            //bits 3-0
            rssi = 9,
            //bits 7-4
            size = 9,

            //Thermostat1
            pType = 0x40,
            Digimax = 0x0,
            //Digimax with long packet 
            DigimaxShort = 0x1
            //Digimax with short packet (no set point)
        }

        enum THERMOSTAT2 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            unitcode = 4,
            cmnd = 5,
            filler = 6,
            //bits 3-0
            rssi = 6,
            //bits 7-4
            size = 6,

            //Thermostat2
            pType = 0x41,
            HE105 = 0x0,
            //HE105
            RTS10 = 0x1,
            //RTS10

            sOff = 0,
            sOn = 1,
            sProgram = 2
        }

        enum THERMOSTAT3 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            unitcode1 = 4,
            unitcode2 = 5,
            unitcode3 = 6,
            cmnd = 7,
            filler = 9,
            //bits 3-0
            rssi = 9,
            //bits 7-4
            size = 9,

            //Thermostat3
            pType = 0x42,
            MertikG6RH4T1 = 0x0,
            //Mertik G6R-H4T1
            MertikG6RH4TB = 0x1,
            //Mertik G6R-H4TB

            sOff = 0,
            sOn = 1,
            sUp = 2,
            sDown = 3,
            sRunUp = 4,
            Off2nd = 4,
            sRunDown = 5,
            On2nd = 5,
            sStop = 6
        }

        enum TEMP : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            temperatureh = 6,
            //bits 6-0
            tempsign = 6,
            //bit 7
            temperaturel = 7,
            battery_level = 8,
            //bits 3-0
            rssi = 8,
            //bits 7-4
            size = 8,

            //Temperature
            pType = 0x50,
            TEMP1 = 0x1,
            //THR128/138, THC138
            TEMP2 = 0x2,
            //THC238/268,THN132,THWR288,THRN122,THN122,AW129/131
            TEMP3 = 0x3,
            //THWR800
            TEMP4 = 0x4,
            //RTHN318
            TEMP5 = 0x5,
            //LaCrosse TX3
            TEMP6 = 0x6
            //TS15C
        }

        enum HUM : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            humidity = 6,
            humidity_status = 7,
            battery_level = 8,
            //bits 3-0
            rssi = 8,
            //bits 7-4
            size = 8,

            //humidity
            pType = 0x51,
            HUM1 = 0x1
            //LaCrosse TX3
        }

        enum TEMP_HUM : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            temperatureh = 6,
            //bits 6-0
            tempsign = 6,
            //bit 7
            temperaturel = 7,
            humidity = 8,
            humidity_status = 9,
            battery_level = 10,
            //bits 3-0
            rssi = 10,
            //bits 7-4
            size = 10,

            //temperature+humidity
            pType = 0x52,
            TH1 = 0x1,
            //THGN122/123,/THGN132,THGR122/228/238/268
            TH2 = 0x2,
            //THGR810/THGN800
            TH3 = 0x3,
            //RTGR328
            TH4 = 0x4,
            //THGR328
            TH5 = 0x5,
            //WTGR800
            TH6 = 0x6,
            //THGR918,THGRN228,THGN500
            TH7 = 0x7,
            //TFA TS34C, Cresta
            TH8 = 0x8
            //Esic
        }

        enum BARO : byte
        {
            //barometric
            pType = 0x53
            //not used
        }

        enum TEMP_HUM_BARO : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            temperatureh = 6,
            //bits 6-0
            tempsign = 6,
            //bit 7
            temperaturel = 7,
            humidity = 8,
            humidity_status = 9,
            baroh = 10,
            barol = 11,
            forecast = 12,
            battery_level = 13,
            //bits 3-0
            rssi = 13,
            //bits 7-4
            size = 13,

            //temperature+humidity+baro
            pType = 0x54,
            THB1 = 0x1,
            //BTHR918
            THB2 = 0x2
            //BTHR918N,BTHR968
        }

        enum RAIN : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            rainrateh = 6,
            rainratel = 7,
            raintotal1 = 8,
            raintotal2 = 9,
            raintotal3 = 10,
            battery_level = 11,
            //bits 3-0
            rssi = 11,
            //bits 7-4
            size = 11,

            //rain
            pType = 0x55,
            RAIN1 = 0x1,
            //RGR126/682/918
            RAIN2 = 0x2,
            //PCR800
            RAIN3 = 0x3,
            //TFA
            RAIN4 = 0x4
            //UPM
        }

        enum WIND : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            directionh = 6,
            directionl = 7,
            av_speedh = 8,
            av_speedl = 9,
            gusth = 10,
            gustl = 11,
            temperatureh = 12,
            //bits 6-0
            tempsign = 12,
            //bit 7
            temperaturel = 13,
            chillh = 14,
            //bits 6-0
            chillsign = 14,
            //bit 7
            chilll = 15,
            battery_level = 16,
            //bits 3-0
            rssi = 16,
            //bits 7-4
            size = 16,

            //wind
            pType = 0x56,
            WIND1 = 0x1,
            //WTGR800
            WIND2 = 0x2,
            //WGR800
            WIND3 = 0x3,
            //STR918,WGR918
            WIND4 = 0x4,
            //TFA
            WIND5 = 0x5
            //UPM
        }

        enum UV : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            uv = 6,
            temperatureh = 7,
            //bits 6-0
            tempsign = 7,
            //bit 7
            temperaturel = 8,
            battery_level = 9,
            //bits 3-0
            rssi = 9,
            //bits 7-4
            size = 9,

            //uv
            pType = 0x57,
            UV1 = 0x1,
            //UVN128,UV138
            UV2 = 0x2,
            //UVN800
            UV3 = 0x3
            //TFA
        }

        enum DT : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            yy = 6,
            mm = 7,
            dd = 8,
            dow = 9,
            hr = 10,
            min = 11,
            sec = 12,
            battery_level = 13,
            //bits 3-0
            rssi = 13,
            //bits 7-4
            size = 13,

            //date & time
            pType = 0x58,
            DT1 = 0x1
            //RTGR328N
        }

        enum CURRENT : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            count = 6,
            ch1h = 7,
            ch1l = 8,
            ch2h = 9,
            ch2l = 10,
            ch3h = 11,
            ch3l = 12,
            battery_level = 13,
            //bits 3-0
            rssi = 13,
            //bits 7-4
            size = 13,

            //current
            pType = 0x59,
            ELEC1 = 0x1
            //CM113,Electrisave
        }

        enum ENERGY : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            count = 6,
            instant1 = 7,
            instant2 = 8,
            instant3 = 9,
            instant4 = 10,
            total1 = 11,
            total2 = 12,
            total3 = 13,
            total4 = 14,
            total5 = 15,
            total6 = 16,
            battery_level = 17,
            //bits 3-0
            rssi = 17,
            //bits 7-4
            size = 17,

            //energy
            pType = 0x5a,
            ELEC2 = 0x1
            //CM119/160
        }

        enum GAS : byte
        {
            //gas
            pType = 0x5b
            //not used
        }

        enum WATER : byte
        {
            //water
            pType = 0x5c
            //not used
        }

        enum WEIGHT : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            weighthigh = 6,
            weightlow = 7,
            filler = 8,
            //bits 3-0
            rssi = 8,
            //bits 7-4
            size = 8,

            //weight scales
            pType = 0x5d,
            WEIGHT1 = 0x1,
            //BWR102
            WEIGHT2 = 0x2
            //GR101
        }

        enum RFXSENSOR : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id = 4,
            msg1 = 5,
            msg2 = 6,
            filler = 7,
            //bits 3-0
            rssi = 7,
            //bits 7-4
            size = 7,

            //RFXSensor
            pType = 0x70,
            Temp = 0x0,
            AD = 0x1,
            Volt = 0x2,
            Message = 0x3
        }

        enum RFXMETER : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            id1 = 4,
            id2 = 5,
            count1 = 6,
            count2 = 7,
            count3 = 8,
            count4 = 9,
            filler = 10,
            //bits 3-0
            rssi = 10,
            //bits 7-4
            size = 10,

            //RFXMeter
            pType = 0x71,
            Count = 0x0,
            Interval = 0x1,
            Calib = 0x2,
            Addr = 0x3,
            CounterReset = 0x4,
            CounterSet = 0xb,
            SetInterval = 0xc,
            SetCalib = 0xd,
            SetAddr = 0xe,
            Ident = 0xf
        }

        enum FS20 : byte
        {
            packetlength = 0,
            packettype = 1,
            subtype = 2,
            seqnbr = 3,
            hc1 = 4,
            hc2 = 5,
            addr = 6,
            cmd1 = 7,
            cmd2 = 8,
            filler = 9,
            //bits 3-0
            rssi = 9,
            //bits 7-4
            size = 9,

            //FS20
            pType = 0x72,
            sTypeFS20 = 0x0,
            sTypeFHT8V = 0x1,
            sTypeFHT80 = 0x2
        }
    # endregion

    }
}
