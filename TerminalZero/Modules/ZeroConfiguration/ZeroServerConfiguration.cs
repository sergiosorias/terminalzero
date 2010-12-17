using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroConfiguration.Entities;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.Interfaces;
using System.IO;
using System.Data;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses;

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

        private EntitiesContext<ConfigurationEntities> CurrentContext;
        private int maxConnectioMinutes = 10;
        public ZeroServerConfiguration()
        {
            CurrentContext = new EntitiesContext<ConfigurationEntities>();

        }

        public void Init()
        {
            
        }

        public bool ValidateTerminal(int tcode, string tname, out string msg)
        {
            bool ret = true;
            msg = "";
            Terminal T = CurrentContext.Context.Terminals.FirstOrDefault(C => C.Code == tcode);
            if (T == default(Terminal))
            {
                if (CurrentContext.Context.Terminals.Count() >= 5)
                {
                    ret = false;
                    msg = "Se excedió el número máximo de terminales";
                }
                else if (string.IsNullOrEmpty(tname) || tname.Length < 4)
                {
                    ret = false;
                    msg = "Nombre de terminal inválido, mínimo 4 caracteres";
                }
                else
                {
                    msg = "Terminal nueva, se intentará crearla";
                    ConfigurationEntities.AddNewTerminal(CurrentContext.Context, tcode, tname);
                }
            }
            else
            {
                if (T.Name != tname)
                {
                    ret = false;
                    msg = "El nombre no concuerda con el existente en la central.";
                }

                if (!T.Active)
                {
                    ret = false;
                    msg += "\nLa Terminal fue desactivada para realizar esta acción.";
                }
            }

            return ret;
        }

        public string CreateConnection(int TerminalCode)
        {
            Connection cnn = global::ZeroConfiguration.Entities.Connection.CreateConnection(Guid.NewGuid().ToString(), TerminalCode, DateTime.Now);
            cnn.Terminal = CurrentContext.Context.Terminals.FirstOrDefault(C => C.Code == TerminalCode);
            CurrentContext.Context.AddToConnections(cnn);
            CurrentContext.Context.SaveChanges();
            return cnn.Code;
        }

        public bool ValidateConnection(string connID, out int TerminalCode, out string msg)
        {
            bool ret = false;
            Connection cnn = CurrentContext.Context.Connections.FirstOrDefault(C => C.Code == connID);
            TerminalCode = -1;
            msg = "";
            if (cnn != default(Connection))
            {
                DateTime d = cnn.Stamp.AddMinutes(maxConnectioMinutes);

                ret = DateTime.Now < d;
                if (ret)
                {
                    TerminalCode = cnn.TerminalCode;
                }
                else
                {
                    ret = false;
                    msg = "La conexión ha superado el tiempo de espera máximo.";
                }
            }
            else
                msg = "La conexión no fue inicializada";


            return ret;
        }

        public void UpdateConnectionStatus(string connID, ConnectionState state)
        {
            Connection Con = CurrentContext.Context.Connections.First(c => c.Code == connID);
            Con.ConnectionStatu = CurrentContext.Context.ConnectionStatus.First(CS => CS.Code == (int)state);
            if (state == ConnectionState.Ended) //Correcto
            {
                if (!Con.TerminalReference.IsLoaded)
                    Con.TerminalReference.Load();

                Con.Terminal.LastSync = DateTime.Now;
            }

            CurrentContext.Context.SaveChanges();
        }

        public void InsertModule(int terminalCode, ZeroModule module)
        {
            ConfigurationEntities.AddNewModule(CurrentContext.Context, terminalCode, module.ModuleCode, "", module.Description);
        }

        public string[] GetPackNamesToSend(int terminalCode)
        {
            List<string> ret = new List<string>();

            Terminal ter = CurrentContext.Context.Terminals.First(t => t.Code == terminalCode);
            if (ter.ExistsMasterData.HasValue && ter.ExistsMasterData.Value)
            {
                using (ZeroCommonClasses.Entities.CommonEntities ent = new ZeroCommonClasses.Entities.CommonEntities())
                {
                    var algo = from packs in
                                   (from pack in ent.Packs
                                    select new
                                       {
                                           Name = pack.Name,
                                           Stamp = pack.Stamp,
                                           ConnectionCode = pack.ConnectionCode,
                                           IsOK = (pack.IsMasterData.HasValue && pack.IsMasterData.Value && pack.PackStatusCode == 2)
                                       })
                               join conns in ent.Connections
                               on packs.ConnectionCode equals conns.Code
                               where conns.TerminalCode != terminalCode
                               orderby packs.Stamp descending
                               select packs.Name;

                    if (algo != null && algo.Count()>0)
                        ret.Add(algo.First());
                }
            }

            return ret.ToArray();
        }

        internal IEnumerable<Terminal> GetTerminals(int tCode)
        {
            return CurrentContext.Context.Terminals.Where(t => t.Code != tCode);
        }

        internal IEnumerable<TerminalProperty> GetTerminalProperties(int tCode)
        {
            if (ConfigurationEntities.IsTerminalZero(CurrentContext.Context, tCode))
                return CurrentContext.Context.TerminalProperties;
            else
                return CurrentContext.Context.TerminalProperties.Where(tp => tp.TerminalCode == tCode);
        }

        public void MarkPackReceived(int terminalCode,string packName)
        {
            using (ZeroCommonClasses.Entities.CommonEntities ent = new ZeroCommonClasses.Entities.CommonEntities())
            {
                ZeroCommonClasses.Entities.Pack P = ent.Packs.First(p => p.Name == packName);
                if (P.IsMasterData.HasValue && P.IsMasterData.Value)
                {
                    Terminal ter = CurrentContext.Context.Terminals.First(t => t.Code == terminalCode);
                    ter.ExistsMasterData = false;
                    CurrentContext.Context.SaveChanges();
                }

            }
        }

        public void MergeModules(IEnumerable<Module> mods, int terminalCaller)
        {
            foreach (var item in mods)
            {
                if (CurrentContext.Context.Modules.FirstOrDefault(m => m.Code == item.Code) == null)
                {
                    item.Terminals.Add(CurrentContext.Context.Terminals.First(t => t.Code == terminalCaller));
                    CurrentContext.Context.Modules.AddObject(item);
                }
            }
            
            CurrentContext.Context.SaveChanges();
        }

        internal void MergeTerminalProperties(int terminalCode, IEnumerable<TerminalProperty> terProp)
        {
            foreach (var item in terProp)
            {
                if (CurrentContext.Context.TerminalProperties.FirstOrDefault(m => m.Code == item.Code && m.TerminalCode == item.TerminalCode) == null)
                    CurrentContext.Context.TerminalProperties.AddObject(item);
                else if (ConfigurationEntities.IsTerminalZero(CurrentContext.Context,terminalCode))
                    CurrentContext.Context.TerminalProperties.ApplyCurrentValues(item);
            }

            CurrentContext.Context.SaveChanges();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (CurrentContext != null && CurrentContext.Context != null)
                CurrentContext.Context.Dispose();
        }

        #endregion
        
        internal void MergeTerminal(int tCode, IEnumerable<Terminal> iEnumerable)
        {
            Terminal tAux = null;
            foreach (var item in iEnumerable)
            {
                tAux = CurrentContext.Context.Terminals.FirstOrDefault(m => m.Code == item.Code);
                if (tAux  != null)
                {
                    tAux.Name = item.Name;
                    tAux.Description = item.Description;
                    tAux.Active = item.Active;
                    if (item.ExistsMasterData.HasValue && item.ExistsMasterData.Value)
                        tAux.ExistsMasterData = item.ExistsMasterData;
                }
            }

            CurrentContext.Context.SaveChanges();
        }
    }
}
