using System;
using ZeroCommonClasses.Entities;

namespace ZeroCommonClasses.Interfaces
{
    public interface IExportableEntity
    {
        int TerminalDestination { get; }
        Nullable<global::System.DateTime> Stamp { get; set; }
        global::System.Int16 Status { get; set; }
        void UpdateStatus(EntityStatus status);
    }
}
