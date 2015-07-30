using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class mobile_index : System.Web.UI.Page
{
    private int restPort = 8732;

    protected void Page_Load(object sender, EventArgs e)
    {
        getRestPort();
    }

    private void getRestPort()
    {

        if (!OSAEObjectPropertyManager.GetObjectPropertyValue("Rest", "REST Port").Id.Equals(String.Empty))
        {
            try
            {
                restPort = int.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue("Rest", "REST Port").Value);
            }
            catch (FormatException)
            {
                // do nothing and move on 
            }
            catch (OverflowException)
            {
                // do nothing and move on
            }
            catch (ArgumentNullException)
            {
                // do nothing and move on
            }
        }

        hdnRestPort.Value = restPort.ToString();
    }

}