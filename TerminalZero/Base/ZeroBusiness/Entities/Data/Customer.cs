using System;
using System.ComponentModel;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class Customer : ISelectable
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

        partial void OnName1Changing(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new BusinessValidationException("Nombre obligatorio");
        }

        partial void OnTaxPositionCodeChanging(int? value)
        {
            if (!value.HasValue)
                throw new BusinessValidationException("Campo obligatorio");
        }
        
    }
}
