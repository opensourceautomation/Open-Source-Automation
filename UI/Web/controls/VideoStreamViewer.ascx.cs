using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MjpegProcessor;
using OSAE;

public partial class controls_VideoStreamViewer : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public string width, height;
    double imgRatio;
    public double ControlWidth;
    public double ControlHeight;
    string streamURI;
    string newData;

    protected void Page_Load(object sender, EventArgs e)
    {
        OSAEObject obj = OSAEObjectManager.GetObjectByName(screenObject.Property("Object Name").Value);
        streamURI = renameingSys(obj.Property("Stream Address").Value);
        double imgWidth = Convert.ToDouble(obj.Property("Width").Value);
        double imgHeight = Convert.ToDouble(obj.Property("Height").Value);
        ControlWidth = Convert.ToDouble(screenObject.Property("Width").Value);
        imgRatio = ControlWidth / imgWidth;
        ControlHeight = Convert.ToInt32(imgHeight * imgRatio);
        width = ControlWidth.ToString();
        height = ControlHeight.ToString();
        vidPlayer.ImageUrl = streamURI;
        vidPlayer.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
    }

    private string renameingSys(string fieldData)
    {
        string newData = fieldData.Replace("http://", "");
        while (newData.IndexOf("[") != -1)
        {
            int ss = newData.IndexOf("[");
            int es = newData.IndexOf("]");
            string renameProperty = newData.Substring(ss + 1, (es - ss) - 1);
            string getProperty = OSAEObjectPropertyManager.GetObjectPropertyValue(screenObject.Property("Object Name").Value, renameProperty).Value;
            // log any errors
            if (getProperty.Length > 0)
                newData = newData.Replace("[" + renameProperty + "]", getProperty);
        }
        newData = @"http://" + newData;
        return newData;
    }
}