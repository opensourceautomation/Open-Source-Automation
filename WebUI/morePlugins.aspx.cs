using ICSharpCode.SharpZipLib.Zip;
using OSAE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class morePlugins : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetPlugins();
        }
    }
       
    private void GetPlugins()
    {
        string url = "http://www.opensourceautomation.com/checkPluginUpdates.php";
        WebRequest request = HttpWebRequest.Create(url);
        WebResponse response = request.GetResponse();
        Stream responseStream = response.GetResponseStream();

        XElement xml = XElement.Load(responseStream);
        
        var query = from e in xml.Elements("plugin")
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

        AvailablePluginsGridView.DataSource = query;
        AvailablePluginsGridView.DataBind();
    }

    protected void AvailablePluginsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //if (e.CommandName == "install")
        //{
        //    PluginInstaller pi = new PluginInstaller();
        //    pi.DownloadAndInstallPlugin((string)e.CommandArgument);
        //}
    }

    protected void AvailablePluginsGridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        
    }
}