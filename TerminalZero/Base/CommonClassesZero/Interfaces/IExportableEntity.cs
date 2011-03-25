using ZeroCommonClasses.Entities;

namespace ZeroCommonClasses.Interfaces
{
    public interface IExportableEntity
    {
        int TerminalDestination { get; }

        void UpdateStatus(EntityStatus status);
    }
}
