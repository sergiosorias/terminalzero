﻿using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using ZeroCommonClasses.Interfaces.Services;

namespace ZeroCommonClasses.Context
{
    public static class ConfigurationContext
    {
        public static TraceSwitch LogLevel { get; private set; }
        public static bool IsOnServer { get; private set; }
        public static ConnectionStringSettings ServerConnectionString { get; private set; }
        public static ConnectionStringSettings ClientConnectionString { get; private set; }
        public static ConnectionStringSettings UsersConnectionString { get; private set; }
        public static int TerminalCode { get; private set; }
        public static string TerminalName { get; set; }
        static ConfigurationContext()
        {
            ServerConnectionString = ConfigurationManager.ConnectionStrings["TZeroHost.Properties.Settings.ConfigConn"];
            string aux = ConfigurationManager.AppSettings["TerminalZeroClient.Properties.Settings.TerminalCode"];
            int auxInt; 
            int.TryParse(aux, out auxInt);
            TerminalCode = auxInt;
            TerminalName = Environment.MachineName;
            IsOnServer = ServerConnectionString != null;
            ClientConnectionString = ConfigurationManager.ConnectionStrings["TerminalZeroClient.Properties.Settings.ConfigConn"];
            UsersConnectionString = ConfigurationManager.ConnectionStrings["TZeroHost.Properties.Settings.UsersConn"];
            LogLevel = new TraceSwitch("ZeroLogLevelSwitch", "Zero Log Level Switch", "Error");
        }

        public static ConnectionStringSettings GetConnectionForCurrentEnvironment()
        {
            ConnectionStringSettings set;

            set = IsOnServer ? ServerConnectionString : ClientConnectionString;
            
            if (set == null)
                throw new Exception("Connection String not found");

            return set;
        }

        public static EntityConnection GetConnectionForCurrentEnvironment(string modelName)
        {
            ConnectionStringSettings set = GetConnectionForCurrentEnvironment();
            
            var conStrIntegratedSecurity = new EntityConnectionStringBuilder
                                                                         {
                Metadata = "res://*/Entities." + modelName + ".csdl|"
                + "res://*/Entities." + modelName + ".ssdl|"
                + "res://*/Entities." + modelName + ".msl",
                Provider = set.ProviderName,
                ProviderConnectionString = set.ConnectionString
            };


            return new EntityConnection(conStrIntegratedSecurity.ToString());

        }

        //public static EntityConnection GetMobileConnectionForCurrentEnvironment(string modelName)
        //{
        //    System.Configuration.ConnectionStringSettings set = null;
        //    set = System.Configuration.ConfigurationManager.ConnectionStrings["TZeroHost.Properties.Settings.ConfigConn"];
        //    if (set == null)
        //    {
        //        set = System.Configuration.ConfigurationManager.ConnectionStrings["TerminalZeroClient.Properties.Settings.ConfigMobileConn"];
        //    }

        //    if (set == null)
        //        throw new Exception("Connection String not found");

        //    EntityConnectionStringBuilder conStrIntegratedSecurity = new EntityConnectionStringBuilder()
        //    {
        //        Metadata = "res://*/Entities." + modelName + ".csdl|"
        //        + "res://*/Entities." + modelName + ".ssdl|"
        //        + "res://*/Entities." + modelName + ".msl",
        //        Provider = set.ProviderName,
        //        ProviderConnectionString = set.ConnectionString
        //    };


        //    return new EntityConnection(conStrIntegratedSecurity.ToString());

        //}
        
        public static ISyncService CreateSyncConnection()
        {
            var channelFactory = new ChannelFactory<ISyncService>("BasicHttpBinding_ISyncService");
            ISyncService ret = channelFactory.CreateChannel();
            ((ICommunicationObject)ret).Open();
            return ret;
        }

        public static IFileTransfer CreateFileTranferConnection()
        {
            var channelFactory = new ChannelFactory<IFileTransfer>("BasicHttpBinding_IFileTransfer");
            IFileTransfer ret = channelFactory.CreateChannel();
            ((ICommunicationObject)ret).Open();
            return ret;
        }

        #region Nested type: Directories

        public class Directories
        {
            public const string WorkingDirSubfix = ".WD";

            static Directories()
            {

                if (!Directory.Exists(ModulesFolder))
                    Directory.CreateDirectory(ModulesFolder);
                if (!Directory.Exists(UpgradeFolder))
                    Directory.CreateDirectory(UpgradeFolder);

            }

            public static string ModulesFolder
            {
                get { return Path.Combine(Environment.CurrentDirectory, "Modules"); }
            }

            public static string UpgradeFolder
            {
                get { return Path.Combine(Environment.CurrentDirectory, "Upgrade"); }
            }

            public static string ApplicationFolder
            {
                get { return Environment.CurrentDirectory; }
            }

            public static string ApplicationPath
            {
                get
                {
                    if(Environment.GetCommandLineArgs().Length == 0)
                        return Path.Combine(Environment.CurrentDirectory, "");

                    return Environment.GetCommandLineArgs()[0];
                }
            }

            public static string ApplicationUpdaterPath
            {
                get
                {
                    return string.Format("{0}.Updater.exe",ApplicationPath);
                }
            }

        }

        #endregion


        
    }
}


