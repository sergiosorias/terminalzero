using System;
using System.Linq;
using ZeroCommonClasses.Files;
using ZeroCommonClasses.Interfaces.Services;
using TZeroHost.Handlers;

namespace TZeroHost.Services
{
    public class FileTransfer : IFileTransfer
    {
        public static event EventHandler<IncomingPackEventArgs> FileReceived;

        static FileTransfer()
        {
            Helpers.AppDirectories.Init();
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
            System.IO.Stream stream = null;
            long length = 0;
            // get some info about the input file
            if (!request.IsFromDB)
            {
                string filePath = System.IO.Path.Combine(Helpers.AppDirectories.DownloadFolder, request.FileName);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                length = fileInfo.Length;
                // report start
                System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose, "Sending stream " + request.FileName + " to client");
                System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose,"Size " + fileInfo.Length);

                // check if exists
                if (!fileInfo.Exists)
                {
                    System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceError, "File not found");
                    throw new System.IO.FileNotFoundException("File not found", request.FileName);
                }

                // open stream
                stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }
            else
            {
                using (ZeroCommonClasses.Entities.CommonEntities ent = new ZeroCommonClasses.Entities.CommonEntities())
                {

                    ZeroCommonClasses.Entities.Pack P = ent.Packs.FirstOrDefault(p => p.Code == request.Code);
                    if (P != null)
                    {
                        System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose, "Sending pack " + request.Code + " to client");
                        stream = new System.IO.MemoryStream(P.Data);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceWarning, "Pack " + request.Code + " Not found");
                    }
                }
            }
            // return result
            RemoteFileInfo result = new RemoteFileInfo();
            result.FileName = request.FileName;
            result.Length = length;
            result.FileByteStream = stream;
            return result;

            // after returning to the client download starts. Stream remains open and on server and the client reads it, although the execution of this method is completed.
        }

        public ServerFileInfo UploadFile(RemoteFileInfo request)
        {
            // report start

            System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose,"Start uploading " + request.FileName, "Verbose");
            System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose, "Size " + request.Length, "Verbose");

            string filePath = System.IO.Path.Combine(Helpers.AppDirectories.UploadFolder, System.IO.Path.GetFileName(request.FileName));
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            ServerFileInfo ret = null;

            int chunkSize = 1024 * 4;
            byte[] buffer = new byte[chunkSize];

            using (System.IO.FileStream writeStream = new System.IO.FileStream(filePath, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write))
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
                    System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose, "Done!", "Verbose");
                    ret = new ServerFileInfo { FileName = filePath };

                    OnPackReceived(filePath,request.ConnectionID);

                    return ret;
                }
                catch (Exception exe)
                {
                    Console.WriteLine(exe);
                    throw exe;
                }
                finally
                {
                    writeStream.Close();
                }

                
            }

        }
    }
}
