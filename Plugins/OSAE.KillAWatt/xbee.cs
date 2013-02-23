using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;

namespace OSAE.KillAWatt
{
    class xbee
    {
        private SerialPort _port = new SerialPort();
        Queue buffer = new Queue();
        byte[] gpacket = new byte[88];
        List<PowerCollection> pcList = new List<PowerCollection>();
        int VREF = 498;

        public delegate void xbeePacketReceivedEventHandler(xbeePacket xbp);
        public event xbeePacketReceivedEventHandler xbeePacketReceived;

        public xbee(int port)
        {
            _port.BaudRate = 9600;
            _port.PortName = "COM" + port.ToString();
            _port.ReceivedBytesThreshold = 88;
            _port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            _port.Open();
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            int len = _port.BytesToRead;
            //addToLog("data received: " + len);
            for (int i = 0; i < len; i++)
            {
                buffer.Enqueue(_port.ReadByte());
            }

            if (buffer.Count >= 88)
                fetch();   
        }

        private void fetch()
        {
            xbeePacket xbp = new xbeePacket();
           
            if (buffer.Count > 0)
            {
                xbp = getPacket();
                if (xbp.Address > 0)
                {
                    if (this.xbeePacketReceived != null)
                        this.xbeePacketReceived(xbp);
                }
            }
        }

        private xbeePacket getPacket()
        {
            xbeePacket xbp = new xbeePacket();
            //addToLog("-- get packet started.  buffer.count: " + buffer.Count);
            int i = 0;
            int[] packet = new int[88];
            //addToLog("getting packet...");
            try
            {
                while (i < 88)
                {
                    //addToLog("i: " + i + " | buffer.count: " + buffer.Count);
                    if (buffer.Count > 0)
                    {
                        packet[i] = (int)buffer.Dequeue();
                        if (i == 0 && packet[i] != 126)
                        {
                            //osae.AddToLog("bad first byte.  i remains 0");

                        }
                        else
                            i = i + 1;
                    }
                }
                //addToLog("packet found");
            }
            catch (Exception ex)
            {
                //osae.AddToLog("error getting packet");
                packet[0] = 0;
            }

            xbp.Address = (packet[4] * 256) + packet[5];
            xbp.RSSI = packet[6];
            xbp.Samples = packet[8];
            for (int k = 3; k < 88; k++)
            {
                xbp.Checksum = xbp.Checksum + packet[k];
            }
            int x = 0;
            for (int j = 11; j < 87; j = j + 4)
            {
                xbp.Voltage[x] = packet[j] * 256 + packet[j + 1];
                xbp.Amp[x] = packet[j + 2] * 256 + packet[j + 3];

                x = x + 1;
            }
            
            return xbp;
        }

    }

    class xbeePacket
    {
        private int _address;
        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private int[] _voltage;
        public int[] Voltage
        {
            get { return _voltage; }
            set { _voltage = value; }
        }

        private double[] _amp;
        public double[] Amp
        {
            get { return _amp; }
            set { _amp = value; }
        }

        private List<int[]> _digital;
        public List<int[]> Digital
        {
            get { return _digital; }
            set { _digital = value; }
        }

        private List<int[]> _analog;
        public List<int[]> Analog
        {
            get { return _analog; }
            set { _analog = value; }
        }

        private int _rssi;
        public int RSSI
        {
            get { return _rssi; }
            set { _rssi = value; }
        }

        private int _checksum;
        public int Checksum
        {
            get { return _checksum; }
            set { _checksum = value; }
        }

        private int _samples;
        public int Samples
        {
            get { return _samples; }
            set { _samples = value; }
        }

        public xbeePacket()
        {
            _voltage = new int[19];
            _amp = new double[19];

            _analog = new List<int[]>();
            _digital = new List<int[]>();
        }
    }
}
