using System;
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
            return ComparisonExtentions.ContainsIgnoreCase(data.Replace("-", ""), Name1, Name2, _LegalCode != null ? _LegalCode.Replace("-", "") : "");
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
