namespace OSAE
{
    using System;

    public class OSAEScreenControl
    {
        public string ControlName { get; set; }
        public string ControlType { get; set; }
        public string ObjectName { get; set; }
        public string ObjectState { get; set; }
        public DateTime LastUpdated { get; set; }
        public string TimeInState { get; set; }
    }
}
