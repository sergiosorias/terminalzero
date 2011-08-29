using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.ServiceModel.DomainServices.EntityFramework;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Text;
using TerminalZeroRiaWebClient.Web.Classes;
using TerminalZeroRiaWebClient.Web.Helpers;
using TerminalZeroRiaWebClient.Web.Models;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Entities;
using Connection = ZeroCommonClasses.Entities.Connection;
using System.Reactive;

namespace TerminalZeroRiaWebClient.Web.Services
{
    [RequiresAuthentication]
    [RequiresRole(Security.AdministratorRoleId)]
    [EnableClientAccess]
    public class TerminalZeroConfigDomainService : LinqToEntitiesDomainService<CommonEntitiesManager>
    {
        public IQueryable<Connection> GetConnections()
        {

            return ObjectContext.Connections;
        }

        public IQueryable<PackStatu> GetPackStatus()
        {
            return ObjectContext.PackStatus;
        }

        #region Terminal Status

        public IEnumerable<TerminalStatus> GetTerminalStatus()
        {
            var result = new List<TerminalStatus>();

            using (var ent = new ConfigurationModelManager())
            {
                ent.ContextOptions.LazyLoadingEnabled = false;
                
                foreach (var terminal in ent.Terminals)
                {
                    var tst = new TerminalStatus();
                    tst.Terminal = terminal;
                    tst.Info += string.Format("IP: {0}\n", terminal.LastKnownIP);
                    
                    var tt = ObjectContext.GetPacksToSend(tst.Terminal.Code);
                    
                    StringBuilder sb = new StringBuilder();
                    int count = 0;
                    foreach (var i in tt)
                    {
                        count++;
                        sb.AppendLine(string.Format("Módulo: {0}(Pack: {1})", i.PackCode,i.PackCode));
                    }
                    tst.Info+= (string.Format("Packs pendientes: {0}{1}", count, Environment.NewLine));
                    tst.Info += sb.ToString();

                    result.Add(tst);
                }

                foreach (var item in result)
                {
                    TerminalProperty prop =
                        ent.TerminalProperties.FirstOrDefault(
                            tp => tp.TerminalCode == item.Terminal.Code && tp.Code == "SYNC_EVERY");
                    if (prop != null)
                    {
                        if (item.Terminal.LastSync.HasValue)
                        {
                            item.Terminal.IsSyncronized = item.Terminal.LastSync.Value.AddMinutes(int.Parse(prop.Value)) >
                                                          DateTime.Now;
                        }
                    }

                }

            }

            return result;
        }
        #endregion

        #region Pack
        public IQueryable<Pack> GetPacks(DateTime startDate, DateTime endDate)
        {
            IQueryable<Pack> res = ObjectContext.Packs.Where(p => p.Stamp >= startDate && p.Stamp <= endDate);
            
            foreach (Pack pack in res)
            {
                pack.Data = null;
            }
            
            return res;
        }

        public void InsertPack(Pack pack)
        {
            if ((pack.EntityState != EntityState.Detached))
            {
                ObjectContext.ObjectStateManager.ChangeObjectState(pack, EntityState.Added);
            }
            else
            {
                ObjectContext.Packs.AddObject(pack);
            }
        }

        public void UpdatePack(Pack currentPack)
        {
            ObjectContext.Packs.AttachAsModified(currentPack, ChangeSet.GetOriginal(currentPack));
        }

        public void DeletePack(Pack pack)
        {
            if ((pack.EntityState != EntityState.Detached))
            {
                ObjectContext.ObjectStateManager.ChangeObjectState(pack, EntityState.Deleted);
            }
            else
            {
                ObjectContext.Packs.Attach(pack);
                ObjectContext.Packs.DeleteObject(pack);
            }
        }

        public string UploadFileSilverlight(string fileName, byte[] fileByteStream)
        {
            Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, "Start web uploading " + fileName, "Verbose");
            Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, "Size " + fileByteStream.Length, "Verbose");
            string destFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetExtension(fileName);
            string filePath = Path.Combine(AppDirectories.UploadFolder, destFileName);
            if (File.Exists(filePath)) File.Delete(filePath);

            using (var writeStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                try
                {
                    writeStream.Write(fileByteStream, 0, fileByteStream.Length);

                    // report end
                    Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, "Done!", "Verbose");
                    IncomingPackManager.Instance.AddPack(Security.CurrentUser.Name, filePath);
                    return destFileName;
                }
                catch (Exception exe)
                {
                    Trace.WriteLineIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceError, exe);
                }
                finally
                {
                    writeStream.Close();
                }
                return "";
            }
        }
        #endregion

        #region Pack Pending
        public IQueryable<PackPending> GetPackPendings()
        {
            return ObjectContext.PackPendings;
        }

        public void InsertPackPending(PackPending packPending)
        {
            if ((packPending.EntityState != EntityState.Detached))
            {
                ObjectContext.ObjectStateManager.ChangeObjectState(packPending, EntityState.Added);
            }
            else
            {
                ObjectContext.PackPendings.AddObject(packPending);
            }
        }

        public void UpdatePackPending(PackPending currentPackPending)
        {
            ObjectContext.PackPendings.AttachAsModified(currentPackPending, ChangeSet.GetOriginal(currentPackPending));
        }

        public void DeletePackPending(PackPending packPending)
        {
            if ((packPending.EntityState != EntityState.Detached))
            {
                ObjectContext.ObjectStateManager.ChangeObjectState(packPending, EntityState.Deleted);
            }
            else
            {
                ObjectContext.PackPendings.Attach(packPending);
                ObjectContext.PackPendings.DeleteObject(packPending);
            }
        }
        #endregion
    }
}


