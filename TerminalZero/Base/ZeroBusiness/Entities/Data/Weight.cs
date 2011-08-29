using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ZeroBusiness.Exceptions;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    [MetadataType(typeof(WeightMetadata))]
    public partial class Weight : IExportableEntity, IDataErrorInfo
    {
        public Weight()
        {
            Enable = true;
        }

        public Weight(double quantity)
            :this()
        {
            Code = GetNextCode();
            Quantity = quantity;
        }
    
        private static int GetNextCode()
        {
            return BusinessContext.Instance.Model.Weights.Count();
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

    public class WeightMetadata
    {
        [Required(AllowEmptyStrings=false, ErrorMessage="Nombre Obligatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La cantidad tiene que ser mayor a cero")]
        public double Quantity { get; set; }
    }
}
