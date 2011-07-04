using System;

namespace UpdatesManager
{
    public abstract class AppVersion
    {
        public const int KRetries = 5;
        public abstract string RootDir { get; protected set; }
        public abstract string Version { get; protected set; }
        public abstract void RunUpdate();

        public abstract event EventHandler<AppVersionUpdateProgress> ProgressChanged;
    }
}