﻿using System;
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
        public const string KInFolderName = "In";

        protected ZeroModule(ITerminal iCurrentTerminal,int code, string description)
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

        private string _workingDirectory = "";
        [DataMember]
        public string WorkingDirectory 
        { 
            get
            {
                return _workingDirectory;
            }
            set
            {
                _workingDirectory = value;
                if (!System.IO.Directory.Exists(_workingDirectory))
                    System.IO.Directory.CreateDirectory(_workingDirectory);

                WorkingDirectoryIn = System.IO.Path.Combine(_workingDirectory, KInFolderName);

                
            }
        }

        private string _workingDirectoryIn = "";

        [DataMember]
        public string WorkingDirectoryIn
        {
            get
            {
                return _workingDirectoryIn;
            }
            private set
            {
                _workingDirectoryIn = value;
                if (!System.IO.Directory.Exists(_workingDirectoryIn))
                    System.IO.Directory.CreateDirectory(_workingDirectoryIn);
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