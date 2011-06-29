using System;

namespace UpdatesManager
{
    public class AppVersionUpdateProgress : EventArgs
    {
        public bool HasError { get; set; }
        public bool CanRetry { get; set; }
        public string Message { get; set; }
    }
}