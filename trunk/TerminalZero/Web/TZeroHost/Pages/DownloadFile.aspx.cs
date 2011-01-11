using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GeoWebServer.Frames
{
    public partial class DownloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.ContentType = "application/vnd.ms-excel";
            //System.IO.Stream ss = null;
            //long FileLenght = 0;
            //string fileName="";
            //try
            //{
            //    int chunkSize = 1024 * 4;

            //    byte[] buffer = new byte[chunkSize];
            //    switch (Request.QueryString["FNAME"])
            //    {
            //        case "TEMPLATE":
            //            Response.AppendHeader("content-disposition", "attachment; filename=Template.xls");
            //            GetTemplateFile(out fileName, out FileLenght, out ss); 
            //            break;
            //        case "EXPORT":
            //            GetExportFile(out fileName, out FileLenght, out ss);
            //            Response.AppendHeader("content-disposition", "attachment; filename=" + fileName);
            //            break;
            //        default:
            //            break;
            //    }

            //    if (ss != null)
            //    {
            //        while (true)
            //        {
            //            int bytesRead = ss.Read(buffer, 0, chunkSize);
            //            if (bytesRead == 0) break;
            //            Response.OutputStream.Write(buffer, 0, bytesRead);

            //        }

            //        Response.Flush();
            //        Response.Close();
            //    }


            //}
            //catch (Exception ex)
            //{
                
            //}
            //finally
            //{
            //    try
            //    {
            //        if(ss!=null)
            //            ss.Close();
            //    }
            //    catch { }
            //}
        }
    }
}
