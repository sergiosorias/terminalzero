using System.Linq;
using System.Printing;
using System.Windows.Controls;

namespace ZeroPrinters.Printers
{
    public class General : SystemPrinter
    {
        protected PrintDialog Dialog { get; set; }

        public General(PrinterInfo info)
            :base(info)
        {
            IsOnLine = true;
        }

        public bool LoadPrintDialog(out PrintDialog dialog)
        {
            if (Dialog == null)
            {
                dialog = new PrintDialog();
                var printServer = new LocalPrintServer();
                PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues();
                dialog.PrintQueue = printQueuesOnLocalServer.FirstOrDefault(printer => printer.Name == Name);
                Dialog = dialog;
                
            }
            else
            {
                dialog = Dialog;
            }

            return Dialog.PrintQueue != null;
        }

        #region Overrides of SystemPrinter

        public override bool IsOnLine
        {
            get;
            protected set;
        }

        #endregion
    }
}