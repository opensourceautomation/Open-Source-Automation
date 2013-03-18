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

        if (args[0] == "gvQueue")
        {
            hdnSelectedQueueRow.Text = args[1];
            hdnSelectedQueueID.Text = gvQueue.DataKeys[Int32.Parse(hdnSelectedQueueRow.Text)]["schedule_id"].ToString();
            btnQueueDelete.Visible = true;
        }
        else if (args[0] == "gvRecurring")
        {
            hdnSelectedRecurringRow.Text = args[1];
            hdnSelectedRecurringID.Text = gvRecurring.DataKeys[Int32.Parse(hdnSelectedRecurringRow.Text)]["recurring_id"].ToString();
            hdnSelectedRecurringName.Text = gvRecurring.DataKeys[Int32.Parse(hdnSelectedRecurringRow.Text)]["schedule_name"].ToString();
            btnUpdate.Visible = true;
            btnDelete.Visible = true;
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        loadQueue();
        loadRecurring();
        if (!Page.IsPostBack)
        {
            loadDDLs();
        }
        
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
        txtPickedDate.Text = txtPickedDate.Text;
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
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvQueue_" + e.Row.RowIndex.ToString()));
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
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvRecurring_" + e.Row.RowIndex.ToString()));
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

    private void loadDDLs()
    {
        ddlScript.DataSource = OSAESql.RunSQL("SELECT script_name as Text, script_id as Value  FROM osae_script ORDER BY script_name"); ;
        ddlScript.DataBind();

        ddlObject.DataSource = OSAESql.RunSQL("SELECT object_name as Text, object_id as Value FROM osae_object ORDER BY object_name"); ;
        ddlObject.DataBind();
        if (ddlObject.Items.Count == 0)
            ddlObject.Visible = false;
        else
            ddlObject.Visible = true;
        ddlObject.Items.Insert(0, new ListItem(String.Empty, String.Empty));

    }

    protected void rbScheduleType_SelectedIndexChanged(object sender, EventArgs e)
    {
        datepicker.Visible = false;
        txtMinutes.Visible = false;
        pnlDaily.Visible = false;
        pnlMonthly.Visible = false;
        if (rbScheduleType.SelectedValue == "1")
        {
            datepicker.Visible = true;
        }
        else if(rbScheduleType.SelectedValue == "T")
        {
            txtMinutes.Visible = true;
        }
        else if(rbScheduleType.SelectedValue == "D")
        {
            pnlDaily.Visible = true;
        }
        else if(rbScheduleType.SelectedValue == "M")
        {
            pnlMonthly.Visible = true;
        }
        else if(rbScheduleType.SelectedValue == "Y")
        {
            datepicker.Visible = true;
        }
    }

    protected void rblAction_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnAdd.Visible = true;

        pnlMethod.Visible = false;
        ddlScript.Visible = false;
        if (rblAction.SelectedValue == "1")
        {
            ddlScript.Visible = true;
        }
        else if (rblAction.SelectedValue == "2")
        {
            pnlMethod.Visible = true;
        }
    }
    protected void ddlObject_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlMethod.DataSource = OSAESql.RunSQL("SELECT method_label as Text, method_name as Value FROM osae_v_object_method WHERE object_id = " + ddlObject.SelectedValue + " ORDER BY method_label");
        ddlMethod.Items.Insert(0, new ListItem(String.Empty, String.Empty));
        ddlMethod.DataBind();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (txtPickedDate.Text == "")
            txtPickedDate.Text = "2000-01-01";
        if(rbScheduleType.SelectedValue == "1")
        {

        }
        else
        {
            if(rblAction.SelectedValue == "T")
            {
                
                OSAEScheduleManager.ScheduleRecurringAdd(txtName.Text, "", "", "", "", ddlScript.SelectedItem.Text, tsTime.Hour.ToString() + ":" + tsTime.Minute.ToString() + ":" + tsTime.Second.ToString(),
                    chkSunday.Checked, chkMonday.Checked, chkTuesday.Checked, chkWednesday.Checked, chkThursday.Checked, chkFriday.Checked, chkSaturday.Checked, rbScheduleType.SelectedValue,
                    Int32.Parse(txtMinutes.Text), ddlMonthDay.SelectedValue, txtPickedDate.Text);
            }
            else
            {
                OSAEScheduleManager.ScheduleRecurringAdd(txtName.Text, ddlObject.SelectedValue, ddlMethod.SelectedValue, txtParam1.Text, txtParam2.Text, ddlScript.SelectedItem.Text, tsTime.Hour.ToString() + ":" + tsTime.Minute.ToString() + ":" + tsTime.Second.ToString(),
                    chkSunday.Checked, chkMonday.Checked, chkTuesday.Checked, chkWednesday.Checked, chkThursday.Checked, chkFriday.Checked, chkSaturday.Checked, rbScheduleType.SelectedValue,
                    Int32.Parse(txtMinutes.Text), ddlMonthDay.SelectedValue, txtPickedDate.Text);
            }
        }
        loadQueue();
        loadRecurring();
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        if (txtPickedDate.Text == "")
            txtPickedDate.Text = "2000-01-01";
        if(rblAction.SelectedValue == "T")
        {
                
            OSAEScheduleManager.ScheduleRecurringUpdate(hdnSelectedRecurringName.Text, txtName.Text, "", "", "", "", ddlScript.SelectedItem.Text, tsTime.Hour.ToString() + ":" + tsTime.Minute.ToString() + ":" + tsTime.Second.ToString(),
                chkSunday.Checked, chkMonday.Checked, chkTuesday.Checked, chkWednesday.Checked, chkThursday.Checked, chkFriday.Checked, chkSaturday.Checked, rbScheduleType.SelectedValue,
                Int32.Parse(txtMinutes.Text), ddlMonthDay.SelectedValue, txtPickedDate.Text);
        }
        else
        {
            OSAEScheduleManager.ScheduleRecurringUpdate(hdnSelectedRecurringName.Text, txtName.Text, ddlObject.SelectedValue, ddlMethod.SelectedValue, txtParam1.Text, txtParam2.Text, ddlScript.SelectedItem.Text, tsTime.Hour.ToString() + ":" + tsTime.Minute.ToString() + ":" + tsTime.Second.ToString(),
                chkSunday.Checked, chkMonday.Checked, chkTuesday.Checked, chkWednesday.Checked, chkThursday.Checked, chkFriday.Checked, chkSaturday.Checked, rbScheduleType.SelectedValue,
                Int32.Parse(txtMinutes.Text), ddlMonthDay.SelectedValue, txtPickedDate.Text);
        }
        loadQueue();
        loadRecurring();
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        OSAEScheduleManager.ScheduleRecurringDelete(hdnSelectedRecurringName.Text);
        int selectedRow = Int32.Parse(hdnSelectedRecurringRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedRecurringRow.Text = "";
        else
            hdnSelectedRecurringRow.Text = selectedRow.ToString();

        loadRecurring();
    }
    protected void btnQueueDelete_Click(object sender, EventArgs e)
    {
        OSAEScheduleManager.ScheduleQueueDelete(Int32.Parse(hdnSelectedQueueID.Text));
        int selectedRow = Int32.Parse(hdnSelectedQueueRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedQueueRow.Text = "";
        else
            hdnSelectedQueueRow.Text = selectedRow.ToString();
        loadQueue();
    }
}