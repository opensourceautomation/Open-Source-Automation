using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class schedules : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    protected void Page_Load(object sender, EventArgs e)
    {
        schedulesGridView.DataSource = OSAESql.RunSQL("SELECT * FROM osae_v_schedule_recurring");
        schedulesGridView.DataBind();
        
    }

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "schedulesGridView")
        {
            hdnSelectedRow.Text = args[1];
            //panelEditForm.Visible = true;
            //panelPropForm.Visible = false;
            hdnSelectedPropRow.Text = "";
            hdnSelectedObjectName.Text = schedulesGridView.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["schedule_name"].ToString();
            //loadDDLs();
            //loadProperties();
        }
        //else if (args[0] == "gvProperties")
        //{
        //    hdnSelectedPropRow.Text = args[1];
        //    panelPropForm.Visible = true;
        //    hdnSelectedPropName.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_name"].ToString();
        //    lblPropName.Text = hdnSelectedPropName.Text;
        //    txtPropValue.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_value"].ToString();
        //}
    }
}