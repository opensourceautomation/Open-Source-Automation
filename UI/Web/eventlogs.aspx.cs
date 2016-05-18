using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class eventlogs : System.Web.UI.Page
{
    private OSAE.General.OSAELog Log = new OSAE.General.OSAELog("SYSTEM");

    // Get current Admin Trust Settings
    OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("Event Log Trust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet) Response.Redirect("~/permissionError.aspx");
        if (!IsPostBack) BindData();
        applySecurity();
    }

    private void BindData()
    {
        try
        {
            eventLogGridView.DataSource = OSAESql.RunSQL("SELECT log_time,object_name,event_label,parameter_1,parameter_2,from_object_name FROM osae_v_event_log ORDER BY osae_v_event_log.log_time DESC LIMIT 500");
            eventLogGridView.DataBind();
        }
        catch (Exception ex)
        { Master.Log.Error("Error retreiving event log", ex); }
    }
   
    protected void clearLogButton_Click(object sender, EventArgs e)
    {
        OSAE.General.OSAELog.EventLogClear();
        BindData();
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        BindData();
    }

    #region Trust Settings
    protected void applySecurity()
    {
        int sessTrust = Convert.ToInt32(Session["TrustLevel"].ToString());
        clearLogButton.Enabled = false;
        clearLogButton2.Enabled = false;
        if (sessTrust >= adSet.LogsClearTrust)
        {
            clearLogButton.Enabled = true;
            clearLogButton2.Enabled = true;
        }
    }
    #endregion
}