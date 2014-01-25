using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class methodlog : System.Web.UI.Page
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
            methodLogGridView.DataSource = OSAESql.RunSQL("SELECT entry_time, object_name, method_name, parameter_1, parameter_2, debug_trace FROM osae_v_method_log ORDER BY entry_time DESC, method_log_id DESC LIMIT 500");
            methodLogGridView.DataBind();
        }
        catch (Exception ex)
        {
            logging.AddToLog("Error retreiving method log: " + ex.Message, true);
        }
    }
   
    protected void clearLogButton_Click(object sender, EventArgs e)
    {
        logging.EventLogClear();
        BindData();
    }
}