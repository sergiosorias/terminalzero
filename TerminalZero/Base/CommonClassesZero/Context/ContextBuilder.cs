using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;
using System.ServiceModel;
using ZeroCommonClasses.Interfaces.Services;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.Diagnostics;

namespace ZeroCommonClasses.Context
{
    public static class ContextBuilder
    {
        public static TraceSwitch LogLevel { get; private set; }
        public static System.Configuration.ConnectionStringSettings ServerConnectionString { get; private set; }
        public static System.Configuration.ConnectionStringSettings ClientConnectionString { get; private set; }
        public static System.Configuration.ConnectionStringSettings UsersConnectionString { get; private set; }

        static ContextBuilder()
        {
            ServerConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TZeroHost.Properties.Settings.ConfigConn"];
            ClientConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TerminalZeroClient.Properties.Settings.ConfigConn"];
            UsersConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TZeroHost.Properties.Settings.UsersConn"];
            LogLevel = new TraceSwitch("ZeroLogLevelSwitch", "Zero Log Level Switch", "Error");
        }

        public static System.Configuration.ConnectionStringSettings GetConnectionForCurrentEnvironment()
        {
            System.Configuration.ConnectionStringSettings set;

            if (ServerConnectionString != null)
                set = ServerConnectionString;
            else
                set = ClientConnectionString;
            
            if (set == null)
                throw new Exception("Connection String not found");

            return set;
        }

        public static EntityConnection GetConnectionForCurrentEnvironment(string modelName)
        {
            System.Configuration.ConnectionStringSettings set = GetConnectionForCurrentEnvironment();
            
            EntityConnectionStringBuilder conStrIntegratedSecurity = new EntityConnectionStringBuilder()
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
            ChannelFactory<ISyncService> channelFactory = new ChannelFactory<ISyncService>("BasicHttpBinding_ISyncService");
            ISyncService ret = channelFactory.CreateChannel();
            ((ICommunicationObject)ret).Open();
            return ret;
        }

        public static IFileTransfer CreateFileTranferConnection()
        {
            ChannelFactory<IFileTransfer> channelFactory = new ChannelFactory<IFileTransfer>("BasicHttpBinding_IFileTransfer");
            IFileTransfer ret = channelFactory.CreateChannel();
            ((ICommunicationObject)ret).Open();
            return ret;
        }

    }
}


