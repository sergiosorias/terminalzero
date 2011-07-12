using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SLFramework.ViewModel;
using TerminalZeroWebClient.FileTranferReference;
using TerminalZeroWebClient.ServiceHelperReference;

namespace TerminalZeroWebClient.Presentation
{
    public class ImportPageViewModel : ViewModel
    {
        ServiceHelperClient client;
        FileTransferClient _uploadClient = new FileTransferClient();
        
        private DateTime endSelectedDate;

        public DateTime EndSelectedDate
        {
            get { return endSelectedDate; }
            set
            {
                if (endSelectedDate != value)
                {
                    endSelectedDate = value;
                    OnPropertyChanged("EndSelectedDate");
                }
            }
        }

        private DateTime startSelectedDate;

        public DateTime StartSelectedDate
        {
            get { return startSelectedDate; }
            set
            {
                if (startSelectedDate != value)
                {
                    startSelectedDate = value;
                    OnPropertyChanged("StartSelectedDate");
                }
            }
        }


        public ImportPageViewModel()
        {
            client = new ServiceHelperClient();
            //client.GetPackCompleted += client_GetPackCompleted;
            //_uploadClient.UploadFileSilverlightCompleted += _uploadClient_UploadFileSilverlightCompleted;
            //client.GetPackDataCompleted += client_GetPackDataCompleted;
            EndSelectedDate = DateTime.Now.Date;
            StartSelectedDate = DateTime.Now.AddDays(1).Date;
        }

        
    }
}
