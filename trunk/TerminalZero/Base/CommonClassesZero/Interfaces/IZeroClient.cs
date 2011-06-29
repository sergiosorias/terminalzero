using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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
        void ShowDialog(object view, Action<bool> result, MessageBoxButton buttons = MessageBoxButton.OKCancel);
        void ShowWindow(object view, Action closed = null);
        void ShowEnable(bool enable);
    }
}
