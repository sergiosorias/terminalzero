
using System;

namespace ZeroCommonClasses.Interfaces
{
    public interface ILogBuilder
    {
        void Add(string log);
        void Add(Exception ex);
    }
}
