using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.Interfaces
{
    public interface IProgressNotifier
    {
        void SetProcess(string newProgress);
        void SetProgress(int newProgress);
        void SetUserMessage(bool isMandatory, string message);
        void SendNotification(string message);
        void Log(System.Diagnostics.TraceLevel level, string message);
    }
}
