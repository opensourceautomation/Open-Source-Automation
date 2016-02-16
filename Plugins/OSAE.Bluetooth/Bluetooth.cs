namespace OSAE.Bluetooth
{
    using System;
    using System.Threading;
    using System.Timers;
    using InTheHand.Net;
    using InTheHand.Net.Bluetooth;
    using InTheHand.Net.Sockets;
    using System.Collections.Generic;

    public class Bluetooth : OSAEPluginBase
    {
        private OSAE.General.OSAELog Log;
        BluetoothClient bc;
        BluetoothDeviceInfo[] nearosaeDevices;
        System.Timers.Timer Clock;
        private Thread search_thread;
        string gAppName;
        bool gDebug = false;

        public override void ProcessCommand(OSAEMethod method)
        {
            //This plugin does not have commands
        }

        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            Log = new General.OSAELog(gAppName);
            Log.Info("*** Running Interface! ***");
            OwnTypes();

            try
            {
                gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value);
            }
            catch
            { Log.Error("I think the Debug property is missing from the Speech object type!"); }

            Log.Info("Debug Mode Set to " + gDebug);

            int iScanInterval = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Scan Interval").Value);
            Log.Info("Scan Interval Set to " + iScanInterval);

            Clock = new System.Timers.Timer();
            Clock.Interval = iScanInterval * 1000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);

            search_thread = new Thread(new ThreadStart(search));
            search_thread.Start();
            Log.Info("Bluetooth Plugin is now scanning for devices.");
        }

        public override void Shutdown()
        {
            Clock.Stop();
            search_thread.Abort();
            Log.Info("*** Shutting Down ***");
        }
       
        #region Plugin Specific Code

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            if (sender == Clock)
            {
                if (!search_thread.IsAlive)
                {
                    search_thread = new Thread(new ThreadStart(search));
                    search_thread.Start();
                }
            }
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("BLUETOOTH");

            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, gAppName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant);
                Log.Info("Bluetooth Plugin took ownership of the BLUETOOTH Object Type.");
            }
            else
                Log.Info("Bluetooth Plugin correctly owns the BLUETOOTH Object Type.");
        }

        private void search()
        {
            try
            {
                Guid uuid = BluetoothService.L2CapProtocol;
                BluetoothDeviceInfo bdi;
                BluetoothAddress ba;
                byte tmp;
                bool found = false;
                int discarded;

                try
                {
                    bc = new BluetoothClient();
                }
                catch 
                {
                    Log.Error("No Bluetooth Adapters found!");
                    OSAEMethodManager.MethodQueueAdd(gAppName, "OFF","","",gAppName);
                    return;
                }

                bc.InquiryLength = new TimeSpan(0, 0, 0, int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Discover Length").Value), 0);
                nearosaeDevices = bc.DiscoverDevices(10, false, false, true);

                for (int j = 0; j < nearosaeDevices.Length; j++)
                {
                    string addr = nearosaeDevices[j].DeviceAddress.ToString();
                    Object obj = OSAEObjectManager.GetObjectByAddress(addr);

                    if (obj == null)
                    {
                        if (OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Learning Mode").Value == "TRUE")
                        {
                            OSAEObjectManager.ObjectAdd(nearosaeDevices[j].DeviceName, "", nearosaeDevices[j].DeviceName, "BLUETOOTH DEVICE", nearosaeDevices[j].DeviceAddress.ToString(), string.Empty, 50, true);
                            OSAEObjectPropertyManager.ObjectPropertySet(nearosaeDevices[j].DeviceName, "Discover Type", "0", gAppName);
                            if (gDebug) Log.Debug(addr + " - " + nearosaeDevices[j].DeviceName + ": added to OSA");
                        }
                    }
                }

                OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("BLUETOOTH DEVICE");

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
                    //if (gDebug) Log.Debug("Begin search for " + address);

                    for (int j = 0; j < nearosaeDevices.Length; j++)
                    {
                        if (nearosaeDevices[j].DeviceAddress.ToString() == address)
                        {
                            found = true;
                            if (gDebug) Log.Debug(address + " - " + obj.Name + ": found with DiscoverDevices");
                        }
                    }
                    if (!found)
                        if (gDebug) Log.Debug(address + " - " + obj.Name + ": failed with DiscoverDevices");

                    try
                    {
                        if (!found && (int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Discover Type").Value) == 2 || Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Discover Type").Value) == 0))
                        {
                            if (gDebug) Log.Debug(address + " - " + obj.Name + ": attempting GetServiceRecords");

                            bdi.GetServiceRecords(uuid);
                            found = true;
                            if (gDebug) Log.Debug(address + " - " + obj.Name + " found with GetServiceRecords");
                        }
                    }
                    catch (Exception ex)
                    { if (gDebug) Log.Debug(address + " - " + obj.Name + " failed GetServiceRecords. exception: " + ex.Message); }

                    try
                    {
                        if (!found && (int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Discover Type").Value) == 3 || int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Discover Type").Value) == 0))
                        {
                            if (gDebug) Log.Debug(address + " - " + obj.Name + ": attempting Connection");
                            //attempt a connect
                            BluetoothEndPoint ep;
                            ep = new BluetoothEndPoint(bdi.DeviceAddress, BluetoothService.Handsfree);
                            //MessageBox.Show("attempt connect: " + pairedDevices[i].DeviceAddress);
                            bc.Connect(ep);
                            if (gDebug) Log.Debug(address + " - " + obj.Name + " found with Connect attempt");
                            bc.Close();
                            found = true;
                        }
                    }
                    catch (Exception ex)
                    { Log.Error(address + " - " + obj.Name + " failed with Connect attempt. exception: " + ex.Message); }

                    if (found)
                    {
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", gAppName);
                        if (gDebug) Log.Debug(obj.Name + " Status Updated in osae");
                    }
                    else
                    {
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", gAppName);
                        if (gDebug) Log.Debug(obj.Name + " Status Updated in osae");
                    }
                }
            }
            catch (Exception ex)
            { Log.Error("Error searching for devices", ex); }
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
