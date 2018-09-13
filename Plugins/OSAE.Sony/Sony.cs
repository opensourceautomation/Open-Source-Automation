using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Timers;
using System.Threading;
using SonyAPILib;
using OSAE;

// This version of the plugin is written using NEW logging system and is only compatiable with OSA version 4.8 and Later.
// Also to Commonize versions of plugins with versions of OSA, this version will be 0.4.9
// Built using version 5.4 of SonyAPILib.dll


namespace OSAE.Sony
{
    public class Sony : OSAEPluginBase
    {
        SonyAPILib.APILibrary mySonyLib = new APILibrary();
        SonyAPILib.APILibrary.SonyDevice mySonyDevice = new SonyAPILib.APILibrary.SonyDevice();
        OSAE.General.OSAELog Log = new OSAE.General.OSAELog("Sony");
        string sMethod;
        string objName;
        bool mySonyReg;
        string curStatus;
        bool debug;
        int refresh;
        OSAEObject sonyobj;
        Thread updateOnlineThread;
        System.Timers.Timer OnlineUpdateTimer;
        int Onlineupdatetime = 60000;

        #region ProcessCommand
        public override void ProcessCommand(OSAEMethod method)
        {
            objName = method.ObjectName;
            this.Log.Info("RECEIVED: " + method.ObjectName + " - " + method.MethodName + " : " + method.MethodLabel);
            string gCommand = method.MethodLabel;
            sMethod = method.MethodName;
            objName = method.ObjectName;
            
            #region Discovery Method
            if (sMethod == "DISCOVERY")
            {
                discoverDevices();
                return;
            }
            #endregion

            #region Set Debug Method
            else if (sMethod == "SETDEBUG")
            {
                OSAEObjectPropertyManager.ObjectPropertySet("Sony", "Debug", method.Parameter1, "Sony Plugin");
                this.Log.Info("Debug for Sony Plugin set to:" + method.Parameter1);
                this.debug = Convert.ToBoolean(method.Parameter1);
                return;
            }
            #endregion

            #region obtain information
            sonyobj = OSAEObjectManager.GetObjectByName(objName);
            mySonyDevice. Name = sonyobj.Name;
            mySonyDevice.DocumentUrl = sonyobj.Property("DocumentURL").Value;
            mySonyDevice.ServerName = "OSAE.Sony";
            mySonyDevice.BuildFromDocument(new Uri(mySonyDevice.DocumentUrl));
            string isreg = sonyobj.Property("Registered").Value;
            Log.Info("Device says it's regerister property is: " + mySonyDevice.Registered.ToString());
            Log.Info("Database says the Device's regerister property is: " + isreg);
            if (isreg == "TRUE")
            {
                if (mySonyDevice.Registered == false)
                {
                    mySonyReg = false;
                    OSAEObjectPropertyManager.ObjectPropertySet(objName, "Registered", "false", "Sony");
                }
                else
                {
                    mySonyReg = true;
                }
            }
            else
            {
                if (mySonyDevice.Registered == true)
                {
                    mySonyReg = true;
                    OSAEObjectPropertyManager.ObjectPropertySet(objName, "Registered", "true", "Sony");
                }
                else
                {
                    mySonyReg = false;
                }
            }
            #endregion
            
            #region Register Method
            if (sMethod == "REGISTER")
            {
                if (mySonyReg == false)
                {
                    registerDevice(method);
                }
                else
                {
                    this.Log.Info("Can not execute REGISTER method if Registered property is TRUE!");
                }
            }
            #endregion
            
            #region SetChannel Method
            else if (sMethod == "SETCHANNEL")
            {
                if (mySonyReg == true)
                {
                    bool reg = checkStatus(method);
                    if (reg == true)
                    {
                        setchannel(method);
                    }
                    else
                    {
                        this.Log.Info("The Sony Set Channel Method did NOT complete because the Sony Device did not respond!");
                    }
                }
                else
                {
                    this.Log.Info("The Sony: " + mySonyDevice.Name + " must be Registered!");
                }
            }
            #endregion        
            
            #region Send Text Method
            else if (sMethod == "SENDTEXT")
            {
                if (mySonyReg == true)
                {
                    bool reg = checkStatus(method);
                    if (reg == true)
                    {
                        sendtext(method);
                    }
                    else
                    {
                        this.Log.Info("The Sony Set Channel Method did NOT complete because the Sony Device did not respond!");
                    }
                }
                else
                {
                    this.Log.Info("The Sony: " + mySonyDevice.Name + " must be Registered!");
                }
            }
            #endregion
            
            #region Process ALL Other Commands
            else
            {
                if (mySonyReg == true)
                {
                    bool reg = checkStatus(method);
                    if (reg == true)
                    {
                        string sCommand = "";
                        try
                        {
                            sCommand = mySonyDevice.GetCommandString(gCommand);
                            string results = mySonyDevice.Ircc.SendIRCC(mySonyDevice,sCommand);
                            if (results == "")
                            {
                                this.Log.Error("The Method " + gCommand + ":" + sCommand + " was not executed by " + mySonyDevice.Name);
                            }
                            else
                            {
                                this.Log.Info("Executed: " + mySonyDevice.Name + " - " + gCommand + ":" + sCommand);
                                if (debug)
                                {
                                    this.Log.Debug(mySonyDevice.Name + " returned this information: " + results);
                                }
                            }
                            System.Threading.Thread.Sleep(500);
                        }
                        catch (Exception ex)
                        {
                            this.Log.Error("The Sony Method " + gCommand + ":" + sCommand + " did not execute!:");
                            if (debug)
                            {
                                this.Log.Debug("An error occurred Processing the Command!: " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        this.Log.Error("The Sony Method did NOT Execute because the Sony Device did not report its Status!");
                    }
                }
                else
                {
                    this.Log.Info("The Sony: " + mySonyDevice.Name + " must be Registered!");
                }
            }
            #endregion
        }
        #endregion

        #region RunInterface
        public override void RunInterface(string pluginName)
        {
            this.Log.Info("Found Plugin Object: " + pluginName);
            this.Log.Info(pluginName + " is starting...");
            OSAEObject plugin = OSAEObjectManager.GetObjectByName(pluginName);
            string refsh = plugin.Property("Refresh").Value;
            debug = Convert.ToBoolean(plugin.Property("Debug").Value);

            // Set API Logging
            mySonyLib.Log.Enable = true;

            // If Plugin Debug is TRUE:
            if (debug)
            {
                // then Log ALL Lines!
                mySonyLib.Log.Level = "All";
            }
            else
            {
                // else only Log BASIC Lines!
                mySonyLib.Log.Level = "Basic";
            }
            // Sets Logging to OSA Plugins folder
            mySonyLib.Log.Path = OSAE.Common.ApiPath + @"\Plugins\Sony\";
            mySonyLib.Log.Name = "SonyPlugin_LOG.txt";

            //Start a new file every time service is restarted
            // Old file is renamed with Time/Date Stamp.
            string saveLogDate = DateTime.Now.ToShortDateString();
            saveLogDate = saveLogDate.Replace("/", "_");
            string saveLogTime = DateTime.Now.ToShortTimeString();
            saveLogTime = saveLogTime.Replace(":", "_");
            saveLogTime.Replace(" ", "");
            string saveLog = saveLogDate + "_" + saveLogTime + "_SonyPlugin_OLD.txt";
            mySonyLib.Log.ClearLog(saveLog);

            // Setup Update Timer
            OnlineUpdateTimer = new System.Timers.Timer();
            refresh = Convert.ToInt16(refsh);
            if (refresh == 0)
            {
                Onlineupdatetime = Onlineupdatetime * 1;
            }
            else
            {
                Onlineupdatetime = Onlineupdatetime * refresh;
            }
                OnlineUpdateTimer.Interval = Onlineupdatetime;
                OnlineUpdateTimer.Start();
                OnlineUpdateTimer.Elapsed += new ElapsedEventHandler(OnlineUpdateTime);
                this.updateOnlineThread = new Thread(new ThreadStart(updateonline));
                this.updateOnlineThread.Start();
                Thread.Sleep(5000);
        }
        #endregion

        #region Shutdown
        public override void Shutdown()
        {
            //throw new NotImplementedException();
            OnlineUpdateTimer.Dispose();
            this.Log.Info("Shutting Down Plugin: Sony");
            this.Log.Info("The Sony Plugin has STOPPED!");
        }
        #endregion

        #region Discover Devices Logic
        public void discoverDevices()
        {
            this.Log.Info(objName + " is performing Device Discovery...");
            List<string> fDev = mySonyLib.Locator.LocateDevices();
            if (fDev.Count > 0)
            {
                this.Log.Info("Discovery found: " + fDev.Count + " Device(s)");
                for (int i=0; i < fDev.Count; i++)
                {
                    string selDev = fDev[i];
                    try
                    {
                        this.Log.Info("Building Object from Document: " + selDev);
                        mySonyDevice.BuildFromDocument(new Uri(selDev));
                    }
                    catch (Exception e)
                    {
                        this.Log.Error("ERROR - Building Object for " + mySonyDevice.Name, e);
                        return; 
                    }
                    this.Log.Info("Checking if Object " + mySonyDevice.Name + " is SONY Compatiable");
                    if (mySonyDevice.Ircc.ControlUrl != null)
                    {
                        if (!CheckObjByName(mySonyDevice.Name.ToString()))
                        {
                            try
                            {
                                this.Log.Info("Creating Sony Object Type for: " + mySonyDevice.Name);
                                OSAEObjectTypeManager.ObjectTypeAdd(mySonyDevice.Name, "Sony Device", "Sony", mySonyDevice.Name, true, false, false, true);
                                OSAEObjectTypeManager.ObjectTypeMethodAdd(mySonyDevice.Name, "REGISTER", "Register", "", "", "", "","Executing this Method will Register this device wity OSA");
                                OSAEObjectTypeManager.ObjectTypeStateAdd(mySonyDevice.Name, "ON", "Online","This state represents that the Sony Device is Online");
                                OSAEObjectTypeManager.ObjectTypeStateAdd(mySonyDevice.Name, "OFF", "OffLine", "This state represents that the Sony Device is Offline");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd(mySonyDevice.Name, "DocumentURL", "String", "String", "", false, true, "Enter the URL to the Sony Device Document");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd(mySonyDevice.Name, "Current_Channel", "String", "String", "", false, false, "This will contain the current channel");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd(mySonyDevice.Name, "Current_Volume", "String", "String", "", false, false, "This will contain the current Volume Level");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd(mySonyDevice.Name, "Current_Status", "String", "String", "", false, false, "This will contain the current Device Status");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd(mySonyDevice.Name, "Registered", "Boolean", "String", "FALSE", false, true, "Select True/False if this device is registered");
                                OSAEObjectTypeManager.ObjectTypePropertyAdd(mySonyDevice.Name, "Online", "Boolean", "String", "FALSE", false, true, "Select True/False if this device is Online");
                                this.Log.Info("Sony Object Type was Created: " + mySonyDevice.Name);
                                this.Log.Info("Creating Object for " + mySonyDevice.Name);
                                OSAEObjectManager.ObjectAdd(mySonyDevice.Name, "Sony Device", "Sony Device", mySonyDevice.Name, mySonyDevice.IPAddress, "",30, true);
                                OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "OFF", "Sony");
                                OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "DocumentURL", mySonyDevice.DocumentUrl, "Sony");
                                OSAEObjectManager.ObjectUpdate(mySonyDevice.Name, mySonyDevice.Name, "Sony Device", "Sony Device", mySonyDevice.Name, mySonyDevice.IPAddress, "",30, true);
                                this.Log.Info("Sony Object Created: " + mySonyDevice.Name);

                                foreach (APILibrary.SonyCommands sCmd in mySonyDevice.Commands)
                                {
                                    string cName = sCmd.name;;
                                    string cValue = sCmd.value;
                                    OSAEObjectTypeManager.ObjectTypeMethodAdd(mySonyDevice.Name, cName.ToUpper(), cName, "", "", "", "", "Retrieved Method from Device");
                                    this.Log.Info("Created Method: " + cName + " for device " + mySonyDevice.Name);
                                    if (cName == "ChannelUp")
                                    {
                                        OSAEObjectTypeManager.ObjectTypeMethodAdd(mySonyDevice.Name, "SETCHANNEL", "Set Channel", "Channel #", "", "", "", "Executing this method will set the entered channel on the device");
                                        this.Log.Info("Created Method: Set Channel for device " + mySonyDevice.Name);
                                    }
                                }
                                OSAEObjectTypeManager.ObjectTypeMethodAdd(mySonyDevice.Name, "GETSTATUS", "Get Status", "", "", "", "", "Executing this method will retrieve the current status from the Sony Device");
                                this.Log.Info("Created Method: Get Status for device " + mySonyDevice.Name);
                                OSAEObjectTypeManager.ObjectTypeMethodAdd(mySonyDevice.Name, "SENDTEXT", "Send Text", "Text", "", "", "", "Executing this method will send the netered text to the Sony Device");
                                this.Log.Info("Created Method: Send Text for device " + mySonyDevice.Name);
                                OSAEObjectTypeManager.ObjectTypeMethodAdd(mySonyDevice.Name, "GETTEXT", "Get Text", "", "", "", "", "Executing this method will check if the Sony device is accepting a Text input");
                                this.Log.Info("Created Method: Get Text for device " + mySonyDevice.Name);


                                if (debug)
                                {
                                    this.Log.Debug("Run the Register method for the : " + mySonyDevice.Name + " Object to register this device!");
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Log.Error("An error occurred!!!: " + ex.Message);
                                if (debug)
                                {
                                    this.Log.Debug("An Error occured in the Object Creation for: " + mySonyDevice.Name);
                                }
                            }
                        }
                        else
                        {
                            this.Log.Error("The object " + mySonyDevice.Name + " already exist. Object NOT Created");
                        }
                    }
                    else
                    {
                        this.Log.Debug("The object " + mySonyDevice.Name + " is NOT compatiable");
                    }
                }
            }
            else
            {
                this.Log.Error("The Sony Discovery Method did NOT find any devices");
            }
        }
#endregion

        #region Register Device Logic
        public void registerDevice(OSAEMethod method)
        {
            try
            {
                objName = method.ObjectName;
                if (CheckObjByName(method.ObjectName))
                {
                    sonyobj = OSAEObjectManager.GetObjectByName(objName);
                    mySonyDevice.Name = sonyobj.Name;
                    mySonyDevice.DocumentUrl = sonyobj.Property("DocumentURL").Value;
                    mySonyDevice.ServerName = "OSAE.Sony";
                    mySonyDevice.BuildFromDocument(new Uri(mySonyDevice.DocumentUrl));
                    if (mySonyDevice.Registered == false)
                    {
                        mySonyReg = mySonyDevice.Register();
                        if (mySonyReg == true)
                        {
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Registered", "TRUE", "Sony");
                            this.Log.Info("The Sony Device : " + mySonyDevice.Name + " was registered Successfully.");
                            sonyobj = OSAEObjectManager.GetObjectByName(objName);
                        }
                        else
                        {
                            this.Log.Error("The Sony Registered Method did NOT complete. See Debug Messages..");
                            if (debug)
                            {
                                this.Log.Debug("Make sure the Device is ON before running the Register Method for any Sony Device!");
                                this.Log.Debug("You may need to go to the device and respond to the registration process!");
                                this.Log.Debug("If a Pin code was displayed, you need to run sendAuth(Pincode) next!");
                                this.Log.Debug("Registration returned: " + mySonyReg.ToString() + " for : " + mySonyDevice.Name);
                            }
                        }
                    }
                    if (mySonyDevice.Registered == true)
                    {
                        string response = mySonyDevice.CheckStatus();
                        if (response != "")
                        {
                            this.Log.Info("Setting State for : " + mySonyDevice.Name + " to Online.");
                            OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "ON", "Sony Plugin");
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Online", "TRUE", "Sony");
                        }
                        else
                        {
                            this.Log.Info("The Sony device: " + mySonyDevice.Name + ", did not respond!");
                            this.Log.Info("Setting State for : " + mySonyDevice.Name + " to Offline.");
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Online", "FALSE", "Sony");
                            OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "OFF", "Sony");
                            if (debug)
                            {
                                this.Log.Debug("Make sure the Device is ON before running any Methods for any Sony Device!");
                                this.Log.Debug("If you continue to have issues, set the Registered Property to FALSE and re-run the REGISTER method for this device.");
                            }
                        }
                    }
                }
                else
                {
                    this.Log.Error("The Sony Register Method did NOT find the object: " + objName);
                    if (debug)
                    {
                        this.Log.Debug("The object " + objName + " was not found");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("An error occurred!!!: " + ex.Message);
            }
        }
        #endregion

        #region Check Status Logic
        public bool checkStatus(OSAEMethod method)
        {
            try
            {
                objName = method.ObjectName;
                if (CheckObjByName(objName))
                {
                    sonyobj = OSAEObjectManager.GetObjectByName(objName);
                    mySonyDevice.Name = sonyobj.Name;
                    mySonyDevice.DocumentUrl = sonyobj.Property("DocumentURL").Value;
                    mySonyDevice.ServerName = "OSAE.Sony";
                    bool OK = mySonyDevice.BuildFromDocument(new Uri(mySonyDevice.DocumentUrl));
                    curStatus = mySonyDevice.CheckStatus();
                    if (OK == false) curStatus = "";
                    if (curStatus != "")
                    {
                        this.Log.Info("Setting State for : " + mySonyDevice.Name + " to Online.");
                        OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "ON", "Sony");
                        OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Online", "TRUE", "Sony");
                        OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Current_Status", curStatus, "Sony");
                        mySonyReg = true;
                        int cV = mySonyDevice.RenderingControl.GetVolume(mySonyDevice);
                        string cC = mySonyDevice.RenderingControl.ChannelState;
                        OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Current_Volume", cV.ToString(), "Sony");
                        OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Current_Channel", cC.ToString(), "Sony");
                    }
                    else
                    {
                        this.Log.Info("The Sony device: " + mySonyDevice.Name + ", did not respond!");
                        this.Log.Info("Setting State for : " + mySonyDevice.Name + " to Offline.");
                        OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Online", "FALSE", "Sony");
                        OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "OFF", "Sony");
                        if (debug)
                        {
                            this.Log.Debug("Make sure the Device is ON before running any Methods for " + mySonyDevice.Name);
                            this.Log.Debug("If you continue to have issues, set the Registered Property to FALSE, and Re-DO the registration process again.");
                        }
                        mySonyReg = false;
                    }
                }
                else
                {
                    this.Log.Error("The Sony plugin did NOT find the object: " + objName);
                    if (debug)
                    {
                        this.Log.Debug("The object " + objName + " Must be Powered ON!");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("The Check Status was unsuccessful.");
                if (debug)
                {
                    this.Log.Debug("An error occurred while Checking the Status of the Device: " + ex.Message);
                }
            }
            return mySonyReg;
        }
        #endregion

        #region CheckObjByName
        public bool CheckObjByName(string Objname)
        {
            bool Objexist = false;
            try
            {
                OSAEObject checkObj = OSAEObjectManager.GetObjectByName(Objname);
                if (checkObj.Name == Objname)
                {
                    if (debug)
                    {
                        this.Log.Debug("The object " + checkObj.Name + " Was found.");
                    }
                    Objexist = true;
                }
            }
            catch
            {
                Objexist = false;
                if (debug)
                {
                    this.Log.Debug("The object " + Objname + " Was not found.");
                }
            }
            return Objexist;
        }
        #endregion

        #region setchannel

        /// This method is only available when a "ChannelUp" method also exist!
        // Will not be available on STR devices
        public void setchannel(OSAEMethod method)
        {
            objName = method.ObjectName;
            sonyobj = OSAEObjectManager.GetObjectByName(objName);
            mySonyDevice.Name = sonyobj.Name;
            mySonyDevice.DocumentUrl = sonyobj.Property("DocumentURL").Value;
            mySonyDevice.ServerName = "OSAE.Sony";
            mySonyDevice.BuildFromDocument(new Uri(mySonyDevice.DocumentUrl));
            mySonyDevice.GetRemoteCommandList();
            string channel = method.Parameter1;
            try
            {
                mySonyDevice.ChannelSet(channel);
                OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Current_Channel", channel, "Sony");
                this.Log.Info("Executed: " + mySonyDevice.Name + " - Set Channel to: " + channel);
            }
            catch (Exception ex)
            {
                this.Log.Error("Set Channel Method: Error occurred!!!: " + ex.Message);
            }
        }
        #endregion

        #region sendtext
        public void sendtext(OSAEMethod method)
        {
            bool reg = checkStatus(method);
            if (reg == true)
            {
                try
                {
                    objName = method.ObjectName;
                    sonyobj = OSAEObjectManager.GetObjectByName(objName);
                    mySonyDevice.Name = sonyobj.Name;
                    mySonyDevice.DocumentUrl = sonyobj.Property("DocumentURL").Value;
                    mySonyDevice.ServerName = "OSAE.Sony";
                    mySonyDevice.BuildFromDocument(new Uri(mySonyDevice.DocumentUrl));
                    mySonyDevice.SendText(method.Parameter1);
                }
                catch (Exception ex)
                {
                    this.Log.Info("An error occurred!!!: " + ex.Message);
                    if (debug)
                    {
                        this.Log.Debug("The Send Text method was unsuccessful.");
                    }
                }
            }
        }
        #endregion

        #region OnlineUpdateTimer
        public void OnlineUpdateTime(object sender, EventArgs eArgs)
        {
            if (sender == OnlineUpdateTimer)
            {
                if (!updateOnlineThread.IsAlive)
                {
                    this.updateOnlineThread = new Thread(new ThreadStart(updateonline));
                    this.updateOnlineThread.Start();
                }
            }
        }
        #endregion

        #region updateonline
        public void updateonline()
        {
            OSAEObjectCollection sonydevices = OSAEObjectManager.GetObjectsByOwner("Sony");
            if (refresh > 0)
            {
                foreach (OSAEObject device in sonydevices)
                {
                    string objName = device.Name;
                    if (objName != "Sony")
                    {
                        if (device.Property("Registered").Value == "TRUE")
                        {
                            if (debug)
                            {
                                this.Log.Debug("Updating Online Status for: " + objName);
                            }
                            sonyobj = OSAEObjectManager.GetObjectByName(objName);
                            if (debug)
                            {
                                this.Log.Debug("Initializing: " + sonyobj.Name);
                            }
                            mySonyDevice.Name = sonyobj.Name;
                            mySonyDevice.DocumentUrl = sonyobj.Property("DocumentURL").Value;
                            mySonyDevice.ServerName = "OSAE.Sony";
                            mySonyDevice.BuildFromDocument(new Uri(mySonyDevice.DocumentUrl));
                            OSAEMethod updatereg = new OSAEMethod();
                            updatereg.ObjectName = mySonyDevice.Name;
                            checkStatus(updatereg);
                        }
                    }
                }
            }
        }
        #endregion

    }
}
