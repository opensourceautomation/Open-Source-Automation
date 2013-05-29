using ICSharpCode.SharpZipLib.Zip;
using OSAE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using WCF;

public partial class morePlugins : System.Web.UI.Page, WCF.IMessageCallback
{
    /// <summary>
    /// WCF Object used for communicating to other components.
    /// </summary>
    IWCFService wcfObj;

    protected void Page_Load(object sender, EventArgs e)
    {
        ConnectToService();

        if (!IsPostBack)
        {
            GetPlugins();
        }
    }

    private void ConnectToService()
    {
        InstanceContext site = new InstanceContext(this);
        NetTcpBinding tcpBinding = new NetTcpBinding();
        tcpBinding.TransactionFlow = false;
        tcpBinding.ReliableSession.Ordered = true;
        tcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
        tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
        tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
        tcpBinding.Security.Mode = SecurityMode.None;

        EndpointAddress myEndpoint = new EndpointAddress("net.tcp://" + Common.DBConnection + ":8731/WCFService/");
        var myChannelFactory = new DuplexChannelFactory<IWCFService>(site, tcpBinding);

        wcfObj = myChannelFactory.CreateChannel(myEndpoint);
        wcfObj.Subscribe();
    }
       
    private void GetPlugins()
    {
        string url = "http://www.opensourceautomation.com/checkPluginUpdates.php";
        WebRequest request = HttpWebRequest.Create(url);
        WebResponse response = request.GetResponse();
        Stream responseStream = response.GetResponseStream();

        XElement xml = XElement.Load(responseStream);
        List<string> installedPlugins = GetInstalledPlugins();

        var query = from e in xml.Elements("plugin")
                    where !installedPlugins.Contains(e.Element("name").ToString())
                    select new { name = e.Element("name").Value,
                        guid = e.Element("guid").Value,
                        link = "http://www.opensourceautomation.com/" + e.Element("link").Value,
                        version = e.Element("major").Value + "." + e.Element("minor").Value + "." + e.Element("revision").Value + " " + e.Element("state").Value,
                        imageURL = "http://www.opensourceautomation.com/osap/plugins/" + 
                            e.Element("guid").Value + "/" + 
                            e.Element("major").Value + "." + 
                            e.Element("minor").Value + "." +
                            e.Element("revision").Value + "/" +
                            e.Element("state").Value + "/Screenshot.jpg"
                   };

        var notInstalled = from e in query
                       where !installedPlugins.Contains(e.name)
                       select e;

        AvailablePluginsGridView.DataSource = notInstalled;
        AvailablePluginsGridView.DataBind();
    }

    private List<string> GetInstalledPlugins()
    {
        List<string> pluginNames = new List<string>();        
        List<string> osapdFiles = new List<string>();

        string[] pluginFile = Directory.GetFiles(Common.ApiPath + "\\Plugins", "*.osapd", SearchOption.AllDirectories);
        osapdFiles.AddRange(pluginFile);

        foreach (string path in osapdFiles)
        {
            if (!string.IsNullOrEmpty(path))
            {
                PluginDescription desc = new PluginDescription();
                desc.Deserialize(path);
                pluginNames.Add(desc.Type);
            }
        }

        return pluginNames;
    }

    protected void AvailablePluginsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {        
        if (e.CommandName == "install")
        {
            wcfObj.InstallPluginFromWeb((string)e.CommandArgument);
        }
    }

    protected void AvailablePluginsGridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        
    }

    public void OnMessageReceived(WCF.OSAEWCFMessage message)
    {
        throw new NotImplementedException();
    }
}