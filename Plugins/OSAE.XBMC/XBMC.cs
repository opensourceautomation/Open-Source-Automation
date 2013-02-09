using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using XBMCRPC;
using All = XBMCRPC.List.Fields.All;
using Player = XBMCRPC.Methods.Player;

namespace OSAE.XBMC
{
    public class XBMC : OSAEPluginBase
    {
        private OSAE osae = new OSAE("XBMC");
        private Logging logging = new Logging("XBMC");
        private List<XBMCSystem> Systems = new List<XBMCSystem>();
        private string pName;
        //private System.Timers.Timer Clock;




        public override void ProcessCommand(OSAEMethod method)
        {
            //System.Data.DataRow row = table.Rows[0];
            //logging.AddToLog("Found Command: " + row["method_name"].ToString() + " | param1: " + row["parameter_1"].ToString() + " | param2: " + row["parameter_1"].ToString(), false);

            //XBMCSystem s = getXBMCSystem(row["object_name"].ToString());
            //if (s != null)
            //{
            //    switch (row["method_name"].ToString())
            //    {
            //        case "VPLAYPAUSE":
            //            s.Connection.Player.PlayPause();
            //            break;
            //        case "VSTOP":
            //            s.Connection.Player.Stop();
            //            break;
            //        case "VBIGSKIPFORWARD":
            //            s.Connection.Player.BigSkipForward();
            //            break;
            //        case "VBIGSKIPBACK":
            //            s.Connection.Player.BigSkipBackward();
            //            break;
            //    }
            //}

        }

        public override void RunInterface(string pluginName)
        {
            logging.AddToLog("Running interface", false);
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
                logging.AddToLog("Creating new XBMC System connection: " + obj.Name + " - " + ip, false);
                XBMCSystem system = new XBMCSystem(obj.Name, ip, port, username, password);
                system.Connect();
                Systems.Add(system);
            }

            //Clock = new System.Timers.Timer();
            //Clock.Interval = 5000;
            //Clock.Start();
            //Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);
        }

        public override void Shutdown()
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


                    //if (!r.isConnected())
                    //{
                    //    logging.AddToLog("Trying to reconnect", false);
                    //    r.Connect();
                    //}
                    r.getStatus();

                    if (r.Playing)
                    {
                        logging.AddToLog("Checking " + r.Name + " - Playing", false);
                        osae.ObjectStateSet(r.Name, "Playing");
                    }
                    else
                    {
                        logging.AddToLog("Checking " + r.Name + " - Stopped", false);
                        osae.ObjectStateSet(r.Name, "Stopped");
                    }
                }
            }
            catch (Exception ex)
            {
                logging.AddToLog("Error on timer tick: " + ex.Message, true);
            }

        }
    }

    public class XBMCSystem
    {
        private System.Timers.Timer Clock;
        private string _name;
        private string _username;
        private string _password;
        private string _ip;
        private int _port = 0;
        private bool _connected;
        private bool _playing;
        private Client _xbmcSystem;
        private OSAE osae = new OSAE("XBMC");
        private Logging logging = new Logging("XBMC");

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
        public bool Playing
        {
            get { return _playing; }
            set { _playing = value; }
        }
        public Client xbmcSystem
        {
            get { return _xbmcSystem; }
            set { _xbmcSystem = value; }
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
            _xbmcSystem = new Client(_ip, _port, _username, _password);
            logging.AddToLog("Client connected", false);

            _xbmcSystem.Player.OnPlay += Player_OnPlay;
            _xbmcSystem.Player.OnStop += Player_OnStop;
            _xbmcSystem.Player.OnPause += Player_OnPause;

            _xbmcSystem.StartNotificationListener();
            logging.AddToLog("Events wired up", false);
        }


        //private void eventListener_OnXEventReceived(object sender, XEventType type, Dictionary<string, string> parameters)
        //{
        //    try
        //    {
        //        logging.AddToLog("EventListener captured event : " + type.ToString() + " - " + sender.ToString() + " - " + _ip, false);

        //        switch (type)
        //        {
        //            case XEventType.PlaybackPaused:
        //                osae.ObjectStateSet(Name, "PAUSED");
        //                break;
        //            case XEventType.PlaybackResumed:
        //            case XEventType.PlaybackStarted:
        //                //if (_connection.System.GetActivePlayers() == xbmc_json_async.Player.XPlayerType.VideoPlayer)
        //                //    osae.ObjectPropertySet(Name, "Current Player", "Video");
        //                //else if (_connection.System.GetActivePlayers() == xbmc_json_async.Player.XPlayerType.AudioPlayer)
        //                //    osae.ObjectPropertySet(Name, "Current Player", "Audio");
        //                //else
        //                //    osae.ObjectPropertySet(Name, "Current Player", "Picture");

        //                osae.ObjectStateSet(Name, "PLAYING");
        //                break;
        //            case XEventType.PlaybackStopped:
        //            case XEventType.PlaybackEnded:
        //                osae.ObjectStateSet(Name, "STOPPED");
        //                break;
        //            case XEventType.ConnectionSuccessful:
        //                _connected = true;
        //                break;
        //            case XEventType.ConnectionFailed:
        //                _connected = false;
        //                break;
        //            //case XEventType.ApplicationStop:
        //            //    _connected = false;
        //                break;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        logging.AddToLog("Error receiving message: " + ex.Message, true);
        //    }
        //}

        //public async void checkConnection()
        //{
        //    if (_connected)
        //        try
        //        {
        //            var players = await _xbmcSystem.Player.GetActivePlayers();
        //            int x = players[0];

        //            if (players.Count() > 0)
        //                return true;
        //            else
        //                return false;
        //        }
        //        catch(Exception ex)
        //        {
        //            logging.AddToLog("Error checking if connected: " + ex.Message, true);
        //            return false;
        //        }
                        //else
        //        return false;
        //}

        //public bool isPlaying()
        //{
        //    XSystem xs = _connection.System;
        //    xbmc_json_async.Player.XPlayerType xpt = xs.GetActivePlayers();
        //    if (xpt == xbmc_json_async.Player.XPlayerType.Audio || xpt == xbmc_json_async.Player.XPlayerType.Video || xpt == xbmc_json_async.Player.XPlayerType.Picture)
        //        return true;
        //    return false;
        //}

        public async void getStatus()
            {
            var players = await _xbmcSystem.Player.GetActivePlayers();
            int c = players.Count();

            if (c > 0)
                _playing = true;
            else
                _playing = false;

            logging.AddToLog(" --- " + _playing.ToString(), false);
        }

        void Player_OnPlay(string sender = null, XBMCRPC.Player.Notifications.Data data = null)
        {
            logging.AddToLog(Name + " started playing", false);
            osae.ObjectStateSet(Name, "Playing");
                }

        void Player_OnStop(string sender = null, Player.OnStopdataType data = null)
                {
            logging.AddToLog(Name + " stopped playing", false); 
            osae.ObjectStateSet(Name, "Stopped");
        }

        void Player_OnPause(string sender = null, XBMCRPC.Player.Notifications.Data data = null)
        {
            logging.AddToLog(Name + " paused", false);
            osae.ObjectStateSet(Name, "Stopped");
        }
    }
}
