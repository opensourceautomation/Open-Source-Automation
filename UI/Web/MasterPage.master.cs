using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Headers["User-Agent"] != null && (Request.Browser["IsMobileDevice"] == "true" || Request.UserAgent.ToUpper().Contains("MIDP") || Request.UserAgent.ToUpper().Contains("CLDC") || Request.UserAgent.ToLower().Contains("iphone") || Request.UserAgent.ToLower().Contains("avant") || Request.UserAgent.ToLower().Contains("nokia") || Request.UserAgent.ToLower().Contains("pda") || Request.UserAgent.ToLower().Contains("moto") || Request.UserAgent.ToLower().Contains("windows ce") || Request.UserAgent.ToLower().Contains("hand") || Request.UserAgent.ToLower().Contains("mobi") || Request.UserAgent.ToUpper().Contains("HTC") || Request.UserAgent.ToLower().Contains("sony") || Request.UserAgent.ToLower().Contains("panasonic") || Request.UserAgent.ToLower().Contains("blackberry") || Request.UserAgent.ToLower().Contains("240x320") || Request.UserAgent.ToLower().Contains("voda")))
        {

            Response.Redirect("mobile/index.aspx");
        }
    }
    protected void cog_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("~/config.aspx");
    }
}
