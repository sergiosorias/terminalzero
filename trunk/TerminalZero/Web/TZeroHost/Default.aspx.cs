using System;
using System.IO;

namespace TZeroHost
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                filesToShow.Value = SetFilesToDownload();
            }
            catch (Exception)
            {
                filesToShow.Value = "Error";
            }
            
        }

        private string SetFilesToDownload()
        {
            string upFolder = MapPath("./UPLOAD");
            DirectoryInfo dir = new DirectoryInfo(upFolder);

            string value = "";
            foreach (FileInfo info in dir.GetFiles())
            {
                value += string.Format("{0}|", info.Name);
            }

            return value;
        }
    }
}