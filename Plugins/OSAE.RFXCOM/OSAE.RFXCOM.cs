namespace OSAE.RFXCOM
{
    using System;
    using System.IO.Ports;

    public class RFXCOM : OSAEPluginBase
    {
        private OSAE.General.OSAELog Log;
        private System.Timers.Timer tmrRead = new System.Timers.Timer(100);
        private string rcvdStr = "";
        private string pName;
        private SerialPort RS232Port = new SerialPort();
        private bool gRecComPortEnabled = false;
        private int Resettimer = 0;
        private int trxType = 0;
        private byte[] recbuf = new byte[40];
        private byte recbytes;
        private byte bytSeqNbr = 0;
        private byte bytFWversion;
        private bool tcp;
        private byte maxticks = 0;
        private byte[] TCPData = new byte[1025];
        public override void ProcessCommand(OSAEMethod method)
        {
            Log.Debug("--------------Processing Command---------------");
            Log.Debug("Command: " + method.MethodName);

            OSAEObject obj = OSAEObjectManager.GetObjectByName(method.ObjectName);
            Log.Debug("Object Name: " + obj.Name);
            Log.Debug("Object Type: " + obj.Type);
            Log.Debug("Object Adress: " + obj.Address);

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
                        Log.Debug("Executing Lighting1 command");
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
                                OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", pName);
                                break;
                            case "ON":
                                kar[(byte)LIGHTING1.cmnd] = (byte)LIGHTING1.sOn;
                                OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", pName);
                                break;
                            case "ALL OFF":
                                kar[(byte)LIGHTING1.cmnd] = (byte)LIGHTING1.sAllOff;
                                OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", pName);
                                break;
                            case "ALL ON":
                                kar[(byte)LIGHTING1.cmnd] = (byte)LIGHTING1.sAllOn;
                                OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", pName);
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
                            command += ("0" + bt.ToString()).Substring(("0" + bt.ToString()).Length - 2) + " ";

                        Log.Debug("Lighting1 command:" + command);

                        break;
                    #endregion

                    #region Lighting 2

                    case "AC DIMMER SWITCH":
                    case "HEU DIMMER SWITCH":
                    case "ANSLUT DIMMER SWITCH":
                        Log.Debug("Executing Lighting2 command");
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
                                OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", pName);
                                break;
                            case "ON":
                                if (method.Parameter1 != "")
                                {
                                    kar[(byte)LIGHTING2.cmnd] = (byte)LIGHTING2.sOn;
                                    kar[(byte)LIGHTING2.level] = (byte)0;
                                }
                                else
                                    kar[(byte)LIGHTING2.cmnd] = (byte)LIGHTING2.sOn;
                                    //kar[(byte)LIGHTING2.level] = (byte)Math.Round((double)Int32.Parse(method.Parameter1) / 7, 0);
                                OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", pName);
                                break;
                        }

                        kar[(byte)LIGHTING2.filler] = 0;

                        Log.Debug("Writing command. len: " + kar.Length.ToString());
                        WriteCom(kar);
                        string command_l2 = "";
                        foreach (byte bt in kar)
                            command_l2 += ("0" + bt.ToString("X")).Substring(("0" + bt.ToString("X")).Length - 2) + " ";

                        Log.Debug("Lighting2 command:" + command_l2);
                        break;

                    #endregion

                    #region Lighting 5

                    case "LIGHTWAVERF DIMMER SWITCH":
                    case "LIGHTWAVERF BINARY SWITCH":
                    case "EMW100 BINARY SWITCH":
                        Log.Debug("Executing Lighting5 command");

                        kar = new byte[(byte)LIGHTING5.size + 1];
                        Log.Debug("Lighting 5 device");

                        if (bytFWversion < 29)
                        {
                            Log.Error("RFXtrx433 firmware version must be > 28, flash your RFXtrx433 with the latest firmware");
                            return;
                        }

                        string[] l5_addr = obj.Address.Split('-');
                        if (l5_addr.Length != 4)
                        {
                            Log.Error("invalid unit address");
                            break;
                        }
                        else
                        {
                            byte subtype = (byte)0;
                            if (obj.Type == "LIGHTWAVERF DIMMER SWITCH" || obj.Type == "LIGHTWAVERF BINARY SWITCH")
                                subtype = (byte)0;
                            else if (obj.Type == "EMW100 BINARY SWITCH")
                                subtype = (byte)1;

                            kar[(byte)LIGHTING5.packetlength] = GetByte(LIGHTING5.size.ToString("X"));
                            Log.Debug("kar[(byte)LIGHTING5.packetlength]: " + kar[(byte)LIGHTING5.packetlength].ToString());
                            kar[(byte)LIGHTING5.packettype] = GetByte(LIGHTING5.pType.ToString("X"));
                            Log.Debug("kar[(byte)LIGHTING5.packettype]: " + kar[(byte)LIGHTING5.packettype].ToString());
                            kar[(byte)LIGHTING5.subtype] = subtype;
                            Log.Debug("kar[(byte)LIGHTING5.subtype]: " + subtype.ToString("X"));
                            kar[(byte)LIGHTING5.seqnbr] = bytSeqNbr;
                            Log.Debug("kar[(byte)LIGHTING5.seqnbr]: " + bytSeqNbr.ToString("X"));
                            kar[(byte)LIGHTING5.id1] = GetByte(l5_addr[0]);
                            Log.Debug("kar[(byte)LIGHTING5.id1]: " + l5_addr[0]);
                            kar[(byte)LIGHTING5.id2] = GetByte(l5_addr[1]);
                            Log.Debug("kar[(byte)LIGHTING5.id2]: " + l5_addr[1]);
                            kar[(byte)LIGHTING5.id3] = GetByte(l5_addr[2]);
                            Log.Debug("kar[(byte)LIGHTING5.id3]: " + l5_addr[2]);
                            kar[(byte)LIGHTING5.unitcode] = GetByte(l5_addr[3]);
                            Log.Debug("kar[(byte)LIGHTING5.unitcode]: " + l5_addr[3]);

                            switch (method.MethodName)
                            {
                                case "OFF":
                                    kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sOff;
                                    Log.Debug("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString());
                                    OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF",pName);
                                    break;
                                case "ON":
                                    if (method.Parameter1 == "")
                                    {
                                        kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sOn;
                                        Log.Debug("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString());
                                        kar[(byte)LIGHTING5.level] = 0;
                                        Log.Debug("kar[(byte)LIGHTING5.level]: " + kar[(byte)LIGHTING5.level].ToString());
                                    }
                                    else
                                    {
                                        kar[(byte)LIGHTING5.cmnd] = (byte)LIGHTING5.sSetLevel;
                                        Log.Debug("kar[(byte)LIGHTING5.cmnd]: " + kar[(byte)LIGHTING5.cmnd].ToString());
                                        kar[(byte)LIGHTING5.level] = (byte)Math.Round((double)Int32.Parse(method.Parameter1) / 3, 0);
                                        Log.Debug("kar[(byte)LIGHTING5.level]: " + kar[(byte)LIGHTING5.level].ToString());
                                    }
                                    OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON",pName);
                                    break;
                            }

                            kar[(byte)LIGHTING5.filler] = 0;
                            Log.Debug("kar[(byte)LIGHTING5.filler]: " + kar[(byte)LIGHTING5.filler].ToString());

                            //not used commands
                            if (kar[(byte)LIGHTING5.cmnd] == 8 | kar[(byte)LIGHTING5.cmnd] == 9)
                            {
                                Log.Error("not used command");
                                return;
                            }

                            if (kar[(byte)LIGHTING5.id1] == 0 & kar[(byte)LIGHTING5.id2] == 0 & kar[(byte)LIGHTING5.id3] == 0)
                            {
                                Log.Error("invalid unit address");
                                return;
                            }
                            Log.Info("Writing command to port");
                            WriteCom(kar);
                            string command_l5 = BitConverter.ToString(kar).Replace('-', ' ');
                            //foreach (byte bt in kar)
                            //{
                            //    command_l5 += ("0" + bt.ToString()).Substring(("0" + bt.ToString()).Length - 2) + " ";

                            //    command_l5 += BitConverter.ToString(bt);
                            //}
                            Log.Info("Lighting5 command:" + command_l5);
                        }
                        break;
                    #endregion
                }
            }
            catch (Exception ex)
            { Log.Error("Error processing command", ex); }
            Log.Debug("-----------------------------------------------");
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            Log = new General.OSAELog(pName);
           
            RSInit("COM" + OSAEObjectPropertyManager.GetObjectPropertyValue(pluginName,"Port").Value, 38400);
            if(RSOpen()) gRecComPortEnabled = true;

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
                    return false;
            }
            catch (Exception ex)
            { return false; }
        }

        public bool RSInit(string comport, int baudrate)
        {
            try
            {
                if (RS232Port.IsOpen) RS232Port.Close();

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
            catch (Exception Ex)
            { return false; }
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

            Log.Debug("================================================");
            Log.Debug(message);
            foreach (byte bt in kar)
                msgStr += ("0" + bt.ToString()).Substring(("0" + bt.ToString()).Length - 2, 2) + " ";

            Log.Debug(msgStr);
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
                { Log.Error("Unable to write to COM port"); }
            }
        }

        public bool RSTxData(byte[] buffer, int intLength)
        {
            try
            {
                RS232Port.Write(buffer, 0, intLength);
            }
            catch
            { return false; }
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
                        Log.Info(" Buffer flushed due to timeout");
                    }
                }
            }
            else
            {
                Resettimer = Resettimer - 1;
                // decrement resettimer
                if (Resettimer == 0)
                {
                    if (gRecComPortEnabled) RSDiscardInBuffer();
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
            catch
            { return false; }
        }

        public int RSBytesToRead()
        {
            try
            {
                if (RS232Port.IsOpen)
                    return RS232Port.BytesToRead;
                else
                    return 0;
            }
            catch
            { return -1; }
        }

        public int RSReadByte()
        {
            try
            {
                return RS232Port.ReadByte();
            }
            catch (TimeoutException pEx)
            { return -2; }
            catch (Exception pEx)
            { return -1; }
        }

        public void ProcessReceivedChar(byte sComChar)
        {
            
            if (Resettimer != 0) return;
                //ignore received characters after a reset cmd until resettimer = 0

            maxticks = 0;
            //reset receive timeout

            //1st char of a packet received
            if (recbytes == 0)
            {
                Log.Debug("------------------------------------------------");
                if (sComChar == 0) return;
                    //ignore 1st char if 00
            }

            recbuf[recbytes] = sComChar;
            //store received char
            recbytes += 1;
            //increment char counter

            //all bytes of the packet received?
            if (recbytes > recbuf[0])
            {
                rcvdStr += " " + ("0" + sComChar.ToString()).Substring(("0" + sComChar.ToString()).Length - 2, 2);
                Log.Debug(rcvdStr);
                rcvdStr = "";
                // Write the output to the screen.
                decode_messages();
                //decode message
                recbytes = 0;
                //set to zero to receive next message
            }
            else
                rcvdStr += " " + ("0" + sComChar.ToString()).Substring(("0" + sComChar.ToString()).Length - 2, 2);
                // Write the output to the screen.
        }

        #region "Decode messages"
        public void decode_messages()
        {
            Log.Debug("---------------Received Message----------------");
            switch (recbuf[1])
            {
                case (byte)IRESPONSE.pType:
                    Log.Debug("Packettype        = Interface Message");
                    decode_InterfaceMessage();
                    break;
                case (byte)RXRESPONSE.pType:
                    Log.Debug("Packettype        = Receiver/Transmitter Message");
                    decode_RecXmitMessage();
                    break;
                case (byte)UNDECODED.pType:
                    Log.Debug("Packettype        = UNDECODED RF Message");
                    decode_UNDECODED();
                    break;
                //case (byte)LIGHTING1.pType:
                //    Log.Debug("Packettype    = Lighting1");
                //    decode_Lighting1();

                //    break;
                case (byte)LIGHTING2.pType:
                    Log.Debug("Packettype    = Lighting2");
                    decode_Lighting2();
                    break;
                //case (byte)LIGHTING3.pType:
                //    Log.Debug("Packettype    = Lighting3");
                //    decode_Lighting3();

                //    break;
                //case (byte)LIGHTING4.pType:
                //    Log.Debug("Packettype    = Lighting4");
                //    decode_Lighting4();

                //    break;
                case (byte)LIGHTING5.pType:
                    Log.Debug("Packettype    = Lighting5");
                    decode_Lighting5();
                    break;
                //case (byte)SECURITY1.pType:
                //    Log.Debug("Packettype    = Security1");
                //    decode_Security1();

                //    break;
                //case (byte)CAMERA1.pType:
                //    Log.Debug("Packettype    = Camera1");
                //    decode_Camera1();

                //    break;
                //case (byte)REMOTE.pType:
                //    Log.Debug("Packettype    = Remote control & IR");
                //    decode_Remote();

                //    break;
                //case (byte)THERMOSTAT1.pType:
                //    Log.Debug("Packettype    = Thermostat1");
                //    decode_Thermostat1();

                //    break;
                //case (byte)THERMOSTAT2.pType:
                //    Log.Debug("Packettype    = Thermostat2");
                //    decode_Thermostat2();

                //    break;
                //case (byte)THERMOSTAT3.pType:
                //    Log.Debug("Packettype    = Thermostat3");
                //    decode_Thermostat3();

                //    break;
                case (byte)TEMP.pType:
                    Log.Debug("Packettype    = TEMP");
                    decode_Temp();
                    break;
                case (byte)HUM.pType:
                    Log.Debug("Packettype    = HUM");
                    decode_Hum();
                    break;
                case (byte)TEMP_HUM.pType:
                    Log.Debug("Packettype    = TEMP_HUM");
                    decode_TempHum();
                    break;
                case (byte)BARO.pType:
                    Log.Debug("Packettype    = BARO");
                    decode_Baro();
                    break;
                case (byte)TEMP_HUM_BARO.pType:
                    Log.Debug("Packettype    = TEMP_HUM_BARO");
                    decode_TempHumBaro();
                    break;
                case (byte)RAIN.pType:
                    Log.Debug("Packettype    = RAIN");
                    decode_Rain();
                    break;
                case (byte)WIND.pType:
                    Log.Debug("Packettype    = WIND");
                    decode_Wind();
                    break;
                case (byte)UV.pType:
                    Log.Debug("Packettype    = UV");
                    decode_UV();
                    break;
                case (byte)DT.pType:
                    Log.Debug("Packettype    = DT");
                    decode_DateTime();
                    break;
                case (byte)CURRENT.pType:
                    Log.Debug("Packettype    = CURRENT");
                    decode_Current();
                    break;
                case (byte)ENERGY.pType:
                    Log.Debug("Packettype    = ENERGY");
                    decode_Energy();
                    break;
                case (byte)GAS.pType:
                    Log.Debug("Packettype    = GAS");
                    decode_Gas();
                    break;
                case (byte)WATER.pType:
                    Log.Debug("Packettype    = WATER");
                    decode_Water();
                    break;
                case (byte)WEIGHT.pType:
                    Log.Debug("Packettype    = WEIGHT");
                    decode_Weight();
                    break;
                //case (byte)RFXSENSOR.pType:
                //    Log.Debug("Packettype    = RFXSensor");
                //    decode_RFXSensor();

                //    break;
                //case (byte)RFXMETER.pType:
                //    Log.Debug("Packettype    = RFXMeter");
                //    decode_RFXMeter();

                //    break;
                //case (byte)FS20.pType:
                //    Log.Debug("Packettype    = FS20");
                //    decode_FS20();

                //    break;
                default:
                    Log.Error("ERROR: Unknown Packet type:" + recbuf[1].ToString());
                    break;
            }
            Log.Debug("-----------------------------------------------");
        }

        public void decode_InterfaceMessage()
        {
            switch (recbuf[(byte)IRESPONSE.subtype])
            {
                case (byte)IRESPONSE.sType:
                    Log.Debug("subtype           = Interface Response");
                    Log.Debug("Sequence nbr      = " + recbuf[(byte)IRESPONSE.seqnbr].ToString());
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
                            Log.Debug("response on cmnd  = ");
                            switch (recbuf[(byte)IRESPONSE.cmnd])
                            {
                                case (byte)ICMD.STATUS:
                                    Log.Debug("Get Status");
                                    break;
                                case (byte)ICMD.SETMODE:
                                    Log.Debug("Set Mode");
                                    break;
                                case (byte)ICMD.sel310:
                                    Log.Debug("Select 310MHz");
                                    break;
                                case (byte)ICMD.sel315:
                                    Log.Debug("Select 315MHz");
                                    break;
                                case (byte)ICMD.sel800:
                                    Log.Debug("Select 868.00MHz");
                                    break;
                                case (byte)ICMD.sel800F:
                                    Log.Debug("Select 868.00MHz FSK");
                                    break;
                                case (byte)ICMD.sel830:
                                    Log.Debug("Select 868.30MHz");
                                    break;
                                case (byte)ICMD.sel830F:
                                    Log.Debug("Select 868.30MHz FSK");
                                    break;
                                case (byte)ICMD.sel835:
                                    Log.Debug("Select 868.35MHz");
                                    break;
                                case (byte)ICMD.sel835F:
                                    Log.Debug("Select 868.35MHz FSK");
                                    break;
                                case (byte)ICMD.sel895:
                                    Log.Debug("Select 868.95MHz");
                                    break;
                                default:
                                    Log.Debug("Error: unknown response");
                                    break;
                            }
                            switch (recbuf[(byte)IRESPONSE.msg1])
                            {
                                case (byte)IRESPONSE.recType310:
                                    Log.Debug("Transceiver type  = 310MHz");
                                    break;
                                case (byte)IRESPONSE.recType315:
                                    Log.Debug("Receiver type     = 315MHz");
                                    break;
                                case (byte)IRESPONSE.recType43392:
                                    Log.Debug("Receiver type     = 433.92MHz (receive only)");
                                    break;
                                case (byte)IRESPONSE.trxType43392:
                                    Log.Debug("Transceiver type  = 433.92MHz");
                                    break;
                                case (byte)IRESPONSE.recType86800:
                                    Log.Debug("Receiver type     = 868.00MHz");
                                    break;
                                case (byte)IRESPONSE.recType86800FSK:
                                    Log.Debug("Receiver type     = 868.00MHz FSK");
                                    break;
                                case (byte)IRESPONSE.recType86830:
                                    Log.Debug("Receiver type     = 868.30MHz");
                                    break;
                                case (byte)IRESPONSE.recType86830FSK:
                                    Log.Debug("Receiver type     = 868.30MHz FSK");
                                    break;
                                case (byte)IRESPONSE.recType86835:
                                    Log.Debug("Receiver type     = 868.35MHz");
                                    break;
                                case (byte)IRESPONSE.recType86835FSK:
                                    Log.Debug("Receiver type     = 868.35MHz FSK");
                                    break;
                                case (byte)IRESPONSE.recType86895:
                                    Log.Debug("Receiver type     = 868.95MHz");
                                    break;
                                default:
                                    Log.Debug("Receiver type     = unknown");
                                    break;
                            }
                            trxType = recbuf[(byte)IRESPONSE.msg1];
                            Log.Debug("Firmware version  = " + recbuf[(byte)IRESPONSE.msg2].ToString());
                            bytFWversion = recbuf[(byte)IRESPONSE.msg2];

                            if ((recbuf[(byte)IRESPONSE.msg3] & (byte)IRESPONSE.msg3_undec) != 0)
                                Log.Debug("Undec             on");
                            else
                                Log.Debug("Undec             off");

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_X10) != 0)
                                Log.Debug("X10               enabled");
                            else
                                Log.Debug("X10               disabled");

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_ARC) != 0)
                                Log.Debug("ARC               enabled");
                            else
                                Log.Debug("ARC               disabled");

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_AC) != 0)
                                Log.Debug("AC                enabled");
                            else
                                Log.Debug("AC                disabled");

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_HEU) != 0)
                                Log.Debug("HomeEasy EU       enabled");
                            else
                                Log.Debug("HomeEasy EU       disabled");

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_KOP) != 0)
                                Log.Debug("Ikea Koppla       enabled");
                            else
                                Log.Debug("Ikea Koppla       disabled");

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_OREGON) != 0)
                                Log.Debug("Oregon Scientific enabled");
                            else
                                Log.Debug("Oregon Scientific disabled");

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_ATI) != 0)
                                Log.Debug("ATI               enabled");
                            else
                                Log.Debug("ATI               disabled");

                            if ((recbuf[(byte)IRESPONSE.msg5] & (byte)IRESPONSE.msg5_VISONIC) != 0)
                                Log.Debug("Visonic           enabled");
                            else
                                Log.Debug("Visonic           disabled");

                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_MERTIK) != 0)
                                Log.Debug("Mertik            enabled");
                            else
                                Log.Debug("Mertik            disabled");

                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_AD) != 0)
                                Log.Debug("AD                enabled");
                            else
                                Log.Debug("AD                disabled");

                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_HID) != 0)
                                Log.Debug("Hideki            enabled");
                            else
                                Log.Debug("Hideki            disabled");

                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_LCROS) != 0)
                                Log.Debug("La Crosse         enabled");
                            else
                                Log.Debug("La Crosse         disabled");

                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_FS20) != 0)
                                Log.Debug("FS20              enabled");
                            else
                                Log.Debug("FS20              disabled");

                            if ((recbuf[(byte)IRESPONSE.msg4] & (byte)IRESPONSE.msg4_PROGUARD) != 0)
                                Log.Debug("ProGuard          enabled");
                            else
                                Log.Debug("ProGuard          disabled");

                            if ((recbuf[(byte)IRESPONSE.msg4] & 0x80) != 0)
                                Log.Debug("RFU protocol 7    enabled");
                            else
                                Log.Debug("RFU protocol 7    disabled");

                            break;
                        case (byte)ICMD.ENABLEALL:
                            Log.Debug("response on cmnd  = Enable All RF");
                            break;
                        case (byte)ICMD.UNDECODED:
                            Log.Debug("response on cmnd  = UNDECODED on");
                            break;
                        case (byte)ICMD.SAVE:
                            Log.Debug("response on cmnd  = Save");
                            break;
                        case (byte)ICMD.DISX10:
                            Log.Debug("response on cmnd  = Disable X10 RF");
                            break;
                        case (byte)ICMD.DISARC:
                            Log.Debug("response on cmnd  = Disable ARC RF");
                            break;
                        case (byte)ICMD.DISAC:
                            Log.Debug("response on cmnd  = Disable AC RF");
                            break;
                        case (byte)ICMD.DISHEU:
                            Log.Debug("response on cmnd  = Disable HomeEasy EU RF");
                            break;
                        case (byte)ICMD.DISKOP:
                            Log.Debug("response on cmnd  = Disable Ikea Koppla RF");
                            break;
                        case (byte)ICMD.DISOREGON:
                            Log.Debug("response on cmnd  = Disable Oregon Scientific RF");
                            break;
                        case (byte)ICMD.DISATI:
                            Log.Debug("response on cmnd  = Disable ATI remote RF");
                            break;
                        case (byte)ICMD.DISVISONIC:
                            Log.Debug("response on cmnd  = Disable Visonic RF");
                            break;
                        case (byte)ICMD.DISMERTIK:
                            Log.Debug("response on cmnd  = Disable Mertik RF");
                            break;
                        case (byte)ICMD.DISAD:
                            Log.Debug("response on cmnd  = Disable AD RF");
                            break;
                        case (byte)ICMD.DISHID:
                            Log.Debug("response on cmnd  = Disable Hideki RF");
                            break;
                        case (byte)ICMD.DISLCROS:
                            Log.Debug("response on cmnd  = Disable La Crosse RF");

                            break;
                        //For internal use by RFXCOM only, do not use this coding.
                        //=========================================================
                        case 0x8:
                            Log.Debug("response on cmnd  = T1");
                            if (recbuf[(byte)IRESPONSE.msg9] == 0)
                                Log.Debug("Not OK!");
                            else
                                Log.Debug("On");
                            break;
                        case 0x9:
                            Log.Debug("response on cmnd  = T2");
                            if (recbuf[(byte)IRESPONSE.msg9] == 0)
                                Log.Debug("Not OK!");
                            else
                                Log.Debug("Blk On");
                            break;
                        //=========================================================

                        default:
                            Log.Error("ERROR: Unexpected response for Packet type=" + recbuf[(byte)IRESPONSE.packettype].ToString() + ", Sub type=" + recbuf[(byte)IRESPONSE.subtype].ToString() + " cmnd=" + recbuf[(byte)IRESPONSE.cmnd].ToString());
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
                    Log.Debug("subtype           = Receiver lock error");
                    //SystemSounds.Asterisk.Play();
                    Log.Debug("Sequence nbr      = " + recbuf[(byte)RXRESPONSE.seqnbr].ToString());

                    break;
                case (byte)RXRESPONSE.sTypeTransmitterResponse:
                    Log.Debug("subtype           = Transmitter Response");
                    Log.Debug("Sequence nbr      = " + recbuf[(byte)RXRESPONSE.seqnbr].ToString());
                    switch (recbuf[(byte)RXRESPONSE.msg])
                    {
                        case 0x0:
                            Log.Debug("response          = ACK, data correct transmitted");
                            break;
                        case 0x1:
                            Log.Debug("response          = ACK, but transmit started after 6 seconds delay anyway with RF receive data detected");
                            break;
                        case 0x2:
                            Log.Debug("response          = NAK, transmitter did not lock on the requested transmit frequency");
                            //SystemSounds.Asterisk.Play();
                            break;
                        case 0x3:
                            Log.Debug("response          = NAK, AC address zero in id1-id4 not allowed");
                            //SystemSounds.Asterisk.Play();
                            break;
                        default:
                            Log.Debug("ERROR: Unexpected message type=" + recbuf[(byte)RXRESPONSE.msg].ToString());
                            break;
                    }
                    break;
                default:
                    Log.Error("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)RXRESPONSE.packettype].ToString() + ": " + recbuf[(byte)RXRESPONSE.subtype].ToString());
                    break;
            }
        }

        public void decode_UNDECODED()
        {
            Log.Debug("UNDECODED ");
            switch (recbuf[(byte)UNDECODED.subtype])
            {
                case (byte)UNDECODED.sTypeUac:
                    Log.Debug("AC:");
                    break;
                case (byte)UNDECODED.sTypeUarc:
                    Log.Debug("ARC:");
                    break;
                case (byte)UNDECODED.sTypeUati:
                    Log.Debug("ATI:");
                    break;
                case (byte)UNDECODED.sTypeUhideki:
                    Log.Debug("HIDEKI:");
                    break;
                case (byte)UNDECODED.sTypeUlacrosse:
                    Log.Debug("LACROSSE:");
                    break;
                case (byte)UNDECODED.sTypeUlwrf:
                    Log.Debug("LWRF:");
                    break;
                case (byte)UNDECODED.sTypeUmertik:
                    Log.Debug("MERTIK:");
                    break;
                case (byte)UNDECODED.sTypeUoregon1:
                    Log.Debug("OREGON1:");
                    break;
                case (byte)UNDECODED.sTypeUoregon2:
                    Log.Debug("OREGON2:");
                    break;
                case (byte)UNDECODED.sTypeUoregon3:
                    Log.Debug("OREGON3:");
                    break;
                case (byte)UNDECODED.sTypeUproguard:
                    Log.Debug("PROGUARD:");
                    break;
                case (byte)UNDECODED.sTypeUvisonic:
                    Log.Debug("VISONIC:");
                    break;
                case (byte)UNDECODED.sTypeUnec:
                    Log.Debug("NEC:");
                    break;
                case (byte)UNDECODED.sTypeUfs20:
                    Log.Debug("FS20:");
                    break;
                default:
                    Log.Error("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)UNDECODED.packettype] + ": " + recbuf[(byte)UNDECODED.subtype]);
                    break;
            }
            for (int i = 0; i <= recbuf[(byte)UNDECODED.packetlength] - (byte)UNDECODED.msg1; i++)
                Log.Debug("0" + recbuf[(byte)UNDECODED.msg1 + i]);

        }

        //public void decode_Lighting1()
        //{
        //    switch (recbuf(LIGHTING1.subtype))
        //    {
        //        case LIGHTING1.sTypeX10:
        //            Log.Debug("subtype       = X10");
        //            Log.Debug("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            Log.Debug("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            Log.Debug("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            Log.Debug("Command       = ");
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    Log.Debug("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    Log.Debug("On");
        //                    break;
        //                case LIGHTING1.sDim:
        //                    Log.Debug("Dim");
        //                    break;
        //                case LIGHTING1.sBright:
        //                    Log.Debug("Bright");
        //                    break;
        //                case LIGHTING1.sAllOn:
        //                    Log.Debug("All On");
        //                    break;
        //                case LIGHTING1.sAllOff:
        //                    Log.Debug("All Off");
        //                    break;
        //                case LIGHTING1.sChime:
        //                    Log.Debug("Chime");
        //                    break;
        //                default:
        //                    Log.Debug("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        case LIGHTING1.sTypeARC:
        //            Log.Debug("subtype       = ARC");
        //            Log.Debug("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            Log.Debug("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            Log.Debug("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            Log.Debug("Command       = ");
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    Log.Debug("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    Log.Debug("On");
        //                    break;
        //                case LIGHTING1.sAllOn:
        //                    Log.Debug("All On");
        //                    break;
        //                case LIGHTING1.sAllOff:
        //                    Log.Debug("All Off");
        //                    break;
        //                default:
        //                    Log.Debug("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        case LIGHTING1.sTypeAB400D:
        //            Log.Debug("subtype       = ELRO AB400");
        //            Log.Debug("Sequence nbr  = " + recbuf(LIGHTING1.seqnbr).ToString);
        //            Log.Debug("housecode     = " + Strings.Chr(recbuf(LIGHTING1.housecode)));
        //            Log.Debug("unitcode      = " + recbuf(LIGHTING1.unitcode).ToString);
        //            Log.Debug("Command       = ");
        //            switch (recbuf(LIGHTING1.cmnd))
        //            {
        //                case LIGHTING1.sOff:
        //                    Log.Debug("Off");
        //                    break;
        //                case LIGHTING1.sOn:
        //                    Log.Debug("On");
        //                    break;
        //                default:
        //                    Log.Debug("UNKNOWN");
        //                    break;
        //            }

        //            break;
        //        default:
        //            Log.Debug("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(LIGHTING1.packettype)) + ": " + Conversion.Hex(recbuf(LIGHTING1.subtype)));
        //            break;
        //    }
        //    Log.Debug("Signal level  = " + (recbuf(LIGHTING1.rssi) >> 4).ToString());
        //}

        public void decode_Lighting2()
        {
            Log.Debug("Recieved Lighting2 Message.  Type: " + recbuf[(byte)LIGHTING2.subtype].ToString());
            OSAEObject obj = new OSAEObject(); 
            
            switch (recbuf[(byte)LIGHTING2.subtype])
            {
                case (byte)LIGHTING2.sTypeAC:
                case (byte)LIGHTING2.sTypeHEU:
                case (byte)LIGHTING2.sTypeANSLUT:
                    //Log.Debug("id1: " + recbuf[(byte)LIGHTING2.id1].ToString());
                    //Log.Debug("id2: " + recbuf[(byte)LIGHTING2.id2].ToString());
                    //Log.Debug("id3: " + recbuf[(byte)LIGHTING2.id3].ToString());
                    //Log.Debug("id4: " + recbuf[(byte)LIGHTING2.id4].ToString());
                    //Log.Debug("uc: " + recbuf[(byte)LIGHTING2.unitcode].ToString(), true);

                    obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)LIGHTING2.id1].ToString("X") +
                        "-" + recbuf[(byte)LIGHTING2.id2].ToString("X") +
                        "-" + recbuf[(byte)LIGHTING2.id3].ToString("X") +
                        "-" + recbuf[(byte)LIGHTING2.id4].ToString("X") +
                        "-" + recbuf[(byte)LIGHTING2.unitcode].ToString()));
                        
                    switch (recbuf[(byte)LIGHTING2.subtype])
                    {
                        case (byte)LIGHTING2.sTypeAC:
                            Log.Debug("subtype       = AC");
                            break;
                        case (byte)LIGHTING2.sTypeHEU:
                            Log.Debug("subtype       = HomeEasy EU");
                            break;
                        case (byte)LIGHTING2.sTypeANSLUT:
                            Log.Debug("subtype       = ANSLUT");
                            break;
                    }
                    Log.Debug("Sequence nbr  = " + recbuf[(byte)LIGHTING2.seqnbr].ToString());
                    //Log.Debug("ID - Unit            = " + address);
                    Log.Debug("Unit          = " + recbuf[(byte)LIGHTING2.unitcode].ToString());
                    switch (recbuf[(byte)LIGHTING2.cmnd])
                    {
                        case (byte)LIGHTING2.sOff:
                            Log.Debug("Command       = Off");
                            OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", pName);
                            break;
                        case (byte)LIGHTING2.sOn:
                            Log.Debug("Command       = On");
                            OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", pName);
                            break;
                        case (byte)LIGHTING2.sSetLevel:
                            Log.Debug("Set Level:" + recbuf[(byte)LIGHTING2.level].ToString());
                            break;
                        case (byte)LIGHTING2.sGroupOff:
                            Log.Debug("Group Off");
                            break;
                        case (byte)LIGHTING2.sGroupOn:
                            Log.Debug("Group On");
                            break;
                        case (byte)LIGHTING2.sSetGroupLevel:
                            Log.Debug("Set Group Level:" + recbuf[(byte)LIGHTING2.level].ToString());
                            break;
                        default:
                            Log.Debug("UNKNOWN");
                            break;
                    }
                    break;
                default:
                    Log.Error("ERROR: Unknown Sub type for Packet type=" + Convert.ToInt32(recbuf[(byte)LIGHTING2.packettype]) + ": " + Convert.ToInt32(recbuf[(byte)LIGHTING2.subtype]));
                    break;
            }
            Log.Debug("Signal level  = " + (recbuf[(byte)LIGHTING2.rssi] >> 4).ToString());
        }

        //public void decode_Lighting3()
        //{
        //    switch (recbuf(LIGHTING3.subtype))
        //    {
        //        case LIGHTING3.sTypeKoppla:
        //            Log.Debug("subtype       = Ikea Koppla");
        //            Log.Debug("Sequence nbr  = " + recbuf(LIGHTING3.seqnbr).ToString);
        //            Log.Debug("Command       = ");
        //            switch (recbuf(LIGHTING3.cmnd))
        //            {
        //                case 0x0:
        //                    Log.Debug("Off");
        //                    break;
        //                case 0x1:
        //                    Log.Debug("On");
        //                    break;
        //                case 0x20:
        //                    Log.Debug("Set Level:" + recbuf(6).ToString);
        //                    break;
        //                case 0x21:
        //                    Log.Debug("Program");
        //                    break;
        //                default:
        //                    if (recbuf(LIGHTING3.cmnd) >= 0x10 & recbuf(LIGHTING3.cmnd) < 0x18)
        //                    {
        //                        Log.Debug("Dim");
        //                    }
        //                    else if (recbuf(LIGHTING3.cmnd) >= 0x18 & recbuf(LIGHTING3.cmnd) < 0x20)
        //                    {
        //                        Log.Debug("Bright");
        //                    }
        //                    else
        //                    {
        //                        Log.Debug("UNKNOWN");
        //                    }
        //                    break;
        //            }
        //            break;
        //        default:
        //            Log.Debug("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(LIGHTING3.packettype)) + ": " + Conversion.Hex(recbuf(LIGHTING3.subtype)));
        //            break;
        //    }
        //    Log.Debug("Signal level  = " + (recbuf(LIGHTING3.rssi) >> 4).ToString());

        //}

        //public void decode_Lighting4()
        //{
        //    Log.Debug("Not implemented");
        //}

        public void decode_Lighting5()
        {
            Log.Debug("Recieved Lighting5 Message");
            OSAEObject obj = new OSAEObject();
            switch (recbuf[(byte)LIGHTING5.subtype])
            {
                case (byte)LIGHTING5.sTypeLightwaveRF:
                    obj = OSAEObjectManager.GetObjectByAddress("0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString() + "-0" + recbuf[(byte)LIGHTING5.id3].ToString() + "-" + recbuf[(byte)LIGHTING5.unitcode].ToString()); 
                    Log.Debug("subtype       = LightwaveRF");
                    Log.Debug("Sequence nbr  = " + recbuf[(byte)LIGHTING5.seqnbr].ToString());
                    Log.Debug("ID            = " + "0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2] + "-0" + recbuf[(byte)LIGHTING5.id3].ToString());
                    Log.Debug("Unit          = " + recbuf[(byte)LIGHTING5.unitcode].ToString());
                    switch (recbuf[(byte)LIGHTING5.cmnd])
                    {
                        case (byte)LIGHTING5.sOff:
                            OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", pName);
                            Log.Debug("Command       = Off");
                            break;
                        case (byte)LIGHTING5.sOn:
                            OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", pName);
                            Log.Debug("Command       = On");
                            break;
                        case (byte)LIGHTING5.sGroupOff:
                            Log.Debug("Command       = Group Off");
                            break;
                        case (byte)LIGHTING5.sMood1:
                            Log.Debug("Command       = Group Mood 1");
                            break;
                        case (byte)LIGHTING5.sMood2:
                            Log.Debug("Command       = Group Mood 2");
                            break;
                        case (byte)LIGHTING5.sMood3:
                            Log.Debug("Command       = Group Mood 3");
                            break;
                        case (byte)LIGHTING5.sMood4:
                            Log.Debug("Command       = Group Mood 4");
                            break;
                        case (byte)LIGHTING5.sMood5:
                            Log.Debug("Command       = Group Mood 5");
                            break;
                        case (byte)LIGHTING5.sUnlock:
                            Log.Debug("Command       = Unlock");
                            break;
                        case (byte)LIGHTING5.sLock:
                            Log.Debug("Command       = Lock");
                            break;
                        case (byte)LIGHTING5.sAllLock:
                            Log.Debug("Command       = All lock");
                            break;
                        case (byte)LIGHTING5.sClose:
                            Log.Debug("Command       = Close inline relay");
                            break;
                        case (byte)LIGHTING5.sStop:
                            Log.Debug("Command       = Stop inline relay");
                            break;
                        case (byte)LIGHTING5.sOpen:
                            Log.Debug("Command       = Open inline relay");
                            break;
                        case (byte)LIGHTING5.sSetLevel:
                            Log.Debug("Command       = Set dim level to: " + Convert.ToInt32((recbuf[(byte)LIGHTING5.level] * 3.2)).ToString() + "%");
                            break;
                        default:
                            Log.Debug("UNKNOWN");
                            break;
                    }
                    break;
                case (byte)LIGHTING5.sTypeEMW100:
                    obj = OSAEObjectManager.GetObjectByAddress("0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString() + "-" + recbuf[(byte)LIGHTING5.unitcode].ToString()); 
                    Log.Debug("subtype       = EMW100");
                    Log.Debug("Sequence nbr  = " + recbuf[(byte)LIGHTING5.seqnbr].ToString());
                    Log.Debug("ID            = " + "0" + recbuf[(byte)LIGHTING5.id1].ToString() + "-0" + recbuf[(byte)LIGHTING5.id2].ToString());
                    Log.Debug("Unit          = " + recbuf[(byte)LIGHTING5.unitcode].ToString());
                    switch (recbuf[(byte)LIGHTING5.cmnd])
                    {
                        case (byte)LIGHTING5.sOff:
                            OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", pName);
                            Log.Debug("Command       = Off");
                            break;
                        case (byte)LIGHTING5.sOn:
                            OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", pName);
                            Log.Debug("Command       = On");
                            break;
                        case (byte)LIGHTING5.sLearn:
                            Log.Debug("Command       = Learn");
                            break;
                        default:
                            Log.Debug("Command       = UNKNOWN");
                            break;
                    }

                    break;
                default:
                    Log.Error("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)LIGHTING5.packettype].ToString() + ": " + recbuf[(byte)LIGHTING5.subtype].ToString());
                    break;
            }
            Log.Debug("Signal level  = " + (recbuf[(byte)LIGHTING5.rssi] >> 4).ToString());
        }

        //public void decode_Security1()
        //{
        //    switch (recbuf(SECURITY1.subtype))
        //    {
        //        case SECURITY1.SecX10:
        //            Log.Debug("subtype       = X10 security");
        //            break;
        //        case SECURITY1.SecX10M:
        //            Log.Debug("subtype       = X10 security motion");
        //            break;
        //        case SECURITY1.SecX10R:
        //            Log.Debug("subtype       = X10 security remote");
        //            break;
        //        case SECURITY1.KD101:
        //            Log.Debug("subtype       = KD101 smoke detector");
        //            break;
        //        case SECURITY1.PowercodeSensor:
        //            Log.Debug("subtype       = Visonic PowerCode sensor - primary contact");
        //            break;
        //        case SECURITY1.PowercodeMotion:
        //            Log.Debug("subtype       = Visonic PowerCode motion");
        //            break;
        //        case SECURITY1.Codesecure:
        //            Log.Debug("subtype       = Visonic CodeSecure");
        //            break;
        //        case SECURITY1.PowercodeAux:
        //            Log.Debug("subtype       = Visonic PowerCode sensor - auxiliary contact");
        //            break;
        //        default:
        //            Log.Debug("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(SECURITY1.packettype)) + ": " + Conversion.Hex(recbuf(SECURITY1.subtype)));
        //            break;
        //    }
        //    Log.Debug("Sequence nbr  = " + recbuf(SECURITY1.seqnbr).ToString);
        //    Log.Debug("id1-3         = " + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id2)), 2) + VB.Right("0" + Conversion.Hex(recbuf(SECURITY1.id3)), 2));
        //    Log.Debug("status        = ");
        //    switch (recbuf(SECURITY1.status))
        //    {
        //        case SECURITY1.sStatusNormal:
        //            Log.Debug("Normal");
        //            break;
        //        case SECURITY1.sStatusNormalDelayed:
        //            Log.Debug("Normal Delayed");
        //            break;
        //        case SECURITY1.sStatusAlarm:
        //            Log.Debug("Alarm");
        //            break;
        //        case SECURITY1.sStatusAlarmDelayed:
        //            Log.Debug("Alarm Delayed");
        //            break;
        //        case SECURITY1.sStatusMotion:
        //            Log.Debug("Motion");
        //            break;
        //        case SECURITY1.sStatusNoMotion:
        //            Log.Debug("No Motion");
        //            break;
        //        case SECURITY1.sStatusPanic:
        //            Log.Debug("Panic");
        //            break;
        //        case SECURITY1.sStatusPanicOff:
        //            Log.Debug("Panic End");
        //            break;
        //        case SECURITY1.sStatusTamper:
        //            Log.Debug("Tamper");
        //            break;
        //        case SECURITY1.sStatusArmAway:
        //            Log.Debug("Arm Away");
        //            break;
        //        case SECURITY1.sStatusArmAwayDelayed:
        //            Log.Debug("Arm Away Delayed");
        //            break;
        //        case SECURITY1.sStatusArmHome:
        //            Log.Debug("Arm Home");
        //            break;
        //        case SECURITY1.sStatusArmHomeDelayed:
        //            Log.Debug("Arm Home Delayed");
        //            break;
        //        case SECURITY1.sStatusDisarm:
        //            Log.Debug("Disarm");
        //            break;
        //        case SECURITY1.sStatusLightOff:
        //            Log.Debug("Light Off");
        //            break;
        //        case SECURITY1.sStatusLightOn:
        //            Log.Debug("Light On");
        //            break;
        //        case SECURITY1.sStatusLIGHTING2Off:
        //            Log.Debug("Light 2 Off");
        //            break;
        //        case SECURITY1.sStatusLIGHTING2On:
        //            Log.Debug("Light 2 On");
        //            break;
        //        case SECURITY1.sStatusDark:
        //            Log.Debug("Dark detected");
        //            break;
        //        case SECURITY1.sStatusLight:
        //            Log.Debug("Light Detected");
        //            break;
        //        case SECURITY1.sStatusBatLow:
        //            Log.Debug("Battery low MS10 or XX18 sensor");
        //            break;
        //        case SECURITY1.sStatusPairKD101:
        //            Log.Debug("Pair KD101");
        //            break;
        //    }
        //    if ((recbuf(SECURITY1.battery_level) & 0xf) == 0)
        //    {
        //        Log.Debug("battery level = Low");
        //    }
        //    else
        //    {
        //        Log.Debug("battery level = OK");
        //    }
        //    Log.Debug("Signal level  = " + (recbuf(SECURITY1.rssi) >> 4).ToString());
        //}

        //public void decode_Camera1()
        //{
        //    switch (recbuf(CAMERA1.subtype))
        //    {
        //        case CAMERA1.Ninja:
        //            Log.Debug("subtype       = X10 Ninja/Robocam");
        //            Log.Debug("Sequence nbr  = " + recbuf(CAMERA1.seqnbr).ToString);
        //            Log.Debug("Command       = ");
        //            switch (recbuf(CAMERA1.cmnd))
        //            {
        //                case CAMERA1.sLeft:
        //                    Log.Debug("Left");
        //                    break;
        //                case CAMERA1.sRight:
        //                    Log.Debug("Right");
        //                    break;
        //                case CAMERA1.sUp:
        //                    Log.Debug("Up");
        //                    break;
        //                case CAMERA1.sDown:
        //                    Log.Debug("Down");
        //                    break;
        //                case CAMERA1.sPosition1:
        //                    Log.Debug("Position 1");
        //                    break;
        //                case CAMERA1.sProgramPosition1:
        //                    Log.Debug("Position 1 program");
        //                    break;
        //                case CAMERA1.sPosition2:
        //                    Log.Debug("Position 2");
        //                    break;
        //                case CAMERA1.sProgramPosition2:
        //                    Log.Debug("Position 2 program");
        //                    break;
        //                case CAMERA1.sPosition3:
        //                    Log.Debug("Position 3");
        //                    break;
        //                case CAMERA1.sProgramPosition3:
        //                    Log.Debug("Position 3 program");
        //                    break;
        //                case CAMERA1.sPosition4:
        //                    Log.Debug("Position 4");
        //                    break;
        //                case CAMERA1.sProgramPosition4:
        //                    Log.Debug("Position 4 program");
        //                    break;
        //                case CAMERA1.sCenter:
        //                    Log.Debug("Center");
        //                    break;
        //                case CAMERA1.sProgramCenterPosition:
        //                    Log.Debug("Center program");
        //                    break;
        //                case CAMERA1.sSweep:
        //                    Log.Debug("Sweep");
        //                    break;
        //                case CAMERA1.sProgramSweep:
        //                    Log.Debug("Sweep program");
        //                    break;
        //                default:
        //                    Log.Debug("UNKNOWN");
        //                    break;
        //            }
        //            Log.Debug("Housecode     = " + Strings.Chr(recbuf(CAMERA1.housecode)));
        //            break;
        //        default:
        //            Log.Error("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(CAMERA1.packettype)) + ": " + Conversion.Hex(recbuf(CAMERA1.subtype)));
        //            break;
        //    }
        //    Log.Debug("Signal level  = " + (recbuf(CAMERA1.rssi) >> 4).ToString());
        //}

        //public void decode_Remote()
        //{
        //    switch (recbuf(REMOTE.subtype))
        //    {
        //        case REMOTE.ATI:
        //            Log.Debug("subtype       = ATI Remote Wonder");
        //            Log.Debug("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            Log.Debug("ID            = " + recbuf(REMOTE.id).ToString);
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    Log.Debug("Command       = A");
        //                    break;
        //                case 0x1:
        //                    Log.Debug("Command       = B");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("Command       = power");
        //                    break;
        //                case 0x3:
        //                    Log.Debug("Command       = TV");
        //                    break;
        //                case 0x4:
        //                    Log.Debug("Command       = DVD");
        //                    break;
        //                case 0x5:
        //                    Log.Debug("Command       = ?");
        //                    break;
        //                case 0x6:
        //                    Log.Debug("Command       = Guide");
        //                    break;
        //                case 0x7:
        //                    Log.Debug("Command       = Drag");
        //                    break;
        //                case 0x8:
        //                    Log.Debug("Command       = VOL+");
        //                    break;
        //                case 0x9:
        //                    Log.Debug("Command       = VOL-");
        //                    break;
        //                case 0xa:
        //                    Log.Debug("Command       = MUTE");
        //                    break;
        //                case 0xb:
        //                    Log.Debug("Command       = CHAN+");
        //                    break;
        //                case 0xc:
        //                    Log.Debug("Command       = CHAN-");
        //                    break;
        //                case 0xd:
        //                    Log.Debug("Command       = 1");
        //                    break;
        //                case 0xe:
        //                    Log.Debug("Command       = 2");
        //                    break;
        //                case 0xf:
        //                    Log.Debug("Command       = 3");
        //                    break;
        //                case 0x10:
        //                    Log.Debug("Command       = 4");
        //                    break;
        //                case 0x11:
        //                    Log.Debug("Command       = 5");
        //                    break;
        //                case 0x12:
        //                    Log.Debug("Command       = 6");
        //                    break;
        //                case 0x13:
        //                    Log.Debug("Command       = 7");
        //                    break;
        //                case 0x14:
        //                    Log.Debug("Command       = 8");
        //                    break;
        //                case 0x15:
        //                    Log.Debug("Command       = 9");
        //                    break;
        //                case 0x16:
        //                    Log.Debug("Command       = txt");
        //                    break;
        //                case 0x17:
        //                    Log.Debug("Command       = 0");
        //                    break;
        //                case 0x18:
        //                    Log.Debug("Command       = snapshot ESC");
        //                    break;
        //                case 0x19:
        //                    Log.Debug("Command       = C");
        //                    break;
        //                case 0x1a:
        //                    Log.Debug("Command       = ^");
        //                    break;
        //                case 0x1b:
        //                    Log.Debug("Command       = D");
        //                    break;
        //                case 0x1c:
        //                    Log.Debug("Command       = TV/RADIO");
        //                    break;
        //                case 0x1d:
        //                    Log.Debug("Command       = <");
        //                    break;
        //                case 0x1e:
        //                    Log.Debug("Command       = OK");
        //                    break;
        //                case 0x1f:
        //                    Log.Debug("Command       = >");
        //                    break;
        //                case 0x20:
        //                    Log.Debug("Command       = <-");
        //                    break;
        //                case 0x21:
        //                    Log.Debug("Command       = E");
        //                    break;
        //                case 0x22:
        //                    Log.Debug("Command       = v");
        //                    break;
        //                case 0x23:
        //                    Log.Debug("Command       = F");
        //                    break;
        //                case 0x24:
        //                    Log.Debug("Command       = Rewind");
        //                    break;
        //                case 0x25:
        //                    Log.Debug("Command       = Play");
        //                    break;
        //                case 0x26:
        //                    Log.Debug("Command       = Fast forward");
        //                    break;
        //                case 0x27:
        //                    Log.Debug("Command       = Record");
        //                    break;
        //                case 0x28:
        //                    Log.Debug("Command       = Stop");
        //                    break;
        //                case 0x29:
        //                    Log.Debug("Command       = Pause");

        //                    break;
        //                case 0x2c:
        //                    Log.Debug("Command       = TV");
        //                    break;
        //                case 0x2d:
        //                    Log.Debug("Command       = VCR");
        //                    break;
        //                case 0x2e:
        //                    Log.Debug("Command       = RADIO");
        //                    break;
        //                case 0x2f:
        //                    Log.Debug("Command       = TV Preview");
        //                    break;
        //                case 0x30:
        //                    Log.Debug("Command       = Channel list");
        //                    break;
        //                case 0x31:
        //                    Log.Debug("Command       = Video Desktop");
        //                    break;
        //                case 0x32:
        //                    Log.Debug("Command       = red");
        //                    break;
        //                case 0x33:
        //                    Log.Debug("Command       = green");
        //                    break;
        //                case 0x34:
        //                    Log.Debug("Command       = yellow");
        //                    break;
        //                case 0x35:
        //                    Log.Debug("Command       = blue");
        //                    break;
        //                case 0x36:
        //                    Log.Debug("Command       = rename TAB");
        //                    break;
        //                case 0x37:
        //                    Log.Debug("Command       = Acquire image");
        //                    break;
        //                case 0x38:
        //                    Log.Debug("Command       = edit image");
        //                    break;
        //                case 0x39:
        //                    Log.Debug("Command       = Full screen");
        //                    break;
        //                case 0x3a:
        //                    Log.Debug("Command       = DVD Audio");
        //                    break;
        //                case 0x70:
        //                    Log.Debug("Command       = Cursor-left");
        //                    break;
        //                case 0x71:
        //                    Log.Debug("Command       = Cursor-right");
        //                    break;
        //                case 0x72:
        //                    Log.Debug("Command       = Cursor-up");
        //                    break;
        //                case 0x73:
        //                    Log.Debug("Command       = Cursor-down");
        //                    break;
        //                case 0x74:
        //                    Log.Debug("Command       = Cursor-up-left");
        //                    break;
        //                case 0x75:
        //                    Log.Debug("Command       = Cursor-up-right");
        //                    break;
        //                case 0x76:
        //                    Log.Debug("Command       = Cursor-down-right");
        //                    break;
        //                case 0x77:
        //                    Log.Debug("Command       = Cursor-down-left");
        //                    break;
        //                case 0x78:
        //                    Log.Debug("Command       = V");
        //                    break;
        //                case 0x79:
        //                    Log.Debug("Command       = V-End");
        //                    break;
        //                case 0x7c:
        //                    Log.Debug("Command       = X");
        //                    break;
        //                case 0x7d:
        //                    Log.Debug("Command       = X-End");
        //                    break;
        //                default:
        //                    Log.Debug("Command       = unknown");
        //                    break;
        //            }

        //            break;
        //        case REMOTE.ATI2:
        //            Log.Debug("subtype       = ATI Remote Wonder II");
        //            Log.Debug("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            Log.Debug("ID            = " + recbuf(REMOTE.id).ToString);
        //            Log.Debug("Command       = ");
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    Log.Debug("A");
        //                    break;
        //                case 0x1:
        //                    Log.Debug("B");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("power");
        //                    break;
        //                case 0x3:
        //                    Log.Debug("TV");
        //                    break;
        //                case 0x4:
        //                    Log.Debug("DVD");
        //                    break;
        //                case 0x5:
        //                    Log.Debug("?");
        //                    break;
        //                case 0x6:
        //                    Log.Debug("Guide");
        //                    break;
        //                case 0x7:
        //                    Log.Debug("Drag");
        //                    break;
        //                case 0x8:
        //                    Log.Debug("VOL+");
        //                    break;
        //                case 0x9:
        //                    Log.Debug("VOL-");
        //                    break;
        //                case 0xa:
        //                    Log.Debug("MUTE");
        //                    break;
        //                case 0xb:
        //                    Log.Debug("CHAN+");
        //                    break;
        //                case 0xc:
        //                    Log.Debug("CHAN-");
        //                    break;
        //                case 0xd:
        //                    Log.Debug("1");
        //                    break;
        //                case 0xe:
        //                    Log.Debug("2");
        //                    break;
        //                case 0xf:
        //                    Log.Debug("3");
        //                    break;
        //                case 0x10:
        //                    Log.Debug("4");
        //                    break;
        //                case 0x11:
        //                    Log.Debug("5");
        //                    break;
        //                case 0x12:
        //                    Log.Debug("6");
        //                    break;
        //                case 0x13:
        //                    Log.Debug("7");
        //                    break;
        //                case 0x14:
        //                    Log.Debug("8");
        //                    break;
        //                case 0x15:
        //                    Log.Debug("9");
        //                    break;
        //                case 0x16:
        //                    Log.Debug("txt");
        //                    break;
        //                case 0x17:
        //                    Log.Debug("0");
        //                    break;
        //                case 0x18:
        //                    Log.Debug("Open Setup Menu");
        //                    break;
        //                case 0x19:
        //                    Log.Debug("C");
        //                    break;
        //                case 0x1a:
        //                    Log.Debug("^");
        //                    break;
        //                case 0x1b:
        //                    Log.Debug("D");
        //                    break;
        //                case 0x1c:
        //                    Log.Debug("FM");
        //                    break;
        //                case 0x1d:
        //                    Log.Debug("<");
        //                    break;
        //                case 0x1e:
        //                    Log.Debug("OK");
        //                    break;
        //                case 0x1f:
        //                    Log.Debug(">");
        //                    break;
        //                case 0x20:
        //                    Log.Debug("Max/Restore window");
        //                    break;
        //                case 0x21:
        //                    Log.Debug("E");
        //                    break;
        //                case 0x22:
        //                    Log.Debug("v");
        //                    break;
        //                case 0x23:
        //                    Log.Debug("F");
        //                    break;
        //                case 0x24:
        //                    Log.Debug("Rewind");
        //                    break;
        //                case 0x25:
        //                    Log.Debug("Play");
        //                    break;
        //                case 0x26:
        //                    Log.Debug("Fast forward");
        //                    break;
        //                case 0x27:
        //                    Log.Debug("Record");
        //                    break;
        //                case 0x28:
        //                    Log.Debug("Stop");
        //                    break;
        //                case 0x29:
        //                    Log.Debug("Pause");
        //                    break;
        //                case 0x2a:
        //                    Log.Debug("TV2");
        //                    break;
        //                case 0x2b:
        //                    Log.Debug("Clock");
        //                    break;
        //                case 0x2c:
        //                    Log.Debug("i");
        //                    break;
        //                case 0x2d:
        //                    Log.Debug("ATI");
        //                    break;
        //                case 0x2e:
        //                    Log.Debug("RADIO");
        //                    break;
        //                case 0x2f:
        //                    Log.Debug("TV Preview");
        //                    break;
        //                case 0x30:
        //                    Log.Debug("Channel list");
        //                    break;
        //                case 0x31:
        //                    Log.Debug("Video Desktop");
        //                    break;
        //                case 0x32:
        //                    Log.Debug("red");
        //                    break;
        //                case 0x33:
        //                    Log.Debug("green");
        //                    break;
        //                case 0x34:
        //                    Log.Debug("yellow");
        //                    break;
        //                case 0x35:
        //                    Log.Debug("blue");
        //                    break;
        //                case 0x36:
        //                    Log.Debug("rename TAB");
        //                    break;
        //                case 0x37:
        //                    Log.Debug("Acquire image");
        //                    break;
        //                case 0x38:
        //                    Log.Debug("edit image");
        //                    break;
        //                case 0x39:
        //                    Log.Debug("Full screen");
        //                    break;
        //                case 0x3a:
        //                    Log.Debug("DVD Audio");
        //                    break;
        //                case 0x70:
        //                    Log.Debug("Cursor-left");
        //                    break;
        //                case 0x71:
        //                    Log.Debug("Cursor-right");
        //                    break;
        //                case 0x72:
        //                    Log.Debug("Cursor-up");
        //                    break;
        //                case 0x73:
        //                    Log.Debug("Cursor-down");
        //                    break;
        //                case 0x74:
        //                    Log.Debug("Cursor-up-left");
        //                    break;
        //                case 0x75:
        //                    Log.Debug("Cursor-up-right");
        //                    break;
        //                case 0x76:
        //                    Log.Debug("Cursor-down-right");
        //                    break;
        //                case 0x77:
        //                    Log.Debug("Cursor-down-left");
        //                    break;
        //                case 0x78:
        //                    Log.Debug("Left Mouse Button");
        //                    break;
        //                case 0x79:
        //                    Log.Debug("V-End");
        //                    break;
        //                case 0x7c:
        //                    Log.Debug("Right Mouse Button");
        //                    break;
        //                case 0x7d:
        //                    Log.Debug("X-End");
        //                    break;
        //                default:
        //                    Log.Debug("unknown");
        //                    break;
        //            }
        //            if ((recbuf(REMOTE.toggle) & 0x1) == 0x1)
        //            {
        //                Log.Debug("  (button press = odd)");
        //            }
        //            else
        //            {
        //                Log.Debug("  (button press = even)");
        //            }

        //            break;
        //        case REMOTE.Medion:
        //            Log.Debug("subtype       = Medion Remote");
        //            Log.Debug("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            Log.Debug("ID            = " + recbuf(REMOTE.id).ToString);
        //            Log.Debug("Command       = ");
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x0:
        //                    Log.Debug("Mute");
        //                    break;
        //                case 0x1:
        //                    Log.Debug("B");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("power");
        //                    break;
        //                case 0x3:
        //                    Log.Debug("TV");
        //                    break;
        //                case 0x4:
        //                    Log.Debug("DVD");
        //                    break;
        //                case 0x5:
        //                    Log.Debug("Photo");
        //                    break;
        //                case 0x6:
        //                    Log.Debug("Music");
        //                    break;
        //                case 0x7:
        //                    Log.Debug("Drag");
        //                    break;
        //                case 0x8:
        //                    Log.Debug("VOL-");
        //                    break;
        //                case 0x9:
        //                    Log.Debug("VOL+");
        //                    break;
        //                case 0xa:
        //                    Log.Debug("MUTE");
        //                    break;
        //                case 0xb:
        //                    Log.Debug("CHAN+");
        //                    break;
        //                case 0xc:
        //                    Log.Debug("CHAN-");
        //                    break;
        //                case 0xd:
        //                    Log.Debug("1");
        //                    break;
        //                case 0xe:
        //                    Log.Debug("2");
        //                    break;
        //                case 0xf:
        //                    Log.Debug("3");
        //                    break;
        //                case 0x10:
        //                    Log.Debug("4");
        //                    break;
        //                case 0x11:
        //                    Log.Debug("5");
        //                    break;
        //                case 0x12:
        //                    Log.Debug("6");
        //                    break;
        //                case 0x13:
        //                    Log.Debug("7");
        //                    break;
        //                case 0x14:
        //                    Log.Debug("8");
        //                    break;
        //                case 0x15:
        //                    Log.Debug("9");
        //                    break;
        //                case 0x16:
        //                    Log.Debug("txt");
        //                    break;
        //                case 0x17:
        //                    Log.Debug("0");
        //                    break;
        //                case 0x18:
        //                    Log.Debug("snapshot ESC");
        //                    break;
        //                case 0x19:
        //                    Log.Debug("DVD MENU");
        //                    break;
        //                case 0x1a:
        //                    Log.Debug("^");
        //                    break;
        //                case 0x1b:
        //                    Log.Debug("Setup");
        //                    break;
        //                case 0x1c:
        //                    Log.Debug("TV/RADIO");
        //                    break;
        //                case 0x1d:
        //                    Log.Debug("<");
        //                    break;
        //                case 0x1e:
        //                    Log.Debug("OK");
        //                    break;
        //                case 0x1f:
        //                    Log.Debug(">");
        //                    break;
        //                case 0x20:
        //                    Log.Debug("<-");
        //                    break;
        //                case 0x21:
        //                    Log.Debug("E");
        //                    break;
        //                case 0x22:
        //                    Log.Debug("v");
        //                    break;
        //                case 0x23:
        //                    Log.Debug("F");
        //                    break;
        //                case 0x24:
        //                    Log.Debug("Rewind");
        //                    break;
        //                case 0x25:
        //                    Log.Debug("Play");
        //                    break;
        //                case 0x26:
        //                    Log.Debug("Fast forward");
        //                    break;
        //                case 0x27:
        //                    Log.Debug("Record");
        //                    break;
        //                case 0x28:
        //                    Log.Debug("Stop");
        //                    break;
        //                case 0x29:
        //                    Log.Debug("Pause");

        //                    break;
        //                case 0x2c:
        //                    Log.Debug("TV");
        //                    break;
        //                case 0x2d:
        //                    Log.Debug("VCR");
        //                    break;
        //                case 0x2e:
        //                    Log.Debug("RADIO");
        //                    break;
        //                case 0x2f:
        //                    Log.Debug("TV Preview");
        //                    break;
        //                case 0x30:
        //                    Log.Debug("Channel list");
        //                    break;
        //                case 0x31:
        //                    Log.Debug("Video Desktop");
        //                    break;
        //                case 0x32:
        //                    Log.Debug("red");
        //                    break;
        //                case 0x33:
        //                    Log.Debug("green");
        //                    break;
        //                case 0x34:
        //                    Log.Debug("yellow");
        //                    break;
        //                case 0x35:
        //                    Log.Debug("blue");
        //                    break;
        //                case 0x36:
        //                    Log.Debug("rename TAB");
        //                    break;
        //                case 0x37:
        //                    Log.Debug("Acquire image");
        //                    break;
        //                case 0x38:
        //                    Log.Debug("edit image");
        //                    break;
        //                case 0x39:
        //                    Log.Debug("Full screen");
        //                    break;
        //                case 0x3a:
        //                    Log.Debug("DVD Audio");
        //                    break;
        //                case 0x70:
        //                    Log.Debug("Cursor-left");
        //                    break;
        //                case 0x71:
        //                    Log.Debug("Cursor-right");
        //                    break;
        //                case 0x72:
        //                    Log.Debug("Cursor-up");
        //                    break;
        //                case 0x73:
        //                    Log.Debug("Cursor-down");
        //                    break;
        //                case 0x74:
        //                    Log.Debug("Cursor-up-left");
        //                    break;
        //                case 0x75:
        //                    Log.Debug("Cursor-up-right");
        //                    break;
        //                case 0x76:
        //                    Log.Debug("Cursor-down-right");
        //                    break;
        //                case 0x77:
        //                    Log.Debug("Cursor-down-left");
        //                    break;
        //                case 0x78:
        //                    Log.Debug("V");
        //                    break;
        //                case 0x79:
        //                    Log.Debug("V-End");
        //                    break;
        //                case 0x7c:
        //                    Log.Debug("X");
        //                    break;
        //                case 0x7d:
        //                    Log.Debug("X-End");
        //                    break;
        //                default:
        //                    Log.Debug("unknown");
        //                    break;
        //            }

        //            break;
        //        case REMOTE.PCremote:
        //            Log.Debug("subtype       = PC Remote");
        //            Log.Debug("Sequence nbr  = " + recbuf(REMOTE.seqnbr).ToString);
        //            Log.Debug("ID            = " + recbuf(REMOTE.id).ToString);
        //            Log.Debug("Command       = unknown");
        //            switch (recbuf(REMOTE.cmnd))
        //            {
        //                case 0x2:
        //                    Log.Debug("0");
        //                    break;
        //                case 0x82:
        //                    Log.Debug("1");
        //                    break;
        //                case 0xd1:
        //                    Log.Debug("MP3");
        //                    break;
        //                case 0x42:
        //                    Log.Debug("2");
        //                    break;
        //                case 0xd2:
        //                    Log.Debug("DVD");
        //                    break;
        //                case 0xc2:
        //                    Log.Debug("3");
        //                    break;
        //                case 0xd3:
        //                    Log.Debug("CD");
        //                    break;
        //                case 0x22:
        //                    Log.Debug("4");
        //                    break;
        //                case 0xd4:
        //                    Log.Debug("PC or SHIFT-4");
        //                    break;
        //                case 0xa2:
        //                    Log.Debug("5");
        //                    break;
        //                case 0xd5:
        //                    Log.Debug("SHIFT-5");
        //                    break;
        //                case 0x62:
        //                    Log.Debug("6");
        //                    break;
        //                case 0xe2:
        //                    Log.Debug("7");
        //                    break;
        //                case 0x12:
        //                    Log.Debug("8");
        //                    break;
        //                case 0x92:
        //                    Log.Debug("9");
        //                    break;
        //                case 0xc0:
        //                    Log.Debug("CH-");
        //                    break;
        //                case 0x40:
        //                    Log.Debug("CH+");
        //                    break;
        //                case 0xe0:
        //                    Log.Debug("VOL-");
        //                    break;
        //                case 0x60:
        //                    Log.Debug("VOL+");
        //                    break;
        //                case 0xa0:
        //                    Log.Debug("MUTE");
        //                    break;
        //                case 0x3a:
        //                    Log.Debug("INFO");
        //                    break;
        //                case 0x38:
        //                    Log.Debug("REW");
        //                    break;
        //                case 0xb8:
        //                    Log.Debug("FF");
        //                    break;
        //                case 0xb0:
        //                    Log.Debug("PLAY");
        //                    break;
        //                case 0x64:
        //                    Log.Debug("PAUSE");
        //                    break;
        //                case 0x63:
        //                    Log.Debug("STOP");
        //                    break;
        //                case 0xb6:
        //                    Log.Debug("MENU");
        //                    break;
        //                case 0xff:
        //                    Log.Debug("REC");
        //                    break;
        //                case 0xc9:
        //                    Log.Debug("EXIT");
        //                    break;
        //                case 0xd8:
        //                    Log.Debug("TEXT");
        //                    break;
        //                case 0xd9:
        //                    Log.Debug("SHIFT-TEXT");
        //                    break;
        //                case 0xf2:
        //                    Log.Debug("TELETEXT");
        //                    break;
        //                case 0xd7:
        //                    Log.Debug("SHIFT-TELETEXT");
        //                    break;
        //                case 0xba:
        //                    Log.Debug("A+B");
        //                    break;
        //                case 0x52:
        //                    Log.Debug("ENT");
        //                    break;
        //                case 0xd6:
        //                    Log.Debug("SHIFT-ENT");
        //                    break;
        //                case 0x70:
        //                    Log.Debug("Cursor-left");
        //                    break;
        //                case 0x71:
        //                    Log.Debug("Cursor-right");
        //                    break;
        //                case 0x72:
        //                    Log.Debug("Cursor-up");
        //                    break;
        //                case 0x73:
        //                    Log.Debug("Cursor-down");
        //                    break;
        //                case 0x74:
        //                    Log.Debug("Cursor-up-left");
        //                    break;
        //                case 0x75:
        //                    Log.Debug("Cursor-up-right");
        //                    break;
        //                case 0x76:
        //                    Log.Debug("Cursor-down-right");
        //                    break;
        //                case 0x77:
        //                    Log.Debug("Cursor-down-left");
        //                    break;
        //                case 0x78:
        //                    Log.Debug("Left mouse");
        //                    break;
        //                case 0x79:
        //                    Log.Debug("Left mouse-End");
        //                    break;
        //                case 0x7b:
        //                    Log.Debug("Drag");
        //                    break;
        //                case 0x7c:
        //                    Log.Debug("Right mouse");
        //                    break;
        //                case 0x7d:
        //                    Log.Debug("Right mouse-End");
        //                    break;
        //                default:
        //                    Log.Debug("unknown");
        //                    break;
        //            }

        //            break;
        //        default:
        //            Log.Error("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(REMOTE.packettype)) + ":" + Conversion.Hex(recbuf(REMOTE.subtype)));
        //            break;
        //    }
        //    Log.Debug("Signal level  = " + (recbuf(REMOTE.rssi) >> 4).ToString());

        //}

        //public void decode_Thermostat1()
        //{
        //    switch (recbuf(THERMOSTAT1.subtype))
        //    {
        //        case THERMOSTAT1.Digimax:
        //            Log.Debug("subtype       = Digimax");
        //            break;
        //        case THERMOSTAT1.DigimaxShort:
        //            Log.Debug("subtype       = Digimax with short format");
        //            break;
        //        default:
        //            Log.Debug("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(THERMOSTAT1.packettype)) + ":" + Conversion.Hex(recbuf(THERMOSTAT1.subtype)));
        //            break;
        //    }
        //    Log.Debug("Sequence nbr  = " + recbuf(THERMOSTAT1.seqnbr).ToString);
        //    Log.Debug("ID            = " + ((recbuf(THERMOSTAT1.id1) * 256 + recbuf(THERMOSTAT1.id2))).ToString());
        //    Log.Debug("Temperature   = " + recbuf(THERMOSTAT1.temperature).ToString + " °C");
        //    if (recbuf(THERMOSTAT1.subtype) == THERMOSTAT1.Digimax)
        //    {
        //        Log.Debug("Set           = " + recbuf(THERMOSTAT1.set_point).ToString + " °C");
        //        if ((recbuf(THERMOSTAT1.mode) & 0x80) == 0)
        //        {
        //            Log.Debug("Mode          = heating");
        //        }
        //        else
        //        {
        //            Log.Debug("Mode          = Cooling");
        //        }
        //        switch ((recbuf(THERMOSTAT1.status) & 0x3))
        //        {
        //            case 0:
        //                Log.Debug("Status        = no status available");
        //                break;
        //            case 1:
        //                Log.Debug("Status        = demand");
        //                break;
        //            case 2:
        //                Log.Debug("Status        = no demand");
        //                break;
        //            case 3:
        //                Log.Debug("Status        = initializing");
        //                break;
        //        }
        //    }

        //    Log.Debug("Signal level  = " + (recbuf(THERMOSTAT1.rssi) >> 4).ToString());
        //}

        //public void decode_Thermostat2()
        //{
        //    Log.Debug("Not implemented");
        //}

        //public void decode_Thermostat3()
        //{
        //    switch (recbuf(THERMOSTAT3.subtype))
        //    {
        //        case THERMOSTAT3.MertikG6RH4T1:
        //            Log.Debug("subtype       = Mertik G6R-H4T1");
        //            break;
        //        case THERMOSTAT3.MertikG6RH4TB:
        //            Log.Debug("subtype       = Mertik G6R-H4TB");
        //            break;
        //        default:
        //            Log.Debug("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(THERMOSTAT3.packettype)) + ":" + Conversion.Hex(recbuf(THERMOSTAT3.subtype)));
        //            break;
        //    }
        //    Log.Debug("Sequence nbr  = " + recbuf(THERMOSTAT3.seqnbr).ToString);

        //    Log.Debug("ID            = 0x" + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode2)), 2) + VB.Right("0" + Conversion.Hex(recbuf(THERMOSTAT3.unitcode3)), 2));

        //    switch (recbuf(THERMOSTAT3.cmnd))
        //    {
        //        case 0:
        //            Log.Debug("Command       = Off");
        //            break;
        //        case 1:
        //            Log.Debug("Command       = On");
        //            break;
        //        case 2:
        //            Log.Debug("Command       = Up");
        //            break;
        //        case 3:
        //            Log.Debug("Command       = Down");
        //            break;
        //        case 4:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                Log.Debug("Command       = Run Up");
        //            }
        //            else
        //            {
        //                Log.Debug("Command       = 2nd Off");
        //            }
        //            break;
        //        case 5:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                Log.Debug("Command       = Run Down");
        //            }
        //            else
        //            {
        //                Log.Debug("Command       = 2nd On");
        //            }
        //            break;
        //        case 6:
        //            if (recbuf(THERMOSTAT3.subtype) == THERMOSTAT3.MertikG6RH4T1)
        //            {
        //                Log.Debug("Command       = Stop");
        //            }
        //            else
        //            {
        //                Log.Debug("Command       = unknown");
        //            }
        //            break;
        //        default:
        //            Log.Debug("Command       = unknown");
        //            break;
        //    }

        //    Log.Debug("Signal level  = " + (recbuf(THERMOSTAT3.rssi) >> 4).ToString());
        //}

        public void decode_Temp()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName,"Learning Mode").Value == "TRUE")
            {
                Log.Info("New temperature sensor found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Temperature Sensor - " + (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString(), "", "Temperature Sensor", "OS TEMP SENSOR", (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString());
            }

            if (obj != null)
            {
                switch (recbuf[(byte)TEMP.subtype])
                {
                    case (byte)TEMP.TEMP1:
                        Log.Debug("subtype       = TEMP1 - THR128/138, THC138");
                        Log.Debug("                channel " + recbuf[(byte)TEMP.id2].ToString());
                        break;
                    case (byte)TEMP.TEMP2:
                        Log.Debug("subtype       = TEMP2 - THC238/268,THN132,THWR288,THRN122,THN122,AW129/131");
                        Log.Debug("                channel " + recbuf[(byte)TEMP.id2].ToString());
                        break;
                    case (byte)TEMP.TEMP3:
                        Log.Debug("subtype       = TEMP3 - THWR800");
                        break;
                    case (byte)TEMP.TEMP4:
                        Log.Debug("subtype       = TEMP4 - RTHN318");
                        Log.Debug("                channel " + recbuf[(byte)TEMP.id2].ToString());
                        break;
                    case (byte)TEMP.TEMP5:
                        Log.Debug("subtype       = TEMP5 - LaCrosse TX3, TX4, TX17");
                        break;
                    case (byte)TEMP.TEMP6:
                        Log.Debug("subtype       = TEMP6 - TS15C");
                        break;
                    default:
                        Log.Debug("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP.packettype].ToString() + ":" + recbuf[(byte)TEMP.subtype].ToString());
                        break;
                }
                Log.Debug("Sequence nbr  = " + recbuf[(byte)TEMP.seqnbr].ToString());
                Log.Debug("ID            = " + (recbuf[(byte)TEMP.id1] * 256 + recbuf[(byte)TEMP.id2]).ToString());

                double temp = Math.Round((double)(recbuf[(byte)TEMP.temperatureh] * 256 + recbuf[(byte)TEMP.temperaturel]) / 10, 2);
                string strTemp = "";
                double prevTemp = Double.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Temperature").Value);

                if (OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Temp Units").Value.Trim() == "Farenheit")
                {
                    temp = (temp * 9 / 5) + 32;
                    strTemp = temp.ToString() + " °F";
                }
                else
                    strTemp = temp.ToString() + " °C";

                if (Math.Abs(prevTemp - temp) < 5)
                {
                    if ((recbuf[(byte)TEMP.tempsign] & 0x80) == 0)
                    {
                        Log.Debug("Temperature   = " + strTemp);
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", temp.ToString(), pName);
                    }
                    else
                    {
                        Log.Debug("Temperature   = -" + strTemp);
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", "-" + temp.ToString(), pName);
                    }
                    Log.Debug("Signal level  = " + (recbuf[(byte)TEMP.rssi] >> 4).ToString());
                    if ((recbuf[(byte)TEMP.battery_level] & 0xf) == 0)
                    {
                        Log.Debug("Battery       = Low");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
                    }
                    else
                    {
                        Log.Debug("Battery       = OK");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
                    }
                }
                else
                    Log.Debug("Temperature not logged.  Too much of a difference from previous temp");
            }
        }

        public void decode_Hum()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New humidity sensor found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Humidity Sensor - " + (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString(), "", "Humidity Sensor", "HUMIDITY METER", (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString());
            }

            switch (recbuf[(byte)HUM.subtype])
            {
                case (byte)HUM.HUM1:
                    Log.Debug("subtype       = HUM1 - LaCrosse TX3");
                    break;
                default:
                    Log.Error("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)HUM.packettype] + ":" + recbuf[(byte)HUM.subtype]);
                    break;
            }
            Log.Debug("Sequence nbr  = " + recbuf[(byte)HUM.seqnbr].ToString());
            Log.Debug("ID            = " + (recbuf[(byte)HUM.id1] * 256 + recbuf[(byte)HUM.id2]).ToString());
            Log.Debug("Humidity      = " + recbuf[(byte)HUM.humidity].ToString());
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Humidity", recbuf[(byte)HUM.humidity].ToString(), pName);

            switch (recbuf[(byte)HUM.humidity_status])
            {
                case 0x0:
                    Log.Debug("Status        = Dry");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Dry", pName);
                    break;
                case 0x1:
                    Log.Debug("Status        = Comfortable");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Comfortable", pName);
                    break;
                case 0x2:
                    Log.Debug("Status        = Normal");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Normal", pName);
                    break;
                case 0x3:
                    Log.Debug("Status        = Wet");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Wet", pName);
                    break;
            }
            Log.Debug("Signal level  = " + (recbuf[(byte)HUM.rssi] >> 4).ToString());
            if ((recbuf[(byte)HUM.battery_level] & 0xf) == 0)
            {
                Log.Debug("Battery       = Low");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
            }
            else
            {
                Log.Debug("Battery       = OK");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
            }
        }

        public void decode_TempHum()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New temperature and humidity sensor found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Temp and Humidity Sensor - " + (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString(), "", "Temp and Humidity Sensor", "TEMP HUM METER", (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString());
            }
            
            switch (recbuf[(byte)TEMP_HUM.subtype])
            {
                case (byte)TEMP_HUM.TH1:
                    Log.Debug("subtype       = TH1 - THGN122/123,/THGN132,THGR122/228/238/268");
                    Log.Debug("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString());
                    break;
                case (byte)TEMP_HUM.TH2:
                    Log.Debug("subtype       = TH2 - THGR810,THGN800");
                    Log.Debug("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString());
                    break;
                case (byte)TEMP_HUM.TH3:
                    Log.Debug("subtype       = TH3 - RTGR328");
                    Log.Debug("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString());
                    break;
                case (byte)TEMP_HUM.TH4:
                    Log.Debug("subtype       = TH4 - THGR328");
                    Log.Debug("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString());
                    break;
                case (byte)TEMP_HUM.TH5:
                    Log.Debug("subtype       = TH5 - WTGR800");
                    break;
                case (byte)TEMP_HUM.TH6:
                    Log.Debug("subtype       = TH6 - THGR918,THGRN228,THGN500");
                    Log.Debug("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString());
                    break;
                case (byte)TEMP_HUM.TH7:
                    Log.Debug("subtype       = TH7 - Cresta, TFA TS34C");
                    if (recbuf[(byte)TEMP_HUM.id1] < 0x40)
                        Log.Debug("                channel 1");
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0x60)
                        Log.Debug("                channel 2");
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0x80)
                        Log.Debug("                channel 3");
                    else if (recbuf[(byte)TEMP_HUM.id1] > 0x9f & (byte)TEMP_HUM.id1 < 0xc0)
                        Log.Debug("                channel 4");
                    else if (recbuf[(byte)TEMP_HUM.id1] < 0xe0)
                        Log.Debug("                channel 5");
                    else
                        Log.Debug("                channel ??");
                    break;
                case (byte)TEMP_HUM.TH8:
                    Log.Debug("subtype       = TH8 - WT440H,WT450H");
                    Log.Debug("                channel " + recbuf[(byte)TEMP_HUM.id2].ToString());
                    break;
                default:
                    Log.Debug("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP_HUM.packettype] + ":" + recbuf[(byte)TEMP_HUM.subtype]);
                    break;
            }
            Log.Debug("Sequence nbr  = " + recbuf[(byte)TEMP_HUM.seqnbr].ToString());
            Log.Debug("ID            = " + (recbuf[(byte)TEMP_HUM.id1] * 256 + recbuf[(byte)TEMP_HUM.id2]).ToString());
            if ((recbuf[(byte)TEMP_HUM.tempsign] & 0x80) == 0)
            {
                Log.Debug("Temperature   = " + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
            }
            else
            {
                Log.Debug("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM.temperatureh] * 256 + recbuf[(byte)TEMP_HUM.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
            }
            Log.Debug("Humidity      = " + recbuf[(byte)TEMP_HUM.humidity].ToString());
            switch (recbuf[(byte)TEMP_HUM.humidity_status])
            {
                case 0x0:
                    Log.Debug("Status        = Dry");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Dry", pName);
                    break;
                case 0x1:
                    Log.Debug("Status        = Comfortable");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Comfortable", pName);
                    break;
                case 0x2:
                    Log.Debug("Status        = Normal");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Normal", pName);
                    break;
                case 0x3:
                    Log.Debug("Status        = Wet");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Wet", pName);
                    break;
            }
            Log.Debug("Signal level  = " + (recbuf[(byte)TEMP_HUM.rssi] >> 4).ToString());
            if (recbuf[(byte)TEMP_HUM.subtype] == (byte)TEMP_HUM.TH6)
            {
                switch (recbuf[(byte)TEMP_HUM.battery_level])
                {
                    case 0:
                        Log.Debug("Battery       = 10%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "10%", pName);
                        break;
                    case 1:
                        Log.Debug("Battery       = 20%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "20%", pName);
                        break;
                    case 2:
                        Log.Debug("Battery       = 30%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "30%", pName);
                        break;
                    case 3:
                        Log.Debug("Battery       = 40%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "40%", pName);
                        break;
                    case 4:
                        Log.Debug("Battery       = 50%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "50%", pName);
                        break;
                    case 5:
                        Log.Debug("Battery       = 60%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "60%", pName);
                        break;
                    case 6:
                        Log.Debug("Battery       = 70%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "70%", pName);
                        break;
                    case 7:
                        Log.Debug("Battery       = 80%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "80%", pName);
                        break;
                    case 8:
                        Log.Debug("Battery       = 90%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "90%", pName);
                        break;
                    case 9:
                        Log.Debug("Battery       = 100%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "100%", pName);
                        break;
                }
            }
            else
            {
                if ((recbuf[(byte)TEMP_HUM.battery_level] & 0xf) == 0)
                {
                    Log.Debug("Battery       = Low");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
                }
                else
                {
                    Log.Debug("Battery       = OK");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
                }
            }
        }

        public void decode_Baro()
        {
            Log.Error("Baro Not implemented");
        }

        public void decode_TempHumBaro()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New temperature, humidity and barometric sensor found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Temp, Humidity and Baro Sensor - " + (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString(), "", "Temp, Humidity and Baro Sensor", "TEMP HUM BARO METER", (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString());
            }
            
            switch (recbuf[(byte)TEMP_HUM_BARO.subtype])
            {
                case (byte)TEMP_HUM_BARO.THB1:
                    Log.Debug("subtype       = THB1 - BTHR918");
                    Log.Debug("                channel " + recbuf[(byte)TEMP_HUM_BARO.id2].ToString());
                    break;
                case (byte)TEMP_HUM_BARO.THB2:
                    Log.Debug("subtype       = THB2 - BTHR918N, BTHR968");
                    Log.Debug("                channel " + recbuf[(byte)TEMP_HUM_BARO.id2].ToString());
                    break;
                default:
                    Log.Debug("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)TEMP_HUM_BARO.packettype] + ":" + recbuf[(byte)TEMP_HUM_BARO.subtype]);
                    break;
            }
            Log.Debug("Sequence nbr  = " + recbuf[(byte)TEMP_HUM_BARO.seqnbr].ToString());
            Log.Debug("ID            = " + (recbuf[(byte)TEMP_HUM_BARO.id1] * 256 + recbuf[(byte)TEMP_HUM_BARO.id2]).ToString());
            if ((recbuf[(byte)TEMP_HUM_BARO.tempsign] & 0x80) == 0)
            {
                Log.Debug("Temperature   = " + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
            }
            else
            {
                Log.Debug("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)TEMP_HUM_BARO.temperatureh] * 256 + recbuf[(byte)TEMP_HUM_BARO.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
            }
            Log.Debug("Humidity      = " + recbuf[(byte)TEMP_HUM_BARO.humidity].ToString());
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Humidity", recbuf[(byte)TEMP_HUM_BARO.humidity].ToString(), pName);
            
            switch (recbuf[(byte)TEMP_HUM_BARO.humidity_status])
            {
                case 0x0:
                    Log.Debug("Status        = Dry");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Dry", pName);
                    break;
                case 0x1:
                    Log.Debug("Status        = Comfortable");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Comfortable", pName);
                    break;
                case 0x2:
                    Log.Debug("Status        = Normal");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Normal", pName);
                    break;
                case 0x3:
                    Log.Debug("Status        = Wet");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Status", "Wet", pName);
                    break;
            }
            Log.Debug("Barometer     = " + recbuf[(byte)TEMP_HUM_BARO.baroh] * 256 + recbuf[(byte)TEMP_HUM_BARO.barol].ToString());
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Barometer", recbuf[(byte)TEMP_HUM_BARO.baroh] * 256 + recbuf[(byte)TEMP_HUM_BARO.barol].ToString(), pName);

            switch (recbuf[(byte)TEMP_HUM_BARO.forecast])
            {
                case 0x0:
                    Log.Debug("Forecast      = No information available");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Forecast", "No information available", pName);
                    break;
                case 0x1:
                    Log.Debug("Forecast      = Sunny");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Forecast", "Sunny", pName);
                    break;
                case 0x2:
                    Log.Debug("Forecast      = Partly Cloudy");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Forecast", "Partly Cloudy", pName);
                    break;
                case 0x3:
                    Log.Debug("Forecast      = Cloudy");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Forecast", "Cloudy", pName);
                    break;
                case 0x4:
                    Log.Debug("Forecast      = Rain");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Forecast", "Rain", pName);
                    break;
            }

            Log.Debug("Signal level  = " + (recbuf[(byte)TEMP_HUM_BARO.rssi] >> 4).ToString());
            if ((recbuf[(byte)TEMP_HUM_BARO.battery_level] & 0xf) == 0)
            {
                Log.Debug("Battery       = Low");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
            }
            else
            {
                Log.Debug("Battery       = OK");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
            }
        }

        public void decode_Rain()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New temperature sensor found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Rain Meter - " + (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString(), "", "Rain Meter", "OS RAIN METER", (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString());
            }

            switch (recbuf[(byte)RAIN.subtype])
            {
                case (byte)RAIN.RAIN1:
                    Log.Debug("subtype       = RAIN1 - RGR126/682/918");
                    break;
                case (byte)RAIN.RAIN2:
                    Log.Debug("subtype       = RAIN2 - PCR800");
                    break;
                case (byte)RAIN.RAIN3:
                    Log.Debug("subtype       = RAIN3 - TFA");
                    break;
                case (byte)RAIN.RAIN4:
                    Log.Debug("subtype       = RAIN4 - UPM RG700");
                    break;
                default:
                    Log.Error("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)RAIN.packettype].ToString() + ":" + recbuf[(byte)RAIN.subtype].ToString());
                    break;
            }
            Log.Error("Sequence nbr  = " + recbuf[(byte)RAIN.seqnbr].ToString());
            Log.Error("ID            = " + (recbuf[(byte)RAIN.id1] * 256 + recbuf[(byte)RAIN.id2]).ToString());

            if (recbuf[(byte)RAIN.subtype] == (byte)RAIN.RAIN1)
            {
                Log.Error("Rain rate     = " + ((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]).ToString() + " mm/h");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Rain Rate", ((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]).ToString(), pName);
            }
            else if (recbuf[(byte)RAIN.subtype] == (byte)RAIN.RAIN2)
            {
                Log.Error("Rain rate     = " + (((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]) / 100).ToString() + " mm/h");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Rain Rate", (((recbuf[(byte)RAIN.rainrateh] * 256) + recbuf[(byte)RAIN.rainratel]) / 100).ToString(), pName);
            }

            Log.Error("Total rain    = " + Math.Round((double)((recbuf[(byte)RAIN.raintotal1] * 65535) + recbuf[(byte)RAIN.raintotal2] * 256 + recbuf[(byte)RAIN.raintotal3]) / 10, 2).ToString() + " mm");
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Total Rain", Math.Round((double)((recbuf[(byte)RAIN.raintotal1] * 65535) + recbuf[(byte)RAIN.raintotal2] * 256 + recbuf[(byte)RAIN.raintotal3]) / 10, 2).ToString(), pName);

            Log.Error("Signal level  = " + (recbuf[(byte)RAIN.rssi] >> 4).ToString());
            if ((recbuf[(byte)RAIN.battery_level] & 0xf) == 0)
            {
                Log.Error("Battery       = Low");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
            }
            else
            {
                Log.Error("Battery       = OK");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
            }
        }

        public void decode_Wind()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New wind sensor found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Wind Sensor - " + (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString(), "", "Wind Sensor", "WIND SENSOR", (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString());
            }

            int intDirection = 0;
            int intSpeed = 0;
            string strDirection = null;

            switch (recbuf[(byte)WIND.subtype])
            {
                case (byte)WIND.WIND1:
                    Log.Debug("subtype       = WIND1 - WTGR800");
                    break;
                case (byte)WIND.WIND2:
                    Log.Debug("subtype       = WIND2 - WGR800");
                    break;
                case (byte)WIND.WIND3:
                    Log.Debug("subtype       = WIND3 - STR918, WGR918");
                    break;
                case (byte)WIND.WIND4:
                    Log.Debug("subtype       = WIND4 - TFA");
                    break;
                case (byte)WIND.WIND5:
                    Log.Debug("subtype       = WIND5 - UPM WDS500");
                    break;
                default:
                    Log.Debug("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)WIND.packettype] + ":" + recbuf[(byte)WIND.subtype]);
                    break;
            }
            Log.Debug("Sequence nbr  = " + recbuf[(byte)WIND.seqnbr].ToString());
            Log.Debug("ID            = " + (recbuf[(byte)WIND.id1] * 256 + recbuf[(byte)WIND.id2]).ToString());
            intDirection = (recbuf[(byte)WIND.directionh] * 256) + recbuf[(byte)WIND.directionl];
            if (intDirection > 348.75 | intDirection < 11.26)
                strDirection = "N";
            else if (intDirection < 33.76)
                strDirection = "NNE";
            else if (intDirection < 56.26)
                strDirection = "NE";
            else if (intDirection < 78.76)
                strDirection = "ENE";
            else if (intDirection < 101.26)
                strDirection = "E";
            else if (intDirection < 123.76)
                strDirection = "ESE";
            else if (intDirection < 146.26)
                strDirection = "SE";
            else if (intDirection < 168.76)
                strDirection = "SSE";
            else if (intDirection < 191.26)
                strDirection = "S";
            else if (intDirection < 213.76)
                strDirection = "SSW";
            else if (intDirection < 236.26)
                strDirection = "SW";
            else if (intDirection < 258.76)
                strDirection = "WSW";
            else if (intDirection < 281.26)
                strDirection = "W";
            else if (intDirection < 303.76)
                strDirection = "WNW";
            else if (intDirection < 326.26)
                strDirection = "NW";
            else if (intDirection < 348.76)
                strDirection = "NNW";
            else
                strDirection = "---";

            Log.Debug("Direction     = " + intDirection.ToString() + " degrees  " + strDirection);
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Direction", intDirection.ToString() + " degrees  " + strDirection, pName);

            intSpeed = (recbuf[(byte)WIND.av_speedh] * 256) + recbuf[(byte)WIND.av_speedl];
            if (recbuf[(byte)WIND.subtype] != (byte)WIND.WIND5)
            {
                Log.Debug("Average speed = " + (intSpeed / 10).ToString() + " mtr/sec = " + Math.Round((intSpeed * 0.36), 2).ToString() + " km/hr = " + Math.Round((intSpeed * 0.223693629) / 10, 2).ToString() + " mph");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Average Speed", Math.Round((intSpeed * 0.223693629) / 10, 2).ToString(), pName);
            }

            intSpeed = (recbuf[(byte)WIND.gusth] * 256) + recbuf[(byte)WIND.gustl];
            Log.Debug("Wind gust     = " + (intSpeed / 10).ToString() + " mtr/sec = " + Math.Round((intSpeed * 0.36), 2).ToString() + " km/hr = " + Math.Round((intSpeed * 0.223693629) / 10, 2).ToString() + " mph");
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Wind Gust", Math.Round((intSpeed * 0.223693629) / 10, 2).ToString(), pName); 
            
            if (recbuf[(byte)WIND.subtype] == (byte)WIND.WIND4)
            {
                if (((byte)WIND.tempsign & 0x80) == 0)
                {
                    Log.Debug("Temperature   = " + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
                }
                else
                {
                    Log.Debug("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", "-" + (((Math.Round((double)(recbuf[(byte)WIND.temperatureh] * 256 + recbuf[(byte)WIND.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
                }

                if (((byte)WIND.chillsign & 0x80) == 0)
                {
                    Log.Debug("Chill         = " + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Windchill", (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
                }
                else
                {
                    Log.Debug("Chill         = -" + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Windchill", "-" + (((Math.Round((double)(recbuf[(byte)WIND.chillh] * 256 + recbuf[(byte)WIND.chillh]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
                }
            }

            Log.Debug("Signal level  = " + (recbuf[(byte)WIND.rssi] >> 4).ToString());
            if (recbuf[(byte)WIND.subtype] == (byte)WIND.WIND3)
            {
                switch (recbuf[(byte)WIND.battery_level])
                {
                    case 0:
                        Log.Debug("Battery       = 10%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "10%", pName);
                        break;
                    case 1:
                        Log.Debug("Battery       = 20%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "20%", pName);
                        break;
                    case 2:
                        Log.Debug("Battery       = 30%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "30%", pName);
                        break;
                    case 3:
                        Log.Debug("Battery       = 40%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "40%", pName);
                        break;
                    case 4:
                        Log.Debug("Battery       = 50%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "50%", pName);
                        break;
                    case 5:
                        Log.Debug("Battery       = 60%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "60%", pName);
                        break;
                    case 6:
                        Log.Debug("Battery       = 70%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "70%", pName);
                        break;
                    case 7:
                        Log.Debug("Battery       = 80%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "80%", pName);
                        break;
                    case 8:
                        Log.Debug("Battery       = 90%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "90%", pName);
                        break;
                    case 9:
                        Log.Debug("Battery       = 100%");
                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "100%", pName);
                        break;
                }
            }
            else
            {
                if ((recbuf[(byte)WIND.battery_level] & 0xf) == 0)
                {
                    Log.Debug("Battery       = Low");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
                }
                else
                {
                    Log.Debug("Battery       = OK");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
                }
            }
        }

        public void decode_UV()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New UV sensor found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("UV Sensor - " + (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString(), "", "UV Sensor", "UV SENSOR", (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString());
            }

            switch (recbuf[(byte)UV.subtype])
            {
                case (byte)UV.UV1:
                    Log.Debug("Subtype       = UV1 - UVN128, UV138");
                    break;
                case (byte)UV.UV2:
                    Log.Debug("Subtype       = UV2 - UVN800");
                    break;
                case (byte)UV.UV3:
                    Log.Debug("Subtype       = UV3 - TFA");
                    break;
                default:
                    Log.Debug("ERROR: Unknown Sub type for Packet type=" + (byte)UV.packettype + ":" + recbuf[(byte)UV.subtype]);
                    break;
            }
            Log.Debug("Sequence nbr  = " + recbuf[(byte)UV.seqnbr].ToString());
            Log.Debug("ID            = " + (recbuf[(byte)UV.id1] * 256 + recbuf[(byte)UV.id2]).ToString());
            Log.Debug("Level         = " + (recbuf[(byte)UV.uv] / 10).ToString());
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Level", (recbuf[(byte)UV.uv] / 10).ToString(), pName);

            if (recbuf[(byte)UV.subtype] == (byte)UV.UV3)
            {
                if (((byte)UV.tempsign & 0x80) == 0)
                {
                    Log.Debug("Temperature   = " + (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Level", (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
                }
                else
                {
                    Log.Debug("Temperature   = -" + (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString() + " °F");
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Level", (((Math.Round((double)(recbuf[(byte)UV.temperatureh] * 256 + recbuf[(byte)UV.temperaturel]) / 10, 2)) * 9 / 5) + 32).ToString(), pName);
                }
            }
            if (recbuf[(byte)UV.uv] < 3)
            {
                Log.Debug("Description = Low");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Description", "Low", pName);
            }
            else if (recbuf[(byte)UV.uv] < 6)
            {
                Log.Debug("Description = Medium");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Description", "Medium", pName);
            }
            else if (recbuf[(byte)UV.uv] < 8)
            {
                Log.Debug("Description = High");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Description", "High", pName);
            }
            else if (recbuf[(byte)UV.uv] < 11)
            {
                Log.Debug("Description = Very high");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Description", "Very high", pName);
            }
            else
            {
                Log.Debug("Description = Dangerous");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Description", "Dangerous", pName);
            }
            Log.Debug("Signal level  = " + (recbuf[(byte)UV.rssi] >> 4).ToString());
            if ((recbuf[(byte)UV.battery_level] & 0xf) == 0)
            {
                Log.Debug("Battery       = Low");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
            }
            else
            {
                Log.Debug("Battery       = OK");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
            }
        }


        public void decode_DateTime()
        {
            Log.Error("DateTime Not implemented");
        }

        public void decode_Current()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New Current meter found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Current Meter - " + (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString(), "", "Current Meter", "CURRENT METER", (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString());
            }

            switch (recbuf[(byte)CURRENT.subtype])
            {
                case (byte)CURRENT.ELEC1:
                    Log.Debug("subtype       = ELEC1 - OWL CM113, Electrisave, cent-a-meter");
                    break;
                default:
                    Log.Debug("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)CURRENT.packettype] + ":" + recbuf[(byte)CURRENT.subtype]);
                    break;
            }
            Log.Debug("Sequence nbr  = " + recbuf[(byte)CURRENT.seqnbr].ToString());
            Log.Debug("ID            = " + (recbuf[(byte)CURRENT.id1] * 256 + recbuf[(byte)CURRENT.id2]).ToString());
            Log.Debug("Count         = " + recbuf[5].ToString());
            Log.Debug("Channel 1     = " + ((recbuf[(byte)CURRENT.ch1h] * 256 + recbuf[(byte)CURRENT.ch1l]) / 10).ToString() + " ampere");
            Log.Debug("Channel 2     = " + ((recbuf[(byte)CURRENT.ch2h] * 256 + recbuf[(byte)CURRENT.ch2l]) / 10).ToString() + " ampere");
            Log.Debug("Channel 3     = " + ((recbuf[(byte)CURRENT.ch3h] * 256 + recbuf[(byte)CURRENT.ch3l]) / 10).ToString() + " ampere");
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Count", recbuf[5].ToString(), pName);
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Channel 1", ((recbuf[(byte)CURRENT.ch1h] * 256 + recbuf[(byte)CURRENT.ch1l]) / 10).ToString(), pName);
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Channel 2", ((recbuf[(byte)CURRENT.ch2h] * 256 + recbuf[(byte)CURRENT.ch2l]) / 10).ToString(), pName);
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Channel 3", ((recbuf[(byte)CURRENT.ch3h] * 256 + recbuf[(byte)CURRENT.ch3l]) / 10).ToString(), pName);

            Log.Debug("Signal level  = " + (recbuf[(byte)CURRENT.rssi] >> 4).ToString());
            if ((recbuf[(byte)CURRENT.battery_level] & 0xf) == 0)
            {
                Log.Debug("Battery       = Low");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
            }
            else
            {
                Log.Debug("Battery       = OK");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
            }
        }

        public void decode_Energy()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New Energy meter found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Energy Meter - " + (recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString(), "", "Energy Meter", "ENERGY METER", (recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString());
            }

            long instant = 0;
            double usage = 0;

            instant = Convert.ToInt64(recbuf[(byte)ENERGY.instant1]) * 0x1000000 + recbuf[(byte)ENERGY.instant2] * 0x10000 + recbuf[(byte)ENERGY.instant3] * 0x100 + recbuf[(byte)ENERGY.instant4];
            usage = (Convert.ToDouble(recbuf[(byte)ENERGY.total1]) * 0x10000000000L + Convert.ToDouble(recbuf[(byte)ENERGY.total2]) * 0x100000000L + Convert.ToDouble(recbuf[(byte)ENERGY.total3]) * 0x1000000 + recbuf[(byte)ENERGY.total4] * 0x10000 + recbuf[(byte)ENERGY.total5] * 0x100 + recbuf[(byte)ENERGY.total6]) / 223.666;

            switch (recbuf[(byte)ENERGY.subtype])
            {
                case (byte)ENERGY.ELEC2:
                    Log.Debug("subtype       = ELEC2 - OWL CM119, CM160");
                    break;
                default:
                    Log.Debug("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)ENERGY.packettype] + ":" + recbuf[(byte)ENERGY.subtype]);
                    break;
            }
            Log.Debug("Sequence nbr  = " + recbuf[(byte)ENERGY.seqnbr].ToString());
            Log.Debug("ID            = " + (recbuf[(byte)ENERGY.id1] * 256 + recbuf[(byte)ENERGY.id2]).ToString());
            Log.Debug("Count         = " + recbuf[(byte)ENERGY.count].ToString());
            Log.Debug("Instant usage = " + instant.ToString() + " Watt");
            Log.Debug("total usage   = " + usage.ToString() + " Wh");
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Count", recbuf[5].ToString(), pName);
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Instant usage", instant.ToString(), pName);
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Total usage", usage.ToString(), pName);

            Log.Debug("Signal level  = " + (recbuf[(byte)ENERGY.rssi] >> 4).ToString());
            if ((recbuf[(byte)ENERGY.battery_level] & 0xf) == 0)
            {
                Log.Debug("Battery       = Low");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "Low", pName);
            }
            else
            {
                Log.Debug("Battery       = OK");
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Battery", "OK", pName);
            }
        }

        public void decode_Gas()
        {
            Log.Debug("Gas Not implemented");
        }

        public void decode_Water()
        {
            Log.Debug("Water Not implemented");
        }

        public void decode_Weight()
        {
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString());
            if (obj == null && OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
            {
                Log.Info("New Scale found.  Adding to OSA");
                OSAEObjectManager.ObjectAdd("Scale Meter - " + (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString(), "", "Scale Meter", "SCALE", (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString(), "", 30, true);
                obj = obj = OSAEObjectManager.GetObjectByAddress((recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString());
            }
            
            switch (recbuf[(byte)WEIGHT.subtype])
            {
                case (byte)WEIGHT.WEIGHT1:
                    Log.Debug("subtype       = BWR102");
                    break;
                case (byte)WEIGHT.WEIGHT2:
                    Log.Debug("subtype       = GR101");
                    break;
                default:
                    Log.Debug("ERROR: Unknown Sub type for Packet type=" + recbuf[(byte)WEIGHT.packettype] + ":" + recbuf[(byte)WEIGHT.subtype]);
                    break;
            }
            Log.Debug("Sequence nbr  = " + recbuf[(byte)WEIGHT.seqnbr].ToString());
            Log.Debug("ID            = " + (recbuf[(byte)WEIGHT.id1] * 256 + recbuf[(byte)WEIGHT.id2]).ToString());
            Log.Debug("Weight        = " + (((recbuf[(byte)WEIGHT.weighthigh] * 25.6) + recbuf[(byte)WEIGHT.weightlow] / 10).ToString() + 2.2).ToString() + " lb");
            Log.Debug("Signal level  = " + (recbuf[(byte)WEIGHT.rssi] >> 4).ToString());

            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Weight", (((recbuf[(byte)WEIGHT.weighthigh] * 25.6) + recbuf[(byte)WEIGHT.weightlow] / 10).ToString() + 2.2).ToString(), pName);
        }

        //public void decode_RFXSensor()
        //{
        //    switch (recbuf(RFXSENSOR.subtype))
        //    {
        //        case RFXSENSOR.Temp:
        //            Log.Debug("subtype       = Temperature");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            Log.Debug("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            //positive temperature?
        //            if ((recbuf(RFXSENSOR.msg1) & 0x80) == 0)
        //            {
        //                Log.Debug("msg           = " + Math.Round(((recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)) / 100), 2).ToString() + " °C");
        //            }
        //            else
        //            {
        //                Log.Debug("msg           = " + Math.Round((0 - ((recbuf(RFXSENSOR.msg1) & 0x7f) * 256 + recbuf(RFXSENSOR.msg2)) / 100), 2).ToString() + " °C");
        //            }
        //            break;
        //        case RFXSENSOR.AD:
        //            Log.Debug("subtype       = A/D");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            Log.Debug("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            Log.Debug("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString() + " mV");
        //            break;
        //        case RFXSENSOR.Volt:
        //            Log.Debug("subtype       = Voltage");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            Log.Debug("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            Log.Debug("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString() + " mV");
        //            break;
        //        case RFXSENSOR.Message:
        //            Log.Debug("subtype       = Message");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXSENSOR.seqnbr).ToString);
        //            Log.Debug("ID            = " + recbuf(RFXSENSOR.id).ToString);
        //            switch (recbuf(RFXSENSOR.msg2))
        //            {
        //                case 0x1:
        //                    Log.Debug("msg           = sensor addresses incremented");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("msg           = battery low detected");
        //                    break;
        //                case 0x81:
        //                    Log.Debug("msg           = no 1-wire device connected");
        //                    break;
        //                case 0x82:
        //                    Log.Debug("msg           = 1-Wire ROM CRC error");
        //                    break;
        //                case 0x83:
        //                    Log.Debug("msg           = 1-Wire device connected is not a DS18B20 or DS2438");
        //                    break;
        //                case 0x84:
        //                    Log.Debug("msg           = no end of read signal received from 1-Wire device");
        //                    break;
        //                case 0x85:
        //                    Log.Debug("msg           = 1-Wire scratchpad CRC error");
        //                    break;
        //                default:
        //                    Log.Debug("ERROR: unknown message");
        //                    break;
        //            }

        //            Log.Debug("msg           = " + (recbuf(RFXSENSOR.msg1) * 256 + recbuf(RFXSENSOR.msg2)).ToString());
        //            break;
        //        default:
        //            Log.Debug("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(RFXSENSOR.packettype)) + ":" + Conversion.Hex(recbuf(RFXSENSOR.subtype)));
        //            break;
        //    }
        //    Log.Debug("Signal level  = " + (recbuf(RFXSENSOR.rssi) >> 4).ToString());

        //}

        //public void decode_RFXMeter()
        //{
        //    long counter = 0;

        //    switch (recbuf(RFXMETER.subtype))
        //    {
        //        case RFXMETER.Count:
        //            Log.Debug("subtype       = RFXMeter counter");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            counter = (Convert.ToInt64(recbuf(RFXMETER.count1)) << 24) + (Convert.ToInt64(recbuf(RFXMETER.count2)) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4);
        //            Log.Debug("Counter       = " + counter.ToString());
        //            Log.Debug("if RFXPwr     = " + (counter / 1000).ToString() + " kWh");
        //            break;
        //        case RFXMETER.Interval:
        //            Log.Debug("subtype       = RFXMeter new interval time set");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            Log.Debug("Interval time = ");
        //            switch (recbuf(RFXMETER.count3))
        //            {
        //                case 0x1:
        //                    Log.Debug("30 seconds");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("1 minute");
        //                    break;
        //                case 0x4:
        //                    Log.Debug("6 minutes");
        //                    break;
        //                case 0x8:
        //                    Log.Debug("12 minutes");
        //                    break;
        //                case 0x10:
        //                    Log.Debug("15 minutes");
        //                    break;
        //                case 0x20:
        //                    Log.Debug("30 minutes");
        //                    break;
        //                case 0x40:
        //                    Log.Debug("45 minutes");
        //                    break;
        //                case 0x80:
        //                    Log.Debug("1 hour");
        //                    break;
        //            }

        //            break;
        //        case RFXMETER.Calib:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    Log.Debug("subtype       = Calibrate mode for channel 1");
        //                    break;
        //                case 0x40:
        //                    Log.Debug("subtype       = Calibrate mode for channel 2");
        //                    break;
        //                case 0x80:
        //                    Log.Debug("subtype       = Calibrate mode for channel 3");
        //                    break;
        //            }
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            counter = ((Convert.ToInt64(recbuf(RFXMETER.count2) & 0x3f) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)) / 1000;
        //            Log.Debug("Calibrate cnt = " + counter.ToString() + " msec");
        //            Log.Debug("RFXPwr        = " + Convert.ToString(Round(1 / ((16 * counter) / (3600000 / 62.5)), 3)) + " kW");
        //            break;
        //        case RFXMETER.Addr:
        //            Log.Debug("subtype       = New address set, push button for next address");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.CounterReset:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    Log.Debug("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 1 will be executed");
        //                    break;
        //                case 0x40:
        //                    Log.Debug("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 2 will be executed");
        //                    break;
        //                case 0x80:
        //                    Log.Debug("subtype       = Push the button for next mode within 5 seconds or else RESET COUNTER channel 3 will be executed");
        //                    break;
        //            }
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.CounterSet:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    Log.Debug("subtype       = Counter channel 1 is reset to zero");
        //                    break;
        //                case 0x40:
        //                    Log.Debug("subtype       = Counter channel 2 is reset to zero");
        //                    break;
        //                case 0x80:
        //                    Log.Debug("subtype       = Counter channel 3 is reset to zero");
        //                    break;
        //            }
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            Log.Debug("Counter       = " + ((Convert.ToInt64(recbuf(RFXMETER.count1)) << 24) + (Convert.ToInt64(recbuf(RFXMETER.count2)) << 16) + (Convert.ToInt64(recbuf(RFXMETER.count3)) << 8) + recbuf(RFXMETER.count4)).ToString());

        //            break;
        //        case RFXMETER.SetInterval:
        //            Log.Debug("subtype       = Push the button for next mode within 5 seconds or else SET INTERVAL MODE will be entered");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.SetCalib:
        //            switch ((recbuf(RFXMETER.count2) & 0xc0))
        //            {
        //                case 0x0:
        //                    Log.Debug("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 1 will be executed");
        //                    break;
        //                case 0x40:
        //                    Log.Debug("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 2 will be executed");
        //                    break;
        //                case 0x80:
        //                    Log.Debug("subtype       = Push the button for next mode within 5 seconds or else CALIBRATION mode for channel 3 will be executed");
        //                    break;
        //            }
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.SetAddr:
        //            Log.Debug("subtype       = Push the button for next mode within 5 seconds or else SET ADDRESS MODE will be entered");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());

        //            break;
        //        case RFXMETER.Ident:
        //            Log.Debug("subtype       = RFXMeter identification");
        //            Log.Debug("Sequence nbr  = " + recbuf(RFXMETER.seqnbr).ToString);
        //            Log.Debug("ID            = " + (recbuf(RFXMETER.id1) * 256 + recbuf(RFXMETER.id2)).ToString());
        //            Log.Debug("FW version    = " + Conversion.Hex(recbuf(RFXMETER.count3)));
        //            Log.Debug("Interval time = ");
        //            switch (recbuf(RFXMETER.count4))
        //            {
        //                case 0x1:
        //                    Log.Debug("30 seconds");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("1 minute");
        //                    break;
        //                case 0x4:
        //                    Log.Debug("6 minutes");
        //                    break;
        //                case 0x8:
        //                    Log.Debug("12 minutes");
        //                    break;
        //                case 0x10:
        //                    Log.Debug("15 minutes");
        //                    break;
        //                case 0x20:
        //                    Log.Debug("30 minutes");
        //                    break;
        //                case 0x40:
        //                    Log.Debug("45 minutes");
        //                    break;
        //                case 0x80:
        //                    Log.Debug("1 hour");
        //                    break;
        //            }
        //            break;
        //        default:
        //            Log.Debug("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(RFXMETER.packettype)) + ":" + Conversion.Hex(recbuf(RFXMETER.subtype)));
        //            break;
        //    }

        //    Log.Debug("Signal level  = " + (recbuf(RFXMETER.rssi) >> 4).ToString());
        //}

        //public void decode_FS20()
        //{
        //    switch (recbuf(FS20.subtype))
        //    {
        //        case FS20.sTypeFS20:
        //            Log.Debug("subtype       = FS20");
        //            Log.Debug("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            Log.Debug("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            Log.Debug("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            Log.Debug("Cmd1          = ");
        //            switch ((recbuf(FS20.cmd1) & 0x1f))
        //            {
        //                case 0x0:
        //                    Log.Debug("Off");
        //                    break;
        //                case 0x1:
        //                    Log.Debug("dim level 1 = 6.25%");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("dim level 2 = 12.5%");
        //                    break;
        //                case 0x3:
        //                    Log.Debug("dim level 3 = 18.75%");
        //                    break;
        //                case 0x4:
        //                    Log.Debug("dim level 4 = 25%");
        //                    break;
        //                case 0x5:
        //                    Log.Debug("dim level 5 = 31.25%");
        //                    break;
        //                case 0x6:
        //                    Log.Debug("dim level 6 = 37.5%");
        //                    break;
        //                case 0x7:
        //                    Log.Debug("dim level 7 = 43.75%");
        //                    break;
        //                case 0x8:
        //                    Log.Debug("dim level 8 = 50%");
        //                    break;
        //                case 0x9:
        //                    Log.Debug("dim level 9 = 56.25%");
        //                    break;
        //                case 0xa:
        //                    Log.Debug("dim level 10 = 62.5%");
        //                    break;
        //                case 0xb:
        //                    Log.Debug("dim level 11 = 68.75%");
        //                    break;
        //                case 0xc:
        //                    Log.Debug("dim level 12 = 75%");
        //                    break;
        //                case 0xd:
        //                    Log.Debug("dim level 13 = 81.25%");
        //                    break;
        //                case 0xe:
        //                    Log.Debug("dim level 14 = 87.5%");
        //                    break;
        //                case 0xf:
        //                    Log.Debug("dim level 15 = 93.75%");
        //                    break;
        //                case 0x10:
        //                    Log.Debug("On (100%)");
        //                    break;
        //                case 0x11:
        //                    Log.Debug("On ( at last dim level set)");
        //                    break;
        //                case 0x12:
        //                    Log.Debug("Toggle between Off and On (last dim level set)");
        //                    break;
        //                case 0x13:
        //                    Log.Debug("Bright one step");
        //                    break;
        //                case 0x14:
        //                    Log.Debug("Dim one step");
        //                    break;
        //                case 0x15:
        //                    Log.Debug("Start dim cycle");
        //                    break;
        //                case 0x16:
        //                    Log.Debug("Program(Timer)");
        //                    break;
        //                case 0x17:
        //                    Log.Debug("Request status from a bidirectional device");
        //                    break;
        //                case 0x18:
        //                    Log.Debug("Off for timer period");
        //                    break;
        //                case 0x19:
        //                    Log.Debug("On (100%) for timer period");
        //                    break;
        //                case 0x1a:
        //                    Log.Debug("On ( at last dim level set) for timer period");
        //                    break;
        //                case 0x1b:
        //                    Log.Debug("Reset");
        //                    break;
        //                default:
        //                    Log.Debug("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                Log.Debug("                command to receiver");
        //            }
        //            else
        //            {
        //                Log.Debug("                response from receiver");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x40) == 0)
        //            {
        //                Log.Debug("                unidirectional command");
        //            }
        //            else
        //            {
        //                Log.Debug("                bidirectional command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x20) == 0)
        //            {
        //                Log.Debug("                additional cmd2 byte not present");
        //            }
        //            else
        //            {
        //                Log.Debug("                additional cmd2 byte present");
        //            }

        //            if ((recbuf(FS20.cmd1) & 0x20) != 0)
        //            {
        //                Log.Debug("Cmd2          = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2));
        //            }

        //            break;
        //        case FS20.sTypeFHT8V:
        //            Log.Debug("subtype       = FHT 8V valve");
        //            Log.Debug("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            Log.Debug("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            Log.Debug("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            Log.Debug("Cmd1          = ");
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                Log.Debug("new command");
        //            }
        //            else
        //            {
        //                Log.Debug("repeated command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x40) == 0)
        //            {
        //                Log.Debug("                unidirectional command");
        //            }
        //            else
        //            {
        //                Log.Debug("                bidirectional command");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x20) == 0)
        //            {
        //                Log.Debug("                additional cmd2 byte not present");
        //            }
        //            else
        //            {
        //                Log.Debug("                additional cmd2 byte present");
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x10) == 0)
        //            {
        //                Log.Debug("                battery empty beep not enabled");
        //            }
        //            else
        //            {
        //                Log.Debug("                enable battery empty beep");
        //            }
        //            switch ((recbuf(FS20.cmd1) & 0xf))
        //            {
        //                case 0x0:
        //                    Log.Debug("                Synchronize now");
        //                    Log.Debug("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0x1:
        //                    Log.Debug("                open valve");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("                close valve");
        //                    break;
        //                case 0x6:
        //                    Log.Debug("                open valve at percentage level");
        //                    Log.Debug("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0x8:
        //                    Log.Debug("                relative offset (cmd2 bit 7=direction, bit 5-0 offset value)");
        //                    break;
        //                case 0xa:
        //                    Log.Debug("                decalcification cycle");
        //                    Log.Debug("Cmd2          = valve position: " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd2)), 2) + " is " + (Convert.ToInt32(recbuf(FS20.cmd2) / 2.55)).ToString() + "%");
        //                    break;
        //                case 0xc:
        //                    Log.Debug("                synchronization active");
        //                    Log.Debug("Cmd2          = count down is " + (recbuf(FS20.cmd2) >> 1).ToString() + " seconds");
        //                    break;
        //                case 0xe:
        //                    Log.Debug("                test, drive valve and produce an audible signal");
        //                    break;
        //                case 0xf:
        //                    Log.Debug("                pair valve (cmd2 bit 7-1 is count down in seconds, bit 0=1)");
        //                    Log.Debug("Cmd2          = count down is " + recbuf(FS20.cmd2) >> 1 + " seconds");
        //                    break;
        //                default:
        //                    Log.Debug("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }

        //            break;
        //        case FS20.sTypeFHT80:
        //            Log.Debug("subtype       = FHT80 door/window sensor");
        //            Log.Debug("Sequence nbr  = " + recbuf(FS20.seqnbr).ToString);
        //            Log.Debug("House code    = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc1)), 2) + VB.Right("0" + Conversion.Hex(recbuf(FS20.hc2)), 2));
        //            Log.Debug("Address       = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.addr)), 2));
        //            Log.Debug("Cmd1          = ");
        //            switch ((recbuf(FS20.cmd1) & 0xf))
        //            {
        //                case 0x1:
        //                    Log.Debug("sensor opened");
        //                    break;
        //                case 0x2:
        //                    Log.Debug("sensor closed");
        //                    break;
        //                case 0xc:
        //                    Log.Debug("synchronization active");
        //                    break;
        //                default:
        //                    Log.Debug("ERROR: Unknown command = " + VB.Right("0" + Conversion.Hex(recbuf(FS20.cmd1)), 2));
        //                    break;
        //            }
        //            if ((recbuf(FS20.cmd1) & 0x80) == 0)
        //            {
        //                Log.Debug("                new command");
        //            }
        //            else
        //            {
        //                Log.Debug("                repeated command");
        //            }

        //            break;
        //        default:
        //            Log.Debug("ERROR: Unknown Sub type for Packet type=" + Conversion.Hex(recbuf(FS20.packettype)) + ":" + Conversion.Hex(recbuf(FS20.subtype)));
        //            break;
        //    }
        //    Log.Debug("Signal level  = " + (recbuf(FS20.rssi) >> 4).ToString());
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

        static byte GetByte(string str)
        {
            return byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
