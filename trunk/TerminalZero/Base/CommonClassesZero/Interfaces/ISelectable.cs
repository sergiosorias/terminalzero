using System;

namespace ZeroCommonClasses.Interfaces
{
    public interface ISelectable
    {
        bool Contains(string data);
        bool Contains(DateTime data);
    }
}
