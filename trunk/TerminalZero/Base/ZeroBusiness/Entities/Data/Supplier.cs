using System;
using System.Xml.Serialization;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;
using System.ComponentModel;

namespace ZeroBusiness.Entities.Data
{
    public partial class Supplier : ISelectable, IDataErrorInfo
    {
        public bool Contains(string data)
        {
            return ComparisonExtentions.ContainsIgnoreCase(data, Name1, Name2);
        }

        public bool Contains(DateTime data)
        {
            throw new NotImplementedException();
        }



        #region IDataErrorInfo Members

        [XmlIgnore]
        public string Error { get; private set; }

        public string this[string columnName]
        {
            get
            {
                Error = "";
                
                if (columnName == "Name1")
                {
                    if (string.IsNullOrEmpty(Name1))
                    {
                        Error = "Nombre obligatorio";
                        return Error;
                    }
                }

                if (columnName == "TaxPositionCode")
                {
                    if (!TaxPositionCode.HasValue)
                    {
                        Error = "Campo obligatorio";
                        return Error;
                    }
                }

                if (columnName == "PaymentInstrumentCode")
                {
                    if (!PaymentInstrumentCode.HasValue)
                    {
                        Error = "Campo obligatorio";
                        return Error;
                    }
                }



                return Error;
            }
        }

        #endregion
    }
}
