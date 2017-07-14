using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.ServiceProcess;
using OSAE;

public partial class MasterPage : System.Web.UI.MasterPage
{
    public OSAE.General.OSAELog Log = new OSAE.General.OSAELog("Web Server");      
    
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Request.Headers["User-Agent"] != null && (Request.Browser["IsMobileDevice"] == "true" || Request.UserAgent.ToUpper().Contains("MIDP") || Request.UserAgent.ToUpper().Contains("CLDC") || Request.UserAgent.ToLower().Contains("iphone") || Request.UserAgent.ToLower().Contains("avant") || Request.UserAgent.ToLower().Contains("nokia") || Request.UserAgent.ToLower().Contains("pda") || Request.UserAgent.ToLower().Contains("moto") || Request.UserAgent.ToLower().Contains("windows ce") || Request.UserAgent.ToLower().Contains("hand") || Request.UserAgent.ToLower().Contains("mobi") || Request.UserAgent.ToUpper().Contains("HTC") || Request.UserAgent.ToLower().Contains("sony") || Request.UserAgent.ToLower().Contains("panasonic") || Request.UserAgent.ToLower().Contains("blackberry") || Request.UserAgent.ToLower().Contains("240x320") || Request.UserAgent.ToLower().Contains("voda")))
            Response.Redirect("mobile/index.aspx");

        OSAEObjectCollection screens = OSAEObjectManager.GetObjectsByType("SCREEN");

        SetSessionTimeout();       

        foreach (OSAEObject s in screens)
        {
            HtmlGenericControl li = new HtmlGenericControl("li");
            ddlScreens.Controls.Add(li);

            HtmlGenericControl anchor = new HtmlGenericControl("a");
            anchor.Attributes.Add("href", "screens.aspx?id="+s.Name);
            anchor.InnerText = s.Name;

            li.Controls.Add(anchor);
        }

        ServiceController sc = new ServiceController("OSAE");

        if (sc.Status != ServiceControllerStatus.Running)
        {
            cog.ImageUrl = "~/Images/cog_red.png";
            cog.ToolTip = "Config Settings: OSA service is not running.";
        }
        else
        {
            cog.ToolTip = "Config Settings: OSA service is running.";
        }
        if (Session["UserName"] != null)
        {
            btnUser.Text = Session["UserName"].ToString();
        }
       // if (Session["SecurityLevel"].ToString() != "Admin") btnAdmin.Visible = false;
    }

    protected void cog_Click(object sender, ImageClickEventArgs e)
    {
        int conSet = OSAEAdminManager.GetAdminSettingsByName("Config Trust");
        if (Convert.ToInt32(Session["TrustLevel"].ToString()) < conSet) Response.Redirect("~/permissionError.aspx");

        Response.Redirect("~/config.aspx");
    }

    private void SetSessionTimeout()
    {
        try
        {
            int timeout = 0;
            if (int.TryParse(OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "Timeout").Value, out timeout))
                Session.Timeout = timeout;
            else Session.Timeout = 60;
        }
        catch (Exception ex)
        {
            Log.Error("Error setting session timeout", ex);
            Response.Redirect("~/error.aspx");
        }
    }

    protected void btnUser_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.RemoveAll();
        Session.Abandon();
        Response.Redirect("default.aspx");
    }

   // protected void btnAdmin_Click(object sender, EventArgs e)
   // {
   //     Response.Redirect("admin.aspx");
   // }
}
