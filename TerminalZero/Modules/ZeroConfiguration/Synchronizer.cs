﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using ZeroBusiness;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Files;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroCommonClasses.Pack;

namespace ZeroConfiguration
{
    public class Synchronizer : IDisposable
    {
        private delegate bool SyncStep(SyncStartingEventArgs Config);

        public class SyncStartingEventArgs : EventArgs
        {
            public bool Cancel { get; set; }
            public ISyncService SyncService { get; set; }
            public List<ZeroModule> Modules { get; set; }
            public IFileTransfer FileTransferService { get; set; }
            public IProgressNotifier Notifier { get; set; }
        }

        public class SyncCountdownTickEventArgs : EventArgs
        {
            public TimeSpan RemainingTime { get; set; }
        }

        public event EventHandler<SyncStartingEventArgs> SyncStarting;
        private void OnSyncStarting(SyncStartingEventArgs Config)
        {
            if (SyncStarting != null)
            {
                SyncStarting(this, Config);
            }
            else
                Config.Cancel = true;
        }

        public event EventHandler SyncFinished;

        public void OnSyncFinished()
        {
            EventHandler handler = SyncFinished;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<SyncCountdownTickEventArgs> SyncCountdownTick;
        protected void OnSyncCountdownTick(SyncCountdownTickEventArgs tick)
        {
            if (SyncCountdownTick != null)
                SyncCountdownTick(this, tick);
        }

        private DateTime LastSync;
        private IProgressNotifier lastNotifier;

        private const string kSyncFileExtention = "sync";
        private string CurrentConnectionID = "";
        private double SyncEvery = 3600 * 10;
        private Timer Syncronizer;
        private Timer SyncronizerStatus;

        private ConfigurationModelManager CurrentContext;
        private List<SyncStep> Steps;
        private ZeroSession Session;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="syncEvery">Minutes</param>
        public Synchronizer()
        {
            Steps = new List<SyncStep>();
        }

        public double LoadConfiguration(ConfigurationModelManager Context, ZeroSession session)
        {
            Session = session;
            SyncEvery = LoadSyncRecurrence(Context);
            Steps.Add(ExecuteHelloCommand);
            if (ZeroCommonClasses.Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))
            {
                Steps.Add(SendTerminals);
                Steps.Add(SendModules);
            }
            else
            {
                Steps.Add(ValidateCurrentModules);
                Steps.Add(GetTerminalProperties);
            }
            Steps.Add(GetTerminals);
            Steps.Add(SendTerminalProperties);
            Steps.Add(SendExistingPacks);
            Steps.Add(GetExistingPacks);
            Steps.Add(ExecuteByeCommand);
            Steps.Add(ReLoadSyncRecurrence);

            return SyncEvery;
        }

        private double LoadSyncRecurrence(ConfigurationModelManager Context)
        {
            var ter = Context.Terminals.First(t => t.Code == ZeroCommonClasses.Terminal.Instance.Code);
            if (!ter.TerminalProperties.IsLoaded)
                ter.TerminalProperties.Load();
            TerminalProperty value = ter.TerminalProperties.FirstOrDefault(tp => tp.Code == "SYNC_EVERY");

            LastSync = ter.LastSync.HasValue ? ter.LastSync.Value : DateTime.MinValue;
            double aux = 0;
            if (!double.TryParse(value.Value, out aux))
                aux = 10;

            aux = (60 * 1000) * aux;
            if (aux > int.MaxValue)
                aux = int.MaxValue;
            
            return aux;
        }

        public void Start()
        {
            if (Syncronizer == null)
            {
                Syncronizer = new Timer(SyncEvery);
                SyncronizerStatus = new Timer(1000);
                SyncronizerStatus.Elapsed += SyncronizerStatus_Elapsed;
                Syncronizer.Elapsed += SyncEntryPoint;
                Syncronizer.Enabled = true;
                Syncronizer.Start();
                LastSync = DateTime.Now;
                SyncronizerStatus.Start();
            }
        }

        void SyncronizerStatus_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnSyncCountdownTick(new SyncCountdownTickEventArgs { 
                RemainingTime = new TimeSpan(LastSync.AddMilliseconds(SyncEvery).Ticks - DateTime.Now.Ticks) }
            );
        }

        #region Private methods

        private void SyncEntryPoint(object sender, ElapsedEventArgs e)
        {
            SyncronizerStatus.Enabled = Syncronizer.Enabled = false;
            var Config = new SyncStartingEventArgs();
            OnSyncStarting(Config);
            if (!Config.Cancel)
                try
                {
                    lastNotifier = Config.Notifier;
                    CurrentContext = new ConfigurationModelManager();
                    Config.Notifier.SetProcess("Iniciando conexion");
                    bool finishState = false;
                    Config.Notifier.SetProgress(0);
                    int step = 0;
                    foreach (var item in Steps)
                    {
                        Config.Notifier.SetProgress(++step * 100 / Steps.Count);
                        Config.Notifier.Log(TraceLevel.Verbose, string.Format("Executing {0}, step {1}", item.Method,step));
                        finishState = item(Config);
                        if (!finishState)
                            break;
                    }
                    
                    Config.Notifier.SetUserMessage(!finishState, "Sincronizacion Finalizada");
                    Config.Notifier.SetProgress(100);
                    Config.Notifier.SetProcess(finishState ? "Listo" : "Error");
                    LastSync = DateTime.Now;
                    
                }
                catch (Exception ex)
                {
                    Config.Notifier.Log(TraceLevel.Error, string.Format("Sincronizacion Finalizada con error. {0}", ex));
                    Config.Notifier.SetUserMessage(true, "Sincronizacion Finalizada con error: " + ex.Message);
                    Config.Notifier.SetProgress(100);
                    Config.Notifier.SetProcess("Error");
                }
                finally
                {
                    if (CurrentContext != null)
                        CurrentContext.Dispose();
                }
            Config.Cancel = false;
            OnSyncFinished();
            SyncronizerStatus.Enabled = Syncronizer.Enabled = true;
        }

        private bool ReLoadSyncRecurrence(SyncStartingEventArgs Config)
        {
            SyncEvery = LoadSyncRecurrence(CurrentContext);
            Syncronizer.Interval = SyncEvery;
            return true;
        }
        
        private bool GetExistingPacks(SyncStartingEventArgs Config)
        {
            bool ret = true;
            try
            {
                Config.Notifier.SetProcess("Recibiendo Paquetes de datos");

                ZeroResponse<Dictionary<int, int>> res = Config.SyncService.GetExistingPacks(CurrentConnectionID);

                if (res.Result.Count > 0)
                {
                    foreach (var item in res.Result)
                    {
                        //esto es solo una validacion tonta de que los primeros caracteres sean un numero,
                        //el mismo tiene que pertenecer a los modulos existentes, sino se pasa al siguiente archivo
                        ZeroModule Mod = Config.Modules.FirstOrDefault(m => m.ModuleCode == item.Value);
                        if (Mod != null)
                        {

                            var aFile =
                            new ServerFileInfo
                            {
                                Code = item.Key,
                                IsFromDB = true
                            };

                            RemoteFileInfo inf = Config.FileTransferService.DownloadFile(aFile);

                            string packPath = Path.Combine(Mod.WorkingDirectoryIn, string.Format(PackManager.kPackNameFromat, item.Value, item.Key, DateTime.Now.ToString("yyyyMMddhhmmss")));
                            ret = DownloadFile(Config, packPath, inf);
                            if (ret)
                            {
                                Config.SyncService.MarkPackReceived(CurrentConnectionID, item.Key);
                                Mod.NewPackReceived(packPath);
                            }
                        }
                        
                    }
                }
                else
                    ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                Config.Notifier.SetUserMessage(false, "Error al enviar archivos: " + ex);
            }

            return ret;

        }

        private bool DownloadFile(SyncStartingEventArgs Config, string filePath, RemoteFileInfo inf)
        {
            bool ret = true;
            int chunkSize = 1024 * 4;
            var buffer = new byte[chunkSize];
            using (var writeStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                try
                {
                    do
                    {
                        // read bytes from input stream
                        int bytesRead = inf.FileByteStream.Read(buffer, 0, chunkSize);
                        if (bytesRead == 0) break;

                        // write bytes to output stream
                        writeStream.Write(buffer, 0, bytesRead);
                    } while (true);



                }
                catch (Exception exe)
                {
                    ret = false;
                    Config.Notifier.Log(TraceLevel.Error, string.Format("Error Al recibir archivo. {0} - error {1}",filePath, exe));
                    Config.Notifier.SetProcess("Error Recibiendo Paquetes de datos");
                }
                finally
                {
                    writeStream.Close();
                }


            }
            return ret;
        }

        private bool SendExistingPacks(SyncStartingEventArgs Config)
        {
            bool ret = true;
            try
            {
                Config.Notifier.SetProcess("Enviando Paquetes de datos");
                foreach (var module in Config.Modules)
                {
                    string[] filesToSend = new string[] {};
                    try
                    {
                        filesToSend = module.GetFilesToSend();
                    }
                    catch (Exception exe)
                    {
                        Config.Notifier.SetUserMessage(true, "Error al generar paquetes del módulo N°" + module.ModuleCode);
                    }
                    if (filesToSend.Length > 0)
                    {
                        Config.Notifier.SetUserMessage(false, "Enviando paquetes del módulo N°" + module.ModuleCode);
                        Config.Notifier.SetUserMessage(false, "Total " + filesToSend.Length);

                        foreach (var filePath in filesToSend)
                        {
                            var file = new FileInfo(filePath);
                            var aFile =
                            new RemoteFileInfo
                            {
                                FileName = filePath,
                                Length = file.Length,
                                ConnectionID = CurrentConnectionID,
                                FileByteStream = file.OpenRead()
                            };

                            ServerFileInfo inf = Config.FileTransferService.UploadFile(aFile);
                            aFile.FileByteStream.Close();
                            File.Move(filePath, Path.ChangeExtension(filePath, kSyncFileExtention));

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                Config.Notifier.SetUserMessage(false, "Error al enviar archivos: " + ex);
            }

            return ret;

        }

        private bool ExecuteHelloCommand(SyncStartingEventArgs Config)
        {
            string msg = "";
            bool ret = true;
            Config.Notifier.SetUserMessage(false, "Enviando 'hola'");

            ZeroResponse<string> r = Config.SyncService.SayHello(ZeroCommonClasses.Terminal.Instance.TerminalName, ZeroCommonClasses.Terminal.Instance.Code);
            msg = r.Message;
            if (r.IsValid)
                CurrentConnectionID = r.Result;

            ret = r.IsValid;

            Config.Notifier.SetUserMessage(false, "Rta: " + msg);

            return ret;
        }


        private bool ExecuteByeCommand(SyncStartingEventArgs Config)
        {
            string msg = "";
            bool ret = true;
            Config.Notifier.SetUserMessage(false, "Enviando 'bye'");

            ZeroResponse<DateTime> r = Config.SyncService.SayBye(CurrentConnectionID);
            msg = r.Message;
            if (r.IsValid && r.Result != DateTime.MinValue)
            {
                CurrentContext.Terminals.First(t => t.Code == ZeroCommonClasses.Terminal.Instance.Code).LastSync = DateTime.Now;
                CurrentContext.SaveChanges();
            }

            ret = r.IsValid;

            Config.Notifier.SetUserMessage(!ret, "Rta: " + msg);

            return ret;
        }

        private bool SendModules(SyncStartingEventArgs Config)
        {
            bool ret = true;

            Config.Notifier.SetProcess("Enviando datos");
            Config.Notifier.SetUserMessage(false, "Enviando datos sobre módulos actuales al servidor.");
            string modulesToSend;
            if (ZeroCommonClasses.Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))
            {
                modulesToSend = ContextExtentions.GetEntitiesAsXMLObjectList(CurrentContext.Modules);
            }
            else
            {
                var T = CurrentContext.Terminals.First(t => t.Code == ZeroCommonClasses.Terminal.Instance.Code);
                if (!T.Modules.IsLoaded)
                    T.Modules.Load();

                modulesToSend = ContextExtentions.GetEntitiesAsXMLObjectList(T.Modules);
            }

            ZeroResponse<bool> rest = Config.SyncService.SendClientModules(CurrentConnectionID,modulesToSend);
            if (rest.IsValid && rest.Result)
                Config.Notifier.SetUserMessage(false, "Datos enviados.");
            else
            {
                ret = false;
                Config.Notifier.SetProcess("Error");
                Config.Notifier.SetUserMessage(false, rest.Message);
            }


            return ret;
        }

        private bool ValidateCurrentModules(SyncStartingEventArgs Config)
        {
            string msg = "OK";
            bool ret = true;
            Config.Notifier.SetUserMessage(false, "Validando Módulos");

            foreach (var item in Config.Modules)
            {
                item.TerminalStatus = ModuleStatus.Valid;
            }

            Config.Notifier.SetUserMessage(false, "Rta: " + msg);

            return ret;
        }

        private bool SendTerminals(SyncStartingEventArgs Config)
        {
            bool ret = true;
            Config.Notifier.SetProcess("Enviando Terminales");
            
            ZeroResponse<bool> res1 = Config.SyncService.SendClientTerminals(CurrentConnectionID, ContextExtentions.GetEntitiesAsXMLObjectList(CurrentContext.Terminals));
            if (!res1.IsValid)
            {
                ret = true;
                Config.Notifier.SetUserMessage(false, res1.Message);
            }

            return ret;
        }

        private bool SendTerminalProperties(SyncStartingEventArgs Config)
        {
            bool ret = true;
            Config.Notifier.SetProcess("Enviando Propiedades");
            
            ZeroResponse<bool> res1 = Config.SyncService.SendClientProperties(CurrentConnectionID, ContextExtentions.GetEntitiesAsXMLObjectList(CurrentContext.TerminalProperties));
            if (!res1.IsValid)
            {
                ret = true;
                Config.Notifier.SetUserMessage(false, res1.Message);
            }

            return ret;
        }

        private bool GetTerminalProperties(SyncStartingEventArgs Config)
        {
            bool ret = false;
            Config.Notifier.SetProcess("Obteniendo Propiedades");
            
            ZeroResponse<string> res2 = Config.SyncService.GetServerProperties(CurrentConnectionID);
            if (res2.IsValid)
            {
                IEnumerable<TerminalProperty> props = ContextExtentions.GetEntitiesFromXMLObjectList<TerminalProperty>(res2.Result);

                foreach (var item in props)
                {
                    if (CurrentContext.TerminalProperties.FirstOrDefault(tp => tp.TerminalCode == item.TerminalCode && tp.Code == item.Code) == null)
                        CurrentContext.TerminalProperties.AddObject(item);
                    else
                        CurrentContext.TerminalProperties.ApplyCurrentValues(item);
                }

                CurrentContext.SaveChanges();
                ret = true;
            }
            else
                Config.Notifier.SetUserMessage(false, res2.Message);

            return ret;
        }

        private bool GetTerminals(SyncStartingEventArgs Config)
        {
            bool ret = false;
            Config.Notifier.SetProcess("Obteniendo Terminales");
            
            ZeroResponse<string> res2 = Config.SyncService.GetTerminals(CurrentConnectionID);
            if (res2.IsValid)
            {
                IEnumerable<ZeroBusiness.Entities.Configuration.Terminal> props = ContextExtentions.GetEntitiesFromXMLObjectList<ZeroBusiness.Entities.Configuration.Terminal>(res2.Result);

                foreach (var item in props)
                {
                    var T = CurrentContext.Terminals.FirstOrDefault(t => t.Code == item.Code);
                    if (T == null)
                    {
                        CurrentContext.Terminals.AddObject(item);
                    }
                    else
                    {
                        T.LastSync = item.LastSync;
                        T.Description = item.Description;
                        T.Name = item.Name;
                        T.IsTerminalZero = item.IsTerminalZero;
                    }
                    ConfigurationModelManager.CreateTerminalProperties(CurrentContext, item.Code);
                }

                CurrentContext.SaveChanges();
                ret = true;
            }
            else
                Config.Notifier.SetUserMessage(false, res2.Message);

            return ret;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Syncronizer.Dispose();
        }

        #endregion
    }
}
