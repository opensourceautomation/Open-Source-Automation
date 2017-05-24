using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using OSAE;

public partial class home : System.Web.UI.Page
{
    string objectSQL = "";
    string groupByVal = "";
    Boolean objPropMissing;
    Boolean objPropError;
    OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvObjects")
        {
            alert.Visible = false;
            objAddError.Visible = false;
            objAddErrorMsg.Visible = false;
            gvObjects.SelectedIndex = Int32.Parse(args[1]);
            hdnSelectedObjectName.Text = gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString();
            panelEditForm.Visible = true;
            btnUpdate.Visible = true;
            panelPropForm.Visible = false;
            divParameters.Visible = false;
            loadDDLs();
            loadProperties();
            loadDetails();
        }
        else if (args[0] == "gvProperties")
        {
            panelPropForm.Visible = true;
            propSaveError.Visible = false;
            gvProperties.SelectedIndex = Int32.Parse(args[1]);
            hdnSelectedPropName.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString();
            bool propRequired = Convert.ToBoolean(gvProperties.DataKeys[gvProperties.SelectedIndex]["property_required"].ToString());
            lblPropName.Text = hdnSelectedPropName.Text;
            lblSourceName.Text = "Source: " + gvProperties.DataKeys[gvProperties.SelectedIndex]["source_name"].ToString();
            lblTrustLevel.Text = "Trust Level: " + gvProperties.DataKeys[gvProperties.SelectedIndex]["trust_level"].ToString();
            lblInterestLevel.Text = "Interest Level: " + gvProperties.DataKeys[gvProperties.SelectedIndex]["interest_level"].ToString();
            lblPropLastUpd.Text = "Last Updated: " + gvProperties.DataKeys[gvProperties.SelectedIndex]["last_updated"].ToString();
            hdnSelectedPropType.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString();
            lblPropType.Text = "Type: " + hdnSelectedPropType.Text;
            if (propRequired) lblRequired.Visible = true;
            else lblRequired.Visible = false;

            if (gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString() == "List")
            {
                loadPropertyList();
                txtPropValue.Visible = false;
                btnPropSave.Visible = false;
                lblPropName.Visible = false;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = false;
                lblInterestLevel.Visible = false;
                btnEditPropList.Visible = true;
                ddlPropValue.Visible = false;
            }
            else if (gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString() == "Boolean")
            {
                ddlPropValue.Items.Clear();
                ddlPropValue.Items.Add(new ListItem("TRUE", "TRUE"));
                ddlPropValue.Items.Add(new ListItem("FALSE", "FALSE"));
                if (!string.IsNullOrEmpty(gvProperties.DataKeys[gvProperties.SelectedIndex]["property_value"].ToString()) && ddlPropValue.Items.FindByValue(gvProperties.DataKeys[gvProperties.SelectedIndex]["property_value"].ToString()) != null)
                    ddlPropValue.SelectedValue = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_value"].ToString();

                txtPropValue.Visible = false;
                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                lblSourceName.Visible = true;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = true;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = true;
            }
            else if (gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString() == "Object")
            {
                ddlPropValue.Items.Clear();
                DataSet options = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE object_type NOT IN ('CONTROL','SCREEN') ORDER BY object_name");
                foreach (DataRow dr in options.Tables[0].Rows)
                    ddlPropValue.Items.Add(new ListItem(dr["object_name"].ToString()));

                txtPropValue.Visible = false;
                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                lblSourceName.Visible = true;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = true;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = true;
            }
            else if (gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString() == "Object Type")
            {
                ddlPropValue.Items.Clear();
                DataSet options = OSAESql.RunSQL("SELECT object_name FROM osae_v_object WHERE object_type ='" + gvProperties.DataKeys[gvProperties.SelectedIndex]["property_object_type"].ToString() + "' ORDER BY object_name");
                foreach (DataRow dr in options.Tables[0].Rows)
                    ddlPropValue.Items.Add(new ListItem(dr["object_name"].ToString()));

                txtPropValue.Visible = false;
                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                lblSourceName.Visible = true;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = true;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = true;
            }
            else
            {
                string propID = gvProperties.DataKeys[gvProperties.SelectedIndex]["object_property_id"].ToString();
                DataSet options = OSAESql.RunSQL("SELECT option_name FROM osae_object_type_property_option ootpo INNER JOIN osae_object_property oop ON oop.object_type_property_id = ootpo.property_id WHERE oop.object_property_id=" + propID + " ORDER BY option_name");
                ddlPropValue.Items.Clear();
                if (options.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in options.Tables[0].Rows)
                        ddlPropValue.Items.Add(new ListItem(dr["option_name"].ToString(), dr["option_name"].ToString()));

                    if (!string.IsNullOrEmpty(gvProperties.DataKeys[gvProperties.SelectedIndex]["property_value"].ToString()) && ddlPropValue.Items.FindByValue(gvProperties.DataKeys[gvProperties.SelectedIndex]["property_value"].ToString()) != null)
                        ddlPropValue.SelectedValue = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_value"].ToString();

                    txtPropValue.Visible = false;
                    ddlPropValue.Visible = true;
                }
                else
                {
                    txtPropValue.Text = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_value"].ToString();
                    txtPropValue.Visible = true;
                    ddlPropValue.Visible = false;
                }

                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                btnEditPropList.Visible = false;
            }
            if (ddlType.Text == "PERSON")
            {
                string pTrust = gvProperties.DataKeys[gvProperties.SelectedIndex]["trust_level"].ToString();
                string pName = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_name"].ToString();
                string pType = gvProperties.DataKeys[gvProperties.SelectedIndex]["property_datatype"].ToString();
                string oName = gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString();
                string oMinTrust = gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString();
                applyPersonPropertySecurity(oName, oMinTrust, pName, pType, pTrust);
            }
            if (Convert.ToInt32(Session["TrustLevel"].ToString()) < Convert.ToInt32(gvProperties.DataKeys[gvProperties.SelectedIndex]["trust_level"].ToString()))
            {
                txtPropValue.Visible = false;
                btnPropSave.Visible = false;
                lblPropName.Visible = false;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = false;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = false;
            }
        }
        else if (args[0] == "gvPropList")
        {
            txtListItem.Text = gvPropList.DataKeys[gvPropList.SelectedIndex]["item_name"].ToString();
        }
        }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
            Response.Redirect("~/Default.aspx?ReturnUrl=objects.aspx");

        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < adSet.ObjectsTrust)
            Response.Redirect("~/permissionError.aspx");

        bool hideControls = Convert.ToBoolean(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "Hide Controls").Value);
        if (hideControls)
            objectSQL = "SELECT object_id, container_name, object_name, object_type, state_label, state_name, DATE_FORMAT(last_updated,'%m/%d %h:%i:%s %p') as last_updated, address, object_type_tooltip FROM osae_v_object WHERE base_type NOT IN ('CONTROL','SCREEN','USER CONTROL') order by container_name, object_name";
        else
            objectSQL = "SELECT object_id, container_name, object_name, object_type, state_label, state_name, DATE_FORMAT(last_updated,'%m/%d %h:%i:%s %p') as last_updated, address, object_type_tooltip FROM osae_v_object order by container_name, object_name";

        if (!IsPostBack)  
        {
            ViewState["sortOrder"] = "";  
            bindGridView("","");
            alert.Visible = false;
            panelEditForm.Visible = true;
            btnUpdate.Visible = true;
            panelPropForm.Visible = false;
            divParameters.Visible = false;
            hdnSelectedObjectName.Text = gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString();
            objAddError.Visible = false;
            objAddErrorMsg.Visible = false;
            loadDDLs();
            loadProperties();
            loadDetails();
            Master.Log.Info("Object.aspx Loaded");
        }
        propSaveError.Visible = false;
    }

    public void bindGridView(string sortExp,string sortDir)  
    {
        DataSet myDataSet = OSAESql.RunSQL(objectSQL);
        DataView myDataView = new DataView();
        myDataView = myDataSet.Tables[0].DefaultView;

        if (sortExp != string.Empty) myDataView.Sort = string.Format("{0} {1}", sortExp, sortDir);

        gvObjects.DataSource = myDataView;
        gvObjects.DataBind();
        if (!IsPostBack) loadDDLs();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedObjectName.Text != "")
            txtExportScript.Text = OSAEObjectManager.ObjectExport(hdnSelectedObjectName.Text);
    }

    protected void gvObjects_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.ToolTip = gvObjects.DataKeys[e.Row.RowIndex]["object_type_tooltip"].ToString();
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvObjects_" + e.Row.RowIndex.ToString()));
        }
    }

    protected void gvObjects_OnSorting(object sender, GridViewSortEventArgs e)
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
        set  
        { ViewState["sortOrder"] = value; }  
    }  
    
    protected void gvProperties_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            OSAEObjectProperty curProp = OSAEObjectPropertyManager.GetObjectPropertyValue(hdnSelectedObjectName.Text, e.Row.Cells[0].Text);
            // Retrieve Help Text for this Property from Object Types
            e.Row.Cells[0].ToolTip = gvProperties.DataKeys[e.Row.RowIndex]["property_tooltip"].ToString();

            // Check if Property value is empty
            if (string.IsNullOrEmpty(curProp.Value))
            {
                // Check if property AllowEmpty is True/False
                try
                {
                    bool allowEmpty = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "AllowEmptyProperties").Value);
                    if (allowEmpty == false)
                    {
                        // This will highlight EMPTY properties in Yellow, and show a tooltip message
                        e.Row.Cells[1].ToolTip = "Missing Information.";
                        e.Row.Cells[1].Attributes.Add("style", "background-color: yellow;");
                        objPropMissing = true;
                    }
                }
                catch { }
                // Check if property is required
                bool propRequired = Convert.ToBoolean(gvProperties.DataKeys[e.Row.RowIndex]["property_required"].ToString());
                if (propRequired)
                {
                    e.Row.Cells[1].ToolTip = "ERROR: This property is REQUIRED and MUST have a value!";
                    e.Row.Cells[1].Attributes.Add("style", "background-color: red;");
                    objPropError = true;
                }
            }
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvProperties_" + e.Row.RowIndex.ToString()));
            if (curProp.DataType == "Password") e.Row.Cells[1].Text = new string('*', e.Row.Cells[1].Text.Length);
        }
    }

    protected void gvPropList_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");

             e.Row.Attributes.Add("onclick", "selectPropListItem('" + gvPropList.DataKeys[e.Row.RowIndex]["item_name"].ToString().Replace("'","^^^") + "', '" + gvPropList.DataKeys[e.Row.RowIndex]["item_label"].ToString().Replace("'", "^^^") + "', this);");
         //  e.Row.Attributes.Add("onclick", "selectPropListItem(" + e.Row.RowIndex.ToString() + ", this);");
        
        }
    }

    protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
    {
        OSAEObjectStateManager.ObjectStateSet(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), ddlState.SelectedItem.Value, Session["Username"].ToString());
        lblAlert.Text = "State set successfully to " + ddlState.SelectedItem.Text;
        DataSet stateData = OSAESql.RunSQL("SELECT state_label as Text, state_name as Value, state_tooltip as Tooltip FROM osae_object_type_state ts INNER JOIN osae_object o ON o.object_type_id = ts.object_type_id where object_name = '" + gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString().Replace("'", "''") + "' ORDER BY state_label");
        int i = ddlState.SelectedIndex;
        ddlState.ToolTip = stateData.Tables[0].Rows[i]["Tooltip"].ToString();
        alert.Visible = true;        
    }

    protected void ddlMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataSet ds = OSAESql.RunSQL("SELECT param_1_label, param_2_label, param_1_default, param_2_default FROM osae_v_object_type_method otm INNER JOIN osae_object oo ON oo.object_type_id = otm.object_type_id WHERE object_name = '" + gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString().Replace("'", "''") + "' AND method_name = '" + ddlMethod.SelectedItem.Value + "'");
        DataTable dt = ds.Tables[0];
        if (dt.Rows.Count > 0)
        {
            if (!string.IsNullOrEmpty(dt.Rows[0]["param_1_label"].ToString()))
            {
                divParameters.Visible = true;
                txtParam1.Text = dt.Rows[0]["param_1_default"].ToString();
                txtParam2.Text = dt.Rows[0]["param_2_default"].ToString();
                if (!string.IsNullOrEmpty(dt.Rows[0]["param_1_label"].ToString()))
                    lblParam1.Text = "(" + dt.Rows[0]["param_1_label"].ToString() + ")";
                if (!string.IsNullOrEmpty(dt.Rows[0]["param_2_label"].ToString()))
                    lblParam2.Text = "(" + dt.Rows[0]["param_2_label"].ToString() + ")";
            }
            else
            {
                OSAEMethodManager.MethodQueueAdd(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), ddlMethod.SelectedItem.Value, "", "", Session["Username"].ToString());
                lblAlert.Text = "Method successfuly executed: " + ddlMethod.SelectedItem.Text;
                alert.Visible = true;
            }
        }
        else
        {
            OSAEMethodManager.MethodQueueAdd(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), ddlMethod.SelectedItem.Value, "", "", Session["Username"].ToString());
            lblAlert.Text = "Method successfuly executed: " + ddlMethod.SelectedItem.Text;
            alert.Visible = true;
        }
    }

    protected void btnExecute_Click(object sender, EventArgs e)
    {
        OSAEMethodManager.MethodQueueAdd(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), ddlMethod.SelectedItem.Value, txtParam1.Text, txtParam2.Text, Session["Username"].ToString());
        lblAlert.Text = "Method successfuly executed: " + ddlMethod.SelectedItem.Text;
        alert.Visible = true;
        divParameters.Visible = false;
    }

    protected void ddlEvent_SelectedIndexChanged(object sender, EventArgs e)
    {
        OSAEObjectManager.EventTrigger(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), ddlEvent.SelectedItem.Value);
        lblAlert.Text = "Event set successfully to " + ddlEvent.SelectedItem.Text;
        alert.Visible = true;
    }

    private void loadDDLs()
    {
        DataSet stateData = OSAESql.RunSQL("SELECT state_label as Text, state_name as Value, state_tooltip as Tooltip FROM osae_object_type_state ts INNER JOIN osae_object o ON o.object_type_id = ts.object_type_id where object_name = '" + gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString().Replace("'", "''") + "' ORDER BY state_label");
        ddlState.DataSource = stateData;
        ddlState.DataBind();
        for(int i = 0; i < ddlState.Items.Count; i++)
        {
            ddlState.Items[i].Attributes.Add("title", stateData.Tables[0].Rows[i]["Tooltip"].ToString());
        }
        if (ddlState.Items.Count == 0) divState.Visible = false;
        else divState.Visible = true;

        DataSet methodData = OSAESql.RunSQL("SELECT method_label as Text, method_name as Value, method_tooltip as Tooltip FROM osae_object_type_method ts INNER JOIN osae_object o ON o.object_type_id = ts.object_type_id where object_name = '" + gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString().Replace("'", "''") + "' ORDER BY method_label");
        ddlMethod.DataSource = methodData;
        ddlMethod.DataBind();
        for (int i = 0; i < ddlMethod.Items.Count; i++)
        {
            ddlMethod.Items[i].Attributes.Add("title", methodData.Tables[0].Rows[i]["Tooltip"].ToString());
        }
        if (ddlMethod.Items.Count == 0) divMethod.Visible = false;
        else divMethod.Visible = true;

        ddlMethod.Items.Insert(0, new ListItem(string.Empty, string.Empty));

        DataSet eventData = OSAESql.RunSQL("SELECT event_label as Text, event_name as Value, event_tooltip as Tooltip FROM osae_object_type_event ts INNER JOIN osae_object o ON o.object_type_id = ts.object_type_id where object_name = '" + gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString().Replace("'", "''") + "' ORDER BY event_label");
        ddlEvent.DataSource = eventData;
        ddlEvent.DataBind();
        for (int i = 0; i < ddlEvent.Items.Count; i++)
        {
            ddlEvent.Items[i].Attributes.Add("title", eventData.Tables[0].Rows[i]["Tooltip"].ToString());
        }
        if (ddlEvent.Items.Count == 0) divEvent.Visible = false;
        else divEvent.Visible = true;

        ddlEvent.Items.Insert(0, new ListItem(string.Empty, string.Empty));

        DataSet typeData = OSAESql.RunSQL("SELECT object_type as Text, object_type as Value, object_type_tooltip as Tooltip FROM osae_object_type ORDER BY object_type");
        ddlType.DataSource = typeData;
        ddlType.DataBind();
        for (int i = 0; i < ddlType.Items.Count; i++)
        {
            ddlType.Items[i].Attributes.Add("title", typeData.Tables[0].Rows[i]["Tooltip"].ToString());
        }
        if (ddlType.Items.Count == 0) ddlType.Visible = false;
        else ddlType.Visible = true;

        ddlType.Items.Insert(0, new ListItem(string.Empty, string.Empty));

        ddlContainer.DataSource = OSAESql.RunSQL("SELECT object_name as Text, object_name as Value FROM osae_v_object where container = 1 ORDER BY object_name"); ;
        ddlContainer.DataBind();
        if (ddlContainer.Items.Count == 0) ddlContainer.Visible = false;
        else ddlContainer.Visible = true;

        ddlContainer.Items.Insert(0, new ListItem(string.Empty, string.Empty));
    }

    private void loadProperties()
    {
        objPropMissing = false;
        objPropError = false;
        gvProperties.DataSource = OSAESql.RunSQL("SELECT property_name, property_value, property_datatype, object_property_id, DATE_FORMAT(last_updated,'%m/%d %h:%i:%s %p') as last_updated,source_name, trust_level,interest_level,property_object_type,property_tooltip,property_required FROM osae_v_object_property where object_name='" + gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString().Replace("'", "''") + "' ORDER BY property_name");
        gvProperties.DataBind();
        gvProperties.SelectedIndex = 0;
        if(objPropMissing == true) propEmpty.Visible = true;
        else propEmpty.Visible = false;
        if (objPropError == true) propError.Visible = true;
        else propError.Visible = false;
    }

    private void loadPropertyList()
    {
        gvPropList.DataSource = OSAESql.RunSQL("SELECT item_name, item_label FROM osae_object_property_array WHERE object_property_id = " + gvProperties.DataKeys[gvProperties.SelectedIndex]["object_property_id"].ToString() + " ORDER BY item_name");
        gvPropList.DataBind();
        //btnListItemUpdate.Enabled = false;
    }

    private void loadDetails()
    {
        OSAEObject obj = OSAEObjectManager.GetObjectByName(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString());
        txtName.Text = obj.Name;
        txtAlias.Text = obj.Alias;
        txtDescr.Text = obj.Description;
        txtAddress.Text = obj.Address;
        txtTrustLevel.Text = obj.MinTrustLevel.ToString();
        ddlContainer.SelectedValue = obj.Container;
        if (obj.State.Value != "")
        {
            ddlState.SelectedValue = obj.State.Value;
            DataSet stateData = (DataSet)ddlState.DataSource;
            int i = ddlState.SelectedIndex;
            ddlState.ToolTip = stateData.Tables[0].Rows[i]["Tooltip"].ToString();
        }

        ddlType.SelectedValue = obj.Type;
        chkEnabled.Checked = obj.Enabled;
            
        OSAEObjectType objtype = OSAEObjectTypeManager.ObjectTypeLoad(obj.Type);
        txtOwned.Text = objtype.OwnedBy;
        applyObjectSecurity(obj.Name, obj.MinTrustLevel.ToString());
    }

    protected void btnPropSave_Click(object sender, EventArgs e)
    {
        string value = "";
        string errMsg = "";
        if (hdnSelectedPropType.Text.ToUpper() == "BOOLEAN")
            value = ddlPropValue.SelectedValue;
        else
        {
            if (ddlPropValue.Visible) value = ddlPropValue.SelectedValue;
            else value = txtPropValue.Text;
        }
        if (propValidate(hdnSelectedPropType.Text.ToUpper(), value, out errMsg))
        {
            OSAEObjectPropertyManager.ObjectPropertySet(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), hdnSelectedPropName.Text, value, Session["Username"].ToString());
            loadProperties();
            propSaveError.Visible = false;
        }
        else
        {
            propSaveError.Text = errMsg;
            propSaveError.Visible = true;
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (OSAEObjectManager.ObjectExists(txtName.Text))
        {
            txtName.Text = "";
            txtDescr.Text = "";
            txtAlias.Text = "";
            txtAddress.Text = "";
            txtTrustLevel.Text = "";
            loadDDLs();
            ddlContainer.SelectedValue = string.Empty;
            ddlType.SelectedValue = string.Empty;
            chkEnabled.Checked = false;
            panelEditForm.Visible = false;
        }
        else
        {
            string eRr;
            string mSg;
            if (objValidate("Add", out eRr, out mSg))
            {
                OSAEObjectManager.ObjectAdd(txtName.Text, txtAlias.Text, txtDescr.Text, ddlType.SelectedItem.Value, txtAddress.Text, ddlContainer.SelectedValue, Convert.ToInt16(txtTrustLevel.Text), chkEnabled.Checked);
                gvObjects.DataSource = OSAESql.RunSQL(objectSQL);
                gvObjects.DataBind();
                objAddError.Text = "Object was CREATED successfully!";
                objAddError.ForeColor = System.Drawing.Color.Green;
                objAddError.Visible = true;
                if(mSg != "")
                {
                    objAddErrorMsg.Text = mSg;
                    objAddErrorMsg.Visible = true;
                }
            }
            else
            {
                objAddError.ForeColor = System.Drawing.Color.Red;
                objAddError.Text = "ERROR: " + eRr;
                objAddError.Visible = true;

            }
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string eRr;
        string mSg;
        if (objValidate("Delete", out eRr, out mSg))
        {
            OSAEObjectManager.ObjectDelete(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString());
            gvObjects.DataSource = OSAESql.RunSQL(objectSQL);
            gvObjects.DataBind();
            txtName.Text = "";
            txtAlias.Text = "";
            txtDescr.Text = "";
            txtAddress.Text = "";
            txtTrustLevel.Text = "";
            loadDDLs();
            ddlContainer.SelectedValue = string.Empty;
            ddlType.SelectedValue = string.Empty;
            chkEnabled.Checked = false;
            panelEditForm.Visible = false;
            objAddError.Text = "Object was DELETED successfully!";
            objAddError.ForeColor = System.Drawing.Color.Green;
            objAddError.Visible = true;
            hdnSelectedObjectName.Text = "";
            if (mSg != "")
            {
                objAddErrorMsg.Text = mSg;
                objAddErrorMsg.Visible = true;
            }
        }
        else
        {
            objAddError.ForeColor = System.Drawing.Color.Red;
            objAddError.Text = "ERROR: " + eRr;
            objAddError.Visible = true;
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        string eRr;
        string mSg;
        if (objValidate("Update", out eRr, out mSg ))
        {
            OSAEObjectManager.ObjectUpdate(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), txtName.Text, txtAlias.Text, txtDescr.Text, ddlType.SelectedValue, txtAddress.Text, ddlContainer.SelectedValue, Convert.ToInt16(txtTrustLevel.Text), chkEnabled.Checked);
            gvObjects.DataSource = OSAESql.RunSQL(objectSQL);
            gvObjects.DataBind();
            objAddError.Text = "Object was UPDATED successfully!";
            objAddError.ForeColor = System.Drawing.Color.Green;
            objAddError.Visible = true;
            if (mSg != "")
            {
                objAddErrorMsg.Text = mSg;
                objAddErrorMsg.Visible = true;
            }
        }
        else
        {
            objAddError.ForeColor = System.Drawing.Color.Red;
            objAddError.Text = "ERROR: " + eRr;
            objAddError.Visible = true;
        }
    }

    protected void btnListItemAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectPropertyManager.ObjectPropertyArrayAdd(hdnSelectedObjectName.Text, hdnSelectedPropName.Text, txtListItem.Text, txtListItemLabel.Text);
        hdnEditingPropList.Value = "1";
        loadPropertyList();
    }

    protected void btnListItemUpdate_Click(object sender, EventArgs e)
    {
        OSAEObjectPropertyManager.ObjectPropertyArrayUpdate(hdnSelectedObjectName.Text, hdnSelectedPropName.Text, hdnPropListItemName.Value,txtListItem.Text,txtListItemLabel.Text);
        hdnEditingPropList.Value = "1";
        loadPropertyList();
    }

    protected void btnListItemDelete_Click(object sender, EventArgs e)
    {
        //     OSAEObjectPropertyManager.ObjectPropertyArrayDelete(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), hdnSelectedPropName.Text, hdnPropListItemName.Value);
       // OSAEObjectPropertyManager.ObjectPropertyArrayDelete(gvObjects.DataKeys[gvObjects.SelectedIndex]["object_name"].ToString(), hdnSelectedPropName.Text, gvPropList.DataKeys[gvPropList.SelectedIndex]["item_name"].ToString());
        OSAEObjectPropertyManager.ObjectPropertyArrayDelete(hdnSelectedObjectName.Text, hdnSelectedPropName.Text, txtListItem.Text);

        hdnEditingPropList.Value = "1";
        loadPropertyList();
    }

    #region Trust Settings
    protected void applyPersonPropertySecurity(string objName, string objMinTrust, string propName, string propType, string propTrust)
    {
        if (Convert.ToInt32(Session["TrustLevel"].ToString()) < Convert.ToInt32(propTrust))
        {
            txtPropValue.Visible = false;
            lblRequired.Visible = false;
            btnPropSave.Visible = false;
            lblPropName.Visible = false;
            lblSourceName.Visible = false;
            lblTrustLevel.Visible = true;
            lblInterestLevel.Visible = false;
            btnEditPropList.Visible = false;
            ddlPropValue.Visible = false;
        }
        else if (propName == "Security Level")
        {
            OSAEObjectCollection osaAdmins = OSAE.OSAEObjectManager.GetObjectsByPropertyValue("Security Level", "Admin");
            string sL = OSAEObjectPropertyManager.GetObjectPropertyValue(objName, propName).Value;
            if(sL =="Admin" && osaAdmins.Count()<2)
            {
                txtPropValue.Visible = false;
                lblRequired.Visible = false;
                btnPropSave.Visible = false;
                lblPropName.Visible = false;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = false;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = false;
            }
            else if (Session["SecurityLevel"].ToString() == "Admin")
            {
                txtPropValue.Visible = false;
                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = true;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = true;
            }
            else
            {
                txtPropValue.Visible = false;
                lblRequired.Visible = false;
                btnPropSave.Visible = false;
                lblPropName.Visible = false;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = false;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = false;
            }
        }
        else if (propName == "Trust Level")
        {
            if (Session["SecurityLevel"].ToString() == "Admin")
            {
                txtPropValue.Visible = true;
                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = true;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = false;
            }
            else
            {
                txtPropValue.Visible = false;
                lblRequired.Visible = false;
                btnPropSave.Visible = false;
                lblPropName.Visible = false;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = false;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = false;
            }
        }
        else if (Session["UserName"].ToString() != objName && Session["SecurityLevel"].ToString() != "Admin")
        {
            txtPropValue.Visible = false;
            lblRequired.Visible = false;
            btnPropSave.Visible = false;
            lblPropName.Visible = false;
            lblSourceName.Visible = false;
            lblTrustLevel.Visible = true;
            lblInterestLevel.Visible = false;
            btnEditPropList.Visible = false;
            ddlPropValue.Visible = false;
        }
        else if (propType == "Password")
        {
            if (Session["SecurityLevel"].ToString() == "Admin" || Session["UserName"].ToString() == objName)
            {
                txtPropValue.Visible = true;
                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = true;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = false;
            }
            else
            {
                txtPropValue.Visible = false;
                btnPropSave.Visible = false;
                lblPropName.Visible = false;
                lblSourceName.Visible = false;
                lblTrustLevel.Visible = true;
                lblInterestLevel.Visible = false;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = false;
                lblRequired.Visible = false;
            }
        }
    }

    protected void applyObjectSecurity(string objName, string objMinTrust)
    {
        int sessTrust = Convert.ToInt32(Session["TrustLevel"].ToString());
        txtName.Enabled = false;
        txtAlias.Enabled = false;
        txtAddress.Enabled = false;
        txtDescr.Enabled = false;
        txtTrustLevel.Enabled = false;
        ddlType.Enabled = false;
        chkEnabled.Enabled = false;
        ddlState.Enabled = false;
        ddlMethod.Enabled = false;
        ddlEvent.Enabled = false;
        ddlContainer.Enabled = false;
        btnAdd.Enabled = false;
        btnUpdate.Enabled = false;
        btnDelete.Enabled = false;

        if (sessTrust >= Convert.ToInt32(objMinTrust))
        {
            ddlState.Enabled = true;
            ddlMethod.Enabled = true;
            ddlEvent.Enabled = true;
        }
        if (sessTrust >= adSet.ObjectsDeleteTrust)
        {
            btnDelete.Enabled = true;
        }
        if (sessTrust >= adSet.ObjectsUpdateTrust)
        {
            btnUpdate.Enabled = true;
            txtName.Enabled = true;
            txtAlias.Enabled = true;
            txtAddress.Enabled = true;
            txtDescr.Enabled = true;
            txtTrustLevel.Enabled = true;
            ddlType.Enabled = true;
            chkEnabled.Enabled = true;
            ddlContainer.Enabled = true;
        }
        if (sessTrust >= adSet.ObjectsAddTrust)
        {
            btnAdd.Enabled = true;
            txtName.Enabled = true;
            txtAlias.Enabled = true;
            txtAddress.Enabled = true;
            txtDescr.Enabled = true;
            txtTrustLevel.Enabled = true;
            ddlType.Enabled = true;
            chkEnabled.Enabled = true;
            ddlContainer.Enabled = true;
        }
    }
    #endregion

    #region Validation

    #region Property Validation
    protected Boolean propValidate(string propType, string value, out string msg)
    {
        bool allowEmpty;
        try
        {
            allowEmpty = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "AllowEmptyProperties").Value);
        }
        catch
        {
            allowEmpty = true;
        }
        Boolean vG = false;

        if (lblRequired.Visible == true)
        {
            if (string.IsNullOrEmpty(value))
            {
                vG = false;
                Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Property value required. You MUST enter a value.");
                msg = "Property value required. You MUST enter a value.";
                return vG;
            }
        }
        msg = "";
        switch (propType)
        {
            case "STRING":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty String Property Not Allowed.");
                        msg = "Empty String Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    vG = true;
                    break;
                }
            }

            case "BOOLEAN":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Boolean Property Not Allowed.");
                        msg = "Empty Boolean Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    vG = true;
                    break;
                }
            }

            case "DATETIME":
            {
                //string cul = OSAEObjectPropertyManager.GetObjectPropertyValue("System", "Culture").Value;
                string[] formats = { "MM/dd/yyyy", "M/d/yyyy", "M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt", "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss", "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", "M/d/yyyy h:mm", "M/d/yyyy h:mm", "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm" };
                DateTime dateValue;
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Empty Date/Time Property Not Allowed.");
                        msg = @"Empty Date/Time Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    //if (DateTime.TryParseExact(value, formats, new CultureInfo(cul), DateTimeStyles.None, out dateValue))
                    if (DateTime.TryParseExact(value, formats, new CultureInfo("en-US"), DateTimeStyles.None, out dateValue))
                        {
                        vG = true;
                        break;
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Date/time format");
                        msg = @"Property is an Invalid Date/time format";
                        break;
                    }
                }
            }

            case "FILE":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty File Property Not Allowed.");
                        msg = "Empty File Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    if (File.Exists(value))
                    {
                        vG = true;
                        break;
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property file:" + value  + "was not found.");
                        msg = @"Property file:" + value + "was not found.";
                        break;
                    }
                }
            }

            case "FLOAT":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Float Property Not Allowed.");
                        msg = "Empty Float Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    float isfloat;
                    if (float.TryParse(value, out isfloat))
                    {
                        vG = true;
                        break;
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Float value");
                        msg = @"Property is an Invalid Float value";
                        break;
                    }
                }
            }

            case "CURRENCY":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Currency Property Not Allowed.");
                        msg = "Empty Currency Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    float isfloat;
                    if (float.TryParse(value, out isfloat))
                    {
                        vG = true;
                        break;
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Currency value");
                        msg = @"Property is an Invalid Currency value";
                        break;
                    }
                }
            }

            case "PERCENT":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Percent Property Not Allowed.");
                        msg = "Empty Percent Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    float isfloat;
                    if (float.TryParse(value, out isfloat))
                    {
                        vG = true;
                        break;
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Percent value");
                        msg = @"Property is an Invalid Percent value";
                        break;
                    }
                }
            }

            case "INTEGER":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Integer Property Not Allowed.");
                        msg = "Empty Integer Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    int isint;
                    if (int.TryParse(value, out isint))
                    {
                        vG = true;
                        break;
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Integer value");
                        msg = @"Property is an Invalid Integer value";
                        break;
                    }
                }
            }

            case "LIST":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty List Property Not Allowed.");
                        msg = "Empty List Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    vG = true;
                    break;
                }
            }

            case "OBJECT":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Object Property Not Allowed.");
                        msg = "Empty Object Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    vG = true;
                    break;
                }
            }

            case "OBJECTTYPE":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Object-Type Property Not Allowed.");
                        msg = "Empty Object-Type Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    vG = true;
                    break;
                }
            }

            case "PASSWORD":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Password Property Not Allowed.");
                        msg = "Empty Password Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    vG = true;
                    break;
                }
            }

            case "URL":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty URL Property Not Allowed.");
                        msg = "Empty URL Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    Uri uriResult;
                    bool result = Uri.TryCreate(value, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (result)
                    {
                        vG = true;
                        break;
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid URL.");
                        msg = @"Property is an Invalid URL.";
                        break;
                    }
                }
            }

            case "EMAIL":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Email Property Not Allowed.");
                        msg = "Empty Email Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    if (value.IndexOf("@") > -1)
                    {
                        if (value.IndexOf(".") > -1)
                        {
                            if (value.Substring(0, value.IndexOf("@")).Length > 3)
                            {
                                vG = true;
                                break;
                            }
                            else
                            {
                                vG = false;
                                Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Email Address.");
                                msg = @"Property is an Invalid Email Address.";
                                break;
                            }
                        }
                        else
                        {
                            vG = false;
                            Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Email Address.");
                            msg = @"Property is an Invalid Email Address.";
                            break;
                        }
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Email Address.");
                        msg = @"Property is an Invalid Email Address.";
                        break;
                    }
                }
            }

            case "PHONENUM":
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Phone Number Property Not Allowed.");
                        msg = "Empty Phone Number Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    if (value.Length > 5)
                    {
                        if (value.Length < 15)
                        {
                            vG = true;
                            break;
                        }
                        else
                        {
                            vG = false;
                            Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Phone Number.");
                            msg = @"Property is an Invalid Phone Number.";
                            break;
                        }
                    }
                    else
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + @"Property is an Invalid Phone Number.");
                        msg = @"Property is an Invalid Phone Number.";
                        break;
                    }
                }
            }

            default:
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (allowEmpty == false)
                    {
                        vG = false;
                        Master.Log.Error(hdnSelectedObjectName.Text + ":" + hdnSelectedPropName.Text + " - " + propType + ":" + "Empty Property Not Allowed.");
                            msg = "Empty Property Not Allowed.";
                        break;
                    }
                    else
                    {
                        vG = true;
                        break;
                    }
                }
                else
                {
                    vG = true;
                    break;
                }
            }            
        }
        return vG;
    }
    #endregion

    #region Object Validation
    protected Boolean objValidate(string func, out string eRr, out string mSg)
    {
        Boolean gV = false;
        eRr = "";
        mSg = "";
        switch (func)
        {
            case "Add":
                {
                    Boolean gVa1=true, gVa2 = true, gVa3 = true, gVa4 = true;

                    if (string.IsNullOrEmpty(txtName.Text))
                    {
                        gVa1 = false;
                        eRr = "Missing Object Name";
                    }
                    if (string.IsNullOrEmpty(ddlType.SelectedItem.Value))
                    {
                        gVa2 = false;
                        eRr = "Missing Object Type";
                    }
                    if (string.IsNullOrEmpty(txtTrustLevel.Text))
                    {
                        gVa3 = false;
                        eRr = "Missing Object Trust Level";
                    }
                    if (OSAEObjectManager.ObjectExists(txtName.Text))
                    {
                        gVa4 = false;
                        eRr = "Object With Same Name Already Exist!";
                    }
                    if(gVa1 == true && gVa2 == true && gVa3 == true && gVa4 == true) { gV = true; }
                    if (string.IsNullOrEmpty(txtAlias.Text)) mSg = "Object Alias NOT specified! <br>";
                    if (string.IsNullOrEmpty(txtDescr.Text)) mSg += "Object Description NOT specified! <br>";
                    if (string.IsNullOrEmpty(ddlContainer.SelectedValue)) mSg += "Object Container NOT specified! <br>";
                    if (string.IsNullOrEmpty(txtAddress.Text)) mSg += "Object Address NOT specified! <br>";
                    break;
                }

            case "Update":
                {
                    Boolean gVa1 = true, gVa2 = true, gVa3 = true, gVa4 = true;

                    if (string.IsNullOrEmpty(txtName.Text))
                    {
                        gVa1 = false;
                        eRr = "Missing Object Name";
                    }
                    if (string.IsNullOrEmpty(ddlType.SelectedItem.Value))
                    {
                        gVa2 = false;
                        eRr = "Missing Object Type";
                    }
                    if (string.IsNullOrEmpty(txtTrustLevel.Text))
                    {
                        gVa3 = false;
                        eRr = "Missing Object Trust Level";
                    }
                    //if (!OSAEObjectManager.ObjectExists(txtName.Text))
                    //{
                    //    gVa4 = false;
                    //    eRr = "This Object does NOT Exist!";
                    //}
                    if (gVa1 == true && gVa2 == true && gVa3 == true && gVa4 == true) { gV = true; }
                    if (string.IsNullOrEmpty(txtAlias.Text)) mSg = "Object Alias NOT specified! <br>";
                    if (string.IsNullOrEmpty(txtDescr.Text)) mSg += "Object Description NOT specified! <br>";
                    if (string.IsNullOrEmpty(ddlContainer.SelectedValue)) mSg += "Object Container NOT specified! <br>";
                    if (string.IsNullOrEmpty(txtAddress.Text)) mSg += "Object Address NOT specified! <br>";
                    break;
                }

            case "Delete":
                {
                    Boolean gVa1 = true, gVa2 = true, gVa3 = true, gVa4 = true;
                    if (!OSAEObjectManager.ObjectExists(txtName.Text))
                    {
                        gVa4 = false;
                        eRr = "This Object does NOT Exist!";
                    }
                    if (gVa1 == true && gVa2 == true && gVa3 == true && gVa4 == true) { gV = true; }
                    break;
                }

            default:
                {
                    break;
                }
        }
        
        return gV;
    }
    #endregion

    #endregion
}