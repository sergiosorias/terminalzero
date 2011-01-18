using System;
using System.Collections.Generic;
using ZeroCommonClasses.Interfaces.Services;
using ZeroConfiguration.Entities;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.GlobalObjects;
using ZeroConfiguration;

namespace TZeroHost.Services
{
    public class Sync : ISyncService
    {
        #region ISyncService Members

        public ZeroResponse<string> SayHello(string name, int terminal)
        {
            ZeroResponse<string> ret = new ZeroResponse<string>();
            using (Helpers.ServiceLogHelper hlp = new Helpers.ServiceLogHelper("SayHello", "", name, terminal))
            {
                hlp.TerminalCode = terminal;
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateTerminal(terminal, name, out hlp.StatusMessage))
                            {
                                ret.Result = Config.CreateConnection(terminal);
                                System.Diagnostics.Trace.WriteLine(string.Format("Iniciando Conexión con terminal {0} - ID {1} - ConnID {2}", name, terminal, ret.Result));
                            }
                        }
                    });

                ret.IsValid = hlp.IsValid;
                ret.Message = hlp.StatusMessage;
            }

            return ret;
        }

        public ZeroResponse<DateTime> SayBye(string ID)
        {
            ZeroResponse<DateTime> ret = new ZeroResponse<DateTime>();
            using (Helpers.ServiceLogHelper hlp = new Helpers.ServiceLogHelper("SayBye", ID))
            {
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                Config.UpdateConnectionStatus(ID, ZeroServerConfiguration.ConnectionState.Ended);
                                ret.Result = DateTime.Now;
                                System.Diagnostics.Trace.WriteLine(string.Format("Finalizando Conexión con terminal ID {0} - ConnID {1}", hlp.TerminalCode, ID));
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
            ZeroResponse<bool> ret = new ZeroResponse<bool>();
            using (Helpers.ServiceLogHelper hlp = new Helpers.ServiceLogHelper("SendClientModules", ID, modules))
            {
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                IEnumerable<Module> mods = IEnumerableExtentions.GetEntitiesFromXMLObjectList<Module>(modules);
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
            ZeroResponse<bool> ret = new ZeroResponse<bool>();

            using (Helpers.ServiceLogHelper hlp = new Helpers.ServiceLogHelper("SendClientProperties", ID, properties))
            {
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                ret.Result = true;
                                Config.MergeTerminalProperties(hlp.TerminalCode, IEnumerableExtentions.GetEntitiesFromXMLObjectList<TerminalProperty>(properties));
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
            ZeroResponse<string> ret = new ZeroResponse<string>();
            using (TZeroHost.Helpers.ServiceLogHelper hlp = new TZeroHost.Helpers.ServiceLogHelper("GetServerProperties", ID))
            {
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                IEnumerable<TerminalProperty> list = Config.GetTerminalProperties(hlp.TerminalCode);
                                ret.Result = ZeroCommonClasses.Helpers.IEnumerableExtentions.GetEntitiesAsXMLObjectList<TerminalProperty>(list);
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
            ZeroResponse<Dictionary<int, int>> ret = new ZeroResponse<Dictionary<int, int>>();
            using (TZeroHost.Helpers.ServiceLogHelper hlp = new TZeroHost.Helpers.ServiceLogHelper("GetExistingPacks", ID))
            {
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
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
            ZeroResponse<bool> ret = new ZeroResponse<bool>();
            using (TZeroHost.Helpers.ServiceLogHelper hlp = new TZeroHost.Helpers.ServiceLogHelper("MarkPackReceived", ID, packCode))
            {
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
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
            ZeroResponse<bool> ret = new ZeroResponse<bool>();

            using (TZeroHost.Helpers.ServiceLogHelper hlp = new TZeroHost.Helpers.ServiceLogHelper("SendClientTerminals", ID, terminals))
            {
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
                        {
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                ret.Result = true;
                                Config.MergeTerminal(hlp.TerminalCode, IEnumerableExtentions.GetEntitiesFromXMLObjectList<Terminal>(terminals));
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
            ZeroResponse<string> ret = new ZeroResponse<string>();
            ret.IsValid = false;
            using (TZeroHost.Helpers.ServiceLogHelper hlp = new TZeroHost.Helpers.ServiceLogHelper("GetTerminals", ID))
            {
                hlp.Handle(() =>
                    {
                        using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
                        {
                            int tCode = -1;
                            if (Config.ValidateConnection(ID, out hlp.TerminalCode, out hlp.StatusMessage))
                            {
                                IEnumerable<Terminal> list = Config.GetTerminals(tCode);
                                ret.Result = ZeroCommonClasses.Helpers.IEnumerableExtentions.GetEntitiesAsXMLObjectList<Terminal>(list);
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
