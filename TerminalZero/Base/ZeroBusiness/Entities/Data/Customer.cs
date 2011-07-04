using System;
using System.ComponentModel.DataAnnotations;
using ZeroBusiness.Exceptions;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer : ISelectable, IExportableEntity
    {
        internal Customer()
        {
            
        }

        public Customer(int terminalCode)
        {
            Code = BusinessContext.Instance.Model.GetNextCustomerCode();
            TerminalCode = terminalCode;
            Enable = true;
            UpdateStatus(EntityStatus.New);
        }
        

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

        #region Implementation of IExportableEntity

        public int TerminalDestination
        {
            get { return 0; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }

        #endregion
        
    }

    public class CustomerMetadata
    {
        [Required(AllowEmptyStrings=false)]
        public object WebSite;
    }
}
