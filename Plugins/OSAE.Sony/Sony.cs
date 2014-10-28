using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Timers;
using System.Threading;
using cerDevice;


namespace OSAE.Sony
{
    public class Sony : OSAEPluginBase
    {
        sonyDevice mySonyDevice = new sonyDevice();
        OSAE.General.OSAELog Log = new OSAE.General.OSAELog();

        string sMethod;
        string objName;
        bool mySonyReg;
        OSAEObject sonyobj;
        Thread updateOnlineThread;
        System.Timers.Timer OnlineUpdateTimer;
        int Onlineupdatetime = 60000;

        #region ProcessCommand
        public override void ProcessCommand(OSAEMethod method)
        {
            //throw new NotImplementedException();
            this.Log.Info("RECEIVED: " + method.ObjectName + " - " + method.MethodName);
            sMethod = method.MethodName;
            objName = method.ObjectName;
            if (sMethod == "DISCOVERY")
            {
                discoverDevices();
            }
            else if (sMethod == "REGISTER")
            {
                registerDevice(method);
            }
            else if (sMethod == "RETRIEVE")
            {
                bool reg = registerCheck(method);
                if (reg == true)
                {
                    retrieveDeviceCmd(method);
                }
                else
                {
                    this.Log.Error("The Sony Retreive Method did NOT complete because the Sony Device is Offline!");
                }
            }
            else if (sMethod == "SETCHANNEL")
            {
                bool reg = registerCheck(method);
                if (reg == true)
                {
                    setchannel(method);
                }
                else
                {
                    this.Log.Error("The Sony Set Channel Method did NOT complete because the Sony Device is Offline!");
                }
            }
            else
            {
                bool reg = registerCheck(method);
                if (reg == true)
                {
                    try
                    {
                        objName = method.ObjectName;
                        sonyobj = OSAEObjectManager.GetObjectByName(objName);
                        mySonyDevice.initialize(sonyobj.Name, sonyobj.Property("Host").Value, sonyobj.Property("Port").Value, sonyobj.Property("Mac").Value);
                        string sCommand = method.MethodLabel;
                        mySonyDevice.get_remote_command_list();
                        sCommand = mySonyDevice.getIRCCcommandString(sCommand);
                        string results = mySonyDevice.send_ircc(sCommand);
                        if (results == "")
                        {
                            this.Log.Error("The Method " + method.MethodName + " was not executed by " + mySonyDevice.Name );
                        }
                        else
                        {
                            this.Log.Info("Executed: " + mySonyDevice.Name + " - " + method.MethodLabel);
                            this.Log.Debug(mySonyDevice.Name + " returned this information: " + results);
                        }
                        System.Threading.Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        this.Log.Error("An error occurred!!!: " + ex.Message);
                        this.Log.Debug("The Sony Method" + method.MethodName + " did not execute!");
                    }   
                }
                else
                {
                    this.Log.Error("The Sony Method did NOT Execute because the Sony Device is Offline!");
                }

            }
        }
        #endregion

        #region RunInterface
        public override void RunInterface(string pluginName)
        {
            //throw new NotImplementedException();
            this.Log.Info("Found Plugin Object: " + pluginName);
            this.Log.Info(pluginName + " is starting...");
            OSAEObject plugin = OSAEObjectManager.GetObjectByName(pluginName);
            string refresh = plugin.Property("Refresh").Value;
            int refre = Convert.ToInt16(refresh);
            Onlineupdatetime = Onlineupdatetime * refre;
            OnlineUpdateTimer = new System.Timers.Timer();
            OnlineUpdateTimer.Interval = Onlineupdatetime;
            OnlineUpdateTimer.Start();
            OnlineUpdateTimer.Elapsed += new ElapsedEventHandler(OnlineUpdateTime);
            this.updateOnlineThread = new Thread(new ThreadStart(updateonline));
            this.updateOnlineThread.Start();
            Thread.Sleep(5000);

        }
        # endregion

        #region Shutdown
        public override void Shutdown()
        {
            //throw new NotImplementedException();
            this.Log.Info("Shutting Down Plugin: Sony");
            this.Log.Info("The Sony Plugin has STOPPED!");
        }
        #endregion

        #region DiscoverDevices
        public void discoverDevices()
        {
            this.Log.Info(objName + " is performing Device Discovery...");
            List<string> fDev = mySonyDevice.sonyDiscover(null);
            if (fDev.Count > 0)
            {
                this.Log.Info("Discovery found: " + fDev.Count + " Device(s)");
                for (int i=0; i < fDev.Count; i++)
                {
                    string selDev = fDev[i];
                    mySonyDevice.initialize(selDev);
                    this.Log.Info("Creating Object Type for " + mySonyDevice.Name);
                                   
                    if (!CheckObjByName(mySonyDevice.Name.ToString()))
                    {
                        try
                        {
                            OSAEObjectTypeManager.ObjectTypeAdd(mySonyDevice.Name, "Sony Device", "Sony", mySonyDevice.Name, 1, 0, 0, 1);
                            OSAEObjectTypeManager.ObjectTypeMethodAdd("REGISTER", "Register", mySonyDevice.Name, "", "", "", "");
                            OSAEObjectTypeManager.ObjectTypeMethodAdd("RETRIEVE", "Retrieve", mySonyDevice.Name, "", "", "", "");
                            OSAEObjectTypeManager.ObjectTypeStateAdd("ON", "Online", mySonyDevice.Name);
                            OSAEObjectTypeManager.ObjectTypeStateAdd("OFF", "OFFline", mySonyDevice.Name);
                            OSAEObjectTypeManager.ObjectTypePropertyAdd("Host", "String", "", mySonyDevice.Name, false);
                            OSAEObjectTypeManager.ObjectTypePropertyAdd("Port", "String", "", mySonyDevice.Name, false);
                            OSAEObjectTypeManager.ObjectTypePropertyAdd("Mac", "String", "", mySonyDevice.Name, false);
                            OSAEObjectTypeManager.ObjectTypePropertyAdd("Current_Channel", "String", "", mySonyDevice.Name, false);
                            OSAEObjectTypeManager.ObjectTypePropertyAdd("Current_Volume", "String", "", mySonyDevice.Name, false);
                            OSAEObjectTypeManager.ObjectTypePropertyAdd("Registered", "Boolean", "FALSE", mySonyDevice.Name, false);
                            OSAEObjectTypeManager.ObjectTypePropertyAdd("Online", "Boolean", "FALSE", mySonyDevice.Name, false);
                            this.Log.Info("Sony Object Type was Created: " + mySonyDevice.Name);
                            this.Log.Info("Creating Object for " + mySonyDevice.Name);
                            OSAEObjectManager.ObjectAdd(mySonyDevice.Name, "Sony Device", mySonyDevice.Name, "", "", true);
                            OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "OFF", "Sony Plugin");
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Host", mySonyDevice.Host, "Sony Plugin");
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Mac", mySonyDevice.Mac, "Sony Plugin");
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Port", mySonyDevice.Port, "Sony Plugin");
                            OSAEObjectManager.ObjectUpdate(mySonyDevice.Name, mySonyDevice.Name, "Sony Device", mySonyDevice.Name, mySonyDevice.Host, "", 1);
                            this.Log.Info("Sony Object Created: " + mySonyDevice.Name);
                            this.Log.Debug("Run the Register method for the : " + mySonyDevice.Name + " Object to register this device!");
                        }
                        catch (Exception ex)
                        {
                            this.Log.Error("An error occurred!!!: " + ex.Message);
                            this.Log.Debug("An Error occured in the Object Creation for: " + mySonyDevice.Name);
                        }
                    }
                    else
                    {
                        this.Log.Error("The object " + mySonyDevice.Name + " already exist. Not Creating Object");
                    }
                }
            }
            else
            {
                this.Log.Error("The Sony Discovery Method did NOT find any devices");
            }
        }
#endregion

        #region registerDevice
        public void registerDevice(OSAEMethod method)
        {
            try
            {
                if (CheckObjByName(objName))
                {
                    sonyobj = OSAEObjectManager.GetObjectByName(objName);
                    mySonyDevice.initialize(sonyobj.Name, sonyobj.Property("Host").Value, sonyobj.Property("Port").Value, sonyobj.Property("Mac").Value);
                    if (sonyobj.Property("Registered").Value == "FALSE")
                    {
                        mySonyReg = mySonyDevice.register(mySonyDevice.Mac, "OSAE.Sony");
                        if (mySonyReg == true)
                        {
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Registered", "TRUE", "Sony Plugin");
                            this.Log.Info("The Sony Device : " + mySonyDevice.Name + " was registered Successfully.");
                            sonyobj = OSAEObjectManager.GetObjectByName(objName);
                        }
                        else
                        {
                            this.Log.Error("The Sony Registered Method did NOT complete. See Debug Messages..");
                            this.Log.Debug("Make sure the Device is ON before running the Register Method for any Sony Device!");
                            this.Log.Debug("You may need to go to the device and respond to the registration process!");
                            this.Log.Debug("Registration returned: " + mySonyReg.ToString() + " for Mac: " + mySonyDevice.Mac);
                        }
                    }
                    if (sonyobj.Property("Registered").Value == "TRUE")
                    {
                        mySonyReg = mySonyDevice.register(mySonyDevice.Mac, "OSAE_Sony_Plugin");
                        if (mySonyReg == true)
                        {
                            this.Log.Info("Setting State for : " + mySonyDevice.Name + " to Online.");
                            OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "ON", "Sony Plugin");
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Online", "TRUE", "Sony Plugin");
                        }
                        else
                        {
                            this.Log.Info("The Sony device: " + mySonyDevice.Name + ", did not respond!");
                            this.Log.Info("Setting State for : " + mySonyDevice.Name + " to Offline.");
                            OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Online", "FALSE", "Sony Plugin");
                            OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "OFF", "Sony Plugin");
                            this.Log.Debug("Make sure the Device is ON before running any Methods for any Sony Device!");
                            this.Log.Debug("If you continue to have issues, set the Registered Property to FALSE and re-run the REGISTER method for this device.");
                        }
                    }
                }
                else
                {
                    this.Log.Error("The Sony Register Method did NOT find the object: " + objName);
                    this.Log.Debug("The object " + objName + " was not found");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("An error occurred!!!: " + ex.Message);
            }
        }
        #endregion


        #region registerCheck
        public bool registerCheck(OSAEMethod method)
        {
            try
            {
                if (CheckObjByName(objName))
                {
                    objName = method.ObjectName;
                    sonyobj = OSAEObjectManager.GetObjectByName(objName);
                    mySonyDevice.initialize(sonyobj.Name, sonyobj.Property("Host").Value, sonyobj.Property("Port").Value, sonyobj.Property("Mac").Value);
                    mySonyReg = mySonyDevice.register(mySonyDevice.Mac, "OSAE_Sony_Plugin");
                    if (mySonyReg == true)
                    {
                        this.Log.Info("Setting State for : " + mySonyDevice.Name + " to Online.");
                        OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "ON", "Sony Plugin");
                        OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Online", "TRUE", "Sony Plugin");
                    }
                    else
                    {
                        this.Log.Info("The Sony device: " + mySonyDevice.Name + ", did not respond!");
                        this.Log.Info("Setting State for : " + mySonyDevice.Name + " to Offline.");
                        OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Online", "FALSE", "Sony Plugin");
                        OSAEObjectStateManager.ObjectStateSet(mySonyDevice.Name, "OFF", "Sony Plugin");
                        this.Log.Debug("Make sure the Device is ON before running any Methods for any Sony Device!");
                        this.Log.Debug("If you continue to have issues, set the Registered Property to FALSE and re-run the REGISTER method for this device.");
                    }
                }
                else
                {
                    this.Log.Error("The Sony plugin did NOT find the object: " + objName);
                    this.Log.Debug("The object " + objName + " Must be Powered ON!");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error("An error occurred!!!: " + ex.Message);
                this.Log.Debug("The Sony Register Check was unsuccessful.");
            }
            return mySonyReg;
        }
        #endregion

        #region RetrieveDeviceComd
        public void retrieveDeviceCmd(OSAEMethod method)
        {
            
                if (CheckObjByName(objName))
                {
                    string CmdList = "";
                    objName = method.ObjectName;
                    sonyobj = OSAEObjectManager.GetObjectByName(objName);
                    mySonyDevice.initialize(sonyobj.Name, sonyobj.Property("Host").Value, sonyobj.Property("Port").Value, sonyobj.Property("Mac").Value);
                    try
                    {
                        CmdList = mySonyDevice.get_remote_command_list();
                        this.Log.Debug("The Sony plugin successfully retieved the CmdList.");
                    }
                    catch (Exception ex)
                    {
                        this.Log.Error("An error occurred!!!: " + ex.Message);
                        this.Log.Debug("The Sony plugin did NOT retieve the Command List from " + mySonyDevice.Name);
                    }
                    if (CmdList == "")
                    {
                        this.Log.Error("The Sony Retrieve Method for device " + mySonyDevice.Name + "Was Unsuccessful.");
                    }
                    else
                    {
                        this.Log.Info("Reading XML information retrieved from: " + mySonyDevice.Name);
                        DataSet CommandList = new DataSet();
                        System.IO.StringReader xmlSR = new System.IO.StringReader(CmdList);
                        CommandList.ReadXml(xmlSR, XmlReadMode.Auto);
                        DataTable IRCCcmd = new DataTable();
                        IRCCcmd = CommandList.Tables[0];
                        foreach (DataRow results in IRCCcmd.Rows)
                        {
                            string cName = results.Field<string>("name");
                            string cValue = results.Field<string>("value");
                            string cType = results.Field<string>("type");
                            if (cType == "ircc")
                            {
                                OSAEObjectTypeManager.ObjectTypeMethodAdd(cName.ToUpper(), cName, mySonyDevice.Name, "", "", "", "");
                                //OSAEObjectTypeManager.ObjectTypePropertyAdd(cName.ToUpper(), "String", cValue, mySonyDevice.Name, false);
                                this.Log.Info("Created Method: " + cName + " for device " + mySonyDevice.Name);
                            }
                        }
                        OSAEObjectTypeManager.ObjectTypeMethodAdd("SETCHANNEL", "Set Channel", mySonyDevice.Name, "Channel #", "", "", "");
                        this.Log.Info("Created Method: Set Channel for device " + mySonyDevice.Name);
                    }
                }
                else
                {
                    this.Log.Error("The Sony Retrieve Method did NOT find the object: " + objName);
                    this.Log.Debug("The object " + objName + " Must be Powered ON and Registered!");
                }
        }
        #endregion

        #region CheckObjByName
        public bool CheckObjByName(string Objname)
        {
            bool Objexist;
            try
            {
                OSAEObject checkObj = OSAEObjectManager.GetObjectByName(Objname);
                Objexist = true;
            }
            catch (Exception ex)
            {
                Objexist = false;
                this.Log.Debug("The object " + objName + " Was not found.");
            }

            return Objexist;
        }
        #endregion

        #region setchannel
        public void setchannel(OSAEMethod method)
        {
            objName = method.ObjectName;
            sonyobj = OSAEObjectManager.GetObjectByName(objName);
            mySonyDevice.initialize(sonyobj.Name, sonyobj.Property("Host").Value, sonyobj.Property("Port").Value, sonyobj.Property("Mac").Value);
            mySonyDevice.get_remote_command_list();
            string channel = method.Parameter1;
            try
            {
                mySonyDevice.channel_set(channel);
                OSAEObjectPropertyManager.ObjectPropertySet(mySonyDevice.Name, "Current_Channel", channel, "Sony Plugin");
                this.Log.Info("Executed: " + mySonyDevice.Name + " - Set Channel to: " + channel);
            }
            catch (Exception ex)
            {
                this.Log.Error("Set Channel Method: Error occurred!!!: " + ex.Message);
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
            foreach (OSAEObject device in sonydevices)
            {
                string objName = device.Name;
                sonyobj = OSAEObjectManager.GetObjectByName(objName);
                mySonyDevice.initialize(sonyobj.Name, sonyobj.Property("Host").Value, sonyobj.Property("Port").Value, sonyobj.Property("Mac").Value);
                OSAEMethod updatereg = new OSAEMethod();
                updatereg.ObjectName = mySonyDevice.Name;
                registerCheck(updatereg);
            }
        }
        #endregion

    }
}
