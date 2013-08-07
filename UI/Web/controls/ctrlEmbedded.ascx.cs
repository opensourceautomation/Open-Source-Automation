using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class controls_ctrlEmbedded : System.Web.UI.UserControl
{
    public OSAEObject screenObject { get; set; }
    public string Source;
    public string width, height;

    protected void Page_Load(object sender, EventArgs e)
    {
        frame.Attributes.Add("width", width);
        frame.Attributes.Add("height", height);
        frame.Attributes.Add("src", Source);
        frame.Attributes.Add("Style", "position:absolute;top:" + (Int32.Parse(screenObject.Property("Y").Value) + 50).ToString() + "px;left:" + (Int32.Parse(screenObject.Property("X").Value) + 10).ToString() + "px;z-index:" + (Int32.Parse(screenObject.Property("ZOrder").Value) + 10).ToString() + ";");
    }
}