namespace OSAE.OneWire
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using com.dalsemi.onewire;
    using com.dalsemi.onewire.adapter;
    using com.dalsemi.onewire.container;

    public class OneWire : OSAEPluginBase
    {
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        string pName;
        string uom;

        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("OneWire");
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
            this.Log.Info("Loading (0.2.4)...");
            this.Log.Info("Current Environment Version: " + Environment.Version.Major.ToString());
            if (Environment.Version.Major >= 4)
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework64\v2.0.50727");
                if (!Directory.Exists(folder))
                    folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v2.0.50727");
                this.Log.Debug("vjsnativ.dll path: " + folder);
                folder = Path.GetFullPath(folder);
                LoadLibrary(Path.Combine(folder, "vjsnativ.dll"));
            }
            pName = pluginName;
            string adapterProp = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Adapter").Value;
            string port = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Port").Value;
            uom = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Unit of Measure").Value;
            this.Log.Debug("adapterProp: " + adapterProp);
            this.Log.Debug("port: " + port);
            this.Log.Debug("uom: " + uom);
            try
            {
                adapter = OneWireAccessProvider.getAdapter(adapterProp, "COM" + port);
            }
            catch (Exception ex)
            {
                this.Log.Error("Failed to GetAdapter ", ex);
            }


            if (adapter.adapterDetected())
            {
                this.Log.Info("Adapter Detected");
                adapter.setSearchAllDevices();

                Clock = new System.Threading.Timer(poll, null, 0, 10000);

                if (restarting)
                {
                    restarting = false;
                }
            }
            else
                this.Log.Info("Adapter(" + adapterProp + ") not found on port " + port);

        }

        private void poll(object nothing)
        {
            threadCount++;
            this.Log.Debug("Thread Started.  Threads running: " + threadCount);
            if (threadCount < 2)
            {
                lock (thisLock)
                {
                    try
                    {
                        adapter.beginExclusive(true);
                        this.Log.Debug("polling...");
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

                            this.Log.Debug("- Device Name: " + owc.getName());
                            this.Log.Debug(" - Type: " + owcType);
                            this.Log.Debug(" - Address: " + addr);
                            this.Log.Debug(" - Description: " + desc);

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

                                        obj = OSAEObjectManager.GetObjectByAddress(owc10.getAddressAsString());
                                        if (obj == null)
                                        {
                                            OSAEObjectManager.ObjectAdd("Temp Sensor-" + addr, "Temp Sensor-" + addr, "1WIRE TEMP SENSOR", addr, "", true);
                                            obj = OSAEObjectManager.GetObjectByAddress(addr);
                                        }



                                        //Reads the temperature
                                        temp = tempConvert(owc10.getTemperature(state));




                                        this.Log.Debug(" - Temperature: " + temp);
                                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", temp.ToString(), pName);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Log.Error("Container type 10 poll error", ex);
                                    }
                                    break;

                                case "OneWireContainer22":
                                    //DS1822 - iButton family type 22

                                    try
                                    {
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Log.Error("Container type 22 poll error", ex);
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

                                        obj = OSAEObjectManager.GetObjectByAddress(addr);
                                        if (obj == null)
                                        {
                                            OSAEObjectManager.ObjectAdd("Temp Sensor-" + addr, "Temp Sensor-" + addr, "1WIRE TEMP SENSOR", addr, "", true);
                                            obj = OSAEObjectManager.GetObjectByAddress(addr);
                                        }

                                        //Reads the temperature
                                        owc28.doTemperatureConvert(state);
                                        temp = tempConvert(owc28.getTemperature(state));




                                        this.Log.Debug(" - Temperature: " + temp);
                                        OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Temperature", temp.ToString(), pName);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Log.Error("Container type 28 poll error", ex);
                                    }
                                    break;

                            }
                            owc = adapter.getNextDeviceContainer();
                        }
                        adapter.endExclusive();
                    }
                    catch (Exception ex)
                    {
                        this.Log.Error("Cannot get exclusive use of the 1-wire adapter", ex);
                    }
                }

                threadCount--;
                this.Log.Debug("Thread Ended.  Threads running: " + threadCount);
                abortCount = 0;

            }
            else
            {
                threadCount--;
                this.Log.Debug("Thread aborted.  Threads running: " + threadCount);
                abortCount++;
                if (abortCount > 2 && !restarting)
                {
                    this.Log.Debug("Restarting plugin");
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
            this.Log.Info("Running shutdown logic");
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

