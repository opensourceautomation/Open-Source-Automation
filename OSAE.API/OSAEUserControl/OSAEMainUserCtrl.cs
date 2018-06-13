namespace OSAE
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;

    /// <summary>
    /// Class holds an instance of a Static CustomUserControl
    /// </summary>
    [Serializable]
    public class OSAEMainUserCtrl : UserControl
    {
        public string ControlName { get; set; }
        public string ControlType { get; set; }
        public string Screen { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int ZOrder { get; set; }
        public System.Windows.Point Location;
    }
}
