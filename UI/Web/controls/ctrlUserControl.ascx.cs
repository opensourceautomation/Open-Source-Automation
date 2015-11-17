using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
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
    public double LightLevel;

    protected void Page_Load(object sender, EventArgs e)
    {
        //ObjectName = screenObject.Property("Object Name").Value;
        hdnObjName.Value = ObjectName;
        //CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
        //OSAEObject curObj = OSAEObjectManager.GetObjectByName(ObjectName);
        //hdnCurState.Value = CurState;
        //LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;
        PlaceHolder.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
    }

    public void initialize()
    {        
    }
}