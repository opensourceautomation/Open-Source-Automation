namespace OSAE.Nest
{
    using System;
	using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using System.Net.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using FirebaseSharp.Portable;
    using System.Threading;
    using System.Timers;

    public class Nest : OSAEPluginBase
    {
        private static OSAE.General.OSAELog Log;

        /// <summary>
        /// Plugin name
        /// </summary>
        string pName;
        string accessToken;

        System.Timers.Timer Clock = new System.Timers.Timer();
        Thread updateThread;

        /// <summary>
        /// OSA Plugin Interface - called on start up to allow plugin to do any tasks it needs
        /// </summary>
        /// <param name="pluginName">The name of the plugin from the system</param>
        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            Log = new General.OSAELog(pName);

            //make sure object types are owned by the Nest Plugin
            OSAEObjectTypeManager.ObjectTypeUpdate("NEST STRUCTURE", "NEST STRUCTURE", "Nest Structure", pluginName, "NEST STRUCTURE", false, false, false, true, "Nest Structure");
            OSAEObjectTypeManager.ObjectTypeUpdate("NEST THERMOSTAT", "NEST THERMOSTAT", "Nest Thermostat", pluginName, "NEST THERMOSTAT", false, false, false, true, "Nest Thermostat");
            OSAEObjectTypeManager.ObjectTypeUpdate("NEST PROTECT", "NEST PROTECT", "Nest Protect", pluginName, "NEST PROTECT", false, false, false, true, "Nest Protect");

            Log.Info("Starting Nest...");


            Ititialize("startup");

            //heartbeat to check online devices
            int interval = 10; //in minutes

            //bool isNum = Int32.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Poll Interval").Value, out interval);
            Clock = new System.Timers.Timer();
            //if (isNum)
            Clock.Interval = interval * 60 * 1000;
            //else
            //    Clock.Interval = 60000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);
            updateThread = new Thread(new ThreadStart(update));
        }

        private void Ititialize(string calledfrom)
        {
            OSAEObject pObject = OSAEObjectManager.GetObjectByName(pName);

            string code = pObject.Property("Pin").Value;
            accessToken = pObject.Property("Access Token").Value;
            
            if (accessToken.Equals(""))
            {
                if (code.Equals("")) //&& calledfrom.Equals("method")
                    Log.Info("No pin provided. Please visit https://home.nest.com/login/oauth2?client_id=ff5f73fb-2fd9-472f-ba3d-73cb236a1808&state=STATE to get a pin code. Update PIN property on Nest Plugin object and run method Get Access Token. ");

                accessToken = GetAccessToken(code);
                OSAEObjectPropertyManager.ObjectPropertySet(pName, "Access Token", accessToken, pName);
            }
            GetNestData(accessToken);
            SubscribeToNestDataUpdates(accessToken);
        }

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            
            if (sender == Clock)
            {
                if (!updateThread.IsAlive)
                {
                    updateThread = new Thread(new ThreadStart(update));
                    updateThread.Start();
                }
            }
        }

        public void update()
        {
            if (GetNestData(accessToken))
            {
                Log.Info("data doesn't match. Our streaming updates may be stopped. Lets start them back. ");
                SubscribeToNestDataUpdates(accessToken);
            }
        }


        /// <summary>
        /// OSA Plugin Interface - Commands the be processed by the plugin
        /// </summary>
        /// <param name="method">Method containging the command to run</param>
        public override void ProcessCommand(OSAEMethod method)
        {
            // place the plugin command code here leave empty if unable to process commands

            Log.Debug("Found Command: name: " + method.MethodName + " | label: " + method.MethodLabel + " | param1: " + method.Parameter1 + " | param2: " + method.Parameter2 + " | obj: " + method.ObjectName + " | addr: " + method.Address);

            OSAEObject methodObject = OSAEObjectManager.GetObjectByName(method.ObjectName);
            string returndata = "returndata=Method Name " + method.MethodName + " not found"; //will get logged if no method is matched below
            string warningdata = "";
            bool abortcommand = false;

            if (method.MethodName.Equals("SEARCH FOR NEW DEVICES"))
            {
                GetNestData(accessToken);
                returndata = "returndata=Search Complete";
            }

            if (method.MethodName.Equals("RESTART STREAMING UPDATES"))
            {
                SubscribeToNestDataUpdates(accessToken);
                returndata = "returndata=Restarted";
            }

            if (method.MethodName.Equals("GET ACCESS TOKEN"))
            {
                Ititialize("method");
                returndata = "returndata=Requested Access Token";
            }

            if (methodObject.Type.Equals("NEST STRUCTURE"))
            {
                string type = "structures";
                if (method.MethodName.Equals("AWAY"))
                    returndata = "returndata="+UpdateNestData(type, methodObject.Address, "{\"away\":\"away\"}", accessToken);

                if (method.MethodName.Equals("HOME"))
                    returndata = "returndata=" + UpdateNestData(type, methodObject.Address, "{\"away\":\"home\"}", accessToken);

                if (method.MethodName.Equals("SET ETA"))
                {
                    if (methodObject.State.Value.ToUpper().Equals("HOME"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true; 
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the eta when Structure is set to Home";
                    }
                    

                    string tripid = method.Parameter1;
                    string arrivalbegin;
                    string arrivalend;

                    string[] parts = method.Parameter2.Split(',');

                    arrivalbegin = parts[0];

                    DateTime arrivalbeginDate;
                    DateTime arrivalendDate;
                    bool invalidDate = false;

                    if (!DateTime.TryParse(arrivalbegin, out arrivalbeginDate))
                    {
                        invalidDate = true;
                        Log.Error("invalid date entered for eta: " + arrivalbegin);
                    }

                    if (parts.Length >= 2)
                    {
                        arrivalend = parts[1];
                        if (!DateTime.TryParse(arrivalend, out arrivalendDate))
                        {
                            invalidDate = true;
                            Log.Error("invalid date entered for eta: " + arrivalend);
                        }
                    }
                    else
                    {
                        TimeSpan time = new TimeSpan(0, 1, 0, 0);
                        arrivalendDate = arrivalbeginDate.Add(time);
                    }

                    Log.Debug("about to send these dates - arrival begin: " + arrivalbeginDate.ToString() + ", arrival end: " + arrivalendDate.ToString());

                    string UTCarrivalbeginDate = arrivalbeginDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff") + "Z";
                    string UTCarrivalendDate = arrivalendDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff") + "Z";

                    if (invalidDate && !abortcommand)
                    {
                        abortcommand = true;
                        returndata = "returndata= Method Name " + method.MethodName + " was found, but dates passed as parameters could not be parsed";
                    }
                    
                    if (!abortcommand)
                    {
                        string data = "{\"trip_id\": \"" + tripid + "\",\"estimated_arrival_window_begin\": \"" + UTCarrivalbeginDate + "\",\"estimated_arrival_window_end\": \"" + UTCarrivalendDate + "\"}";
                        returndata = UpdateNestData(type, methodObject.Address + "/eta", data, accessToken);
                        returndata = warningdata + System.Environment.NewLine + "returndata=" + returndata;
                    }   
                }

                if (method.MethodName.Equals("CANCEL ETA"))
                {
                    string tripid = method.Parameter1;
                    string data = "{\"trip_id\": \"" + tripid + "\",\"estimated_arrival_window_begin\": 0}";
                    //string data = "{\"estimated_arrival_window_begin\": 0}";
                    returndata = "returndata=" + UpdateNestData(type, methodObject.Address + "/eta", data, accessToken);
                }
            }

            if (methodObject.Type.Equals("NEST THERMOSTAT"))
            {
                string type = "devices/thermostats";
                if (method.MethodName.Equals("HEAT"))
                    returndata = "returndata=" + UpdateNestData(type, methodObject.Address, "{\"hvac_mode\":\"heat\"}", accessToken);

                if (method.MethodName.Equals("COOL"))
                    returndata = "returndata=" + UpdateNestData(type, methodObject.Address, "{\"hvac_mode\":\"cool\"}", accessToken);

                if (method.MethodName.Equals("HEAT-COOL"))
                    returndata = "returndata=" + UpdateNestData(type, methodObject.Address, "{\"hvac_mode\":\"heat-cool\"}", accessToken);

                if (method.MethodName.Equals("OFF"))
                    returndata = "returndata=" + UpdateNestData(type, methodObject.Address, "{\"hvac_mode\":\"off\"}", accessToken);

                if (method.MethodName.Equals("SET TARGET"))
                {
                    if (!methodObject.State.Value.ToUpper().Equals("HEAT") && !methodObject.State.Value.ToUpper().Equals("COOL"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target temperature unless thermostat is in heat mode or cool mode";
                    }
                    if (!IsMyStructureHome(methodObject))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target temperature if the house is not in Home mode";
                    }
                    if (methodObject.Property("Is Online").Value.ToLower().Equals("false"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target temperature if the thermostat is offline";
                    }
                    if (methodObject.Property("Using Emergency Heat").Value.ToLower().Equals("true"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target temperature if the thermostat is using emergency heat";
                    }
                    if (!IsTempOK("normal", method.Parameter1,methodObject))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target temperature because it violates contraints. Temp must be between 50 and 90 (F) or 9 and 32 (C)";
                    }
                    if (!abortcommand)
                    {
                        string temp = method.Parameter1;
                        string temp_scale = methodObject.Property("Temperature Scale").Value.ToLower();
                        returndata = UpdateNestData(type, methodObject.Address, "{\"target_temperature_" + temp_scale + "\":" + temp + "}", accessToken);
                        returndata = warningdata + System.Environment.NewLine + "returndata=" + returndata;
                    }
                }
                if (method.MethodName.Equals("SET TARGET HIGH"))
                {
                    if (!methodObject.State.Value.ToUpper().Equals("HEAT-COOL"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target high temperature unless thermostat is in heat-cool mode";
                    }
                    if (!IsMyStructureHome(methodObject))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target high temperature if the house is not in Home mode";
                    }
                    if (methodObject.Property("Is Online").Value.ToLower().Equals("false"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target high temperature if the thermostat is offline";
                    }
                    if (methodObject.Property("Using Emergency Heat").Value.ToLower().Equals("true"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target high temperature if the thermostat is using emergency heat";
                    }
                    if (!IsTempOK("normal", method.Parameter1, methodObject))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target high temperature because it violates contraints. Temp must be between 50 and 90 (F) or 9 and 32 (C), also the target high temp must be higher than the target low temp";
                    }
                    if (!abortcommand)
                    {
                        string temp = method.Parameter1;
                        string temp_scale = methodObject.Property("Temperature Scale").Value.ToLower();
                        returndata = UpdateNestData(type, methodObject.Address, "{\"target_temperature_high_" + temp_scale + "\":" + temp + "}", accessToken);
                        returndata = warningdata + System.Environment.NewLine + "returndata=" + returndata;
                    }
                }
                if (method.MethodName.Equals("SET TARGET LOW"))
                {
                    if (!methodObject.State.Value.ToUpper().Equals("HEAT-COOL"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target low temperature unless thermostat is in heat-cool mode";
                    }
                    if (!IsMyStructureHome(methodObject))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target low temperature if the house is not in Home mode";
                    }
                    if (methodObject.Property("Is Online").Value.ToLower().Equals("false"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target low temperature if the thermostat is offline";
                    }
                    if (methodObject.Property("Using Emergency Heat").Value.ToLower().Equals("true"))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target low temperature if the thermostat is using emergency heat";
                    }
                    if (!IsTempOK("normal", method.Parameter1, methodObject))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot set the target low temperature because it violates contraints. Temp must be between 50 and 90 (F) or 9 and 32 (C), also the target low temp must be lower than the target high temp";
                    }
                    if (!abortcommand)
                    {
                        string temp = method.Parameter1;
                        string temp_scale = methodObject.Property("Temperature Scale").Value.ToLower();
                        returndata = UpdateNestData(type, methodObject.Address, "{\"target_temperature_low_" + temp_scale + "\":" + temp + "}", accessToken);
                        returndata = warningdata + System.Environment.NewLine + "returndata=" + returndata;
                    }
                }
                if (method.MethodName.Equals("TURN ON FAN"))
                {
                    if (!IsMyStructureHome(methodObject))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot turn on the fan if the house is not in Home mode";
                    }
                    if (!abortcommand)
                    {
                        returndata = UpdateNestData(type, methodObject.Address, "{\"fan_timer_active\":true}", accessToken);
                        returndata = warningdata + System.Environment.NewLine + "returndata=" + returndata;
                    }
                }
                if (method.MethodName.Equals("TURN OFF FAN"))
                {
                    if (!IsMyStructureHome(methodObject))
                    {
                        //go ahead and try command in case our data is outdated
                        //abortcommand = true;
                        warningdata = "warningdata=Method Name " + method.MethodName + " was found, but we cannot turn off the fan if the house is not in Home mode";
                    }
                    if (!abortcommand)
                    {
                        returndata = UpdateNestData(type, methodObject.Address, "{\"fan_timer_active\":false}", accessToken);
                        returndata = warningdata + System.Environment.NewLine + "returndata=" + returndata;
                    }
                }
            }
            Log.Info(returndata);
        }
        
        /// <summary>
        /// OSA Plugin Interface - The plugin has been asked to shut down
        /// </summary>        
        public override void Shutdown()
        {
			Log.Info("Stopping Nest...");
        }

        private string UpdateNestData(string type, string id, string data, string accessToken)
        {
            string r="";
            if (!accessToken.Equals(""))
            {
                string path = type + "/" + id;
                Log.Debug("about to send update to nest: path=" + path + ", value=" + data + "");
                var firebaseClient = new Firebase("https://developer-api.nest.com", accessToken);
                try
                {
                    r = firebaseClient.Put(path, data);
                }
                catch (Exception e)
                {
                    string[] lines = e.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    r = "Error updating firebase: " + lines[0];
                }
                firebaseClient.Dispose();
            }
            return r;
        }

        //returns true if data has changed (used to try to determine when data streaming has stopped working)
        private Boolean GetNestData(string accessToken)
        {
            bool datachanged = false;
            if (!accessToken.Equals(""))
            {
                var firebaseClient = new Firebase("https://developer-api.nest.com", accessToken);
                //get structure data
                try
                {
                    string structureData = firebaseClient.Get("structures");
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(structureData);
                    var myStructures = new List<Structure>();
                    foreach (var itemDynamic in data)
                        myStructures.Add(JsonConvert.DeserializeObject<Structure>(((JProperty)itemDynamic).Value.ToString()));

                    foreach (Structure structure in myStructures)
                    {
                        //if structure does not exist then add it
                        if (!OSAEObjectManager.ObjectExists(structure.structure_id))
                        {
                            Log.Debug("about to add structure: " + structure.name + " address: " + structure.structure_id + ".");
                            OSAEObject _nameObject = OSAEObjectManager.GetObjectByName(structure.name);
                            //OSAEObjectManager.GetObjectByAddress(structure.structure_id)
                            if (_nameObject != null)
                                Log.Error("Object with name: " + _nameObject.Name + " exists but it's address doesn't match - objects address: " + _nameObject.Address + ". - device_id from nest:" + structure.structure_id + ".");

                            OSAEObjectManager.ObjectAdd(structure.name,"", structure.name, "NEST STRUCTURE", structure.structure_id, "",30, true);
                        }
                        string objectName = OSAEObjectManager.GetObjectByAddress(structure.structure_id).Name;
                        //load current data into structure object
                        Structure currentdata = new Structure();
                        currentdata.loadData(objectName);
                        //compare structure data to data we have
                        if (!structure.SameData(currentdata))
                        {
                            datachanged = true;
                            // list the changes temporarily to see how it is working
                            List<Variance> variances = structure.DetailedCompare(currentdata);
                            foreach (Variance v in variances)
                                Log.Debug(v.Prop + " value changed from " + v.valB.ToString() + " to " + v.valA.ToString());
                        }
                        //update structure data
                        OSAEObjectStateManager.ObjectStateSet(objectName, structure.away, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Id", structure.structure_id, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name", structure.name, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Country Code", structure.country_code, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Postal Code", structure.postal_code, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Peak Start Time", structure.peak_period_start_time, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Peak End Time", structure.peak_period_end_time, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Time Zone", structure.time_zone, pName);
                    }
                }
                catch (Exception e)
                {
                    string[] lines = e.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    Log.Error("error getting structure data. " + lines[0]);
                }

                //get thermostat data
                try
                {
                    string thermostatData = firebaseClient.Get("devices/thermostats");

                    dynamic data = JsonConvert.DeserializeObject<dynamic>(thermostatData);
                    var myThermostats = new List<Thermostat>();
                    foreach (var itemDynamic in data)
                        myThermostats.Add(JsonConvert.DeserializeObject<Thermostat>(((JProperty)itemDynamic).Value.ToString()));

                    foreach (Thermostat thermostat in myThermostats)
                    {
                        //if thermostat does not exist then add it
                        if (!OSAEObjectManager.ObjectExists(thermostat.device_id))
                        {
                            Log.Debug("about to add thermostat: " + thermostat.name_long + " address: " + thermostat.device_id);
                            OSAEObject _nameObject = OSAEObjectManager.GetObjectByName(thermostat.name);
                            if (_nameObject != null)
                                Log.Error("Object with name: " + _nameObject.Name + " exists but it's address doesn't match - objects address: " + _nameObject.Address + ". - device_id from nest:" + thermostat.structure_id + ".");

                            OSAEObjectManager.ObjectAdd(thermostat.name,"", thermostat.name_long, "NEST THERMOSTAT", thermostat.device_id, "", 30, true);
                        }
                        string objectName = OSAEObjectManager.GetObjectByAddress(thermostat.device_id).Name;

                        //load current data into thermostat object
                        Thermostat currentdata = new Thermostat();
                        currentdata.loadData(objectName);
                        //compare thermostat data to data we have
                        thermostat.tempToScale(); // set all temps to current scale for comparision since we don't save both in OSA object (will have to load current scale into both _c and _f fields)
                        thermostat.convertLastConnection(); //converts last connection to current time zone and to the same format that OSA will return for comaprison

                        if (!thermostat.SameData(currentdata))
                        {
                            datachanged = true;
                            // list the changes temporarily to see how it is working
                            List<Variance> variances = thermostat.DetailedCompare(currentdata);
                            Log.Debug("Number of fields changed: " + variances.Count);
                            foreach (Variance v in variances)
                                Log.Debug(v.Prop + " value changed from " + v.valB.ToString() + " to " + v.valA.ToString());
                        }

                        //update structure data
                        if (thermostat.is_online)
                            OSAEObjectStateManager.ObjectStateSet(objectName, thermostat.hvac_mode, pName);
                        else
                            OSAEObjectStateManager.ObjectStateSet(objectName, "Offline", pName);

                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Id", thermostat.device_id, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name", thermostat.name, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name Long", thermostat.name_long, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Ambient Temperature", (thermostat.temperature_scale == "F") ? thermostat.ambient_temperature_f.ToString() : thermostat.ambient_temperature_c.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Humidity", thermostat.humidity.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature", (thermostat.temperature_scale == "F") ? thermostat.target_temperature_f.ToString() : thermostat.target_temperature_c.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature High", (thermostat.temperature_scale == "F") ? thermostat.target_temperature_high_f.ToString() : thermostat.target_temperature_high_c.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature Low", (thermostat.temperature_scale == "F") ? thermostat.target_temperature_low_f.ToString() : thermostat.target_temperature_low_c.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Away Temperature High", (thermostat.temperature_scale == "F") ? thermostat.away_temperature_high_f.ToString() : thermostat.away_temperature_high_c.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Away Temperature Low", (thermostat.temperature_scale == "F") ? thermostat.away_temperature_low_f.ToString() : thermostat.away_temperature_low_c.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Has Leaf", thermostat.has_leaf.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Temperature Scale", thermostat.temperature_scale, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Locale", thermostat.locale, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Software Version", thermostat.software_version, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Structure ID", thermostat.structure_id, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Last Connection", thermostat.last_connection, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Is Online", thermostat.is_online.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Can Cool", thermostat.can_cool.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Can Heat", thermostat.can_heat.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Using Emergency Heat", thermostat.is_using_emergency_heat.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Has Fan", thermostat.has_fan.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Fan Timer Active", thermostat.fan_timer_active.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Fan Timer Timeout", thermostat.fan_timer_timeout, pName);
                    }
                }
                catch (Exception e)
                {
                    string[] lines = e.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    Log.Error("error getting thermostat data. " + lines[0]);
                }

                //get smokealarm data
                try
                {
                    string smokeData = firebaseClient.Get("devices/smoke_co_alarms");

                    dynamic data = JsonConvert.DeserializeObject<dynamic>(smokeData);
                    var mySmokeAlarms = new List<SmokeAlarm>();
                    foreach (var itemDynamic in data)
                        mySmokeAlarms.Add(JsonConvert.DeserializeObject<SmokeAlarm>(((JProperty)itemDynamic).Value.ToString()));

                    foreach (SmokeAlarm smokealarm in mySmokeAlarms)
                    {
                        //if smokealarm does not exist then add it
                        if (!OSAEObjectManager.ObjectExists(smokealarm.device_id))
                        {
                            Log.Debug("about to add nest protect: " + smokealarm.name_long + " address: " + smokealarm.device_id);

                            OSAEObject _nameObject = OSAEObjectManager.GetObjectByName(smokealarm.name);
                            if (_nameObject != null)
                                Log.Error("Object with name: " + _nameObject.Name + " exists but it's address doesn't match - objects address: " + _nameObject.Address + ". - device_id from nest:" + smokealarm.structure_id + ".");

                            OSAEObjectManager.ObjectAdd(smokealarm.name,"", smokealarm.name_long, "NEST PROTECT", smokealarm.device_id, "", 30, true);
                        }
                        string objectName = OSAEObjectManager.GetObjectByAddress(smokealarm.device_id).Name;

                        //load current data into smokealarm object
                        SmokeAlarm currentdata = new SmokeAlarm();
                        smokealarm.convertLastConnection();
                        currentdata.loadData(objectName);
                        //compare smokealarm data to data we have
                        if (!smokealarm.SameData(currentdata))
                        {
                            datachanged = true;
                            // list the changes temporarily to see how it is working
                            List<Variance> variances = smokealarm.DetailedCompare(currentdata);
                            Log.Debug("Number of fields changed: " + variances.Count);
                            foreach (Variance v in variances)
                                Log.Debug(v.Prop + " value changed from " + v.valB.ToString() + " to " + v.valA.ToString());
                        }

                        //update structure data
                        if (smokealarm.is_online)
                        {
                            if (smokealarm.smoke_alarm_state.Equals("emergency"))
                                OSAEObjectStateManager.ObjectStateSet(objectName, "Smoke Emergency", pName);
                            else if (smokealarm.co_alarm_state.Equals("emergency"))
                                OSAEObjectStateManager.ObjectStateSet(objectName, "CO Emergency", pName);
                            else if (smokealarm.smoke_alarm_state.Equals("warning"))
                                OSAEObjectStateManager.ObjectStateSet(objectName, "Smoke Warning", pName);
                            else if (smokealarm.co_alarm_state.Equals("warning"))
                                OSAEObjectStateManager.ObjectStateSet(objectName, "CO Warning", pName);
                            else if (smokealarm.battery_health.Equals("replace"))
                                OSAEObjectStateManager.ObjectStateSet(objectName, "Battery Replace", pName);
                            else
                                OSAEObjectStateManager.ObjectStateSet(objectName, "Online", pName);
                        }
                        else
                            OSAEObjectStateManager.ObjectStateSet(objectName, "Offline", pName);

                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Id", smokealarm.device_id, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name", smokealarm.name, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name Long", smokealarm.name_long, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Battery Health", smokealarm.battery_health, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "CO Alarm State", smokealarm.co_alarm_state, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Smoke Alarm State", smokealarm.smoke_alarm_state, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Manual Test Active", smokealarm.is_manual_test_active.ToString(), pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Last Manual Test", smokealarm.last_manual_test_time, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "UI Color State", smokealarm.ui_color_state, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Locale", smokealarm.locale, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Software Version", smokealarm.software_version, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Structure ID", smokealarm.structure_id, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Last Connection", smokealarm.last_connection, pName);
                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Is Online", smokealarm.is_online.ToString(), pName);
                    }
                }
                catch (Exception e)
                {
                    string[] lines = e.ToString().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    Log.Debug("error getting smokealarm data. " + lines[0]);
                }
                firebaseClient.Dispose();
            }            
            return datachanged;
        }

        private void SubscribeToNestDataUpdates(string accessToken)
        {
            if (!accessToken.Equals(""))
            {
                var firebaseClient = new Firebase("https://developer-api.nest.com", accessToken);
                //suscribe to structure data
                try
                {
                    var response_structure = firebaseClient.GetStreaming("structures",
                    changed: (s, e) =>
                    {
                        if (!e.Data.Equals(e.OldData) || e.Data == null || e.OldData == null)
                        {
                            string[] parts = e.Path.Split('/');
                            string deviceid = parts[parts.Count() - 2];
                            string dataelement = parts[parts.Count() - 1];
                            Log.Debug("device(" + deviceid + ") " + dataelement + " changed from " + e.OldData + " to " + e.Data + ".");
                            string objectName = OSAEObjectManager.GetObjectByAddress(deviceid).Name;
                            if (dataelement.Equals("away"))
                                OSAEObjectStateManager.ObjectStateSet(objectName, e.Data, pName);

                            if (dataelement.Equals("name"))
                                OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name", e.Data, pName);

                            if (dataelement.Equals("country_code"))
                                OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Country Code", e.Data, pName);

                            if (dataelement.Equals("postal_code"))
                                OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Postal Code", e.Data, pName);

                            if (dataelement.Equals("time_zone"))
                                OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Time Zone", e.Data, pName);

                            if (dataelement.Equals("peak_period_start_time"))
                                OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Peak Start Time", e.Data, pName);

                            if (dataelement.Equals("peak_period_end_time"))
                                OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Peak End Time", e.Data, pName);
                        }
                    });
                }
                catch (Exception)
                { Log.Error("error getting structure updates"); }

                //suscribe to device data
                try
                {
                    var response_thermostat = firebaseClient.GetStreaming("devices",
                    changed: (s, e) =>
                    {
                        if (!e.Data.Equals(e.OldData) || e.Data == null || e.OldData == null)
                        {
                            string[] parts = e.Path.Split('/');

                            string deviceid = parts[parts.Count() - 2];
                            string dataelement = parts[parts.Count() - 1];

                            Log.Debug("device(" + deviceid + ") " + dataelement + " changed from " + e.OldData + " to " + e.Data + ".");

                            string objectName = OSAEObjectManager.GetObjectByAddress(deviceid).Name;
                            string objectType = OSAEObjectManager.GetObjectByAddress(deviceid).Type;
                            string tempScale = OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Temperature Scale").Value;
                            if (objectType.Equals("NEST THERMOSTAT"))
                            {
                                if (dataelement.Equals("hvac_mode"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, e.Data, pName);

                                if (dataelement.Equals("is_online") && e.Data.Equals("False"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, "OFFLINE", pName);

                                if (dataelement.Equals("name"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name", e.Data, pName);

                                if (dataelement.Equals("name_long"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name Long", e.Data, pName);

                                if (dataelement.Equals("device_id"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "ID", e.Data, pName);

                                if (dataelement.Equals("locale"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Locale", e.Data, pName);

                                if (dataelement.Equals("software_version"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Software Version", e.Data, pName);

                                if (dataelement.Equals("structure_id"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Structure ID", e.Data, pName);

                                if (dataelement.Equals("last_connection"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Last Connection", convertDate(e.Data), pName);

                                if (dataelement.Equals("is_online"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Is Online", e.Data, pName);

                                if (dataelement.Equals("can_cool"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Can Cool", e.Data, pName);

                                if (dataelement.Equals("can_heat"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Can Heat", e.Data, pName);

                                if (dataelement.Equals("is_using_emergency_heat"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Using Emergency Heat", e.Data, pName);

                                if (dataelement.Equals("has_fan"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Has Fan", e.Data, pName);

                                if (dataelement.Equals("fan_timer_active"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Fan Timer Active", e.Data, pName);

                                if (dataelement.Equals("fan_timer_timeout"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Fan Timer Timeout", e.Data, pName);

                                if (dataelement.Equals("has_leaf"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Has Leaf", e.Data, pName);

                                if (dataelement.Equals("temperature_scale"))
                                {
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Temperature Scale", e.Data, pName);
                                    GetNestData(accessToken); // redownload all data so temps will be displayed in new scale
                                }
                                if (dataelement.Equals("target_temperature_f") && tempScale.Equals("F"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature", e.Data, pName);

                                if (dataelement.Equals("target_temperature_c") && tempScale.Equals("C"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature", e.Data, pName);

                                if (dataelement.Equals("target_temperature_high_f") && tempScale.Equals("F"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature High", e.Data, pName);

                                if (dataelement.Equals("target_temperature_high_c") && tempScale.Equals("C"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature High", e.Data, pName);

                                if (dataelement.Equals("target_temperature_low_f") && tempScale.Equals("F"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature Low", e.Data, pName);

                                if (dataelement.Equals("target_temperature_low_c") && tempScale.Equals("C"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Target Temperature Low", e.Data, pName);

                                if (dataelement.Equals("away_temperature_high_f") && tempScale.Equals("F"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Away Temperature High", e.Data, pName);

                                if (dataelement.Equals("away_temperature_high_c") && tempScale.Equals("C"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Away Temperature High", e.Data, pName);

                                if (dataelement.Equals("away_temperature_low_f") && tempScale.Equals("F"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Away Temperature Low", e.Data, pName);

                                if (dataelement.Equals("away_temperature_low_c") && tempScale.Equals("C"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Away Temperature Low", e.Data, pName);

                                if (dataelement.Equals("ambient_temperature_f"))
                                {
                                    if (tempScale.Equals("F"))
                                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Ambient Temperature", e.Data, pName);
                                }
                                if (dataelement.Equals("ambient_temperature_c"))
                                {
                                    if (tempScale.Equals("C"))
                                        OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Ambient Temperature", e.Data, pName);
                                }
                                if (dataelement.Equals("humidity"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Humidity", e.Data, pName);
                            }

                            if (objectType.Equals("NEST PROTECT"))
                            {
                                if (dataelement.Equals("smoke_alarm_state") && e.Data.Equals("emergency"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, "SMOKE EMERGENCY", pName);

                                if (dataelement.Equals("smoke_alarm_state") && e.Data.Equals("warning"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, "SMOKE WARNING", pName);

                                if (dataelement.Equals("co_alarm_state") && e.Data.Equals("emergency"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, "CO EMERGENCY", pName);

                                if (dataelement.Equals("co_alarm_state") && e.Data.Equals("warning"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, "CO WARNING", pName);

                                if (dataelement.Equals("battery_health") && e.Data.Equals("replace"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, "BATTERY REPLACE", pName);

                                if (dataelement.Equals("is_online") && e.Data.Equals("False"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, "OFFLINE", pName);

                                if (dataelement.Equals("device_id"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "ID", e.Data, pName);

                                if (dataelement.Equals("locale"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Locale", e.Data, pName);

                                if (dataelement.Equals("software_version"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Software Version", e.Data, pName);

                                if (dataelement.Equals("structure_id"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Structure ID", e.Data, pName);

                                if (dataelement.Equals("name"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name", e.Data, pName);

                                if (dataelement.Equals("name_long"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Name Long", e.Data, pName);

                                if (dataelement.Equals("last_connection"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Last Connection", convertDate(e.Data), pName);

                                if (dataelement.Equals("is_online"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Is Online", e.Data, pName);

                                if (dataelement.Equals("battery_health"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Battery Health", e.Data, pName);

                                if (dataelement.Equals("co_alarm_state"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "CO Alarm State", e.Data, pName);

                                if (dataelement.Equals("smoke_alarm_state"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Smoke Alarm State", e.Data, pName);

                                if (dataelement.Equals("is_manual_test_active"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Manual Test Active", e.Data, pName);

                                if (dataelement.Equals("last_manual_test_time"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "Last Manual Test", e.Data, pName);

                                if (dataelement.Equals("ui_color_state"))
                                    OSAEObjectPropertyManager.ObjectPropertySet(objectName, "UI Color State", e.Data, pName);

                                //set state to online if no waring state is active and device is not offline
                                if (OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Smoke Alarm State").Value.Equals("ok") && OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "CO Alarm State").Value.Equals("ok") && OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Battery Health").Value.Equals("ok") && OSAEObjectPropertyManager.GetObjectPropertyValue(objectName, "Is Online").Value.Equals("True"))
                                    OSAEObjectStateManager.ObjectStateSet(objectName, "ONLINE", pName);
                            }
                        }
                    });
                }
                catch (Exception)
                { Log.Error("error getting device updates"); }
            }
        }

        private string GetAccessToken(string authorizationCode)
        {

            // trade pin code for access token
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.home.nest.com");
                var content = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("code", authorizationCode), 
                new KeyValuePair<string, string>("client_id", "ff5f73fb-2fd9-472f-ba3d-73cb236a1808"),
                new KeyValuePair<string, string>("client_secret", "cunKFYT3UqaRbseJWAPOr5PbI"),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });
                var result = client.PostAsync("/oauth2/access_token", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                var accessToken = JsonConvert.DeserializeObject(resultContent);

                return (accessToken as dynamic).access_token;
            }
        }

        private bool IsMyStructureHome(OSAEObject myobject)
        {
            bool ishome;
            string mystructure = myobject.Property("Structure ID").Value;

            if (OSAEObjectManager.GetObjectByAddress(mystructure).State.Value.ToUpper().Equals("HOME"))
                ishome = true;
            else
                ishome = false;

            return ishome;
        }

        public string convertDate(string last_connection)
        {
            //for testing last_connection = "2015-02-06T20:43:16.772Z";
            DateTime _lastConnection;

            if (!DateTime.TryParse(last_connection, out _lastConnection))
                last_connection = "";
            else
                last_connection = _lastConnection.ToString("G");

            return last_connection;
        }

        private bool IsTempOK(string temptype,string temp,OSAEObject myobject)
        {
            bool tempisok = true;
            float mylowtemp = -1, myhightemp = 100;
            string tempscale;

            float.TryParse(myobject.Property("Target Temperature Low").Value, out mylowtemp);
            float.TryParse(myobject.Property("Target Temperature High").Value, out myhightemp);
            tempscale = myobject.Property("Temperature Scale").Value;

            if (tempscale.ToLower().Equals("f"))
            {
                int convertedtemp;
                if (Int32.TryParse(temp, out convertedtemp))
                {
                    if (convertedtemp < 50 || convertedtemp > 90) tempisok = false;
                    if (temptype.Equals("high") && convertedtemp < mylowtemp) tempisok = false;
                    if (temptype.Equals("low") && convertedtemp > myhightemp) tempisok = false;
                }
                else
                    tempisok = false;
            }
            else
            {
                float convertedtemp;
                if (float.TryParse(temp, out convertedtemp))
                {
                    if (convertedtemp < 9 || convertedtemp > 32) tempisok = false;

                    if (temptype.Equals("high") && convertedtemp < mylowtemp) tempisok = false;

                    if (temptype.Equals("low") && convertedtemp > myhightemp) tempisok = false;
                }
                else
                    tempisok = false;
            }
            return tempisok;
        }
    }
}
