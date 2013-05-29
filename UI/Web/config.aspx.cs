using OSAE;
using System;
using System.Data;
using System.ServiceProcess;
using System.Xml.Linq;
using System.Net;
using System.IO;

public partial class config : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblVersion.Text = OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "DB Version").Value;
            if (OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM", "Debug").Value == "TRUE")
            {
                ddlDebug.SelectedIndex = 0;
            }
            else
            {
                ddlDebug.SelectedIndex = 1;
            }
            CheckServiceStatus();
            GetDBSize();
            GetTableSizes();
            checkForUpdates();
        }
    }
    protected void ddlDebug_SelectedIndexChanged(object sender, EventArgs e)
    {
        string s = ddlDebug.SelectedItem.Text.ToUpper();
        OSAEObjectPropertyManager.ObjectPropertySet("SYSTEM", "Debug", ddlDebug.SelectedItem.Text.ToUpper(), "Web UI");
    }

    private void CheckServiceStatus()
    {
        try
        {
            ServiceController sc = new ServiceController("OSAE");
            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    serviceLabel.Text = "Running";
                    break;
                case ServiceControllerStatus.Stopped:
                    serviceLabel.Text = "Stopped";
                    break;
                case ServiceControllerStatus.Paused:
                    serviceLabel.Text = "Paused";
                    break;
                case ServiceControllerStatus.StopPending:
                    serviceLabel.Text = "Stopping";
                    break;
                case ServiceControllerStatus.StartPending:
                    serviceLabel.Text = "Starting";
                    break;
                default:
                    serviceLabel.Text = "Status Changing";
                    break;
            }
        }
        catch (Exception exc)
        {
            serviceLabel.Text = "Could not find service";
        }
    }

    private void GetDBSize()
    {
        string sql = "SELECT SUM( data_length + index_length) / 1024 / 1024 AS size FROM information_schema.TABLES WHERE table_schema = 'OSAE';";
        DataSet d = OSAESql.RunSQL(sql);
        decimal size = (decimal)d.Tables[0].Rows[0]["size"];
        dbSize.Text = String.Format("{0:0.##}", size) + "Mb";
    }

    private void GetTableSizes()
    {
        imagesSize.Text = GetTableSize("osae_images");
        debugLog.Text = GetTableSize("osae_debug_log");
        objectsSize.Text = GetTableSize("osae_object");
        eventLogSize.Text = GetTableSize("osae_event_log");
        methodLogSize.Text = GetTableSize("osae_method_log");
        scriptSize.Text = GetTableSize("osae_script");
        objectStateHistory.Text = GetTableSize("osae_object_state_history");
        objectPropertyHistory.Text = GetTableSize("osae_object_property_history");
        methodQueueSize.Text = GetTableSize("osae_method_queue");
    }

    private string GetTableSize(string tableName)
    {
        decimal size = 0;
        string sql;
        DataSet d;

        sql = "SELECT SUM( data_length + index_length) / 1024 / 1024 AS size FROM information_schema.TABLES WHERE table_schema = 'OSAE' AND TABLE_NAME = '" + tableName + "';";
        d = OSAESql.RunSQL(sql);
        size = (decimal)d.Tables[0].Rows[0]["size"];
        return String.Format("{0:0.##}", size) + "Mb";
    }

    private void checkForUpdates()
    {
        try
        {
            string url = "http://www.opensourceautomation.com/getLatestVersion.php?v=" + OSAEObjectPropertyManager.GetObjectPropertyValue("SYSTEM","DB Version").Value;
            //logging.AddToLog("Checking for plugin updates: " + url, false);
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            //XmlReader rdr = XmlReader.Create(responseStream);

            XElement xml = XElement.Load(responseStream);
            XElement svc = xml.Element("service");

            string latest = svc.Element("major").Value + "." + svc.Element("minor").Value + "." + svc.Element("revision").Value;

            if (latest != lblVersion.Text)
                hypUpgrade.Visible = true;
            else
                hypUpgrade.Visible = false;

            response.Close();
        }
        catch (Exception ex)
        {

        }
    }
    protected void scriptsExportButton_Click(object sender, EventArgs e)
    {
        DataSet d = OSAESql.RunSQL("SELECT * FROM osae_script");
        d.DataSetName = "Scripts";
        d.Tables[0].TableName = "Script";

        MemoryStream s = new MemoryStream();        
        d.WriteXml(s);

        Response.BinaryWrite(s.ToArray());
        Response.ContentType = "text/xml";
        Response.AddHeader("Content-Disposition", "attachment; filename=Scripts.xml");
        Response.End();
    }
    protected void objectsExportButton_Click(object sender, EventArgs e)
    {
        DataSet d = OSAESql.RunSQL("SELECT object_id, container_name, object_name, object_type, state_name, last_updated, address FROM osae_v_object order by container_name, object_name");
        MemoryStream s = new MemoryStream();
        d.DataSetName = "Objects";
        d.Tables[0].TableName = "Object";
        d.WriteXml(s);

        Response.BinaryWrite(s.ToArray());
        Response.ContentType = "text/xml";
        Response.AddHeader("Content-Disposition", "attachment; filename=Objects.xml");
        Response.End();
    }
}