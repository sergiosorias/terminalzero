using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.Interfaces
{
    public interface ISelectable
    {
        bool Contains(string data);
        bool Contains(DateTime data);
    }
}
