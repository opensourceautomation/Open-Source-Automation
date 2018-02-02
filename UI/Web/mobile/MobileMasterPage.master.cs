using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceProcess;

public partial class mobile_MobileMasterPage : System.Web.UI.MasterPage
{
    public OSAE.General.OSAELog Log = new OSAE.General.OSAELog("Web Server");

    protected void Page_Load(object sender, EventArgs e)
    {
        //SetSessionTimeout();
    }

    private void SetSessionTimeout()
    {
        try
        {
            int timeout = 0;
            if (int.TryParse(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "Timeout").Value, out timeout))
                Session.Timeout = timeout;
            else Session.Timeout = 60;
        }
        catch (Exception ex)
        {
            Log.Error("Error setting session timeout", ex);
            Response.Redirect("~/error.aspx");
        }
    }
}
