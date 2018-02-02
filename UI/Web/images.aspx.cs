using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class images : System.Web.UI.Page
{
    // Get current Admin Trust Settings
    OSAEAdmin adSet = OSAEAdminManager.GetAdminSettings();

    public void RaisePostBackEvent(string eventArgument)
    {
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null) Response.Redirect("~/Default.aspx?ReturnUrl=images.aspx");
        int objSet = OSAEAdminManager.GetAdminSettingsByName("Images Trust");
        int tLevel = Convert.ToInt32(Session["TrustLevel"].ToString());
        if (tLevel < objSet) Response.Redirect("~/permissionError.aspx");
        SetSessionTimeout();
        if (!Page.IsPostBack)
            loadImages();
        else
            if (fileUpload.HasFile) txtName.Text = fileUpload.FileName;

        applyObjectSecurity();
    }
    
    private void loadImages()
    {
        gvImages.DataSource = OSAESql.RunSQL("SELECT image_name, image_type, image_width, image_height, image_dpi, image_id FROM osae_images ORDER BY image_name");
        gvImages.DataBind();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (fileUpload.HasFile)
        {
            try
            {
                if (System.IO.Path.GetExtension(fileUpload.FileName).ToLower() != ".jpg" && System.IO.Path.GetExtension(fileUpload.FileName).ToLower() != ".png" && System.IO.Path.GetExtension(fileUpload.FileName).ToLower() != ".jpeg" && System.IO.Path.GetExtension(fileUpload.FileName).ToLower() != ".gif")
                {
                    Master.Log.Error("Image not added, Wrong file type");
                    return; // wrong file type
                }
                else
                {
                    if (fileUpload.PostedFile.ContentLength < 2502400) //202400
                    {
                        OSAEImage img = new OSAEImage();
                        img.Data = fileUpload.FileBytes;
                        img.Name = txtName.Text;
                        img.Type = System.IO.Path.GetExtension(fileUpload.FileName).ToLower().Substring(1);

                        var imageManager = new OSAE.OSAEImageManager();
                        imageManager.AddImage(img);

                        loadImages();
                    }
                    else
                    {
                        Master.Log.Error("Image not added, file is to large.");
                        return; //file to big
                    }
                }
            }
            catch (Exception ex)
            { }
        }
    }

    protected void gvImages_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteImage")
        {
            GridViewRow row = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
            OSAEImageManager imgmrg = new OSAEImageManager();
            imgmrg.DeleteImage(Int32.Parse(gvImages.DataKeys[row.RowIndex].Value.ToString()));

            loadImages();
        }
    }

    #region Trust Settings
    protected void applyObjectSecurity()
    {
        int sessTrust = Convert.ToInt32(Session["TrustLevel"].ToString());
        btnAdd.Enabled = false;
        fileUpload.Enabled = false;
        if (sessTrust >= adSet.ImagesAddTrust)
        {
            btnAdd.Enabled = true;
            fileUpload.Enabled = true;
        }
    }
    #endregion

    protected void gvImages_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int sessTrust = Convert.ToInt32(Session["TrustLevel"].ToString());
        if(sessTrust<adSet.ImagesDeleteTrust)
        {
            e.Row.Cells[6].Enabled = false;

        }
    }

    private void SetSessionTimeout()
    {
        try
        {
            int timeout = 0;
            if (int.TryParse(OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue("Web Server", "Timeout").Value, out timeout))
                Session.Timeout = timeout;
            else Session.Timeout = 60;
        }
        catch (Exception ex)
        {
            Master.Log.Error("Error setting session timeout", ex);
            Response.Redirect("~/error.aspx");
        }
    }
}