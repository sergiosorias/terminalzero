using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using ZeroBusiness.Exceptions;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    [MetadataType(typeof(PriceMetadata))]
    public partial class Price : IExportableEntity, IDataErrorInfo
    {
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

    public class PriceMetadata
    {
        [Range(0,double.MaxValue,ErrorMessage="Tiene que ser un valor mayor a cero")]
        public double Value { get; set; }
    }
}
