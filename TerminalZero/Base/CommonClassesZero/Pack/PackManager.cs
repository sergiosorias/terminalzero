using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroCommonClasses.Pack
{
    public abstract class PackManager : IDisposable
    {
        public const string kPackExtention = ".zpack";
        public const string kPackNameFromat = "{0}_{1}_{2}" + kPackExtention;
        
        [Flags]
        public enum PackStatus
        {
            Starting = 0,
            InProgress = 1,
            Imported = 2,
            Error = 3,
        }

        #region Statics
        public static PackManager GetDefaultManager()
        {
            return null;
        }

        public static string[] GetPacks(int moduleCode, string workingDirectory)
        {
            var res = new List<string>();
            res.AddRange(Directory.GetFiles(workingDirectory, moduleCode + "*" + kPackExtention));
            return res.ToArray();
        }

        public static int GetModule(string name)
        {
            string[] args = name.Split('_');

            int moduleCode = 0;
            if (args.Length > 1)
                int.TryParse(args[0], out moduleCode);

            return moduleCode;

        }

        private static IEnumerable<int> GetTerminalDestinationList(Entities.Pack aPack)
        {
            var ret = new List<int>();
            string[] parts = aPack.Name.Split('_');
            if (parts.Length > 1)
            {
                string[] terminals = parts[1].ToUpper().Split("T".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string terminal in terminals)
                {
                    ret.Add(int.Parse(terminal));
                }
            }

            return ret.ToArray();
        }

        #endregion

        #region Events

        public event EventHandler<PackProcessingEventArgs> Exporting;
        public event EventHandler<PackProcessingEventArgs> Exported;
        public event EventHandler<PackProcessingEventArgs> Importing;
        public event EventHandler<PackProcessingEventArgs> Imported;
        public event ErrorEventHandler Error;

        private void OnExporting(PackProcessingEventArgs e)
        {
            if (Exporting != null)
                Exporting(this, e);
        }

        private void OnExported(PackProcessingEventArgs e)
        {
            if (Exported != null)
                Exported(this, e);
        }

        private void OnImporting(PackProcessingEventArgs e)
        {
            if (Importing != null)
                Importing(this, e);
        }

        private void OnImported(PackProcessingEventArgs e)
        {
            if (Imported != null)
                Imported(this, e);
        }

        private void OnError(ErrorEventArgs e)
        {
            if (Error != null)
                Error(this, e);
        }

        #endregion

        private readonly ITerminal _terminal;
        private string ImportPackPath = "";
        private string _infoFileName = "Info_";
        public string ConnectionID { get; set; }

        private PackInfoBase PackInfo { get; set; }

        protected PackManager()
        {
            _terminal = null;
        }

        protected PackManager(ITerminal terminal)
        {
            _terminal = terminal;
        }

        #region Public Methods

        public bool Import(string packPath)
        {
            bool ret = true;
            try
            {
                ImportPackPath = packPath;
                InternalImport();
            }
            catch (Exception ex)
            {
                ret = false;
                var e = new ErrorEventArgs(ex);
                OnError(e);
            }

            return ret;
        }

        public bool Export(PackInfoBase info)
        {
            bool ret = true;
            try
            {
                
                PackInfo = info;
                PackInfo.TerminalCode = _terminal.TerminalCode;
                PackInfo.WorkingDirectory = Path.Combine(PackInfo.RootDirectory, Guid.NewGuid().ToString());
                if (!Directory.Exists(PackInfo.WorkingDirectory))
                    Directory.CreateDirectory(PackInfo.WorkingDirectory);

                InternalExport();
            }
            catch (Exception ex)
            {
                ret = false;
                var e = new ErrorEventArgs(ex);
                OnError(e);
            }

            return ret;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Clean();
        }

        #endregion

        #endregion

        protected virtual void ExportProcess(PackProcessingEventArgs args)
        {
            OnExporting(args);
        }

        protected virtual void ImportProcess(PackProcessingEventArgs args)
        {
            OnExporting(args);
        }

        #region Private methods

        private void InternalExport()
        {
            var args = new PackProcessingEventArgs {PackInfo = PackInfo};
            ExportProcess(args);
            SerializePackInfo();
            CreateZip();
            Clean();
            OnExported(args);
        }

        private void InternalImport()
        {
            Entities.Pack aPack = null;
            CommonEntitiesManager dbent = null;
            var args = new PackProcessingEventArgs();
            try
            {
                dbent = new CommonEntitiesManager();
                
                aPack = InsertPackInDb(ImportPackPath, dbent);
                string workingDirectory = Path.Combine(Path.GetDirectoryName(ImportPackPath), Path.GetFileNameWithoutExtension(ImportPackPath)); ;

                UpdatePackStatus(aPack, dbent, PackStatus.Starting, null);
                args.Pack = aPack;

                ExtractZip(ImportPackPath, workingDirectory);
                DeserializePackInfo(workingDirectory);
                PackInfo.TerminalToCodes = new List<int>(GetTerminalDestinationList(args.Pack));
                args.PackInfo = PackInfo;
                
                aPack.IsMasterData = aPack.IsMasterData.GetValueOrDefault(false);
                aPack.IsUpgrade = aPack.IsUpgrade.GetValueOrDefault(false);
                UpdatePackStatus(aPack, dbent, PackStatus.InProgress, null);
                ImportProcess(args);
                UpdatePackStatus(aPack, dbent, PackStatus.Imported, null);
                OnImported(args);
                
                Clean();

            }
            catch (Exception ex)
            {
                if (dbent != null && aPack != null)
                    UpdatePackStatus(aPack, dbent, PackStatus.Error, ex.ToString());

                throw;
            }
            finally
            {
                if (dbent != null)
                {
                    dbent.Dispose();
                }
            }
        }

        private static void UpdatePackStatus(Entities.Pack aPack, CommonEntitiesManager dbent, PackStatus newStatus, string message)
        {
            aPack.Stamp = DateTime.Now;
            aPack.PackStatusCode = (int)newStatus;
            aPack.Result = message != null ? aPack.Result +"\n"+ message : aPack.Result;
            dbent.SaveChanges();
        }

        private Entities.Pack InsertPackInDb(string packFilePath, CommonEntitiesManager dbent)
        {
            if (dbent == null) throw new ArgumentNullException("dbent");

            string name = Path.GetFileName(packFilePath);
            Entities.Pack P = dbent.Packs.FirstOrDefault(p => p.Name == name);
            if (default(Entities.Pack) == P)
            {
                P = Entities.Pack.CreatePack(0, true);
                P.Name = name;
                P.Data = File.ReadAllBytes(packFilePath);
                if (!string.IsNullOrWhiteSpace(ConnectionID))
                    P.ConnectionCode = ConnectionID;
                dbent.AddToPacks(P);
                dbent.SaveChanges();
            }

            return P;

        }

        private void CreateZip()
        {
            var events = new FastZipEvents();
            var zip = new FastZip(events);
            var terminals = new StringBuilder();
            foreach (var terminal in PackInfo.TerminalToCodes)
            {
                terminals.AppendFormat("T{0}", terminal);
            }
            zip.CreateZip(Path.Combine(PackInfo.RootDirectory, string.Format(kPackNameFromat, PackInfo.ModuleCode, terminals, PackInfo.Stamp.ToString("yyyyMMddhhmmss"))), PackInfo.WorkingDirectory, true, "");
        }

        private void ExtractZip(string packFilePath, string dir)
        {
            var events = new FastZipEvents();
            var zip = new FastZip(events);
            zip.ExtractZip(packFilePath, dir, FastZip.Overwrite.Always, null, "", "", false);
        }

        private void SerializePackInfo()
        {
            var writer = new XmlSerializer(PackInfo.GetType());
            using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(PackInfo.WorkingDirectory,_infoFileName+ PackInfo.GetType())))
            {
                writer.Serialize(xmlwriter, PackInfo);
                xmlwriter.Close();
            }
        }

        private void DeserializePackInfo(string dir)
        {
            string[] files = Directory.GetFiles(dir, _infoFileName + "*.*");
            if (files.Length > 0)
            {
                Type infoType = Type.GetType(Path.GetFileName(files[0]).Remove(0, _infoFileName.Length));
                var reader = new XmlSerializer(infoType);
                using (XmlReader xmlreader = XmlReader.Create(files[0]))
                {
                    PackInfo = (PackInfoBase)reader.Deserialize(xmlreader);
                    xmlreader.Close();
                }

            }
            else
            {
                if (PackInfo == null)
                    PackInfo = new PackInfoBase { TerminalCode = -1, ModuleCode = GetModule(ImportPackPath), RootDirectory = ImportPackPath, Stamp = DateTime.Now };

            }

            PackInfo.WorkingDirectory = dir;
        }

        private void Clean()
        {
            if (Directory.Exists(PackInfo.WorkingDirectory))
                Directory.Delete(PackInfo.WorkingDirectory, true);

            if (!string.IsNullOrEmpty(ImportPackPath) && File.Exists(ImportPackPath))
                File.Delete(ImportPackPath);
        }

        #endregion

    }
}
