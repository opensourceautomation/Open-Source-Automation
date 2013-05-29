using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using OSAE;

public partial class objtypes : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvObjectTypes")
        {
            hdnSelectedRow.Text = args[1];
            panelEditForm.Visible = true;
            hdnSelectedPropRow.Text = "";
            hdnSelectedStateRow.Text = "";
            hdnSelectedMethodRow.Text = "";
            hdnSelectedEventRow.Text = "";
            hdnSelectedObjectName.Text = gvObjectTypes.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["object_type"].ToString();
            loadDDLs();
            loadProperties();
            loadStates();
            loadMethods();
            loadEvents();
            loadDetails();
        }
        else if (args[0] == "gvProperties")
        {
            hdnSelectedPropRow.Text = args[1];
            panelPropForm.Visible = true;
            hdnSelectedPropName.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_name"].ToString();
            loadPropertyOptions();
        }
        else if (args[0] == "gvStates")
        {
            hdnSelectedStateRow.Text = args[1];
            pnlStateForm.Visible = true;
            hdnSelectedStateName.Text = gvStates.DataKeys[Int32.Parse(hdnSelectedStateRow.Text)]["state_name"].ToString();
        }
        else if (args[0] == "gvMethods")
        {
            hdnSelectedMethodRow.Text = args[1];
            pnlMethodForm.Visible = true;
            hdnSelectedMethodName.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["method_name"].ToString();
        }
        else if (args[0] == "gvEvents")
        {
            hdnSelectedEventRow.Text = args[1];
            pnlEventForm.Visible = true;
            hdnSelectedEventName.Text = gvEvents.DataKeys[Int32.Parse(hdnSelectedEventRow.Text)]["event_name"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        loadObjectTypes();

        gvObjectTypes.DataSource = OSAESql.RunSQL("SELECT base_type, object_type, object_type_description FROM osae_v_object_type ORDER BY base_type");
        gvObjectTypes.DataBind();

        if (hdnSelectedPropRow.Text != "")
        {
            loadProperties();
        }
        if (hdnSelectedStateRow.Text != "")
        {
            loadStates();
        }
        if (hdnSelectedMethodRow.Text != "")
        {
            loadMethods();
        }
        if (hdnSelectedEventRow.Text != "")
        {
            loadEvents();
        }
        if (!this.IsPostBack)
        {
            loadDDLs();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedRow.Text != "")
        {
            gvObjectTypes.Rows[Int32.Parse(hdnSelectedRow.Text)].Attributes.Remove("onmouseout");
            gvObjectTypes.Rows[Int32.Parse(hdnSelectedRow.Text)].Style.Add("background", "lightblue");
        }
        if (hdnSelectedPropRow.Text != "")
        {
            txtPropName.Text = hdnSelectedPropName.Text;
            txtPropDefault.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_default"].ToString();
            chkTrackChanges.Checked = (bool)gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["track_history"];
            ddlPropType.SelectedValue = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_datatype"].ToString();
            gvProperties.Rows[Int32.Parse(hdnSelectedPropRow.Text)].Attributes.Remove("onmouseout");
            gvProperties.Rows[Int32.Parse(hdnSelectedPropRow.Text)].Style.Add("background", "lightblue");
        }
        if (hdnSelectedStateRow.Text != "")
        {
            gvStates.Rows[Int32.Parse(hdnSelectedStateRow.Text)].Attributes.Remove("onmouseout");
            gvStates.Rows[Int32.Parse(hdnSelectedStateRow.Text)].Style.Add("background", "lightblue");
            txtStateName.Text = hdnSelectedStateName.Text;
            txtStateLabel.Text = gvStates.DataKeys[Int32.Parse(hdnSelectedStateRow.Text)]["state_label"].ToString();
        }
        if (hdnSelectedMethodRow.Text != "")
        {
            gvMethods.Rows[Int32.Parse(hdnSelectedMethodRow.Text)].Attributes.Remove("onmouseout");
            gvMethods.Rows[Int32.Parse(hdnSelectedMethodRow.Text)].Style.Add("background", "lightblue");
            txtMethodName.Text = hdnSelectedMethodName.Text;
            txtMethodLabel.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["method_label"].ToString();
            txtParam1Label.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_1_label"].ToString();
            txtParam2Label.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_2_label"].ToString();
            txtParam1Default.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_1_default"].ToString();
            txtParam2Default.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_2_default"].ToString();
        }
        if (hdnSelectedEventRow.Text != "")
        {
            gvEvents.Rows[Int32.Parse(hdnSelectedEventRow.Text)].Attributes.Remove("onmouseout");
            gvEvents.Rows[Int32.Parse(hdnSelectedEventRow.Text)].Style.Add("background", "lightblue");
            txtEventName.Text = hdnSelectedEventName.Text;
            txtEventLabel.Text = gvEvents.DataKeys[Int32.Parse(hdnSelectedEventRow.Text)]["event_label"].ToString();
        }

        //if (gvMethods.Rows.Count == 0)
        //    divMethods.Visible = false;
        //else
        //    divMethods.Visible = true;

        //if (gvStates.Rows.Count == 0)
        //    divStates.Visible = false;
        //else
        //    divStates.Visible = true;

        //if (gvProperties.Rows.Count == 0)
        //    divProps.Visible = false;
        //else
        //    divProps.Visible = true;

        //if (gvEvents.Rows.Count == 0)
        //    divEvents.Visible = false;
        //else
        //    divEvents.Visible = true;
    }

    protected void gvObjectTypes_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvObjectTypes_" + e.Row.RowIndex.ToString()));
        }

    }

    protected void gvProperties_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvProperties_" + e.Row.RowIndex.ToString()));
        }

    }

    protected void gvStates_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvStates_" + e.Row.RowIndex.ToString()));
        }

    }

    protected void gvMethods_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvMethods_" + e.Row.RowIndex.ToString()));
        }

    }

    protected void gvEvents_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(../images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvEvents_" + e.Row.RowIndex.ToString()));
        }

    }

    protected void gvPropOptions_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", "selectPropOptionsItem('" + gvPropOptions.DataKeys[e.Row.RowIndex]["option_name"].ToString() + "', this);");
        }

    }

    private void loadDDLs()
    {
        ddlBaseType.DataSource = OSAESql.RunSQL("SELECT object_type as Text, object_type as Value FROM osae_object_type"); ;
        ddlBaseType.DataBind();
        if (ddlBaseType.Items.Count == 0)
            ddlBaseType.Visible = false;
        else
            ddlBaseType.Visible = true;
        ddlBaseType.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlOwnedBy.DataSource = OSAESql.RunSQL("SELECT object_name as Text, object_name as Value FROM osae_v_object where object_type_owner = 1"); ;
        ddlOwnedBy.DataBind();
        if (ddlOwnedBy.Items.Count == 0)
            ddlOwnedBy.Visible = false;
        else
            ddlOwnedBy.Visible = true;
        ddlOwnedBy.Items.Insert(0, new ListItem(String.Empty, String.Empty));

    }

    private void loadObjectTypes()
    {
        gvObjectTypes.DataSource = OSAESql.RunSQL("SELECT base_type, object_type, object_type_description FROM osae_v_object_type ORDER BY base_type");
        gvObjectTypes.DataBind();
    }
    private void loadProperties()
    {
        gvProperties.DataSource = OSAESql.RunSQL("SELECT property_name, property_datatype, property_default, track_history, property_id FROM osae_v_object_type_property where object_type='" + hdnSelectedObjectName.Text + "'");
        gvProperties.DataBind();
    }

    private void loadStates()
    {
        gvStates.DataSource = OSAESql.RunSQL("SELECT state_name, state_label FROM osae_v_object_type_state where object_type='" + hdnSelectedObjectName.Text + "'");
        gvStates.DataBind();
    }

    private void loadMethods()
    {
        gvMethods.DataSource = OSAESql.RunSQL("SELECT method_name, method_label, param_1_label, param_2_label, param_1_default, param_2_default FROM osae_v_object_type_method where object_type='" + hdnSelectedObjectName.Text + "'");
        gvMethods.DataBind();
    }

    private void loadEvents()
    {
        gvEvents.DataSource = OSAESql.RunSQL("SELECT event_name, event_label FROM osae_v_object_type_event where object_type='" + hdnSelectedObjectName.Text + "'");
        gvEvents.DataBind();
    }

    private void loadDetails()
    {
        OSAEObjectType type = OSAEObjectTypeManager.ObjectTypeLoad(gvObjectTypes.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["object_type"].ToString());
        txtName.Text = type.Name;
        txtDescr.Text = type.Description;
        ddlOwnedBy.SelectedValue = type.OwnedBy;
        ddlBaseType.SelectedValue = type.BaseType;
        chkContainer.Checked = type.Container;
        chkHideEvents.Checked = type.HideRedundant;
        chkOwner.Checked = type.Owner;
        chkSysType.Checked = type.SysType;
    }

    protected void btnStateSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeStateUpdate(hdnSelectedStateName.Text, txtStateName.Text, txtStateLabel.Text, hdnSelectedObjectName.Text);
        hdnSelectedStateName.Text = txtStateName.Text;
        loadStates();
    }

    protected void btnStateAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeStateAdd(txtStateName.Text, txtStateLabel.Text, hdnSelectedObjectName.Text);
        loadStates();
    }

    protected void btnStateDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeStateDelete(txtStateName.Text, hdnSelectedObjectName.Text);
        txtStateName.Text = "";
        txtStateLabel.Text = "";
        loadStates();
        int selectedRow = Int32.Parse(hdnSelectedStateRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedStateRow.Text = "";
        else
            hdnSelectedStateRow.Text = selectedRow.ToString();
    }

    protected void btnPropSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypePropertyUpdate(hdnSelectedPropName.Text, txtPropName.Text, ddlPropType.SelectedValue, txtPropDefault.Text, hdnSelectedObjectName.Text, chkTrackChanges.Checked);
        hdnSelectedPropName.Text = txtPropName.Text;
        loadProperties();
    }



    protected void btnMethodSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeMethodUpdate(hdnSelectedMethodName.Text, txtMethodName.Text, txtMethodLabel.Text, hdnSelectedObjectName.Text, txtParam1Label.Text, txtParam2Label.Text, txtParam1Default.Text, txtParam2Default.Text);
        hdnSelectedMethodName.Text = txtMethodName.Text;
        loadMethods();
    }

    protected void btnEventSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeEventUpdate(hdnSelectedEventName.Text, txtEventName.Text, txtEventLabel.Text, hdnSelectedObjectName.Text);
        hdnSelectedEventName.Text = txtEventName.Text;
        loadEvents();
    }

    protected void btnPropAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypePropertyAdd(txtPropName.Text, ddlPropType.SelectedValue, txtPropDefault.Text, hdnSelectedObjectName.Text, chkTrackChanges.Checked);
    }



    protected void btnMethodAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeMethodAdd(txtMethodName.Text, txtMethodLabel.Text, hdnSelectedObjectName.Text, txtParam1Label.Text, txtParam2Label.Text, txtParam1Default.Text, txtParam2Default.Text);
        loadMethods();
    }

    protected void btnEventAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeEventAdd(txtEventName.Text, txtEventLabel.Text, hdnSelectedObjectName.Text);
        loadEvents();
    }

    protected void btnPropDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypePropertyDelete(txtPropName.Text, hdnSelectedObjectName.Text);
        txtPropName.Text = "";
        txtPropDefault.Text = "";
        loadProperties();
        int selectedRow = Int32.Parse(hdnSelectedPropRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedPropRow.Text = "";
        else
            hdnSelectedPropRow.Text = selectedRow.ToString();
    }

    protected void btnMethodDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeMethodDelete(txtMethodName.Text, hdnSelectedObjectName.Text);
        txtMethodName.Text = "";
        txtMethodLabel.Text = "";
        txtParam1Label.Text = "";
        txtParam2Label.Text = "";
        txtParam1Default.Text = "";
        txtParam2Default.Text = "";
        loadMethods();
        int selectedRow = Int32.Parse(hdnSelectedMethodRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedMethodRow.Text = "";
        else
            hdnSelectedMethodRow.Text = selectedRow.ToString();
    }

    protected void btnEventDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeEventDelete(txtEventName.Text, hdnSelectedObjectName.Text);
        txtEventName.Text = "";
        txtEventLabel.Text = "";
        loadEvents();
        int selectedRow = Int32.Parse(hdnSelectedEventRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedEventRow.Text = "";
        else
            hdnSelectedEventRow.Text = selectedRow.ToString();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int owner = 0, container = 0, systype = 0, hideevents = 0;
        if (chkOwner.Checked)
            owner = 1;
        if (chkContainer.Checked)
            container = 1;
        if (chkSysType.Checked)
            systype = 1;
        if (chkHideEvents.Checked)
            hideevents = 1;

        OSAEObjectTypeManager.ObjectTypeAdd(txtName.Text, txtDescr.Text, ddlOwnedBy.SelectedValue, ddlBaseType.SelectedValue, owner, systype, container, hideevents);
        txtName.Text = "";
        txtDescr.Text = "";
        chkOwner.Checked = false;
        chkContainer.Checked = false;
        chkSysType.Checked = false;
        chkHideEvents.Checked = false;
        ddlOwnedBy.SelectedValue = "";
        ddlBaseType.SelectedValue = "";
        loadObjectTypes();
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeDelete(hdnSelectedObjectName.Text);
        txtName.Text = "";
        txtDescr.Text = "";
        chkOwner.Checked = false;
        chkContainer.Checked = false;
        chkSysType.Checked = false;
        chkHideEvents.Checked = false;
        ddlOwnedBy.SelectedValue = "";
        ddlBaseType.SelectedValue = "";
        loadObjectTypes();
        int selectedRow = Int32.Parse(hdnSelectedRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedRow.Text = "";
        else
            hdnSelectedRow.Text = selectedRow.ToString();
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        int owner = 0, container = 0, systype = 0, hideevents = 0;
        if (chkOwner.Checked)
            owner = 1;
        if (chkContainer.Checked)
            container = 1;
        if (chkSysType.Checked)
            systype = 1;
        if (chkHideEvents.Checked)
            hideevents = 1;

        OSAEObjectTypeManager.ObjectTypeUpdate(hdnSelectedObjectName.Text, txtName.Text, txtDescr.Text, ddlOwnedBy.SelectedValue, ddlBaseType.SelectedValue, owner, systype, container, hideevents);
        hdnSelectedObjectName.Text = txtName.Text;
        loadObjectTypes();
    }

    private void loadPropertyOptions()
    {
        gvPropOptions.DataSource = OSAESql.RunSQL("SELECT option_name FROM osae_object_type_property_option WHERE property_id = " + gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_id"].ToString());
        gvPropOptions.DataBind();
    }

    protected void btnOptionsItemSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypePropertyOptionAdd(hdnSelectedObjectName.Text, hdnSelectedPropName.Text, txtOptionsItem.Text);
        hdnEditingPropOptions.Value = "1";
        loadPropertyOptions();
    }

    protected void btnOptionsItemDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypePropertyOptionDelete(hdnSelectedObjectName.Text, hdnSelectedPropName.Text, hdnPropOptionsItemName.Value);
        hdnEditingPropOptions.Value = "1";
        loadPropertyOptions();
    }
}