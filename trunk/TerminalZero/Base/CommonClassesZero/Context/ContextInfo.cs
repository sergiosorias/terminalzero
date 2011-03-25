using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using ZeroCommonClasses.Interfaces.Services;

namespace ZeroCommonClasses.Context
{
    public static class ContextInfo
    {
        public static TraceSwitch LogLevel { get; private set; }
        public static bool IsOnServer { get; private set; }
        public static ConnectionStringSettings ServerConnectionString { get; private set; }
        public static ConnectionStringSettings ClientConnectionString { get; private set; }
        public static ConnectionStringSettings UsersConnectionString { get; private set; }

        static ContextInfo()
        {
            ServerConnectionString = ConfigurationManager.ConnectionStrings["TZeroHost.Properties.Settings.ConfigConn"];
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
        }

        #endregion

    }
}


