using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using TerminalZeroRiaWebClient.Web.Classes;
using TerminalZeroRiaWebClient.Web.Helpers;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Files;
using ZeroCommonClasses.Interfaces.Services;

namespace TerminalZeroRiaWebClient.Web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class FileTransfer : IFileTransfer
    {
        public RemoteFileInfo DownloadFile(ServerFileInfo request)
        {
            Stream stream = null;
            long length = 0;
            // get some info about the input file
            if (!request.IsFromDB)
            {
                string filePath = Path.Combine(AppDirectories.DownloadFolder, request.FileName);
                var fileInfo = new FileInfo(filePath);
                length = fileInfo.Length;
                // report start
                Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, "Sending stream " + request.FileName + " to client");
                Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose,"Size " + fileInfo.Length);

                // check if exists
                if (!fileInfo.Exists)
                {
                    Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceError, "File not found");
                    throw new FileNotFoundException("File not found", request.FileName);
                }

                // open stream
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            else
            {
                using (var ent = new CommonEntitiesManager())
                {

                    Pack P = ent.Packs.FirstOrDefault(p => p.Code == request.Code);
                    if (P != null)
                    {
                        Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, "Sending pack " + request.Code + " to client","Verbose");
                        stream = new MemoryStream(P.Data);
                    }
                    else
                    {
                        Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceWarning, "Pack " + request.Code + " Not found", "Verbose");
                    }
                }
            }
            // return result
            var result = new RemoteFileInfo();
            result.FileName = request.FileName;
            result.Length = length;
            result.FileByteStream = stream;
            return result;

            // after returning to the client download starts. Stream remains open and on server and the client reads it, although the execution of this method is completed.
        }

        public ServerFileInfo UploadFile(RemoteFileInfo request)
        {
            // report start

            Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose,"Start uploading " + request.FileName, "Verbose");
            Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, "Size " + request.Length, "Verbose");

            string filePath = Path.Combine(AppDirectories.UploadFolder, Path.GetFileName(request.FileName));
            if (File.Exists(filePath)) File.Delete(filePath);

            ServerFileInfo ret = null;

            int chunkSize = 1024 * 4;
            var buffer = new byte[chunkSize];

            using (var writeStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                try
                {
                    do
                    {
                        // read bytes from input stream
                        int bytesRead = request.FileByteStream.Read(buffer, 0, chunkSize);
                        if (bytesRead == 0) break;

                        // write bytes to output stream
                        writeStream.Write(buffer, 0, bytesRead);
                    } while (true);

                    // report end
                    Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, "Done!", "Verbose");
                    ret = new ServerFileInfo {FileName = filePath};

                    IncomingPackManager.Instance.AddPack(request.ConnectionID, filePath);
                    return ret;
                }
                catch (Exception exe)
                {
                    Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceError, exe);
                    throw;
                }
                finally
                {
                    writeStream.Close();
                }
            }
        }
    }
}
