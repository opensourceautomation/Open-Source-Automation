using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSAE
{
    public class OSAEScreenControl
    {
        public string ControlName { get; set; }
        public string ControlType { get; set; }
        public string ObjectName { get; set; }
        public string ObjectState { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
