using System;
using System.IO;

namespace TerminalZeroRiaWebClient.Web
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
            DirectoryInfo dir = new DirectoryInfo(Helpers.AppDirectories.UploadFolder);

            string value = "";
            foreach (FileInfo info in dir.GetFiles())
            {
                value += string.Format("{0}|", info.Name);
            }

            return value;
        }

        protected void btnUploadClick(object sender, EventArgs e)
        {
            if(FileUpload1.HasFile)
            {
                FileUpload1.SaveAs(Path.Combine(Helpers.AppDirectories.DownloadFolder, Path.GetFileName(FileUpload1.FileName)));
            }
        }
    }
}