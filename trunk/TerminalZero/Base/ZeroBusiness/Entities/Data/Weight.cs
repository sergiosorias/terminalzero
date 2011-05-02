using System.ComponentModel;
using System.Xml.Serialization;

namespace ZeroBusiness.Entities.Data
{
    public partial class Weight : IDataErrorInfo
    {
        #region Implementation of IDataErrorInfo

        public string this[string columnName]
        {
            get
            {
                Error = "";
                if (columnName == "Name")
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        Error = "Nombre Obligatorio";
                        return Error;
                    }
                }

                if (columnName == "Quantity")
                {
                    if (Quantity == 0)
                    {
                        Error = "Cantidad obligatoria";
                        return Error;
                    }
                }

                return Error;
            }
        }

        [XmlIgnore]
        public string Error { get; private set; }

        #endregion
    }
}
