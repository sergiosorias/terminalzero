using System.ComponentModel;
using System.Xml.Serialization;

namespace ZeroBusiness.Entities.Data
{
    public partial class Price : IDataErrorInfo
    {
        #region IDataErrorInfo Members

        [XmlIgnore]
        public string Error { get; private set; }

        public string this[string columnName]
        {
            get
            {
                Error = "";
                if (columnName == "Value")
                {
                    if (Value <0)
                    {
                        Error = "Número inválido";
                        return Error;
                    }
                }

                return Error;
            }
        }

        #endregion
    }
}
