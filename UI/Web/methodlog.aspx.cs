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
            methodLogGridView.DataSource = OSAESql.RunSQL("SELECT entry_time, osae_v_method_log.object_name, method_name, if(method_name='RUN SCRIPT', (select script_name from osae_script where script_id=parameter_1), parameter_1) as parameter_1, parameter_2, osae_object.object_name As from_object FROM osae_v_method_log join osae_object on (osae_v_method_log.from_object_id = osae_object.object_id) ORDER BY osae_v_method_log.entry_time DESC LIMIT 500");
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

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        BindData();
    }
}