using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class patterns : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvPatterns")
        {
            hdnSelectedPatternRow.Text = args[1];
            hdnSelectedPatternName.Text = gvPatterns.DataKeys[Int32.Parse(hdnSelectedPatternRow.Text)]["pattern"].ToString();
            hdnSelectedMatchRow.Text = "";
            loadMatches();
            loadScripts();
        }
        else if (args[0] == "gvMatches")
        {
            hdnSelectedMatchRow.Text = args[1];
            hdnSelectedMatchID.Text = gvMatches.DataKeys[Int32.Parse(hdnSelectedMatchRow.Text)]["match_id"].ToString();
            hdnSelectedMatchName.Text = gvMatches.DataKeys[Int32.Parse(hdnSelectedMatchRow.Text)]["match"].ToString();
        }
        else if (args[0] == "gvScripts")
        {
            hdnSelectedScriptRow.Text = args[1];
            hdnSelectedScriptID.Text = gvScripts.DataKeys[Int32.Parse(hdnSelectedScriptRow.Text)]["pattern_script_id"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        loadPatterns();
        if (!this.IsPostBack)
        {
            loadScriptDDL();
        }

        if (hdnSelectedPatternRow.Text != "")
        {
            loadMatches();
            loadScripts();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedPatternRow.Text != "")
        {
            gvPatterns.Rows[Int32.Parse(hdnSelectedPatternRow.Text)].Attributes.Remove("onmouseout");
            gvPatterns.Rows[Int32.Parse(hdnSelectedPatternRow.Text)].Style.Add("background", "lightblue");
            pnlMatchForm.Visible = true;
            pnlScriptForm.Visible = true;
        }
        if (hdnSelectedMatchRow.Text != "")
        {
            gvMatches.Rows[Int32.Parse(hdnSelectedMatchRow.Text)].Attributes.Remove("onmouseout");
            gvMatches.Rows[Int32.Parse(hdnSelectedMatchRow.Text)].Style.Add("background", "lightblue");
        }

        if (hdnSelectedScriptRow.Text != "")
        {
            gvScripts.Rows[Int32.Parse(hdnSelectedScriptRow.Text)].Attributes.Remove("onmouseout");
            gvScripts.Rows[Int32.Parse(hdnSelectedScriptRow.Text)].Style.Add("background", "lightblue");
        }
    }

    protected void gvPatterns_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvPatterns_" + e.Row.RowIndex.ToString()));
        }

    }
    protected void gvMatches_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvMatches_" + e.Row.RowIndex.ToString()));
        }

    }
    protected void gvScripts_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvScripts_" + e.Row.RowIndex.ToString()));
        }

    }

    private void loadPatterns()
    {
        gvPatterns.DataSource = OSAESql.RunSQL("SELECT pattern, pattern_id FROM osae_pattern ORDER BY pattern");
        gvPatterns.DataBind();
    }

    private void loadMatches()
    {
        gvMatches.DataSource = OSAESql.RunSQL("SELECT `match`, match_id FROM osae_v_pattern WHERE pattern = '" + hdnSelectedPatternName.Text + "' ORDER BY `match`");
        gvMatches.DataBind();
    }

    private void loadScripts()
    {
        gvScripts.DataSource = OSAESql.RunSQL("SELECT script_name, os.script_id, script_sequence, pattern_script_id FROM osae_script os INNER JOIN osae_pattern_script s ON s.script_id = os.script_id INNER JOIN osae_pattern p ON p.pattern_id = s.pattern_id WHERE pattern = '" + hdnSelectedPatternName.Text + "' ORDER BY script_sequence ASC");
        gvScripts.DataBind();
    }

    private void loadScriptDDL()
    {
        ddlScript.DataSource = OSAESql.RunSQL("SELECT script_name as Text, script_id as Value FROM osae_script ORDER BY script_name"); ;
        ddlScript.DataBind();
    }
    protected void btnScriptDelete_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.PatternScriptDelete(hdnSelectedScriptID.Text);
        loadScripts();
        int selectedRow = Int32.Parse(hdnSelectedScriptRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedScriptRow.Text = "";
        else
            hdnSelectedScriptRow.Text = selectedRow.ToString();
    }
    protected void btnScriptAdd_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.PatternScriptAdd(hdnSelectedPatternName.Text, Int32.Parse(ddlScript.SelectedValue));
        loadScripts();
    }
    protected void btnMatchDelete_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.PatternMatchDelete(hdnSelectedMatchName.Text);
        loadMatches();
        int selectedRow = Int32.Parse(hdnSelectedMatchRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedMatchRow.Text = "";
        else
            hdnSelectedMatchRow.Text = selectedRow.ToString();
    }
    protected void btnMatchAdd_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.PatternMatchAdd(hdnSelectedPatternName.Text, txtMatch.Text);
        loadMatches();
    }
    protected void btnPatternAdd_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.PatternAdd(txtPattern.Text);
        loadPatterns();
    }
    protected void btnPatternDelete_Click(object sender, EventArgs e)
    {
        if (OSAEScriptManager.PatternDelete(hdnSelectedPatternName.Text))
        {
            loadPatterns();
            int selectedRow = Int32.Parse(hdnSelectedPatternRow.Text) - 1;
            if (selectedRow < 0)
                hdnSelectedPatternRow.Text = "";
            else
                hdnSelectedPatternRow.Text = selectedRow.ToString();
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "MyKey",
                "alert('Unable to delete pattern.  Maybe a script is attached?');",
                true);
        }
    }
}