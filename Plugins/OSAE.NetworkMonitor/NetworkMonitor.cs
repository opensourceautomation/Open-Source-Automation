using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;

namespace OSAE.NetworkMonitor
{
    public class NetworkMonitor : OSAEPluginBase
    {
        /// <summary>
        /// Provides access to logging
        /// </summary>
        Logging logging = Logging.GetLogger("Network Monitor");

        System.Timers.Timer Clock = new System.Timers.Timer();
        Thread updateThread;
        string pName;

        #region OSAEPlugin Members

        public override void ProcessCommand(OSAEMethod method)
        {
            //No commands to process
        }

        public override void RunInterface(string pluginName)
        {
            pName = pluginName;
            logging.AddToLog("Running Interface!", true);
            int interval;
            bool isNum = Int32.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Poll Interval").Value, out interval);
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
                List<OSAEObject> objects = OSAEObjectManager.GetObjectsByType("NETWORK DEVICE");
                logging.AddToLog("# NETWORK DEVICE: " + objects.Count.ToString(), false);
                
                foreach (OSAEObject obj in objects)
                {
                    logging.AddToLog("Pinging: " + obj.Address, false);
                    if (CanPing(obj.Address.ToString()))
                    {
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", pName);
                    }
                    else
                    {
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", pName);
                    }
                }

                objects = OSAEObjectManager.GetObjectsByType("COMPUTER");
                logging.AddToLog("# COMPUTERS: " + objects.Count.ToString(), false);
                
                foreach (OSAEObject obj in objects)
                {
                    logging.AddToLog("Pinging: " + obj.Address, false);
                    if (CanPing(obj.Address))
                    {
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "ON", pName);
                    }
                    else
                    {
                        OSAEObjectStateManager.ObjectStateSet(obj.Name, "OFF", pName);
                    }
                }

            }
            catch (Exception ex)
            {
                logging.AddToLog("Error pinging: " + ex.Message, true);
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
                    logging.AddToLog("On-Line!", false);
                    return true;
                }
                else
                {
                    logging.AddToLog("Off-Line", false);
                    return false;
                }
            }
            catch (PingException)
            {
                logging.AddToLog("Off-Line", false);
                return false;
            }


           
        }
    }
}
