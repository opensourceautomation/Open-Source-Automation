using System;
using System.Web.UI.WebControls;
using System.Data;
using OSAE;

public partial class objtypes : System.Web.UI.Page
{
    OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvObjectTypes")
        {
            gvObjectTypes.SelectedIndex = Int32.Parse(args[1]);
            panelEditForm.Visible = true;
            gvProperties.SelectedIndex = 0;
            hdnSelectedStateRow.Text = "";
            hdnSelectedMethodRow.Text = "";
            hdnSelectedEventRow.Text = "";
            hdnSelectedObjectName.Text = gvObjectTypes.DataKeys[gvObjectTypes.SelectedIndex]["object_type"].ToString();
            loadDDLs();
            loadProperties();
            hdnSelectedPropDataType.Text = "";
            panelPropForm.Visible = true;
            try
            {
                txtPropName.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString();
                txtPropDefault.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_default"].ToString();
                chkTrackChanges.Checked = (bool)gvProperties.DataKeys[gvProperties.SelectedIndex]["track_history"];
                ddlPropType.SelectedValue = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString();
            }
            catch { }
            loadStates();
            loadMethods();
            loadEvents();
            loadDetails();
        }
        else if (args[0] == "gvProperties")
        {
            gvProperties.SelectedIndex = Int32.Parse(args[1]);
            hdnSelectedPropDataType.Text = "";
            panelPropForm.Visible = true;
            txtPropName.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString();
            txtPropDefault.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_default"].ToString();
            chkTrackChanges.Checked = (bool)gvProperties.DataKeys[gvProperties.SelectedIndex]["track_history"];
            ddlPropType.SelectedValue = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString();
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
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("ObjectType Trust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet) Response.Redirect("~/permissionError.aspx");

        if (!IsPostBack)
        {
            ViewState["sortOrder"] = "";
            bindGridView("", "");
            panelEditForm.Visible = true;
            hdnSelectedStateRow.Text = "";
            hdnSelectedMethodRow.Text = "";
            hdnSelectedEventRow.Text = "";
            hdnSelectedObjectName.Text = gvObjectTypes.DataKeys[gvObjectTypes.SelectedIndex]["object_type"].ToString();
            loadDDLs();
            loadProperties();
            loadStates();
            loadMethods();
            loadEvents();
            loadDetails();
        }
    }

    public void bindGridView(string sortExp, string sortDir)
    {
        DataSet myDataSet = OSAESql.RunSQL("SELECT base_type, object_type, object_type_description FROM osae_v_object_type ORDER BY base_type, object_type");
        DataView myDataView = new DataView();
        myDataView = myDataSet.Tables[0].DefaultView;

        if (sortExp != string.Empty)
            myDataView.Sort = string.Format("{0} {1}", sortExp, sortDir);

        gvObjectTypes.DataSource = myDataView;
        gvObjectTypes.DataBind();
        if (!this.IsPostBack) loadDDLs();

        loadProperties();
        if (hdnSelectedStateRow.Text != "") loadStates();
        if (hdnSelectedMethodRow.Text != "") loadMethods();
        if (hdnSelectedEventRow.Text != "") loadEvents();
        if (!this.IsPostBack) loadDDLs();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (ddlPropType.SelectedItem.ToString() == "Object Type")
        {
            ddlBaseType2.SelectedValue = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_object_type"].ToString();
            ddlBaseType2.Visible = true;
            lblPropObjectType.Visible = true;
        }
        else
        {
            ddlBaseType2.SelectedValue = "";
            ddlBaseType2.Visible = false;
            lblPropObjectType.Visible = false;
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

        if (hdnSelectedObjectName.Text != "")
            lblExportScript.Text = OSAEObjectTypeManager.ObjectTypeExport(hdnSelectedObjectName.Text);

        // lblExportScript.Text = OSAEObjectTypeManager.ObjectTypeExport(hdnSelectedObjectName.Text).Replace(";",";" + Environment.NewLine);
        // lblExportScript.Text = "Line 1 " + Environment.NewLine + "Line 2";
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
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvObjectTypes_" + e.Row.RowIndex.ToString()));
    }

    protected void gvProperties_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvProperties_" + e.Row.RowIndex.ToString()));
        }
    }

    protected void gvStates_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='Yellow';");
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
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='Yellow';");
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
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='Yellow';");
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
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='Yellow';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");

            e.Row.Attributes.Add("onclick", "selectPropOptionsItem('" + gvPropOptions.DataKeys[e.Row.RowIndex]["option_name"].ToString() + "', this);");
        }
    }

    private void loadDDLs()
    {
        ddlBaseType.DataSource = OSAESql.RunSQL("SELECT object_type as Text, object_type as Value FROM osae_object_type ORDER BY object_type"); ;
        ddlBaseType.DataBind();
        if (ddlBaseType.Items.Count == 0)
            ddlBaseType.Visible = false;
        else
            ddlBaseType.Visible = true;

        ddlBaseType.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlBaseType2.DataSource = OSAESql.RunSQL("SELECT object_type as Text, object_type as Value FROM osae_object_type ORDER BY object_type"); ;
        ddlBaseType2.DataBind();

        ddlBaseType2.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlOwnedBy.DataSource = OSAESql.RunSQL("SELECT object_name as Text, object_name as Value FROM osae_v_object where object_type_owner = 1 ORDER BY object_name"); ;
        ddlOwnedBy.DataBind();
        if (ddlOwnedBy.Items.Count == 0)
            ddlOwnedBy.Visible = false;
        else
            ddlOwnedBy.Visible = true;

        ddlOwnedBy.Items.Insert(0, new ListItem(String.Empty, String.Empty));
    }

    private void loadObjectTypes()
    {
        gvObjectTypes.DataSource = OSAESql.RunSQL("SELECT base_type, object_type, object_type_description FROM osae_v_object_type ORDER BY base_type, object_type");
        gvObjectTypes.DataBind();
    }

    private void loadProperties()
    {
        gvProperties.DataSource = OSAESql.RunSQL("SELECT property_name, property_datatype, property_object_type, property_default, track_history, property_id FROM osae_v_object_type_property where object_type='" + hdnSelectedObjectName.Text + "' ORDER BY property_name");
        gvProperties.DataBind();
    }

    private void loadStates()
    {
        gvStates.DataSource = OSAESql.RunSQL("SELECT state_name, state_label FROM osae_v_object_type_state where object_type='" + hdnSelectedObjectName.Text + "' ORDER BY state_name");
        gvStates.DataBind();
    }

    private void loadMethods()
    {
        gvMethods.DataSource = OSAESql.RunSQL("SELECT method_name, method_label, param_1_label, param_2_label, param_1_default, param_2_default FROM osae_v_object_type_method where object_type='" + hdnSelectedObjectName.Text + "' ORDER BY method_name");
        gvMethods.DataBind();
    }

    private void loadEvents()
    {
        gvEvents.DataSource = OSAESql.RunSQL("SELECT event_name, event_label FROM osae_v_object_type_event where object_type='" + hdnSelectedObjectName.Text + "' ORDER BY event_name");
        gvEvents.DataBind();
    }

    private void loadDetails()
    {
        OSAEObjectType type = OSAEObjectTypeManager.ObjectTypeLoad(gvObjectTypes.DataKeys[gvObjectTypes.SelectedIndex]["object_type"].ToString());
        txtName.Text = type.Name;
        txtDescr.Text = type.Description;
        try
        { ddlOwnedBy.SelectedValue = type.OwnedBy; }
        catch
        { }
        ddlBaseType.SelectedValue = type.BaseType;
        chkContainer.Checked = type.Container;
        chkHideEvents.Checked = type.HideRedundant;
        chkOwner.Checked = type.Owner;
        chkSysType.Checked = type.SysType;
        applyObjectSecurity(type.Name);
    }

    protected void btnStateSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeStateUpdate(hdnSelectedStateName.Text, txtStateName.Text, txtStateLabel.Text, hdnSelectedObjectName.Text);
        hdnSelectedStateName.Text = txtStateName.Text;
        loadStates();
    }

    protected void btnStateAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeStateAdd(hdnSelectedObjectName.Text, txtStateName.Text, txtStateLabel.Text);
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
        if (ddlPropType.SelectedValue == "Object Type" & ddlBaseType2.SelectedValue == "")
        {
            Response.Write("<script>alert('You must select an Object Type!');</script>");
        }
        else
        {
            OSAEObjectTypeManager.ObjectTypePropertyUpdate(gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString(), txtPropName.Text, ddlPropType.SelectedValue, ddlBaseType2.SelectedValue, txtPropDefault.Text, hdnSelectedObjectName.Text, chkTrackChanges.Checked);
            loadProperties();
        }
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
        if (ddlPropType.SelectedValue == "Object Type" && ddlBaseType2.SelectedValue != "")
        {
            OSAEObjectTypeManager.ObjectTypePropertyAdd(hdnSelectedObjectName.Text, txtPropName.Text, ddlPropType.SelectedValue, ddlBaseType2.SelectedItem.ToString(), txtPropDefault.Text, chkTrackChanges.Checked);
            loadProperties();
        }
        else
        {
            Response.Write("<script>alert('You must select an Object Type!');</script>");
        }
    }

    protected void btnMethodAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeMethodAdd(hdnSelectedObjectName.Text, txtMethodName.Text, txtMethodLabel.Text, txtParam1Label.Text, txtParam2Label.Text, txtParam1Default.Text, txtParam2Default.Text);
        loadMethods();
    }

    protected void btnEventAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeEventAdd(hdnSelectedObjectName.Text, txtEventName.Text, txtEventLabel.Text);
        loadEvents();
    }

    protected void btnPropDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypePropertyDelete(txtPropName.Text, hdnSelectedObjectName.Text);
        txtPropName.Text = "";
        txtPropDefault.Text = "";
        loadProperties();
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
        OSAEObjectTypeManager.ObjectTypeClone(txtName.Text, hdnSelectedObjectName.Text);
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
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeUpdate(hdnSelectedObjectName.Text, txtName.Text, txtDescr.Text, ddlOwnedBy.SelectedValue, ddlBaseType.SelectedValue, chkOwner.Checked, chkSysType.Checked, chkContainer.Checked, chkHideEvents.Checked);
        hdnSelectedObjectName.Text = txtName.Text;
        loadObjectTypes();
    }

    private void loadPropertyOptions()
    {
        gvPropOptions.DataSource = OSAESql.RunSQL("SELECT option_name FROM osae_object_type_property_option WHERE property_id = " + gvProperties.DataKeys[gvProperties.SelectedIndex]["property_id"].ToString() + " ORDER BY option_name");
        gvPropOptions.DataBind();
    }

    protected void btnOptionsItemSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypePropertyOptionAdd(hdnSelectedObjectName.Text, gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString(), txtOptionsItem.Text);
        hdnEditingPropOptions.Value = "1";
        loadPropertyOptions();
    }

    protected void btnOptionsItemDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypePropertyOptionDelete(hdnSelectedObjectName.Text, gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString(), hdnPropOptionsItemName.Value);
        hdnEditingPropOptions.Value = "1";
        loadPropertyOptions();
    }

    protected void gvObjectTypes_OnSorting(object sender, GridViewSortEventArgs e)
    {
        bindGridView(e.SortExpression, sortOrder);
    }

    public string sortOrder
    {
        get
        {
            if (ViewState["sortOrder"].ToString() == "desc")
                ViewState["sortOrder"] = "asc";
            else
                ViewState["sortOrder"] = "desc";

            return ViewState["sortOrder"].ToString();
        }
        set
        {
            ViewState["sortOrder"] = value;
        }
    }
    protected void ddlPropType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPropType.SelectedItem.Value == "Integer")
            txtPropDefault.Text = "0";
        else
            txtPropDefault.Text = "";

        //string s = ddlBaseType2.SelectedItem.Text.ToUpper();
        hdnSelectedPropDataType.Text = ddlPropType.SelectedValue.ToString();
    }

    #region Trust Settings
    protected void applyObjectSecurity(string objName)
    {
        int sessTrust = Convert.ToInt32(Session["TrustLevel"].ToString());
        txtName.Enabled = false;
        txtDescr.Enabled = false;
        ddlOwnedBy.Enabled = false;
        ddlBaseType.Enabled = false;
        ddlBaseType2.Enabled = false;
        chkOwner.Enabled = false;
        chkContainer.Enabled = false;
        chkHideEvents.Enabled = false;
        chkSysType.Enabled = false;
        btnAdd.Enabled = false;
        btnUpdate.Enabled = false;
        btnDelete.Enabled = false;
        btnPropAdd.Enabled = false;
        btnPropDelete.Enabled = false;
        btnPropSave.Enabled = false;
        btnEventAdd.Enabled = false;
        btnEventDelete.Enabled = false;
        btnEventSave.Enabled = false;
        btnMethodAdd.Enabled = false;
        btnMethodDelete.Enabled = false;
        btnMethodSave.Enabled = false;
        btnStateAdd.Enabled = false;
        btnStateDelete.Enabled = false;
        btnStateSave.Enabled = false;
        btnEditPropOptions.Enabled = false;
        chkTrackChanges.Enabled = false;
        if (sessTrust >= adSet.ObjectTypeAddTrust)
        {
            txtName.Enabled = true;
            txtDescr.Enabled = true;
            ddlOwnedBy.Enabled = true;
            ddlBaseType.Enabled = true;
            ddlBaseType2.Enabled = true;
            chkOwner.Enabled = true;
            chkContainer.Enabled = true;
            chkHideEvents.Enabled = true;
            chkSysType.Enabled = true;
            btnAdd.Enabled = true;
            btnStateAdd.Enabled = true;
            btnMethodAdd.Enabled = true;
            btnEventAdd.Enabled = true;
            btnPropAdd.Enabled = true;
        }
        if (sessTrust >= adSet.ObjectTypeUpdateTrust)
        {
            txtName.Enabled = true;
            txtDescr.Enabled = true;
            ddlOwnedBy.Enabled = true;
            ddlBaseType.Enabled = true;
            ddlBaseType2.Enabled = true;
            chkOwner.Enabled = true;
            chkContainer.Enabled = true;
            chkHideEvents.Enabled = true;
            chkSysType.Enabled = true;
            btnUpdate.Enabled = true;
            btnStateSave.Enabled = true;
            btnMethodSave.Enabled = true;
            btnEventSave.Enabled = true;
            btnPropSave.Enabled = true;
            btnEditPropOptions.Enabled = true;
            chkTrackChanges.Enabled = true;
        }
        if (sessTrust >= adSet.ObjectTypeDeleteTrust)
        {
            btnDelete.Enabled = true;
            btnStateDelete.Enabled = true;
            btnMethodDelete.Enabled = true;
            btnEventDelete.Enabled = true;
            btnPropDelete.Enabled = true;
        }
        #endregion
    }
}
