using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class methodlog : System.Web.UI.Page
{
    // Get current Admin Trust Settings
    OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();
    Logging logging = Logging.GetLogger("Web UI");

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx?ReturnUrl=methodlog.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("Method Log Trust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet) Response.Redirect("~/permissionError.aspx");
        SetSessionTimeout();
        if (!IsPostBack) BindData();
        applySecurity();
    }

    private void BindData()
    {
        try
        {
            methodLogGridView.DataSource = OSAESql.RunSQL("SELECT entry_time, osae_v_method_log.object_name, method_name, if(method_name='RUN SCRIPT', (select script_name from osae_script where script_id=parameter_1), parameter_1) as parameter_1, parameter_2, osae_object.object_name As from_object FROM osae_v_method_log join osae_object on (osae_v_method_log.from_object_id = osae_object.object_id) ORDER BY osae_v_method_log.entry_time DESC LIMIT 500");
            methodLogGridView.DataBind();
        }
        catch (Exception ex)
        {
            Master.Log.Error("Error retreiving method log: " + ex.Message);
        }
    }
   
    protected void clearLogButton_Click(object sender, EventArgs e)
    {
        logging.EventLogClear();
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

    private void SetSessionTimeout()
    {
        try
        {
            int timeout = 0;
            if (int.TryParse(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "Timeout").Value, out timeout))
                Session.Timeout = timeout;
            else Session.Timeout = 60;
        }
        catch (Exception ex)
        {
            Master.Log.Error("Error setting session timeout", ex);
            Response.Redirect("~/error.aspx");
        }
    }
}