using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using XBMCRPC;
using All = XBMCRPC.List.Fields.All;
using Player = XBMCRPC.Methods.Player;

namespace OSAE.XBMC
{
    public class XBMC : OSAEPluginBase
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("XBMC");
        private List<XBMCSystem> Systems = new List<XBMCSystem>();
        private string pName;
        private System.Timers.Timer Clock;




        public override void ProcessCommand(OSAEMethod method)
        {
            this.Log.Debug("Found Command: " + method.MethodName + " | param1: " + method.Parameter1 + " | param2: " + method.Parameter2);

            XBMCSystem s = getXBMCSystem(method.ObjectName);
            if (s != null)
            {
                switch (method.MethodName)
                {
                    case "VPLAYPAUSE":
                        s.xbmcSystem.Player.PlayPause();
                        break;
                    case "VSTOP":
                        s.xbmcSystem.Player.Stop();
                        break;
                    case "VBIGSKIPFORWARD":
                        s.xbmcSystem.Player.Seek2(0,Player.Seekvalue.bigforward);
                        break;
                    case "VBIGSKIPBACK":
                        s.xbmcSystem.Player.Seek2(0, Player.Seekvalue.bigbackward);
                        break;
                }
            }

        }

        public override void RunInterface(string pluginName)
        {
            this.Log.Debug("Running interface");
            pName = pluginName;
            OSAEObjectTypeManager.ObjectTypeUpdate("XBMC SYSTEM", "XBMC SYSTEM", "XBMC System", pluginName, "XBMC SYSTEM", 0, 0, 0, 1);

            OSAEObjectCollection XBMCInstances = OSAEObjectManager.GetObjectsByType("XBMC System");

            foreach (OSAEObject obj in XBMCInstances)
            {
                string ip = "", username = "", password = "";
                int port = 0;

                foreach (OSAEObjectProperty p in obj.Properties)
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
                this.Log.Debug("Creating new XBMC System connection: " + obj.Name + " - " + ip);
                try
                {
                    XBMCSystem system = new XBMCSystem(obj.Name, ip, port, username, password);
                    if (system.Connect())
                        Systems.Add(system);
                }
                catch (Exception ex)
                {
                    this.Log.Error("Error connecting to XBMC system: " + ex.Message + " - " + ex.InnerException.Message);
                }
            }

            Clock = new System.Timers.Timer();
            Clock.Interval = 5000;
            
            Clock.Elapsed += new ElapsedEventHandler(Timer_Tick);
            Clock.Start();
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
                    this.Log.Debug("Checking " + r.Name + " - pinging: " + r.Pinging.ToString());
                    if (!r.Pinging)
                    {
                        if (!r.Connected)
                        {
                            this.Log.Info("Removing system from list");
                            Systems.Remove(r);
                        }
                    }
                }

                OSAEObjectCollection XBMCInstances = OSAEObjectManager.GetObjectsByType("XBMC System");
                foreach (OSAEObject obj in XBMCInstances.ToList())
                {
                    foreach (XBMCSystem r in Systems)
                    {
                        if (obj.Name == r.Name)
                            XBMCInstances.Remove(obj);
                    }
                }
                foreach (OSAEObject obj in XBMCInstances)
                {
                    string ip = "", username = "", password = "";
                    int port = 0;

                    foreach (OSAEObjectProperty p in obj.Properties)
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
                    this.Log.Debug("Creating new XBMC System connection: " + obj.Name + " - " + ip);
                    XBMCSystem system = new XBMCSystem(obj.Name, ip, port, username, password);
                    if (system.Connect())
                        Systems.Add(system);
                }
            
            }
            catch (Exception ex)
            {
                this.Log.Error("Error on timer tick", ex);
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
        private bool _pinging;
        private Client _xbmcSystem;
        //OSAELog
        private OSAE.General.OSAELog Log = new General.OSAELog("XBMC");

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        public bool Connected
        {
            get 
            {
                try
                {
                    _pinging = true;
                    string ping = "";
                    if (_xbmcSystem != null)
                    {
                        this.Log.Debug("Pinging client: " + _name);
                        var task = Task.Run(async () =>
                        {
                            return await _xbmcSystem.JSONRPC.Ping();
                        });
                        ping = task.Result;
                        this.Log.Debug(_name + " status: " + ping);
                    }
                    if (ping == "pong")
                    {
                        _pinging = false;
                        return true;
                    }
                    else
                    {
                        _pinging = false;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    this.Log.Error("Ping failed", ex);
                    return false;
                }
            }
            set { _connected = value; }
        }
        public bool Playing
        {
            get { return _playing; }
            set { _playing = value; }
        }
        public bool Pinging
        {
            get { return _pinging; }
            set { _pinging = value; }
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

        public bool Connect()
        {
            this.Log.Debug("Attempting connection: " + Name);
            try
            {
                _pinging = true;
                _xbmcSystem = new Client(_ip, _port, _username, _password);
                var task = Task.Run(async () =>
                {
                    return await _xbmcSystem.JSONRPC.Ping();
                });
                if (task.Result == "pong")
                {
                    this.Log.Debug("Client connected");

                    _xbmcSystem.Player.OnPlay += Player_OnPlay;
                    _xbmcSystem.Player.OnStop += Player_OnStop;
                    _xbmcSystem.Player.OnPause += Player_OnPause;

                    _xbmcSystem.StartNotificationListener();
                    this.Log.Debug("Events wired up");
                    _pinging = false;
                    return true;
                }
                else
                {
                    _pinging = false;
                    return false;
                }
            }
            catch(Exception ex)
            {
                this.Log.Error("Error connecting to XBMC system", ex);
                _pinging = false;
                return false;
            }
        }


        //private void eventListener_OnXEventReceived(object sender, XEventType type, Dictionary<string, string> parameters)
        //{
        //    try
        //    {
        //        this.Log.Debug("EventListener captured event : " + type.ToString() + " - " + sender.ToString() + " - " + _ip, false);

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
        //        this.Log.Error("Error receiving message: " + ex.Message, true);
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
        //            this.Log.Info("Error checking if connected: " + ex.Message, true);
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

            this.Log.Debug(" --- " + _playing.ToString());
        }

        void Player_OnPlay(string sender = null, XBMCRPC.Player.Notifications.Data data = null)
        {
            this.Log.Debug(Name + " started playing");
            OSAEObjectStateManager.ObjectStateSet(Name, "Playing", "XBMC");
        }

        void Player_OnStop(string sender = null, Player.OnStopdataType data = null)
        {
            this.Log.Debug(Name + " stopped playing");
            OSAEObjectStateManager.ObjectStateSet(Name, "Stopped", "XBMC");
        }

        void Player_OnPause(string sender = null, XBMCRPC.Player.Notifications.Data data = null)
        {
            this.Log.Debug(Name + " paused");
            OSAEObjectStateManager.ObjectStateSet(Name, "Stopped", "XBMC");
        }
    }
}
