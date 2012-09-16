using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using System.Timers;
using OpenSourceAutomation;
using xbmc_json_async.Media;
using xbmc_json_async.System;

namespace OSAE.XBMC
{
    [AddIn("XBMC", Version = "0.1.1")]
    public class XBMC : IOpenSourceAutomationAddIn
    {
        private OSAE osae = new OSAE("XBMC");
        private List<XBMCSystem> Systems = new List<XBMCSystem>();
        private string pName;
        private System.Timers.Timer Clock;
        public void ProcessCommand(System.Data.DataTable table)
        {
            System.Data.DataRow row = table.Rows[0];
            osae.AddToLog("Found Command: " + row["method_name"].ToString() + " | param1: " + row["parameter_1"].ToString() + " | param2: " + row["parameter_1"].ToString(), false);

            XBMCSystem s = getXBMCSystem(row["object_name"].ToString());
            if (s != null)
            {
                switch (row["method_name"].ToString())
                {
                    case "VPLAYPAUSE":
                        s.Connection.Player.PlayPause();
                        break;
                    case "VSTOP":
                        s.Connection.Player.Stop();
                        break;
                    case "VBIGSKIPFORWARD":
                        s.Connection.Player.BigSkipForward();
                        break;
                    case "VBIGSKIPBACK":
                        s.Connection.Player.BigSkipBackward();
                        break;
                }
            }

        }

        public void RunInterface(string pluginName)
        {
            osae.AddToLog("Running interface", false);
            pName = pluginName;
            osae.ObjectTypeUpdate("XBMC SYSTEM", "XBMC SYSTEM", "XBMC System", pluginName, "XBMC SYSTEM", 0, 0, 0, 1);

            List<OSAEObject> XBMCInstances = osae.GetObjectsByType("XBMC System");

            foreach (OSAEObject obj in XBMCInstances)
            {
                string ip = "", username = "", password = "";
                int port = 0;

                foreach (ObjectProperty p in obj.Properties)
                {
                    switch (p.Name)
                    {
                        case "IP":
                            ip = p.Value;
                            break;
                        case "Port":
                            port = Int32.Parse(p.Value);
                            break;
                        case "Username":
                            username = p.Value;
                            break;
                        case "Password":
                            password = p.Value;
                            break;
                    }
                }
                osae.AddToLog("Creating new XBMC System connection: " + obj.Name + " - " + ip, false);
                XBMCSystem system = new XBMCSystem(obj.Name, ip, port, username, password);
                system.Connect();
                Systems.Add(system);
            }

            Clock = new System.Timers.Timer();
            Clock.Interval = 10000;
            Clock.Start();
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);
        }

        public void Shutdown()
        {
            
        }

        public XBMCSystem getXBMCSystem(string name)
        {
            foreach (XBMCSystem r in Systems)
            {
                if (r.Name == name)
                    return r;
            }
            return null;
        }

        public void Timer_Tick(object sender, EventArgs eArgs)
        {
            try
            {
                foreach (XBMCSystem r in Systems)
                {
                    osae.AddToLog("Checking " + r.Name + " - " + r.isConnected().ToString(), false);


                    if (!r.isConnected())
                    {
                        osae.AddToLog("Trying to reconnect", false);
                        r.Connect();
                    }
                    //if (r.isPlaying())
                    //{
                    //    osae.ObjectStateSet(r.Name, "Playing");
                    //}
                }
            }
            catch (Exception ex)
            {
                osae.AddToLog("Error on timer tick: " + ex.Message, true);
            }

        }
    }

    public class XBMCSystem
    {
        private string _name;
        private string _username;
        private string _password;
        private string _ip;
        private int _port = 0;
        private bool _connected;
        private XConnection _connection;
        private OSAE osae = new OSAE("XBMC");
        private XEventListener _eventListener;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public bool Connected
        {
            get { return _connected; }
            set { _connected = value; }
        }
        public XConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public XBMCSystem(string name, string ip, int port, string username, string password)
        {
            _name = name;
            _ip = ip;
            _port = port;
            _username = username;
            _password = password;
        }

        public void Connect()
        {
            _connection = new XConnection(_ip, _port, _username, _password);
            _eventListener = new XEventListener(_ip, 9090);
            _eventListener.OnXEventReceived += new XEventReceivedEventHandler(eventListener_OnXEventReceived);
            _eventListener.Connect();
        }


        private void eventListener_OnXEventReceived(object sender, XEventType type, Dictionary<string, string> parameters)
        {
            try
            {
                osae.AddToLog("EventListener captured event : " + type.ToString() + " - " + sender.ToString() + " - " + _ip, false);

                switch (type)
                {
                    case XEventType.PlaybackPaused:
                        osae.ObjectStateSet(Name, "PAUSED");
                        break;
                    case XEventType.PlaybackResumed:
                    case XEventType.PlaybackStarted:
                        //if (_connection.System.GetActivePlayers() == xbmc_json_async.Player.XPlayerType.VideoPlayer)
                        //    osae.ObjectPropertySet(Name, "Current Player", "Video");
                        //else if (_connection.System.GetActivePlayers() == xbmc_json_async.Player.XPlayerType.AudioPlayer)
                        //    osae.ObjectPropertySet(Name, "Current Player", "Audio");
                        //else
                        //    osae.ObjectPropertySet(Name, "Current Player", "Picture");

                        osae.ObjectStateSet(Name, "PLAYING");
                        break;
                    case XEventType.PlaybackStopped:
                    case XEventType.PlaybackEnded:
                        osae.ObjectStateSet(Name, "STOPPED");
                        break;
                    case XEventType.ConnectionSuccessful:
                        _connected = true;
                        break;
                    case XEventType.ConnectionFailed:
                        _connected = false;
                        break;
                    //case XEventType.ApplicationStop:
                    //    _connected = false;
                        break;
                }

            }
            catch (Exception ex)
            {
                osae.AddToLog("Error receiving message: " + ex.Message, true);
            }
        }

        public bool isConnected()
        {
            if (_connected)
                try
                {
                    return _eventListener.isConnected();
                }
                catch(Exception ex)
                {
                    osae.AddToLog("Error checking if connected: " + ex.Message, true);
                    return false;
                }
            else
                return false;
        }

        public bool isPlaying()
        {
            XSystem xs = _connection.System;
            xbmc_json_async.Player.XPlayerType xpt = xs.GetActivePlayers();
            if (xpt == xbmc_json_async.Player.XPlayerType.Audio || xpt == xbmc_json_async.Player.XPlayerType.Video || xpt == xbmc_json_async.Player.XPlayerType.Picture)
                return true;
            return false;
        }
    }
}
