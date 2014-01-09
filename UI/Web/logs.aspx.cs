using OSAE;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

public partial class logs : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetLogs();           
        }
    }




    /// <summary>
    /// See what logs are available and add them to the list box
    /// </summary>
    private void GetLogs()
    {
        try
        {
            gvLog.DataSource = OSAE.General.OSAELog.Load();
            gvLog.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        GetLogs();     
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        ///

    }
}