using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using TZeroHost.Handlers;
using TZeroHost.Helpers;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Files;
using ZeroCommonClasses.Interfaces.Services;

namespace TZeroHost.Services
{
    public class FileTransfer : IFileTransfer
    {
        public static event EventHandler<IncomingPackEventArgs> FileReceived;

        static FileTransfer()
        {
            AppDirectories.Init();
        }

        private void OnPackReceived(string fileName, string connectionId)
        {
            if (FileReceived != null)
            {
                FileReceived(this, new IncomingPackEventArgs(fileName, connectionId));
            }
        }

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
                Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose, "Sending stream " + request.FileName + " to client");
                Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose,"Size " + fileInfo.Length);

                // check if exists
                if (!fileInfo.Exists)
                {
                    Trace.WriteLineIf(ContextInfo.LogLevel.TraceError, "File not found");
                    throw new FileNotFoundException("File not found", request.FileName);
                }

                // open stream
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            else
            {
                using (var ent = new ZeroCommonClasses.Entities.CommonEntitiesManager())
                {

                    Pack P = ent.Packs.FirstOrDefault(p => p.Code == request.Code);
                    if (P != null)
                    {
                        Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose, "Sending pack " + request.Code + " to client","Verbose");
                        stream = new MemoryStream(P.Data);
                    }
                    else
                    {
                        Trace.WriteLineIf(ContextInfo.LogLevel.TraceWarning, "Pack " + request.Code + " Not found", "Verbose");
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

            Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose,"Start uploading " + request.FileName, "Verbose");
            Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose, "Size " + request.Length, "Verbose");

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
                    Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose, "Done!", "Verbose");
                    ret = new ServerFileInfo {FileName = filePath};

                    OnPackReceived(filePath, request.ConnectionID);

                    return ret;
                }
                catch (Exception exe)
                {
                    Trace.WriteLineIf(ContextInfo.LogLevel.TraceError, exe);
                    throw;
                }
                finally
                {
                    writeStream.Close();
                }
            }
        }

        public string UploadFileSilverlight(string fileName, byte[] fileByteStream)
        {
            Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose, "Start web uploading " + fileName, "Verbose");
            Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose, "Size " + fileByteStream.Length, "Verbose");
            string destFileName = Path.GetFileNameWithoutExtension(fileName)+"_"+DateTime.Now.ToString("yyyyMMddhhmmss")+Path.GetExtension(fileName);
            string filePath = Path.Combine(AppDirectories.UploadFolder, destFileName);
            if (File.Exists(filePath)) File.Delete(filePath);

            using (var writeStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                try
                {
                    writeStream.Write(fileByteStream, 0, fileByteStream.Length);

                    // report end
                    Trace.WriteLineIf(ContextInfo.LogLevel.TraceVerbose, "Done!", "Verbose");
                    OnPackReceived(filePath, null);
                    return destFileName;
                }
                catch (Exception exe)
                {
                    Trace.WriteLineIf(ContextInfo.LogLevel.TraceError, exe);
                }
                finally
                {
                    writeStream.Close();
                }
                return "";
            }
        }
    }
}
