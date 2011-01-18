using System;
using System.Runtime.Serialization;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroCommonClasses
{
    public enum ModuleStatus
    {
        Unknown = -1,
        Invalid = 0,
        Valid = 1,
        NeedsSync = 2,
    };

    [DataContract]
    public abstract partial class ZeroModule
    {
        public const string kInFolderName = "In";

        public ZeroModule(ITerminal iCurrentTerminal,int code, string description)
        {
            Terminal = iCurrentTerminal;
            ModuleCode = code;
            Description = description;
        }

        [DataMember]
        public int ModuleCode { get; private set; }
        [DataMember]
        public string Description { get; private set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public ModuleStatus TerminalStatus { get; set; }
        [DataMember]
        public ModuleStatus UserStatus { get; set; }
        [DataMember]
        public bool? IsActive { get; set; }

        public event EventHandler<ModuleNotificationEventArgs> Notifing;
        protected void OnModuleNotifing(ModuleNotificationEventArgs args)
        {
            if (Notifing != null)
                Notifing(this, args);
        }

        private string _WorkingDirectory = "";
        [DataMember]
        public string WorkingDirectory 
        { 
            get
            {
                return _WorkingDirectory;
            }
            set
            {
                _WorkingDirectory = value;
                if (!System.IO.Directory.Exists(_WorkingDirectory))
                    System.IO.Directory.CreateDirectory(_WorkingDirectory);

                WorkingDirectoryIn = System.IO.Path.Combine(_WorkingDirectory, kInFolderName);

                
            }
        }

        private string _WorkingDirectoryIn = "";

        [DataMember]
        public string WorkingDirectoryIn
        {
            get
            {
                return _WorkingDirectoryIn;
            }
            private set
            {
                _WorkingDirectoryIn = value;
                if (!System.IO.Directory.Exists(_WorkingDirectoryIn))
                    System.IO.Directory.CreateDirectory(_WorkingDirectoryIn);
            }
        }

        protected ITerminal Terminal { get; private set; }

        public abstract string[] GetFilesToSend();

        /// <summary>
        /// en este momento es donde se cargan y se hacen las cosas necesarias para el modulo
        /// </summary>
        public abstract void Init();

        public virtual void NewPackReceived(string path)
        {
            if (Terminal.Session != null && Terminal.Session.Notifier != null)
                Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Module {0}-{1}, Pack Received {2}", ModuleCode, Description, path));
        }
                
    }
}
