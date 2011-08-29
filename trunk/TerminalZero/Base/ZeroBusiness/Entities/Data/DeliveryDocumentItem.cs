using System;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class DeliveryDocumentItem : IExportableEntity
    {
        #region IExportableEntity
        public int TerminalDestination
        {
            get { return TerminalToCode; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }
        #endregion
    }
}