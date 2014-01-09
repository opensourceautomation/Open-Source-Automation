using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class debuglog : System.Web.UI.Page
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
            debugLogGridView.DataSource = OSAESql.RunSQL("SELECT log_time, entry, debug_trace FROM osae_debug_log ORDER BY log_time DESC LIMIT 500");
            debugLogGridView.DataBind();
        }
        catch (Exception ex)
        {
            Master.Log.Error("Error retreiving debug log", ex);
        }
    }
   
    protected void clearLogButton_Click(object sender, EventArgs e)
    {
        logging.EventLogClear();
        BindData();
    }
}