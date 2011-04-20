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
    }
}
