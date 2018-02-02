using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web;
using OSAE;

public partial class mobile_index : System.Web.UI.Page
{
    private int restPort = 8732;
    private static string currentuser;
    protected static string authKey2 = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx?ReturnUrl=mobile/index.aspx");
        currentuser = Session["Username"].ToString();
        hdnUser.Value = currentuser;
        btnUser.Text = currentuser;
        getRestPort();
        SetSessionTimeout();
    }

    [WebMethod]
    public static string refreshAuthKey()
    {
        authKey2 = OSAESecurity.generateCurrentAuthKey(currentuser);
        return authKey2;
    }

    private void getRestPort()
    {
        if (!OSAEObjectPropertyManager.GetObjectPropertyValue("Rest", "REST Port").Id.Equals(String.Empty))
        {
            try
            {
                restPort = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue("Rest", "REST Port").Value);
            }
            catch (FormatException)
            {
                // do nothing and move on 
            }
            catch (OverflowException)
            {
                // do nothing and move on
            }
            catch (ArgumentNullException)
            {
                // do nothing and move on
            }
        }
        hdnRestPort.Value = restPort.ToString();
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
            //Log.Error("Error setting session timeout", ex);
            Response.Redirect("~/error.aspx");
        }
    }
}