using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class config : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblVersion.Text = OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "DB Version").Value;
    }
}