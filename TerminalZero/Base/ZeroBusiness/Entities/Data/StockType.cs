using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroBusiness.Entities.Data
{
    public partial class StockType
    {
        public enum Types
        {
            New = 0,
            Modify = 1,
        }

        public Types CodeTypes
        {
            get
            {
                return (Types)Code;
            }
            set
            {
                Code = (int)value;
            }
        } 
    }
}
