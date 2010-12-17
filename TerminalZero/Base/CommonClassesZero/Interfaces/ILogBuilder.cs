
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.Interfaces
{
    public interface ILogBuilder
    {
        void SetState();
        void Add(string log);
        void Add(Exception ex);
    }
}
