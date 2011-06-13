using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class PaymentInstrument : ISelectable
    {
        public bool Contains(string data)
        {
            return Code.ToString().Contains(data) || Name.Contains(data) || Description.Contains(data);
        }

        public bool Contains(DateTime data)
        {
            throw new NotImplementedException();
        }
        
    }
}
