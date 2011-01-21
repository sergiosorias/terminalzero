﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCommonClasses;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.Entities;
using ZeroConfiguration.Entities;
using ZeroConfiguration.Properties;
using Connection = ZeroConfiguration.Entities.Connection;

namespace ZeroConfiguration
{
    public class ZeroServerConfiguration : IDisposable
    {
        public enum ConnectionState
        {
            Initialized = 0,
            InProgress = 1,
            Ended = 2,
            Error = 3

        }

// ReSharper disable FieldCanBeMadeReadOnly.Local
        private EntitiesContext<ConfigurationEntities> _currentContext;
// ReSharper restore FieldCanBeMadeReadOnly.Local
        private const int MaxConnectioMinutes = 10;
        public ZeroServerConfiguration()
        {
            _currentContext = new EntitiesContext<ConfigurationEntities>();

        }

        public void Init()
        {
            
        }

        public bool ValidateTerminal(int tcode, string tname, out string msg)
        {
            bool ret = true;
            msg = "";
            System.Diagnostics.Trace.WriteLineIf(ContextBuilder.LogLevel.TraceVerbose, string.Format("Name: {0}, Code: {1}", tcode, tname), "ValidateTerminal");
            Terminal T = _currentContext.Context.Terminals.FirstOrDefault(c => c.Code == tcode);
            if (T == default(Terminal))
            {
                if (_currentContext.Context.Terminals.Count() >= 5)
                {
                    ret = false;
                    msg = Resources.MaxTerminalsReach;
                }
                else if (string.IsNullOrEmpty(tname) || tname.Length < 4)
                {
                    ret = false;
                    msg = Resources.InvalidTerminalName;
                }
                else
                {
                    msg = Resources.NewTerminal;
                    ConfigurationEntities.AddNewTerminal(_currentContext.Context, tcode, tname);
                }
            }
            else
            {
                if (T.Name != tname)
                {
                    ret = false;
                    msg = Resources.WrongTerminalName;
                }

                if (!T.Active)
                {
                    ret = false;
                    msg += Resources.DeactivatedTerminal;
                }
            }

            return ret;
        }

        public string CreateConnection(int terminalCode)
        {
            Connection cnn = global::ZeroConfiguration.Entities.Connection.CreateConnection(Guid.NewGuid().ToString(), terminalCode, DateTime.Now);
            cnn.Terminal = _currentContext.Context.Terminals.FirstOrDefault(c => c.Code == terminalCode);
            _currentContext.Context.AddToConnections(cnn);
            _currentContext.Context.SaveChanges();
            return cnn.Code;
        }

        public bool ValidateConnection(string connId, out int terminalCode, out string msg)
        {
            bool ret = false;
            Connection cnn = _currentContext.Context.Connections.FirstOrDefault(c => c.Code == connId);
            terminalCode = -1;
            msg = "";
            if (cnn != default(Connection))
            {
                DateTime d = cnn.Stamp.AddMinutes(MaxConnectioMinutes);

                ret = DateTime.Now < d;
                if (ret)
                {
                    terminalCode = cnn.TerminalCode;
                }
                else
                {
                    msg = Resources.MaxTimeReach;
                }
            }
            else
                msg = Resources.ConnectionFinished;

            return ret;
        }

        public void UpdateConnectionStatus(string connId, ConnectionState state)
        {
            Connection con = _currentContext.Context.Connections.First(c => c.Code == connId);
            con.ConnectionStatu = _currentContext.Context.ConnectionStatus.First(cs => cs.Code == (int)state);
            if (state == ConnectionState.Ended) //Correcto
            {
                if (!con.TerminalReference.IsLoaded)
                    con.TerminalReference.Load();

                con.Terminal.LastSync = DateTime.Now;
            }

            _currentContext.Context.SaveChanges();
        }

        public void InsertModule(int terminalCode, ZeroModule module)
        {
            ConfigurationEntities.AddNewModule(_currentContext.Context, terminalCode, module.ModuleCode, "", module.Description);
        }

        public Dictionary<int, int> GetPacksToSend(int terminalCode)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            Terminal ter = _currentContext.Context.Terminals.First(t => t.Code == terminalCode);
            
            using (var ent = new ZeroCommonClasses.Entities.CommonEntities())
            {
                var query = ent.GetPacksToSend(terminalCode);
                foreach (var item in query)
                {
                    int module;
                    if (int.TryParse(item.PackName.Substring(0, item.PackName.IndexOf('_')), out module))
                    {
                        ret.Add(item.PackCode, module);
                    }
                    else
                    {
                        module = -1;
                    }
                }
            }
            

            return ret;
        }

        public IEnumerable<Terminal> GetTerminals(int tCode)
        {
            return _currentContext.Context.Terminals.Where(t => t.Code != tCode);
        }

        public IEnumerable<TerminalProperty> GetTerminalProperties(int tCode)
        {
            if (ConfigurationEntities.IsTerminalZero(_currentContext.Context, tCode))
                return _currentContext.Context.TerminalProperties;

            return _currentContext.Context.TerminalProperties.Where(tp => tp.TerminalCode == tCode);
        }

        public void MarkPackReceived(int terminalCode,int packCode)
        {
            PackPending packPending;
            using (var ent = new ZeroCommonClasses.Entities.CommonEntities())
            {
                packPending = ent.PackPendings.FirstOrDefault(pp => pp.PackCode == packCode && pp.TerminalCode == terminalCode);
                if (packPending != null)
                {
                    ent.PackPendings.DeleteObject(packPending);
                    ent.SaveChanges();
                }

                bool mark = Enumerable.All(ent.Packs.Where(p => p.Code == packCode), item => !item.IsMasterData.HasValue || !item.IsMasterData.Value);
                if (!mark)
                {
                    
                }
            }
        }

        public void MergeModules(IEnumerable<Module> mods, int terminalCaller)
        {
            foreach (var item in mods)
            {
                if (_currentContext.Context.Modules.FirstOrDefault(m => m.Code == item.Code) == null)
                {
                    item.Terminals.Add(_currentContext.Context.Terminals.First(t => t.Code == terminalCaller));
                    _currentContext.Context.Modules.AddObject(item);
                }
            }

            _currentContext.Context.SaveChanges();
        }

        public void MergeTerminalProperties(int terminalCode, IEnumerable<TerminalProperty> terProp)
        {
            foreach (var item in terProp)
            {
                if (_currentContext.Context.TerminalProperties.FirstOrDefault(m => m.Code == item.Code && m.TerminalCode == item.TerminalCode) == null)
                    _currentContext.Context.TerminalProperties.AddObject(item);
                else if (ConfigurationEntities.IsTerminalZero(_currentContext.Context,terminalCode))
                    _currentContext.Context.TerminalProperties.ApplyCurrentValues(item);
            }

            _currentContext.Context.SaveChanges();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_currentContext != null && _currentContext.Context != null)
                _currentContext.Context.Dispose();
        }

        #endregion
        
        public void MergeTerminal(int tCode, IEnumerable<Terminal> iEnumerable)
        {
            Terminal tAux;
            foreach (var item in iEnumerable)
            {
                tAux = _currentContext.Context.Terminals.FirstOrDefault(m => m.Code == item.Code);
                if (tAux  != null)
                {
                    tAux.Name = item.Name;
                    tAux.Description = item.Description;
                    tAux.Active = item.Active;
                }
            }

            _currentContext.Context.SaveChanges();
        }
    }
}