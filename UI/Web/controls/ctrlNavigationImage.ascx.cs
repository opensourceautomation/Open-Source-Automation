using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class controls_ctrlNavigationImage : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public string TargetScreenName;
    private OSAEImageManager imgMgr = new OSAEImageManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        TargetScreenName = screenObject.Property("Screen").Value;
        string imgName = screenObject.Property("Image").Value;
        OSAEImage img = imgMgr.GetImage(imgName);
        if (img != null)
        {
            imgNavigationImage.ImageUrl = "~/ImageHandler.ashx?id=" + img.ID;
            //    imgNavigationImage.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value)).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
            imgNavigationImage.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");

            imgNavigationImage.ToolTip = TargetScreenName;
            imgNavigationImage.PostBackUrl = "~/screens.aspx?id=" + TargetScreenName;
        }

    }
}