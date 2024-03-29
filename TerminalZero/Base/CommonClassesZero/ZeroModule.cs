﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using ZeroCommonClasses.GlobalObjects.Actions;
using System.Linq;

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
    public abstract class ZeroModule
    {
        public const string KInFolderName = "In";

        protected ZeroModule(int code, string description)
        {
            ModuleCode = code;
            Description = description;
            BuildActions(GetType());
        }

        private void BuildActions(Type type)
        {
            bool lookForRules = true;
            foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Union(type.GetMethods(BindingFlags.Public | BindingFlags.Instance)))
            {
                foreach (var attribute in method.GetCustomAttributes(typeof(ZeroActionAttribute), true).Cast<ZeroActionAttribute>())
                {
                    Terminal.Instance.Session.Actions.Add(attribute.GetAction(this,method));
                    lookForRules = false;
                }

                if (lookForRules)
                {
                    foreach (var attribute in method.GetCustomAttributes(typeof (ZeroRuleAttribute), true).Cast<ZeroRuleAttribute>())
                    {
                        Terminal.Instance.Session.Rules.Add(attribute.RuleName, attribute.GetPredicate(this, method));
                    }
                }
                lookForRules = true;
            }
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
        public bool? IsActive { get; set; }

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
                if (!Directory.Exists(_workingDirectory))
                    Directory.CreateDirectory(_workingDirectory);

                WorkingDirectoryIn = Path.Combine(_workingDirectory, KInFolderName);

                
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
                if (!Directory.Exists(_workingDirectoryIn))
                    Directory.CreateDirectory(_workingDirectoryIn);
            }
        }

        /// <summary>
        /// Returns file list to be send to the Server
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetFilesToSend()
        {
            return new string[] {};
        }

        /// <summary>
        /// en este momento es donde se cargan y se hacen las cosas necesarias para el modulo
        /// </summary>
        public abstract void Initialize();

        protected virtual void LoadConfiguration()
        {
            
        }

        public virtual void NewPackReceived(string path)
        {
            if (Terminal.Instance.Session != null && Terminal.Instance.Client.Notifier != null)
                Terminal.Instance.Client.Notifier.Log(TraceLevel.Verbose, string.Format("Module {0}-{1}, Pack Received {2}", ModuleCode, Description, path));
        }
                
    }
}
