using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class controls_ctrlStateImage : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public DateTime LastUpdated;
    public DateTime LastStateChange;
    public string ObjectName;
    private OSAEImageManager imgMgr = new OSAEImageManager();
    public string StateMatch;
    public string CurState;

    protected void Page_Load(object sender, EventArgs e)
    {
        
        ObjectName = screenObject.Property("Object Name").Value;
        hdnObjName.Value = ObjectName;
        CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
        hdnCurState.Value = CurState;
        LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;

        foreach (OSAEObjectProperty p in screenObject.Properties)
        {
            if (p.Value.ToLower() == CurState.ToLower())
            {
                StateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
            }
        }

        string imgName = screenObject.Property(StateMatch + " Image").Value;
        OSAEImage img = imgMgr.GetImage(imgName);
        if (img != null)
        {
            imgStateImage.ImageUrl = "~/ImageHandler.ashx?id=" + img.ID;
            imgStateImage.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property(StateMatch + " Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property(StateMatch + " X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
            imgStateImage.ToolTip = ObjectName + "\n" + CurState + " since: " + LastStateChange;
            if (CurState == "ON")
            {
                imgStateImage.Attributes.Add("onclick", "runMethod('" + ObjectName + "','OFF','','');");
            }
            else
            {
                imgStateImage.Attributes.Add("onclick", "runMethod('" + ObjectName + "','ON','','');");
            }
        }
    }

    public void Update()
    {
        if (OSAEObjectStateManager.GetObjectStateValue(hdnObjName.Value).Value != hdnCurState.Value)
        {
            CurState = OSAEObjectStateManager.GetObjectStateValue(ObjectName).Value;
            LastStateChange = OSAEObjectStateManager.GetObjectStateValue(ObjectName).LastStateChange;

            foreach (OSAEObjectProperty p in screenObject.Properties)
            {
                if (p.Value.ToLower() == CurState.ToLower())
                {
                    StateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                }
            }

            string imgName = screenObject.Property(StateMatch + " Image").Value;
            OSAEImage img = imgMgr.GetImage(imgName);
            imgStateImage.ImageUrl = "~/ImageHandler.ashx?id=" + img.ID;
            imgStateImage.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property(StateMatch + " Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property(StateMatch + " X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
            imgStateImage.ToolTip = ObjectName + "\n" + CurState + " since: " + LastStateChange;
            if (CurState == "ON")
            {
                imgStateImage.Attributes.Add("onclick", "runMethod('" + ObjectName + "','OFF','','');");
            }
            else
            {
                imgStateImage.Attributes.Add("onclick", "runMethod('" + ObjectName + "','ON','','');");
            }
        }
    }
}