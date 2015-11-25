using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class controls_ctrlBrowser: System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public string Source;
    public string width, height;

    protected void Page_Load(object sender, EventArgs e)
    {
        //OSAEObject obj = OSAEObjectManager.GetObjectByName(screenObject.Property("Object Name").Value);
        //Source = renameingSys(obj.Property("URI").Value);
        Source = renameingSys(screenObject.Property("URI").Value);
        width = screenObject.Property("Width").Value;
        height = screenObject.Property("Height").Value;
        frame.Attributes.Add("width", width);
        frame.Attributes.Add("height", height);
        frame.Attributes.Add("src", Source);
        frame.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
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