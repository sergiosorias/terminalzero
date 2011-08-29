using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;
using System.ComponentModel;
using System.Linq;

namespace ZeroBusiness.Entities.Data
{
    [MetadataType(typeof(SupplierMetadata))]
    public partial class Supplier : ISelectable, IExportableEntity, IDataErrorInfo
    {
        #region ISelectable
        public bool Contains(string data)
        {
            return ComparisonExtentions.ContainsIgnoreCase(data, Name1, Name2);
        }

        public bool Contains(DateTime data)
        {
            throw new NotImplementedException();
        }
        #endregion
        
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

        #region IDataErrorInfo

        public string this[string columnName]
        {
            get { return ContextExtentions.ValidateProperty(this, columnName); }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
       
    }

    public class SupplierMetadata
    {
        [Required(AllowEmptyStrings=false,ErrorMessage="Nombre Obligatorio")]
        public string Name1 { get; set; }

        [Required(ErrorMessage="Forma de pago obligatoria")]
        public int? PaymentInstrumentCode { get; set; }

        [Required(ErrorMessage = "Campo obligatoria")]
        public int? TaxPositionCode { get; set; }
    }
}
