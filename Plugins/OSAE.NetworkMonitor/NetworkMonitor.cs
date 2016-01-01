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

            this.Log.Info("Running Interface!");
            int interval;
            bool isNum = int.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Poll Interval").Value, out interval);
            Clock = new System.Timers.Timer();
            if(isNum)
                Clock.Interval = interval * 1000;
            else
                Clock.Interval = 30000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);

            this.updateThread = new Thread(new ThreadStart(update));
            this.updateThread.Start();
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
                    this.updateThread = new Thread(new ThreadStart(update));
                    this.updateThread.Start();
                }
            }

        }

        public void update()
        {
            try
            {
                OSAEObjectCollection objects = OSAEObjectManager.GetObjectsByType("NETWORK DEVICE");
                this.Log.Debug("# NETWORK DEVICE: " + objects.Count.ToString());
                
                foreach (OSAEObject obj in objects)
                {
                    this.Log.Debug("Pinging: " + obj.Address);
                    if (CanPing(obj.Address.ToString()))
                    {
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", gAppName);
                    }
                    else
                    {
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", gAppName);
                    }
                }

                objects = OSAEObjectManager.GetObjectsByType("COMPUTER");
                Log.Debug("# COMPUTERS: " + objects.Count.ToString());
                
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
                this.Log.Error("Error pinging", ex);
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
                    Log.Debug(address + " is On-Line!");
                    return true;
                }
                else
                {
                    Log.Debug(address + " is Off-Line");
                    return false;
                }
            }
            catch (PingException)
            {
                Log.Debug(address + " is Off-Line");
                return false;
            }


           
        }
    }
}
