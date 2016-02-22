using System;
using OSAE;

public partial class controls_ctrlClickImage : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public string ObjectName;
    public string MethodName;
    public string ScriptName;
    public string mp1 = "", mp2 = "", sp1 = "", sp2 = "";
    private OSAEImageManager imgMgr = new OSAEImageManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        ObjectName = screenObject.Property("Press Object Name").Value;
        MethodName = screenObject.Property("Press Method Name").Value;
        mp1 = screenObject.Property("Press Method Param 1").Value;
        mp2 = screenObject.Property("Press Method Param 2").Value;
        ScriptName = screenObject.Property("Press Script Name").Value;
        sp1 = screenObject.Property("Press Script Param 1").Value;
        sp2 = screenObject.Property("Press Script Param 2").Value;

        string imgName = screenObject.Property("Pressed Image").Value;
        OSAEImage img = imgMgr.GetImage(imgName);
        if (img != null)
        {
            imgClickImage.ImageUrl = "~/ImageHandler.ashx?id=" + img.ID;
            imgClickImage.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
            imgClickImage.ToolTip = ObjectName + " - " + MethodName;
            imgClickImage.Attributes.Add("onclick", "runMethod('" + ObjectName + "','" + MethodName + "','" + mp1 + "','" + mp2 + "');");
        }
    }
}