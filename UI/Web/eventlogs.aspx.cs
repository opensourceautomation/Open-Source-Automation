using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class eventlogs : System.Web.UI.Page
{
    private OSAE.General.OSAELog Log = new OSAE.General.OSAELog();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) BindData();
    }

    private void BindData()
    {
        try
        {
            eventLogGridView.DataSource = OSAESql.RunSQL("SELECT DATE_FORMAT(log_time,'%m-%d %H:%i:%s.%f') as log_time,object_name,event_label,parameter_1,parameter_2,from_object_name FROM osae_v_event_log ORDER BY osae_v_event_log.log_time DESC LIMIT 500");
            eventLogGridView.DataBind();
        }
        catch (Exception ex)
        {
            Master.Log.Error("Error retreiving event log", ex);
        }
    }
   
    protected void clearLogButton_Click(object sender, EventArgs e)
    {
        OSAE.General.OSAELog.EventLogClear();
        BindData();
    }
}