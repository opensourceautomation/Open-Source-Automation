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

        //OSAELog
        private static OSAE.General.OSAELog Log = new General.OSAELog();

        string pName;
        List<MCDevice> mcdevices = new List<MCDevice>();
        
        System.Timers.Timer Clock = new System.Timers.Timer();
        Thread updateThread;

        public override void RunInterface(string pluginName)
        {
            Log.Info("Starting MediaCenter Plugin");
            pName = pluginName;
            OSAEObjectTypeManager.ObjectTypeUpdate("MediaCenter Device", "MediaCenter Device", "MediaCenter Device", pluginName, "MediaCenter Device", false, false, false, true);

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

            Log.Debug("Run Interface Complete");
            
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

            Log.Debug("Found Command: " + method_name + " | param1: " + parameter_1 + " | param2: " + parameter_2);

            if (object_name == pName)
            {
                
                switch (method_name)
                {
                    case "SCAN":
                        //will eventaully try to run a network scan to check if any devices are active on port 40400 or 40500
                        Log.Info("Scan event triggered... currently it does nothing ");
                        break;
                    case "NOTIFYALL":
                        mycommand = @"msgbox ""OSA"" """ + parameter_1 + @""" " + parameter_2;
                        Log.Info("NOTIFYALL event triggered, command to send=" + mycommand);

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
                    Log.Debug("Poll- device was null so we are creating it-" + obj.Name);
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
            Log.Debug("Poll to check which media center devices are online(" + msg + ")");

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
            Log.Info("Added MCDevice to list: " + d.Name);

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