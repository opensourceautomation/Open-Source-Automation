using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE.RadioThermostat
{
    public class RadioThermostat : OSAEPluginBase
    {
        private OSAE.General.OSAELog Log;
        string pName;
        System.Timers.Timer Clock;

        public override void ProcessCommand(OSAEMethod method)
        {
            Log.Debug("Process command: " + method.MethodName);
            switch (method.MethodName)
            {
                case "SET TEMPORARY COOL":
                    ThermostatLib.ThermostatInfo.SetTemporaryCool(method.Address, Double.Parse(method.Parameter1));
                    break;
                case "SET TEMPORARY HEAT":
                    ThermostatLib.ThermostatInfo.SetTemporaryHeat(method.Address, Double.Parse(method.Parameter1));
                    break;
                case "SET HOLD":
                    ThermostatLib.ThermostatInfo.SetHold(method.Address, true);
                    break;
                case "REMOVE HOLD":
                    ThermostatLib.ThermostatInfo.SetHold(method.Address, false);
                    break;
                case "REBOOT":
                    ThermostatLib.SystemInfo.Reboot(method.Address);
                    break;
                case "SET LED":
                    ThermostatLib.SystemInfo.SetLED(method.Address, method.Parameter1);
                    break;
            }
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            Log = new General.OSAELog(pName);
            Log.Info("Running Interface!");

            OSAEObjectType objt = OSAEObjectTypeManager.ObjectTypeLoad("RADIO THERMOSTAT DEVICE");
            OSAEObjectTypeManager.ObjectTypeUpdate(objt.Name, objt.Name, "Radio Thermostat Device", pName, "RADIO THERMOSTAT DEVICE", objt.Owner, objt.SysType, objt.Container, objt.HideRedundant, objt.Tooltip);
            OSAEObjectCollection devices = OSAEObjectManager.GetObjectsByType("RADIO THERMOSTAT DEVICE");

            foreach (OSAEObject obj in devices)
            {   
                ThermostatLib.SystemInfo si = ThermostatLib.SystemInfo.Load(obj.Address);
                Log.Info("---------------------------------");
                Log.Info("Device Name: " + ThermostatLib.SystemInfo.LoadSystemName(obj.Address));
                Log.Info("API Version: " + si.ApiVersion.ToString());
                Log.Info("Firmware Version: " + si.FirmwareVersion);
                Log.Info("UUID: " + si.UUID);
                Log.Info("WLAN Version: " + si.WlanFirmwareVersion);
                Log.Info("Model: " + ThermostatLib.SystemInfo.LoadModelName(obj.Address));
                Log.Info("Operating Mode: " + ThermostatLib.SystemInfo.LoadOperatingMode(obj.Address)); 
                
                ThermostatLib.Services services = ThermostatLib.Services.Load(obj.Address);
                string service = "";
                foreach (ThermostatLib.HttpdHandler handler in services.Handlers)
                {
                    service = "";
                    service += handler.Url + " (";
                    if (handler.AllowsGet && handler.AllowsPost) service += "GET, POST";
                    else
                    {
                        if (handler.AllowsGet) service += "GET";
                        if (handler.AllowsPost) service += "POST";
                    }
                    service += ")";
                    Log.Info("Service: " + service);
                }
            }
            Clock = new System.Timers.Timer();
            Clock.Interval = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Poll Interval").Value) * 60000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick); 
        }

        public override void Shutdown()
        {
            Clock.Stop();
            Log.Info("Service Stopped");
        }

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            PollDevices();
        }

        private void PollDevices()
        {
            OSAEObjectCollection devices = OSAEObjectManager.GetObjectsByType("Radio Thermostat Device");
            foreach (OSAEObject obj in devices)
            {
                ThermostatLib.ThermostatInfo status = ThermostatLib.ThermostatInfo.Load(obj.Address);

                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Current Temperature", status.Temperature.ToString(), pName);
                Log.Debug("Current Temperature: " + status.Temperature.ToString());

                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Thermostat State", status.ThermostatState, pName);
                Log.Debug("Thermostat State: " + status.ThermostatState);

                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Fan State", status.FanState, pName);
                Log.Debug("Fan State: " + status.FanState);

                if (status.TemporaryCool > 0)
                {
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Set Temperature", status.TemporaryCool.ToString(), pName);
                    Log.Debug("Set Temperature: " + status.TemporaryCool.ToString());
                }
                else
                {
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Set Temperature", status.TemporaryHeat.ToString(), pName);
                    Log.Debug("Set Temperature: " + status.TemporaryHeat.ToString());
                }
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Thermostat Mode", status.ThermostatMode, pName);
                Log.Debug("Thermostat Mode: " + status.ThermostatMode);

                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Fan Mode", status.FanMode, pName);
                Log.Debug("Fan Mode: " + status.FanMode);

                if (status.Hold) OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Hold", "Yes", pName); else OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Hold", "No", pName);
                if (status.Override) OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Override", "Yes", pName); else OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Override", "No", pName);
            }
        }
    }
}
