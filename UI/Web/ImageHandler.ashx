<%@ WebHandler Language="C#" Class="ImageHandler" %>

using System;
using System.Web;

public class ImageHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var idParam = context.Request["id"];
        int id = 0;
        int.TryParse(idParam, out id);
        if (id != 0)
        {
            FetchImage(context, id);
        }
    }

    private void FetchImage(HttpContext context, int id)
    {
        var imageManager = new OSAE.OSAEImageManager();
        var image = imageManager.GetImage(id, true);
        if (image != null)
        {
            context.Response.Buffer = true;
            context.Response.Charset = "";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.ContentType = image.Type;
            context.Response.AddHeader("content-disposition", "attachment;filename=" + image.Name);
            context.Response.BinaryWrite(image.Data);
            context.Response.Flush();
            context.Response.End();
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}