namespace OSAE.Bluetooth
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Timers;
    using InTheHand.Net;
    using InTheHand.Net.Bluetooth;
    using InTheHand.Net.Sockets;

    public class Bluetooth : OSAEPluginBase
    {
        #region OSAEPlugin Members

        private OSAE.General.OSAELog Log = new General.OSAELog();
        System.Timers.Timer Clock;
        private Thread search_thread;
        string gAppName;
        bool gDebug = false;

        BluetoothClient localClient = new BluetoothClient();

        public override void ProcessCommand(OSAEMethod method)
        {
            //This plugin does not have commands
        }

        public override void RunInterface(string pluginName)
        {
            this.Log.Info("*** Running Interface! ***");
            gAppName = pluginName;
            if (OSAEObjectManager.ObjectExists(gAppName))
               Log.Info("Found Bluetooth Client's Object (" + gAppName + ")");

            try
            {
                gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value);
            }
            catch
            {
                Log.Error("I think the Debug property is missing from the Speech object type!");
            }
            Log.Info("Debug Mode Set to " + gDebug);

            int iScanInterval = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Scan Interval").Value);
            Log.Info("Scan Interval Set to " + iScanInterval);

            Clock = new System.Timers.Timer();
            Clock.Interval = iScanInterval * 1000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);

            this.search_thread = new Thread(new ThreadStart(search));
            this.search_thread.Start();
            Log.Info("Bluetooth Plugin is now scanning for devices.");
        }

        public override void Shutdown()
        {
            Clock.Stop();
            Log.Info("*** Shutting Down ***");
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
            Log.Debug("Search beginning");
            try
            {
                if (OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Learning Mode").Value == "TRUE")
                {
                    // client is used to manage connections
                    localClient = new BluetoothClient();
                    localClient.InquiryLength = new TimeSpan(0, 0, 0, int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Discover Length").Value), 0);
                    // component is used to manage device discovery
                    BluetoothComponent localComponent = new BluetoothComponent(localClient);
                    // async methods, can be done synchronously too
                    localComponent.DiscoverDevicesAsync(255, true, true, true, false, null);
                    localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesProgress);
                    localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesComplete);
                }

                OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("BLUETOOTH DEVICE");

                int discarded;
                byte tmp;
                BluetoothAddress ba;
                BluetoothDeviceInfo bdi;
                foreach (OSAEObject obj in objects)
                {
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

                    if (gDebug) Log.Debug(obj.Name + " (" + address + ") - Attempt connection");
                    Guid uuid = new Guid("{F13F471D-47CB-41d6-9409-BAD0690BF891}");
                    try
                    {
                        ServiceRecord[] records = bdi.GetServiceRecords(uuid);
                        Log.Debug(obj.Name + " (" + address + ") in Range");
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", gAppName);
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(obj.Name + " (" + address + ") NOT in Range");
                        //OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", gAppName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error searching for devices", ex);
            }
        }

        private void component_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            if (gDebug) Log.Debug("DiscoverDevicesEventArgs triggered");
            // log and save all found devices
            for (int i = 0; i < e.Devices.Length; i++)
            {
                if (e.Devices[i].Remembered)
                {

                }
                else
                {
                    if (gDebug) Log.Debug(e.Devices[i].DeviceName + " (" + e.Devices[i].DeviceAddress.ToString() + "): Device is unknown");
                    string addr = e.Devices[i].DeviceAddress.ToString();
                    Object obj = OSAEObjectManager.GetObjectByAddress(addr);
                    if (obj == null)
                    {
                        Log.Debug(e.Devices[i].DeviceName + " (" + e.Devices[i].DeviceAddress.ToString() + "): Adding device to OSA");
                        OSAEObjectManager.ObjectAdd(e.Devices[i].DeviceName, e.Devices[i].DeviceName, e.Devices[i].DeviceName, "BLUETOOTH DEVICE", e.Devices[i].DeviceAddress.ToString(), string.Empty, true);
                        OSAEObjectPropertyManager.ObjectPropertySet(e.Devices[i].DeviceName, "Discover Type", "0", gAppName);
                        if (gDebug) Log.Debug(addr + " - " + e.Devices[i].DeviceName + ": added to OSA");
                    }
                }
            }
        }

        private void component_DiscoverDevicesComplete(object sender, DiscoverDevicesEventArgs e)
        {
            // Discovery Complete
            Log.Debug("Search complete");

        }
        

        #endregion
    }

    public class HexEncoding
    {
        public HexEncoding()
        {
            // TODO: Add constructor logic here
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
                numHexChars--;

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
                hex = new String(new char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }

        public static string ToString(byte[] bytes)
        {
            string hexString = string.Empty;
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
        public static bool IsHexDigit(char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = char.ToUpper(c);
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
