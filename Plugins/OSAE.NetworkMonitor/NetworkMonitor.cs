using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;

namespace OSAE.NetworkMonitor
{
    public class NetworkMonitor : OSAEPluginBase
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog();

        System.Timers.Timer Clock = new System.Timers.Timer();
        Thread updateThread;
        string gAppName;
        bool gDebug = false;

        #region OSAEPlugin Members

        public override void ProcessCommand(OSAEMethod method)
        {
            //No commands to process
        }

        public override void RunInterface(string pluginName)
        {
            gAppName = pluginName;
            if (OSAEObjectManager.ObjectExists(gAppName))
                Log.Info("Found the Network Monitor plugin's Object (" + gAppName + ")");

            try
            {
                gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value);
            }
            catch
            { Log.Error("I think the Debug property is missing from the Speech object type!"); }

            Log.Info("Running Interface!");
            int interval;
            bool isNum = int.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Poll Interval").Value, out interval);
            Clock = new System.Timers.Timer();
            if(isNum)
                Clock.Interval = interval * 1000;
            else
                Clock.Interval = 30000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);

            updateThread = new Thread(new ThreadStart(update));
            updateThread.Start();
        }

        public override void Shutdown()
        {
            Clock.Stop();
        }

        #endregion

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
            try
            {
                OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("NETWORK DEVICE");
                if (gDebug) Log.Debug("# NETWORK DEVICE: " + objects.Count.ToString());
                
                foreach (OSAEObject obj in objects)
                {
                    if (gDebug) Log.Debug("Pinging: " + obj.Address);
                    if (CanPing(obj.Address.ToString()))
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", gAppName);
                    else
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", gAppName);
                }

                objects = OSAEObjectManager.GetObjectsByType("COMPUTER");
                if (gDebug) Log.Debug("# COMPUTERS: " + objects.Count.ToString());
                
                foreach (OSAEObject obj in objects)
                {
                    if (CanPing(obj.Address))
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", gAppName);
                    else
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", gAppName);
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error pinging", ex);
            }
        }

        public bool CanPing(string address)
        {
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(address);
                if (reply == null) return false;

                if (reply.Status == IPStatus.Success)
                {
                    if (gDebug) Log.Debug(address + " is On-Line!");
                    return true;
                }
                else
                {
                    if (gDebug) Log.Debug(address + " is Off-Line");
                    return false;
                }
            }
            catch (PingException)
            {
                if (gDebug) Log.Debug(address + " is Off-Line");
                return false;
            }
        }
    }
}
