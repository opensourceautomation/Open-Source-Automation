using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OSAE;

public partial class images : System.Web.UI.Page
{
    Logging logging = Logging.GetLogger("Web UI");

    public void RaisePostBackEvent(string eventArgument)
    {

    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        loadImages();
    }

    protected void gvImages_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
    {

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


    protected void btnDelete_Click(object sender, EventArgs e)
    {
        
        try
        {
            Button btn = (Button)sender;
            int ID = 0;

            foreach (GridViewRow rw in gvImages.Rows)
            {
                Button b = (Button)rw.FindControl("btnDelete");
                if (btn.CssClass == b.CssClass)
                {
                    ID = Int32.Parse(((Label)rw.FindControl("lblID")).Text);
                }
            }

            OSAEImageManager imgmrg = new OSAEImageManager();
            imgmrg.DeleteImage(ID);

            loadImages();
        }
        catch (Exception ex)
        {
            logging.AddToLog("Error deleting image: " + ex.Message + " Inner Exception: " + ex.InnerException, true);
        }
    }

    private void loadImages()
    {

        gvImages.DataSource = OSAESql.RunSQL("SELECT image_name, image_type, image_id FROM osae_images");
        gvImages.DataBind();
    }

}