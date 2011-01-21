using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroCommonClasses.PackClasses
{
    public abstract class PackManager : IDisposable
    {
        public const string kPackExtention = ".zpack";
        public const string kPackNameFromat = "{0}_{1}_{2}_{3}" + kPackExtention;
        
        [Flags]
        public enum PackFlags
        {
            MasterData = 2,
            Upgrade = 4,
        }

        private enum Mode
        {
            Export,
            Import,
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

        #endregion

        #region Events

        public event EventHandler<PackEventArgs> Exporting;
        public event EventHandler<PackEventArgs> Exported;
        public event EventHandler<PackEventArgs> Importing;
        public event EventHandler<PackEventArgs> Imported;
        public event ErrorEventHandler Error;

        private void OnExporting(PackEventArgs e)
        {
            if (Exporting != null)
                Exporting(this, e);
        }

        private void OnExported(PackEventArgs e)
        {
            if (Exported != null)
                Exported(this, e);
        }

        private void OnImporting(PackEventArgs e)
        {
            if (Importing != null)
                Importing(this, e);
        }

        private void OnImported(PackEventArgs e)
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
        private string WorkingDirectory = "";
        private string ImportPackPath = "";
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
                WorkingDirectory = Path.Combine(Path.GetDirectoryName(packPath), Path.GetFileNameWithoutExtension(packPath));
                ImportPackPath = packPath;
                ImportProcess();
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
                WorkingDirectory = Path.Combine(PackInfo.Path, Guid.NewGuid().ToString());
                if (!Directory.Exists(WorkingDirectory))
                    Directory.CreateDirectory(WorkingDirectory);

                ExportProcess();
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

        private void ExportProcess()
        {
            var args = new PackEventArgs();

            args.PackInfo = PackInfo;
            args.WorkingDirectory = WorkingDirectory;
            OnExporting(args);
            AddPackageData();
            CreateZip();
            Clean();
            OnExported(args);

        }

        private void CreateZip()
        {
            var events = new FastZipEvents();
            var zip = new FastZip(events);
            zip.CreateZip(Path.Combine(PackInfo.Path, string.Format(kPackNameFromat, PackInfo.ModuleCode, PackInfo.DestinationTerminalCode, PackInfo.Stamp.ToString("yyyyMMddhhmmss"))), WorkingDirectory, true, "");
        }

        private void ImportProcess()
        {
            Pack aPack = null;
            CommonEntities dbent = null;
            var args = new PackEventArgs();
            try
            {
                dbent = new CommonEntities();

                aPack = InsertPackInDb(ImportPackPath, dbent);
                args.WorkingDirectory = WorkingDirectory;

                UpdatePackStatus(aPack, dbent, 0, null);
                args.Pack = aPack;

                ExtractZip(ImportPackPath);

                UpdatePackStatus(aPack, dbent, 1, null);
                OnImporting(args);
                UpdatePackStatus(aPack, dbent, 2, null);
                OnImported(args);

                Clean();

            }
            catch (Exception ex)
            {
                if (dbent != null && aPack != null)
                    UpdatePackStatus(aPack, dbent, 3, ex.ToString());

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

        private static void UpdatePackStatus(Pack aPack, CommonEntities dbent, int newStatus, string message)
        {
            aPack.Stamp = DateTime.Now;
            aPack.PackStatusCode = newStatus;
            aPack.Result = message;
            dbent.SaveChanges();
        }

        private Pack InsertPackInDb(string packFilePath, CommonEntities dbent)
        {

            string name = Path.GetFileName(packFilePath);
            Pack P = dbent.Packs.FirstOrDefault(p => p.Name == name);
            if (default(Pack) == P)
            {
                P = Pack.CreatePack(0, true);
                P.Name = name;
                P.Data = File.ReadAllBytes(packFilePath);
                if (!string.IsNullOrWhiteSpace(ConnectionID))
                    P.ConnectionCode = ConnectionID;
                dbent.AddToPacks(P);
                dbent.SaveChanges();
            }

            return P;

        }

        private void ExtractZip(string packFilePath)
        {
            var events = new FastZipEvents();
            var zip = new FastZip(events);
            zip.ExtractZip(packFilePath, WorkingDirectory, FastZip.Overwrite.Always, null, "", "", false);
        }

        private void AddPackageData()
        {
            var writer = new XmlSerializer(PackInfo.GetType());
            using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(WorkingDirectory, PackInfo.GetType().ToString())))
            {
                writer.Serialize(xmlwriter, PackInfo);
                xmlwriter.Close();
            }
        }

        private void Clean()
        {
            if (Directory.Exists(WorkingDirectory))
                Directory.Delete(WorkingDirectory, true);

            if (!string.IsNullOrEmpty(ImportPackPath) && File.Exists(ImportPackPath))
                File.Delete(ImportPackPath);
        }

    }
}
