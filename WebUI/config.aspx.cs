using OSAE;
using System;

public partial class config : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblVersion.Text = OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "DB Version").Value;
    }
}