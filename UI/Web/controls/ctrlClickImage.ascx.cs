using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class controls_ctrlClickImage : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public string pressObjectName;
    public string releaseObjectName;
    public string pressMethodName;
    public string releaseMethodName;
    public string pMp1;
    public string pMp2;
    public string rMp1;
    public string rMp2;
    private OSAEImageManager imgMgr = new OSAEImageManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        pressObjectName = screenObject.Property("Press Object Name").Value;
        releaseObjectName = screenObject.Property("Press Object Name").Value;
        OSAEObject pObj = OSAEObjectManager.GetObjectByName(pressObjectName);
        OSAEObject rObj = OSAEObjectManager.GetObjectByName(releaseObjectName);
        string pressObjTrust = pObj.MinTrustLevel.ToString();
        string releaseObjTrust = rObj.MinTrustLevel.ToString();
        pressMethodName = screenObject.Property("Press Method Name").Value;
        releaseMethodName = screenObject.Property("Release Method Name").Value;
        pMp1 = screenObject.Property("Press Method Param 1").Value;
        pMp2 = screenObject.Property("Press Method Param 2").Value;
        rMp1 = screenObject.Property("Release Method Param 1").Value;
        rMp2 = screenObject.Property("Release Method Param 2").Value;
        string normalImgName = screenObject.Property("Normal Image").Value;
        string pressImgName = screenObject.Property("Pressed Image").Value;
        OSAEImage img = imgMgr.GetImage(normalImgName);
        OSAEImage img2 = imgMgr.GetImage(pressImgName);
        string normalImgURL = "ImgHandler.aspx?ImageID=" + img.ID;
        string pressImgURL = "ImgHandler.aspx?ImageID=" + img2.ID;
        imgClickImage.ImageUrl = "~/imgHandler.aspx?ImageID=" + img.ID;
        imgClickImage.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
        imgClickImage.ToolTip = pressObjectName + " - " + pressMethodName;
        imgClickImage.Attributes.Add("onmousedown", "methFunc('" + pressObjectName + "','" + pressMethodName + "','" + pMp1 + "','" + pMp2 + "','" + pressObjTrust + "','" + pressImgURL + "', this.id);");
        imgClickImage.Attributes.Add("onmouseup", "methFunc('" + releaseObjectName + "','" + releaseMethodName + "','" + rMp1 + "','" + rMp2 + "','" + releaseObjTrust + "','" + normalImgURL + "', this.id);");
    }
}