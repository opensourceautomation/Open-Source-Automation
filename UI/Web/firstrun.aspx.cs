using System;
using System.Data;
using OSAE;

public partial class firstrun : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //don't let them create a user if a user already exists
        // OSAEObjectCollection objects = new OSAEObjectCollection();
        //objects = OSAEObjectManager.GetObjectsByType("PERSON");
        DataSet dataset = new DataSet();
        dataset = OSAE.OSAESql.RunSQL("select count(object_id)from osae_v_object_property where object_type = 'PERSON' and property_name = 'password' and length(property_value) > 0");
        if (Convert.ToInt16(dataset.Tables[0].Rows[0][0].ToString()) > 0) Response.Redirect("~/Default.aspx");

        //if (objects.Count > 0) Response.Redirect("~/Default.aspx");
    }
    protected void createUserLinkButton_Click(object sender, EventArgs e)
    {
        if (txtUser.Text != "")
        {
            if (txtPass.Text == txtPass2.Text)
            {
                OSAEObjectManager.ObjectAdd(txtUser.Text, "", "Web UI user", "PERSON", "", "House",50, true);
                OSAEObjectPropertyManager.ObjectPropertySet(txtUser.Text, "Password", txtPass.Text, "SYSTEM");
                OSAEObjectPropertyManager.ObjectPropertySet(txtUser.Text, "Security Level", "Admin", "SYSTEM");
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