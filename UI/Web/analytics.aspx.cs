using System;
using OSAE;

public partial class analytics : System.Web.UI.Page
{
    private int restPort = 8732;

    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');

        if (args[0] == "gvProperties")
        {
            hdnSelectedRow.Text = args[1];
            hdnSelectedPropID.Text = gvProperties.DataKeys[Int32.Parse(hdnSelectedRow.Text)]["prop_id"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx?ReturnUrl=analytics.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("Analytics Trust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet) Response.Redirect("~/permissionError.aspx");
        loadProperties();
        loadStates();
        getRestPort();
        SetSessionTimeout();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (hdnSelectedRow.Text != "")
        {
            gvProperties.Rows[Int32.Parse(hdnSelectedRow.Text)].Attributes.Remove("onmouseout");
            gvProperties.Rows[Int32.Parse(hdnSelectedRow.Text)].Style.Add("background", "lightblue");
        }
    }
    
    private void loadProperties()
    {
        gvProperties.DataSource = OSAESql.RunSQL("SELECT DISTINCT CONCAT(object_name,' - ',property_name) as prop_name, object_name, property_name, LEFT(property_datatype, 1) AS property_datatype FROM osae_v_object_property_history WHERE property_datatype IN ('Integer', 'Float', 'Boolean') ORDER BY prop_name");
        gvProperties.DataBind();
    }

    private void loadStates()
    {
        gvStates.DataSource = OSAESql.RunSQL("SELECT DISTINCT object_name FROM osae_v_object_state_change_history ORDER BY object_name");
        gvStates.DataBind();
    }

    private void getRestPort()
    {
        if (!OSAEObjectPropertyManager.GetObjectPropertyValue("Rest", "REST Port").Id.Equals(String.Empty))
        {
            try
            {
                restPort = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue("Rest", "REST Port").Value);
            }
            catch (FormatException) { }
            catch (OverflowException) { }
            catch (ArgumentNullException) { }
        }
        hdnRestPort.Value = restPort.ToString();
    }

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
