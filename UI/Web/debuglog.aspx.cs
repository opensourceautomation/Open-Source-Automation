using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class debuglog : System.Web.UI.Page
{
    //OSAELog
    private OSAE.General.OSAELog Log = new OSAE.General.OSAELog("WebUI");

    // Get current Admin Trust Settings
    OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("DebugLogTrust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet)
        {
            Response.Redirect("~/permissionError.aspx");
        }
        if (!IsPostBack) BindData();
        applySecurity();
    }

    private void BindData()
    {
        try
        {
            debugLogGridView.DataSource = OSAESql.RunSQL("SELECT log_time, entry, debug_trace FROM osae_debug_log ORDER BY log_time DESC LIMIT 500");
            debugLogGridView.DataBind();
        }
        catch (Exception ex)
        { Master.Log.Error("Error retreiving debug log", ex); }
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