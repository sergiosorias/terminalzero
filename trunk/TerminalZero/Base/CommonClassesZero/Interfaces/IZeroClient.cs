using System;
using System.Collections.Generic;
using ZeroCommonClasses.GlobalObjects;

namespace ZeroCommonClasses.Interfaces
{
    public interface IZeroClient : IDisposable
    {
        event EventHandler Loaded;
        IProgressNotifier Notifier { get; set; }
        List<ZeroModule> ModuleList { get; }
        ZeroMenu MainMenu { get; }
        bool Initialize();
        void Load();
        void ShowView(object view);
        void ShowDialog(object view,string title = null, Action<bool> result = null, MessageBoxButtonEnum buttons = MessageBoxButtonEnum.OkCancel);
        void ShowWindow(object view, Action closed = null);
        void ShowEnable(bool enable);
    }
}
