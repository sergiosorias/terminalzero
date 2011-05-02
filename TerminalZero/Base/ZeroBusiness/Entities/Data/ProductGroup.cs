using System.ComponentModel;
using System.Xml.Serialization;

namespace ZeroBusiness.Entities.Data
{
    public partial class ProductGroup : IDataErrorInfo
    {

        #region IDataErrorInfo Members

        [XmlIgnore]
        public string Error { get; private set; }

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

                return Error;
            }
        }

        #endregion
    }
}
