using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using OSAE;

public partial class scripts : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvScripts")
        {
            hdnSelectedRow.Text = args[1];
            hdnSelectedScriptName.Text = gvScripts.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["script_name"].ToString();
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "MyKey",
                "setSytaxHighlighter();",
                true);
        }
        else if (args[0] == "gvEventScripts")
        {
            hdnSelectedEventScriptRow.Text = args[1];
            hdnSelectedEventScriptID.Text = gvEventScripts.DataKeys[Int32.Parse(hdnSelectedEventScriptRow.Text)]["event_script_id"].ToString();
            btnDeleteEventScript.Visible = true;
        }
        else if (args[0] == "gvObjTypeScripts")
        {
            hdnSelectedObjTypeEventScriptRow.Text = args[1];
            hdnSelectedObjTypeEventScriptID.Text = gvObjTypeScripts.DataKeys[Int32.Parse(hdnSelectedObjTypeEventScriptRow.Text)]["object_type_event_script_id"].ToString();
            btnDeleteObjTypeEventScript.Visible = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        loadScripts();
        alert.Visible = false;
        saveAlert.Visible = false;
        deleteAlert.Visible = false;
        if (!this.IsPostBack)
        {
            loadDDLs();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedRow.Text != "")
        {
            gvScripts.Rows[Int32.Parse(hdnSelectedRow.Text)].Attributes.Remove("onmouseout");
            gvScripts.Rows[Int32.Parse(hdnSelectedRow.Text)].Style.Add("background", "lightblue");
            txtName.Text = hdnSelectedScriptName.Text;
            ddlScriptProcessor.SelectedValue = gvScripts.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["script_processor_name"].ToString();
            hdnScript.Value = OSAEScriptManager.GetScript(gvScripts.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["script_id"].ToString());
            loadLinkage(gvScripts.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["script_id"].ToString());
        }
        if (hdnSelectedEventScriptRow.Text != "")
        {
            gvEventScripts.Rows[Int32.Parse(hdnSelectedEventScriptRow.Text)].Attributes.Remove("onmouseout");
            gvEventScripts.Rows[Int32.Parse(hdnSelectedEventScriptRow.Text)].Style.Add("background", "lightblue");
        }

        if (hdnSelectedObjTypeEventScriptRow.Text != "")
        {
            gvObjTypeScripts.Rows[Int32.Parse(hdnSelectedObjTypeEventScriptRow.Text)].Attributes.Remove("onmouseout");
            gvObjTypeScripts.Rows[Int32.Parse(hdnSelectedObjTypeEventScriptRow.Text)].Style.Add("background", "lightblue");
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
    
    protected void gvEventScripts_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvEventScripts_" + e.Row.RowIndex.ToString()));
        }

    }
    
    protected void gvObjTypeScripts_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvObjTypeScripts_" + e.Row.RowIndex.ToString()));
        }

    }
    
    private void loadScripts()
    {
        gvScripts.DataSource = OSAESql.RunSQL("SELECT script_name, script_id, s.script_processor_id, script_processor_name FROM osae_script s INNER JOIN osae_script_processors sp ON sp.script_processor_id = s.script_processor_id ORDER BY script_name");
        gvScripts.DataBind();

        
    }

    private void loadEventScripts()
    {
        gvEventScripts.DataSource = OSAESql.RunSQL("SELECT script_name, script_id, script_sequence, event_script_id FROM osae_v_object_event_script WHERE object_id = " + ddlObject.SelectedValue + " AND event_name = '" + ddlEvent.SelectedValue + "' ORDER BY script_sequence ASC");
        gvEventScripts.DataBind();

        if (gvEventScripts.Rows.Count == 0)
            pnlEventScripts.Visible = false;
        else
            pnlEventScripts.Visible = true;

    }

    private void loadObjTypeEventScripts()
    {
        gvObjTypeScripts.DataSource = OSAESql.RunSQL("SELECT script_name, script_id, script_sequence, object_type_event_script_id FROM osae_v_object_type_event_script WHERE object_type_id = " + ddlObjectType.SelectedValue + " AND event_name = '" + ddlObjTypeEvent.SelectedValue + "' ORDER BY script_sequence ASC");
        gvObjTypeScripts.DataBind();

        if (gvObjTypeScripts.Rows.Count == 0)
            pnlObjTypeEventScripts.Visible = false;
        else
            pnlObjTypeEventScripts.Visible = true;

    }

    private void loadDDLs()
    {
        ddlScriptProcessor.DataSource = OSAESql.RunSQL("SELECT script_processor_name as Text, script_processor_name as Value FROM osae_script_processors ORDER BY script_processor_name"); ;
        ddlScriptProcessor.DataBind();
        if (ddlScriptProcessor.Items.Count == 0)
            ddlScriptProcessor.Visible = false;
        else
            ddlScriptProcessor.Visible = true;
        ddlScriptProcessor.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlObject.DataSource = OSAESql.RunSQL("SELECT object_name as Text, object_id as Value FROM osae_object ORDER BY object_name"); ;
        ddlObject.DataBind();
        if (ddlObject.Items.Count == 0)
            ddlObject.Visible = false;
        else
            ddlObject.Visible = true;
        ddlObject.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlObjectType.DataSource = OSAESql.RunSQL("SELECT object_type as Text, object_type_id as Value FROM osae_object_type ORDER BY object_type"); ;
        ddlObjectType.DataBind();
        if (ddlObjectType.Items.Count == 0)
            ddlObjectType.Visible = false;
        else
            ddlObjectType.Visible = true;
        ddlObjectType.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlScript.DataSource = OSAESql.RunSQL("SELECT script_name as Text, script_id as Value  FROM osae_script ORDER BY script_name"); ;
        ddlScript.DataBind();

        ddlObjTypeScript.DataSource = OSAESql.RunSQL("SELECT script_name as Text, script_id as Value  FROM osae_script ORDER BY script_name"); ;
        ddlObjTypeScript.DataBind();
    }

    private void loadLinkage(string id)
    {
        gvLinkage.DataSource = OSAESql.RunSQL("SELECT object_name, event_name FROM osae_v_object_event_script WHERE script_id=" + id);
        gvLinkage.DataBind();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (txtName.Text == "" || ddlScriptProcessor.SelectedValue == "")
        {
            alert.Visible = true;
        }
        else
        {
            OSAEScriptManager.ScriptAdd(txtName.Text,ddlScriptProcessor.SelectedValue, hdnScript.Value);
            loadScripts();
            loadDDLs();
            saveAlert.Visible = true;
        }
    }
    
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        if (txtName.Text == "" || ddlScriptProcessor.SelectedValue == "")
        {
            alert.Visible = true;
        }
        else
        {
            OSAEScriptManager.ScriptUpdate(hdnSelectedScriptName.Text, txtName.Text, ddlScriptProcessor.SelectedValue, hdnScript.Value);
            loadScripts();
            loadDDLs();
            saveAlert.Visible = true;
            hdnSelectedScriptName.Text = txtName.Text;
        }
        Page.ClientScript.RegisterStartupScript(
                GetType(),
                "MyKey",
                "setSytaxHighlighter();",
                true);
    }
    
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.ScriptDelete(hdnSelectedScriptName.Text);
        loadScripts();
        deleteAlert.Visible = true;
        int selectedRow = Int32.Parse(hdnSelectedRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedRow.Text = "";
        else
            hdnSelectedRow.Text = selectedRow.ToString();
    }

    protected void ddlObject_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlEvent.DataSource = OSAESql.RunSQL("SELECT event_label as Text, event_name as Value FROM osae_v_object_event WHERE object_id = " + ddlObject.SelectedValue + " ORDER BY event_label");
        ddlEvent.Items.Insert(0, new ListItem(String.Empty, String.Empty));
        ddlEvent.DataBind();
        loadEventScripts();
    }
    
    protected void ddlEvent_SelectedIndexChanged(object sender, EventArgs e)
    {
        loadEventScripts();
    }
    
    protected void btnAddEventScript_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.ObjectEventScriptAdd(ddlObject.SelectedItem.Text, ddlEvent.SelectedItem.Value, Int32.Parse(ddlScript.SelectedValue));
        loadEventScripts();
    }

    protected void btnDeleteEventScript_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.ObjectEventScriptDelete(hdnSelectedEventScriptID.Text);
        loadEventScripts();
        int selectedRow = Int32.Parse(hdnSelectedEventScriptRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedEventScriptRow.Text = "";
        else
            hdnSelectedEventScriptRow.Text = selectedRow.ToString();
    }

    protected void ddlObjectType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlObjTypeEvent.DataSource = OSAESql.RunSQL("SELECT event_label as Text, event_name as Value FROM osae_v_object_type_event WHERE object_type_id = " + ddlObjectType.SelectedValue + " ORDER BY event_label");
        ddlObjTypeEvent.Items.Insert(0, new ListItem(String.Empty, String.Empty));
        ddlObjTypeEvent.DataBind();
        loadObjTypeEventScripts();
    }

    protected void ddlObjTypeEvent_SelectedIndexChanged(object sender, EventArgs e)
    {
        loadObjTypeEventScripts();
    }

    protected void btnDeleteObjTypeEventScript_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.ObjectTypeEventScriptDelete(hdnSelectedObjTypeEventScriptID.Text);
        loadObjTypeEventScripts();
        int selectedRow = Int32.Parse(hdnSelectedObjTypeEventScriptRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedObjTypeEventScriptRow.Text = "";
        else
            hdnSelectedObjTypeEventScriptRow.Text = selectedRow.ToString();
    }
    
    protected void btnAddObjTypeEventScript_Click(object sender, EventArgs e)
    {
        OSAEScriptManager.ObjectTypeEventScriptAdd(ddlObjectType.SelectedItem.Text, ddlObjTypeEvent.SelectedItem.Value, Int32.Parse(ddlObjTypeScript.SelectedValue));
        loadObjTypeEventScripts();
    }
}