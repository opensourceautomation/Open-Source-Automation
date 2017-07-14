using System;
using System.Web;
using System.Web.Security;
using System.Data;
using OSAE;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DataSet dataset = new DataSet();
        dataset = OSAE.OSAESql.RunSQL("select count(object_id) from osae_v_object_property where object_type = 'PERSON' and property_name = 'password' and length(property_value) > 0");
        if (Convert.ToInt16(dataset.Tables[0].Rows[0][0].ToString()) > 0) txtUserName.Focus();
        else Response.Redirect("~/firstrun.aspx");
    }

    protected void imgSubmit_Click(object sender, EventArgs e)
    {
        OSAEObject obj = OSAEObjectManager.GetObjectByName(txtUserName.Text);         
        if (obj != null)
        {
            string pass = obj.Property("Password").Value;
            if (pass == txtPassword.Text)
            {
                if (pass != "")
                {
                    // Success, create non-persistent authentication cookie.
                    FormsAuthentication.SetAuthCookie(txtUserName.Text.Trim(), false);
                    Int32 cto = Convert.ToInt32(OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "Timeout").Value);
                    FormsAuthenticationTicket ticket1 = new FormsAuthenticationTicket(txtUserName.Text.Trim(), true, cto);
                    HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket1));
                    Response.Cookies.Add(cookie1);
                    Session["UserName"] = OSAEObjectManager.GetObjectByName(this.txtUserName.Text.Trim()).Name;
                    Session["TrustLevel"] = OSAEObjectPropertyManager.GetObjectPropertyValue(this.txtUserName.Text.Trim(), "Trust Level").Value;
                    Session["SecurityLevel"] = OSAEObjectPropertyManager.GetObjectPropertyValue(this.txtUserName.Text.Trim(), "Security Level").Value;
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(txtUserName.Text.Trim(), false);
                    Int32 cto = Convert.ToInt32(OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "Timeout").Value);
                    FormsAuthenticationTicket ticket1 = new FormsAuthenticationTicket(txtUserName.Text.Trim(), true, cto);
                    HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket1));
                    Response.Cookies.Add(cookie1);
                    Session["UserName"] = OSAEObjectManager.GetObjectByName(this.txtUserName.Text.Trim()).Name;
                    Session["TrustLevel"] = OSAEObjectPropertyManager.GetObjectPropertyValue(this.txtUserName.Text.Trim(), "Trust Level").Value;
                    Session["SecurityLevel"] = OSAEObjectPropertyManager.GetObjectPropertyValue(this.txtUserName.Text.Trim(), "Security Level").Value;
                }

                // Do the redirect. 
                string returnUrl1;
                OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();
                int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
                if (Session["SecurityLevel"].ToString() != "Admin" & tLevel < adSet.ObjectsTrust)
                    returnUrl1 = "screens.aspx?id=" + adSet.defaultScreen;
                else
                {
                    if (Request.QueryString["ReturnUrl"] == null) returnUrl1 = "objects.aspx";  // the login is successful
                    else returnUrl1 = Request.QueryString["ReturnUrl"];  //login not unsuccessful 
                }
                Response.Redirect(returnUrl1);
            }
            else lblError.Visible = true;
        }
        lblError.Visible = true;
    }
}