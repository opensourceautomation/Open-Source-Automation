using System;
using System.AddIn;
using System.Collections.Generic;
using System.IO;

namespace OSAE.Android
{
    [AddIn("Android", Version = "0.0.1")]
    public class Android : OSAEPluginBase
    {
        //OSAELog
        private static OSAE.General.OSAELog Log = new General.OSAELog();

        string pName;
        List<AndroidDevice> mdevices = new List<AndroidDevice>();

        public override void RunInterface(string pluginName)
        {
            Log.Info("Starting Android plugin");
            pName = pluginName;

            OSAEObjectTypeManager.ObjectTypeUpdate("Android Device", "Android Device", "Android Device", pluginName, "Android Device", false, false, false, true);

            //connect to devices
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("Android Device");

            foreach (OSAEObject obj in objects)
            {
                createdevice(obj);
            }

            Log.Debug("Run Interface Complete");
        }


        public override void Shutdown()
        {

            foreach (AndroidDevice d in mdevices)
            {

            }

        }

        public override void ProcessCommand(OSAEMethod method)
        {

            String object_name = method.ObjectName;
            String method_name = method.MethodName;
            String parameter_1 = method.Parameter1;
            String parameter_2 = method.Parameter2;

            Log.Debug("Found Command: " + method_name + " | param1: " + parameter_1 + " | param2: " + parameter_2);

            if (object_name == pName)
            {

                switch (method_name)
                {

                    case "NOTIFYALL":
                        Log.Debug("NOTIFYALL event triggered");

                        Log.Debug("NOTIFYALL devices to loop:" + mdevices.Count);

                        foreach (AndroidDevice d in mdevices)
                        {
                            Log.Debug("NOTIFYALL loop for device:" + d.Name);
                            d.ProcessCommand("NOTIFY", parameter_1, parameter_2);
                        }

                        break;

                    case "EXECUTEALL":
                        Log.Debug("EXECUTEALL event triggered");

                        Log.Debug("EXECUTEALL devices to loop:" + mdevices.Count);

                        foreach (AndroidDevice d in mdevices)
                        {
                            Log.Debug("EXECUTEALL loop for device:" + d.Name);
                            d.ProcessCommand("EXECUTE", parameter_1, parameter_2);
                        }

                        break;

                }
            }
            else
            {
                AndroidDevice d = getAndroidDevice(object_name);

                if (d == null)
                {
                    OSAEObject obj = OSAEObjectManager.GetObjectByName(object_name);
                    createdevice(obj);
                    d = getAndroidDevice(object_name);
                }

                if (d != null)
                {
                    d.ProcessCommand(method_name, parameter_1, parameter_2);
                }
                
            }
        }


        public void createdevice(OSAEObject obj)
        {
            AndroidDevice d = new AndroidDevice(obj.Name, pName);

            //d.GCMID = obj.Address;

            foreach (OSAEObjectProperty prop in obj.Properties)
            {
                switch (prop.Name)
                {
                    case "Owner":
                        d.Owner = prop.Value;
                        break;
                    case "GCMID":
                        d.GCMID = prop.Value;
                        break;
                }
            }

            mdevices.Add(d);
            Log.Info("Added AndroidDevice to list: " + d.Name);

        }

        public AndroidDevice getAndroidDevice(string name)
        {
            foreach (AndroidDevice d in mdevices)
            {
                if (d.Name == name)
                    return d;
            }
            return null;
        }
    }
}
