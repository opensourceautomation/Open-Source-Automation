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

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
public partial class plugins : System.Web.UI.Page, WCFServiceReference.IWCFServiceCallback
{
    private BindingList<PluginDescription> pluginList = new BindingList<PluginDescription>();
    WCFServiceReference.WCFServiceClient wcfObj;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            loadPlugins();
        }
    }

    private void loadPlugins()
    {
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
                desc.Status = "Stopped";
                desc.Enabled = false;

                

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
                    }
                }
                pluginList.Add(desc);
                
                //logging.AddToLog("Plugin found: Name:" + desc.Name + " Desc ID: " + desc.ID, true);
            }
        }
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
                {
                    pluginName = ((Label)rw.FindControl("lblObject")).Text;
                }
            }
        
            //logging.AddToLog("checked: " + pd.Name, true);

            connectToService();
            if (wcfObj.State == CommunicationState.Opened)
            {
                Thread thread = new Thread(() => messageHost(WCFServiceReference.OSAEWCFMessageType.PLUGIN, "ENABLEPLUGIN|" + pluginName + "|" + enabled));
                thread.Start();
                //logging.AddToLog("Sending message: " + "ENABLEPLUGIN|" + pd.Name + "|True", true);
                foreach (PluginDescription plugin in pluginList)
                {
                    if (plugin.Name == pluginName && plugin.Name != null)
                    {
                        plugin.Status = "Starting...";
                    }
                }
            }

            OSAEObject obj = OSAEObjectManager.GetObjectByName(pluginName);
            OSAEObjectManager.ObjectUpdate(obj.Name, obj.Name, obj.Description, obj.Type, obj.Address, obj.Container, 1);
            loadPlugins();
        }
        catch (Exception ex)
        {
            //logging.AddToLog("Error enabling plugin: " + ex.Message + " Inner Exception: " + ex.InnerException, true);
        }
    }

    private void messageHost(WCFServiceReference.OSAEWCFMessageType msgType, string message)
    {
        try
        {
            if (wcfObj.State == CommunicationState.Opened)
                wcfObj.messageHost(msgType, message, Common.ComputerName);
            else
            {
                if (connectToService())
                    wcfObj.messageHost(msgType, message, Common.ComputerName);
            }
        }
        catch (Exception ex)
        {
            //logging.AddToLog("Error messaging host: " + ex.Message, true);
        }
    }

    private bool connectToService()
    {
        try
        {
            EndpointAddress ep = new EndpointAddress("net.tcp://" + Common.DBConnection + ":8731/WCFService/");
            InstanceContext context = new InstanceContext(this);
            wcfObj = new WCFServiceReference.WCFServiceClient(context, "NetTcpBindingEndpoint", ep);
            wcfObj.Subscribe();
            //logging.AddToLog("Connected to Service", true);
            //reloadPlugins();
            return true;
        }
        catch (Exception ex)
        {
            //logging.AddToLog("Unable to connect to service.  Is it running? - " + ex.Message, true);
            return false;
        }
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

            foreach (PluginDescription d in pluginList)
            {
                string[] split = d.Version.Split('.');
                curMajor = Int32.Parse(split[0]);
                curMinor = Int32.Parse(split[1]);
                curRevion = Int32.Parse(split[2]);

                var query = from e in xml.Elements("plugin")
                            select new { A = e.Element("name").Value, B = e.Element("major").Value, C = e.Element("minor").Value, D = e.Element("revision").Value };

                foreach (var e in query)
                {
                    if (e.A == d.Type)
                    {
                        latestMajor = Int32.Parse(e.B);
                        latestMinor = Int32.Parse(e.C);
                        latestRevision = Int32.Parse(e.D);

                        if ((latestMajor > curMajor) || (latestMajor == curMajor && latestMinor > curMinor) || (latestMajor == curMajor && latestMinor == curMinor && latestRevision > curRevion))
                        {
                            d.Upgrade = latestMajor + "." + latestMinor + "." + latestRevision;
                        }
                        else
                        {
                            d.Upgrade = "";
                        }
                    }
                }
                response.Close();
            }
        }
        catch (Exception ex)
        { 
           
        }
    }

    public void OnMessageReceived(WCFServiceReference.OnMessageReceived request)
    {
       
    }
}