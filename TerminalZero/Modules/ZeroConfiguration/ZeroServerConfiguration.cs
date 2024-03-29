﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Environment;
using ZeroCommonClasses.Entities;
using ZeroConfiguration.Properties;

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
        private ConfigurationModelManager _currentContext;
// ReSharper restore FieldCanBeMadeReadOnly.Local
        private const int MaxConnectioMinutes = 10;
        public ZeroServerConfiguration()
        {
            _currentContext = new ConfigurationModelManager();
        }

        public void Init()
        {
            
        }

        public bool ValidateTerminal(int tcode, string tname, out string msg)
        {
            bool ret = true;
            msg = "";
            Trace.WriteLineIf(Config.LogLevel.TraceVerbose, string.Format("Name: {0}, Code: {1}", tcode, tname), "ValidateTerminal");
            Terminal T = _currentContext.Terminals.FirstOrDefault(c => c.Code == tcode);
            if (T == default(Terminal))
            {
                if (_currentContext.Terminals.Count() >= 5)
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
                    Terminal.AddNewTerminal(_currentContext, tcode, tname);
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

        public string CreateConnection(int terminalCode, string externalIp)
        {
            var cnn = ZeroBusiness.Entities.Configuration.Connection.CreateConnection(Guid.NewGuid().ToString(), terminalCode, DateTime.Now);
            cnn.Terminal = _currentContext.Terminals.FirstOrDefault(c => c.Code == terminalCode);
            cnn.Terminal.LastKnownIP = externalIp;
            _currentContext.AddToConnections(cnn);
            _currentContext.SaveChanges();
            return cnn.Code;
        }

        public bool ValidateConnection(string connId, out int terminalCode, out string msg)
        {
            bool ret = false;
            var cnn = _currentContext.Connections.FirstOrDefault(c => c.Code == connId);
            terminalCode = -1;
            msg = "";
            if (cnn != default(ZeroBusiness.Entities.Configuration.Connection))
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
            var con = _currentContext.Connections.First(c => c.Code == connId);
            con.ConnectionStatu = _currentContext.ConnectionStatus.First(cs => cs.Code == (int)state);
            if (state == ConnectionState.Ended) //Correcto
            {
                if (!con.TerminalReference.IsLoaded)
                    con.TerminalReference.Load();

                con.Terminal.LastSync = DateTime.Now;
            }

            _currentContext.SaveChanges();
        }

        public void InsertModule(int terminalCode, ZeroCommonClasses.ZeroModule module)
        {
            Module.AddNewModule(_currentContext, terminalCode, module.ModuleCode, "", module.Description);
        }

        public Dictionary<int, int> GetPacksToSend(int terminalCode)
        {
            var ret = new Dictionary<int, int>();
            using (var ent = new CommonEntitiesManager())
            {
                var query = ent.GetPacksToSend(terminalCode);
                foreach (var item in query)
                {
                    int module;
                    if (int.TryParse(item.PackName.Substring(0, item.PackName.IndexOf('_')), out module))
                    {
                        ret.Add(item.PackCode, module);
                    }
                }
            }
            

            return ret;
        }

        public IEnumerable<Terminal> GetTerminals(int tCode)
        {
            return _currentContext.Terminals.Where(t => t.Code != tCode);
        }

        public IEnumerable<TerminalProperty> GetTerminalProperties(int tCode)
        {
            if (ConfigurationModelManager.IsTerminalZero(_currentContext, tCode))
                return _currentContext.TerminalProperties;

            return _currentContext.TerminalProperties.Where(tp => tp.TerminalCode == tCode);
        }

        public void MarkPackReceived(int terminalCode,int packCode)
        {
            PackPending packPending;
            using (var ent = new CommonEntitiesManager())
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
                if (_currentContext.Modules.FirstOrDefault(m => m.Code == item.Code) == null)
                {
                    item.Terminals.Add(_currentContext.Terminals.First(t => t.Code == terminalCaller));
                    _currentContext.Modules.AddObject(item);
                }
            }

            _currentContext.SaveChanges();
        }

        public void MergeTerminalProperties(int terminalCode, IEnumerable<TerminalProperty> terProp)
        {
            foreach (var item in terProp)
            {
                if (_currentContext.TerminalProperties.FirstOrDefault(m => m.Code == item.Code && m.TerminalCode == item.TerminalCode) == null)
                    _currentContext.TerminalProperties.AddObject(item);
                else if (ConfigurationModelManager.IsTerminalZero(_currentContext,terminalCode))
                    _currentContext.TerminalProperties.ApplyCurrentValues(item);
            }

            _currentContext.SaveChanges();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_currentContext != null && _currentContext != null)
                _currentContext.Dispose();
        }

        #endregion
        
        public void MergeTerminal(int tCode, IEnumerable<Terminal> iEnumerable)
        {
            Terminal tAux;
            foreach (var item in iEnumerable)
            {
                tAux = _currentContext.Terminals.FirstOrDefault(m => m.Code == item.Code);
                if (tAux  != null)
                {
                    tAux.Name = item.Name;
                    tAux.Description = item.Description;
                    tAux.Active = item.Active;
                    tAux.IsTerminalZero = item.IsTerminalZero;
                }
            }

            _currentContext.SaveChanges();
        }
    }
}
