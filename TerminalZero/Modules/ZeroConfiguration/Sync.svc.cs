using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces.Services;
using ZeroCommonClasses.Interfaces;
using ZeroConfiguration.Entities;
using System.Data;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.GlobalObjects;

namespace ZeroConfiguration
{
    public class Sync : ISyncService
    {
        #region ISyncService Members

        public ZeroResponse<string> SayHello(string name, int terminal)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("Name: {0}, Code: {1}",name,terminal), "SayHello");
            System.Diagnostics.Trace.Indent();
            ZeroResponse<string> ret = new ZeroResponse<string>();
            ZeroServerConfiguration Config = new ZeroServerConfiguration();
            string msg = "";
            ret.IsValid = Config.ValidateTerminal(terminal, name, out msg);
            if (ret.IsValid)
            {
                ret.Result = Config.CreateConnection(terminal);
                msg += " OK";
            }
            System.Diagnostics.Trace.Unindent();
            System.Diagnostics.Trace.WriteLine(string.Format("Name: {0}, Code: {1}, Message: {2}", name, terminal, ret), "SayHello");
            ret.Status = msg;
            return ret;
        }

        public ZeroResponse<DateTime> SayBye(string ID)
        {
            ZeroResponse<DateTime> ret = new ZeroResponse<DateTime>();
            ret.IsValid = true;
            using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
            {
                string msg;
                int tCode = -1;
                if (Config.ValidateConnection(ID, out tCode, out msg))
                {
                    Config.UpdateConnectionStatus(ID, ZeroServerConfiguration.ConnectionState.Ended);
                    ret.Result = DateTime.Now;
                }
                else
                {
                    if (tCode >= 0)
                        Config.UpdateConnectionStatus(ID, ZeroServerConfiguration.ConnectionState.Error);

                    ret.Result = DateTime.MinValue;
                    ret.Status = msg;
                }
            }

            return ret;
        }

        public ZeroResponse<bool> SendClientModules(string ID, string modules)
        {
            int TCode = 0;
            string msg = "";
            bool stat = true;
            using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
            {
                stat = Config.ValidateConnection(ID, out TCode, out msg);
                if (stat)
                {
                    IEnumerable<Module> mods = IEnumerableExtentions.GetEntitiesFromXMLObjectList<Module>(modules);
                    Config.MergeModules(mods, TCode);
                }
            }
            ZeroResponse<bool> ret = new ZeroResponse<bool>
            {
                IsValid = stat,
                Result = stat,
                Status = (stat) ? "OK" : msg
            };

            return ret;
        }

        public ZeroResponse<bool> SendClientProperties(string ID, string properties)
        {
            ZeroResponse<bool> ret = new ZeroResponse<bool>();
            ret.IsValid = true;
            using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
            {
                string msg;
                int tCode = -1;
                if (Config.ValidateConnection(ID, out tCode, out msg))
                {
                    ret.Result = true;
                    Config.MergeTerminalProperties(tCode,IEnumerableExtentions.GetEntitiesFromXMLObjectList<TerminalProperty>(properties));
                }
                
                ret.Result = false;
                ret.Status = msg;
            }

            return ret;
        }

        public ZeroResponse<string> GetServerProperties(string ID)
        {
            ZeroResponse<string> ret = new ZeroResponse<string>();
            ret.IsValid = true;
            using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
            {
                string msg;
                int tCode = -1;
                if (Config.ValidateConnection(ID, out tCode, out msg))
                {
                    IEnumerable<TerminalProperty> list = Config.GetTerminalProperties(tCode);
                    ret.Result = ZeroCommonClasses.Helpers.IEnumerableExtentions.GetEntitiesAsXMLObjectList<TerminalProperty>(list);
                }

                ret.Status = msg;
            }
            return ret;
        }

        public ZeroResponse<string[]> GetExistingPacks(string ID)
        {
            ZeroResponse<string[]> ret = new ZeroResponse<string[]>
            {
                IsValid = true,
            };

            using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
            {
                int TCode = 0;
                string msg = "";
                bool stat = Config.ValidateConnection(ID, out TCode, out msg);
                if (stat)
                {
                    ret.Result = Config.GetPackNamesToSend(TCode);
                }
                else
                {
                    if (TCode > 0)
                        Config.UpdateConnectionStatus(ID, ZeroServerConfiguration.ConnectionState.Error);
                }
                ret.Status = msg;
            }
            
            return ret;
        }

        public ZeroResponse<bool> MarkPackReceived(string ID, string packName)
        {
            ZeroResponse<bool> ret = new ZeroResponse<bool>();
            ret.IsValid = true;
            using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
            {
                string msg;
                int tCode = -1;
                if (Config.ValidateConnection(ID, out tCode, out msg))
                {
                    Config.MarkPackReceived(tCode, packName);
                    ret.Result = true;
                }

                ret.Result = false;
                ret.Status = msg;
            }

            return ret;
        }

        public ZeroResponse<bool> SendClientTerminals(string ID, string terminals)
        {
            ZeroResponse<bool> ret = new ZeroResponse<bool>();
            ret.IsValid = true;
            using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
            {
                string msg;
                int tCode = -1;
                if (Config.ValidateConnection(ID, out tCode, out msg))
                {
                    ret.Result = true;
                    Config.MergeTerminal(tCode, IEnumerableExtentions.GetEntitiesFromXMLObjectList<Terminal>(terminals));
                }

                ret.Result = false;
                ret.Status = msg;
            }

            return ret;
        }

        public ZeroResponse<string> GetTerminals(string ID)
        {
            ZeroResponse<string> ret = new ZeroResponse<string>();
            ret.IsValid = true;
            using (ZeroServerConfiguration Config = new ZeroServerConfiguration())
            {
                string msg;
                int tCode = -1;
                if (Config.ValidateConnection(ID, out tCode, out msg))
                {
                    IEnumerable<Terminal> list = Config.GetTerminals(tCode);
                    ret.Result = ZeroCommonClasses.Helpers.IEnumerableExtentions.GetEntitiesAsXMLObjectList<Terminal>(list);
                }

                ret.Status = msg;
            }
            return ret;
        }
        

        #endregion
    }
}
