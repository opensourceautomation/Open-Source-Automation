namespace OSAE.KillAWatt
{
    using System;
    using System.Collections.Generic;

    public class KillAWatt : OSAEPluginBase
    {
        Logging logging = Logging.GetLogger("KillAWatt");
        string pName = string.Empty;
        xbee xb;
        List<xbeePacket> xbl = new List<xbeePacket>();
        int packetCount = 0;
        List<PowerCollection> pcList = new List<PowerCollection>();
        int VREF = 489;
        int intervalLength = 60;
        Object thisLock = new Object();


        public override void ProcessCommand(OSAEMethod method)
        {
            // No commands
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            OSAEObjectTypeManager.ObjectTypeUpdate("KILLAWATT MODULE", "KILLAWATT MODULE", "Kill-A-Watt Module", pName, "KILLAWATT MODULE", false, false, false, true);

            xb = new xbee(Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value));
            xb.xbeePacketReceived += new xbee.xbeePacketReceivedEventHandler(xb_xbeePacketReceived);
        }

        public override void Shutdown()
        {
            // Nothing to clean up
        }

        private void xb_xbeePacketReceived(xbeePacket xbp)
        {
            parsePacket(xbp);
            packetCount++;
            foreach (PowerCollection pc in pcList)
            {
                if (pc.PacketCount >= intervalLength)
                    interval();
            }
        }

        private void parsePacket(xbeePacket packet)
        {
            lock (thisLock)
            {
                try
                {
                    logging.AddToLog("Received Packet: " + packet.Address, false);
                    VREF = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "VREF").Value);
                    double watts = 0;
                    int max = 0;
                    int min = 1024;
                    int avgv, vpp;

                    if (packet.Checksum % 256 == 255)
                    {
                        int x = 0;
                        for (int i = 0; i < 19; i++)
                        {
                            if (packet.Voltage[i] < min)
                                min = packet.Voltage[i];
                            if (packet.Voltage[i] > max)
                                max = packet.Voltage[i];
                        }

                        avgv = (min + max) / 2;
                        vpp = max - min;
                        logging.AddToLog("  avgv: " + avgv, false);

                        for (int i = 0; i < 19; i++)
                        {
                            //osae.AddToLog("voltage[" + i + "]: " + packet.Voltage[i]);
                            //osae.AddToLog("amp[" + i + "]: " + packet.Amp[i]);

                            packet.Voltage[i] = (packet.Voltage[i] - avgv) * 340 / vpp;
                            packet.Amp[i] = (packet.Amp[i] - VREF) / 17;
                            watts = watts + (packet.Voltage[i] * packet.Amp[i]);
                        }

                        watts = Math.Round(watts / 19, 3);


                        if (watts < 0)
                            watts = 0;
                        try
                        {
                            PowerCollection pc = GetPowerCollection(packet.Address);
                            logging.AddToLog("  watts: " + watts.ToString(), false);
                            pc.DataWattBuffer = pc.DataWattBuffer + watts;
                            pc.PacketCount = pc.PacketCount + 1;
                            pc.RSSI = packet.RSSI;
                            logging.AddToLog("  RSSI: " + pc.RSSI.ToString(), false);
                        }

                        catch (Exception ex)
                        {
                            logging.AddToLog("  error updating object statuses: " + ex.ToString(), false);
                        }
                    }
                    else
                    {
                        logging.AddToLog("  bad checksum", false);
                    }
                    //addToLog("- parse packet ended");
                }
                catch (Exception ex)
                {
                    logging.AddToLog("- Error parsing packet:" + ex.Message, true);
                }
            }
        }

        private void interval()
        {
            logging.AddToLog("--- interval started", false);
            intervalLength = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Interval").Value);
            foreach(PowerCollection pc in pcList)
            {
                try
                {
                    if (pc.PacketCount >= intervalLength)
                    {
                        
                        string errorStr = OSAEObjectPropertyManager.GetObjectPropertyValue(OSAEObjectManager.GetObjectByAddress("KAW" + pc.Address).Name, "Error Correction").Value;
                        logging.AddToLog("  errorStr: " + errorStr, false); 
                        Double errorCorrection = 0, currentWatts;
                        System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.AllowTrailingSign | System.Globalization.NumberStyles.Float;

                        try
                        {
                            if (errorStr.Length > 1)
                                errorCorrection = Double.Parse(errorStr, styles);
                        }
                        catch
                        { }
                       
                        currentWatts = pc.GetInterval() + errorCorrection;
                        if (currentWatts < 0)
                            currentWatts = 0;

                        logging.AddToLog("  device address: " + pc.Address, false);
                        logging.AddToLog("  dataWattBuffer: " + pc.DataWattBuffer, false);
                        logging.AddToLog("  dataWattCount: " + pc.PacketCount, false);
                        logging.AddToLog("  errorStr: " + errorStr, false);
                        OSAEObjectPropertyManager.ObjectPropertySet(OSAEObjectManager.GetObjectByAddress("KAW" + pc.Address).Name, "RSSI", pc.RSSI.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(OSAEObjectManager.GetObjectByAddress("KAW" + pc.Address).Name, "Current Watts", currentWatts.ToString(), pName);
                        pc.DataWattBuffer = 0;
                        pc.PacketCount = 0;
                    }
                }
                catch (Exception ex)
                {
                    logging.AddToLog("  error inserting data: " + ex.ToString(), false);
                    break;
                }
            }
            
            //addToLog("--- interval ended");

        }
       
        private PowerCollection GetPowerCollection(int address)
        {
            foreach (PowerCollection pc in pcList)
            {
                if (pc.Address == address)
                    return pc;
            }
            pcList.Add(new PowerCollection(address));
            if(OSAEObjectManager.GetObjectByAddress("KAW" + address.ToString()) == null)
                OSAEObjectManager.ObjectAdd("KillAWatt - " + address.ToString(),"", "Kill-A-Watt device", "KILLAWATT MODULE", "KAW" + address.ToString(), "", true);

            return GetPowerCollection(address);
        }

    }
}
