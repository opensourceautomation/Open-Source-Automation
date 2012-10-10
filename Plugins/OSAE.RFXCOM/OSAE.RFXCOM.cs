using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.AddIn;
using System.Globalization;
using OpenSourceAutomation;

namespace OSAE.RFXCOM
{
    [AddIn("RFXCOM", Version = "0.2.8")]
    public class RFXCOM : IOpenSourceAutomationAddInv2
    {
        OSAE osae = new OSAE("RFXCOM");

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

        public void ProcessCommand(OSAEMethod method)
        {
            osae.AddToLog("--------------Processing Command---------------", false);
            osae.AddToLog("Command: " + method.MethodName, false);

            OSAEObject obj = osae.GetObjectByName(method.ObjectName);
            osae.AddToLog("Object Name: " + obj.Name, false);
            osae.AddToLog("Object Type: " + obj.Type, false);
            osae.AddToLog("Object Adress: " + obj.Address, false);

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
                        osae.AddToLog("Executing Lighting1 command", false);

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
                        osae.AddToLog("Lighting1 command:" + command, false);

                        break;
                    #endregion

                    #region Lighting 2

                    case "AC DIMMER SWITCH":
                    case "HEU DIMMER SWITCH":
                    case "ANSLUT DIMMER SWITCH":
                        osae.AddToLog("Executing Lighting2 command", false);

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
                        kar[(byte)LIGHTING2.unitcode] = (byte)Int32.Parse(addr[4], System.Globalization.NumberStyles.HexNumber);


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
                                    kar[(byte)LIGHTING2.level] = (byte)Math.Round((double)Int32.Parse(method.Parameter1) / 7, 0);
                                }
                                osae.ObjectStateSet(obj.Name, "ON");

                                break;
                        }

                        kar[(byte)LIGHTING2.filler] = 0;

                        osae.AddToLog("Writing command. len: " + kar.Length.ToString(), false);
                        WriteCom(kar);
                        string command_l2 = "";
                        foreach (byte bt in kar)
                        {
                            command_l2 += ("0" + bt.ToString("X")).Substring(("0" + bt.ToString("X")).Length - 2) + " ";
                        }
                        osae.AddToLog("Lighting2 command:" + command_l2, false);
                        break;

                    #endregion

                    #region Lighting 5

                    case "LIGHTWAVERF DIMMER SWITCH":
                    case "EMW100 BINARY SWITCH":
                        osae.AddToLog("Executing Lighting5 command", false);

                        kar = new byte[(byte)LIGHTING5.size + 1];
                        osae.AddToLog("Lighting 5 device", false);

                        if (bytFWversion < 29)
                        {
                            osae.AddToLog("RFXtrx433 firmware version must be > 28, flash your RFXtrx433 with the latest firmware", true);
                            return;
                        }

                        string[] l5_addr = obj.Address.Split('-');
                        if (l5_addr.Length != 4)
                        {
                            osae.AddToLog("invalid unit address", true);
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
                            osae.AddToLog("kar[(byte)LIGHTING5.packetlength]: " + kar[(byte)LIGHTING5.packetlength].ToString(), false);
                            kar[(byte)LIGHTING5.packettype] = (byte)LIGHTING5.pType;
                            osae.AddToLog("kar[(byte)LIGHTING5.packettype]: " + kar[(byte)LIGHTING5.packettype].ToString(), false);
                            kar[(byte)LIGHTING5.subtype] = subtype;
                            osae.AddToLog("kar[(byte)LIGHTING5.subtype]: " + subtype.ToString(), false);
                            kar[(byte)LIGHTING5.seqnbr] = bytSeqNbr;
                            osae.AddToLog("kar[(byte)LIGHTING5.seqnbr]: " + bytSeqNbr.ToString(), false);
                            kar[(byte)LIGHTING5.id1] = (byte)Int32.Parse(l5_addr[0]);
                            osae.AddToLog("kar[(byte)LIGHTING5.id1]: " + l5_addr[0], false);
                            kar[(byte)LIGHTING5.id2] = (byte)Int32.Parse(l5_addr[1]);
                            osae.AddToLog("kar[(byte)LIGHTING5.id2]: " + l5_addr[1], false);
                            kar[(byte)LIGHTING5.id3] = (byte)Int32.Parse(l5_addr[2]);
                            osae.AddToLog("kar[(byte)LIGHTING5.id3]: " + l5_addr[2], false);
                            kar[(byte)LIGHTING5.unitcode] = (byte)Int32.Parse(l5_addr[3]);
                            osae.AddToLog("kar[(byte)LIGHTING5.unitcode]: " + l5_addr[3], false);

                            switch (method.MethodName)
                            {
                                case "OFF":
                                    kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sOff;
                                    osae.AddToLog("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString(), false);
                                    osae.ObjectStateSet(obj.Name, "OFF");
                                    break;
                                case "ON":
                                    if (method.Parameter1 == "")
                                    {
                                        kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sOn;
                                        osae.AddToLog("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString(), false);
                                        kar[(byte)LIGHTING5.level] = 0;
                                        osae.AddToLog("kar[(byte)LIGHTING5.level]: " + kar[(byte)LIGHTING5.level].ToString(), false);
                                    }
                                    else
                                    {
                                        kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sSetLevel;
                                        osae.AddToLog("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString(), false);
                                        kar[(byte)LIGHTING5.level] = (byte)Math.Round((double)Int32.Parse(method.Parameter1) / 3, 0);
                                        osae.AddToLog("kar[(byte)LIGHTING5.level]: " + kar[(byte)LIGHTING5.level].ToString(), false);
                                    }
                                    osae.ObjectStateSet(obj.Name, "ON");

                                    break;
                            }

                            kar[(byte)LIGHTING5.filler] = 0;
                            osae.AddToLog("kar[(byte)LIGHTING5.filler]: " + kar[(byte)LIGHTING5.filler].ToString(), false);

                            //not used commands
                            if (kar[(byte)LIGHTING5.cmnd] == 8 | kar[(byte)LIGHTING5.cmnd] == 9)
                            {
                                osae.AddToLog("not used command", true);
                                return;
                            }

                            if (kar[(byte)LIGHTING5.id1] == 0 & kar[(byte)LIGHTING5.id2] == 0 & kar[(byte)LIGHTING5.id3] == 0)
                            {
                                osae.AddToLog("invalid unit address", true);
                                return;
                            }
                            osae.AddToLog("Writing command to port", true);
                            WriteCom(kar);
                            osae.AddToLog("Lighting5 command:", true);
                            string command_l5 = "";
                            foreach (byte bt in kar)
                            {
                                command_l5 += ("0" + bt.ToString()).Substring(("0" + bt.ToString()).Length - 2) + " ";
                            }
                            osae.AddToLog("Lighting5 command:" + command_l5, true);

                        }
                        break;

                    #endregion

                }
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error processing command: " + ex.Message, true);
            }
            osae.AddToLog("-----------------------------------------------", false);
        }

        public void RunInterface(string pluginName)
        {
            osae.AddToLog("Plugin version: 0.2.8", true);
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

        public void Shutdown()
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

            osae.AddToLog("================================================", false);
            osae.AddToLog(message, false);
            foreach (byte bt in kar)
            {
                msgStr += ("0" + bt.ToString()).Substring(("0" + bt.ToString()).Length - 2, 2) + " ";
            }
            osae.AddToLog(msgStr, false);
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
                    osae.AddToLog("Unable to write to COM port", true);
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
                        osae.AddToLog(" Buffer flushed due to timeout", true);
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
                osae.AddToLog("------------------------------------------------", false);
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
                osae.AddToLog(rcvdStr, false);
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
            osae.AddToLog("---------------Received Message----------------", false);
            switch (recbuf[1])
            {
                case (byte)IRESPONSE.pType:
                    osae.AddToLog("Packettype        = Interface Message", false);
                    decode_InterfaceMessage();

                    break;
                case (byte)RXRESPONSE.pType:
                    osae.AddToLog("Packettype        = Receiver/Transmitter Message", false);
                    decode_RecXmitMessage();

                    break;
                case (byte)UNDECODED.pType:
                    osae.AddToLog("Packettype        = UNDECODED RF Message", false);
                    decode_UNDECODED();

                    break;
                //case (byte)LIGHTING1.pType:
                //    osae.AddToLog("Packettype    = Lighting1", false);
                //    decode_Lighting1();

                //    break;
                case (byte)LIGHTING2.pType:
                    osae.AddToLog("Packettype    = Lighting2", false);
                    decode_Lighting2();

                    break;
                //case (byte)LIGHTING3.pType:
                //    osae.AddToLog("Packettype    = Lighting3", false);
                //    decode_Lighting3();

                //    break;
                //case (byte)LIGHTING4.pType:
                //    osae.AddToLog("Packettype    = Lighting4", false);
                //    decode_Lighting4();

                //    break;
                case (byte)LIGHTING5.pType:
                    osae.AddToLog("Packettype    = Lighting5", false);
                    decode_Lighting5();

                    break;
                //case (byte)SECURITY1.pType:
                //    osae.AddToLog("Packettype    = Security1", false);
                //    decode_Security1();

                //    break;
                //case (byte)CAMERA1.pType:
                //    osae.AddToLog("Packettype    = Camera1", false);
                //    decode_Camera1();

                //    break;
                //case (byte)REMOTE.pType:
                //    osae.AddToLog("Packettype    = Remote control & IR", false);
                //    decode_Remote();

                //    break;
                //case (byte)THERMOSTAT1.pType:
                //    osae.AddToLog("Packettype    = Thermostat1", false);
                //    decode_Thermostat1();

                //    break;
                //case (byte)THERMOSTAT2.pType:
                //    osae.AddToLog("Packettype    = Thermostat2", false);
                //    decode_Thermostat2();

                //    break;
                //case (byte)THERMOSTAT3.pType:
                //    osae.AddToLog("Packettype    = Thermostat3", false);
                //    decode_Thermostat3();

                //    break;
                case (byte)TEMP.pType:
                    osae.AddToLog("Packettype    = TEMP", false);
                    decode_Temp();

                    break;
                case (byte)HUM.pType:
                    osae.AddToLog("Packettype    = HUM", false);
                    decode_Hum();

                    break;
                case (byte)TEMP_HUM.pType:
                    osae.AddToLog("Packettype    = TEMP_HUM", false);
                    decode_TempHum();

                    break;
                case (byte)BARO.pType:
                    osae.AddToLog("Packettype    = BARO", false);
                    decode_Baro();

                    break;
                case (byte)TEMP_HUM_BARO.pType:
                    osae.AddToLog("Packettype    = TEMP_HUM_BARO", false);
                    decode_TempHumBaro();

                    break;
                case (byte)RAIN.pType:
                    osae.AddToLog("Packettype    = RAIN", false);
                    decode_Rain();

                    break;
                case (byte)WIND.pType:
                    osae.AddToLog("Packettype    = WIND", false);
                    decode_Wind();

                    break;
                case (byte)UV.pType:
                    osae.AddToLog("Packettype    = UV", false);
                    decode_UV();

                    break;
                case (byte)DT.pType:
                    osae.AddToLog("Packettype    = DT", false);
                    decode_DateTime();

                    break;
                case (byte)CURRENT.pType:
                    osae.AddToLog("Packettype    = CURRENT", false);
                    decode_Current();

                    break;
                case (byte)ENERGY.pType:
                    osae.AddToLog("Packettype    = ENERGY", false);
                    decode_Energy();

                    break;
                case (byte)GAS.pType:
                    osae.AddToLog("Packettype    = GAS", false);
                    decode_Gas();

                    break;
                case (byte)WATER.pType:
                    osae.AddToLog("Packettype    = WATER", false);
                    decode_Water();

                    break;
                case (byte)WEIGHT.pType:
                    osae.AddToLog("Packettype    = WEIGHT", false);
                    decode_Weight();

                    break;
                //case (byte)RFXSENSOR.pType:
                //    osae.AddToLog("Packettype    = RFXSensor", false);
                //    decode_RFXSensor();

                //    break;
                //case (byte)RFXMETER.pType:
                //    osae.AddToLog("Packettype    = RFXMeter", false);
                //    decode_RFXMeter();

                //    break;
                //case (byte)FS20.pType:
                //    osae.AddToLog("Packettype    = FS20", false);
                //    decode_FS20();

                //    break;
                default:
                    osae.AddToLog("ERROR: Unknown Packet type:" + recbuf[1].ToString(), true);
                    break;
            }
            osae.AddToLog("-----------------------------------------------", false);
        }

        public void decode_InterfaceMessage()
        {
            switch (recbuf[(byte)IRESPONSE.subtype])
            {
                case (byte)IRESPONSE.sType:
                    osae.AddToLog("subtype           = Interface Response", false);
                    osae.AddToLog("Sequence nbr      = " + recbuf[(byte)IRESPONSE.seqnbr].ToString(), false);
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
                            osae.AddToLog("response on cmnd  = ", false);
                            switch (recbuf[(byte)IRESPONSE.cmnd])
                            {
                                case (byte)ICMD.STATUS:
                                    osae.AddToLog("Get Status", false);
                                    break;
                                case (byte)ICMD.SETMODE:
                                    osae.AddToLog("Set Mode", false);
                                    break;
                                case (byte)ICMD.sel310:
                                    osae.AddToLog("Select 310MHz", false);
                                    break;
                                case (byte)ICMD.sel315:
                                    osae.AddToLog("Select 315MHz", false);
                                    break;
                                case (byte)ICMD.sel800:
                                    osae.AddToLog("Select 868.00MHz", false);
                                    break;
                                case (byte)ICMD.sel800F:
                                    osae.AddToLog("Select 868.00MHz FSK", false);
                                    break;
                                case (byte)ICMD.sel830:
                                    osae.AddToLog("Select 868.30MHz", false);
                                    break;
                                case (byte)ICMD.sel830F:
                                    osae.AddToLog("Select 868.30MHz FSK", false);
                                    break;
                                case (byte)ICMD.sel835:
                                    osae.AddToLog("Select 868.35MHz", false);
                                    break;
                                case (byte)ICMD.sel835F:
                                    osae.AddToLog("Select 868.35MHz FSK", false);
                                    break;
                                case (byte)ICMD.sel895:
                                    osae.AddToLog("Select 868.95MHz", false);
                                    break;
                                default:
                                    osae.AddToLog("Error: unknown response", false);
                                    break;
                            }
                            switch (recbuf[(byte)IRESPONSE.msg1])
                            {
                                case (byte)IRESPONSE.recType310:
                                    osae.AddToLog("Transceiver type  = 310MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType315:
                                    osae.AddToLog("Receiver type     = 315MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType43392:
                                    osae.AddToLog("Receiver type     = 433.92MHz (receive only)", false);
                                    break;
                                case (byte)IRESPONSE.trxType43392:
                                    osae.AddToLog("Transceiver type  = 433.92MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType86800:
                                    osae.AddToLog("Receiver type     = 868.00MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType86800FSK:
                                    osae.AddToLog("Receiver type     = 868.00MHz FSK", false);
                                    break;
                                case (byte)IRESPONSE.recType86830:
                                    osae.AddToLog("Receiver type     = 868.30MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType86830FSK:
                                    osae.AddToLog("Receiver type     = 868.30MHz FSK", false);
                                    break;
                                case (byte)IRESPONSE.recType86835:
                                    osae.AddToLog("Receiver type     = 868.35MHz", false);
                                    break;
                                case (byte)IRESPONSE.recType86835FSK:
                                    osae.AddToLog("Receiver type     = 868.35MHz FSK", false);
                                    break;
                                case (byte)IRESPONSE.recType86895:
                                    osae.AddToLog("Receiver type     = 868.95MHz", false);
                                    break;
                                default:
                                    osae.AddToLog("Receiver type     = unknown", false);
                                    break;
                            }
                            trxType = recbuf[(byte)IRESPONSE.msg1];
                            osae.AddToLog("Firmware version  = " + recbuf[(byte)IRESPONSE.msg2].ToString(), false);
                            bytFWversion = recbuf[(byte)IRESPONSE.msg2];

                            if ((recbuf[(byte)IRESPONSE.msg3] & (byte)IRESPONSE.msg3_undec) != 0)
                            {
                                osae.AddToLog("Undec             on", false);
                            }
                            else
                            {
                                osae.AddToLog("Undec             off", false);
                            }

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_X10) != 0)
                            {
                                osae.AddToLog("X10               enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("X10               disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_ARC) != 0)
                            {
                                osae.AddToLog("ARC               enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("ARC               disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_AC) != 0)
                            {
                                osae.AddToLog("AC                enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("AC                disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_HEU) != 0)
                            {
                                osae.AddToLog("HomeEasy EU       enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("HomeEasy EU       disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_KOP) != 0)
                            {
                                osae.AddToLog("Ikea Koppla       enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("Ikea Koppla       disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_OREGON) != 0)
                            {
                                osae.AddToLog("Oregon Scientific enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("Oregon Scientific disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_ATI) != 0)
                            {
                                osae.AddToLog("ATI               enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("ATI               disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_VISONIC) != 0)
                            {
                                osae.AddToLog("Visonic           enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("Visonic           disabled", false);
                            }

                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_MERTIK) != 0)
                            {
                                osae.AddToLog("Mertik            enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("Mertik            disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_AD) != 0)
                            {
                                osae.AddToLog("AD                enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("AD                disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_HID) != 0)
                            {
                                osae.AddToLog("Hideki            enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("Hideki            disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_LCROS) != 0)
                            {
                                osae.AddToLog("La Crosse         enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("La Crosse         disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_FS20) != 0)
                            {
                                osae.AddToLog("FS20              enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("FS20              disabled", false);
                            }
                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_PROGUARD) != 0)
                            {
                                osae.AddToLog("ProGuard          enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("ProGuard          disabled", false);
                            }

                            if ((recbuf[(byte)IRESPONSE.msg4] & 0x80) != 0)
                            {
                                osae.AddToLog("RFU protocol 7    enabled", false);
                            }
                            else
                            {
                                osae.AddToLog("RFU protocol 7    disabled", false);
                            }

                            break;
                        case (byte)ICMD.ENABLEALL:
                            osae.AddToLog("response on cmnd  = Enable All RF", false);
                            break;
                        case (byte)ICMD.UNDECODED:
                            osae.AddToLog("response on cmnd  = UNDECODED on", false);
                            break;
                        case (byte)ICMD.SAVE:
                            osae.AddToLog("response on cmnd  = Save", false);
                            break;
                        case (byte)ICMD.DISX10:
                            osae.AddToLog("response on cmnd  = Disable X10 RF", false);
                            break;
                        case (byte)ICMD.DISARC:
                            osae.AddToLog("response on cmnd  = Disable ARC RF", false);
                            break;
                        case (byte)ICMD.DISAC:
                            osae.AddToLog("response on cmnd  = Disable AC RF", false);
                            break;
                        case (byte)ICMD.DISHEU:
                            osae.AddToLog("response on cmnd  = Disable HomeEasy EU RF", false);
                            break;
                        case (byte)ICMD.DISKOP:
                            osae.AddToLog("response on cmnd  = Disable Ikea Koppla RF", false);
                            break;
                        case (byte)ICMD.DISOREGON:
                            osae.AddToLog("response on cmnd  = Disable Oregon Scientific RF", false);
                            break;
                        case (byte)ICMD.DISATI:
                            osae.AddToLog("response on cmnd  = Disable ATI remote RF", false);
                            break;
                        case (byte)ICMD.DISVISONIC:
                            osae.AddToLog("response on cmnd  = Disable Visonic RF", false);
                            break;
                        case (byte)ICMD.DISMERTIK:
                            osae.AddToLog("response on cmnd  = Disable Mertik RF", false);
                            break;
                        case (byte)ICMD.DISAD:
                            osae.AddToLog("response on cmnd  = Disable AD RF", false);
                            break;
                        case (byte)ICMD.DISHID:
                            osae.AddToLog("response on cmnd  = Disable Hideki RF", false);
                            break;
                        case (byte)ICMD.DISLCROS:
                            osae.AddToLog("response on cmnd  = Disable La Crosse RF", false);

                            break;
                        //For internal use by RFXCOM only, do not use this coding.
                        //=========================================================
                        case 0x8:
                            osae.AddToLog("response on cmnd  = T1", false);
                            if (recbuf[(byte)IRESPONSE.msg9] == 0)
                            {
                                osae.AddToLog("Not OK!", false);
                            }
                            else
                            {
                                osae.AddToLog("On", false);
                            }

                            break;
                        case 0x9:
                            osae.AddToLog("response on cmnd  = T2", false);
                            if (recbuf[(byte)IRESPONSE.msg9] == 0)
                            {
                                osae.AddToLog("Not OK!", false);
                            }
                            else
                            {
                                osae.AddToLog("Blk On", false);
                            }
                            break;
                        //=========================================================

                        default:
                            osae.AddToLog("ERROR: Unexpected response for Packet type=" + recbuf[(byte)IRESPONSE.packettype].ToString() + ", Sub type=" + recbuf[(byte)IRESPONSE.subtype].ToString() + " cmnd=" + recbuf[(byte)IRESPONSE.cmnd].ToString(), false);
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
                    osae.AddToLog("subtype           = Receiver lock error", false);
                    //SystemSounds.Asterisk.Play();
                    osae.AddToLog("Sequence nbr      = " + recbuf[(byte)RXRESPONSE.seqnbr].ToString(), false);

                    break;
                case (byte)RXRESPONSE.sTypeTransmitterResponse:
                    osae.AddToLog("subtype           = Transmitter Response", false);
                    osae.AddToLog("Sequence nbr      = " + recbuf[(byte)RXRESPONSE.seqnbr].ToString(), false);
                    switch (recbuf[(byte)RXRESPONSE.msg])
                    {
                        case 0x0:
                            osae.AddToLog("response          = ACK, data correct transmitted", false);
                            break;
                        case 0x1:
                            osae.AddToLog("response          = ACK, but transmit started after 6 seconds delay anyway with RF receive data detected", false);
                            break;
                        case 0x2:
                            osae.AddToLog("response          = NAK, transmitter did not lock on the requested transmit frequency", false);
                            //SystemSounds.Asterisk.Play();
                            break;
                        case 0x3:
                            osae.AddToLog("response          = NAK, AC address zero in id1-id4 not allowed", false);
                            //SystemSounds.Asterisk.Play();
                            break;
                        default:
                            osae.AddToLog("ERROR: Unexpected message type=" + recbuf[(byte)RXRESPONSE.msg].ToString(), false);
                            break;
                    }
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)RXRESPONSE.packettype].ToString() + ": " + recbuf[(byte)RXRESPONSE.subtype].ToString(), false);
                    break;
            }
        }

        public void decode_UNDECODED()
        {
            osae.AddToLog("UNDECODED ", false);
            switch (recbuf[(byte)UNDECODED.subtype])
            {
                case (byte)UNDECODED.sTypeUac:
                    osae.AddToLog("AC:", false);
                    break;
                case (byte)UNDECODED.sTypeUarc:
                    osae.AddToLog("ARC:", false);
                    break;
                case (byte)UNDECODED.sTypeUati:
                    osae.AddToLog("ATI:", false);
                    break;
                case (byte)UNDECODED.sTypeUhideki:
                    osae.AddToLog("HIDEKI:", false);
                    break;
                case (byte)UNDECODED.sTypeUlacrosse:
                    osae.AddToLog("LACROSSE:", false);
                    break;
                case (byte)UNDECODED.sTypeUlwrf:
                    osae.AddToLog("LWRF:", false);
                    break;
                case (byte)UNDECODED.sTypeUmertik:
                    osae.AddToLog("MERTIK:", false);
                    break;
                case (byte)UNDECODED.sTypeUoregon1:
                    osae.AddToLog("OREGON1:", false);
                    break;
                case (byte)UNDECODED.sTypeUoregon2:
                    osae.AddToLog("OREGON2:", false);
                    break;
                case (byte)UNDECODED.sTypeUoregon3:
                    osae.AddToLog("OREGON3:", false);
                    break;
                case (byte)UNDECODED.sTypeUproguard:
                    osae.AddToLog("PROGUARD:", false);
                    break;
                case (byte)UNDECODED.sTypeUvisonic:
                    osae.AddToLog("VISONIC:", false);
                    break;
                case (byte)UNDECODED.sTypeUnec:
                    osae.AddToLog("NEC:", false);
                    break;
                case (byte)UNDECODED.sTypeUfs20:
                    osae.AddToLog("FS20:", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)UNDECODED.packettype] + ": " + recbuf[(byte)UNDECODED.subtype], false);
                    break;
            }
            for (int i = 0; i <= recbuf[(byte)UNDECODED.packetlength] - (byte)UNDECODED.msg1; i++)
            {
                osae.AddToLog("0" + recbuf[(byte)UNDECODED.msg1 + i], false);
            }

        }

        //public void decode_Lighting1()
        //{
        //    switch (recbuf(LIGHTING1.subtype))
        //    {
        //        case LIGHTING1.sTypeX10:
        //            osae.AddToLog("subtype       = X10");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            osae.AddToLog("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            osae.AddToLog("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            osae.AddToLog("Command       = ", false);
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    osae.AddToLog("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    osae.AddToLog("On");
        //                    break;
        //                case LIGHTING1.sDim:
        //                    osae.AddToLog("Dim");
        //                    break;
        //                case LIGHTING1.sBright:
        //                    osae.AddToLog("Bright");
        //                    break;
        //                case LIGHTING1.sAllOn:
        //                    osae.AddToLog("All On");
        //                    break;
        //                case LIGHTING1.sAllOff:
        //                    osae.AddToLog("All Off");
        //                    break;
        //                case LIGHTING1.sChime:
        //                    osae.AddToLog("Chime");
        //                    break;
        //                default:
        //                    osae.AddToLog("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        case LIGHTING1.sTypeARC:
        //            osae.AddToLog("subtype       = ARC");
        //            osae.AddToLog("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            osae.AddToLog("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            osae.AddToLog("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            osae.AddToLog("Command       = ", false);
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    osae.AddToLog("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    osae.AddToLog("On");
        //                    break;
        //                case LIGHTING1.sAllOn:
        //                    osae.AddToLog("All On");
        //                    break;
        //                case LIGHTING1.sAllOff:
        //                    osae.AddToLog("All Off");
        //                    break;
        //                default:
        //                    osae.AddToLog("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        case LIGHTING1.sTypeAB400D:
        //            osae.AddToLog("subtype       = ELRO AB400");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            osae.AddToLog("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            osae.AddToLog("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            osae.AddToLog("Command       = ", false);
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    osae.AddToLog("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    osae.AddToLog("On");
        //                    break;
        //                default:
        //                    osae.AddToLog("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(LIGHTING1.packettype)) + ": " + Conversion.Hex(recbuf(LIGHTING1.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Signal level  = " + (recbuf(LIGHTING1.rssi) >> 4).ToString());
        //}

        public void decode_Lighting2()
        {
            osae.AddToLog("Recieved Lighting2 Message.  Type: " + recbuf[(byte)LIGHTING2.subtype].ToString(), false);
            OSAEObject obj = new OSAEObject(); 
            
            switch (recbuf[(byte)LIGHTING2.subtype])
            {
                case (byte)LIGHTING2.sTypeAC:
                case (byte)LIGHTING2.sTypeHEU:
                case (byte)LIGHTING2.sTypeANSLUT:
                    osae.AddToLog("id1: " + recbuf[(byte)LIGHTING2.id1].ToString(), true);
                    osae.AddToLog("id2: " + recbuf[(byte)LIGHTING2.id2].ToString(), true);
                    osae.AddToLog("id3: " + recbuf[(byte)LIGHTING2.id3].ToString(), true);
                    osae.AddToLog("id4: " + recbuf[(byte)LIGHTING2.id4].ToString(), true);
                    osae.AddToLog("uc: " + recbuf[(byte)LIGHTING2.unitcode].ToString(), true);

                   

                    string address = (recbuf[(byte)LIGHTING2.id1].ToString("X") +
                                "-" + recbuf[(byte)LIGHTING2.id2].ToString("X") +
                                "-" + recbuf[(byte)LIGHTING2.id3].ToString("X") +
                                "-" + recbuf[(byte)LIGHTING2.id4].ToString("X") +
                                "-" + recbuf[(byte)LIGHTING2.unitcode].ToString("X"));

                    osae.AddToLog("Address: " + address, true);

                    obj = osae.GetObjectByAddress(address);
                        
                    switch (recbuf[(byte)LIGHTING2.subtype])
                    {
                        case (byte)LIGHTING2.sTypeAC:
                            osae.AddToLog("subtype       = AC", false);
                            break;
                        case (byte)LIGHTING2.sTypeHEU:
                            osae.AddToLog("subtype       = HomeEasy EU", false);
                            break;
                        case (byte)LIGHTING2.sTypeANSLUT:
                            osae.AddToLog("subtype       = ANSLUT", false);
                            break;
                    }
                    osae.AddToLog("Sequence nbr  = " + recbuf[(byte)LIGHTING2.seqnbr].ToString(), false);
                    osae.AddToLog("ID - Unit            = " + address, false);
                    switch (recbuf[(byte)LIGHTING2.cmnd])
                    {
                        case (byte)LIGHTING2.sOff:
                            osae.AddToLog("Command       = Off", false);
                            osae.ObjectStateSet(obj.Name, "OFF");
                            break;
                        case (byte)LIGHTING2.sOn:
                            osae.AddToLog("Command       = On", false);
                            osae.ObjectStateSet(obj.Name, "ON");
                            break;
                        case (byte)LIGHTING2.sSetLevel:
                            osae.AddToLog("Set Level:" + recbuf[(byte)LIGHTING2.level].ToString(), false);
                            break;
                        case (byte)LIGHTING2.sGroupOff:
                            osae.AddToLog("Group Off", false);
                            break;
                        case (byte)LIGHTING2.sGroupOn:
                            osae.AddToLog("Group On", false);
                            break;
                        case (byte)LIGHTING2.sSetGroupLevel:
                            osae.AddToLog("Set Group Level:" + recbuf[(byte)LIGHTING2.level].ToString(), false);
                            break;
                        default:
                            osae.AddToLog("UNKNOWN", false);
                            break;
                    }

                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Convert.ToInt32(recbuf[(byte)LIGHTING2.packettype]) + ": " + Convert.ToInt32(recbuf[(byte)LIGHTING2.subtype]), false);
                    break;
            }
            osae.AddToLog("Signal level  = " + (recbuf[(byte)LIGHTING2.rssi] >> 4).ToString(), false);
        }

        //public void decode_Lighting3()
        //{
        //    switch (recbuf(LIGHTING3.subtype))
        //    {
        //        case LIGHTING3.sTypeKoppla:
        //            osae.AddToLog("subtype       = Ikea Koppla");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(LIGHTING3.seqnbr).ToString);
        //            osae.AddToLog("Command       = ", false);
        //            switch (recbuf(LIGHTING3.cmnd))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("Off");
        //                    break;
        //                case 0x1:
        //                    osae.AddToLog("On");
        //                    break;
        //                case 0x20:
        //                    osae.AddToLog("Set Level:" + recbuf(6).ToString);
        //                    break;
        //                case 0x21:
        //                    osae.AddToLog("Program");
        //                    break;
        //                default:
        //                    if (recbuf(LIGHTING3.cmnd) >= 0x10 & recbuf(LIGHTING3.cmnd) < 0x18)
        //                    {
        //                        osae.AddToLog("Dim");
        //                    }
        //                    else if (recbuf(LIGHTING3.cmnd) >= 0x18 & recbuf(LIGHTING3.cmnd) < 0x20)
        //                    {
        //                        osae.AddToLog("Bright");
        //                    }
        //                    else
        //                    {
        //                        osae.AddToLog("UNKNOWN");
        //                    }
        //                    break;
        //            }
        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(LIGHTING3.packettype)) + ": " + Conversion.Hex(recbuf(LIGHTING3.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Signal level  = " + (recbuf(LIGHTING3.rssi) >> 4).ToString());

        //}

        //public void decode_Lighting4()
        //{
        //    osae.AddToLog("Not implemented");
        //}

        public void decode_Lighting5()
        {
            osae.AddToLog("Recieved Lighting5 Message", false);
            OSAEObject obj = new OSAEObject();
            switch (recbuf[(byte)LIGHTING5.subtype])
            {
                case (byte)LIGHTING5.sTypeLightwaveRF:
                    obj = osae.GetObjectByAddress("0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString() + "-0" + recbuf[(byte)LIGHTING5.id3].ToString() + "-" + recbuf[(byte)LIGHTING5.unitcode].ToString()); 
                    osae.AddToLog("subtype       = LightwaveRF", false);
                    osae.AddToLog("Sequence nbr  = " + recbuf[(byte)LIGHTING5.seqnbr].ToString(), false);
                    osae.AddToLog("ID            = " + "0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2] + "-0" + recbuf[(byte)LIGHTING5.id3].ToString(), false);
                    osae.AddToLog("Unit          = " + recbuf[(byte)LIGHTING5.unitcode].ToString(), false);
                    switch (recbuf[(byte)LIGHTING5.cmnd])
                    {
                        case (byte)LIGHTING5.sOff:
                            osae.ObjectStateSet(obj.Name, "OFF");
                            osae.AddToLog("Command       = Off", false);
                            break;
                        case (byte)LIGHTING5.sOn:
                            osae.ObjectStateSet(obj.Name, "ON");
                            osae.AddToLog("Command       = On", false);
                            break;
                        case (byte)LIGHTING5.sGroupOff:
                            osae.AddToLog("Command       = Group Off", false);
                            break;
                        case (byte)LIGHTING5.sMood1:
                            osae.AddToLog("Command       = Group Mood 1", false);
                            break;
                        case (byte)LIGHTING5.sMood2:
                            osae.AddToLog("Command       = Group Mood 2", false);
                            break;
                        case (byte)LIGHTING5.sMood3:
                            osae.AddToLog("Command       = Group Mood 3", false);
                            break;
                        case (byte)LIGHTING5.sMood4:
                            osae.AddToLog("Command       = Group Mood 4", false);
                            break;
                        case (byte)LIGHTING5.sMood5:
                            osae.AddToLog("Command       = Group Mood 5", false);
                            break;
                        case (byte)LIGHTING5.sUnlock:
                            osae.AddToLog("Command       = Unlock", false);
                            break;
                        case (byte)LIGHTING5.sLock:
                            osae.AddToLog("Command       = Lock", false);
                            break;
                        case (byte)LIGHTING5.sAllLock:
                            osae.AddToLog("Command       = All lock", false);
                            break;
                        case (byte)LIGHTING5.sClose:
                            osae.AddToLog("Command       = Close inline relay", false);
                            break;
                        case (byte)LIGHTING5.sStop:
                            osae.AddToLog("Command       = Stop inline relay", false);
                            break;
                        case (byte)LIGHTING5.sOpen:
                            osae.AddToLog("Command       = Open inline relay", false);
                            break;
                        case (byte)LIGHTING5.sSetLevel:
                            osae.AddToLog("Command       = Set dim level to: " + Convert.ToInt32((recbuf[(byte)LIGHTING5.level] * 3.2)).ToString() + "%", false);
                            break;
                        default:
                            osae.AddToLog("UNKNOWN", false);
                            break;
                    }

                    break;
                case (byte)LIGHTING5.sTypeEMW100:
                    obj = osae.GetObjectByAddress("0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString() + "-" + recbuf[(byte)LIGHTING5.unitcode].ToString()); 
                    osae.AddToLog("subtype       = EMW100", false);
                    osae.AddToLog("Sequence nbr  = " + recbuf[(byte)LIGHTING5.seqnbr].ToString(), false);
                    osae.AddToLog("ID            = " + "0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString(), false);
                    osae.AddToLog("Unit          = " + recbuf[(byte)LIGHTING5.unitcode].ToString(),false);
                    switch (recbuf[(byte)LIGHTING5.cmnd])
                    {
                        case (byte)LIGHTING5.sOff:
                            osae.ObjectStateSet(obj.Name, "OFF");
                            osae.AddToLog("Command       = Off", false);
                            break;
                        case (byte)LIGHTING5.sOn:
                            osae.ObjectStateSet(obj.Name, "ON");
                            osae.AddToLog("Command       = On", false);
                            break;
                        case (byte)LIGHTING5.sLearn:
                            osae.AddToLog("Command       = Learn", false);
                            break;
                        default:
                            osae.AddToLog("Command       = UNKNOWN", false);
                            break;
                    }

                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)LIGHTING5.packettype].ToString() + ": " + recbuf[(byte)LIGHTING5.subtype].ToString(), false);
                    break;
            }
            osae.AddToLog("Signal level  = " + (recbuf[(byte)LIGHTING5.rssi] >> 4).ToString(), false);

        }

        //public void decode_Security1()
        //{
        //    switch (recbuf(SECURITY1.subtype))
        //    {
        //        case SECURITY1.SecX10:
        //            osae.AddToLog("subtype       = X10 security");
        //            break;
        //        case SECURITY1.SecX10M:
        //            osae.AddToLog("subtype       = X10 security motion");
        //            break;
        //        case SECURITY1.SecX10R:
        //            osae.AddToLog("subtype       = X10 security remote");
        //            break;
        //        case SECURITY1.KD101:
        //            osae.AddToLog("subtype       = KD101 smoke detector");
        //            break;
        //        case SECURITY1.PowercodeSensor:
        //            osae.AddToLog("subtype       = Visonic PowerCode sensor - primary contact");
        //            break;
        //        case SECURITY1.PowercodeMotion:
        //            osae.AddToLog("subtype       = Visonic PowerCode motion");
        //            break;
        //        case SECURITY1.Codesecure:
        //            osae.AddToLog("subtype       = Visonic CodeSecure");
        //            break;
        //        case SECURITY1.PowercodeAux:
        //            osae.AddToLog("subtype       = Visonic PowerCode sensor - auxiliary contact");
        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(SECURITY1.packettype)) + ": " + Conversion.Hex(recbuf(SECURITY1.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Sequence nbr  = " + recbuf(SECURITY1.seqnbr).ToString);
        //    osae.AddToLog("id1-3         = " + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id2)), 2) + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id3)), 2));
        //    osae.AddToLog("status        = ", false);
        //    switch (recbuf(SECURITY1.status))
        //    {
        //        case SECURITY1.sStatusNormal:
        //            osae.AddToLog("Normal");
        //            break;
        //        case SECURITY1.sStatusNormalDelayed:
        //            osae.AddToLog("Normal Delayed");
        //            break;
        //        case SECURITY1.sStatusAlarm:
        //            osae.AddToLog("Alarm");
        //            break;
        //        case SECURITY1.sStatusAlarmDelayed:
        //            osae.AddToLog("Alarm Delayed");
        //            break;
        //        case SECURITY1.sStatusMotion:
        //            osae.AddToLog("Motion");
        //            break;
        //        case SECURITY1.sStatusNoMotion:
        //            osae.AddToLog("No Motion");
        //            break;
        //        case SECURITY1.sStatusPanic:
        //            osae.AddToLog("Panic");
        //            break;
        //        case SECURITY1.sStatusPanicOff:
        //            osae.AddToLog("Panic End");
        //            break;
        //        case SECURITY1.sStatusTamper:
        //            osae.AddToLog("Tamper");
        //            break;
        //        case SECURITY1.sStatusArmAway:
        //            osae.AddToLog("Arm Away");
        //            break;
        //        case SECURITY1.sStatusArmAwayDelayed:
        //            osae.AddToLog("Arm Away Delayed");
        //            break;
        //        case SECURITY1.sStatusArmHome:
        //            osae.AddToLog("Arm Home");
        //            break;
        //        case SECURITY1.sStatusArmHomeDelayed:
        //            osae.AddToLog("Arm Home Delayed");
        //            break;
        //        case SECURITY1.sStatusDisarm:
        //            osae.AddToLog("Disarm");
        //            break;
        //        case SECURITY1.sStatusLightOff:
        //            osae.AddToLog("Light Off");
        //            break;
        //        case SECURITY1.sStatusLightOn:
        //            osae.AddToLog("Light On");
        //            break;
        //        case SECURITY1.sStatusLIGHTING2Off:
        //            osae.AddToLog("Light 2 Off");
        //            break;
        //        case SECURITY1.sStatusLIGHTING2On:
        //            osae.AddToLog("Light 2 On");
        //            break;
        //        case SECURITY1.sStatusDark:
        //            osae.AddToLog("Dark detected");
        //            break;
        //        case SECURITY1.sStatusLight:
        //            osae.AddToLog("Light Detected");
        //            break;
        //        case SECURITY1.sStatusBatLow:
        //            osae.AddToLog("Battery low MS10 or XX18 sensor");
        //            break;
        //        case SECURITY1.sStatusPairKD101:
        //            osae.AddToLog("Pair KD101");
        //            break;
        //    }
        //    if ((recbuf(SECURITY1.battery_level) & 0xf) == 0)
        //    {
        //        osae.AddToLog("battery level = Low");
        //    }
        //    else
        //    {
        //        osae.AddToLog("battery level = OK");
        //    }
        //    osae.AddToLog("Signal level  = " + (recbuf(SECURITY1.rssi) >> 4).ToString());
        //}

        //public void decode_Camera1()
        //{
        //    switch (recbuf(CAMERA1.subtype))
        //    {
        //        case CAMERA1.Ninja:
        //            osae.AddToLog("subtype       = X10 Ninja/Robocam");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(CAMERA1.seqnbr).ToString);
        //            osae.AddToLog("Command       = ", false);
        //            switch (recbuf(CAMERA1.cmnd))
        //            {
        //                case CAMERA1.sLeft:
        //                    osae.AddToLog("Left");
        //                    break;
        //                case CAMERA1.sRight:
        //                    osae.AddToLog("Right");
        //                    break;
        //                case CAMERA1.sUp:
        //                    osae.AddToLog("Up");
        //                    break;
        //                case CAMERA1.sDown:
        //                    osae.AddToLog("Down");
        //                    break;
        //                case CAMERA1.sPosition1:
        //                    osae.AddToLog("Position 1");
        //                    break;
        //                case CAMERA1.sProgramPosition1:
        //                    osae.AddToLog("Position 1 program");
        //                    break;
        //                case CAMERA1.sPosition2:
        //                    osae.AddToLog("Position 2");
        //                    break;
        //                case CAMERA1.sProgramPosition2:
        //                    osae.AddToLog("Position 2 program");
        //                    break;
        //                case CAMERA1.sPosition3:
        //                    osae.AddToLog("Position 3");
        //                    break;
        //                case CAMERA1.sProgramPosition3:
        //                    osae.AddToLog("Position 3 program");
        //                    break;
        //                case CAMERA1.sPosition4:
        //                    osae.AddToLog("Position 4");
        //                    break;
        //                case CAMERA1.sProgramPosition4:
        //                    osae.AddToLog("Position 4 program");
        //                    break;
        //                case CAMERA1.sCenter:
        //                    osae.AddToLog("Center");
        //                    break;
        //                case CAMERA1.sProgramCenterPosition:
        //                    osae.AddToLog("Center program");
        //                    break;
        //                case CAMERA1.sSweep:
        //                    osae.AddToLog("Sweep");
        //                    break;
        //                case CAMERA1.sProgramSweep:
        //                    osae.AddToLog("Sweep program");
        //                    break;
        //                default:
        //                    osae.AddToLog("UNKNOWN");
        //                    break;
        //            }
        //            osae.AddToLog("Housecode     = " + Strings.Chr(recbuf(CAMERA1.housecode)));
        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(CAMERA1.packettype)) + ": " + Conversion.Hex(recbuf(CAMERA1.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Signal level  = " + (recbuf(CAMERA1.rssi) >> 4).ToString());
        //}

        //public void decode_Remote()
        //{
        //    switch (recbuf(REMOTE.subtype))
        //    {
        //        case REMOTE.ATI:
        //            osae.AddToLog("subtype       = ATI Remote Wonder");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + recbuf(REMOTE.id).ToString);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("Command       = A", false);
        //                    break;
        //                case 0x1:
        //                    osae.AddToLog("Command       = B", false);
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("Command       = power", false);
        //                    break;
        //                case 0x3:
        //                    osae.AddToLog("Command       = TV", false);
        //                    break;
        //                case 0x4:
        //                    osae.AddToLog("Command       = DVD", false);
        //                    break;
        //                case 0x5:
        //                    osae.AddToLog("Command       = ?", false);
        //                    break;
        //                case 0x6:
        //                    osae.AddToLog("Command       = Guide", false);
        //                    break;
        //                case 0x7:
        //                    osae.AddToLog("Command       = Drag", false);
        //                    break;
        //                case 0x8:
        //                    osae.AddToLog("Command       = VOL+", false);
        //                    break;
        //                case 0x9:
        //                    osae.AddToLog("Command       = VOL-", false);
        //                    break;
        //                case 0xa:
        //                    osae.AddToLog("Command       = MUTE", false);
        //                    break;
        //                case 0xb:
        //                    osae.AddToLog("Command       = CHAN+", false);
        //                    break;
        //                case 0xc:
        //                    osae.AddToLog("Command       = CHAN-", false);
        //                    break;
        //                case 0xd:
        //                    osae.AddToLog("Command       = 1", false);
        //                    break;
        //                case 0xe:
        //                    osae.AddToLog("Command       = 2", false);
        //                    break;
        //                case 0xf:
        //                    osae.AddToLog("Command       = 3", false);
        //                    break;
        //                case 0x10:
        //                    osae.AddToLog("Command       = 4", false);
        //                    break;
        //                case 0x11:
        //                    osae.AddToLog("Command       = 5", false);
        //                    break;
        //                case 0x12:
        //                    osae.AddToLog("Command       = 6", false);
        //                    break;
        //                case 0x13:
        //                    osae.AddToLog("Command       = 7", false);
        //                    break;
        //                case 0x14:
        //                    osae.AddToLog("Command       = 8", false);
        //                    break;
        //                case 0x15:
        //                    osae.AddToLog("Command       = 9", false);
        //                    break;
        //                case 0x16:
        //                    osae.AddToLog("Command       = txt", false);
        //                    break;
        //                case 0x17:
        //                    osae.AddToLog("Command       = 0", false);
        //                    break;
        //                case 0x18:
        //                    osae.AddToLog("Command       = snapshot ESC", false);
        //                    break;
        //                case 0x19:
        //                    osae.AddToLog("Command       = C", false);
        //                    break;
        //                case 0x1a:
        //                    osae.AddToLog("Command       = ^", false);
        //                    break;
        //                case 0x1b:
        //                    osae.AddToLog("Command       = D", false);
        //                    break;
        //                case 0x1c:
        //                    osae.AddToLog("Command       = TV/RADIO", false);
        //                    break;
        //                case 0x1d:
        //                    osae.AddToLog("Command       = <", false);
        //                    break;
        //                case 0x1e:
        //                    osae.AddToLog("Command       = OK", false);
        //                    break;
        //                case 0x1f:
        //                    osae.AddToLog("Command       = >", false);
        //                    break;
        //                case 0x20:
        //                    osae.AddToLog("Command       = <-", false);
        //                    break;
        //                case 0x21:
        //                    osae.AddToLog("Command       = E", false);
        //                    break;
        //                case 0x22:
        //                    osae.AddToLog("Command       = v", false);
        //                    break;
        //                case 0x23:
        //                    osae.AddToLog("Command       = F", false);
        //                    break;
        //                case 0x24:
        //                    osae.AddToLog("Command       = Rewind", false);
        //                    break;
        //                case 0x25:
        //                    osae.AddToLog("Command       = Play", false);
        //                    break;
        //                case 0x26:
        //                    osae.AddToLog("Command       = Fast forward", false);
        //                    break;
        //                case 0x27:
        //                    osae.AddToLog("Command       = Record", false);
        //                    break;
        //                case 0x28:
        //                    osae.AddToLog("Command       = Stop", false);
        //                    break;
        //                case 0x29:
        //                    osae.AddToLog("Command       = Pause", false);

        //                    break;
        //                case 0x2c:
        //                    osae.AddToLog("Command       = TV", false);
        //                    break;
        //                case 0x2d:
        //                    osae.AddToLog("Command       = VCR", false);
        //                    break;
        //                case 0x2e:
        //                    osae.AddToLog("Command       = RADIO", false);
        //                    break;
        //                case 0x2f:
        //                    osae.AddToLog("Command       = TV Preview", false);
        //                    break;
        //                case 0x30:
        //                    osae.AddToLog("Command       = Channel list", false);
        //                    break;
        //                case 0x31:
        //                    osae.AddToLog("Command       = Video Desktop", false);
        //                    break;
        //                case 0x32:
        //                    osae.AddToLog("Command       = red", false);
        //                    break;
        //                case 0x33:
        //                    osae.AddToLog("Command       = green", false);
        //                    break;
        //                case 0x34:
        //                    osae.AddToLog("Command       = yellow", false);
        //                    break;
        //                case 0x35:
        //                    osae.AddToLog("Command       = blue", false);
        //                    break;
        //                case 0x36:
        //                    osae.AddToLog("Command       = rename TAB", false);
        //                    break;
        //                case 0x37:
        //                    osae.AddToLog("Command       = Acquire image", false);
        //                    break;
        //                case 0x38:
        //                    osae.AddToLog("Command       = edit image", false);
        //                    break;
        //                case 0x39:
        //                    osae.AddToLog("Command       = Full screen", false);
        //                    break;
        //                case 0x3a:
        //                    osae.AddToLog("Command       = DVD Audio", false);
        //                    break;
        //                case 0x70:
        //                    osae.AddToLog("Command       = Cursor-left", false);
        //                    break;
        //                case 0x71:
        //                    osae.AddToLog("Command       = Cursor-right", false);
        //                    break;
        //                case 0x72:
        //                    osae.AddToLog("Command       = Cursor-up", false);
        //                    break;
        //                case 0x73:
        //                    osae.AddToLog("Command       = Cursor-down", false);
        //                    break;
        //                case 0x74:
        //                    osae.AddToLog("Command       = Cursor-up-left", false);
        //                    break;
        //                case 0x75:
        //                    osae.AddToLog("Command       = Cursor-up-right", false);
        //                    break;
        //                case 0x76:
        //                    osae.AddToLog("Command       = Cursor-down-right", false);
        //                    break;
        //                case 0x77:
        //                    osae.AddToLog("Command       = Cursor-down-left", false);
        //                    break;
        //                case 0x78:
        //                    osae.AddToLog("Command       = V", false);
        //                    break;
        //                case 0x79:
        //                    osae.AddToLog("Command       = V-End", false);
        //                    break;
        //                case 0x7c:
        //                    osae.AddToLog("Command       = X", false);
        //                    break;
        //                case 0x7d:
        //                    osae.AddToLog("Command       = X-End", false);
        //                    break;
        //                default:
        //                    osae.AddToLog("Command       = unknown", false);
        //                    break;
        //            }

        //            break;
        //        case REMOTE.ATI2:
        //            osae.AddToLog("subtype       = ATI Remote Wonder II");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + recbuf(REMOTE.id).ToString);
        //            osae.AddToLog("Command       = ", false);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("A", false);
        //                    break;
        //                case 0x1:
        //                    osae.AddToLog("B", false);
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("power", false);
        //                    break;
        //                case 0x3:
        //                    osae.AddToLog("TV", false);
        //                    break;
        //                case 0x4:
        //                    osae.AddToLog("DVD", false);
        //                    break;
        //                case 0x5:
        //                    osae.AddToLog("?", false);
        //                    break;
        //                case 0x6:
        //                    osae.AddToLog("Guide", false);
        //                    break;
        //                case 0x7:
        //                    osae.AddToLog("Drag", false);
        //                    break;
        //                case 0x8:
        //                    osae.AddToLog("VOL+", false);
        //                    break;
        //                case 0x9:
        //                    osae.AddToLog("VOL-", false);
        //                    break;
        //                case 0xa:
        //                    osae.AddToLog("MUTE", false);
        //                    break;
        //                case 0xb:
        //                    osae.AddToLog("CHAN+", false);
        //                    break;
        //                case 0xc:
        //                    osae.AddToLog("CHAN-", false);
        //                    break;
        //                case 0xd:
        //                    osae.AddToLog("1", false);
        //                    break;
        //                case 0xe:
        //                    osae.AddToLog("2", false);
        //                    break;
        //                case 0xf:
        //                    osae.AddToLog("3", false);
        //                    break;
        //                case 0x10:
        //                    osae.AddToLog("4", false);
        //                    break;
        //                case 0x11:
        //                    osae.AddToLog("5", false);
        //                    break;
        //                case 0x12:
        //                    osae.AddToLog("6", false);
        //                    break;
        //                case 0x13:
        //                    osae.AddToLog("7", false);
        //                    break;
        //                case 0x14:
        //                    osae.AddToLog("8", false);
        //                    break;
        //                case 0x15:
        //                    osae.AddToLog("9", false);
        //                    break;
        //                case 0x16:
        //                    osae.AddToLog("txt", false);
        //                    break;
        //                case 0x17:
        //                    osae.AddToLog("0", false);
        //                    break;
        //                case 0x18:
        //                    osae.AddToLog("Open Setup Menu", false);
        //                    break;
        //                case 0x19:
        //                    osae.AddToLog("C", false);
        //                    break;
        //                case 0x1a:
        //                    osae.AddToLog("^", false);
        //                    break;
        //                case 0x1b:
        //                    osae.AddToLog("D", false);
        //                    break;
        //                case 0x1c:
        //                    osae.AddToLog("FM", false);
        //                    break;
        //                case 0x1d:
        //                    osae.AddToLog("<", false);
        //                    break;
        //                case 0x1e:
        //                    osae.AddToLog("OK", false);
        //                    break;
        //                case 0x1f:
        //                    osae.AddToLog(">", false);
        //                    break;
        //                case 0x20:
        //                    osae.AddToLog("Max/Restore window", false);
        //                    break;
        //                case 0x21:
        //                    osae.AddToLog("E", false);
        //                    break;
        //                case 0x22:
        //                    osae.AddToLog("v", false);
        //                    break;
        //                case 0x23:
        //                    osae.AddToLog("F", false);
        //                    break;
        //                case 0x24:
        //                    osae.AddToLog("Rewind", false);
        //                    break;
        //                case 0x25:
        //                    osae.AddToLog("Play", false);
        //                    break;
        //                case 0x26:
        //                    osae.AddToLog("Fast forward", false);
        //                    break;
        //                case 0x27:
        //                    osae.AddToLog("Record", false);
        //                    break;
        //                case 0x28:
        //                    osae.AddToLog("Stop", false);
        //                    break;
        //                case 0x29:
        //                    osae.AddToLog("Pause", false);
        //                    break;
        //                case 0x2a:
        //                    osae.AddToLog("TV2", false);
        //                    break;
        //                case 0x2b:
        //                    osae.AddToLog("Clock", false);
        //                    break;
        //                case 0x2c:
        //                    osae.AddToLog("i", false);
        //                    break;
        //                case 0x2d:
        //                    osae.AddToLog("ATI", false);
        //                    break;
        //                case 0x2e:
        //                    osae.AddToLog("RADIO", false);
        //                    break;
        //                case 0x2f:
        //                    osae.AddToLog("TV Preview", false);
        //                    break;
        //                case 0x30:
        //                    osae.AddToLog("Channel list", false);
        //                    break;
        //                case 0x31:
        //                    osae.AddToLog("Video Desktop", false);
        //                    break;
        //                case 0x32:
        //                    osae.AddToLog("red", false);
        //                    break;
        //                case 0x33:
        //                    osae.AddToLog("green", false);
        //                    break;
        //                case 0x34:
        //                    osae.AddToLog("yellow", false);
        //                    break;
        //                case 0x35:
        //                    osae.AddToLog("blue", false);
        //                    break;
        //                case 0x36:
        //                    osae.AddToLog("rename TAB", false);
        //                    break;
        //                case 0x37:
        //                    osae.AddToLog("Acquire image", false);
        //                    break;
        //                case 0x38:
        //                    osae.AddToLog("edit image", false);
        //                    break;
        //                case 0x39:
        //                    osae.AddToLog("Full screen", false);
        //                    break;
        //                case 0x3a:
        //                    osae.AddToLog("DVD Audio", false);
        //                    break;
        //                case 0x70:
        //                    osae.AddToLog("Cursor-left", false);
        //                    break;
        //                case 0x71:
        //                    osae.AddToLog("Cursor-right", false);
        //                    break;
        //                case 0x72:
        //                    osae.AddToLog("Cursor-up", false);
        //                    break;
        //                case 0x73:
        //                    osae.AddToLog("Cursor-down", false);
        //                    break;
        //                case 0x74:
        //                    osae.AddToLog("Cursor-up-left", false);
        //                    break;
        //                case 0x75:
        //                    osae.AddToLog("Cursor-up-right", false);
        //                    break;
        //                case 0x76:
        //                    osae.AddToLog("Cursor-down-right", false);
        //                    break;
        //                case 0x77:
        //                    osae.AddToLog("Cursor-down-left", false);
        //                    break;
        //                case 0x78:
        //                    osae.AddToLog("Left Mouse Button", false);
        //                    break;
        //                case 0x79:
        //                    osae.AddToLog("V-End", false);
        //                    break;
        //                case 0x7c:
        //                    osae.AddToLog("Right Mouse Button", false);
        //                    break;
        //                case 0x7d:
        //                    osae.AddToLog("X-End", false);
        //                    break;
        //                default:
        //                    osae.AddToLog("unknown", false);
        //                    break;
        //            }
        //            if ((recbuf(REMOTE.toggle) & 0x1) == 0x1)
        //            {
        //                osae.AddToLog("  (button press = odd)");
        //            }
        //            else
        //            {
        //                osae.AddToLog("  (button press = even)");
        //            }

        //            break;
        //        case REMOTE.Medion:
        //            osae.AddToLog("subtype       = Medion Remote");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + recbuf(REMOTE.id).ToString);
        //            osae.AddToLog("Command       = ", false);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("Mute");
        //                    break;
        //                case 0x1:
        //                    osae.AddToLog("B");
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("power");
        //                    break;
        //                case 0x3:
        //                    osae.AddToLog("TV");
        //                    break;
        //                case 0x4:
        //                    osae.AddToLog("DVD");
        //                    break;
        //                case 0x5:
        //                    osae.AddToLog("Photo");
        //                    break;
        //                case 0x6:
        //                    osae.AddToLog("Music");
        //                    break;
        //                case 0x7:
        //                    osae.AddToLog("Drag");
        //                    break;
        //                case 0x8:
        //                    osae.AddToLog("VOL-");
        //                    break;
        //                case 0x9:
        //                    osae.AddToLog("VOL+");
        //                    break;
        //                case 0xa:
        //                    osae.AddToLog("MUTE");
        //                    break;
        //                case 0xb:
        //                    osae.AddToLog("CHAN+");
        //                    break;
        //                case 0xc:
        //                    osae.AddToLog("CHAN-");
        //                    break;
        //                case 0xd:
        //                    osae.AddToLog("1");
        //                    break;
        //                case 0xe:
        //                    osae.AddToLog("2");
        //                    break;
        //                case 0xf:
        //                    osae.AddToLog("3");
        //                    break;
        //                case 0x10:
        //                    osae.AddToLog("4");
        //                    break;
        //                case 0x11:
        //                    osae.AddToLog("5");
        //                    break;
        //                case 0x12:
        //                    osae.AddToLog("6");
        //                    break;
        //                case 0x13:
        //                    osae.AddToLog("7");
        //                    break;
        //                case 0x14:
        //                    osae.AddToLog("8");
        //                    break;
        //                case 0x15:
        //                    osae.AddToLog("9");
        //                    break;
        //                case 0x16:
        //                    osae.AddToLog("txt");
        //                    break;
        //                case 0x17:
        //                    osae.AddToLog("0");
        //                    break;
        //                case 0x18:
        //                    osae.AddToLog("snapshot ESC");
        //                    break;
        //                case 0x19:
        //                    osae.AddToLog("DVD MENU");
        //                    break;
        //                case 0x1a:
        //                    osae.AddToLog("^");
        //                    break;
        //                case 0x1b:
        //                    osae.AddToLog("Setup");
        //                    break;
        //                case 0x1c:
        //                    osae.AddToLog("TV/RADIO");
        //                    break;
        //                case 0x1d:
        //                    osae.AddToLog("<");
        //                    break;
        //                case 0x1e:
        //                    osae.AddToLog("OK");
        //                    break;
        //                case 0x1f:
        //                    osae.AddToLog(">");
        //                    break;
        //                case 0x20:
        //                    osae.AddToLog("<-");
        //                    break;
        //                case 0x21:
        //                    osae.AddToLog("E");
        //                    break;
        //                case 0x22:
        //                    osae.AddToLog("v");
        //                    break;
        //                case 0x23:
        //                    osae.AddToLog("F");
        //                    break;
        //                case 0x24:
        //                    osae.AddToLog("Rewind");
        //                    break;
        //                case 0x25:
        //                    osae.AddToLog("Play");
        //                    break;
        //                case 0x26:
        //                    osae.AddToLog("Fast forward");
        //                    break;
        //                case 0x27:
        //                    osae.AddToLog("Record");
        //                    break;
        //                case 0x28:
        //                    osae.AddToLog("Stop");
        //                    break;
        //                case 0x29:
        //                    osae.AddToLog("Pause");

        //                    break;
        //                case 0x2c:
        //                    osae.AddToLog("TV");
        //                    break;
        //                case 0x2d:
        //                    osae.AddToLog("VCR");
        //                    break;
        //                case 0x2e:
        //                    osae.AddToLog("RADIO");
        //                    break;
        //                case 0x2f:
        //                    osae.AddToLog("TV Preview");
        //                    break;
        //                case 0x30:
        //                    osae.AddToLog("Channel list");
        //                    break;
        //                case 0x31:
        //                    osae.AddToLog("Video Desktop");
        //                    break;
        //                case 0x32:
        //                    osae.AddToLog("red");
        //                    break;
        //                case 0x33:
        //                    osae.AddToLog("green");
        //                    break;
        //                case 0x34:
        //                    osae.AddToLog("yellow");
        //                    break;
        //                case 0x35:
        //                    osae.AddToLog("blue");
        //                    break;
        //                case 0x36:
        //                    osae.AddToLog("rename TAB");
        //                    break;
        //                case 0x37:
        //                    osae.AddToLog("Acquire image");
        //                    break;
        //                case 0x38:
        //                    osae.AddToLog("edit image");
        //                    break;
        //                case 0x39:
        //                    osae.AddToLog("Full screen");
        //                    break;
        //                case 0x3a:
        //                    osae.AddToLog("DVD Audio");
        //                    break;
        //                case 0x70:
        //                    osae.AddToLog("Cursor-left");
        //                    break;
        //                case 0x71:
        //                    osae.AddToLog("Cursor-right");
        //                    break;
        //                case 0x72:
        //                    osae.AddToLog("Cursor-up");
        //                    break;
        //                case 0x73:
        //                    osae.AddToLog("Cursor-down");
        //                    break;
        //                case 0x74:
        //                    osae.AddToLog("Cursor-up-left");
        //                    break;
        //                case 0x75:
        //                    osae.AddToLog("Cursor-up-right");
        //                    break;
        //                case 0x76:
        //                    osae.AddToLog("Cursor-down-right");
        //                    break;
        //                case 0x77:
        //                    osae.AddToLog("Cursor-down-left");
        //                    break;
        //                case 0x78:
        //                    osae.AddToLog("V");
        //                    break;
        //                case 0x79:
        //                    osae.AddToLog("V-End");
        //                    break;
        //                case 0x7c:
        //                    osae.AddToLog("X");
        //                    break;
        //                case 0x7d:
        //                    osae.AddToLog("X-End");
        //                    break;
        //                default:
        //                    osae.AddToLog("unknown");
        //                    break;
        //            }

        //            break;
        //        case REMOTE.PCremote:
        //            osae.AddToLog("subtype       = PC Remote");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + recbuf(REMOTE.id).ToString);
        //            osae.AddToLog("Command       = unknown", false);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x2:
        //                    osae.AddToLog("0");
        //                    break;
        //                case 0x82:
        //                    osae.AddToLog("1");
        //                    break;
        //                case 0xd1:
        //                    osae.AddToLog("MP3");
        //                    break;
        //                case 0x42:
        //                    osae.AddToLog("2");
        //                    break;
        //                case 0xd2:
        //                    osae.AddToLog("DVD");
        //                    break;
        //                case 0xc2:
        //                    osae.AddToLog("3");
        //                    break;
        //                case 0xd3:
        //                    osae.AddToLog("CD");
        //                    break;
        //                case 0x22:
        //                    osae.AddToLog("4");
        //                    break;
        //                case 0xd4:
        //                    osae.AddToLog("PC or SHIFT-4");
        //                    break;
        //                case 0xa2:
        //                    osae.AddToLog("5");
        //                    break;
        //                case 0xd5:
        //                    osae.AddToLog("SHIFT-5");
        //                    break;
        //                case 0x62:
        //                    osae.AddToLog("6");
        //                    break;
        //                case 0xe2:
        //                    osae.AddToLog("7");
        //                    break;
        //                case 0x12:
        //                    osae.AddToLog("8");
        //                    break;
        //                case 0x92:
        //                    osae.AddToLog("9");
        //                    break;
        //                case 0xc0:
        //                    osae.AddToLog("CH-");
        //                    break;
        //                case 0x40:
        //                    osae.AddToLog("CH+");
        //                    break;
        //                case 0xe0:
        //                    osae.AddToLog("VOL-");
        //                    break;
        //                case 0x60:
        //                    osae.AddToLog("VOL+");
        //                    break;
        //                case 0xa0:
        //                    osae.AddToLog("MUTE");
        //                    break;
        //                case 0x3a:
        //                    osae.AddToLog("INFO");
        //                    break;
        //                case 0x38:
        //                    osae.AddToLog("REW");
        //                    break;
        //                case 0xb8:
        //                    osae.AddToLog("FF");
        //                    break;
        //                case 0xb0:
        //                    osae.AddToLog("PLAY");
        //                    break;
        //                case 0x64:
        //                    osae.AddToLog("PAUSE");
        //                    break;
        //                case 0x63:
        //                    osae.AddToLog("STOP");
        //                    break;
        //                case 0xb6:
        //                    osae.AddToLog("MENU");
        //                    break;
        //                case 0xff:
        //                    osae.AddToLog("REC");
        //                    break;
        //                case 0xc9:
        //                    osae.AddToLog("EXIT");
        //                    break;
        //                case 0xd8:
        //                    osae.AddToLog("TEXT");
        //                    break;
        //                case 0xd9:
        //                    osae.AddToLog("SHIFT-TEXT");
        //                    break;
        //                case 0xf2:
        //                    osae.AddToLog("TELETEXT");
        //                    break;
        //                case 0xd7:
        //                    osae.AddToLog("SHIFT-TELETEXT");
        //                    break;
        //                case 0xba:
        //                    osae.AddToLog("A+B");
        //                    break;
        //                case 0x52:
        //                    osae.AddToLog("ENT");
        //                    break;
        //                case 0xd6:
        //                    osae.AddToLog("SHIFT-ENT");
        //                    break;
        //                case 0x70:
        //                    osae.AddToLog("Cursor-left");
        //                    break;
        //                case 0x71:
        //                    osae.AddToLog("Cursor-right");
        //                    break;
        //                case 0x72:
        //                    osae.AddToLog("Cursor-up");
        //                    break;
        //                case 0x73:
        //                    osae.AddToLog("Cursor-down");
        //                    break;
        //                case 0x74:
        //                    osae.AddToLog("Cursor-up-left");
        //                    break;
        //                case 0x75:
        //                    osae.AddToLog("Cursor-up-right");
        //                    break;
        //                case 0x76:
        //                    osae.AddToLog("Cursor-down-right");
        //                    break;
        //                case 0x77:
        //                    osae.AddToLog("Cursor-down-left");
        //                    break;
        //                case 0x78:
        //                    osae.AddToLog("Left mouse");
        //                    break;
        //                case 0x79:
        //                    osae.AddToLog("Left mouse-End");
        //                    break;
        //                case 0x7b:
        //                    osae.AddToLog("Drag");
        //                    break;
        //                case 0x7c:
        //                    osae.AddToLog("Right mouse");
        //                    break;
        //                case 0x7d:
        //                    osae.AddToLog("Right mouse-End");
        //                    break;
        //                default:
        //                    osae.AddToLog("unknown");
        //                    break;
        //            }

        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(REMOTE.packettype)) + ":" + Conversion.Hex(recbuf(REMOTE.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Signal level  = " + (recbuf(REMOTE.rssi) >> 4).ToString());

        //}

        //public void decode_Thermostat1()
        //{
        //    switch (recbuf(THERMOSTAT1.subtype))
        //    {
        //        case THERMOSTAT1.Digimax:
        //            osae.AddToLog("subtype       = Digimax");
        //            break;
        //        case THERMOSTAT1.DigimaxShort:
        //            osae.AddToLog("subtype       = Digimax with short format");
        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(THERMOSTAT1.packettype)) + ":" + Conversion.Hex(recbuf(THERMOSTAT1.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Sequence nbr  = " + recbuf(THERMOSTAT1.seqnbr).ToString);
        //    osae.AddToLog("ID            = " + ((recbuf(THERMOSTAT1.id1) * 256 + recbuf(THERMOSTAT1.id2))).ToString());
        //    osae.AddToLog("Temperature   = " + recbuf(THERMOSTAT1.temperature).ToString + " °C");
        //    if (recbuf(THERMOSTAT1.subtype) == THERMOSTAT1.Digimax)
        //    {
        //        osae.AddToLog("Set           = " + recbuf(THERMOSTAT1.set_point).ToString + " °C");
        //        if ((recbuf(THERMOSTAT1.mode) & 0x80) == 0)
        //        {
        //            osae.AddToLog("Mode          = heating");
        //        }
        //        else
        //        {
        //            osae.AddToLog("Mode          = Cooling");
        //        }
        //        switch ((recbuf(THERMOSTAT1.status) & 0x3))
        //        {
        //            case 0:
        //                osae.AddToLog("Status        = no status available");
        //                break;
        //            case 1:
        //                osae.AddToLog("Status        = demand");
        //                break;
        //            case 2:
        //                osae.AddToLog("Status        = no demand");
        //                break;
        //            case 3:
        //                osae.AddToLog("Status        = initializing");
        //                break;
        //        }
        //    }

        //    osae.AddToLog("Signal level  = " + (recbuf(THERMOSTAT1.rssi) >> 4).ToString());
        //}

        //public void decode_Thermostat2()
        //{
        //    osae.AddToLog("Not implemented");
        //}

        //public void decode_Thermostat3()
        //{
        //    switch (recbuf(THERMOSTAT3.subtype))
        //    {
        //        case THERMOSTAT3.MertikG6RH4T1:
        //            osae.AddToLog("subtype       = Mertik G6R-H4T1");
        //            break;
        //        case THERMOSTAT3.MertikG6RH4TB:
        //            osae.AddToLog("subtype       = Mertik G6R-H4TB");
        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(THERMOSTAT3.packettype)) + ":" + Conversion.Hex(recbuf(THERMOSTAT3.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Sequence nbr  = " + recbuf(THERMOSTAT3.seqnbr).ToString);

        //    osae.AddToLog("ID            = 0x" + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode2)), 2) + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode3)), 2));

        //    switch (recbuf(THERMOSTAT3.cmnd))
        //    {
        //        case 0:
        //            osae.AddToLog("Command       = Off");
        //            break;
        //        case 1:
        //            osae.AddToLog("Command       = On");
        //            break;
        //        case 2:
        //            osae.AddToLog("Command       = Up");
        //            break;
        //        case 3:
        //            osae.AddToLog("Command       = Down");
        //            break;
        //        case 4:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                osae.AddToLog("Command       = Run Up");
        //            }
        //            else
        //            {
        //                osae.AddToLog("Command       = 2nd Off");
        //            }
        //            break;
        //        case 5:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                osae.AddToLog("Command       = Run Down");
        //            }
        //            else
        //            {
        //                osae.AddToLog("Command       = 2nd On");
        //            }
        //            break;
        //        case 6:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                osae.AddToLog("Command       = Stop");
        //            }
        //            else
        //            {
        //                osae.AddToLog("Command       = unknown");
        //            }
        //            break;
        //        default:
        //            osae.AddToLog("Command       = unknown");
        //            break;
        //    }

        //    osae.AddToLog("Signal level  = " + (recbuf(THERMOSTAT3.rssi) >> 4).ToString());
        //}

        public void decode_Temp()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName,"Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New temperature sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Temperature Sensor - " + (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString(), "Temperature Sensor", "OS TEMP SENSOR", (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString());
            }

            switch (recbuf[(byte)TEMP.subtype])
            {
                case (byte)TEMP.TEMP1:
                    osae.AddToLog("subtype       = TEMP1 - THR128/138, THC138", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP.id2].ToString(), false);
                    break;
                case (byte)TEMP.TEMP2:
                    osae.AddToLog("subtype       = TEMP2 - THC238/268,THN132,THWR288,THRN122,THN122,AW129/131", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP.id2].ToString(), false);
                    break;
                case (byte)TEMP.TEMP3:
                    osae.AddToLog("subtype       = TEMP3 - THWR800", false);
                    break;
                case (byte)TEMP.TEMP4:
                    osae.AddToLog("subtype       = TEMP4 - RTHN318", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP.id2].ToString(), false);
                    break;
                case (byte)TEMP.TEMP5:
                    osae.AddToLog("subtype       = TEMP5 - LaCrosse TX3, TX4, TX17", false);
                    break;
                case (byte)TEMP.TEMP6:
                    osae.AddToLog("subtype       = TEMP6 - TS15C", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP.packettype].ToString() + ":" + recbuf[(byte)TEMP.subtype].ToString(), false);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)TEMP.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString(), false);

            if ((recbuf[(byte)TEMP.tempsign] & 0x80) == 0)
            {
                osae.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)TEMP.temperatureh] * 256 + recbuf[(byte)TEMP.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)TEMP.temperatureh] * 256 + recbuf[(byte)TEMP.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            else
            {
                osae.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)TEMP.temperatureh] * 256 + recbuf[(byte)TEMP.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)TEMP.temperatureh] * 256 + recbuf[(byte)TEMP.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            osae.AddToLog("Signal level  = " + (recbuf[(byte)TEMP.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)TEMP.battery_level] & 0xf) == 0)
            {
                osae.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                osae.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Hum()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New humidity sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Humidity Sensor - " + (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString(), "Humidity Sensor", "HUMIDITY METER", (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString());
            }

            switch (recbuf[(byte)HUM.subtype])
            {
                case (byte)HUM.HUM1:
                    osae.AddToLog("subtype       = HUM1 - LaCrosse TX3", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)HUM.packettype] + ":" + recbuf[(byte)HUM.subtype], true);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)HUM.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString(), false);
            osae.AddToLog("Humidity      = " + recbuf[(byte)HUM.humidity].ToString(), false);
            osae.ObjectPropertySet(obj.Name, "Humidity", recbuf[(byte)HUM.humidity].ToString());

            switch (recbuf[(byte)HUM.humidity_status])
            {
                case 0x0:
                    osae.AddToLog("Status        = Dry", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Dry");
                    break;
                case 0x1:
                    osae.AddToLog("Status        = Comfortable", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Comfortable");
                    break;
                case 0x2:
                    osae.AddToLog("Status        = Normal", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Normal");
                    break;
                case 0x3:
                    osae.AddToLog("Status        = Wet", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Wet");
                    break;
            }
            osae.AddToLog("Signal level  = " + (recbuf[(byte)HUM.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)HUM.battery_level] & 0xf) == 0)
            {
                osae.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                osae.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_TempHum()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New temperature and humidity sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Temp and Humidity Sensor - " + (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString(), "Temp and Humidity Sensor", "TEMP HUM METER", (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString());
            }
            
            switch (recbuf[(byte)TEMP_HUM.subtype])
            {
                case (byte)TEMP_HUM.TH1:
                    osae.AddToLog("subtype       = TH1 - THGN122/123,/THGN132,THGR122/228/238/268", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH2:
                    osae.AddToLog("subtype       = TH2 - THGR810,THGN800", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH3:
                    osae.AddToLog("subtype       = TH3 - RTGR328", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH4:
                    osae.AddToLog("subtype       = TH4 - THGR328", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH5:
                    osae.AddToLog("subtype       = TH5 - WTGR800", false);
                    break;
                case (byte)TEMP_HUM.TH6:
                    osae.AddToLog("subtype       = TH6 - THGR918,THGRN228,THGN500", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM.TH7:
                    osae.AddToLog("subtype       = TH7 - Cresta, TFA TS34C", false);
                    if (recbuf[(byte)TEMP_HUM.id1] < 0x40)
                    {
                        osae.AddToLog("                channel 1", false);
                    }
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0x60)
                    {
                        osae.AddToLog("                channel 2", false);
                    }
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0x80)
                    {
                        osae.AddToLog("                channel 3", false);
                    }
                    else if (recbuf[(byte)TEMP_HUM.id1] > 0x9f & (byte)TEMP_HUM.id1 < 0xc0)
                    {
                        osae.AddToLog("                channel 4", false);
                    }
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0xe0)
                    {
                        osae.AddToLog("                channel 5", false);
                    }
                    else
                    {
                        osae.AddToLog("                channel ??", false);
                    }
                    break;
                case (byte)TEMP_HUM.TH8:
                    osae.AddToLog("subtype       = TH8 - WT440H,WT450H", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString(), false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP_HUM.packettype] + ":" + recbuf[(byte)TEMP_HUM.subtype], false);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)TEMP_HUM.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString(), false);
            if ((recbuf[(byte)TEMP_HUM.tempsign] & 0x80) == 0)
            {
                osae.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            else
            {
                osae.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            osae.AddToLog("Humidity      = " + recbuf[(byte)TEMP_HUM.humidity].ToString(), false);
            switch (recbuf[(byte)TEMP_HUM.humidity_status])
            {
                case 0x0:
                    osae.AddToLog("Status        = Dry", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Dry");
                    break;
                case 0x1:
                    osae.AddToLog("Status        = Comfortable", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Comfortable");
                    break;
                case 0x2:
                    osae.AddToLog("Status        = Normal", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Normal");
                    break;
                case 0x3:
                    osae.AddToLog("Status        = Wet", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Wet");
                    break;
            }
            osae.AddToLog("Signal level  = " + (recbuf[(byte)TEMP_HUM.rssi] >> 4).ToString(), false);
            if (recbuf[(byte)TEMP_HUM.subtype] == (byte)TEMP_HUM.TH6)
            {
                switch (recbuf[(byte)TEMP_HUM.battery_level])
                {
                    case 0:
                        osae.AddToLog("Battery       = 10%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "10%");
                        break;
                    case 1:
                        osae.AddToLog("Battery       = 20%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "20%");
                        break;
                    case 2:
                        osae.AddToLog("Battery       = 30%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "30%");
                        break;
                    case 3:
                        osae.AddToLog("Battery       = 40%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "40%");
                        break;
                    case 4:
                        osae.AddToLog("Battery       = 50%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "50%");
                        break;
                    case 5:
                        osae.AddToLog("Battery       = 60%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "60%");
                        break;
                    case 6:
                        osae.AddToLog("Battery       = 70%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "70%");
                        break;
                    case 7:
                        osae.AddToLog("Battery       = 80%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "80%");
                        break;
                    case 8:
                        osae.AddToLog("Battery       = 90%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "90%");
                        break;
                    case 9:
                        osae.AddToLog("Battery       = 100%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "100%");
                        break;
                }
            }
            else
            {
                if ((recbuf[(byte)TEMP_HUM.battery_level] & 0xf) == 0)
                {
                    osae.AddToLog("Battery       = Low", false);
                    osae.ObjectPropertySet(obj.Name, "Battery", "Low");
                }
                else
                {
                    osae.AddToLog("Battery       = OK", false);
                    osae.ObjectPropertySet(obj.Name, "Battery", "OK");
                }
            }
        }

        public void decode_Baro()
        {
            osae.AddToLog("Baro Not implemented", true);
        }

        public void decode_TempHumBaro()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New temperature, humidity and barometric sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Temp, Humidity and Baro Sensor - " + (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString(), "Temp, Humidity and Baro Sensor", "TEMP HUM BARO METER", (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString());
            }
            
            switch (recbuf[(byte)TEMP_HUM_BARO.subtype])
            {
                case (byte)TEMP_HUM_BARO.THB1:
                    osae.AddToLog("subtype       = THB1 - BTHR918", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP_HUM_BARO.id2].ToString(), false);
                    break;
                case (byte)TEMP_HUM_BARO.THB2:
                    osae.AddToLog("subtype       = THB2 - BTHR918N, BTHR968", false);
                    osae.AddToLog("                channel " + recbuf[(byte)TEMP_HUM_BARO.id2].ToString(), false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP_HUM_BARO.packettype] + ":" + recbuf[(byte)TEMP_HUM_BARO.subtype], false);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)TEMP_HUM_BARO.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString(), false);
            if ((recbuf[(byte)TEMP_HUM_BARO.tempsign] & 0x80) == 0)
            {
                osae.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            else
            {
                osae.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                osae.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
            }
            osae.AddToLog("Humidity      = " + recbuf[(byte)TEMP_HUM_BARO.humidity].ToString(), false);
            osae.ObjectPropertySet(obj.Name, "Humidity", recbuf[(byte)TEMP_HUM_BARO.humidity].ToString());
            
            switch (recbuf[(byte)TEMP_HUM_BARO.humidity_status])
            {
                case 0x0:
                    osae.AddToLog("Status        = Dry", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Dry");
                    break;
                case 0x1:
                    osae.AddToLog("Status        = Comfortable", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Comfortable");
                    break;
                case 0x2:
                    osae.AddToLog("Status        = Normal", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Normal");
                    break;
                case 0x3:
                    osae.AddToLog("Status        = Wet", false);
                    osae.ObjectPropertySet(obj.Name, "Status", "Wet");
                    break;
            }
            osae.AddToLog("Barometer     = " + recbuf[(byte)TEMP_HUM_BARO.baroh] * 256 + recbuf[(byte)TEMP_HUM_BARO.barol].ToString(), false);
            osae.ObjectPropertySet(obj.Name, "Barometer", recbuf[(byte)TEMP_HUM_BARO.baroh] * 256 + recbuf[(byte)TEMP_HUM_BARO.barol].ToString());

            switch (recbuf[(byte)TEMP_HUM_BARO.forecast])
            {
                case 0x0:
                    osae.AddToLog("Forecast      = No information available", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "No information available");
                    break;
                case 0x1:
                    osae.AddToLog("Forecast      = Sunny", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "Sunny");
                    break;
                case 0x2:
                    osae.AddToLog("Forecast      = Partly Cloudy", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "Partly Cloudy");
                    break;
                case 0x3:
                    osae.AddToLog("Forecast      = Cloudy", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "Cloudy");
                    break;
                case 0x4:
                    osae.AddToLog("Forecast      = Rain", false);
                    osae.ObjectPropertySet(obj.Name, "Forecast", "Rain");
                    break;
            }

            osae.AddToLog("Signal level  = " + (recbuf[(byte)TEMP_HUM_BARO.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)TEMP_HUM_BARO.battery_level] & 0xf) == 0)
            {
                osae.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                osae.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Rain()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New temperature sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Rain Meter - " + (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString(), "Rain Meter", "OS RAIN METER", (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString());
            }

            switch (recbuf[(byte)RAIN.subtype])
            {
                case (byte)RAIN.RAIN1:
                    osae.AddToLog("subtype       = RAIN1 - RGR126/682/918", false);
                    break;
                case (byte)RAIN.RAIN2:
                    osae.AddToLog("subtype       = RAIN2 - PCR800", false);
                    break;
                case (byte)RAIN.RAIN3:
                    osae.AddToLog("subtype       = RAIN3 - TFA", false);
                    break;
                case (byte)RAIN.RAIN4:
                    osae.AddToLog("subtype       = RAIN4 - UPM RG700", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)RAIN.packettype].ToString() + ":" + recbuf[(byte)RAIN.subtype].ToString(), true);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)RAIN.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString(), false);

            if (recbuf[(byte)RAIN.subtype] == (byte)RAIN.RAIN1)
            {
                osae.AddToLog("Rain rate     = " + ((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]).ToString() + " mm/h", false);
                osae.ObjectPropertySet(obj.Name, "Rain Rate", ((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]).ToString());
            }
            else if (recbuf[(byte)RAIN.subtype] == (byte)RAIN.RAIN2)
            {
                osae.AddToLog("Rain rate     = " + (((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]) / 100).ToString() + " mm/h", false);
                osae.ObjectPropertySet(obj.Name, "Rain Rate", (((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]) / 100).ToString());
            }

            osae.AddToLog("Total rain    = " + Math.Round((double)((recbuf[(byte)RAIN.raintotal1] * 65535) + recbuf[(byte)RAIN.raintotal2] * 256 + recbuf[(byte)RAIN.raintotal3]) / 10, 2).ToString() + " mm", false);
            osae.ObjectPropertySet(obj.Name, "Total Rain", Math.Round((double)((recbuf[(byte)RAIN.raintotal1] * 65535) + recbuf[(byte)RAIN.raintotal2] * 256 + recbuf[(byte)RAIN.raintotal3]) / 10, 2).ToString());

            osae.AddToLog("Signal level  = " + (recbuf[(byte)RAIN.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)RAIN.battery_level] & 0xf) == 0)
            {
                osae.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                osae.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Wind()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New wind sensor found.  Adding to OSA", true);
                osae.ObjectAdd("Wind Sensor - " + (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString(), "Wind Sensor", "WIND SENSOR", (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString());
            }

            int intDirection = 0;
            int intSpeed = 0;
            string strDirection = null;

            switch (recbuf[(byte)WIND.subtype])
            {
                case (byte)WIND.WIND1:
                    osae.AddToLog("subtype       = WIND1 - WTGR800", false);
                    break;
                case (byte)WIND.WIND2:
                    osae.AddToLog("subtype       = WIND2 - WGR800", false);
                    break;
                case (byte)WIND.WIND3:
                    osae.AddToLog("subtype       = WIND3 - STR918, WGR918", false);
                    break;
                case (byte)WIND.WIND4:
                    osae.AddToLog("subtype       = WIND4 - TFA", false);
                    break;
                case (byte)WIND.WIND5:
                    osae.AddToLog("subtype       = WIND5 - UPM WDS500", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)WIND.packettype] + ":" + recbuf[(byte)WIND.subtype], false);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)WIND.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString(), false);
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
            osae.AddToLog("Direction     = " + intDirection.ToString() + " degrees  " + strDirection, false);
            osae.ObjectPropertySet(obj.Name, "Direction", intDirection.ToString() + " degrees  " + strDirection);

            intSpeed = (recbuf[(byte)WIND.av_speedh] * 256) + recbuf[(byte)WIND.av_speedl];
            if (recbuf[(byte)WIND.subtype] != (byte)WIND.WIND5)
            {
                osae.AddToLog("Average speed = " + (intSpeed / 10).ToString() + " mtr/sec = " + Math.Round((intSpeed * 0.36), 2).ToString() + " km/hr = " + Math.Round((intSpeed * 0.223693629) / 10, 2).ToString() + " mph", false);
                osae.ObjectPropertySet(obj.Name, "Average Speed", Math.Round((intSpeed * 0.223693629) / 10, 2).ToString());
            }

            intSpeed = (recbuf[(byte)WIND.gusth] * 256) + recbuf[(byte)WIND.gustl];
            osae.AddToLog("Wind gust     = " + (intSpeed / 10).ToString() + " mtr/sec = " + Math.Round((intSpeed * 0.36), 2).ToString() + " km/hr = " + Math.Round((intSpeed * 0.223693629) / 10, 2).ToString() + " mph", false);
            osae.ObjectPropertySet(obj.Name, "Wind Gust", Math.Round((intSpeed * 0.223693629) / 10, 2).ToString()); 
            
            if (recbuf[(byte)WIND.subtype] == (byte)WIND.WIND4)
            {
                if (((byte)WIND.tempsign & 0x80) == 0)
                {
                    osae.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
                else
                {
                    osae.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
                }

                if (((byte)WIND.chillsign & 0x80) == 0)
                {
                    osae.AddToLog("Chill         = " + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Windchill", (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
                else
                {
                    osae.AddToLog("Chill         = -" + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Windchill", "-" + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
            }

            osae.AddToLog("Signal level  = " + (recbuf[(byte)WIND.rssi] >> 4).ToString(), false);
            if (recbuf[(byte)WIND.subtype] == (byte)WIND.WIND3)
            {
                switch (recbuf[(byte)WIND.battery_level])
                {
                    case 0:
                        osae.AddToLog("Battery       = 10%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "10%");
                        break;
                    case 1:
                        osae.AddToLog("Battery       = 20%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "20%");
                        break;
                    case 2:
                        osae.AddToLog("Battery       = 30%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "30%");
                        break;
                    case 3:
                        osae.AddToLog("Battery       = 40%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "40%");
                        break;
                    case 4:
                        osae.AddToLog("Battery       = 50%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "50%");
                        break;
                    case 5:
                        osae.AddToLog("Battery       = 60%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "60%");
                        break;
                    case 6:
                        osae.AddToLog("Battery       = 70%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "70%");
                        break;
                    case 7:
                        osae.AddToLog("Battery       = 80%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "80%");
                        break;
                    case 8:
                        osae.AddToLog("Battery       = 90%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "90%");
                        break;
                    case 9:
                        osae.AddToLog("Battery       = 100%", false);
                        osae.ObjectPropertySet(obj.Name, "Battery", "100%");
                        break;
                }
            }
            else
            {
                if ((recbuf[(byte)WIND.battery_level] & 0xf) == 0)
                {
                    osae.AddToLog("Battery       = Low", false);
                    osae.ObjectPropertySet(obj.Name, "Battery", "Low");
                }
                else
                {
                    osae.AddToLog("Battery       = OK", false);
                    osae.ObjectPropertySet(obj.Name, "Battery", "OK");
                }
            }
        }

        public void decode_UV()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New UV sensor found.  Adding to OSA", true);
                osae.ObjectAdd("UV Sensor - " + (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString(), "UV Sensor", "UV SENSOR", (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString());
            }

            switch (recbuf[(byte)UV.subtype])
            {
                case (byte)UV.UV1:
                    osae.AddToLog("Subtype       = UV1 - UVN128, UV138", false);
                    break;
                case (byte)UV.UV2:
                    osae.AddToLog("Subtype       = UV2 - UVN800", false);
                    break;
                case (byte)UV.UV3:
                    osae.AddToLog("Subtype       = UV3 - TFA", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + (byte)UV.packettype + ":" + recbuf[(byte)UV.subtype], false);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)UV.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString(), false);
            osae.AddToLog("Level         = " + (recbuf[(byte)UV.uv] / 10).ToString(), false);
            osae.ObjectPropertySet(obj.Name, "Level", (recbuf[(byte)UV.uv] / 10).ToString());

            if (recbuf[(byte)UV.subtype] == (byte)UV.UV3)
            {
                if (((byte)UV.tempsign & 0x80) == 0)
                {
                    osae.AddToLog("Temperature   = " + (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Level", (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
                else
                {
                    osae.AddToLog("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F", false);
                    osae.ObjectPropertySet(obj.Name, "Level", (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString());
                }
            }
            if (recbuf[(byte)UV.uv] < 3)
            {
                osae.AddToLog("Description = Low", false);
                osae.ObjectPropertySet(obj.Name, "Description", "Low");
            }
            else if (recbuf[(byte)UV.uv] < 6)
            {
                osae.AddToLog("Description = Medium", false);
                osae.ObjectPropertySet(obj.Name, "Description", "Medium");
            }
            else if (recbuf[(byte)UV.uv] < 8)
            {
                osae.AddToLog("Description = High", false);
                osae.ObjectPropertySet(obj.Name, "Description", "High");
            }
            else if (recbuf[(byte)UV.uv] < 11)
            {
                osae.AddToLog("Description = Very high", false);
                osae.ObjectPropertySet(obj.Name, "Description", "Very high");
            }
            else
            {
                osae.AddToLog("Description = Dangerous", false);
                osae.ObjectPropertySet(obj.Name, "Description", "Dangerous");
            }
            osae.AddToLog("Signal level  = " + (recbuf[(byte)UV.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)UV.battery_level] & 0xf) == 0)
            {
                osae.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                osae.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }


        public void decode_DateTime()
        {
            osae.AddToLog("DateTime Not implemented", true);
        }

        public void decode_Current()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New Current meter found.  Adding to OSA", true);
                osae.ObjectAdd("Current Meter - " + (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString(), "Current Meter", "CURRENT METER", (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString());
            }

            switch (recbuf[(byte)CURRENT.subtype])
            {
                case (byte)CURRENT.ELEC1:
                    osae.AddToLog("subtype       = ELEC1 - OWL CM113, Electrisave, cent-a-meter", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)CURRENT.packettype] + ":" + recbuf[(byte)CURRENT.subtype], false);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)CURRENT.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString(), false);
            osae.AddToLog("Count         = " + recbuf[5].ToString(), false);
            osae.AddToLog("Channel 1     = " + ((recbuf[(byte)CURRENT.ch1h] * 256 + recbuf[(byte)CURRENT.ch1l]) / 10).ToString() + " ampere", false);
            osae.AddToLog("Channel 2     = " + ((recbuf[(byte)CURRENT.ch2h] * 256 + recbuf[(byte)CURRENT.ch2l]) / 10).ToString() + " ampere", false);
            osae.AddToLog("Channel 3     = " + ((recbuf[(byte)CURRENT.ch3h] * 256 + recbuf[(byte)CURRENT.ch3l]) / 10).ToString() + " ampere", false);
            osae.ObjectPropertySet(obj.Name, "Count", recbuf[5].ToString());
            osae.ObjectPropertySet(obj.Name, "Channel 1", ((recbuf[(byte)CURRENT.ch1h] * 256 + recbuf[(byte)CURRENT.ch1l]) / 10).ToString());
            osae.ObjectPropertySet(obj.Name, "Channel 2", ((recbuf[(byte)CURRENT.ch2h] * 256 + recbuf[(byte)CURRENT.ch2l]) / 10).ToString());
            osae.ObjectPropertySet(obj.Name, "Channel 3", ((recbuf[(byte)CURRENT.ch3h] * 256 + recbuf[(byte)CURRENT.ch3l]) / 10).ToString());

            osae.AddToLog("Signal level  = " + (recbuf[(byte)CURRENT.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)CURRENT.battery_level] & 0xf) == 0)
            {
                osae.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                osae.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Energy()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New Energy meter found.  Adding to OSA", true);
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
                    osae.AddToLog("subtype       = ELEC2 - OWL CM119, CM160", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)ENERGY.packettype] + ":" + recbuf[(byte)ENERGY.subtype], false);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)ENERGY.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString(), false);
            osae.AddToLog("Count         = " + recbuf[(byte)ENERGY.count].ToString(), false);
            osae.AddToLog("Instant usage = " + instant.ToString() + " Watt", false);
            osae.AddToLog("total usage   = " + usage.ToString() + " Wh", false);
            osae.ObjectPropertySet(obj.Name, "Count", recbuf[5].ToString());
            osae.ObjectPropertySet(obj.Name, "Instant usage", instant.ToString());
            osae.ObjectPropertySet(obj.Name, "Total usage", usage.ToString());

            osae.AddToLog("Signal level  = " + (recbuf[(byte)ENERGY.rssi] >> 4).ToString(), false);
            if ((recbuf[(byte)ENERGY.battery_level] & 0xf) == 0)
            {
                osae.AddToLog("Battery       = Low", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "Low");
            }
            else
            {
                osae.AddToLog("Battery       = OK", false);
                osae.ObjectPropertySet(obj.Name, "Battery", "OK");
            }
        }

        public void decode_Gas()
        {
            osae.AddToLog("Gas Not implemented", false);
        }

        public void decode_Water()
        {
            osae.AddToLog("Water Not implemented", false);
        }

        public void decode_Weight()
        {
            OSAEObject obj = osae.GetObjectByAddress((recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString());
            if (obj == null && osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                osae.AddToLog("New Scale found.  Adding to OSA", true);
                osae.ObjectAdd("Scale Meter - " + (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString(), "Scale Meter", "SCALE", (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString(), "", true);
                obj = obj = osae.GetObjectByAddress((recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString());
            }
            
            switch (recbuf[(byte)WEIGHT.subtype])
            {
                case (byte)WEIGHT.WEIGHT1:
                    osae.AddToLog("subtype       = BWR102", false);
                    break;
                case (byte)WEIGHT.WEIGHT2:
                    osae.AddToLog("subtype       = GR101", false);
                    break;
                default:
                    osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)WEIGHT.packettype] + ":" + recbuf[(byte)WEIGHT.subtype], false);
                    break;
            }
            osae.AddToLog("Sequence nbr  = " + recbuf[(byte)WEIGHT.seqnbr].ToString(), false);
            osae.AddToLog("ID            = " + (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString(), false);
            osae.AddToLog("Weight        = " + (((recbuf[(byte)WEIGHT.weighthigh] * 25.6) + recbuf[(byte)WEIGHT.weightlow] / 10).ToString() + 2.2).ToString() + " lb", false);
            osae.AddToLog("Signal level  = " + (recbuf[(byte)WEIGHT.rssi] >> 4).ToString(), false);

            osae.ObjectPropertySet(obj.Name, "Weight", (((recbuf[(byte)WEIGHT.weighthigh] * 25.6) + recbuf[(byte)WEIGHT.weightlow] / 10).ToString() + 2.2).ToString());
        }

        //public void decode_RFXSensor()
        //{
        //    switch (recbuf(RFXSENSOR.subtype))
        //    {
        //        case RFXSENSOR.Temp:
        //            osae.AddToLog("subtype       = Temperature");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            //positive temperature?
        //            if ((recbuf(RFXSENSOR.msg1) & 0x80) == 0)
        //            {
        //                osae.AddToLog("msg           = " + Math.Round(((recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)) / 100), 2).ToString() + " °C");
        //            }
        //            else
        //            {
        //                osae.AddToLog("msg           = " + Math.Round((0 - ((recbuf(RFXSENSOR.msg1) & 0x7f) * 256 + recbuf(RFXSENSOR.msg2)) / 100), 2).ToString() + " °C");
        //            }
        //            break;
        //        case RFXSENSOR.AD:
        //            osae.AddToLog("subtype       = A/D");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            osae.AddToLog("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString() + " mV");
        //            break;
        //        case RFXSENSOR.Volt:
        //            osae.AddToLog("subtype       = Voltage");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            osae.AddToLog("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString() + " mV");
        //            break;
        //        case RFXSENSOR.Message:
        //            osae.AddToLog("subtype       = Message");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            switch (recbuf(RFXSENSOR.msg2))
        //            {
        //                case 0x1:
        //                    osae.AddToLog("msg           = sensor addresses incremented");
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("msg           = battery low detected");
        //                    break;
        //                case 0x81:
        //                    osae.AddToLog("msg           = no 1-wire device connected");
        //                    break;
        //                case 0x82:
        //                    osae.AddToLog("msg           = 1-Wire ROM CRC error");
        //                    break;
        //                case 0x83:
        //                    osae.AddToLog("msg           = 1-Wire device connected is not a DS18B20 or DS2438");
        //                    break;
        //                case 0x84:
        //                    osae.AddToLog("msg           = no end of read signal received from 1-Wire device");
        //                    break;
        //                case 0x85:
        //                    osae.AddToLog("msg           = 1-Wire scratchpad CRC error");
        //                    break;
        //                default:
        //                    osae.AddToLog("ERROR: unknown message");
        //                    break;
        //            }

        //            osae.AddToLog("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString());
        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(RFXSENSOR.packettype)) + ":" + Conversion.Hex(recbuf(RFXSENSOR.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Signal level  = " + (recbuf(RFXSENSOR.rssi) >> 4).ToString());

        //}

        //public void decode_RFXMeter()
        //{
        //    long counter = 0;

        //    switch (recbuf(RFXMETER.subtype))
        //    {
        //        case RFXMETER.Count:
        //            osae.AddToLog("subtype       = RFXMeter counter");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            counter = (Convert.ToInt64(recbuf(RFXMETER.count1)) << 24) + (Convert.ToInt64(recbuf(RFXMETER.count2)) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4);
        //            osae.AddToLog("Counter       = " + counter.ToString());
        //            osae.AddToLog("if RFXPwr     = " + (counter / 1000).ToString() + " kWh");
        //            break;
        //        case RFXMETER.Interval:
        //            osae.AddToLog("subtype       = RFXMeter new interval time set");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            osae.AddToLog("Interval time = ", false);
        //            switch (recbuf(RFXMETER.count3))
        //            {
        //                case 0x1:
        //                    osae.AddToLog("30 seconds");
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("1 minute");
        //                    break;
        //                case 0x4:
        //                    osae.AddToLog("6 minutes");
        //                    break;
        //                case 0x8:
        //                    osae.AddToLog("12 minutes");
        //                    break;
        //                case 0x10:
        //                    osae.AddToLog("15 minutes");
        //                    break;
        //                case 0x20:
        //                    osae.AddToLog("30 minutes");
        //                    break;
        //                case 0x40:
        //                    osae.AddToLog("45 minutes");
        //                    break;
        //                case 0x80:
        //                    osae.AddToLog("1 hour");
        //                    break;
        //            }

        //            break;
        //        case RFXMETER.Calib:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("subtype       = Calibrate mode for channel 1");
        //                    break;
        //                case 0x40:
        //                    osae.AddToLog("subtype       = Calibrate mode for channel 2");
        //                    break;
        //                case 0x80:
        //                    osae.AddToLog("subtype       = Calibrate mode for channel 3");
        //                    break;
        //            }
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            counter = ((Convert.ToInt64(recbuf(RFXMETER.count2) & 0x3f) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)) / 1000;
        //            osae.AddToLog("Calibrate cnt = " + counter.ToString() + " msec");
        //            osae.AddToLog("RFXPwr        = " + Convert.ToString(Round(1 / ((16 * counter) / (3600000 / 62.5)), 3)) + " kW", false);
        //            break;
        //        case RFXMETER.Addr:
        //            osae.AddToLog("subtype       = New address set, push button for next address");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.CounterReset:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 1 will be executed");
        //                    break;
        //                case 0x40:
        //                    osae.AddToLog("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 2 will be executed");
        //                    break;
        //                case 0x80:
        //                    osae.AddToLog("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 3 will be executed");
        //                    break;
        //            }
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.CounterSet:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("subtype       = Counter channel 1 is reset to zero");
        //                    break;
        //                case 0x40:
        //                    osae.AddToLog("subtype       = Counter channel 2 is reset to zero");
        //                    break;
        //                case 0x80:
        //                    osae.AddToLog("subtype       = Counter channel 3 is reset to zero");
        //                    break;
        //            }
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            osae.AddToLog("Counter       = " + ((Convert.ToInt64(recbuf(RFXMETER.count1)) << 24) + (Convert.ToInt64(recbuf(RFXMETER.count2)) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)).ToString());

        //            break;
        //        case RFXMETER.SetInterval:
        //            osae.AddToLog("subtype       = Push the button for next mode within 5 seconds or else SET INTERVAL MODE will be entered");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.SetCalib:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 1 will be executed");
        //                    break;
        //                case 0x40:
        //                    osae.AddToLog("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 2 will be executed");
        //                    break;
        //                case 0x80:
        //                    osae.AddToLog("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 3 will be executed");
        //                    break;
        //            }
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.SetAddr:
        //            osae.AddToLog("subtype       = Push the button for next mode within 5 seconds or else SET ADDRESS MODE will be entered");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.Ident:
        //            osae.AddToLog("subtype       = RFXMeter identification");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            osae.AddToLog("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            osae.AddToLog("FW version    = " + Conversion.Hex(recbuf(RFXMETER.count3)));
        //            osae.AddToLog("Interval time = ", false);
        //            switch (recbuf(RFXMETER.count4))
        //            {
        //                case 0x1:
        //                    osae.AddToLog("30 seconds");
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("1 minute");
        //                    break;
        //                case 0x4:
        //                    osae.AddToLog("6 minutes");
        //                    break;
        //                case 0x8:
        //                    osae.AddToLog("12 minutes");
        //                    break;
        //                case 0x10:
        //                    osae.AddToLog("15 minutes");
        //                    break;
        //                case 0x20:
        //                    osae.AddToLog("30 minutes");
        //                    break;
        //                case 0x40:
        //                    osae.AddToLog("45 minutes");
        //                    break;
        //                case 0x80:
        //                    osae.AddToLog("1 hour");
        //                    break;
        //            }
        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(RFXMETER.packettype)) + ":" + Conversion.Hex(recbuf(RFXMETER.subtype)));
        //            break;
        //    }

        //    osae.AddToLog("Signal level  = " + (recbuf(RFXMETER.rssi) >> 4).ToString());
        //}

        //public void decode_FS20()
        //{
        //    switch (recbuf(FS20.subtype))
        //    {
        //        case FS20.sTypeFS20:
        //            osae.AddToLog("subtype       = FS20");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            osae.AddToLog("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            osae.AddToLog("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            osae.AddToLog("Cmd1          = ", false);
        //            switch ((recbuf(FS20.cmd1) & 0x1f))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("Off");
        //                    break;
        //                case 0x1:
        //                    osae.AddToLog("dim level 1 = 6.25%");
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("dim level 2 = 12.5%");
        //                    break;
        //                case 0x3:
        //                    osae.AddToLog("dim level 3 = 18.75%");
        //                    break;
        //                case 0x4:
        //                    osae.AddToLog("dim level 4 = 25%");
        //                    break;
        //                case 0x5:
        //                    osae.AddToLog("dim level 5 = 31.25%");
        //                    break;
        //                case 0x6:
        //                    osae.AddToLog("dim level 6 = 37.5%");
        //                    break;
        //                case 0x7:
        //                    osae.AddToLog("dim level 7 = 43.75%");
        //                    break;
        //                case 0x8:
        //                    osae.AddToLog("dim level 8 = 50%");
        //                    break;
        //                case 0x9:
        //                    osae.AddToLog("dim level 9 = 56.25%");
        //                    break;
        //                case 0xa:
        //                    osae.AddToLog("dim level 10 = 62.5%");
        //                    break;
        //                case 0xb:
        //                    osae.AddToLog("dim level 11 = 68.75%");
        //                    break;
        //                case 0xc:
        //                    osae.AddToLog("dim level 12 = 75%");
        //                    break;
        //                case 0xd:
        //                    osae.AddToLog("dim level 13 = 81.25%");
        //                    break;
        //                case 0xe:
        //                    osae.AddToLog("dim level 14 = 87.5%");
        //                    break;
        //                case 0xf:
        //                    osae.AddToLog("dim level 15 = 93.75%");
        //                    break;
        //                case 0x10:
        //                    osae.AddToLog("On (100%)");
        //                    break;
        //                case 0x11:
        //                    osae.AddToLog("On ( at last dim level set)");
        //                    break;
        //                case 0x12:
        //                    osae.AddToLog("Toggle between Off and On (last dim level set)");
        //                    break;
        //                case 0x13:
        //                    osae.AddToLog("Bright one step");
        //                    break;
        //                case 0x14:
        //                    osae.AddToLog("Dim one step");
        //                    break;
        //                case 0x15:
        //                    osae.AddToLog("Start dim cycle");
        //                    break;
        //                case 0x16:
        //                    osae.AddToLog("Program(Timer)");
        //                    break;
        //                case 0x17:
        //                    osae.AddToLog("Request status from a bidirectional device");
        //                    break;
        //                case 0x18:
        //                    osae.AddToLog("Off for timer period");
        //                    break;
        //                case 0x19:
        //                    osae.AddToLog("On (100%) for timer period");
        //                    break;
        //                case 0x1a:
        //                    osae.AddToLog("On ( at last dim level set) for timer period");
        //                    break;
        //                case 0x1b:
        //                    osae.AddToLog("Reset");
        //                    break;
        //                default:
        //                    osae.AddToLog("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                osae.AddToLog("                command to receiver");
        //            }
        //            else
        //            {
        //                osae.AddToLog("                response from receiver");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x40) == 0)
        //            {
        //                osae.AddToLog("                unidirectional command");
        //            }
        //            else
        //            {
        //                osae.AddToLog("                bidirectional command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x20) == 0)
        //            {
        //                osae.AddToLog("                additional cmd2 byte not present");
        //            }
        //            else
        //            {
        //                osae.AddToLog("                additional cmd2 byte present");
        //            }

        //            if ((recbuf(FS20.cmd1) & 0x20) != 0)
        //            {
        //                osae.AddToLog("Cmd2          = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2));
        //            }

        //            break;
        //        case FS20.sTypeFHT8V:
        //            osae.AddToLog("subtype       = FHT 8V valve");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            osae.AddToLog("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            osae.AddToLog("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            osae.AddToLog("Cmd1          = ", false);
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                osae.AddToLog("new command");
        //            }
        //            else
        //            {
        //                osae.AddToLog("repeated command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x40) == 0)
        //            {
        //                osae.AddToLog("                unidirectional command");
        //            }
        //            else
        //            {
        //                osae.AddToLog("                bidirectional command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x20) == 0)
        //            {
        //                osae.AddToLog("                additional cmd2 byte not present");
        //            }
        //            else
        //            {
        //                osae.AddToLog("                additional cmd2 byte present");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x10) == 0)
        //            {
        //                osae.AddToLog("                battery empty beep not enabled");
        //            }
        //            else
        //            {
        //                osae.AddToLog("                enable battery empty beep");
        //            }
        //            switch ((recbuf(FS20.cmd1) & 0xf))
        //            {
        //                case 0x0:
        //                    osae.AddToLog("                Synchronize now");
        //                    osae.AddToLog("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0x1:
        //                    osae.AddToLog("                open valve");
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("                close valve");
        //                    break;
        //                case 0x6:
        //                    osae.AddToLog("                open valve at percentage level");
        //                    osae.AddToLog("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0x8:
        //                    osae.AddToLog("                relative offset (cmd2 bit 7=direction, bit 5-0 offset value)");
        //                    break;
        //                case 0xa:
        //                    osae.AddToLog("                decalcification cycle");
        //                    osae.AddToLog("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0xc:
        //                    osae.AddToLog("                synchronization active");
        //                    osae.AddToLog("Cmd2          = count down is " + (recbuf(FS20.cmd2) >> 1).ToString() + " seconds");
        //                    break;
        //                case 0xe:
        //                    osae.AddToLog("                test, drive valve and produce an audible signal");
        //                    break;
        //                case 0xf:
        //                    osae.AddToLog("                pair valve (cmd2 bit 7-1 is count down in seconds, bit 0=1)");
        //                    osae.AddToLog("Cmd2          = count down is " + recbuf(FS20.cmd2) >> 1 + " seconds");
        //                    break;
        //                default:
        //                    osae.AddToLog("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }

        //            break;
        //        case FS20.sTypeFHT80:
        //            osae.AddToLog("subtype       = FHT80 door/window sensor");
        //            osae.AddToLog("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            osae.AddToLog("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            osae.AddToLog("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            osae.AddToLog("Cmd1          = ", false);
        //            switch ((recbuf(FS20.cmd1) & 0xf))
        //            {
        //                case 0x1:
        //                    osae.AddToLog("sensor opened");
        //                    break;
        //                case 0x2:
        //                    osae.AddToLog("sensor closed");
        //                    break;
        //                case 0xc:
        //                    osae.AddToLog("synchronization active");
        //                    break;
        //                default:
        //                    osae.AddToLog("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                osae.AddToLog("                new command");
        //            }
        //            else
        //            {
        //                osae.AddToLog("                repeated command");
        //            }

        //            break;
        //        default:
        //            osae.AddToLog("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(FS20.packettype)) + ":" + Conversion.Hex(recbuf(FS20.subtype)));
        //            break;
        //    }
        //    osae.AddToLog("Signal level  = " + (recbuf(FS20.rssi) >> 4).ToString());
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
