using System;
using System.AddIn;
using System.Collections.Generic;
using System.IO;

namespace OSAE.Android
{
    [AddIn("Android", Version = "0.0.1")]
    public class Android : OSAEPluginBase
    {
        private static OSAE.General.OSAELog Log;// = new General.OSAELog();
        string pName;
        List<AndroidDevice> mdevices = new List<AndroidDevice>();

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            Log = new General.OSAELog(pName);
            Log.Info("Starting Android plugin");
            OwnTypes();

            //connect to devices
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("ANDROID DEVICE");

            foreach (OSAEObject obj in objects)
                createdevice(obj);

            Log.Debug("Run Interface Complete");
        }

        public void OwnTypes()
        {
            //Added the follow to automatically own Speech Base types that have no owner.
            OSAEObjectType oType = OSAEObjectTypeManager.ObjectTypeLoad("ANDROID");
            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant, oType.Tooltip);
                Log.Info("Android Plugin took ownership of the ANDROID Object Type.");
            }
            else
                Log.Info("Android Plugin correctly owns the ANDROID Object Type.");

            oType = OSAEObjectTypeManager.ObjectTypeLoad("ANDROID DEVICE");
            if (oType.OwnedBy == "")
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant,oType.Tooltip);
                Log.Info("Android Plugin took ownership of the ANDROID DEVICE Object Type.");
            }
            else
                Log.Info("Android Plugin correctly owns the ANDROID DEVICE Object Type.");
        }

        public override void Shutdown()
        {
            foreach (AndroidDevice d in mdevices)
            { }
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            try {
                string object_name = method.ObjectName;
                string method_name = method.MethodName;
                string parameter_1 = method.Parameter1;
                string parameter_2 = method.Parameter2;

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
                        d.ProcessCommand(method_name, parameter_1, parameter_2);

                }
            }
            catch (Exception ex)
            {
                Log.Error("Error processing command!",ex);
            }
        }


        public void createdevice(OSAEObject obj)
        {
            AndroidDevice d = new AndroidDevice(obj.Name, pName);

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
                if (d.Name == name) return d;
            }
            return null;
        }
    }
}
