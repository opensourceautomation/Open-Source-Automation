using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class eventlogs : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        try
        {
            eventLogGridView.DataSource = OSAESql.RunSQL("SELECT log_time,object_name,event_label,parameter_1,parameter_2,from_object_name FROM osae_v_event_log ORDER BY log_time DESC LIMIT 500");
            eventLogGridView.DataBind();
        }
        catch (Exception ex)
        {
            logging.AddToLog("Error retreiving event log: " + ex.Message, true);
        }
    }
   
    protected void clearLogButton_Click(object sender, EventArgs e)
    {
        logging.EventLogClear();
        BindData();
    }
}