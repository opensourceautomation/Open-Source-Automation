using OSAE;
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
            logContentTextBox.Text = File.ReadAllText(Common.ApiPath + @"\Logs\" + gvLogs.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["logName"].ToString() + ".log");
            panelLogContent.Visible = true;
        }
    }


    /// <summary>
    /// See what logs are available and add them to the list box
    /// </summary>
    private void GetLogs()
    {
        List<string> logsList = new List<string>();
        if (Directory.Exists(Common.ApiPath + @"\Logs"))
        {
            string[] fileList = Directory.GetFiles(Common.ApiPath + @"\Logs");

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
}