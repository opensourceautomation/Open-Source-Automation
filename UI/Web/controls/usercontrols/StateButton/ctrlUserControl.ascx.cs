using System;
using System.Data;
using System.Web.UI.WebControls;
using OSAE;

public partial class controls_ctrlUserControl : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public DateTime LastUpdated;
    public DateTime LastStateChange;
    public string ObjectName;
    private OSAEImageManager imgMgr = new OSAEImageManager();
    public string StateMatch;
    public string CurState;
    public string State1Name, State1Label, State2Name, State2Label;
    public int ControlWidth;
    public int ControlHeight;
    public double LightLevel;

    protected void Page_Load(object sender, EventArgs e)
    {
        ObjectName = screenObject.Property("Object Name").Value;
        hdnObjName.Value = ObjectName;
        CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
        OSAEObject curObj = OSAEObjectManager.GetObjectByName(ObjectName);
        hdnCurState.Value = CurState;
        LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
        btnState.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");

        DataSet ds = OSAEObjectStateManager.ObjectStateListGet(ObjectName);
        if (ds.Tables[0].Rows.Count > 0)
        {
            State1Name = ds.Tables[0].Rows[0]["state_name"].ToString();
            State1Label = ds.Tables[0].Rows[0]["state_label"].ToString();
        }
        if (ds.Tables[0].Rows.Count > 1)
        {
            State2Name = ds.Tables[0].Rows[1]["state_name"].ToString();
            State2Label = ds.Tables[0].Rows[1]["state_label"].ToString();
        }
        CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
        LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
        try
        {
            ControlWidth = Convert.ToInt32(OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Name, "Width").Value);
            ControlHeight = Convert.ToInt32(OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Name, "Height").Value);
        }
        catch (Exception ex)
        { }
        btnState.Height = ControlHeight;
        btnState.Width = ControlWidth; // (ObjectName.Length + 4) * 8;
        string sBackColor = screenObject.Property("Back Color").Value;
        string sForeColor = screenObject.Property("Fore Color").Value;
        int iFontSize = Convert.ToInt16(screenObject.Property("Font Size").Value) - 3;
        string sFontName = screenObject.Property("Font Name").Value;
        if (sBackColor != "")
        {
            try
            { btnState.BackColor = System.Drawing.Color.FromName(sBackColor); }
            catch (Exception)
            { }
        }
        if (sForeColor != "")
        {
            try
            { btnState.ForeColor = System.Drawing.Color.FromName(sForeColor); }
            catch (Exception)
            { }
        }
        if (iFontSize != 0)
        {
            try
            { btnState.Font.Size = new FontUnit(iFontSize); }
            catch (Exception)
            { }
        }
        if (sFontName != "")
        {
            try
            { btnState.Font.Name = sFontName; }
            catch (Exception)
            { }
        }

        if (CurState == State1Name)
        {
            btnState.Text = ObjectName + " " + State1Label;
            btnState.ToolTip = "Click to Set " + ObjectName + " to " + State2Label;
        }
        else
        {
            btnState.Text = ObjectName + " " + State2Label;
            btnState.ToolTip = "Click to Set " + ObjectName + " to " + State1Label;
        }

    }

    public void initialize()
    {

    }

    protected void btnState_Click(object sender, EventArgs e)
    {
        if (CurState == State1Name)
        {
            OSAEMethodManager.MethodQueueAdd(ObjectName, State2Name, "0", "", "SYSTEM");
            OSAEObjectStateManager.ObjectStateSet(ObjectName, State2Name, "SYSTEM");
        }
        else
        {
            OSAEMethodManager.MethodQueueAdd(ObjectName, State1Name, "0", "", "SYSTEM");
            OSAEObjectStateManager.ObjectStateSet(ObjectName, State1Name, "SYSTEM");
        }
    }
}