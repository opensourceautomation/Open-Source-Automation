using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using OSAE;

public partial class MasterPage : System.Web.UI.MasterPage
{
    //OSAELog
    public OSAE.General.OSAELog Log = new OSAE.General.OSAELog("Web UI");
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Headers["User-Agent"] != null && (Request.Browser["IsMobileDevice"] == "true" || Request.UserAgent.ToUpper().Contains("MIDP") || Request.UserAgent.ToUpper().Contains("CLDC") || Request.UserAgent.ToLower().Contains("iphone") || Request.UserAgent.ToLower().Contains("avant") || Request.UserAgent.ToLower().Contains("nokia") || Request.UserAgent.ToLower().Contains("pda") || Request.UserAgent.ToLower().Contains("moto") || Request.UserAgent.ToLower().Contains("windows ce") || Request.UserAgent.ToLower().Contains("hand") || Request.UserAgent.ToLower().Contains("mobi") || Request.UserAgent.ToUpper().Contains("HTC") || Request.UserAgent.ToLower().Contains("sony") || Request.UserAgent.ToLower().Contains("panasonic") || Request.UserAgent.ToLower().Contains("blackberry") || Request.UserAgent.ToLower().Contains("240x320") || Request.UserAgent.ToLower().Contains("voda")))
        {
            Response.Redirect("mobile/index.aspx");
        }
        OSAEObjectCollection screens = OSAEObjectManager.GetObjectsByType("SCREEN");

        this.SetSessionTimeout();       

        foreach (OSAEObject s in screens)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            ddlScreens.Controls.Add(li);

            HtmlGenericControl anchor = new HtmlGenericControl("a");
            anchor.Attributes.Add("href", "screens.aspx?id="+s.Name);
            anchor.InnerText = s.Name;

            li.Controls.Add(anchor);
        }
    }

    protected void cog_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/config.aspx");
    }

    private void SetSessionTimeout()
    {
        try
        {
            int timeout = 0;
            if (int.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "Timeout").Value, out timeout))
            {
                Session.Timeout = timeout;
            }
            else
            {
                // we failed to get the value from the system so default it to 60 minutes
                Session.Timeout = 60;
            }
        }
        catch (Exception ex)
        {
            this.Log.Error("Error setting session timeout", ex);
        }
    }
}
