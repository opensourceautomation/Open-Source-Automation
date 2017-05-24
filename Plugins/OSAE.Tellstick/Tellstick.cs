//**********************************************************************************************************
//  OSA plugin for Telldus Tellstick. See www.telldus.com
//  The plugin should be able to handle both Tellstick (only Tx), and TellstickDuo (both Rx and Tx).
//     2012-08-15  0.0.1    Per Tobiasson   First version.
//     2012-09-15  0.0.2    Per Tobiasson   Improved handling of sensors and object status.
//                                          Handling of raw events.
//     2013-05-06  0.0.3    Per Tobiasson   Adjusted for OSA version 4.2
//     2014-03-04  0.0.4    Per Tobiasson   Adjusted for OSA version 4.4
//     2014-11-13  0.0.5    Per Tobiasson   Changed behaviour of Debug
//     2016-07-13  0.4.8    Per Tobiasson   Adjusted for OSA version 4.8
//     2017-05-04  0.4.9    Per Vaughn      Removed hard coded sql, should just read install.sql for restores
//**********************************************************************************************************
using System;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using TelldusWrapper;
using System.Runtime.InteropServices;
using System.Globalization;

namespace OSAE.Tellstick
{
    public class Tellstick : OSAEPluginBase
    {
        //int trustLevel = 100;   // The trustLevel this plugin will use to update objects, 100 is default will be adjusted in RunInterface
        int defaultTrustLevel = 50; // TrustLevel that will be set on objects created by this plugin
        private OSAE.General.OSAELog logger;
        //Logging oldLogger = Logging.GetLogger("Tellstick");
        string Name = "";
        string addrPrefix = "";
        string addrPrefixSensor = "";
        bool rawEvents = false;
        private static System.Timers.Timer avgTimer;   // Timer for calculating average values for sensors
        int sensorAverageTime = 10;  // Time period for calculation ov average values for sensors
        //private static System.Timers.Timer aTimer;   // Timer for surveilance of sensors
        //int sensorErrorTime = 10;  // If a sensor does not send a value for sensorErrorTime minutes it will go to Error state
        Boolean debug = true; //
        int pos = 0;    // Debug position
        //int debugLevel = 5; // 0=Nothing, 1=Errors, 2=Warnings, 3=Events, 4=Tracing, 5=Debug

        static int eventIdEvents = 0;
        static int eventIdDeviceEvents = 0;
        static int eventIdSensorEvents = 0;
        static int eventIdRawEvents = 0;

        private static TelldusNETWrapper ts = new TelldusNETWrapper();

        private class sensor            // Stores and calculates average temperature and humidity readings from sensors
        {
            private Object thisLock = new Object();     // 
            public float sumTemp;      // Sum of temperature readings
            public int numberTemp;     // Number of temperature readings
            public float sumHum;       // Sum of humidity readings
            public int numberHum;      // Number of humidity readings

            public sensor()
            {
                sumTemp = 0;
                sumHum = 0;
                numberTemp = 0;
                numberHum = 0;
            }

            public void addValue(int devId, float value, int dataType)
            {
                lock (thisLock)
                {
                    switch (dataType)
                    {
                        case TelldusNETWrapper.TELLSTICK_HUMIDITY:
                            sumHum = sumHum + value;
                            numberHum = numberHum + 1;
                            break;
                        case TelldusNETWrapper.TELLSTICK_TEMPERATURE:
                            sumTemp = sumTemp + value;
                            numberTemp = numberTemp + 1;
                            break;
                        default:
                            break;
                    }
                }
            }

            public float getAvg(int devId, int dataType)
            {
                float r;
                lock (thisLock)
                {
                    switch (dataType)
                    {
                        case TelldusNETWrapper.TELLSTICK_HUMIDITY:
                            if (numberHum > 0)
                                r = sumHum / numberHum;
                            else r = 0;
                            break;
                        case TelldusNETWrapper.TELLSTICK_TEMPERATURE:
                            if (numberTemp > 0)
                                return sumTemp / numberTemp;
                            else r = 0;
                            break;
                        default:
                            r = 0;
                            break;
                    }
                    return r;
                }
            }

            public float getAvgReset(int devId, int dataType)
            {
                float r;
                lock (thisLock)
                {
                    switch (dataType)
                    {
                        case TelldusNETWrapper.TELLSTICK_HUMIDITY:
                            if (numberHum > 0)
                            {
                                r = sumHum / numberHum;
                                sumHum = 0;
                                numberHum = 0;
                            }
                            else r = 0;
                            break;
                        case TelldusNETWrapper.TELLSTICK_TEMPERATURE:
                            if (numberTemp > 0)
                            {
                                r = sumTemp / numberTemp;
                                sumTemp = 0;
                                numberTemp = 0;
                            }
                            else r = 0;
                            break;
                        default:
                            r = 0;
                            break;
                    }
                }
                return r;
            }

        }

        private static Dictionary<int, sensor> sensorList = new Dictionary<int, sensor>();

        #region constants
        const string objOwner = "TELLSTICK";    // Owner of all Tellstick devices

        // ObjectType names for all Tellstick devices
        const string objTypeBell = "TELLSTICK BELL";
        const string objTypeOnOff = "TELLSTICK BINARY SWITCH";
        const string objTypeDim = "TELLSTICK MULTILEVEL SWITCH";
        const string objTypeUpDown = "TELLSTICK UP-DOWN DEVICE";
        const string objTypeGroup = "TELLSTICK GROUP DEVICE";
        const string objTypeTemp = "TELLSTICK TEMPERATURE SENSOR";
        const string objTypeHum = "TELLSTICK HUMIDITY SENSOR";
        const string objTypeTempHum = "TELLSTICK TEMP-HUMIDITY SENSOR";

        // Property names for all properties
        const string propDeviceaddr = "DeviceAddress";
        const string propDevChgEvent = "DeviceChangeEvents";
        const string propDevId = "DeviceId";
        const string propRawEvents = "RawEvents";
        const string propDevType = "DeviceType";
        const string propLastCmd = "LastSentCommand";
        const string propLevel = "Level";
        const string propMethods = "Methods";
        const string propProtocol = "Protocol";
        const string propModel = "Model";
        const string propTemp = "Temperature";
        const string propHum = "Humidity";
        const string propUnit = "Unit";
        const string propAvgHum = "AverageHum";
        const string propAvgTemp = "AverageTemp";
        const string propMaxHum = "MaxHum";
        const string propMinHum = "MinHum";
        const string propMaxTemp = "MaxTemp";
        const string propMinTemp = "MinTemp";
        const string pluginPropDebug = "Debug";

        // Properties for Tellstick object type
        const string propTrustLevel = "Trust Level";
        const string propAddrPrefix = "AddressPrefix";
        const string propAddrPrefixSensor = "AddressPrefixSensor";
        const string propNoDev = "NoOfDevices";
        const string propAddDevices = "AddDevices";
        const string propAddSensors = "AddSensors";
        const string propAvgSensors = "AverageInt";
        const string propErrSensors = "ErrorTimeSensors";
        const string propDebug = "Debug";
        const string propDebugLevel = "DebugLevel";
        const string propAddrComputerName = "Computer Name";
        const string propPort = "Port";
        const string propSystemPlugin = "System Plugin";
        const string propAuthor = "Author";
        const string propVersion = "Version";

        // Names of each object Status
        const string objStatOn = "ON";
        const string objStatOff = "OFF";
        const string objStatDisable = "DISABLE";
        const string objStatUnKnown = "UNKNOWN";
        //const string objStatError = "ERROR";

        // Names of each object method
        const string objMtdOn = "ON";
        const string objMtdOff = "OFF";
        const string objMtdBell = "BELL";
        const string objMtdToggle = "TOGGLE";
        const string objMtdDim = "DIM";
        const string objMtdLearn = "LEARN";
        const string objMtdExecute = "EXECUTE";
        const string objMtdUp = "UP";
        const string objMtdDown = "DOWN";
        const string objMtdStop = "STOP";
        const string objMtdEnable = "ENABLE";
        const string objMtdDis = "DISABLE";
        const string objMtdInit = "REINIT";
        const string objMtdDebugOn = "DEBUGON";
        const string objMtdDebugOff = "DEBUGOFF";
        const string objMtdRestMaxMin = "RESETMAXMIN";
        const string objMtdRestore = "RESTORE";
        const string pluginMtdDebugOn = "DEBUGON";
        const string pluginMtdDebugOff = "DEBUGOFF";
        const string pluginMtdRestore = "RESTORE";

        // Names of each object event
        const string objEveOn = "ON";
        const string objEveOff = "OFF";
        const string objEveBell = "BELL";
        const string objEveToggle = "TOGGLE";
        const string objEveDim = "DIM";
        const string objEveLearn = "LEARN";
        const string objEveExecute = "EXECUTE";
        const string objEveUp = "UP";
        const string objEveDown = "DOWN";
        const string objEveStop = "STOP";

        // Names for sensor dataTypes
        const string sensDTTemp = "temperature";
        const string sensDTHum = "humidity";
        const string sensDTTempHum = "temperaturehumidity";


        // Names of containers to place new objects in
        const string contSensors = "SENSORS";
        const string contSwitches = "SWITCHES";

        // Strings for sensors in Windows Registry
        const string pathSensors = @"HKEY_CURRENT_USER\Software\Telldus\TelldusCenter\com.telldus.sensors\sensors";

        int w = 20;     // Used for formating in log strings
        int v = 20;



        #endregion

        #region public methods
        /// <summary>
        ///  RunInterface will be called everytime the plugin starts. Will discover all devices existing in Telldus center.
        ///  New OSA objects will be created for each device if they are not already existing, unless the property AddDevices is set to FALSE. 
        /// </summary>
        /// <param name="pluginName">Name of the plugin, i.e. Tellstick</param>
        override public void RunInterface(string pluginName)
        {
            pos = 0;
            //            // ### Code to make it possible to debug RunInterface. Put breakpoint on line with Sleep(0), then manually move execution out of the loop.
            //            Name = "debug";
            //            while (Name == "debug")
            //            {
            //                System.Threading.Thread.Sleep(0);
            //            }
            //            // ### Until here
            Name = pluginName;
            logger = new General.OSAELog(Name);
            //oldLogger = OSAE.Logging.GetLogger("Tellstick");
            OSAEObject obj = null;
            pos = 2;
            log("RunInterface", "", "********** Starting... **********");
            initPlugin(Name);

            try
            {
                // Initialise the plugin
                // TelldusNETWrapper.tdInit();   // Initialise the Telldus dll
                int NumberOfDevices = TelldusNETWrapper.tdGetNumberOfDevices();
                pos = 3;
                log("RunInterface", "", "********** " + pluginName + " started, found " + NumberOfDevices + " devices **********");
                log("RunInterface", "", "*****************************");
                pos = 4;
                if (OSAEObjectPropertyManager.ObjectPropertyExists(Name, propNoDev))
                    OSAEObjectManager.GetObjectByName(pluginName).SetProperty(propNoDev, NumberOfDevices.ToString(), pluginName);
                else
                    logW("RunInterface", pluginName, propNoDev + " Property is missing. (Integer)");
                log("RunInterface", addrPrefix, "AddressPrefix for devices set. ");
                log("RunInterface", addrPrefixSensor, "AddressPrefix for sensors set. ");

                // All old devices to DISABLED
                log("RunInterface",  "OFF", "Updating all old devices to status 'OFF' or 'UNKNOWN'.");
                pos = 5;
                OSAEObjectCollection devices = OSAEObjectManager.GetObjectsByOwner(objOwner);
                devices.ToList().ForEach(StatusDefault);

                // Initialise all current devices
                log("RunInterface", "", "Initialise all current devices.");
                for (int i = 0; i < NumberOfDevices; i++)
                {
                    try
                    {
                        pos = 6;
                        int deviceId = TelldusNETWrapper.tdGetDeviceId(i);
                        string name = TelldusNETWrapper.tdGetName(deviceId);
                        pos = 7;
                        obj = getObject(deviceId);
                        pos = 8;
                        obj.LastUpd = DateTime.Now.ToString();
                        logD("RunInterface", obj.Name, obj.Address.PadRight(v) + "Id-" + deviceId + "/Cont-" + obj.Container +
                                      "/Enab-" + obj.Enabled +
                                      "/Metd-" + TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_ALL) +
                                      "/Stat-" + obj.State + "/Typ-" + obj.Type, obj);
                        pos = 9;
                        setOSAProperties(obj, deviceId);
                        pos = 10;
                        setLastProperties(obj, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                        pos = 11;
                    }
                    catch (Exception ex)
                    {
                        logE("RunInterface", obj.Name, "RunInterface Exception", ex);
                        // Exception for one device, continue with the other devices
                    }
                } //for

                pos = 12;
                // Create a timer for creation of average sensor values.  
                avgTimer = new System.Timers.Timer(60000 * sensorAverageTime);  // 
                avgTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnAverageEvent);  // Hook up the Elapsed event for the timer.
                pos = 13;

                avgTimer.Enabled = true;
                logD("RunInterface", sensorAverageTime.ToString(),"Timer event for average sensors values added.");

                // Register callback functions
                log("RunInterface", "", "Registering callback functions. ");
                eventIdEvents = ts.tdRegisterDeviceEvent(decodeEvent, null);
                log("RunInterface", "decodeEvent", "Registered.");
                eventIdDeviceEvents = ts.tdRegisterDeviceChangeEvent(decodeChangeEvent, null);
                log("RunInterface", "decodeChangeEvent", "Registered.");
                eventIdSensorEvents = ts.tdRegisterDeviceSensorEvent(decodeSensorEvent, null);
                log("RunInterface", "decodeSensorEvent", "Registered.");
                if (rawEvents)
                {
                    eventIdRawEvents = ts.tdRegisterRawDeviceEvent(decodeRawEvent, null);
                    log("RunInterface", "decodeRawEvent", "Registered.");
                }
                log("RunInterface", "", "**********      Running!      **********");
            }
            catch (Exception ex)
            {
                logE("RunInterface", obj.Name, "RunInterface Exception", ex);
                throw; // Do not continue execution, fatal problem
            }
        } //RunInterface

        /// <summary>
        ///  ProcessCommand will be called by OSA everytime a method is run on an OSA object.
        ///  Will send the apropriate commands to Tellstick and update the object properties accordingly.
        /// </summary>
        /// <param name="method">Name of executed method</param>
        override public void ProcessCommand(OSAEMethod method)
        {
            try
            {
                pos = 1; OSAEObject obj = OSAEObjectManager.GetObjectByName(method.ObjectName);
                pos = 2; log("ProcessCommand", obj.Name, method.MethodName + " Run method on object.");
                pos = 3;
                if (obj.Name == Name)  pluginCommands(obj, obj.Property(propAddrComputerName).Value, method);  // do commands on the Tellstick plugin object
                else doCommand(obj, obj.Property(propDevId).Value, method);       // do commands on the Tellstick devices
                pos = 5;
            }
            catch (Exception ex)
            {
                logE("ProcessCommand", method.ObjectName, method.MethodName + " ProcessCommand Exception. ", ex);
            }
        }

        /// <summary>
        ///  decodeEvent is the callback function for messages from the Tellstick. The function is registered
        ///  with the Telldus service and will then be called from Telldus everytime a device changes its state
        /// </summary>
        /// <param name="deviceId">Tellstick Id of device who changed state</param>
        /// <param name="method">This is the Telldus name of the new state</param>
        /// <param name="data">Data value given by Telldus, for example DIM value</param>
        /// <param name="callbackId">Id given by Telldus when function was registered</param>
        /// <param name="obj">Optional object given to Telldus when callback function was registered. (Not used)</param>
        /// <returns>Success or error code (Will always be 0)</returns>
        public int decodeEvent(int deviceId, int method, string data, int callbackId, Object obj)
        //        public int decodeEvent(int deviceId, int method, char* data, int callbackId, Object obj)
        {
            try
            {
                logD("decodeEvent", TelldusNETWrapper.tdGetName(deviceId), MethodToString(method) + " CallbackEvent. " + data + " CallbackId: " + callbackId);
                OSAEObject o = getObject(deviceId);

                if (o != null)
                {  // If no object then this is a new device, but if objects should not be created there is no object, just ignore it.
                    if (o.State.Value != objStatDisable)
                    {   //Events should be handled
                        logD("decodeEvent", o.Name, MethodToString(method) + " Method will be handled on object.", o);
                        switch (method)
                        {
                            case TelldusNETWrapper.TELLSTICK_TURNON:
                                break;
                            case TelldusNETWrapper.TELLSTICK_TURNOFF:
                                break;
                            case TelldusNETWrapper.TELLSTICK_BELL:
                                break;
                            case TelldusNETWrapper.TELLSTICK_TOGGLE:
                                break;
                            case TelldusNETWrapper.TELLSTICK_DIM:
                                break;
                            case TelldusNETWrapper.TELLSTICK_LEARN:
                                break;
                            case TelldusNETWrapper.TELLSTICK_EXECUTE:
                                break;
                            case TelldusNETWrapper.TELLSTICK_UP:
                                break;
                            case TelldusNETWrapper.TELLSTICK_DOWN:
                                break;
                            case TelldusNETWrapper.TELLSTICK_STOP:
                                break;
                            default:
                                logE("decodeEvent", o.Name, MethodToString(method) + " Unknown Event: Nothing added to eventlog.", null);
                                break;
                        } //switch
                        setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                    }
                    else
                        logD("decodeEvent", o.Name, MethodToString(method) + " DeviceEvents will not be handled.", o);
                }
            }
            catch (Exception ex)
            {

                logE("decodeEvent", "", "Exception in callback function: ", ex);
            }
            int result = 0;
            return result;
        }

        private string getPropertyValue(OSAEObject o, string p)
        {
            string s="";
            if (OSAEObjectPropertyManager.ObjectPropertyExists(o.Name, p))
                return o.Property(p).Value;
            else
            {
                switch (p)
                {
                    case propModel:
                    case propDeviceaddr:
                    case propProtocol:
                    case propTrustLevel:
                    case propAddrComputerName:
                    case propPort:
                    case propLastCmd:
                    case propLevel:
                        logD("getPropertyValue", o.Name, "Property " + p + " does not exist, read ignored.", o);
                        s = "";
                        break;
                    case propMethods:
                        logW("getPropertyValue", o.Name, "Property " + p + " does not exist, no Methods can be executed.");
                        s = "";
                        break;
                    case propDevChgEvent:
                    case propAddDevices:
                    case propAddSensors:
                    case propSystemPlugin:
                        logD("getPropertyValue", o.Name, "Property " + p + " does not exist, default to true.", o);
                        s = "TRUE";
                        break;
                    case propDebug:
                    case propRawEvents:
                        logD("getPropertyValue", o.Name, "Property " + p + " does not exist, default to false.", o);
                        s = "FALSE";
                        break;
                    case propAddrPrefix:
                        logD("getPropertyValue", o.Name, "Property " + p + " does not exist, default to 'TellstickId-'.", o);
                        s = "TellstickId-";
                        break;
                    case propAddrPrefixSensor:
                        logD("getPropertyValue", o.Name, "Property " + p + " does not exist, default to 'TellstickSensor-'.", o);
                        s = "TellstickSensor-";
                        break;
                    case propAvgSensors:
                        logD("getPropertyValue", o.Name, "Property " + p + " does not exist, default to 10.", o);
                        s = "10";
                        break;
                    case propDevId:
                        logW("getPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, propDevId, "Integer", "", "", true,false, propDevId);
                       s="";
                        break;
                    case sensDTTemp:
                        logW("getPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, propTemp, "Float", "", "0", true, false, propHum);
                        s="0";
                        break;
                    case sensDTHum:
                        logW("getPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, propHum, "Float", "", "0", true, false, propHum);
                        s="0";
                        break;
                    case propMinHum:
                    case propMaxHum:
                    case propMinTemp:
                    case propMaxTemp:
                    case propAvgHum:
                    case propAvgTemp:
                        logW("getPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, p, "Float", "", "0", true, false, "Integer");
                        s = "0";
                        break;
                }
                return s;
            }
        }

        private void setPropertyValue(OSAEObject o, string p, string v)
        {
            if (OSAEObjectPropertyManager.ObjectPropertyExists(o.Name, p))
                o.SetProperty(p, v, Name);
            else
            {
                switch (p)
                {
                    case propModel:
                    case propDeviceaddr:
                    case propProtocol:
                    case propDevChgEvent:
                    case propDevType:
                        // These properties are not mandatory, they can be removed, and plugin will work without them
                        logD("setPropertyValue", o.Name, "Property " + p + " does not exist, NOT updated.", o);
                        break;
                    case propMinHum:
                    case propMaxHum:
                    case propMinTemp:
                    case propMaxTemp:
                    case propAvgHum:
                    case propAvgTemp:
                        logW("setPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, p, "Float", "", "0", true, false, p);
                        o.SetProperty(p, v, Name);
                        break;
                    case propDevId:
                    case propLevel:
                        logW("setPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, p, "Integer", "", "", true, false, p);
                        o.SetProperty(p, v, Name);
                        break;
                    case propMethods:
                    case propLastCmd:
                        logW("setPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, p, "String", "", "", true, false, p);
                        o.SetProperty(p, v, Name);
                        break;
                    case sensDTTemp:
                        logW("setPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, propTemp, "Float", "", "0", true, false, propTemp);
                        o.SetProperty(p, v, Name);
                        break;
                    case sensDTHum:
                        logW("setPropertyValue", o.Name, "Property " + p + " does not exist, property will be recreated.");
                        OSAEObjectTypeManager.ObjectTypePropertyAdd(o.Type, propHum, "Float", "", "0", true, false, propTemp);
                        o.SetProperty(p, v, Name);
                        break;
                }
            }
        }

        /// <summary>
        ///  decodeChangeEvent is the callback function for device change messages from the Tellstick. The function is
        ///  registered with the Telldus service and will then be called from Telldus everytime a device is changed.
        ///  This can for example be a new name or address for a device. The object properties will be updated with
        ///  the new values.
        /// </summary>
        /// <param name="deviceId">Tellstick Id of device who changed</param>
        /// <param name="changeEvent">Type of change, added, deleted, changed etc.</param>
        /// <param name="changeType">Specifies which property who changed.</param>
        /// <param name="callbackId">Id given by Telldus when function was registered</param>
        /// <param name="obj">Optional object given to Telldus when callback function was registered. (Not used)</param>
        /// <returns>Success or error code (Will always be 0)</returns>
        public int decodeChangeEvent(int deviceId, int changeEvent, int changeType, int callbackId, Object obj)
        {
            try
            {
                logD("decodeChangeEvent", TelldusNETWrapper.tdGetName(deviceId), changeEventToString(changeEvent) + " CallbackChangeEvent. " + changeEventTypeToString(changeType) + " CallbackId: " + callbackId);
                OSAEObject o = getObject(deviceId);
                if (getPropertyValue(o, propDevChgEvent).ToUpper() == "TRUE")
                {
                    logD("decodeChangeEvent", o.Name, changeEventToString(changeEvent) + " Device change event. " + changeEventTypeToString(changeType), o);
                    switch (changeEvent)
                    {
                        case TelldusNETWrapper.TELLSTICK_DEVICE_ADDED:
                            setOSAProperties(o, deviceId);
                            setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            break;
                        case TelldusNETWrapper.TELLSTICK_DEVICE_CHANGED:
                            switch (changeType) // When a device is saved in TelldusCenter with no change, it will be sent as a Model change
                            {
                                case 0: // missing in TelldusWrapper.cs
                                    break;
                                case TelldusNETWrapper.TELLSTICK_CHANGE_MODEL:
                                    setPropertyValue(o, propModel, TelldusWrapper.TelldusNETWrapper.tdGetModel(deviceId));
                                    setPropertyValue(o, propDeviceaddr, TelldusWrapper.TelldusNETWrapper.tdGetDeviceParameter(deviceId, "house", "") + "/" + TelldusWrapper.TelldusNETWrapper.tdGetDeviceParameter(deviceId, "code", "") + "/" + TelldusWrapper.TelldusNETWrapper.tdGetDeviceParameter(deviceId, "unit", ""));
                                    break;
                                case TelldusNETWrapper.TELLSTICK_CHANGE_NAME:
                                    OSAEObjectManager.ObjectUpdate(o.Name, TelldusWrapper.TelldusNETWrapper.tdGetName(deviceId), o.Alias, o.Description, o.Type, o.Address, o.Container, o.MinTrustLevel, o.Enabled);
                                    break;
                                case TelldusNETWrapper.TELLSTICK_CHANGE_PROTOCOL:
                                    setPropertyValue(o, propProtocol, TelldusWrapper.TelldusNETWrapper.tdGetProtocol(deviceId));
                                    break;
                                case TelldusNETWrapper.TELLSTICK_CHANGE_METHOD:
                                    setMethods(deviceId, o);
                                    break;
                                case TelldusNETWrapper.TELLSTICK_CHANGE_AVAILABLE:
                                    break;
                                case TelldusNETWrapper.TELLSTICK_CHANGE_FIRMWARE:
                                    break;
                                default:
                                    logE("decodeChangeEvent", o.Name, changeType.ToString() + " Unknown changeType.", null);
                                    break;
                            } //switch changetype
                            o.SetState(objStatUnKnown);
                            break;
                        case TelldusNETWrapper.TELLSTICK_DEVICE_REMOVED:
                            o.SetState(objStatDisable);
                            break;
                        case TelldusNETWrapper.TELLSTICK_DEVICE_STATE_CHANGED:
                            break;
                        default:
                            logE("decodeChangeEvent", o.Name, changeEvent.ToString() + " Unknown device change event.", null);
                            break;
                    } //switch
                }
                else
                {
                    logD("decodeChangeEvent", o.Name, "Device change events should not be handled", o);
                } //if
                setPropertyValue(o, propDevId, deviceId.ToString());
                int result = 0;
                return result;
            }
            catch (Exception ex)
            {
                logE("decodeChangeEvent", "","Exception in callback function: ", ex);
                return -1;
            }
        }

        /// <summary>
        ///  decodeSensorEvent is the callback function for sensor messages, humidity or temperature, from the Tellstick.
        ///  The function is registered with the Telldus service and will then be called from Telldus everytime a device
        ///  is changed. The object properties will be updated with the new values.
        /// </summary>
        /// <param name="protocol">Name of protocol used by Tellstick</param>
        /// <param name="model">Sensor model</param>
        /// <param name="deviceId">Tellstick Id of device who changed</param>
        /// <param name="dataType">Type of value from this sensor</param>
        /// <param name="value">New sensor value.</param>
        /// <param name="timestamp">Tellstick timestamp in number of seconds since 1/1 1970.</param>
        /// <param name="callbackId">Id given by Telldus when function was registered</param>
        /// <param name="obj">Optional object given to Telldus when callback function was registered. (Not used)</param>
        /// <returns>Success or error code (Will always be 0)</returns>
        public int decodeSensorEvent(string protocol, string model, int deviceId, int dataType, string value, int timestamp, int callbackId, Object obj)
        {
            logD("decodeSensorEvent", dataTypeToString(dataType), model + " CallbackSensorEvent." + protocol + "Id: " + deviceId + " Value: " + value + " Timestamp: " + timestamp + " CallbackId: " + callbackId);
            int result = 0;
            NumberStyles styles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            float v = float.Parse(value, styles, nfi);  // Tellstick values are using '.' as decimal separator.
            try
            {
                OSAEObject o = getSensorObject(deviceId, dataType, protocol, model);
                if (o != null)
                { // If no object, then this is a new sensor, but objects should not be created, so just ignore it
                    if (o.State.Value != objStatDisable)
                    {
                        if (!sensorList.ContainsKey(deviceId))
                        {
                            sensor s = new sensor();
                            sensorList.Add(int.Parse(o.Property(propDevId).Value), s);
                        }
                        if ((dataType != TelldusNETWrapper.TELLSTICK_HUMIDITY) || (value != "255"))     // What is this?????
                        {
                            logD("decodeSensorEvent", o.Name, dataTypeToString(dataType) + " Sensor event: " + value, o);
                            // Store the value in the parameter. Will only trigger an event if value is changed
                            setPropertyValue(o, dataTypeToString(dataType), v.ToString());    // Write value in local format, i.e. correct decimal separator
                            switch (dataType) {
                                case TelldusNETWrapper.TELLSTICK_HUMIDITY:
                                    sensorList[deviceId].addValue(deviceId , v, TelldusNETWrapper.TELLSTICK_HUMIDITY);
                                    if (v < float.Parse(getPropertyValue(o, propMinHum)))
                                        setPropertyValue(o, propMinHum, v.ToString());
                                    if (v > float.Parse(getPropertyValue(o, propMaxHum)))
                                        setPropertyValue(o, propMaxHum, v.ToString());
                                    break;
                                case TelldusNETWrapper.TELLSTICK_TEMPERATURE:
                                    sensorList[deviceId].addValue(deviceId , v, TelldusNETWrapper.TELLSTICK_TEMPERATURE);
                                    if (v < float.Parse(getPropertyValue(o, propMinTemp)))
                                        setPropertyValue(o, propMinTemp, v.ToString());
                                    if (v > float.Parse(getPropertyValue(o, propMaxTemp)))
                                        setPropertyValue(o, propMaxTemp, v.ToString());
                                    break;
                                default: break;
                            }
                            //Will always trigger an event
                            o.SetState(objStatOn);      // Always set to on, to reset the OFF TIMER
                        }
                    }
                    else
                    {
                        logD("decodeSensorEvent", o.Name, "Sensor events should not be handled: " + o.State.Value, o);
                    } //if
                    result = 0;
                }
                else
                    result = -1;
                return result;
            }
            catch (Exception ex)
            {
                logE("decodeSensorEvent", "", "Exception in callback function: ", ex);
                return -1;
            }
        }

        /// <summary>
        ///  decodeRawEvent is the callback function for raw messages from the Tellstick.
        ///  The function is registered with the Telldus service and will then be called from Telldus everytime a device
        ///  is changed. Nothing will be done with the message, will only create an entry in the logfile.
        /// </summary>
        /// <param name="data">New sensor value.</param>
        /// <param name="controllerId">Tellstick timestamp in number of seconds since 1/1 1970.</param>
        /// <param name="callbackId">Id given by Telldus when function was registered</param>
        /// <param name="obj">Optional object given to Telldus when callback function was registered. (Not used)</param>
        /// <returns>Success or error code (Will always be 0)</returns>
        public int decodeRawEvent(string data, int controllerId, int callbackId, Object obj)
        {
            if (rawEvents)
            {
                logD("decodeRawEvent", controllerId.ToString(), "Raw data event: " + data + " Contr: " + " CallbackId: " + callbackId);
            }
            return 0;
        }

        /// <summary>
        ///  Shutdown will be called everytime when OSA stops the plugin.
        ///  Will clean up the Telldus class and stop the timer
        /// </summary>
        override public void Shutdown()
        {
            try
            {
                log("Shutdown", "", "*******************************      Stopping...      ***************************************");
            }
            catch (Exception ex)
            {
                logE("Shutdown","", "Exception in Shutdown: ", ex);
            }
        }

        #endregion


        /// <summary>
        ///  OnAverageEvent will be called regulary to calculate average values for sensors.
        ///  The timer will be started from RunInterface.
        /// </summary>
        /// <param name="source">Will not be used.</param>
        /// <param name="e">Time when the timer event triggered.</param>
        private void OnAverageEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            logD("OnAverageEvent", sensorAverageTime.ToString(), "Starting average calculation for sensors " + e.SignalTime);
            foreach (KeyValuePair<int, sensor> kvp in sensorList)
            {
                OSAEObject o = OSAEObjectManager.GetObjectByAddress(addrPrefixSensor + kvp.Key);
                if (o != null)
                {
                    logD("OnAverageEvent", kvp.Key.ToString(), "Average calculation for sensor ");
                    if (kvp.Value.numberHum > 0)
                    {
                        setPropertyValue(o, propAvgHum, kvp.Value.getAvgReset(kvp.Key, TelldusNETWrapper.TELLSTICK_HUMIDITY).ToString());
                    }
                    if (kvp.Value.numberTemp > 0)
                    {
                        setPropertyValue(o, propAvgTemp, kvp.Value.getAvgReset(kvp.Key, TelldusNETWrapper.TELLSTICK_TEMPERATURE).ToString());
                    }
                }
                else
                { //No object, must have been changed or deleted, just ignore it
                    logD("OnAverageEvent", kvp.Key.ToString(), "Can not calculate, object changed or deleted.");
                }
            }
        }

        /// <summary>
        ///  getSensorObject will try to find an exsisting object from the deviceId, if not found a new one will be created
        ///  but only if the property AddSensors is TRUE.
        /// </summary>
        /// <param name="deviceId">Id of object to try to find.</param>
        /// <param name="dataType">dataRype used if a new object is created.</param>
        /// <param name="protocol">protocol used if a new object is created.</param>
        /// <param name="model"> model used if a new object is created.</param>
        /// <returns>Returns the object found or created.</returns>
        private OSAEObject getSensorObject(int deviceId, int dataType, string protocol, string model)
        {
            try
            {
                OSAEObject o = OSAEObjectManager.GetObjectByAddress(addrPrefixSensor + deviceId);
                if (o == null)
                {   //No object existed, create the new one
                    if (getPropertyValue(OSAEObjectManager.GetObjectByName(Name), propAddSensors).ToUpper() == "TRUE")
                    {
                        o = createSensorObject(deviceId, dataType, protocol, model);
                        sensor s = new sensor();
                        sensorList.Add(deviceId, s);
                    }
                    else
                    {
                        logD("getSensorObject", model, deviceId.ToString() + " New sensor objects will not be created.");
                        return null;
                    }
                }
                else
                    logD("getSensorObject", o.Name, o.Address + " Object found.", o);
                return o;
            }
            catch (Exception ex)
            {
                logE("getSensorObject", "", "Exception in getSensorObject: ", ex);
                OSAEObject o = null;
                return o;
            }
        }

        /// <summary>
        ///  getObject will try to find an exsisting object from the deviceId or from the Telldus name of this deviceId.
        ///  If not found a new one will be created, otherwise the existing object is updated.
        /// </summary>
        /// <param name="deviceId">Id of object to try to find.</param>
        /// <returns>Returns the object found or created.</returns>
        private OSAEObject getObject(int deviceId)
        {
            try
            {
                OSAEObject o = OSAEObjectManager.GetObjectByAddress(addrPrefix + deviceId);
                if (o == null)
                {  //No object with this deviceId, try to find it on device name
                    o = OSAEObjectManager.GetObjectByName(TelldusNETWrapper.tdGetName(deviceId));
                    if (o != null)
                    { //Object with that name existed, change the object address to the new one
                        logD("getObject", o.Name, o.Address + " Old object with same name found (address will be updated).", o);
                        OSAEObjectManager.ObjectUpdate(o.Name, o.Name, o.Alias, o.Description, o.Type, addrPrefix + deviceId, o.Container, o.MinTrustLevel, o.Enabled);
                    }
                    else
                    {   //No object existed, create the new one
                        //if (OSAEObjectPropertyManager.GetObjectPropertyValue(objOwner, propAddDevices).Value.ToUpper() != "FALSE")
                        if (getPropertyValue(OSAEObjectManager.GetObjectByName(objOwner), propAddDevices).ToUpper() != "FALSE")
                        {
                            OSAEObjectManager.ObjectAdd(TelldusNETWrapper.tdGetName(deviceId),
                                "",
                                TelldusNETWrapper.tdGetModel(deviceId) + " : " + TelldusWrapper.TelldusNETWrapper.tdGetProtocol(deviceId),
                                getObjectType(MethodToString(TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_ALL))),
                                addrPrefix + deviceId, contSwitches, defaultTrustLevel, true);
                            o = OSAEObjectManager.GetObjectByAddress(addrPrefix + deviceId);
                            logD("getObject", o.Name, o.Address + " New object added.", o);
                            setOSAProperties(o, deviceId);
                            setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            setPropertyValue(o, propDevChgEvent, "TRUE");
                        }
                        else
                        {
                            logD("getObject", deviceId.ToString(), "New devices will not be added.");
                        }
                    }
                }
                else
                    logD("getObject", o.Name, o.Address + " Object found.", o);
                return o;
            }
            catch (Exception ex)
            {
                logE("getObject", "", "Exception in getObject: ", ex);
                OSAEObject o = null;
                return o;
            }
        }

        /// <summary>
        ///  Translates a Telldus data type into a readable string.
        /// </summary>
        /// <param name="dataType">Telldus data type.</param>
        /// <returns>Returns a readable string.</returns>
        private string dataTypeToString(int dataType)
        {
            string s = "";
            switch (dataType)
            {
                case TelldusNETWrapper.TELLSTICK_TEMPERATURE:
                    s = sensDTTemp;
                    break;
                case TelldusNETWrapper.TELLSTICK_HUMIDITY:
                    s = sensDTHum;
                    break;
                default:
                    s = "Unknown sensor type";
                    break;
            }
            return s;
        }

        /// <summary>
        ///  Translates a string with supported methods into an object type
        /// </summary>
        /// <param name="methods">Supported methods separated with a '-'.</param>
        /// <returns>Returns an object type.</returns>
        private string getObjectType(string methods)
        {
            string s = "";
            switch (methods)
            {
                case objMtdBell:
                    s = objTypeBell;
                    break;
                case objMtdBell + "-" + objMtdLearn:
                    s = objTypeBell;
                    break;
                case objMtdOn + "-" + objMtdOff:
                    s = objTypeOnOff;
                    break;
                case objMtdOn + "-" + objMtdOff + "-" + objMtdLearn:
                    s = objTypeOnOff;
                    break;
                case objMtdOn + "-" + objMtdOff + "-" + objMtdDim:
                    s = objTypeDim;
                    break;
                case objMtdOn + "-" + objMtdOff + "-" + objMtdDim + "-" + objMtdLearn:
                    s = objTypeDim;
                    break;
                case objMtdUp + "-" + objMtdDown + "-" + objMtdStop:
                    s = objTypeUpDown;
                    break;
                case objMtdUp + "-" + objMtdDown + "-" + objMtdStop + "-" + objMtdLearn:
                    s = objTypeUpDown;
                    break;
                default:
                    s = objTypeGroup;
                    break;
            }
            return s;
        }

        /// <summary>
        ///  initPlugin is called just after plugin start to initialise the plugin.
        ///  Will create the Tellstick object and its properties if they are missing.
        /// </summary>
        /// <param name="Name">The name of the plugin. This name will be used within OSA.</param>
        private void initPlugin(string Name)
        {
            OSAEObject o;
            if (!OSAEObjectManager.ObjectExists(Name))
            {
                logW("initPlugin", Name, "Object is missing!");
                OSAEObjectManager.ObjectAdd(Name, "", "Tellstick plugin", "TELLSTICK", "", "SYSTEM", 50, true);
                log("initPlugin", Name, "Object have been restored!");
            }
            o = OSAEObjectManager.GetObjectByName(Name);
            OSAEObjectType oType;
            //Added the following to automatically own relevant Base types that have no owner.
            oType = OSAEObjectTypeManager.ObjectTypeLoad(Name);
            logD("initPlugin", propDebug, "Checking on the Tellstick Object Type.");
            if (oType.OwnedBy != Name)
            {
                OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, Name, oType.BaseType, oType.Owner, oType.SysType, oType.Container, oType.HideRedundant,oType.Tooltip);
                logE("initPlugin", propDebug, Name + " Plugin took ownership of the Tellstick Object Type.", null);
            }
            debug = (getPropertyValue(o, propDebug).ToUpper() == "TRUE");
            addrPrefix = getPropertyValue(o, propAddrPrefix);
            addrPrefixSensor = getPropertyValue(o, propAddrPrefixSensor);
            rawEvents = (getPropertyValue(o, propRawEvents).ToUpper() == "TRUE");
            if (!int.TryParse(getPropertyValue(o, propAvgSensors), out sensorAverageTime))
            {
                logD("InitPlugin", propAvgSensors, " have an illegal value, default to 10", o);
                sensorAverageTime = 10;
            }
            logD("initPlugin", Name, "Initialise: " + propSystemPlugin + "=" + getPropertyValue(o, propSystemPlugin) + ", " + propAddrComputerName + "=" + getPropertyValue(o, propAddrComputerName) + ", " + propPort + "=" + getPropertyValue(o, propPort) + ", " + propDebug + "=" + debug);
            logD("initPlugin", Name, "Initialise: " + propAddrPrefix + "=" + addrPrefix + ", " + propAddrPrefixSensor + "=" + addrPrefixSensor);
            logD("initPlugin", Name, "Initialise: " + propAddDevices + "=" + getPropertyValue(o, propAddDevices) + ", " + propAddSensors + "=" + getPropertyValue(o, propAddSensors) + ", " + propRawEvents + "=" + rawEvents);
        }

        private int pluginCommands(OSAEObject o, string addr, OSAEMethod method)
        {
            try
            {
                int result = 0;
                switch (method.MethodName)
                {
                    case pluginMtdDebugOn:
                        if (debug != true)
                        {
                            debug = true;
                            log("pluginCommands", o.Name, "Method DebugOn, set debug to TRUE.");
                        }
                        else
                            log("pluginCommands", o.Name, "Method DebugOn, debug already set to TRUE.");
                        break;
                    case pluginMtdDebugOff:
                        if (debug != false)
                        {
                            debug = false;
                            log("pluginCommands", o.Name, "Method DebugOff, set debug to FALSE.");
                        }
                        else
                            log("pluginCommands", o.Name, "Method DebugOff, debug already set to FALSE.");
                        break;
                    case objMtdInit:
                        initPlugin(Name);
                        break;
                    default:
                        logE("pluginCommands", o.Name, method.MethodName + " Unsuported method: ", null);
                        break;
                }
              return result;
            }
            catch (Exception ex)
            {
                logE("pluginCommands", "", "Exception in pluginCommands: ", ex);
                return -1;
            }
        }
        
        private int doCommand(OSAEObject o, string addr, OSAEMethod method)
        {
            try
            {
                int deviceId = System.Convert.ToInt32(addr);
                int result = 0;
                if ((o.State.Value.ToUpper() != objStatDisable) || (method.MethodName.ToUpper() == objMtdEnable) ||
                     (o.Name == Name && method.MethodName.ToUpper() == objMtdInit))
                {
                    switch (method.MethodName)
                    {
                        case objMtdBell:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_BELL) > 0)
                            {
                                result = TelldusNETWrapper.tdBell(deviceId);
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.TELLSTICK_BELL);
                            }
                            else
                                logW("doCommand", o.Name, objMtdBell + " Method is not supported on this object.");
                            break;
                        case objMtdDim:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_DIM) > 0)
                            {
                                try
                                {
                                    result = TelldusNETWrapper.tdDim(deviceId, (char)((Int32)(Double.Parse(method.Parameter1) * (255 / 100.0))));
                                }
                                catch (Exception)
                                {
                                    result = TelldusNETWrapper.tdDim(deviceId, '0');
                                }
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdDim + " Method is not supported on this object.");
                            break;
                        case objMtdDown:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_DOWN) > 0)
                            {
                                result = TelldusNETWrapper.tdDown(deviceId);
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdDown + " Method is not supported on this object.");
                            break;
                        case objMtdUp:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_UP) > 0)
                            {
                                result = TelldusNETWrapper.tdUp(deviceId);
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdUp + " Method is not supported on this object.");
                            break;
                        case objMtdStop:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_STOP) > 0)
                            {
                                result = TelldusNETWrapper.tdStop(deviceId);
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdStop + " Method is not supported on this object.");
                            break;
                        case objMtdOn:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_TURNON) > 0)
                            {
                                result = TelldusNETWrapper.tdTurnOn(deviceId);
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdOn + " Method is not supported on this object.");
                            break;
                        case objMtdOff:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_TURNOFF) > 0)
                            {
                                result = TelldusNETWrapper.tdTurnOff(deviceId);
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdOff + " Method is not supported on this object.");
                            break;
                        case objMtdToggle:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_TOGGLE) > 0)
                            {
                                result = TelldusNETWrapper.TELLSTICK_ERROR_METHOD_NOT_SUPPORTED; // !!!!!!!
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdToggle + " Method is not supported on this object.");
                            break;
                        case objMtdExecute:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_EXECUTE) > 0)
                            {
                                result = TelldusNETWrapper.tdExecute(deviceId);
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdExecute + " Method is not supported on this object.");
                            break;
                        case objMtdLearn:
                            if (TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_LEARN) > 0)
                            {
                                result = TelldusNETWrapper.tdLearn(deviceId);
                                logResult(method.MethodName, result, o);
                                setLastProperties(o, deviceId, TelldusNETWrapper.tdLastSentCommand(deviceId, TelldusNETWrapper.TELLSTICK_ALL));
                            }
                            else
                                logW("doCommand", o.Name, objMtdLearn + " Method is not supported on this object.");
                            break;
                        case objMtdRestMaxMin:
                            switch (o.Type)
                            {
                                case objTypeHum:
                                    setPropertyValue(o, propMaxHum, o.Property(propHum).Value);
                                    setPropertyValue(o, propMinHum, o.Property(propHum).Value);
                                    log("doCommand", o.Name, " Reset Min/Max values.");
                                    break;
                                case objTypeTemp:
                                    setPropertyValue(o, propMaxTemp, o.Property(propTemp).Value);
                                    setPropertyValue(o, propMinTemp, o.Property(propTemp).Value);
                                    log("doCommand", o.Name, " Reset Min/Max values.");
                                    break;
                                case objTypeTempHum:
                                    setPropertyValue(o, propMaxHum, o.Property(propHum).Value);
                                    setPropertyValue(o, propMinHum, o.Property(propHum).Value);
                                    setPropertyValue(o, propMaxTemp, o.Property(propTemp).Value);
                                    setPropertyValue(o, propMinTemp, o.Property(propTemp).Value);
                                    log("doCommand", o.Name, " Reset Min/Max values.");
                                    break;
                                default:
                                    logE("doCommand", o.Name, " Reset Min/Max values.", null);
                                    break;
                            }
                            break;

                        case objMtdEnable:
                            switch (o.Type)
                            {
                                case objTypeHum:
                                case objTypeTemp:
                                case objTypeTempHum:
                                    o.SetState(objStatOff);
                                    log("doCommand", o.Name, objStatOff + " Device state set on object.");
                                    break;
                                default:
                                    o.SetState(objStatUnKnown);
                                    log("doCommand", o.Name, objStatUnKnown + " Device state set on object.");
                                    break;
                            }
                            break;
                        case objMtdDis:
                            o.SetState(objStatDisable);  // Should be done automatically on method with same name as state, but not working
                            log("doCommand", o.Name, objStatDisable + " Device state set on object.");
                            break;
                        case objMtdDebugOn:
                            if (debug != true)
                            {
                                debug = true;
                                log("doCommand", o.Name, "Method DebugOn, set debug to TRUE.");
                            }
                            else
                                log("doCommand", o.Name, "Method DebugOn, debug already set to TRUE.");
                            break;
                        case objMtdDebugOff:
                            if (debug != false)
                            {
                                debug = false;
                                log("doCommand", o.Name, "Method DebugOff, set debug to FALSE.");
                            }
                            else
                                log("doCommand", o.Name, "Method DebugOff, debug already set to FALSE.");
                            break;
                        default:
                            logE("doCommand", o.Name, method.MethodName + "Unsuported method: ", null);
                            break;

                    }
                } // if
                else
                    logW("doCommand", o.Name, objStatDisable + " Methods can not be used when object state is DISABLE!");
                return result;
            }
            catch (Exception ex)
            {
                logE("doCommand", "", "Exception in doCommand: ", ex);
                return -1;
            }
        }

        private void logResult(string methodName, int result, OSAEObject o)
        {
            if (result == 0)
                logD("doCommand", methodName, TelldusNETWrapper.tdGetErrorString(result), o);
            else
                logE("doCommand", methodName, TelldusNETWrapper.tdGetErrorString(result), null);
        }

        private string changeEventToString(int changeEvent)
        {
            string str = "";
            switch (changeEvent)
            {
                case TelldusNETWrapper.TELLSTICK_DEVICE_ADDED:
                    str = "Added";
                    break;
                case TelldusNETWrapper.TELLSTICK_DEVICE_CHANGED:
                    str = "Changed";
                    break;
                case TelldusNETWrapper.TELLSTICK_DEVICE_REMOVED:
                    str = "Removed";
                    break;
                case TelldusNETWrapper.TELLSTICK_DEVICE_STATE_CHANGED:
                    str = "State changed";
                    break;
                default:
                    str = "Unknown changeEvent: " + changeEvent;
                    break;
            }
            return str;
        }

        private string changeEventTypeToString(int changeType)
        {
            string str = "";
            switch (changeType)
            {
                case 0: // Needed when new device is added, missing in TelldusWrapper.cs
                    str = "";
                    break;
                case TelldusNETWrapper.TELLSTICK_CHANGE_MODEL:
                    str = "Model";
                    break;
                case TelldusNETWrapper.TELLSTICK_CHANGE_NAME:
                    str = "Name";
                    break;
                case TelldusNETWrapper.TELLSTICK_CHANGE_PROTOCOL:
                    str = "Protocol";
                    break;
                case TelldusNETWrapper.TELLSTICK_CHANGE_METHOD:
                    str = "Method";
                    break;
                case TelldusNETWrapper.TELLSTICK_CHANGE_AVAILABLE:
                    str = "Available";
                    break;
                case TelldusNETWrapper.TELLSTICK_CHANGE_FIRMWARE:
                    str = "Firmware";
                    break;
                default:
                    str = "Unknown changeType-" + changeType;
                    break;
            }
            return str;
        }

        private string deviceTypeToString(int deviceType)
        {
            string str = "";
            switch (deviceType)
            {
                case TelldusNETWrapper.TELLSTICK_TYPE_DEVICE:
                    str = "Device";
                    break;
                case TelldusNETWrapper.TELLSTICK_TYPE_GROUP:
                    str = "Group";
                    break;
                case TelldusNETWrapper.TELLSTICK_TYPE_SCENE:
                    str = "Scene";
                    break;
            }
            return str;
        }

        private void setMethods(int deviceId, OSAEObject o)
        {
            int Methods = TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_ALL);
            setPropertyValue(o, propMethods, MethodToString(Methods));
        }

        private void StatusDefault(OSAEObject o)        //static?
        {
            if (o.Name!=Name)           //Do not change the state of the plugin object!
                switch (o.Type)
                {
                    case objTypeHum:
                    case objTypeTemp:
                    case objTypeTempHum:
                        o.SetState(objStatOff);
                        //sensor s = new sensor();
                        //sensorList.Add(int.Parse(o.Property(propDevId).Value), s);
                        break;
                    default:
                        o.SetState(objStatUnKnown);
                        break;
                }
        }

        private static string MethodToString(int mtd)
        {
            string method = "";
            if ((mtd & TelldusNETWrapper.TELLSTICK_TURNON) > 0)
                method = method + objMtdOn + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_TURNOFF) > 0)
                method = method + objMtdOff + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_TOGGLE) > 0)
                method = method + objMtdToggle + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_DIM) > 0)
                method = method + objMtdDim + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_UP) > 0)
                method = method + objMtdUp + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_DOWN) > 0)
                method = method + objMtdDown + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_STOP) > 0)
                method = method + objMtdStop + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_BELL) > 0)
                method = method + objMtdBell + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_EXECUTE) > 0)
                method = method + objMtdExecute + "-";
            if ((mtd & TelldusNETWrapper.TELLSTICK_LEARN) > 0)
                method = method + objMtdLearn + "-";
            return method.Substring(0, method.Length - 1);
        }

        private void setLastProperties(OSAEObject o, int deviceId, int lastCmd)
        {
            setPropertyValue(o, propLastCmd, MethodToString(lastCmd));
            switch (lastCmd)
            {
                case TelldusNETWrapper.TELLSTICK_TURNON:
                    setPropertyValue(o, propLevel, "100");
                    o.SetState(objStatOn);
                    break;
                case TelldusNETWrapper.TELLSTICK_TURNOFF:
                    setPropertyValue(o, propLevel, "0");
                    o.SetState(objStatOff);
                    break;
                case TelldusNETWrapper.TELLSTICK_DIM:
                    try
                    {
                        setPropertyValue(o, propLevel, (Decimal.Round((Decimal)(Int32.Parse(TelldusNETWrapper.tdLastSentValue(deviceId)) * (100 / 255.0))).ToString()));
                        if (TelldusNETWrapper.tdLastSentValue(deviceId) != "0")
                            o.SetState(objStatOn);
                        else
                            o.SetState(objStatOff);
                    }
                    catch (Exception)
                    {
                        setPropertyValue(o, propLevel, "");
                        o.SetState(objStatOn);
                    }
                    break;
                case TelldusNETWrapper.TELLSTICK_TOGGLE:
                    if (getPropertyValue(o, propLastCmd) == objEveOn)    // Previous value!
                        o.SetState(objStatOff);
                    else
                        o.SetState(objStatOn);
                    break;
                case TelldusNETWrapper.TELLSTICK_BELL:
                case TelldusNETWrapper.TELLSTICK_DOWN:
                case TelldusNETWrapper.TELLSTICK_EXECUTE:
                case TelldusNETWrapper.TELLSTICK_STOP:
                case TelldusNETWrapper.TELLSTICK_UP:
                    setPropertyValue(o, propLevel, "");
                    o.SetState(objStatOn);
                    break;
                default:
                    o.SetState(objStatUnKnown);
                    break;
            }
        }

        private void setOSAProperties(OSAEObject o, int deviceId)
        {

            try { int Methods = TelldusNETWrapper.tdMethods(deviceId, TelldusNETWrapper.TELLSTICK_ALL); }
            catch { logW("setOSAProperties", o.Name, "Problem when reading Methods."); }
            setPropertyValue(o, propDevId, deviceId.ToString());
            setPropertyValue(o, propModel, TelldusNETWrapper.tdGetModel(deviceId));
            setPropertyValue(o, propDevType, deviceTypeToString(TelldusNETWrapper.tdGetDeviceType(deviceId)));
            setPropertyValue(o, propProtocol, TelldusNETWrapper.tdGetProtocol(deviceId));
            try { setMethods(deviceId, o); }
            catch { logW( "setOSAProperties", o.Name, propMethods + " Problem when setting property."); }
            setPropertyValue(o, propDeviceaddr, TelldusNETWrapper.tdGetDeviceParameter(deviceId, "house", "") + "/" + TelldusWrapper.TelldusNETWrapper.tdGetDeviceParameter(deviceId, "code", "") + "/" + TelldusWrapper.TelldusNETWrapper.tdGetDeviceParameter(deviceId, "unit", ""));
        }

        private string getSensorParameter(int deviceId, string parameter)
        {
            // This code does not work when run from a sercvice. HKEY_CURRENT_USER does not exist for a service!
            try
            {
                int noSensors = 0;
                string size = "";
                size = (string)Registry.GetValue(pathSensors, "size", null);
                noSensors = Int32.Parse(size);  // Number of sensors in registry
                for (int x = 0; x < noSensors; x = x + 1)
                {
                    if ((string)Registry.GetValue(pathSensors + "\\" + x + "\\values", "id", null) == deviceId.ToString())
                        return (string)Registry.GetValue(pathSensors + "\\" + x + "\\values", parameter, null);
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private OSAEObject createSensorObject(int deviceId, int dataType, string protocol, string model)
        {
            try
            {
                OSAEObject o = null;
                string name = addrPrefixSensor + deviceId;
                switch (model)
                {
                    case sensDTTemp:
                        OSAEObjectManager.ObjectAdd(name, "", protocol + "-" + model, objTypeTemp, addrPrefixSensor + deviceId, contSensors, defaultTrustLevel, true);
                        break;
                    case sensDTHum:
                        OSAEObjectManager.ObjectAdd(name, "", protocol + "-" + model, objTypeHum, addrPrefixSensor + deviceId, contSensors, defaultTrustLevel, true);
                        break;
                    case sensDTTempHum:
                        OSAEObjectManager.ObjectAdd(name, "", protocol + "-" + model, objTypeTempHum, addrPrefixSensor + deviceId, contSensors, defaultTrustLevel, true);
                        break;
                    default:
                        logE("createSensorObject", model, "Unknown model.", null);
                        break;
                }
                o = OSAEObjectManager.GetObjectByAddress(addrPrefixSensor + deviceId);
                setPropertyValue(o, propDevType, "Sensor");
                setPropertyValue(o, propDevId, deviceId.ToString());
                setPropertyValue(o, propModel, getSensorParameter(deviceId, "model"));
                setPropertyValue(o, propProtocol, getSensorParameter(deviceId, "protocol"));
                //o.SetProperty(propDevSensEvent, "TRUE", Name);
                log("createSensorObject", o.Name, o.Address + " Sensor object created.");
                return o;
            }
            catch (Exception ex)
            {
                logE("createSensorObject", "", "Exception in createSensorObject: ", ex);
                return null;
            }
        }

        private void log(string procedur, string value, string text)
        {
            logger.Info(procedur.PadRight(v) + value.PadRight(w) + text);
        }

        private void logE(string procedur, string value, string text, Exception ex)
        {
            logger.Error(procedur.PadRight(v) + pos.ToString().PadRight(3) + value.PadRight(w) + text, ex);
        }

        private void logW(string procedur, string value, string text)
        {
            logger.Info(procedur.PadRight(v) + value.PadRight(w) + "Warning! - " + text);
        }

        private void logD(string procedur, string value, string text, OSAEObject o)
        {
            try
            {
                if (debug) logger.Debug(procedur.PadRight(v) + pos.ToString().PadRight(3) + value.PadRight(w) + text);
                
            }
            catch
            {
            }
        }

        private void logD(string procedur, string value, string text)
        {
            if (debug) logger.Debug(procedur.PadRight(v) + pos.ToString().PadRight(3) + value.PadRight(w) + text);
            }
        }



    }
