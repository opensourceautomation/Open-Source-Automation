namespace OSAE.KillAWatt
{
    using System;
    using System.Collections.Generic;

    public class KillAWatt : OSAEPluginBase
    {
        private OSAE.General.OSAELog Log;
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
            Log = new General.OSAELog(pName);

            OSAEObjectType objt = OSAEObjectTypeManager.ObjectTypeLoad("KILLAWATT MODULE");
            OSAEObjectTypeManager.ObjectTypeUpdate(objt.Name, objt.Name, objt.Description, pName, "THING", objt.Owner, objt.SysType, objt.Container, objt.HideRedundant, objt.Tooltip);

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
                if (pc.PacketCount >= intervalLength) interval();
            }
        }

        private void parsePacket(xbeePacket packet)
        {
            lock (thisLock)
            {
                try
                {
                    Log.Debug("Received Packet: " + packet.Address);
                    VREF = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "VREF").Value);
                    double watts = 0;
                    int max = 0, min = 1024, avgv, vpp;

                    if (packet.Checksum % 256 == 255)
                    {
                        int x = 0;
                        for (int i = 0; i < 19; i++)
                        {
                            if (packet.Voltage[i] < min) min = packet.Voltage[i];
                            if (packet.Voltage[i] > max) max = packet.Voltage[i];
                        }

                        avgv = (min + max) / 2;
                        vpp = max - min;
                        Log.Debug("  avgv: " + avgv);

                        for (int i = 0; i < 19; i++)
                        {
                            //osae.AddToLog("voltage[" + i + "]: " + packet.Voltage[i]);
                            //osae.AddToLog("amp[" + i + "]: " + packet.Amp[i]);

                            packet.Voltage[i] = (packet.Voltage[i] - avgv) * 340 / vpp;
                            packet.Amp[i] = (packet.Amp[i] - VREF) / 17;
                            watts = watts + (packet.Voltage[i] * packet.Amp[i]);
                        }

                        watts = Math.Round(watts / 19, 3);

                        if (watts < 0)  watts = 0;
                        try
                        {
                            PowerCollection pc = GetPowerCollection(packet.Address);
                            Log.Debug("  watts: " + watts.ToString());
                            pc.DataWattBuffer = pc.DataWattBuffer + watts;
                            pc.PacketCount = pc.PacketCount + 1;
                            pc.RSSI = packet.RSSI;
                            Log.Debug("  RSSI: " + pc.RSSI.ToString());
                        }

                        catch (Exception ex)
                        { Log.Error("  error updating object statuses!", ex); }
                    }
                    else
                        Log.Debug("  bad checksum");
                }
                catch (Exception ex)
                { Log.Error("- Error parsing packet!", ex); }
            }
        }

        private void interval()
        {
            Log.Debug("--- interval started");
            intervalLength = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Interval").Value);
            foreach(PowerCollection pc in pcList)
            {
                try
                {
                    if (pc.PacketCount >= intervalLength)
                    {
                        
                        string errorStr = OSAEObjectPropertyManager.GetObjectPropertyValue(OSAEObjectManager.GetObjectByAddress("KAW" + pc.Address).Name, "Error Correction").Value;
                        Log.Debug("  errorStr: " + errorStr); 
                        Double errorCorrection = 0, currentWatts;
                        System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.AllowTrailingSign | System.Globalization.NumberStyles.Float;

                        try
                        {
                            if (errorStr.Length > 1) errorCorrection = Double.Parse(errorStr, styles);
                        }
                        catch
                        { }
                       
                        currentWatts = pc.GetInterval() + errorCorrection;
                        if (currentWatts < 0) currentWatts = 0;

                        Log.Debug("  device address: " + pc.Address);
                        Log.Debug("  dataWattBuffer: " + pc.DataWattBuffer);
                        Log.Debug("  dataWattCount: " + pc.PacketCount);
                        Log.Debug("  errorStr: " + errorStr);
                        OSAEObjectPropertyManager.ObjectPropertySet(OSAEObjectManager.GetObjectByAddress("KAW" + pc.Address).Name, "RSSI", pc.RSSI.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(OSAEObjectManager.GetObjectByAddress("KAW" + pc.Address).Name, "Current Watts", currentWatts.ToString(), pName);
                        pc.DataWattBuffer = 0;
                        pc.PacketCount = 0;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("  error inserting dat!", ex);
                    break;
                }
            }
            //addToLog("--- interval ended");
        }
       
        private PowerCollection GetPowerCollection(int address)
        {
            foreach (PowerCollection pc in pcList)
            {
                if (pc.Address == address) return pc;
            }
            pcList.Add(new PowerCollection(address));
            if(OSAEObjectManager.GetObjectByAddress("KAW" + address.ToString()) == null)
                OSAEObjectManager.ObjectAdd("KillAWatt - " + address.ToString(),"", "Kill-A-Watt device", "KILLAWATT MODULE", "KAW" + address.ToString(), "", 50, true);

            return GetPowerCollection(address);
        }
    }
}
