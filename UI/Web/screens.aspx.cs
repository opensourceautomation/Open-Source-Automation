using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OSAE;

public partial class screens : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        List<ASP.ctrlStateImage> stateImages = new List<ASP.ctrlStateImage>();
        DataSet ds = OSAESql.RunSQL("SELECT object_name, property_value, image_id FROM osae_v_object_property p INNER JOIN osae_images i ON i.image_name = p.property_value WHERE object_name = '" + Request.QueryString["id"] + "' AND property_name = 'Background Image'");
        try
        {

            string screenName = ds.Tables[0].Rows[0]["object_name"].ToString();

        OSAEObject screen = OSAEObjectManager.GetObjectByName(screenName);
        List<OSAEScreenControl> controls = OSAEScreenControlManager.GetScreenControls(screenName);
        imgBackground.ImageUrl = "~/ImageHandler.ashx?id=" + ds.Tables[0].Rows[0]["image_id"].ToString();
        
        foreach (OSAEScreenControl sc in controls)
        {
            
            if (sc.ControlType == "CONTROL STATE IMAGE")
            {
                // Create instance of the UserControl SimpleControl
                ASP.ctrlStateImage ctrl = (ASP.ctrlStateImage)LoadControl("~/controls/ctrlStateImage.ascx");
                // Set the Public Properties
                ctrl.screenObject = OSAEObjectManager.GetObjectByName(sc.ControlName);
                UpdatePlaceholder.Controls.Add(ctrl);
                //stateImages.Add(ctrl);
            }
            else if (sc.ControlType == "CONTROL METHOD IMAGE")
            {
                // Create instance of the UserControl SimpleControl
                ASP.ctrlClickImage ctrl = (ASP.ctrlClickImage)LoadControl("~/controls/ctrlClickImage.ascx");
                // Set the Public Properties
                ctrl.screenObject = OSAEObjectManager.GetObjectByName(sc.ControlName);
                StaticPlaceholder.Controls.Add(ctrl);
            }
            else if (sc.ControlType == "CONTROL NAVIGATION IMAGE")
            {
                // Create instance of the UserControl SimpleControl
                ASP.ctrlNavigationImage ctrl = (ASP.ctrlNavigationImage)LoadControl("~/controls/ctrlNavigationImage.ascx");
                // Set the Public Properties
                ctrl.screenObject = OSAEObjectManager.GetObjectByName(sc.ControlName);
                StaticPlaceholder.Controls.Add(ctrl);
            }
            else if (sc.ControlType == "CONTROL PROPERTY LABEL")
            {
                // Create instance of the UserControl SimpleControl
                ASP.ctrlPropertyLabel ctrl = (ASP.ctrlPropertyLabel)LoadControl("~/controls/ctrlPropertyLabel.ascx");
                // Set the Public Properties
                ctrl.screenObject = OSAEObjectManager.GetObjectByName(sc.ControlName);
                UpdatePlaceholder.Controls.Add(ctrl);
            }
            else if (sc.ControlType == "CONTROL TIMER LABEL")
            {
                // Create instance of the UserControl SimpleControl
                ASP.ctrlTimerLabel ctrl = (ASP.ctrlTimerLabel)LoadControl("~/controls/ctrlTimerLabel.ascx");
                // Set the Public Properties
                ctrl.screenObject = OSAEObjectManager.GetObjectByName(sc.ControlName);
                UpdatePlaceholder.Controls.Add(ctrl);
            }
            //else if (sc.ControlType == "CONTROL CAMERA VIEWER")
            //{
            //    // Create instance of the UserControl SimpleControl
            //    ASP.ctrlEmbedded ctrl = (ASP.ctrlEmbedded)LoadControl("~/controls/ctrlEmbedded.ascx");
            //    // Set the Public Properties
            //    ctrl.screenObject = OSAEObjectManager.GetObjectByName(sc.ControlName);
            //    ctrl.Source = OSAEObjectManager.GetObjectByName(ctrl.screenObject.Property("Object Name").Value).Property("Stream Address").Value;
            //    ctrl.width = "400";
            //    ctrl.height = "300";
            //    StaticPlaceholder.Controls.Add(ctrl);
            //}
        }
        }
        catch
        {
            return;
        }
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        //foreach (var control in Page.Controls.OfType<ASP.ctrlStateImage>())
        //{
        //    if (OSAEObjectStateManager.GetObjectStateValue(control.ObjectName).Value != control.CurState)
        //    {
        //        control.Update();
        //    }
        //}
    }
}