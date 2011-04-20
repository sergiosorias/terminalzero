using System;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;

namespace ZeroStock.Entities
{
    public partial class Supplier : ISelectable
    {
        public bool Contains(string data)
        {
            return ComparisonExtentions.ContainsIgnoreCase(data, Name1, Name2);
        }

        public bool Contains(DateTime data)
        {
            throw new NotImplementedException();
        }
    }
}
