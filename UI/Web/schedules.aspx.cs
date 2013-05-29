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
            loadDetails();
            alert.Visible = false;
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


            CheckBox active = ((CheckBox)e.Row.FindControl("chkActive"));

            Label lbl = ((Label)e.Row.FindControl("lblActive"));
            if (lbl.Text == "1")
                active.Checked = true;
            else
                active.Checked = false;
        }
    }

    private void loadQueue()
    {
        gvQueue.DataSource = OSAESql.RunSQL("SELECT schedule_id, queue_datetime, schedule_name FROM osae_v_schedule_queue ORDER BY queue_datetime DESC");
        gvQueue.DataBind();
    }

    private void loadRecurring()
    {
        gvRecurring.DataSource = OSAESql.RunSQL("SELECT recurring_id, interval_unit, schedule_name, COALESCE(active, 1) as active FROM osae_v_schedule_recurring ORDER BY schedule_name DESC");
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

    private void loadDetails()
    {
        OSAERecurringSchedule schedule = OSAEScheduleManager.GetRecurringSchedule(hdnSelectedRecurringName.Text);

        rbScheduleType.SelectedValue = schedule.Interval;
        rbScheduleType_SelectedIndexChanged(null, null);

        txtName.Text = schedule.Name;
        tsTime.Text = schedule.Time.Substring(0,5);
        txtMinutes.Text = schedule.Minutes;
        ddlMonthDay.SelectedItem.Text = schedule.MonthDay;

        string date = schedule.Date;
        DateTime dt = Convert.ToDateTime(date); 
        txtPickedDate.Text = dt.Year + "-" + dt.Month + "-" + dt.Day;

        if (schedule.Sunday == "1")
            chkSunday.Checked = true;
        if (schedule.Saturday == "1")
            chkSaturday.Checked = true;
        if (schedule.Monday == "1")
            chkMonday.Checked = true;
        if (schedule.Tuesday == "1")
            chkTuesday.Checked = true;
        if (schedule.Wednesday == "1")
            chkWednesday.Checked = true;
        if (schedule.Thursday == "1")
            chkThursday.Checked = true;
        if (schedule.Friday == "1")
            chkFriday.Checked = true;

        if (schedule.Active == "1")
            chkActive.Checked = true;

        if (schedule.Object != "")
        {
            rblAction.SelectedValue = "2";
            rblAction_SelectedIndexChanged(null, null);
            ddlObject.SelectedValue = schedule.Object;
            ddlMethod.SelectedValue = schedule.Method;
            txtParam1.Text = schedule.Param1;
            txtParam2.Text = schedule.Param2; 
        }
        else
        {
            rblAction.SelectedValue = "1";
            rblAction_SelectedIndexChanged(null, null);
            ddlScript.SelectedItem.Text = schedule.Script;
        }

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
            string date = txtPickedDate.Text + " " + tsTime.Text + ":00";
            DateTime dt = Convert.ToDateTime(date);  
            if(rblAction.SelectedValue == "1")
            {
                OSAEScheduleManager.ScheduleQueueAdd(dt, null, null, null, null, ddlScript.SelectedItem.Text, 0);
            }
            else
            {
                OSAEScheduleManager.ScheduleQueueAdd(dt, ddlObject.SelectedValue, ddlMethod.SelectedValue, txtParam1.Text, txtParam2.Text, "", 0);
            }
            lblAlert.Text = "One time schedule added successfully";
        }
        else
        {
            if(rblAction.SelectedValue == "1")
            {

                OSAEScheduleManager.ScheduleRecurringAdd(txtName.Text, null, null, null, null, ddlScript.SelectedItem.Text, tsTime.Text + ":00",
                    chkSunday.Checked, chkMonday.Checked, chkTuesday.Checked, chkWednesday.Checked, chkThursday.Checked, chkFriday.Checked, chkSaturday.Checked, rbScheduleType.SelectedValue,
                    Int32.Parse(txtMinutes.Text), ddlMonthDay.SelectedValue, txtPickedDate.Text, chkActive.Checked);
            }
            else
            {
                OSAEScheduleManager.ScheduleRecurringAdd(txtName.Text, ddlObject.SelectedValue, ddlMethod.SelectedValue, txtParam1.Text, txtParam2.Text, "", tsTime.Text + ":00",
                    chkSunday.Checked, chkMonday.Checked, chkTuesday.Checked, chkWednesday.Checked, chkThursday.Checked, chkFriday.Checked, chkSaturday.Checked, rbScheduleType.SelectedValue,
                    Int32.Parse(txtMinutes.Text), ddlMonthDay.SelectedValue, txtPickedDate.Text, chkActive.Checked);
            }
            lblAlert.Text = "Recurring schedule added successfully";
        }
        loadQueue();
        loadRecurring();
        
        alert.Visible = true;
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        if (txtPickedDate.Text == "")
            txtPickedDate.Text = "2000-01-01";
        if(rblAction.SelectedValue == "1")
        {

            OSAEScheduleManager.ScheduleRecurringUpdate(hdnSelectedRecurringName.Text, txtName.Text, "", "", "", "", ddlScript.SelectedItem.Text, tsTime.Text + ":00",
                chkSunday.Checked, chkMonday.Checked, chkTuesday.Checked, chkWednesday.Checked, chkThursday.Checked, chkFriday.Checked, chkSaturday.Checked, rbScheduleType.SelectedValue,
                Int32.Parse(txtMinutes.Text), ddlMonthDay.SelectedValue, txtPickedDate.Text, chkActive.Checked);
        }
        else
        {
            OSAEScheduleManager.ScheduleRecurringUpdate(hdnSelectedRecurringName.Text, txtName.Text, ddlObject.SelectedValue, ddlMethod.SelectedValue, txtParam1.Text, txtParam2.Text, ddlScript.SelectedItem.Text, tsTime.Text + ":00",
                chkSunday.Checked, chkMonday.Checked, chkTuesday.Checked, chkWednesday.Checked, chkThursday.Checked, chkFriday.Checked, chkSaturday.Checked, rbScheduleType.SelectedValue,
                Int32.Parse(txtMinutes.Text), ddlMonthDay.SelectedValue, txtPickedDate.Text, chkActive.Checked);
        }
        loadQueue();
        loadRecurring();
        lblAlert.Text = "Recurring schedule updated successfully";
        alert.Visible = true;
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