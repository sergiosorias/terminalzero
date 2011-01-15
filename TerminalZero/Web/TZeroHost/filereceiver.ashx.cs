using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using TZeroHost.Services;

namespace TZeroHost
{
    /// <summary>
    /// Summary description for filereceiver
    /// </summary>
    public class filereceiver : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string filename = context.Request.QueryString["filename"].ToString();
            FileTransfer trf = new FileTransfer();
            trf.UploadFile(new ZeroCommonClasses.Files.RemoteFileInfo
            {
                ConnectionID = "",
                FileByteStream = context.Request.InputStream,
                FileName = filename,
                Length = context.Request.InputStream.Length
            });
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}