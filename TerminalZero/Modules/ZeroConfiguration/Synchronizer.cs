using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces.Services;
using ZeroCommonClasses;
using ZeroConfiguration.Entities;
using ZeroCommonClasses.Helpers;
using System.IO;
using ZeroCommonClasses.Interfaces;

namespace ZeroConfiguration
{
    public partial class Synchronizer : IDisposable
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
        private System.Timers.Timer Syncronizer;
        private System.Timers.Timer SyncronizerStatus;

        private ConfigurationEntities CurrentContext;
        private ITerminal CurrentTerminal;
        private List<SyncStep> Steps;
        private ZeroSession Session = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="syncEvery">Minutes</param>
        public Synchronizer()
        {
            Steps = new List<SyncStep>();
        }

        public double LoadConfiguration(ITerminal currentTerminal, ConfigurationEntities Context, ZeroSession session)
        {
            Session = session;
            CurrentTerminal = currentTerminal;
            SyncEvery = LoadSyncRecurrence(Context);
            Steps.Add(ExecuteHelloCommand);

            if (Session.ValidateRule("ValidateTerminalZero"))
            {
                Steps.Add(SendTerminals);
                Steps.Add(GetTerminals);
                Steps.Add(SendModules);
            }
            else
            {
                Steps.Add(ValidateCurrentModules);
                Steps.Add(GetTerminalProperties);
            }
            Steps.Add(SendTerminalProperties);
            Steps.Add(SendExistingPacks);
            Steps.Add(GetExistingPacks);
            Steps.Add(ExecuteByeCommand);
            Steps.Add(ReLoadSyncRecurrence);

            return SyncEvery;
        }

        private double LoadSyncRecurrence(ConfigurationEntities Context)
        {
            Terminal ter = Context.Terminals.First(t => t.Code == CurrentTerminal.TerminalCode);
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
                Syncronizer = new System.Timers.Timer(SyncEvery);
                SyncronizerStatus = new System.Timers.Timer(1000);
                SyncronizerStatus.Elapsed += new System.Timers.ElapsedEventHandler(SyncronizerStatus_Elapsed);
                Syncronizer.Elapsed += new System.Timers.ElapsedEventHandler(SyncEntryPoint);
                Syncronizer.Enabled = true;
                Syncronizer.Start();
                LastSync = DateTime.Now;
                SyncronizerStatus.Start();
            }
        }

        void SyncronizerStatus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OnSyncCountdownTick(new SyncCountdownTickEventArgs { 
                RemainingTime = new TimeSpan(LastSync.AddMilliseconds(SyncEvery).Ticks - DateTime.Now.Ticks) }
            );
        }

        #region Private methods

        private void SyncEntryPoint(object sender, System.Timers.ElapsedEventArgs e)
        {
            SyncronizerStatus.Enabled = Syncronizer.Enabled = false;
            SyncStartingEventArgs Config = new SyncStartingEventArgs();
            OnSyncStarting(Config);
            if (!Config.Cancel)
                try
                {
                    lastNotifier = Config.Notifier;
                    CurrentContext = new ConfigurationEntities();
                    Config.Notifier.SetProcess("Iniciando conexion");
                    bool finishState = false;
                    Config.Notifier.SetProgress(0);

                    int step = 0;
                    foreach (var item in Steps)
                    {
                        Config.Notifier.SetProgress(++step * 100 / Steps.Count);
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
                    Config.Notifier.SetUserMessage(true, "Sincronizacion Finalizada con error: " + ex.ToString());
                    Config.Notifier.SetProgress(100);
                    Config.Notifier.SetProcess("Error");
                }
                finally
                {
                    if (CurrentContext != null)
                        CurrentContext.Dispose();
                }
            Config.Cancel = false;
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

                ZeroResponse<string[]> res = Config.SyncService.GetExistingPacks(CurrentConnectionID);

                if (res.Result.Length > 0)
                {
                    foreach (var packName in res.Result)
                    {
                        //esto es solo una validacion tonta de que los primeros caracteres sean un numero,
                        //el mismo tiene que pertenecer a los modulos existentes, sino se pasa al siguiente archivo
                        int modCode = -1;
                        if (int.TryParse(packName.Substring(0, packName.IndexOf('_')), out modCode))
                        {
                            ZeroModule Mod = Config.Modules.FirstOrDefault(m => m.ModuleCode == modCode);
                            if (Mod != null)
                            {

                                ZeroCommonClasses.Files.ServerFileInfo aFile =
                                new ZeroCommonClasses.Files.ServerFileInfo
                                {
                                    FileName = packName,
                                    IsFromDB = true
                                };

                                ZeroCommonClasses.Files.RemoteFileInfo inf = Config.FileTransferService.DownloadFile(aFile);

                                string packPath = Path.Combine(Mod.WorkingDirectoryIn, packName);
                                ret = DownloadFile(Config, packPath, inf);
                                if (ret)
                                {
                                    Config.SyncService.MarkPackReceived(CurrentConnectionID, packName);
                                    Mod.NewPackReceived(packPath);
                                }
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
                Config.Notifier.SetUserMessage(false, "Error al enviar archivos: " + ex.ToString());
            }

            return ret;

        }

        private bool DownloadFile(SyncStartingEventArgs Config, string filePath, ZeroCommonClasses.Files.RemoteFileInfo inf)
        {
            bool ret = true;
            int chunkSize = 1024 * 4;
            byte[] buffer = new byte[chunkSize];
            using (System.IO.FileStream writeStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
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
                    Config.Notifier.SetProcess("Error Recibiendo Paquetes de datos");
                    Config.Notifier.SetProcess(exe.ToString());
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
                    string[] filesToSend = module.GetFilesToSend();
                    if (filesToSend.Length > 0)
                    {
                        Config.Notifier.SetUserMessage(false, "Enviando paquetes del módulo N°" + module.ModuleCode.ToString());
                        Config.Notifier.SetUserMessage(false, "Total " + filesToSend.Length.ToString());

                        foreach (var filePath in filesToSend)
                        {
                            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                            ZeroCommonClasses.Files.RemoteFileInfo aFile =
                            new ZeroCommonClasses.Files.RemoteFileInfo
                            {
                                FileName = filePath,
                                Length = file.Length,
                                ConnectionID = CurrentConnectionID,
                                FileByteStream = file.OpenRead()
                            };

                            ZeroCommonClasses.Files.ServerFileInfo inf = Config.FileTransferService.UploadFile(aFile);
                            aFile.FileByteStream.Close();
                            System.IO.File.Move(filePath, System.IO.Path.ChangeExtension(filePath, kSyncFileExtention));

                        }
                    }
                }
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                Config.Notifier.SetUserMessage(false, "Error al enviar archivos: " + ex.ToString());
            }

            return ret;

        }

        private bool ExecuteHelloCommand(SyncStartingEventArgs Config)
        {
            string msg = "";
            bool ret = true;
            Config.Notifier.SetUserMessage(false, "Enviando 'hola'");

            ZeroResponse<string> r = Config.SyncService.SayHello(CurrentTerminal.TerminalName, CurrentTerminal.TerminalCode);
            msg = r.Status;
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
            msg = r.Status;
            if (r.IsValid && r.Result != DateTime.MinValue)
            {
                CurrentContext.Terminals.First(t => t.Code == CurrentTerminal.TerminalCode).LastSync = DateTime.Now;
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
            if (Session.ValidateRule("ValidateTerminalZero"))
            {
                modulesToSend = IEnumerableExtentions.GetEntitiesAsXMLObjectList(CurrentContext.Modules);
            }
            else
            {
                Terminal T = CurrentContext.Terminals.First(t=>t.Code == CurrentTerminal.TerminalCode);
                if (!T.Modules.IsLoaded)
                    T.Modules.Load();

                modulesToSend = IEnumerableExtentions.GetEntitiesAsXMLObjectList(T.Modules);
            }

            ZeroResponse<bool> rest = Config.SyncService.SendClientModules(CurrentConnectionID,modulesToSend);
            if (rest.IsValid && rest.Result)
                Config.Notifier.SetUserMessage(false, "Datos enviados.");
            else
            {
                ret = false;
                Config.Notifier.SetProcess("Error");
                Config.Notifier.SetUserMessage(false, rest.Status);
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
            
            ZeroResponse<bool> res1 = Config.SyncService.SendClientTerminals(CurrentConnectionID, IEnumerableExtentions.GetEntitiesAsXMLObjectList<Entities.Terminal>(CurrentContext.Terminals));
            foreach (var item in CurrentContext.Terminals)
            {
                if (item.ExistsMasterData.HasValue && item.ExistsMasterData.Value)
                    item.ExistsMasterData = false;
            }
            CurrentContext.SaveChanges();
            if (!res1.IsValid)
            {
                ret = true;
                Config.Notifier.SetUserMessage(false, res1.Status);
            }

            return ret;
        }

        private bool SendTerminalProperties(SyncStartingEventArgs Config)
        {
            bool ret = true;
            Config.Notifier.SetProcess("Enviando Propiedades");
            
            ZeroResponse<bool> res1 = Config.SyncService.SendClientProperties(CurrentConnectionID, IEnumerableExtentions.GetEntitiesAsXMLObjectList<Entities.TerminalProperty>(CurrentContext.TerminalProperties));
            if (!res1.IsValid)
            {
                ret = true;
                Config.Notifier.SetUserMessage(false, res1.Status);
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
                IEnumerable<TerminalProperty> props = IEnumerableExtentions.GetEntitiesFromXMLObjectList<TerminalProperty>(res2.Result);

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
                Config.Notifier.SetUserMessage(false, res2.Status);

            return ret;
        }

        private bool GetTerminals(SyncStartingEventArgs Config)
        {
            bool ret = false;
            Config.Notifier.SetProcess("Obteniendo Terminales");
            
            ZeroResponse<string> res2 = Config.SyncService.GetTerminals(CurrentConnectionID);
            if (res2.IsValid)
            {
                IEnumerable<Terminal> props = IEnumerableExtentions.GetEntitiesFromXMLObjectList<Terminal>(res2.Result);

                foreach (var item in props)
                {
                    Terminal T = CurrentContext.Terminals.FirstOrDefault(t => t.Code == item.Code);
                    if (T == null)
                    {
                        CurrentContext.Terminals.AddObject(item);
                    }
                    else
                    {
                        T.LastSync = item.LastSync;
                    }
                    ConfigurationEntities.CreateTerminalProperties(CurrentContext, item.Code);
                }

                CurrentContext.SaveChanges();
                ret = true;
            }
            else
                Config.Notifier.SetUserMessage(false, res2.Status);

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
