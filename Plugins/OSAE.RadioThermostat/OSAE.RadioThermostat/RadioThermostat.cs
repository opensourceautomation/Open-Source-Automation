using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace OSAE.RadioThermostat
{
    public class RadioThermostat : OSAEPluginBase
    {
        OSAE osae = new OSAE("Radio Thermostat");
        string pName;
        System.Timers.Timer Clock;

        public override void ProcessCommand(OSAEMethod method)
        {
            osae.AddToLog("Process command: " + method.MethodName, false);

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
            osae.AddToLog("Running Interface!", true);
            pName = pluginName;
            osae.ObjectTypeUpdate("RADIO THERMOSTAT DEVICE", "RADIO THERMOSTAT DEVICE", "Radio Thermostat Device", pName, "RADIO THERMOSTAT DEVICE", 0, 0, 0, 1);
            
            List<OSAEObject> devices = osae.GetObjectsByType("Radio Thermostat Device");

            foreach (OSAEObject obj in devices)
            {   
                ThermostatLib.SystemInfo si = ThermostatLib.SystemInfo.Load(obj.Address);
                osae.AddToLog("---------------------------------",true);
                osae.AddToLog("Device Name: " + ThermostatLib.SystemInfo.LoadSystemName(obj.Address), true);
                osae.AddToLog("API Version: " + si.ApiVersion.ToString(), true);
                osae.AddToLog("Firmware Version: " + si.FirmwareVersion, true);
                osae.AddToLog("UUID: " + si.UUID, true);
                osae.AddToLog("WLAN Version: " + si.WlanFirmwareVersion, true);
                osae.AddToLog("Model: " + ThermostatLib.SystemInfo.LoadModelName(obj.Address), true);
                osae.AddToLog("Operating Mode: " + ThermostatLib.SystemInfo.LoadOperatingMode(obj.Address), true); 
                
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
                    osae.AddToLog("Service: " + service, true);
                }

            }

            Clock = new System.Timers.Timer();
            Clock.Interval = Int32.Parse(osae.GetObjectPropertyValue(pName, "Poll Interval").Value) * 60000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick); 
        }

        public override void Shutdown()
        {
            
        }

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            PollDevices();
        }

        private void PollDevices()
        {
            List<OSAEObject> devices = osae.GetObjectsByType("Radio Thermostat Device");

            foreach (OSAEObject obj in devices)
            {
                ThermostatLib.ThermostatInfo status = ThermostatLib.ThermostatInfo.Load(obj.Address);

                osae.ObjectPropertySet(obj.Name, "Current Temperature", status.Temperature.ToString());
                osae.AddToLog("Current Temperature: " + status.Temperature.ToString(), false);

                osae.ObjectPropertySet(obj.Name, "Thermostat State", status.ThermostatState);
                osae.AddToLog("Thermostat State: " + status.ThermostatState, false);

                osae.ObjectPropertySet(obj.Name, "Fan State", status.FanState);
                osae.AddToLog("Fan State: " + status.FanState, false);

                if (status.TemporaryCool > 0)
                {
                    osae.ObjectPropertySet(obj.Name, "Set Temperature", status.TemporaryCool.ToString());
                    osae.AddToLog("Set Temperature: " + status.TemporaryCool.ToString(), false);
                }
                else
                {
                    osae.ObjectPropertySet(obj.Name, "Set Temperature", status.TemporaryHeat.ToString());
                    osae.AddToLog("Set Temperature: " + status.TemporaryHeat.ToString(), false);
                }
                osae.ObjectPropertySet(obj.Name, "Thermostat Mode", status.ThermostatMode);
                osae.AddToLog("Thermostat Mode: " + status.ThermostatMode, false);

                osae.ObjectPropertySet(obj.Name, "Fan Mode", status.FanMode);
                osae.AddToLog("Fan Mode: " + status.FanMode, false);

                if (status.Hold) osae.ObjectPropertySet(obj.Name, "Hold", "Yes"); else osae.ObjectPropertySet(obj.Name, "Hold", "No");
                if (status.Override) osae.ObjectPropertySet(obj.Name, "Override", "Yes"); else osae.ObjectPropertySet(obj.Name, "Override", "No");
            }
        }
    }
}
