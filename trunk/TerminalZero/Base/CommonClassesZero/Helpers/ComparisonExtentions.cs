using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.Helpers
{
    public static class ComparisonExtentions
    {
        public static bool ContainsIgnoreCase(string criteria, params object[] values)
        {
            return values.Any(value => value!=null && value.ToString().IndexOf(criteria, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        public static bool ContainsType(Type typeOrigin, Type target)
        {
            if (typeOrigin == null || target == null)
                return false;

            return typeOrigin == target || ContainsType(typeOrigin.BaseType, target);
        }
    }
}
