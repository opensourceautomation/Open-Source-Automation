using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class analytics : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvProperties")
        {
            hdnSelectedRow.Text = args[1];
            hdnSelectedPropID.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["prop_id"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        loadProperties();
        loadStates();
        
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedRow.Text != "")
        {
            gvProperties.Rows[Int32.Parse(hdnSelectedRow.Text)].Attributes.Remove("onmouseout");
            gvProperties.Rows[Int32.Parse(hdnSelectedRow.Text)].Style.Add("background", "lightblue");

        }
    }
    
    private void loadProperties()
    {
        gvProperties.DataSource = OSAESql.RunSQL("SELECT DISTINCT CONCAT(object_name,' - ',property_name)as prop_name, object_name, property_name FROM osae_v_object_property_history WHERE property_datatype IN ('Integer', 'Float', 'Boolean') ORDER BY prop_name");
        gvProperties.DataBind();


    }

    private void loadStates()
    {
        gvStates.DataSource = OSAESql.RunSQL("SELECT DISTINCT object_name FROM osae_v_object_state_change_history ORDER BY object_name");
        gvStates.DataBind();


    }
}
