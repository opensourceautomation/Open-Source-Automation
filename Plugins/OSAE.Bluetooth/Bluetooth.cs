using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace OSAE.Bluetooth
{
    public class Bluetooth : OSAEPluginBase
    {
        #region OSAEPlugin Members

        OSAE osae = new OSAE("Bluetooth");
        BluetoothClient bc;
        BluetoothDeviceInfo[] nearosaeDevices;
        System.Timers.Timer Clock;
        private Thread search_thread;
        string pName;

        public override void ProcessCommand(OSAEMethod method)
        {
            //This plugin does not have commands
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            osae.AddToLog("Running Interface!", true);
            Clock = new System.Timers.Timer();
            Clock.Interval = Int32.Parse(osae.GetObjectPropertyValue(pName, "Scan Interval").Value) * 1000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);

            this.search_thread = new Thread(new ThreadStart(search));
            this.search_thread.Start();
        }

        public override void Shutdown()
        {
            Clock.Stop();
            osae.AddToLog("Shutting Down", true);
        }


        #endregion

        #region Plugin Specific Code

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            if (sender == Clock)
            {
                if (!search_thread.IsAlive)
                {
                    this.search_thread = new Thread(new ThreadStart(search));
                    this.search_thread.Start();
                }
            }

        }

        private void search()
        {
            Guid uuid = BluetoothService.L2CapProtocol;
            BluetoothDeviceInfo bdi;
            BluetoothAddress ba;
            byte tmp;
            bool found = false;
            int discarded;

            bc = new BluetoothClient();

            bc.InquiryLength = new TimeSpan(0, 0, 0, Int32.Parse(osae.GetObjectPropertyValue(pName, "Discover Length").Value), 0);
            nearosaeDevices = bc.DiscoverDevices(10, false, false, true);

            for (int j = 0; j < nearosaeDevices.Length; j++)
            {
                string addr = nearosaeDevices[j].DeviceAddress.ToString();

                Object obj = osae.GetObjectByAddress(addr);

                if (obj == null)
                {
                    if (osae.GetObjectPropertyValue(pName, "Learning Mode").Value == "TRUE")
                    {
                        osae.ObjectAdd(nearosaeDevices[j].DeviceName, nearosaeDevices[j].DeviceName, "BLUETOOTH DEVICE", nearosaeDevices[j].DeviceAddress.ToString(),"",true);
                        osae.ObjectPropertySet(nearosaeDevices[j].DeviceName, "Discover Type", "0");
                        osae.AddToLog(addr + " - " + nearosaeDevices[j].DeviceName + ": added to OSA", true);
                    }
                }
            }

            List<OSAEObject> objects = osae.GetObjectsByType("BLUETOOTH DEVICE");

            foreach (OSAEObject obj in objects)
            {
                found = false;
                string address = obj.Address;
                byte[] byteArray = HexEncoding.GetBytes(address, out discarded);
                tmp = byteArray[0];
                byteArray[0] = byteArray[5];
                byteArray[5] = tmp;
                tmp = byteArray[1];
                byteArray[1] = byteArray[4];
                byteArray[4] = tmp;
                tmp = byteArray[2];
                byteArray[2] = byteArray[3];
                byteArray[3] = tmp;
                ba = new BluetoothAddress(byteArray);
                bdi = new BluetoothDeviceInfo(ba);
                osae.AddToLog("begin search for " + address, false);

                for (int j = 0; j < nearosaeDevices.Length; j++)
                {
                    if (nearosaeDevices[j].DeviceAddress.ToString() == address)
                    {
                        found = true;
                        osae.AddToLog(address + " - " + obj.Name + ": found with DiscoverDevices", false);
                    }
                }
                if (!found)
                {
                    osae.AddToLog(address + " - " + obj.Name + ": failed with DiscoverDevices", false);

                }

                try
                {
                    if (!found && (Int32.Parse(osae.GetObjectPropertyValue(obj.Name, "Discover Type").Value) == 2 || Int32.Parse(osae.GetObjectPropertyValue(obj.Name, "Discover Type").Value) == 0))
                    {
                        osae.AddToLog(address + " - " + obj.Name + ": attempting GetServiceRecords", false);

                        bdi.GetServiceRecords(uuid);
                        found = true;
                        osae.AddToLog(address + " - " + obj.Name + " found with GetServiceRecords", false);

                    }
                }
                catch (Exception ex)
                {
                    osae.AddToLog(address + " - " + obj.Name + " failed GetServiceRecords. exception: " + ex.Message, false);

                }

                try
                {
                    if (!found && (Int32.Parse(osae.GetObjectPropertyValue(obj.Name, "Discover Type").Value) == 3 || Int32.Parse(osae.GetObjectPropertyValue(obj.Name, "Discover Type").Value) == 0))
                    {
                        osae.AddToLog(address + " - " + obj.Name + ": attempting Connection", false);
                        //attempt a connect
                        BluetoothEndPoint ep;
                        ep = new BluetoothEndPoint(bdi.DeviceAddress, BluetoothService.Handsfree);
                        //MessageBox.Show("attempt connect: " + pairedDevices[i].DeviceAddress);
                        bc.Connect(ep);
                        osae.AddToLog(address + " - " + obj.Name + " found with Connect attempt", false);
                        bc.Close();
                        found = true;
                    }
                }
                catch (Exception ex)
                {
                    osae.AddToLog(address + " - " + obj.Name + " failed with Connect attempt. exception: " + ex.Message, false);
                }
                


                if (found)
                {
                    osae.ObjectStateSet(obj.Name, "ON");
                    osae.AddToLog("Status Updated in osae", false);
                }
                else
                {
                    osae.ObjectStateSet(obj.Name, "OFF");
                    osae.AddToLog("Status Updated in osae", false);
                }

            }
        }

       

       

        #endregion
    }

    public class HexEncoding
    {
        public HexEncoding()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static int GetByteCount(string hexString)
        {
            int numHexChars = 0;
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    numHexChars++;
            }
            // if odd number of characters, discard last character
            if (numHexChars % 2 != 0)
            {
                numHexChars--;
            }
            return numHexChars / 2; // 2 characters per byte
        }
        /// <summary>
        /// Creates a byte array from the hexadecimal string. Each two characters are combined
        /// to create one byte. First two hexadecimal characters become first byte in returned array.
        /// Non-hexadecimal characters are ignored. 
        /// </summary>
        /// <param name="hexString">string to convert to byte array</param>
        /// <param name="discarded">number of characters in string ignored</param>
        /// <returns>byte array, in the same left-to-right order as the hexString</returns>
        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }
        public static string ToString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }
        /// <summary>
        /// Determines if given string is in proper hexadecimal string format
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static bool InHexFormat(string hexString)
        {
            bool hexFormat = true;

            foreach (char digit in hexString)
            {
                if (!IsHexDigit(digit))
                {
                    hexFormat = false;
                    break;
                }
            }
            return hexFormat;
        }

        /// <summary>
        /// Returns true is c is a hexadecimal digit (A-F, a-f, 0-9)
        /// </summary>
        /// <param name="c">Character to test</param>
        /// <returns>true if hex digit, false if not</returns>
        public static bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }
        /// <summary>
        /// Converts 1 or 2 character string into equivalant byte value
        /// </summary>
        /// <param name="hex">1 or 2 character string</param>
        /// <returns>byte</returns>
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }


    }
}
