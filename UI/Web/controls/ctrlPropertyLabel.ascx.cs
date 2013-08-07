using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using OSAE;

public partial class controls_ctrlPropertyLabel : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public DateTime LastUpdated;
    public string ObjectName;
    public string PropertyName;
    private OSAEImageManager imgMgr = new OSAEImageManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        ObjectName = screenObject.Property("Object Name").Value;
        PropertyName = screenObject.Property("Property Name").Value;

        string sPropertyValue = OSAEObjectPropertyManager.GetObjectPropertyValue(ObjectName, PropertyName).Value;
        string sBackColor = screenObject.Property("Back Color").Value;
        string sForeColor = screenObject.Property("Fore Color").Value;
        string sPrefix = screenObject.Property("Prefix").Value;
        string sSuffix = screenObject.Property("Suffix").Value;
        string iFontSize = screenObject.Property("Font Size").Value;

        PropertyLabel.Text = sPrefix + sPropertyValue + sSuffix;
        PropertyLabel.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
        
        if (sBackColor != "")
        {
            try
            {
                PropertyLabel.BackColor = Color.FromName(sBackColor);
            }
            catch (Exception)
            {
            }
        }
        if (sForeColor != "")
        {
            try
            {
                PropertyLabel.ForeColor = Color.FromName(sForeColor);
            }
            catch (Exception)
            {
            }
        }
        if (iFontSize != "")
        {
            try
            {
                PropertyLabel.Font.Size = new FontUnit(iFontSize);
            }
            catch (Exception)
            {
            }
        }
    }
}