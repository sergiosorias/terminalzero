using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using TZeroHost.Helpers;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces.Services;
using ZeroConfiguration;

namespace TZeroHost.Services
{
    public class Sync : ISyncService
    {
        #region ISyncService Members

        public ZeroResponse<string> SayHello(string name, int terminal)
        {
            var ret = new ZeroResponse<string>();
            using (var hlp = new ServiceLogHelper("SayHello", "", name, terminal))
            {
                hlp.TerminalCode = terminal;
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateTerminal(terminal, name, out hlp.StatusMessage))
                            {
                                string ip = GetWCFMethodCallerIp();
                                ret.Result = Config.CreateConnection(terminal, ip);
                                Trace.WriteLine(string.Format("Iniciando Conexión con terminal {0} - ID {1} - ConnID {2} - IP: {3}", name, terminal, ret.Result, ip));
                            }
                        }
                    });
                
                ret.IsValid = hlp.IsValid;
                ret.Message = hlp.StatusMessage;
            }

            return ret;
        }

        private string GetWCFMethodCallerIp()
        {
            string ret = string.Empty;
            try
            {
                OperationContext context = OperationContext.Current;
                MessageProperties messageProperties = context.IncomingMessageProperties;
                var endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

                if (endpointProperty != null) ret = endpointProperty.Address;
            }
            catch 
            {
            }

            return ret;
        }

        public ZeroResponse<DateTime> SayBye(string ID)
        {
            var ret = new ZeroResponse<DateTime>();
            using (var hlp = new ServiceLogHelper("SayBye", ID))
            {
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                Config.UpdateConnectionStatus(ID, ZeroServerConfiguration.ConnectionState.Ended);
                                ret.Result = DateTime.Now;
                                Trace.WriteLine(string.Format("Finalizando Conexión con terminal ID {0} - ConnID {1}", hlp.TerminalCode, ID));
                            }
                            else
                            {
                                if (hlp.TerminalCode >= 0)
                                    Config.UpdateConnectionStatus(ID, ZeroServerConfiguration.ConnectionState.Error);

                                ret.Result = DateTime.MinValue;
                            }
                        }
                    });

                ret.Message = hlp.StatusMessage;
                ret.IsValid = hlp.IsValid;
            }


            return ret;
        }

        public ZeroResponse<bool> SendClientModules(string ID, string modules)
        {
            var ret = new ZeroResponse<bool>();
            using (var hlp = new ServiceLogHelper("SendClientModules", ID, modules))
            {
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                IEnumerable<Module> mods = ContextExtentions.GetEntitiesFromXMLObjectList<Module>(modules);
                                Config.MergeModules(mods, hlp.TerminalCode);
                                ret.Result = true;
                            }
                        }

                        
                    });

                ret.IsValid = hlp.IsValid;
                ret.Message = hlp.StatusMessage;
            };

            return ret;
        }

        public ZeroResponse<bool> SendClientProperties(string ID, string properties)
        {
            var ret = new ZeroResponse<bool>();

            using (var hlp = new ServiceLogHelper("SendClientProperties", ID, properties))
            {
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                ret.Result = true;
                                Config.MergeTerminalProperties(hlp.TerminalCode, ContextExtentions.GetEntitiesFromXMLObjectList<TerminalProperty>(properties));
                            }
                            else
                            {
                                ret.Result = false;
                            }
                        }
                    });

                ret.IsValid = hlp.IsValid;
                ret.Message = hlp.StatusMessage;
            }

            return ret;
        }

        public ZeroResponse<string> GetServerProperties(string ID)
        {
            var ret = new ZeroResponse<string>();
            using (var hlp = new ServiceLogHelper("GetServerProperties", ID))
            {
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                IEnumerable<TerminalProperty> list = Config.GetTerminalProperties(hlp.TerminalCode);
                                ret.Result = ContextExtentions.GetEntitiesAsXMLObjectList(list);
                            }
                        }
                    });

                ret.IsValid = hlp.IsValid;
                ret.Message = hlp.StatusMessage;
            }
            return ret;
        }

        public ZeroResponse<Dictionary<int, int>> GetExistingPacks(string ID)
        {
            var ret = new ZeroResponse<Dictionary<int, int>>();
            using (var hlp = new ServiceLogHelper("GetExistingPacks", ID))
            {
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                ret.Result = Config.GetPacksToSend(hlp.TerminalCode);
                            }
                            else
                            {
                                if (hlp.TerminalCode >= 0)
                                    Config.UpdateConnectionStatus(ID, ZeroServerConfiguration.ConnectionState.Error);
                            }
                        }
                    });

                ret.IsValid = hlp.IsValid;
                ret.Message = hlp.StatusMessage;
            }

            return ret;
        }

        public ZeroResponse<bool> MarkPackReceived(string ID, int packCode)
        {
            var ret = new ZeroResponse<bool>();
            using (var hlp = new ServiceLogHelper("MarkPackReceived", ID, packCode))
            {
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                Config.MarkPackReceived(hlp.TerminalCode, packCode);
                                ret.Result = true;
                            }
                            else
                            {
                                ret.Result = false;
                            }
                        }
                    });

                ret.Message = hlp.StatusMessage;
                ret.IsValid = hlp.IsValid;

            }

            return ret;
        }

        public ZeroResponse<bool> SendClientTerminals(string ID, string terminals)
        {
            var ret = new ZeroResponse<bool>();

            using (var hlp = new ServiceLogHelper("SendClientTerminals", ID, terminals))
            {
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                ret.Result = true;
                                Config.MergeTerminal(hlp.TerminalCode, ContextExtentions.GetEntitiesFromXMLObjectList<Terminal>(terminals));
                            }
                        }
                    });

                ret.IsValid = hlp.IsValid;
                ret.Message = hlp.StatusMessage;
            }


            return ret;
        }

        public ZeroResponse<string> GetTerminals(string ID)
        {
            var ret = new ZeroResponse<string>();
            ret.IsValid = false;
            using (var hlp = new ServiceLogHelper("GetTerminals", ID))
            {
                hlp.Handle(() =>
                    {
                        using (var Config = new ZeroServerConfiguration())
                        {
                            int tCode = -1;
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                IEnumerable<Terminal> list = Config.GetTerminals(tCode);
                                ret.Result = ContextExtentions.GetEntitiesAsXMLObjectList(list);
                            }

                        }
                    });

                ret.IsValid = hlp.IsValid;
                ret.Message = hlp.StatusMessage;
            }

            return ret;
        }


        #endregion
    }
}
