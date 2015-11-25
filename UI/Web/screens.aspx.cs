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

    private int restPort = 8732;
    public string gScreen;
    private OSAEImageManager imgMgr = new OSAEImageManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        getRestPort();
        gScreen = Request.QueryString["id"];
        try
        {
            OSAEObject screen = OSAEObjectManager.GetObjectByName(gScreen);
            //List<OSAEScreenControl> controls = OSAEScreenControlManager.GetScreenControls(gScreen);
            OSAEObjectCollection screenObjects = OSAEObjectManager.GetObjectsByContainer(gScreen);
            string sImg = OSAEObjectPropertyManager.GetObjectPropertyValue(gScreen, "Background Image").Value.ToString();
            OSAEImage img = imgMgr.GetImage(sImg);
            imgBackground.ImageUrl = "~/ImageHandler.ashx?id=" + img.ID;
            foreach (OSAEObject obj in screenObjects)
            {
                LoadControl(obj);            
            }
        }
        catch
        {
            return;
        }
    }

    private void LoadControl(OSAEObject obj)
    {
        #region State Image
        if (obj.Type == "CONTROL STATE IMAGE")
        {
            // Create instance of the UserControl SimpleControl
            ASP.ctrlStateImage ctrl = (ASP.ctrlStateImage)LoadControl("~/controls/ctrlStateImage.ascx");
            // Set the Public Properties
            ctrl.screenObject = OSAEObjectManager.GetObjectByName(obj.Name);
            UpdatePlaceholder.Controls.Add(ctrl);
            //stateImages.Add(ctrl);
        }
        #endregion

        #region Click Image
        else if (obj.Type == "CONTROL CLICK IMAGE")
        {
            // Create instance of the UserControl SimpleControl
            ASP.ctrlClickImage ctrl = (ASP.ctrlClickImage)LoadControl("~/controls/ctrlClickImage.ascx");
            // Set the Public Properties
            ctrl.screenObject = OSAEObjectManager.GetObjectByName(obj.Name);
            StaticPlaceholder.Controls.Add(ctrl);
        }
        #endregion

        #region Navigation Image
        else if (obj.Type == "CONTROL NAVIGATION IMAGE")
        {
            // Create instance of the UserControl SimpleControl
            ASP.ctrlNavigationImage ctrl = (ASP.ctrlNavigationImage)LoadControl("~/controls/ctrlNavigationImage.ascx");
            // Set the Public Properties
            ctrl.screenObject = OSAEObjectManager.GetObjectByName(obj.Name);
            StaticPlaceholder.Controls.Add(ctrl);
        }
        #endregion

        #region Property Label
        else if (obj.Type == "CONTROL PROPERTY LABEL")
        {
            // Create instance of the UserControl SimpleControl
            ASP.ctrlPropertyLabel ctrl = (ASP.ctrlPropertyLabel)LoadControl("~/controls/ctrlPropertyLabel.ascx");
            // Set the Public Properties
            ctrl.screenObject = OSAEObjectManager.GetObjectByName(obj.Name);
            UpdatePlaceholder.Controls.Add(ctrl);
        }
        #endregion

        #region Timer Label
        else if (obj.Type == "CONTROL TIMER LABEL")
        {
            // Create instance of the UserControl SimpleControl
            ASP.ctrlTimerLabel ctrl = (ASP.ctrlTimerLabel)LoadControl("~/controls/ctrlTimerLabel.ascx");
            // Set the Public Properties
            ctrl.screenObject = OSAEObjectManager.GetObjectByName(obj.Name);
            UpdatePlaceholder.Controls.Add(ctrl);
        }
        #endregion

        #region Camera Viewer
        else if (obj.Type == "CONTROL CAMERA VIEWER")
        {
            // Create instance of the UserControl SimpleControl
            ASP.ctrlEmbedded ctrl = (ASP.ctrlEmbedded)LoadControl("~/controls/ctrlEmbedded.ascx");
            // Set the Public Properties
            ctrl.screenObject = OSAEObjectManager.GetObjectByName(obj.Name);
            //ctrl.initialize();
            ctrl.Source = OSAEObjectManager.GetObjectByName(ctrl.screenObject.Property("Object Name").Value).Property("Stream Address").Value;
            ctrl.width = "400";
            ctrl.height = "300";
            StaticPlaceholder.Controls.Add(ctrl);
        }
        #endregion

        #region User Control
        else if (obj.BaseType == "USER CONTROL")
        {
            string sUCType = obj.Property("Control Type").Value;
            string ucName = sUCType.Replace("USER CONTROL ", "");
            // Create instance of the UserControl SimpleControl
            dynamic ctrl = LoadControl("~/controls/usercontrols/" + ucName + "/ctrlUserControl.ascx") as System.Web.UI.UserControl;
            // Set the Public Properties
            ctrl.screenObject = obj;
            ctrl.initialize();
            UpdatePlaceholder.Controls.Add(ctrl);
        }
        #endregion
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