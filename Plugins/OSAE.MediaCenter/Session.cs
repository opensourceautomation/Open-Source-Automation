using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSAE.MediaCenter
{
    public class Session
    {
        private int _SessionNumber;
        private string _MediaName = "";
        private string _SessionType = "";
        private bool _Play = false;
        private bool _Stop = false;
        private int _MediaTime = 0;
        private string _TrackNumber = "";
        private DateTime _lastupdate;
        private bool _active = true;

        public Session(int sessionnum)
        {
            _SessionNumber = sessionnum;
            _lastupdate = DateTime.Now;
            _active = true;
        }

        public int SessionNumber
        {
            get { return _SessionNumber; }
            set
            {
                _SessionNumber = value;
                _lastupdate = DateTime.Now;
            }
        }

        public string MediaName
        {
            get { return _MediaName; }
            set
            {
                _MediaName = value;
                _lastupdate = DateTime.Now;
            }
        }

        public string SessionType
        {
            get { return _SessionType; }
            set
            {
                _SessionType = value;
                _lastupdate = DateTime.Now;
            }
        }

        public bool Play
        {
            get { return _Play; }
            set
            {
                _Play = value;
                _lastupdate = DateTime.Now;
            }
        }

        public bool Stop
        {
            get { return _Stop; }
            set
            {
                _Stop = value;
                _lastupdate = DateTime.Now;
            }
        }

        public int MediaTime
        {
            get { return _MediaTime; }
            set
            {
                _MediaTime = value;
                _lastupdate = DateTime.Now;
            }
        }

        public String MediaTimeS
        {
            get { return _MediaTime.ToString(); }
            set
            {
                int parseval;
                bool isNum = Int32.TryParse(value, out parseval);
                if (isNum)
                {
                    _MediaTime = parseval;
                    _lastupdate = DateTime.Now;
                }
            }
        }


        public string TrackNumber
        {
            get { return _TrackNumber; }
            set
            {
                _TrackNumber = value;
                _lastupdate = DateTime.Now;
            }
        }

        public DateTime lastupdate
        {
            get { return _lastupdate; }
            set { _lastupdate = value; }
        }

        public bool active
        {
            get { return _active; }
            set
            {
                _active = value;
                _lastupdate = DateTime.Now;
            }
        }
    }
}
