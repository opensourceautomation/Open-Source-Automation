using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OSAE;

public partial class imgHandler : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["ImageID"] != null)
        {
            DataTable dt = OSAESql.RunSQL("SELECT image_data, image_type, image_name FROM osae_images WHERE image_id = " + Request.QueryString["ImageID"]).Tables[0];
            
            if (dt != null)
            {
                Byte[] bytes = (Byte[])dt.Rows[0]["image_data"];
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = dt.Rows[0]["image_type"].ToString();
                Response.AddHeader("content-disposition", "attachment;filename=" + dt.Rows[0]["image_name"].ToString());
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
        }
    }
}