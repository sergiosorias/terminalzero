using System;
using ZeroCommonClasses.Entities;

namespace ZeroCommonClasses.Interfaces
{
    public interface IExportableEntity
    {
        int TerminalDestination { get; }
        DateTime? Stamp { get; set; }
        Int16 Status { get; set; }
        void UpdateStatus(EntityStatus status);
    }
}
