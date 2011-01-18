using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using ZeroCommonClasses.Entities;

namespace ZeroCommonClasses.PackClasses
{
    public abstract class PackManager : IDisposable
    {
        public const string kPackNameFromat = "{0}_{1}_{2}{3}";
        public const string kPackExtention = ".zpack";

        [Flags()]
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
            List<string> res = new List<string>();
            res.AddRange(Directory.GetFiles(workingDirectory, moduleCode.ToString() + "*" + kPackExtention));
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
        public event EventHandler UnknownModeRecieved;
        public event ErrorEventHandler Error;

        private void OnUnknownModeRecieved()
        {
            if (UnknownModeRecieved != null)
                UnknownModeRecieved(this, EventArgs.Empty);
        }

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

        private Mode RunMode = Mode.Import;
        private string WorkingDirectory = "";
        private string ImportPackPath = "";
        public string ConnectionID { get; set; }

        private PackInfoBase PackInfo { get; set; }

        public PackManager(string packPath)
        {
            RunMode = Mode.Import;
            WorkingDirectory = Path.Combine(Path.GetDirectoryName(packPath), Path.GetFileNameWithoutExtension(packPath));
            ImportPackPath = packPath;
        }

        public PackManager(PackInfoBase info)
        {
            RunMode = Mode.Export;
            PackInfo = info;
            SetCurrentDirectory();

        }

        #region Public Methods

        public bool Process()
        {
            bool ret = true;

            try
            {
                switch (RunMode)
                {
                    case Mode.Export:
                        ExportProcess();
                        break;
                    case Mode.Import:
                        ImportProcess();
                        break;
                    default:
                        OnUnknownModeRecieved();
                        ret = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                ret = false;
                ErrorEventArgs e = new ErrorEventArgs(ex);
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
            PackEventArgs args = new PackEventArgs();

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
            FastZipEvents events = new FastZipEvents();
            FastZip zip = new FastZip(events);
            zip.CreateZip(Path.Combine(PackInfo.Path, string.Format(kPackNameFromat, PackInfo.ModuleCode, PackInfo.Flags, PackInfo.Stamp.ToString("yyyyMMddhhmmss"), kPackExtention)), WorkingDirectory, true, "");
        }

        private void ImportProcess()
        {
            Pack aPack = null;
            CommonEntities dbent = null;
            PackEventArgs args = new PackEventArgs();
            try
            {
                dbent = new CommonEntities();

                aPack = InsertPackInDB(ImportPackPath, dbent);
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

        private Pack InsertPackInDB(string packFilePath, CommonEntities dbent)
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
            FastZipEvents events = new FastZipEvents();
            FastZip zip = new FastZip(events);
            zip.ExtractZip(packFilePath, WorkingDirectory, FastZip.Overwrite.Always, null, "", "", false);
        }

        private void SetCurrentDirectory()
        {
            WorkingDirectory = Path.Combine(PackInfo.Path, Guid.NewGuid().ToString());
            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);
        }

        private void AddPackageData()
        {
            XmlSerializer writer = new XmlSerializer(PackInfo.GetType());
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
