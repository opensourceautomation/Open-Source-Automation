using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;


public partial class plugins : System.Web.UI.Page
{
    private BindingList<PluginDescription> pluginList = new BindingList<PluginDescription>();

    protected void Page_Load(object sender, EventArgs e)
    {
        loadPlugins();
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
                        desc.Status = o.State.Value;
                    }
                }
                pluginList.Add(desc);
                //logging.AddToLog("Plugin found: Name:" + desc.Name + " Desc ID: " + desc.ID, true);
            }
        }
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
        }
    }
}