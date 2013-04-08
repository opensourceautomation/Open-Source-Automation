using System;
using System.AddIn;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace OSAE.MediaCenter
{
    [AddIn("MediaCenter", Version = "0.0.1")]
    public class MediaCenter : OSAEPluginBase
    {

        private Logging logging = Logging.GetLogger("MediaCenter");

        string pName;
        List<MCDevice> mcdevices = new List<MCDevice>();
        
        System.Timers.Timer Clock = new System.Timers.Timer();
        Thread updateThread;

        public override void RunInterface(string pluginName)
        {
            log("Running interface", true);
            pName = pluginName;
            OSAEObjectTypeManager.ObjectTypeUpdate("MediaCenter Device", "MediaCenter Device", "MediaCenter Device", pluginName, "MediaCenter Device", 0, 0, 0, 1);

            //heartbeat to check online devices
            int interval;

            bool isNum = Int32.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Poll Interval").Value, out interval);
            Clock = new System.Timers.Timer();
            if (isNum)
                Clock.Interval = interval * 1000;
            else
                Clock.Interval = 60000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);


            //connect to devices
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("MediaCenter Device");

            foreach (OSAEObject obj in objects)
            {
                createdevice(obj);
            }


            this.updateThread = new Thread(new ThreadStart(update));
            this.updateThread.Start();

            log("Run Interface Complete", true);
            
        }


        public override void Shutdown()
        {
            Clock.Stop();

            foreach (MCDevice d in mcdevices)
            {
                d.CloseConnections();
            }

        }

        public override void ProcessCommand(OSAEMethod method)
        {
            
            String object_name = method.ObjectName;
            String method_name = method.MethodName;
            String parameter_1 = method.Parameter1;
            String parameter_2 = method.Parameter2;
            String mycommand;

            log("Found Command: " + method_name + " | param1: " + parameter_1 + " | param2: " + parameter_2, true);

            if (object_name == pName)
            {
                
                switch (method_name)
                {
                    case "SCAN":
                        //will eventaully try to run a network scan to check if any devices are active on port 40400 or 40500
                        log("Scan event triggered... currently it does nothing ", false);
                        break;
                    case "NOTIFYALL":
                        mycommand = @"msgbox ""OSA"" """ + parameter_1 + @""" " + parameter_2;
                        log("NOTIFYALL event triggered, command to send=" + mycommand, false);

                        foreach (MCDevice d in mcdevices)
                        {
                            d.SendCommand_Network(mycommand);
                        }
                        
                        break;

                }
            }
            else
            {
                MCDevice d = getMCDevice(object_name);
                if (d != null)
                {
                    d.ProcessCommand(method_name, parameter_1, parameter_2);
                    
                }
            }
        }

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            if (sender == Clock)
            {
                if (!updateThread.IsAlive)
                {
                    this.updateThread = new Thread(new ThreadStart(update));
                    this.updateThread.Start();
                }
            }

        }

        public void update()
        {
            OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("MediaCenter Device");

            String msg = "";

            //add any new objects created since pluggin started
            foreach (OSAEObject obj in objects)
            {
                MCDevice d = getMCDevice(obj.Name);
                if (d == null)
                {
                    log("Poll- device was null so we are creating it-"+obj.Name, false);
                    createdevice(obj);
                }
            }
            
            foreach (MCDevice d in mcdevices)
            {
                Boolean connected;
                connected=d.CheckConnections(); 

                if (connected)
                {
                    if (OSAEObjectStateManager.GetObjectStateValue(d.Name).Value == "Off")
                    {
                        OSAEObjectStateManager.ObjectStateSet(d.Name, "ON", pName);
                    }
                    if (!msg.Equals(""))
                    {
                        msg = msg + ", ";
                    }
                    msg = msg + d.Name + ":ON";
                }
                else
                {
                    if (OSAEObjectStateManager.GetObjectStateValue(d.Name).Value != "Off")
                    {
                        OSAEObjectStateManager.ObjectStateSet(d.Name, "OFF", pName);
                    }
                    if (!msg.Equals(""))
                    {
                        msg = msg + ", ";
                    }
                    msg = msg + d.Name + ":OFF";
                }

            }
            log("Poll to check which media center devices are online("+msg+")", false);

        }

        public void log(String message, bool alwaysLog)
        {
            try
            {
                logging.AddToLog(message, alwaysLog);
            }
            catch (IOException ex)
            {
                if (ex.Message != "") //failed because another thread was already writting to log, we will try again ... this is probably totally not needed but I was getting some strange failures when writting to log... will investigate more later
                {
                    try
                    {
                        logging.AddToLog("second chance log -" + message, alwaysLog);
                    }
                    catch (IOException ex2)
                    {
                        //do nothing
                    }
                }
            }
            finally
            {
                //do nothing... we tried writting twice so we will skip this message
            }

        }

        public void createdevice(OSAEObject obj)
        {
            MCDevice d = new MCDevice(obj.Name, pName);
            foreach (OSAEObjectProperty prop in obj.Properties)
            {
                switch (prop.Name)
                {
                    case "Type":
                        d.Type = prop.Value;
                        break;
                    case "IP":
                        d.IP = prop.Value;
                        break;
                    case "Network Port":
                        try
                        {
                            d.CommandPort = Int32.Parse(prop.Value);
                        }
                        catch
                        {
                            d.CommandPort = 0;
                        }
                        break;
                }
            }

            mcdevices.Add(d);
            log("Added MCDevice to list: " + d.Name, false);

            d.EstablishInitialConnections();
        }

        public MCDevice getMCDevice(string name)
        {
            foreach (MCDevice d in mcdevices)
            {
                if (d.Name == name)
                    return d;
            }
            return null;
        }

    }
    
}