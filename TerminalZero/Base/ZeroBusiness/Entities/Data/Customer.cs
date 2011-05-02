using System;
using System.ComponentModel;
using System.Xml.Serialization;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class Customer : ISelectable, IDataErrorInfo
    {

        #region ISelectable Members

        public bool Contains(string data)
        {
            return ComparisonExtentions.ContainsIgnoreCase(data, Name1, Name2);
        }

        public bool Contains(DateTime data)
        {
            throw new NotImplementedException();
        }

        #endregion

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
                        Error = "Nombre obligatorio";
                }

                if (columnName == "TaxPositionCode")
                {
                    if (!TaxPositionCode.HasValue)
                        Error = "Campo obligatorio";
                }
                
                return Error;
            }
        }

        #endregion
    }
}
