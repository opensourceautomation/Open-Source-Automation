using OSAE;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;
using System.Web;

public partial class logs : System.Web.UI.Page
{
    // Get current Admin Trust Settings
    OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx?ReturnUrl=logs.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("Server Log Trust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet) Response.Redirect("~/permissionError.aspx");
        SetSessionTimeout();
        if (!IsPostBack) GetLogs();
        DropDownList ddlSource2 = (DropDownList)gvLog.HeaderRow.FindControl("ddlSource");
        btnClear.ToolTip = "Clears " + ddlSource2.Text + " Log Entries";
        btnClear2.ToolTip = "Clears " + ddlSource2.Text + " Log Entries";
        btnExport.ToolTip = "Exports " + ddlSource2.Text + " Log Entries";
        btnExport2.ToolTip = "Exports " + ddlSource2.Text + " Log Entries";
        // Apply Security Admin Settings
        applySecurity();
    }

    private void GetLogs()
    {
        string source = "ALL";
        try
        {
            DropDownList ddlSource = (DropDownList)gvLog.HeaderRow.FindControl("ddlSource");
            source = ddlSource.SelectedValue;
        }
        catch { }

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
        DropDownList ddlSource2 = (DropDownList)gvLog.HeaderRow.FindControl("ddlSource");
        if (ddlSource2.Text == "ALL")
        {
            OSAE.General.OSAELog.Clear();
        }
        else
        {
            OSAE.General.OSAELog.Clear_Log(ddlSource2.Text);
            Response.Redirect("~/logs.aspx");
        }
        GetLogs();   
    }

    #region Export
    protected void btnExport_Click(object sender, EventArgs e)
    {
        DropDownList ddlSource2 = (DropDownList)gvLog.HeaderRow.FindControl("ddlSource");
        string myExportLog = "";
        if (ddlSource2.SelectedValue == "ALL")
        {
            myExportLog = "Server%20Log";
        }
        else
        {
            myExportLog = ddlSource2.SelectedValue.Replace(" ", "%20");
        }
        Response.Redirect(@"~/importexport.aspx?eType=Log&eObject=" + myExportLog);
    }
    #endregion

    protected void CheckedChanged(object sender, EventArgs e)
    {
        GetLogs();
    }

    #region Trust Settings
    protected void applySecurity()
    {
        int sessTrust = Convert.ToInt32(Session["TrustLevel"].ToString());
        btnClear.Enabled = false;
        btnClear2.Enabled = false;
        if (sessTrust >= adSet.LogsClearTrust)
        {
            btnClear.Enabled = true;
            btnClear2.Enabled = true;
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