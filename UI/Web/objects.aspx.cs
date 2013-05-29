using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OSAE;

public partial class home : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvObjects")
        {
            alert.Visible = false;
            hdnSelectedRow.Text = args[1];
            panelEditForm.Visible = true;
            panelPropForm.Visible = false;
            divParameters.Visible = false;
            hdnSelectedPropRow.Text = "";
            hdnSelectedObjectName.Text = gvObjects.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["object_name"].ToString();
            loadDDLs();
            loadProperties();
            loadDetails();
        }
        else if (args[0] == "gvProperties")
        {
            hdnSelectedPropRow.Text = args[1];
            panelPropForm.Visible = true;
            hdnSelectedPropName.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_name"].ToString();
            lblPropName.Text = hdnSelectedPropName.Text;
            lblPropLastUpd.Text = "Last Updated: " + gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["last_updated"].ToString();
            hdnSelectedPropType.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_datatype"].ToString();

            if (gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_datatype"].ToString() == "List")
            {
                loadPropertyList();
                txtPropValue.Visible = false;
                btnPropSave.Visible = false;
                lblPropName.Visible = false;
                btnEditPropList.Visible = true;
                ddlPropValue.Visible = false;
            }
            else if (gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_datatype"].ToString() == "Boolean")
            {
                ddlPropValue.Items.Clear(); 
                ddlPropValue.Items.Add(new ListItem("TRUE", "TRUE"));
                ddlPropValue.Items.Add(new ListItem("FALSE", "FALSE"));
                if (!String.IsNullOrEmpty(gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_value"].ToString()) && ddlPropValue.Items.FindByValue(gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_value"].ToString()) != null)
                    ddlPropValue.SelectedValue = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_value"].ToString();
                txtPropValue.Visible = false;
                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                btnEditPropList.Visible = false;
                ddlPropValue.Visible = true;
            }
            else
            {
                string propID = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["object_property_id"].ToString();
                DataSet options = OSAESql.RunSQL("SELECT option_name FROM osae_object_type_property_option ootpo INNER JOIN osae_object_property oop ON oop.object_type_property_id = ootpo.property_id WHERE oop.object_property_id=" + propID);
                ddlPropValue.Items.Clear();

                if (options.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in options.Tables[0].Rows)
                    {
                        ddlPropValue.Items.Add(new ListItem(dr["option_name"].ToString(), dr["option_name"].ToString()));
                    }
                    if(!string.IsNullOrEmpty(gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_value"].ToString()) && ddlPropValue.Items.FindByValue(gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_value"].ToString()) != null)
                        ddlPropValue.SelectedValue = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_value"].ToString();
                    txtPropValue.Visible = false;
                    ddlPropValue.Visible = true;
                }
                else
                {
                    txtPropValue.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["property_value"].ToString();
                    txtPropValue.Visible = true;
                    ddlPropValue.Visible = false;
                }

                btnPropSave.Visible = true;
                lblPropName.Visible = true;
                btnEditPropList.Visible = false;
            }
            
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        gvObjects.DataSource = OSAESql.RunSQL("SELECT object_id, container_name, object_name, object_type, state_name, last_updated, address FROM osae_v_object order by container_name, object_name");
        gvObjects.DataBind();
        if (!this.IsPostBack)
        {
            loadDDLs();
        }

        if (hdnSelectedPropRow.Text != "")
        {
            loadProperties();
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedRow.Text != "")
        {
            gvObjects.Rows[Int32.Parse(hdnSelectedRow.Text)].Attributes.Remove("onmouseout");
            gvObjects.Rows[Int32.Parse(hdnSelectedRow.Text)].Style.Add("background", "lightblue");
        }
        if (hdnSelectedPropRow.Text != "")
        {
            gvProperties.Rows[Int32.Parse(hdnSelectedPropRow.Text)].Attributes.Remove("onmouseout");
            gvProperties.Rows[Int32.Parse(hdnSelectedPropRow.Text)].Style.Add("background", "lightblue");
        }
        hdnEditingPropList.Value = hdnEditingPropList.Value;
    }

    protected void gvObjects_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvObjects_" + e.Row.RowIndex.ToString()));
        }

    }

    protected void gvProperties_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {
        if ((e.Row.RowType == DataControlRowType.DataRow))
        {
            DataRowView drv = (DataRowView)e.Row.DataItem;
            if (drv["property_datatype"] != DBNull.Value)
            {
                if (((string)drv["property_datatype"]).ToUpper() == "PASSWORD")
                {
                    e.Row.Cells[1].Text = "*****";
                }
            } 
           
            e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.background='lightblue';");
            if (e.Row.RowState == DataControlRowState.Alternate)
                e.Row.Attributes.Add("onmouseout", "this.style.background='#fcfcfc url(Images/grd_alt.png) repeat-x top';");
            else
                e.Row.Attributes.Add("onmouseout", "this.style.background='none';");
            e.Row.Attributes.Add("onclick", ClientScript.GetPostBackClientHyperlink(this, "gvProperties_" + e.Row.RowIndex.ToString()));
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
            e.Row.Attributes.Add("onclick", "selectPropListItem('" + gvPropList.DataKeys[e.Row.RowIndex]["item_name"].ToString() + "', this);");
        }

    }

    protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
    {
        OSAEObjectStateManager.ObjectStateSet(hdnSelectedObjectName.Text, ddlState.SelectedItem.Value, "Web UI");
        lblAlert.Text = "State set successfully to " + ddlState.SelectedItem.Text;
        alert.Visible = true;
    }
    protected void ddlMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataSet ds = OSAESql.RunSQL("SELECT param_1_label, param_2_label, param_1_default, param_2_default FROM osae_v_object_type_method otm INNER JOIN osae_object oo ON oo.object_type_id = otm.object_type_id WHERE object_name = '" + hdnSelectedObjectName.Text + "' AND method_name = '" + ddlMethod.SelectedItem.Value + "'");
        DataTable dt = ds.Tables[0];
        if (dt.Rows.Count > 0)
        {
            if (!string.IsNullOrEmpty(dt.Rows[0]["param_1_label"].ToString()))
            {
                divParameters.Visible = true;
                txtParam1.Text = dt.Rows[0]["param_1_default"].ToString();
                txtParam2.Text = dt.Rows[0]["param_2_default"].ToString();
                if (!String.IsNullOrEmpty(dt.Rows[0]["param_1_label"].ToString()))
                    lblParam1.Text = "(" + dt.Rows[0]["param_1_label"].ToString() + ")";
                if (!String.IsNullOrEmpty(dt.Rows[0]["param_2_label"].ToString()))
                    lblParam2.Text = "(" + dt.Rows[0]["param_2_label"].ToString() + ")";
            }
            else
            {
                OSAEMethodManager.MethodQueueAdd(hdnSelectedObjectName.Text, ddlMethod.SelectedItem.Value, "", "", "Web UI");
                lblAlert.Text = "Method successfuly executed: " + ddlMethod.SelectedItem.Text;
                alert.Visible = true;
            }
        }
        else
        {
            OSAEMethodManager.MethodQueueAdd(hdnSelectedObjectName.Text, ddlMethod.SelectedItem.Value, "", "", "Web UI");
            lblAlert.Text = "Method successfuly executed: " + ddlMethod.SelectedItem.Text;
            alert.Visible = true;
        }
    }

    protected void btnExecute_Click(object sender, EventArgs e)
    {
        OSAEMethodManager.MethodQueueAdd(hdnSelectedObjectName.Text, ddlMethod.SelectedItem.Value, txtParam1.Text, txtParam2.Text, "Web UI");
        lblAlert.Text = "Method successfuly executed: " + ddlMethod.SelectedItem.Text;
        alert.Visible = true;
        divParameters.Visible = false;
    }

    protected void ddlEvent_SelectedIndexChanged(object sender, EventArgs e)
    {
        logging.EventLogAdd(hdnSelectedObjectName.Text, ddlEvent.SelectedItem.Value);
        lblAlert.Text = "Event set successfully to " + ddlEvent.SelectedItem.Text;
        alert.Visible = true;
    }

    private void loadDDLs()
    {
        ddlState.DataSource = OSAESql.RunSQL("SELECT state_label as Text, state_name as Value FROM osae_object_type_state ts INNER JOIN osae_object o ON o.object_type_id = ts.object_type_id where object_name = '" + hdnSelectedObjectName.Text + "'"); ;
        ddlState.DataBind();
        if (ddlState.Items.Count == 0)
            divState.Visible = false;
        else
            divState.Visible = true;
        

        ddlMethod.DataSource = OSAESql.RunSQL("SELECT method_label as Text, method_name as Value FROM osae_object_type_method ts INNER JOIN osae_object o ON o.object_type_id = ts.object_type_id where object_name = '" + hdnSelectedObjectName.Text + "'"); ;
        ddlMethod.DataBind();
        if (ddlMethod.Items.Count == 0)
            divMethod.Visible = false;
        else
            divMethod.Visible = true;
        ddlMethod.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlEvent.DataSource = OSAESql.RunSQL("SELECT event_label as Text, event_name as Value FROM osae_object_type_event ts INNER JOIN osae_object o ON o.object_type_id = ts.object_type_id where object_name = '" + hdnSelectedObjectName.Text + "'"); ;
        ddlEvent.DataBind();
        if (ddlEvent.Items.Count == 0)
            divEvent.Visible = false;
        else
            divEvent.Visible = true;
        ddlEvent.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlType.DataSource = OSAESql.RunSQL("SELECT object_type as Text, object_type as Value FROM osae_object_type"); ;
        ddlType.DataBind();
        if (ddlType.Items.Count == 0)
            ddlType.Visible = false;
        else
            ddlType.Visible = true;
        ddlType.Items.Insert(0, new ListItem(String.Empty, String.Empty));

        ddlContainer.DataSource = OSAESql.RunSQL("SELECT object_name as Text, object_name as Value FROM osae_v_object where container = 1"); ;
        ddlContainer.DataBind();
        if (ddlContainer.Items.Count == 0)
            ddlContainer.Visible = false;
        else
            ddlContainer.Visible = true;
        ddlContainer.Items.Insert(0, new ListItem(String.Empty, String.Empty));
    }

    private void loadProperties()
    {
        gvProperties.DataSource = OSAESql.RunSQL("SELECT property_name, property_value, property_datatype, object_property_id, last_updated FROM osae_v_object_property where object_name='" + hdnSelectedObjectName.Text + "'");
        gvProperties.DataBind();
    }

    private void loadPropertyList()
    {
        gvPropList.DataSource = OSAESql.RunSQL("SELECT item_name,item_label FROM osae_object_property_array WHERE object_property_id = " + gvProperties.DataKeys[Int32.Parse(hdnSelectedPropRow.Text)]["object_property_id"].ToString());
        gvPropList.DataBind();
    }

    private void loadDetails()
    {
        OSAEObject obj = OSAEObjectManager.GetObjectByName(gvObjects.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["object_name"].ToString());
        txtName.Text = obj.Name;
        txtDescr.Text = obj.Description;
        txtAddress.Text = obj.Address;
        ddlContainer.SelectedValue = obj.Container;
        if(obj.State.Value != "")
            ddlState.SelectedValue = obj.State.Value;
        ddlType.SelectedValue = obj.Type;
        if (obj.Enabled == 1)
            chkEnabled.Checked = true;
        else
            chkEnabled.Checked = false;
    }

    protected void btnPropSave_Click(object sender, EventArgs e)
    {
        string value = "";
        if (hdnSelectedPropType.Text.ToUpper() == "BOOLEAN")
            value = ddlPropValue.SelectedValue;
        else
        {
            if(ddlPropValue.Visible)
                value = ddlPropValue.SelectedValue;
            else
                value = txtPropValue.Text;
        }

        OSAEObjectPropertyManager.ObjectPropertySet(hdnSelectedObjectName.Text, hdnSelectedPropName.Text, value, "Web UI");
        loadProperties();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        OSAEObjectManager.ObjectAdd(txtName.Text, txtDescr.Text, ddlType.SelectedItem.Value, txtAddress.Text, ddlContainer.SelectedValue, chkEnabled.Checked);
        gvObjects.DataSource = OSAESql.RunSQL("SELECT object_id, container_name, object_name, object_type, state_name, last_updated, address FROM osae_v_object order by container_name, object_name");
        gvObjects.DataBind();
        txtName.Text = "";
        txtDescr.Text = "";
        txtAddress.Text = "";
        ddlContainer.SelectedValue = "";
        ddlType.SelectedValue = "";
        chkEnabled.Checked = false;
        panelEditForm.Visible = false;
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectManager.ObjectDelete(gvObjects.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["object_name"].ToString());
        gvObjects.DataSource = OSAESql.RunSQL("SELECT object_id, container_name, object_name, object_type, state_name, last_updated, address FROM osae_v_object order by container_name, object_name");
        gvObjects.DataBind();
        txtName.Text = "";
        txtDescr.Text = "";
        txtAddress.Text = "";
        ddlContainer.SelectedValue = "";
        ddlType.SelectedValue = "";
        chkEnabled.Checked = false;
        panelEditForm.Visible = false;
        int selectedRow = Int32.Parse(hdnSelectedRow.Text) - 1;
        if (selectedRow < 0)
            hdnSelectedRow.Text = "";
        else
            hdnSelectedRow.Text = selectedRow.ToString();
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        int enabled = 0;
        if(chkEnabled.Checked)
            enabled = 1;
        OSAEObjectManager.ObjectUpdate(gvObjects.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["object_name"].ToString(), txtName.Text, txtDescr.Text, ddlType.SelectedValue, txtAddress.Text, ddlContainer.SelectedValue, enabled);
        gvObjects.DataSource = OSAESql.RunSQL("SELECT object_id, container_name, object_name, object_type, state_name, last_updated, address FROM osae_v_object order by container_name, object_name");
        gvObjects.DataBind();
    }

    protected void btnListItemSave_Click(object sender, EventArgs e)
    {
        OSAEObjectPropertyManager.ObjectPropertyArrayAdd(hdnSelectedObjectName.Text, hdnSelectedPropName.Text, txtListItem.Text, txtListItemLabel.Text);
        hdnEditingPropList.Value = "1";
        loadPropertyList();
    }

    protected void btnListItemDelete_Click(object sender, EventArgs e)
    {
        OSAEObjectPropertyManager.ObjectPropertyArrayDelete(hdnSelectedObjectName.Text, hdnSelectedPropName.Text, hdnPropListItemName.Value);
        hdnEditingPropList.Value = "1";
        loadPropertyList();
    }
}