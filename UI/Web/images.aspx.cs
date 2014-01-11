using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class images : System.Web.UI.Page
{
    
    public void RaisePostBackEvent(string eventArgument)
    {

    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            loadImages();
        }
    }
    
    //private void CreateDynamicTable()
    //{
    //    OSAEImageManager imgMgr = new OSAE.OSAEImageManager();
    //    List<OSAEImage> images = imgMgr.GetImageList();
    //    PlaceHolder1.Controls.Clear();

    //    // Fetch the number of Rows and Columns for the table 
    //    // using the properties
    //    int tblRows = 0;
    //    int tblCols = 0;
    //    // Create a Table and set its properties 
    //    Table tbl = new Table();
    //    // Add the table to the placeholder control
    //    PlaceHolder1.Controls.Add(tbl);
        
    //    TableRow tr = new TableRow();
    //    foreach(OSAEImage i in images)
    //    {
    //        if(tblCols == 0)
    //        {
    //            tr = new TableRow();
    //        }

    //        TableCell tc = new TableCell();
    //        tc.Width = 200;
    //        tc.Height = 200;
    //        tc.HorizontalAlign = HorizontalAlign.Center;
    //        tc.Style.Add("padding", "10px");
    //        tc.Style.Add("border", "solid");
    //        tc.Style.Add("background-color", "lightgrey");
    //        Image img = new Image();
    //        img.ImageUrl = "imgHandler.aspx?ImageID=" + i.ID.ToString();
    //        img.ID = "img" + i.ID.ToString();
    //        // Add the control to the TableCell
    //        tc.Controls.Add(img);
    //        // Add the TableCell to the TableRow
    //        tr.Cells.Add(tc);
    //        tblCols++;

    //        if (tblCols == 4)
    //        {
    //            tbl.Rows.Add(tr);
    //            tblCols = 0;
    //        }
    //    }


    //}


    private void loadImages()
    {

        gvImages.DataSource = OSAESql.RunSQL("SELECT image_name, image_type, image_id FROM osae_images ORDER BY image_name");
        gvImages.DataBind();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (fileUpload.HasFile)
        {
            try
            {
                if (System.IO.Path.GetExtension(fileUpload.FileName).ToLower() != ".jpg" && System.IO.Path.GetExtension(fileUpload.FileName).ToLower() != ".png" && System.IO.Path.GetExtension(fileUpload.FileName).ToLower() != ".jpeg")
                {
                    // wrong file type
                    return;
                }
                else
                {
                    if (fileUpload.PostedFile.ContentLength < 102400)
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
                        // file to big
                        return;
                    }

                }
            }
            catch (Exception ex)
            {

            }
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
}