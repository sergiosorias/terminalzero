using System;

namespace ZeroCommonClasses.Interfaces
{
    public interface ITerminalManager
    {
        event EventHandler ConfigurationRequired;
        bool InitializeTerminal();
    }
}
