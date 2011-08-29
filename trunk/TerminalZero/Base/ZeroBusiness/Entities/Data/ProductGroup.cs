using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    [MetadataType(typeof(ProductGroupMetadata))]
    public partial class ProductGroup : IExportableEntity, IDataErrorInfo
    {
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

        public string this[string columnName]
        {
            get { return ContextExtentions.ValidateProperty(this, columnName); }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class ProductGroupMetadata
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nombre es obligatorio")]
        public string Name { get; set; }
    }
}
