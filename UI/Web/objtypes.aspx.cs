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
            hdnSelectedStateRow.Text = "";
            hdnSelectedMethodRow.Text = "";
            hdnSelectedEventRow.Text = "";
            hdnSelectedObjectName.Text = gvObjectTypes.DataKeys[gvObjectTypes.SelectedIndex]["object_type"].ToString();
            loadDDLs();
            loadProperties();
            hdnSelectedPropDataType.Text = "";
            //panelPropForm.Visible = true;
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
            txtPropertyTooltip.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_tooltip"].ToString();
            chkTrackChanges.Checked = (bool)gvProperties.DataKeys[gvProperties.SelectedIndex]["track_history"];
            chkRequired.Checked = (bool)gvProperties.DataKeys[gvProperties.SelectedIndex]["property_required"];
            ddlPropType.SelectedValue = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString();
            loadPropertyOptions();
        }
        else if (args[0] == "gvStates")
        {
            gvStates.SelectedIndex = Int32.Parse(args[1]);
            hdnSelectedStateRow.Text = args[1];
            pnlStateForm.Visible = true;
            hdnSelectedStateName.Text = gvStates.DataKeys[Int32.Parse(hdnSelectedStateRow.Text)]["state_name"].ToString();
            txtStateName.Text = gvStates.DataKeys[gvStates.SelectedIndex]["state_name"].ToString(); ;
            txtStateLabel.Text = gvStates.DataKeys[gvStates.SelectedIndex]["state_label"].ToString(); ;
            txtStateTooltip.Text = gvStates.DataKeys[gvStates.SelectedIndex]["state_tooltip"].ToString();
        }
        else if (args[0] == "gvMethods")
        {
            gvMethods.SelectedIndex = Int32.Parse(args[1]);
            hdnSelectedMethodRow.Text = args[1];
            pnlMethodForm.Visible = true;
            hdnSelectedMethodName.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["method_name"].ToString();
        }
        else if (args[0] == "gvEvents")
        {
            gvEvents.SelectedIndex = Int32.Parse(args[1]);
            hdnSelectedEventRow.Text = args[1];
            pnlEventForm.Visible = true;
            hdnSelectedEventName.Text = gvEvents.DataKeys[Int32.Parse(hdnSelectedEventRow.Text)]["event_name"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx?ReturnUrl=objtypes.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("ObjectType Trust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet) Response.Redirect("~/permissionError.aspx");
        SetSessionTimeout();
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

        if (sortExp != string.Empty) myDataView.Sort = string.Format("{0} {1}", sortExp, sortDir);

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
            txtStateName.Text = gvStates.DataKeys[Int32.Parse(hdnSelectedStateRow.Text)]["state_name"].ToString();//hdnSelectedStateName.Text;
            txtStateLabel.Text = gvStates.DataKeys[Int32.Parse(hdnSelectedStateRow.Text)]["state_label"].ToString();
            txtStateTooltip.Text = gvStates.DataKeys[Int32.Parse(hdnSelectedStateRow.Text)]["state_tooltip"].ToString();
        }
        if (hdnSelectedMethodRow.Text != "")
        {
            txtMethodName.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["method_name"].ToString();//hdnSelectedMethodName.Text;
            txtMethodLabel.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["method_label"].ToString();
            txtParam1Label.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_1_label"].ToString();
            txtParam2Label.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_2_label"].ToString();
            txtParam1Default.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_1_default"].ToString();
            txtParam2Default.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_2_default"].ToString();
            txtMethodTooltip.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["method_tooltip"].ToString();
        }
        if (hdnSelectedEventRow.Text != "")
        {
            txtEventName.Text = gvEvents.DataKeys[Int32.Parse(hdnSelectedEventRow.Text)]["event_name"].ToString();//hdnSelectedEventName.Text;
            txtEventLabel.Text = gvEvents.DataKeys[Int32.Parse(hdnSelectedEventRow.Text)]["event_label"].ToString();
            txtEventTooltip.Text = gvEvents.DataKeys[Int32.Parse(hdnSelectedEventRow.Text)]["event_tooltip"].ToString();
        }

        if (hdnSelectedObjectName.Text != "")
            lblExportScript.Text = OSAEObjectTypeManager.ObjectTypeExport(hdnSelectedObjectName.Text);
    }

    protected void gvObjectTypes_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvObjectTypes_" + e.Row.RowIndex.ToString()));
    }

    protected void gvProperties_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvProperties_" + e.Row.RowIndex.ToString()));
    }

    protected void gvStates_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvStates_" + e.Row.RowIndex.ToString()));
    }

    protected void gvMethods_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvMethods_" + e.Row.RowIndex.ToString()));
    }

    protected void gvEvents_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvEvents_" + e.Row.RowIndex.ToString()));
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
        DataSet baseTypeData = OSAESql.RunSQL("SELECT object_type as Text, object_type as Value, object_type_tooltip as Tooltip FROM osae_object_type ORDER BY object_type");
        ddlBaseType.DataSource = baseTypeData;
        ddlBaseType.DataBind();
        for (int i = 0; i < ddlBaseType.Items.Count; i++)
        {
            ddlBaseType.Items[i].Attributes.Add("title", baseTypeData.Tables[0].Rows[i]["Tooltip"].ToString());
        }
        if (ddlBaseType.Items.Count == 0) ddlBaseType.Visible = false;
        else ddlBaseType.Visible = true;

        ddlBaseType.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlBaseType2.DataSource = OSAESql.RunSQL("SELECT object_type as Text, object_type as Value FROM osae_object_type ORDER BY object_type"); ;
        ddlBaseType2.DataBind();

        ddlBaseType2.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlOwnedBy.DataSource = OSAESql.RunSQL("SELECT object_name as Text, object_name as Value FROM osae_v_object where object_type_owner = 1 ORDER BY object_name"); ;
        ddlOwnedBy.DataBind();
        if (ddlOwnedBy.Items.Count == 0) ddlOwnedBy.Visible = false;
        else ddlOwnedBy.Visible = true;

        ddlOwnedBy.Items.Insert(0, new ListItem(String.Empty, String.Empty));
    }

    private void loadObjectTypes()
    {
        gvObjectTypes.DataSource = OSAESql.RunSQL("SELECT base_type, object_type, object_type_description FROM osae_v_object_type ORDER BY base_type, object_type");
        gvObjectTypes.DataBind();
    }

    private void loadProperties()
    {
        txtPropName.Text = "";
        txtPropertyTooltip.Text = "";
        ddlPropType.ClearSelection();
        gvProperties.DataSource = OSAESql.RunSQL("SELECT property_name, property_datatype, property_object_type, property_default, track_history, property_required, property_tooltip, property_id FROM osae_v_object_type_property where object_type='" + hdnSelectedObjectName.Text + "' ORDER BY property_name");
        gvProperties.DataBind();
        gvProperties.SelectedIndex = 0;
        try
        {
           
            txtPropName.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString();
            txtPropDefault.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_default"].ToString();
            chkTrackChanges.Checked = (bool)gvProperties.DataKeys[gvProperties.SelectedIndex]["track_history"];
            chkRequired.Checked = (bool)gvProperties.DataKeys[gvProperties.SelectedIndex]["property_required"];
            ddlPropType.SelectedValue = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString();
            txtPropertyTooltip.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_tooltip"].ToString();
        }
        catch (Exception ex) { }
    }

    private void loadStates()
    {
        txtStateName.Text = "";
        txtStateLabel.Text = "";
        txtStateTooltip.Text = "";
        gvStates.DataSource = OSAESql.RunSQL("SELECT state_name, state_label, state_tooltip FROM osae_v_object_type_state where object_type='" + hdnSelectedObjectName.Text + "' ORDER BY state_name");
        gvStates.DataBind();
        if (gvStates.Rows.Count > 0)
        {
            gvStates.SelectedIndex = 0;
            try
            {
                hdnSelectedStateRow.Text = gvStates.SelectedIndex.ToString();
                hdnSelectedStateName.Text = gvStates.DataKeys[gvStates.SelectedIndex]["state_name"].ToString();
                txtStateName.Text = gvStates.DataKeys[gvStates.SelectedIndex]["state_name"].ToString();
                txtStateLabel.Text = gvStates.DataKeys[gvStates.SelectedIndex]["state_label"].ToString();
                txtStateTooltip.Text = gvStates.DataKeys[gvStates.SelectedIndex]["state_tooltip"].ToString();
            }
            catch (Exception ex) { }
        }
    }

    private void loadMethods()
    {
        txtMethodName.Text = "";
        txtMethodLabel.Text = "";
        txtMethodTooltip.Text = "";
        gvMethods.DataSource = OSAESql.RunSQL("SELECT method_name, method_label, param_1_label, param_2_label, param_1_default, param_2_default, method_tooltip FROM osae_v_object_type_method where object_type='" + hdnSelectedObjectName.Text + "' ORDER BY method_name");
        gvMethods.DataBind();
        if (gvMethods.Rows.Count > 0)
        {
            gvMethods.SelectedIndex = 0;
            try
            {
                hdnSelectedMethodRow.Text = gvMethods.SelectedIndex.ToString();
                hdnSelectedMethodName.Text = gvMethods.DataKeys[gvMethods.SelectedIndex]["method_name"].ToString();
                txtMethodName.Text = gvMethods.DataKeys[gvMethods.SelectedIndex]["method_name"].ToString();
                txtMethodLabel.Text = gvMethods.DataKeys[gvMethods.SelectedIndex]["method_label"].ToString();
                txtParam1Label.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_1_label"].ToString();
                txtParam2Label.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_2_label"].ToString();
                txtParam1Default.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_1_default"].ToString();
                txtParam2Default.Text = gvMethods.DataKeys[Int32.Parse(hdnSelectedMethodRow.Text)]["param_2_default"].ToString();
                txtMethodTooltip.Text = gvMethods.DataKeys[gvMethods.SelectedIndex]["method_tooltip"].ToString();
            }
            catch (Exception ex) { }
        }
    }

    private void loadEvents()
    {
        txtEventName.Text = "";
        txtEventLabel.Text = "";
        txtEventTooltip.Text = "";
        gvEvents.DataSource = OSAESql.RunSQL("SELECT event_name, event_label, event_tooltip FROM osae_v_object_type_event where object_type='" + hdnSelectedObjectName.Text + "' ORDER BY event_name");
        gvEvents.DataBind();
        if (gvEvents.Rows.Count > 0)
        {
            gvEvents.SelectedIndex = 0;
            try
            {
                hdnSelectedEventName.Text = gvEvents.DataKeys[gvEvents.SelectedIndex]["event_name"].ToString();
                hdnSelectedEventRow.Text = gvEvents.SelectedIndex.ToString();
                txtEventName.Text = gvEvents.DataKeys[gvEvents.SelectedIndex]["event_name"].ToString();
                txtEventLabel.Text = gvEvents.DataKeys[gvEvents.SelectedIndex]["event_label"].ToString();
                txtEventTooltip.Text = gvEvents.DataKeys[gvEvents.SelectedIndex]["event_tooltip"].ToString();
            }
            catch (Exception ex) { }
        }
    }

    private void loadDetails()
    {
        OSAEObjectType type = OSAEObjectTypeManager.ObjectTypeLoad(gvObjectTypes.DataKeys[gvObjectTypes.SelectedIndex]["object_type"].ToString());
        txtName.Text = type.Name;
        txtDescr.Text = type.Description;
        txtObjectTypeTooltip.Text = type.Tooltip;
        ddlOwnedBy.SelectedIndex = 0;
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
        OSAEObjectTypeManager.ObjectTypeStateUpdate(hdnSelectedStateName.Text, txtStateName.Text, txtStateLabel.Text, hdnSelectedObjectName.Text, txtStateTooltip.Text);
        loadStates();
    }

    protected void btnStateAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeStateAdd(hdnSelectedObjectName.Text, txtStateName.Text, txtStateLabel.Text, txtStateTooltip.Text);
        loadStates();
    }

    protected void btnStateDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeStateDelete(txtStateName.Text, hdnSelectedObjectName.Text);
        loadStates();
        int selectedRow = Int32.Parse(hdnSelectedStateRow.Text) - 1;
        if (selectedRow < 0) hdnSelectedStateRow.Text = "";
        else hdnSelectedStateRow.Text = selectedRow.ToString();
    }

    protected void btnPropSave_Click(object sender, EventArgs e)
    {
        if (ddlPropType.SelectedValue == "Object Type" & ddlBaseType2.SelectedValue == "")
            Response.Write("<script>alert('You must select an Object Type!');</script>");
        else
        {
            OSAEObjectTypeManager.ObjectTypePropertyUpdate(gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString(), txtPropName.Text, ddlPropType.SelectedValue, ddlBaseType2.SelectedValue, txtPropDefault.Text, hdnSelectedObjectName.Text, chkTrackChanges.Checked, chkRequired.Checked, txtPropertyTooltip.Text);
            loadProperties();
        }
    }

    protected void btnMethodSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeMethodUpdate(hdnSelectedMethodName.Text, txtMethodName.Text, txtMethodLabel.Text, hdnSelectedObjectName.Text, txtParam1Label.Text, txtParam2Label.Text, txtParam1Default.Text, txtParam2Default.Text, txtMethodTooltip.Text);
        loadMethods();
    }

    protected void btnEventSave_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeEventUpdate(hdnSelectedEventName.Text, txtEventName.Text, txtEventLabel.Text, hdnSelectedObjectName.Text, txtEventTooltip.Text);
        //hdnSelectedEventName.Text = txtEventName.Text;
        loadEvents(); 
    }

    protected void btnPropAdd_Click(object sender, EventArgs e)
    {
        if (ddlPropType.SelectedValue == "Object Type" & ddlBaseType2.SelectedValue == "")
            Response.Write("<script>alert('You must select an Object Type!');</script>");
        else
        {
            OSAEObjectTypeManager.ObjectTypePropertyAdd(hdnSelectedObjectName.Text, txtPropName.Text, ddlPropType.SelectedValue, ddlBaseType2.SelectedItem.ToString(), txtPropDefault.Text, chkTrackChanges.Checked, chkRequired.Checked, txtPropertyTooltip.Text);
            loadProperties();
        }
    }

    protected void btnMethodAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeMethodAdd(hdnSelectedObjectName.Text, txtMethodName.Text, txtMethodLabel.Text, txtParam1Label.Text, txtParam2Label.Text, txtParam1Default.Text, txtParam2Default.Text, txtMethodTooltip.Text);
        loadMethods();
    }

    protected void btnEventAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeEventAdd(hdnSelectedObjectName.Text, txtEventName.Text, txtEventLabel.Text, txtEventTooltip.Text);
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
        loadMethods();
        int selectedRow = Int32.Parse(hdnSelectedMethodRow.Text) - 1;
        if (selectedRow < 0) hdnSelectedMethodRow.Text = "";
        else hdnSelectedMethodRow.Text = selectedRow.ToString();
    }

    protected void btnEventDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeEventDelete(txtEventName.Text, hdnSelectedObjectName.Text);
        txtEventName.Text = "";
        txtEventLabel.Text = "";
        loadEvents();
        int selectedRow = Int32.Parse(hdnSelectedEventRow.Text) - 1;
        if (selectedRow < 0) hdnSelectedEventRow.Text = "";
        else hdnSelectedEventRow.Text = selectedRow.ToString();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectTypeManager.ObjectTypeClone(txtName.Text, hdnSelectedObjectName.Text);
        txtName.Text = "";
        txtDescr.Text = "";
        txtObjectTypeTooltip.Text = "";
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
        OSAEObjectTypeManager.ObjectTypeUpdate(hdnSelectedObjectName.Text, txtName.Text, txtDescr.Text, ddlOwnedBy.SelectedValue, ddlBaseType.SelectedValue, chkOwner.Checked, chkSysType.Checked, chkContainer.Checked, chkHideEvents.Checked, txtObjectTypeTooltip.Text);
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
            if (ViewState["sortOrder"].ToString() == "desc") ViewState["sortOrder"] = "asc";
            else ViewState["sortOrder"] = "desc";

            return ViewState["sortOrder"].ToString();
        }
        set { ViewState["sortOrder"] = value; }

    }
    protected void ddlPropType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPropType.SelectedItem.Value == "Integer") txtPropDefault.Text = "0";
        else txtPropDefault.Text = "";

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
    }
    #endregion

    #region Export
    protected void btnExport_Click(object sender, EventArgs e)
    {
        string objName = hdnSelectedObjectName.Text;
        Response.Redirect(@"~/importexport.aspx?eType=ObjectType&eObject=" + objName);
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
