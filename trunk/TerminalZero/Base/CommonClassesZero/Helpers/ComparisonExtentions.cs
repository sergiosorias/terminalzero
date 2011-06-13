using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;

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

        public static bool Contains(ISelectable item, object[] dataCriteria)
        {
            return dataCriteria.Any(o =>
                                     {
                                         if (o is string)
                                             return item.Contains((string) o);

                                         if (o is DateTime)
                                             return item.Contains((DateTime) o);

                                         return true;
                                     });
        }
    }
}
