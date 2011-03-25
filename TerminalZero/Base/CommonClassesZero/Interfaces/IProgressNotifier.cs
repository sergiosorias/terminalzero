
using System.Diagnostics;

namespace ZeroCommonClasses.Interfaces
{
    public interface IProgressNotifier
    {
        void SetProcess(string newProgress);
        void SetProgress(int newProgress);
        void SetUserMessage(bool isMandatory, string message);
        void SendNotification(string message);
        void Log(TraceLevel level, string message);
    }
}
