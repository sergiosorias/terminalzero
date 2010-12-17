using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ZeroCommonClasses.Files;

namespace ZeroCommonClasses.Interfaces.Services
{
    [ServiceContract]
    public interface IFileTransfer
    {
        [OperationContract]
        RemoteFileInfo DownloadFile(ServerFileInfo request);

        [OperationContract]
        ServerFileInfo UploadFile(RemoteFileInfo request);
    }
}
