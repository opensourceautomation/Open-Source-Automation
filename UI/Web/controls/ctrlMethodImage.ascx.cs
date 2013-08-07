using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class controls_ctrlMethodImage : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public string ObjectName;
    public string MethodName;
    private OSAEImageManager imgMgr = new OSAEImageManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        ObjectName = screenObject.Property("Object Name").Value;
        MethodName = screenObject.Property("Method Name").Value;
        string imgName = screenObject.Property("Image").Value;
        OSAEImage img = imgMgr.GetImage(imgName);
        imgMethodImage.ImageUrl = "~/imgHandler.aspx?ImageID=" + img.ID;
        imgMethodImage.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
        imgMethodImage.ToolTip = ObjectName + " - " + MethodName;
        imgMethodImage.Attributes.Add("onclick", "runMethod('" + ObjectName + "','" + MethodName + "','','');");

    }
}