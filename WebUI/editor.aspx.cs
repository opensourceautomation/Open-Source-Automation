using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using OSAE;

public partial class editor : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("WebUI");

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindData();
        }
    }

    private void BindData()
    {
        scriptProcessorTypeDropDownList.DataSource = OSAEScriptManager.GetScriptProcessors();
        scriptProcessorTypeDropDownList.DataTextField = "Name";
        scriptProcessorTypeDropDownList.DataValueField = "ID";
        scriptProcessorTypeDropDownList.DataBind();
    }
}