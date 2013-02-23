using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace OSAE.RadioThermostat
{
    public class RadioThermostat : OSAEPluginBase
    {
        Logging logging = Logging.GetLogger("Radio Thermostat");
        string pName;
        System.Timers.Timer Clock;

        public override void ProcessCommand(OSAEMethod method)
        {
            logging.AddToLog("Process command: " + method.MethodName, false);

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
            logging.AddToLog("Running Interface!", true);
            pName = pluginName;
            OSAEObjectTypeManager.ObjectTypeUpdate("RADIO THERMOSTAT DEVICE", "RADIO THERMOSTAT DEVICE", "Radio Thermostat Device", pName, "RADIO THERMOSTAT DEVICE", 0, 0, 0, 1);

            OSAEObjectCollection devices = OSAEObjectManager.GetObjectsByType("Radio Thermostat Device");

            foreach (OSAEObject obj in devices)
            {   
                ThermostatLib.SystemInfo si = ThermostatLib.SystemInfo.Load(obj.Address);
                logging.AddToLog("---------------------------------", true);
                logging.AddToLog("Device Name: " + ThermostatLib.SystemInfo.LoadSystemName(obj.Address), true);
                logging.AddToLog("API Version: " + si.ApiVersion.ToString(), true);
                logging.AddToLog("Firmware Version: " + si.FirmwareVersion, true);
                logging.AddToLog("UUID: " + si.UUID, true);
                logging.AddToLog("WLAN Version: " + si.WlanFirmwareVersion, true);
                logging.AddToLog("Model: " + ThermostatLib.SystemInfo.LoadModelName(obj.Address), true);
                logging.AddToLog("Operating Mode: " + ThermostatLib.SystemInfo.LoadOperatingMode(obj.Address), true); 
                
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
                    logging.AddToLog("Service: " + service, true);
                }

            }

            Clock = new System.Timers.Timer();
            Clock.Interval = Int32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Poll Interval").Value) * 60000;
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
            OSAEObjectCollection devices = OSAEObjectManager.GetObjectsByType("Radio Thermostat Device");

            foreach (OSAEObject obj in devices)
            {
                ThermostatLib.ThermostatInfo status = ThermostatLib.ThermostatInfo.Load(obj.Address);

                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Current Temperature", status.Temperature.ToString(), pName);
                logging.AddToLog("Current Temperature: " + status.Temperature.ToString(), false);

                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Thermostat State", status.ThermostatState, pName);
                logging.AddToLog("Thermostat State: " + status.ThermostatState, false);

                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Fan State", status.FanState, pName);
                logging.AddToLog("Fan State: " + status.FanState, false);

                if (status.TemporaryCool > 0)
                {
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Set Temperature", status.TemporaryCool.ToString(), pName);
                    logging.AddToLog("Set Temperature: " + status.TemporaryCool.ToString(), false);
                }
                else
                {
                    OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Set Temperature", status.TemporaryHeat.ToString(), pName);
                    logging.AddToLog("Set Temperature: " + status.TemporaryHeat.ToString(), false);
                }
                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Thermostat Mode", status.ThermostatMode, pName);
                logging.AddToLog("Thermostat Mode: " + status.ThermostatMode, false);

                OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Fan Mode", status.FanMode, pName);
                logging.AddToLog("Fan Mode: " + status.FanMode, false);

                if (status.Hold) OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Hold", "Yes", pName); else OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Hold", "No", pName);
                if (status.Override) OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Override", "Yes", pName); else OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Override", "No", pName);
            }
        }
    }
}
