using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class ReturnReason : IExportableEntity
    {
        internal ReturnReason()
        {
            
        }

        public ReturnReason(int terminalCode)
        {
            Code = BusinessContext.Instance.Model.GetNextReturnReasonCode();
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
}
