using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using OSAE;

public partial class _Default : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    protected void Page_Load(object sender, EventArgs e)
    {
        if(OSAEObjectManager.GetObjectsByType("PERSON").Count == 0)
            Response.Redirect("~/firstrun.aspx");
        txtUserName.Focus();
    }

    protected void imgSubmit_Click(object sender, EventArgs e)
    {
        OSAEObject obj = OSAEObjectManager.GetObjectByName(txtUserName.Text);        
                        
        if (obj != null)
        {
            string pass = obj.Property("Password").Value;
            if (pass == txtPassword.Text)
            {
                // Success, create non-persistent authentication cookie.
                FormsAuthentication.SetAuthCookie(
                        this.txtUserName.Text.Trim(), false);                

                FormsAuthenticationTicket ticket1 = new FormsAuthenticationTicket(this.txtUserName.Text.Trim(),true,10);
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket1));
                Response.Cookies.Add(cookie1);

                // 4. Do the redirect. 
                String returnUrl1;
                // the login is successful
                if (Request.QueryString["ReturnUrl"] == null)
                {
                    returnUrl1 = "objects.aspx";
                }

                //login not unsuccessful 
                else
                {
                    returnUrl1 = Request.QueryString["ReturnUrl"];
                }

                Response.Redirect(returnUrl1);
            }
            else
                lblError.Visible = true;
                
        }
        lblError.Visible = true;
    }
}