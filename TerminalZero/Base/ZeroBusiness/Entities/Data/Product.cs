using System;
using System.ComponentModel.DataAnnotations;
using ZeroBusiness.Exceptions;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product : ISelectable, IExportableEntity
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

    public class ProductMetadata
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Código es obligatorio")]
        public string MasterCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nombre es obligatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El Grupo es obligatorio")]
        public int? Group1 { get; set; }
    }
}
