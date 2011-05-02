using System;
using System.Xml.Serialization;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;
using System.ComponentModel;

namespace ZeroBusiness.Entities.Data
{
    public partial class Product : ISelectable, IDataErrorInfo
    {
        #region ISelectable Members

        public bool Contains(string data)
        {
            return ComparisonExtentions.ContainsIgnoreCase(data, Name, Description, ShortDescription);
        }

        public bool Contains(DateTime data)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDataErrorInfo Members

        [XmlIgnore]
        public string Error { get; private set; }

        partial void OnMasterCodeChanged()
        {
            if (string.IsNullOrWhiteSpace(MasterCode))
            {
                Error = "Código obligatorio";
            }
        }

        partial void OnNameChanged()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Error = "Nombre obligatorio";
            }
        }

        partial void OnGroup1Changed()
        {
            if (!Group1.HasValue)
            {
                Error = "Grupo obligatorio";
            }
        }

        public string this[string columnName]
        {
            get
            {
                Error = "";
                if (columnName == "MasterCode")
                {
                    OnMasterCodeChanged();
                }

                if (columnName == "Name")
                {
                    OnNameChanged();
                }

                if (columnName == "Group1")
                {
                    OnGroup1Changed();
                }

                return Error;
            }
        }

        #endregion
       
    }
}
