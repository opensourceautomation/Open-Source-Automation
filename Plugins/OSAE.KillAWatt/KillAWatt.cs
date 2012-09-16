using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Collections.Generic;
using System.AddIn;
using OpenSourceAutomation;

namespace OSAE.KillAWatt
{
    [AddIn("KillAWatt", Version = "0.3.7")]
    public class KillAWatt : IOpenSourceAutomationAddIn
    {
        OSAE osae = new OSAE("KillAWatt");
        int portNumber = 0;
        string pName = "";
        xbee xb;
        List<xbeePacket> xbl = new List<xbeePacket>();
        int packetCount = 0;
        List<PowerCollection> pcList = new List<PowerCollection>();
        int VREF = 489;
        int intervalLength = 60;
        Object thisLock = new Object();


        public void ProcessCommand(System.Data.DataTable row)
        {
            // No commands
        }

        public void RunInterface(string pluginName)
        {
            pName = pluginName;
            osae.ObjectTypeUpdate("KILLAWATT MODULE", "KILLAWATT MODULE", "Kill-A-Watt Module", pName, "KILLAWATT MODULE", 0, 0, 0, 1);

            xb = new xbee(Int32.Parse(osae.GetObjectProperty(pName, "Port")));
            xb.xbeePacketReceived += new xbee.xbeePacketReceivedEventHandler(xb_xbeePacketReceived);
        }

        public void Shutdown()
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
                    osae.AddToLog("Received Packet: " + packet.Address, false);
                    VREF = Int32.Parse(osae.GetObjectProperty(pName, "VREF"));
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
                        osae.AddToLog("  avgv: " + avgv, false);

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
                            osae.AddToLog("  watts: " + watts.ToString(), false);
                            pc.DataWattBuffer = pc.DataWattBuffer + watts;
                            pc.PacketCount = pc.PacketCount + 1;
                            pc.RSSI = packet.RSSI;
                            osae.AddToLog("  RSSI: " + pc.RSSI.ToString(), false);
                        }

                        catch (Exception ex)
                        {
                            osae.AddToLog("  error updating object statuses: " + ex.ToString(), false);
                        }
                    }
                    else
                    {
                        osae.AddToLog("  bad checksum", false);
                    }
                    //addToLog("- parse packet ended");
                }
                catch (Exception ex)
                {
                    osae.AddToLog("- Error parsing packet:" + ex.Message, true);
                }
            }
        }

        private void interval()
        {
            osae.AddToLog("--- interval started", false);
            intervalLength = Int32.Parse(osae.GetObjectPropertyValue(pName, "Interval").Value);
            foreach(PowerCollection pc in pcList)
            {
                try
                {
                    if (pc.PacketCount >= intervalLength)
                    {
                        
                        string errorStr = osae.GetObjectPropertyValue(osae.GetObjectByAddress("KAW" + pc.Address).Name, "Error Correction").Value;
                        osae.AddToLog("  errorStr: " + errorStr, false); 
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

                        osae.AddToLog("  device address: " + pc.Address, false);
                        osae.AddToLog("  dataWattBuffer: " + pc.DataWattBuffer, false);
                        osae.AddToLog("  dataWattCount: " + pc.PacketCount, false);
                        osae.AddToLog("  errorStr: " + errorStr, false);
                        osae.ObjectPropertySet(osae.GetObjectByAddress("KAW" + pc.Address).Name, "RSSI", pc.RSSI.ToString());
                        osae.ObjectPropertySet(osae.GetObjectByAddress("KAW" + pc.Address).Name, "Current Watts", currentWatts.ToString());
                        pc.DataWattBuffer = 0;
                        pc.PacketCount = 0;
                    }
                }
                catch (Exception ex)
                {
                    osae.AddToLog("  error inserting data: " + ex.ToString(), false);
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
            if(osae.GetObjectByAddress("KAW" + address.ToString()) == null)
                osae.ObjectAdd("KillAWatt - " + address.ToString(), "Kill-A-Watt device", "KILLAWATT MODULE", "KAW" + address.ToString(), "", true);

            return GetPowerCollection(address);
        }

    }
}
