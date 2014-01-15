using OSAE;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
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


    private void GetLogs()
    {
        string source = "ALL";
        try
        {
            DropDownList ddlSource = (DropDownList)gvLog.HeaderRow.FindControl("ddlSource");
            source = ddlSource.SelectedValue;
        }
        catch (Exception ex)
        {
            
        }

        gvLog.DataSource = OSAE.General.OSAELog.Load(chkInfo.Checked, chkDebug.Checked, chkError.Checked, source);
        gvLog.DataBind();

        
        
        DataSet ds = OSAE.General.OSAELog.LoadSources();
        if (ds.Tables[0].Rows.Count > 0)
        {
            DropDownList ddlSource2 = (DropDownList)gvLog.HeaderRow.FindControl("ddlSource");
            ddlSource2.DataSource = ds;
            ddlSource2.DataTextField = "Logger";
            ddlSource2.DataValueField = "Logger";
            ddlSource2.DataBind();
            ddlSource2.SelectedValue = source;
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        GetLogs();     
    }
    protected void btnClear_Click(object sender, EventArgs e)
    {
        OSAE.General.OSAELog.Clear();
        GetLogs();   
    }
    protected void CheckedChanged(object sender, EventArgs e)
    {
        GetLogs();
    }
}