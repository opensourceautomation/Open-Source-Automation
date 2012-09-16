using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE.KillAWatt
{
    class PowerCollection
    {
        private int _address;
        public int Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private double _dataWattBuffer;
        public double DataWattBuffer
        {
            get { return _dataWattBuffer; }
            set { _dataWattBuffer = value; }
        }

        private int _packetCount;
        public int PacketCount
        {
            get { return _packetCount; }
            set { _packetCount = value; }
        }

        private int _rssi;
        public int RSSI
        {
            get { return _rssi; }
            set { _rssi = value; }
        }

        public PowerCollection(int address)
        {
            _address = address;
        }

        public double GetInterval()
        {
            return Math.Round((_dataWattBuffer / _packetCount), 3);
        }
    }
}
