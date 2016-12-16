using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OSAE;

public partial class permissionError : System.Web.UI.Page
{
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('_');
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Master.Log.Error("A Permission Error has occured.");
    }
}