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

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvPatterns")
        {
            hdnSelectedQueueRow.Text = args[1];
            hdnSelectedQueueID.Text = gvQueue.DataKeys[Int32.Parse(hdnSelectedQueueRow.Text)]["schedule_id"].ToString();
        }
        else if (args[0] == "gvMatches")
        {
            hdnSelectedRecurringRow.Text = args[1];
            hdnSelectedRecurringID.Text = gvRecurring.DataKeys[Int32.Parse(hdnSelectedRecurringRow.Text)]["match_id"].ToString();
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        loadQueue();
        loadRecurring();
        
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedQueueRow.Text != "")
        {
            gvQueue.Rows[Int32.Parse(hdnSelectedQueueRow.Text)].Attributes.Remove("onmouseout");
            gvQueue.Rows[Int32.Parse(hdnSelectedQueueRow.Text)].Style.Add("background", "lightblue");
        }
        if (hdnSelectedRecurringRow.Text != "")
        {
            gvRecurring.Rows[Int32.Parse(hdnSelectedRecurringRow.Text)].Attributes.Remove("onmouseout");
            gvRecurring.Rows[Int32.Parse(hdnSelectedRecurringRow.Text)].Style.Add("background", "lightblue");
        }

    }

    protected void gvQueue_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvPatterns_" + e.Row.RowIndex.ToString()));
        }

    }
    protected void gvRecurring_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvMatches_" + e.Row.RowIndex.ToString()));
        }

    }

    private void loadQueue()
    {
        gvQueue.DataSource = OSAESql.RunSQL("SELECT schedule_id, queue_datetime, schedule_name FROM osae_v_schedule_queue ORDER BY queue_datetime DESC");
        gvQueue.DataBind();
    }

    private void loadRecurring()
    {
        gvRecurring.DataSource = OSAESql.RunSQL("SELECT recurring_id, interval_unit, schedule_name FROM osae_v_schedule_recurring ORDER BY schedule_name DESC");
        gvRecurring.DataBind();
    }
    

}