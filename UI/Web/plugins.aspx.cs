using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using System.Threading;
using System.Xml.Linq;
using System.Net;
using OSAE;
using NetworkCommsDotNet;
using ICSharpCode.SharpZipLib.Zip;

public partial class plugins : System.Web.UI.Page
{
    private BindingList<PluginDescription> pluginList = new BindingList<PluginDescription>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack) loadPlugins();
    }    

    private void loadPlugins()
    {
        pluginList = new BindingList<PluginDescription>();
        OSAEObjectCollection objs = OSAEObjectManager.GetObjectsByBaseType("PLUGIN");
        foreach (OSAEObject o in objs)
        {
            PluginDescription desc = new PluginDescription();

            desc.Name = o.Name;
            desc.Computer = o.Container;
            desc.Enabled = o.Enabled;
            if (o.State.Value == "ON")
                desc.Status = "Running";
            else
                desc.Status = "Stopped";

            pluginList.Add(desc);
            Master.Log.Info("Plugin found: Name:" + desc.Name + " Desc ID: " + desc.ID);
        }

        /*
    pluginList = new BindingList<PluginDescription>();
    List<string> osapdFiles = new List<string>();
    string[] pluginFile = Directory.GetFiles(Common.ApiPath + "\\Plugins", "*.osapd", SearchOption.AllDirectories);
    osapdFiles.AddRange(pluginFile);


    foreach (string path in osapdFiles)
    {
        if (!string.IsNullOrEmpty(path))
        {
            PluginDescription desc = new PluginDescription();

            desc.Deserialize(path);
            desc.Status = "No Object";
            desc.Enabled = false;

            if (desc.WikiUrl.Trim() == "")
                desc.WikiUrl = "http://www.opensourceautomation.com/wiki/index.php?title=Plugins";

            OSAEObjectCollection objs = OSAEObjectManager.GetObjectsByType(desc.Type);
            foreach (OSAEObject o in objs)
            {
                if (OSAEObjectPropertyManager.GetObjectPropertyValue(o.Name, "Computer Name").Value == Common.ComputerName || desc.Type == o.Name)
                {
                    desc.Name = o.Name;
                    if (o.Enabled == 1)
                        desc.Enabled = true;
                    if (o.State.Value == "ON")
                        desc.Status = "Running";
                    else
                        desc.Status = "Stopped";

                    pluginList.Add(desc);
                    Master.Log.Info("Plugin found: Name:" + desc.Name + " Desc ID: " + desc.ID);
                }
            }
        }
    }
    */

        // TODO: Load all other objects with base type of PLUGIN.  These objects represent plugins on client instances.  Maybe make a separate grid since it wont be able to load the osapd files on the clients

        checkForUpdates();
        gvPlugins.DataSource = pluginList;
        gvPlugins.DataBind();
    }
    
    protected void gvPlugins_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox enabled = ((CheckBox)e.Row.FindControl("chkEnabled"));
            
            Label lbl = ((Label)e.Row.FindControl("lblEnabled"));
            if (lbl.Text == "True")
                enabled.Checked = true;
            else
                enabled.Checked = false;

            HyperLink hyp = ((HyperLink)e.Row.FindControl("hypUpgrade"));
            Label id = ((Label)e.Row.FindControl("lblID"));
            Label upgr = ((Label)e.Row.FindControl("lblLatestVersion"));
            if (upgr.Text != "")
            {
                hyp.NavigateUrl = "http://www.opensourceautomation.com/plugin_details.php?pid=" + id.Text;
                hyp.Visible = true;
                upgr.Visible = false;
            }
            else
                hyp.Visible = false;
        }
    }

    protected void chkEnabled_OnCheckedChanged(object sender, EventArgs e)
    {
        try
        {
            CheckBox ckbx = (CheckBox)sender;
            string pluginName = "";
            string enabled;
            if (ckbx.Checked)
                enabled = "True";
            else
                enabled = "False";

            foreach (GridViewRow rw in gvPlugins.Rows)
            {
                CheckBox chkBx = (CheckBox)rw.FindControl("chkEnabled");
                if (chkBx == ckbx)
                    pluginName = ((Label)rw.FindControl("lblObject")).Text;
            }

            NetworkComms.SendObject("Plugin", Common.LocalIPAddress(), 10051, pluginName + "|" + enabled);
            Master.Log.Info("Sending message: " + "ENABLEPLUGIN|" + pluginName + "|" + enabled);

            foreach (PluginDescription plugin in pluginList)
            {
                if (plugin.Name == pluginName && plugin.Name != null)
                    plugin.Status = "Starting...";
            }

            OSAEObject obj = OSAEObjectManager.GetObjectByName(pluginName);
            OSAEObjectManager.ObjectUpdate(obj.Name, obj.Name, obj.Alias, obj.Description, obj.Type, obj.Address, obj.Container, obj.MinTrustLevel, true);
            loadPlugins();
        }
        catch (Exception ex)
        { Master.Log.Info("Error enabling plugin: " + ex.Message + " Inner Exception: " + ex.InnerException); }
    }


    private void checkForUpdates()
    {
        try
        {
            string url = "http://www.opensourceautomation.com/checkPluginUpdates.php";
            //logging.AddToLog("Checking for plugin updates: " + url, false);
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            //XmlReader rdr = XmlReader.Create(responseStream);

            XElement xml = XElement.Load(responseStream);

            int curMajor, curMinor, curRevion, latestMajor = 0, latestMinor = 0, latestRevision = 0;

            var query = from e in xml.Elements("plugin")
                        select new { A = e.Element("name").Value, B = e.Element("major").Value, C = e.Element("minor").Value, D = e.Element("revision").Value, link = e.Element("link").Value };

            foreach (PluginDescription d in pluginList)
            {
                string[] split = d.Version.Split('.');
                curMajor = Int32.Parse(split[0]);
                curMinor = Int32.Parse(split[1]);
                curRevion = Int32.Parse(split[2]);
                
                foreach (var e in query)
                {
                    if (e.A == d.Type)
                    {
                        latestMajor = Int32.Parse(e.B);
                        latestMinor = Int32.Parse(e.C);
                        latestRevision = Int32.Parse(e.D);

                        if ((latestMajor > curMajor) || (latestMajor == curMajor && latestMinor > curMinor) || (latestMajor == curMajor && latestMinor == curMinor && latestRevision > curRevion))
                            d.Upgrade = latestMajor + "." + latestMinor + "." + latestRevision;
                        else
                            d.Upgrade = "";
                    }
                }
                response.Close();
            }            
        }
        catch { }
    }

    protected void btnGetMorePlugins_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/morePlugins.aspx");
    }
}