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

    public void RaisePostBackEvent(string eventArgument)
    {
       string[] args = eventArgument.Split('_');

       if (args[0] == "gvLogs")
       {
           hdnSelectedRow.Text = args[1];
           LoadLogContent();
       }
    }

    private void LoadLogContent()
    {
        if (hdnSelectedRow.Text != string.Empty)
        {
            logContentTextBox.Text = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs\" + gvLogs.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["logName"].ToString() + ".log");
            panelLogContent.Visible = true;
            btnClearLog.Visible = true;
        }
        else
            btnClearLog.Visible = false;
    }


    /// <summary>
    /// See what logs are available and add them to the list box
    /// </summary>
    private void GetLogs()
    {
        List<string> logsList = new List<string>();
        if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs\"))
        {
            string[] fileList = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs\");

            var list = from f in fileList
                       select new { logName = Path.GetFileNameWithoutExtension(f) };

            gvLogs.DataKeyNames = new string[] { "logName" };

            gvLogs.DataSource = list;
            gvLogs.DataBind();
        }        
    }

    protected void gvLogs_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
            {
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            }
            else
            {
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            }
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvLogs_" + e.Row.RowIndex.ToString()));
        }

    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        GetLogs();
        LoadLogContent();        
    }
    protected void clearLogs_Click(object sender, EventArgs e)
    {
        string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs\");

        try
        {
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
        catch (Exception ex)
        {
            // not going to do anything as the file may be in use so just carry on
        }

        GetLogs();
        LoadLogContent();       
    }
    protected void clearLog_Click(object sender, EventArgs e)
    {
        try
        {
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs\" + gvLogs.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["logName"].ToString() + ".log");
        }
        catch (Exception ex)
        {
            // not going to do anything as the file may be in use so just carry on
        }

        GetLogs();
        panelLogContent.Visible = false;
        btnClearLog.Visible = false;
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        //try
        //{
        string zipFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\OSA_Logs.zip";

            FastZip fastZip = new FastZip();
            if (File.Exists(zipFileName))
                File.Delete(zipFileName);

            fastZip.CreateZip(zipFileName, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\OSAE\Logs", true, "");

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("Content-Disposition", "attachment; filename=OSA_Logs.zip");
            Response.AddHeader("Content-Length", new FileInfo(zipFileName).Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.Flush();
            Response.TransmitFile(zipFileName);
            Response.End();
        ///}
        //catch (Exception ex)
        //{
            // not going to do anything as the file may be in use so just carry on
        //}

    }
}