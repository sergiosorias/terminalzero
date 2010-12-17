using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.Interfaces
{
    public interface IProgressNotifier
    {
        event EventHandler ExecutionFinished;
        void SetProcess(string newProgress);
        void SetProgress(int newProgress);
        void SetUserMessage(bool isMandatory, string message);
        void SendUserMessage(string message);
        void NotifyExecutionFinished(object sender);
    }
}
