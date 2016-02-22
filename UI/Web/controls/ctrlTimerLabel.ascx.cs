using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using OSAE;

public partial class controls_ctrlTimerLabel : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public DateTime LastUpdated;
    public string ObjectName;
    private int OffTimer;
    private OSAEImageManager imgMgr = new OSAEImageManager();
    private string CurrentState;
    private int TimeInState;
    TimeSpan span;
    string sValue;

    protected void Page_Load(object sender, EventArgs e)
    {
        ObjectName = screenObject.Property("Object Name").Value;
        OSAEObjectState os = OSAEObjectStateManager.GetObjectStateValue(ObjectName);
        CurrentState = os.Value;

        OffTimer = Convert.ToInt16(OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, "OFF TIMER").Value);
        TimeInState = (int)os.TimeInState;

        string sBackColor = screenObject.Property("Back Color").Value;
        string sForeColor = screenObject.Property("Fore Color").Value;
        string sPrefix = screenObject.Property("Prefix").Value;
        string sSuffix = screenObject.Property("Suffix").Value;
        string iFontSize = screenObject.Property("Font Size").Value;

        if (CurrentState == "OFF")
            sValue = os.StateLabel;
        else
        {
            span = TimeSpan.FromSeconds(OffTimer - TimeInState); //Or TimeSpan.FromSeconds(seconds); (see Jakob C´s answer)
            sValue = span.ToString(@"mm\:ss");
        }

        TimerLabel.Text = sValue;
        TimerLabel.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");

        if (sBackColor != "")
        {
            try
            {
                TimerLabel.BackColor = Color.FromName(sBackColor);
            }
            catch (Exception)
            {
            }
        }
        if (sForeColor != "")
        {
            try
            {
                TimerLabel.ForeColor = Color.FromName(sForeColor);
            }
            catch (Exception)
            {
            }
        }
        if (iFontSize != "")
        {
            try
            {
                TimerLabel.Font.Size = new FontUnit(iFontSize);
            }
            catch (Exception)
            {
            }
        }
    }
}