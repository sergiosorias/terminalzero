﻿using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Diagnostics;
using System.ServiceModel;
using ZeroCommonClasses.Interfaces.Services;

namespace ZeroCommonClasses.Environment
{
    public static class Config
    {
        public static TraceSwitch LogLevel { get; private set; }
        public static bool IsOnServer { get; private set; }
        public static ConnectionStringSettings ServerConnectionString { get; private set; }
        public static ConnectionStringSettings ClientConnectionString { get; private set; }
        public static ConnectionStringSettings UsersConnectionString { get; private set; }
        public static int TerminalCode { get; private set; }
        public static string TerminalName { get; private set; }
        static Config()
        {
            ServerConnectionString = ConfigurationManager.ConnectionStrings["TZeroHost.Properties.Settings.ConfigConn"];
            string aux = ConfigurationManager.AppSettings["TerminalZeroClient.Properties.Settings.TerminalCode"];
            int auxInt; 
            int.TryParse(aux, out auxInt);
            TerminalCode = auxInt;
            TerminalName = System.Environment.MachineName;
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
        
    }
}


