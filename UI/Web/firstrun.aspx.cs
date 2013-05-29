using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class firstrun : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void createUserLinkButton_Click(object sender, EventArgs e)
    {
        if (txtUser.Text != "")
        {
            if (txtPass.Text == txtPass2.Text)
            {
                OSAEObjectManager.ObjectAdd(txtUser.Text, "Web UI user", "PERSON", "", "", true);
                OSAEObjectPropertyManager.ObjectPropertySet(txtUser.Text, "Password", txtPass.Text, "Web UI");
                Response.Redirect("~/objects.aspx"); 
            }
            else
            {
                divError.InnerHtml = "The passwords do not match.  Please correct and try again.";
                divError.Visible = true;
            }
        }
        else
        {
            divError.InnerHtml = "Please enter a user name.";
            divError.Visible = true;
        }
    }
}