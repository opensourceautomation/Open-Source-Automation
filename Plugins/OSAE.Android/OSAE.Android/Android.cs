using System;
using System.AddIn;
using System.Collections.Generic;
using System.IO;

namespace OSAE.Android
{
    [AddIn("Android", Version = "0.0.1")]
    public class Android : OSAEPluginBase
    {
        private Logging logging = Logging.GetLogger("Android");

        string pName;
        List<AndroidDevice> mdevices = new List<AndroidDevice>();

        public override void RunInterface(string pluginName)
        {
            log("Running interface", true);
            pName = pluginName;

            OSAEObjectTypeManager.ObjectTypeUpdate("Android Device", "Android Device", "Android Device", pluginName, "Android Device", 0, 0, 0, 1);

            //connect to devices
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("Android Device");

            foreach (OSAEObject obj in objects)
            {
                createdevice(obj);
            }

            log("Run Interface Complete", true);
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

            log("Found Command: " + method_name + " | param1: " + parameter_1 + " | param2: " + parameter_2, true);

            if (object_name == pName)
            {

                switch (method_name)
                {

                    case "NOTIFYALL":
                        log("NOTIFYALL event triggered", false);

                        log("NOTIFYALL devices to loop:"+mdevices.Count, false);

                        foreach (AndroidDevice d in mdevices)
                        {
                            log("NOTIFYALL loop for device:"+d.Name, false);
                            d.ProcessCommand("NOTIFY", parameter_1, parameter_2);
                        }

                        break;

                    case "EXECUTEALL":
                        log("EXECUTEALL event triggered", false);

                        log("EXECUTEALL devices to loop:" + mdevices.Count, false);

                        foreach (AndroidDevice d in mdevices)
                        {
                            log("EXECUTEALL loop for device:" + d.Name, false);
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



        public void log(String message, bool alwaysLog)
        {
            try
            {
                //logging.AddToLog(message, alwaysLog);
                logging.AddToLog(message, true);
            }
            catch (IOException ex)
            {
                        //do nothing
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
            log("Added AndroidDevice to list: " + d.Name, false);

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
