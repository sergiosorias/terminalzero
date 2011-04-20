using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;

namespace ZeroMasterData.Entities
{
    public partial class Product : ISelectable
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
    }
}
