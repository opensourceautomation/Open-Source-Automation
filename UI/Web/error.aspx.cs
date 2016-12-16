using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;
public partial class error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Exception exceptionDetails = Server.GetLastError();

        if (exceptionDetails != null)
        {
            errorDetailTextBox.Text = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Source: " + exceptionDetails.Source);
            sb.AppendLine("Message: " + exceptionDetails.Message);
            sb.AppendLine("Inner Exception: " + exceptionDetails.InnerException);
            sb.AppendLine("Stack Trace: " + exceptionDetails.StackTrace);
            errorDetailTextBox.Text = sb.ToString();
            Server.ClearError();
        }
        else 
        {
            string unError = "There was an Unknown error that orrcured on the previous page." + Environment.NewLine + "Please try again." + Environment.NewLine + "If you continue to get the same Error, please see resolutions at bottom of this page.";
            errorDetailTextBox.Text = unError;
        }
    }    
}