using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiscalPrinterLib;

namespace ZeroPrinters.Extras
{
    public enum CustomerResponsibility
    {
        /// <summary>
        /// Consumidor Final
        /// </summary>
        NoTaxPayer,
        /// <summary>
        /// Single tax system taxpayer (Monotributista)
        /// </summary>
        STS_TaxPayer,
        /// <summary>
        /// Inscripto
        /// </summary>
        TaxPayer
    }

    public enum IdentificationType
    {
        DNI,
        CUIL,
        CUIT,
    }

    public class CustomerInfo
    {
        public CustomerResponsibility TaxPosition { get; set; }

        public IdentificationType DNIType { get; set; }

        public string UniqueID { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }
    }
}
