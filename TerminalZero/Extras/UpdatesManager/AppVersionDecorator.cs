using System;

namespace UpdatesManager
{
    public class AppVersionDecorator : AppVersion
    {
        public override string RootDir
        {
            get { return AppVersion.RootDir; }
            protected set { /*AppVersion.RootDir = value;*/ }
        }

        public override string Version
        {
            get { return AppVersion.Version; }
            protected set { /*AppVersion.Version = value;*/ }
        }

        public override event EventHandler<AppVersionUpdateProgress> ProgressChanged;
        
        protected void InvokeProgressChanged(AppVersionUpdateProgress e)
        {
            EventHandler<AppVersionUpdateProgress> handler = ProgressChanged;
            if (handler != null) handler(this, e);
        }

        protected AppVersion AppVersion {get; private set;}

        public AppVersionDecorator(AppVersion appVersion)
        {
            AppVersion = appVersion;
            AppVersion.ProgressChanged += (o, e) => InvokeProgressChanged(e);
        }

        #region Overrides of AppVersion

        public override void RunUpdate()
        {
            AppVersion.RunUpdate();
        }

        #endregion
    }
}