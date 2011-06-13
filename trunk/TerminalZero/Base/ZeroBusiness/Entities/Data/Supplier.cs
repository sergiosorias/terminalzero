using System;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;
using System.ComponentModel;
using System.Linq;

namespace ZeroBusiness.Entities.Data
{
    public partial class Supplier : ISelectable
    {
        public bool Contains(string data)
        {
            return ComparisonExtentions.ContainsIgnoreCase(data, Name1, Name2);
        }

        public bool Contains(DateTime data)
        {
            throw new NotImplementedException();
        }
        
        partial void OnName1Changing(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new BusinessValidationException("Nombre Obligatorio");

            }
        }

        partial void OnTaxPositionCodeChanging(int? value)
        {
            if (!value.HasValue)
            {
                throw new BusinessValidationException("Campo obligatorio");
            }
        }

        partial void OnPaymentInstrumentCodeChanging(int? value)
        {
            if (!value.HasValue)
            {
                throw new BusinessValidationException("Campo obligatorio");
            }
        }
       
    }
}
