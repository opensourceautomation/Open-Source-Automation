using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Runtime.InteropServices;

using com.dalsemi.onewire;
using com.dalsemi.onewire.adapter;
using com.dalsemi.onewire.container;
using com.dalsemi.onewire.utils;

namespace OSAE.OneWire
{
    public class OneWire : OSAEPluginBase
    {
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        string pName;
        string uom;
        OSAE osae = new OSAE("1Wire");
        private DSPortAdapter adapter = null;
        System.Threading.Timer Clock;
        Object thisLock = new Object();
        int threadCount = 0;
        int abortCount = 0;
        Boolean restarting = false;


        public override void ProcessCommand(OSAEMethod method)
        {
            //throw new NotImplementedException();
        }

        public override void RunInterface(string pluginName)
        {
            osae.AddToLog("Loading (0.2.4)...", true);
            osae.AddToLog("Current Environment Version: " + Environment.Version.Major.ToString(), true);
            if (Environment.Version.Major >= 4)
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework64\v2.0.50727");
                if (!Directory.Exists(folder))
                    folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v2.0.50727");
                osae.AddToLog("vjsnativ.dll path: " + folder, false);
                folder = Path.GetFullPath(folder);
                LoadLibrary(Path.Combine(folder, "vjsnativ.dll"));
            }
            pName = pluginName;
            string adapterProp = osae.GetObjectPropertyValue(pName, "Adapter").Value;
            string port = osae.GetObjectPropertyValue(pName, "Port").Value;
            uom = osae.GetObjectPropertyValue(pName, "Unit of Measure").Value;
            osae.AddToLog("adapterProp: " + adapterProp, false);
            osae.AddToLog("port: " + port, false);
            osae.AddToLog("uom: " + uom, false);
            try
            {
                adapter = OneWireAccessProvider.getAdapter(adapterProp, "COM" + port);
            }
            catch (Exception ex)
            {
                osae.AddToLog("Failed to GetAdapter - " + ex.Message + " - " + ex.InnerException.Message, true);
            }


            if (adapter.adapterDetected())
            {
                osae.AddToLog("Adapter Detected", true);
                adapter.setSearchAllDevices();

                Clock = new System.Threading.Timer(poll, null, 0, 10000);

                if (restarting)
                {
                    restarting = false;
                }
            }
            else
                osae.AddToLog("Adapter(" + adapterProp + ") not found on port " + port, true);

        }

        private void poll(object nothing)
        {
            threadCount++;
            osae.AddToLog("Thread Started.  Threads running: " + threadCount, false);
            if (threadCount < 2)
            {
                lock (thisLock)
                {
                    try
                    {
                        adapter.beginExclusive(true);
                        osae.AddToLog("polling...", false);
                        double temp;
                        OneWireContainer owc = null;

                        owc = adapter.getFirstDeviceContainer();

                        while (owc != null)
                        {
                            sbyte[] state;
                            OSAEObject obj = null;
                            string addr;
                            string desc;

                            string[] ct = owc.GetType().ToString().Split('.');
                            string owcType = ct[ct.Length - 1];

                            addr = owc.getAddressAsString();
                            desc = owc.getDescription();

                            osae.AddToLog("- Device Name: " + owc.getName(), false);
                            osae.AddToLog(" - Type: " + owcType, false);
                            osae.AddToLog(" - Address: " + addr, false);
                            osae.AddToLog(" - Description: " + desc, false);

                            switch (owcType)
                            {
                                case "OneWireContainer10":
                                    //DS1920 or DS18S20 - iButton family type 10

                                    try
                                    {
                                        OneWireContainer10 owc10 = (OneWireContainer10)owc;
                                        state = owc10.readDevice();
                                        owc10.doTemperatureConvert(state);

                                        //Changes the resultion that that will be read
                                        owc10.setTemperatureResolution(OneWireContainer10.RESOLUTION_NORMAL, state);

                                        obj = osae.GetObjectByAddress(owc10.getAddressAsString());
                                        if (obj == null)
                                        {
                                            osae.ObjectAdd("Temp Sensor-" + addr, "Temp Sensor-" + addr, "1WIRE TEMP SENSOR", addr, "", true);
                                            obj = osae.GetObjectByAddress(addr);
                                        }



                                        //Reads the temperature
                                        temp = tempConvert(owc10.getTemperature(state));




                                        osae.AddToLog(" - Temperature: " + temp, false);
                                        osae.ObjectPropertySet(obj.Name, "Temperature", temp.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        osae.AddToLog("Container type 10 poll error: " + ex.Message, true);
                                    }
                                    break;

                                case "OneWireContainer22":
                                    //DS1822 - iButton family type 22

                                    try
                                    {
                                    }
                                    catch (Exception ex)
                                    {
                                        osae.AddToLog("Container type 22 poll error: " + ex.Message, true);
                                    }
                                    break;

                                case "OneWireContainer28":
                                    //DS18B20 - iButton family type 28 

                                    try
                                    {
                                        OneWireContainer28 owc28 = (OneWireContainer28)owc;
                                        state = owc28.readDevice();

                                        ////Changes the resolution that that will be read
                                        owc28.setTemperatureResolution(OneWireContainer28.RESOLUTION_12_BIT, state);

                                        obj = osae.GetObjectByAddress(addr);
                                        if (obj == null)
                                        {
                                            osae.ObjectAdd("Temp Sensor-" + addr, "Temp Sensor-" + addr, "1WIRE TEMP SENSOR", addr, "", true);
                                            obj = osae.GetObjectByAddress(addr);
                                        }

                                        //Reads the temperature
                                        owc28.doTemperatureConvert(state);
                                        temp = tempConvert(owc28.getTemperature(state));




                                        osae.AddToLog(" - Temperature: " + temp, false);
                                        osae.ObjectPropertySet(obj.Name, "Temperature", temp.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        osae.AddToLog("Container type 28 poll error: " + ex.Message, true);
                                    }
                                    break;

                            }
                            owc = adapter.getNextDeviceContainer();
                        }
                        adapter.endExclusive();
                    }
                    catch (Exception ex)
                    {
                        osae.AddToLog("Cannot get exclusive use of the 1-wire adapter: " + ex.Message, true);
                    }
                }

                threadCount--;
                osae.AddToLog("Thread Ended.  Threads running: " + threadCount, false);
                abortCount = 0;

            }
            else
            {
                threadCount--;
                osae.AddToLog("Thread aborted.  Threads running: " + threadCount, false);
                abortCount++;
                if (abortCount > 2 && !restarting)
                {
                    osae.AddToLog("Restarting plugin", false);
                    restarting = true;
                    Shutdown();
                    RunInterface(pName);
                }

            }
        }

        public double tempConvert(double temp)
        {
            if (uom == "F")
                temp = temp * 9 / 5 + 32;
            return Math.Round(temp, 1);
        }

        public override void Shutdown()
        {
            osae.AddToLog("Running shutdown logic", true);
            adapter.freePort();
            adapter = null;
        }

        public static string toString(sbyte[] array)
        {
            System.Text.StringBuilder buf = new System.Text.StringBuilder();
            int maxIndex = array.Length - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                buf.Append((byte)array[i]);
                if (i < maxIndex)
                    buf.Append(".");
            }
            return buf.ToString();
        }
    }
}

