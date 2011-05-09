using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ZeroCommonClasses.Interfaces
{
    public interface IMenuCommandSource
    {
        ICommandSource Save { get; }
        ICommandSource Cancel { get; }
        ICommandSource New { get; }
        ICommandSource Print { get; }
    }
}
