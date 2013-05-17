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

        
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedRow.Text != "")
        {
            GVAnnotatedTimeline1.Visible = true;
            gvProperties.Rows[Int32.Parse(hdnSelectedRow.Text)].Attributes.Remove("onmouseout");
            gvProperties.Rows[Int32.Parse(hdnSelectedRow.Text)].Style.Add("background", "lightblue");

            this.GVAnnotatedTimeline1.DataSource = null;
            List<GoogleChartsNGraphsControls.TimelineEvent> evts = new List<GoogleChartsNGraphsControls.TimelineEvent>();
            foreach (System.Data.DataRow dr in OSAESql.RunSQL("SELECT history_timestamp, CASE property_datatype WHEN 'Boolean' THEN IF(property_value='TRUE', 1, 0) ELSE property_value END AS property_value FROM osae_v_object_property_history WHERE object_property_id =" + hdnSelectedPropID.Text + " ORDER BY history_timestamp asc").Tables[0].Rows)
            {
                evts.Add(new GoogleChartsNGraphsControls.TimelineEvent("Value", DateTime.Parse(dr[0].ToString()), Decimal.Parse(dr[1].ToString())));
            }
            this.GVAnnotatedTimeline1.ChartData(evts.ToArray());
        }
        else
        {
            GVAnnotatedTimeline1.Visible = false;
        }
    }

    protected void gvProperties_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvProperties_" + e.Row.RowIndex.ToString()));
        }

    }

    private void loadProperties()
    {
        gvProperties.DataSource = OSAESql.RunSQL("SELECT DISTINCT CONCAT(object_name,' - ',property_name)as prop_name, object_property_id as prop_id FROM osae_v_object_property_history WHERE property_datatype IN ('Integer', 'Float', 'Boolean') ORDER BY prop_name");
        gvProperties.DataBind();


    }
}
