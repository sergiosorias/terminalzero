using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ZeroCommonClasses.Files;
using System.ServiceModel.Activation;
using ZeroCommonClasses.Interfaces.Services;
using TZeroHost.Handlers;
using TZeroHost.Classes;

namespace TZeroHost.Services
{
    public class FileTransfer : IFileTransfer
    {
        public static event EventHandler<IncomingPackEventArgs> FileReceived;

        static FileTransfer()
        {
            Helpers.AppDirectories.Init();
        }

        private void OnPackReceived(string fileName, string ConnectionID)
        {
            if (FileReceived != null)
            {
                FileReceived(this, new IncomingPackEventArgs(fileName, ConnectionID));
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
                Console.WriteLine("Sending stream " + request.FileName + " to client");
                Console.WriteLine("Size " + fileInfo.Length);

                // check if exists
                if (!fileInfo.Exists) throw new System.IO.FileNotFoundException("File not found", request.FileName);

                // open stream
                stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }
            else
            {
                using (ZeroCommonClasses.Entities.CommonEntities ent = new ZeroCommonClasses.Entities.CommonEntities())
                {
                    ZeroCommonClasses.Entities.Pack P = ent.Packs.FirstOrDefault(p => p.Name == request.FileName);
                    if(P!=null)
                        stream = new System.IO.MemoryStream(P.Data);
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

            Console.WriteLine("Start uploading " + request.FileName, "Verbose");
            Console.WriteLine("Size " + request.Length, "Verbose");

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
                    Console.WriteLine("Done!", "Verbose");
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
