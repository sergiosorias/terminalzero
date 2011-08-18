using System;
using System.Collections.Generic;
using ZeroPrinters.Printers;

namespace ZeroPrinters
{
    /// <summary>
    /// Class used to print
    /// </summary>
    public class TerminalPrinters
    {
        #region Singleton
        private TerminalPrinters()
        {
            var info = new PrinterInfo { Name = "Unknown Printer" };
            GeneralPrinter = GeneralPrinter ?? (GeneralPrinter = new General(info));
            TextOnlyPrinter = TextOnlyPrinter ?? (TextOnlyPrinter = new DriverTextOnly(info));
        }

        private static TerminalPrinters _Instance;
        public static TerminalPrinters Instance
        {
            get { return _Instance ?? (_Instance = new TerminalPrinters()); }
        }
        #endregion

        /// <summary>
        /// Printer used to prnt reports and images
        /// </summary>
        public General GeneralPrinter { get; private set; }

        /// <summary>
        /// Printer used to print Legal invoices
        /// </summary>
        public Legal LegalPrinter { get; private set; }

        /// <summary>
        /// Printer Used to print Tickets an Text only reports
        /// </summary>
        public TextOnlyPrinterBase TextOnlyPrinter { get; private set; }

        public string Error { get; private set; }

        /// <summary>
        /// Try to load printer configurations from given configs
        /// </summary>
        /// <param name="printersConfig"></param>
        public bool Load(List<PrinterInfo> printersConfig)
        {
            bool ret = true;

            foreach (PrinterInfo printerInfo in printersConfig)
            {
                switch (printerInfo.Type)
                {
                    case (int)PrinterType.General:
                        GeneralPrinter = new General(printerInfo);
                        break;
                    case (int)PrinterType.Legal:
                        LegalPrinter = new Legal(printerInfo); 
                        break;
                    case (int)PrinterType.TextOnly:
                        TextOnlyPrinter = new DriverTextOnly(printerInfo);
                        //DriverTextOnlyPrinter = new SerialTextOnly(printerInfo.Name, int.Parse(printerInfo.Parameters["Bouds"]), System.IO.Ports.Parity.None, int.Parse(printerInfo.Parameters["DataBits"]), System.IO.Ports.StopBits.One);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            ret = ValidateConfiguration();
            return ret;
        }

        public bool IsNeeded<T>(T printer) 
            where T : SystemPrinter
        {
            bool ret = false;

            if (printer is Legal)
                ret = true;

            return ret;
        }
        
        private bool ValidateConfiguration()
        {
            bool ret = true;
            ret = ValidateLegalPrinter();
            return ret;
        }

        private bool ValidateLegalPrinter()
        {
            bool ret = true;
            if(LegalPrinter == null)
            {
                Error = "Impresora fiscal no configurada";
                ret = false;
            }
            else
            {
                if(!LegalPrinter.IsOnLine)
                {
                    Error = "Error de conexión con Impresora fiscal";
                    ret = false;
                }
            }
            return ret;
        }
    }
}
