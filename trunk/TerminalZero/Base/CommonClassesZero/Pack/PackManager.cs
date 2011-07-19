using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using ZeroCommonClasses.Entities;

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

        public event EventHandler<PackProcessEventArgs> Exporting;
        public event EventHandler<PackProcessEventArgs> Exported;
        public event EventHandler<PackProcessEventArgs> Importing;
        public event EventHandler<PackProcessEventArgs> Imported;
        public event ErrorEventHandler Error;

        private void OnExporting(PackProcessEventArgs e)
        {
            if (Exporting != null)
                Exporting(this, e);
        }

        private void OnExported(PackProcessEventArgs e)
        {
            if (Exported != null)
                Exported(this, e);
        }

        private void OnImporting(PackProcessEventArgs e)
        {
            if (Importing != null)
                Importing(this, e);
        }

        private void OnImported(PackProcessEventArgs e)
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

        private string workingPackPath = "";
        private const string _infoFileName = "Info_";
        public string ConnectionID { get; set; }

        private PackInfoBase PackInfo { get; set; }

        #region Public Methods

        public bool Import(string packPath)
        {
            bool ret = true;
            try
            {
                workingPackPath = packPath;
                InternalImport();
            }
            catch (Exception ex)
            {
                ret = false;
                var e = new ErrorEventArgs(ex);
                OnError(e);
            }
            finally
            {
                Clean();
            }

            return ret;
        }

        public bool Export(string workingDir)
        {
            bool ret = true;
            try
            {
                workingPackPath = workingDir;
                PackInfo = BuildPackInfo();
                PackInfo.TerminalCode = Terminal.Instance.TerminalCode;
                PackInfo.WorkingDirectory = Path.Combine(workingPackPath, Guid.NewGuid().ToString());
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
            finally
            {
                Clean();
            }

            return ret;
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            Clean();
        }

        #endregion

        #endregion

        protected abstract PackInfoBase BuildPackInfo();

        protected virtual void ExportProcess(PackProcessEventArgs args)
        {
            OnExporting(args);
        }

        protected virtual void ImportProcess(PackProcessEventArgs args)
        {
            OnExporting(args);
        }

        #region Private methods

        private void InternalExport()
        {
            var args = new PackProcessEventArgs {PackInfo = PackInfo};
            ExportProcess(args);
            if (!args.Cancel)
            {
                SerializePackInfo();
                CreateZip();
                OnExported(args);
            }
        }

        private void InternalImport()
        {
            Entities.Pack aPack = null;
            CommonEntitiesManager dbent = null;
            var args = new PackProcessEventArgs();
            try
            {
                dbent = new CommonEntitiesManager();
                
                aPack = InsertPackInDb(workingPackPath, dbent);
                string workingDirectory = Path.Combine(Path.GetDirectoryName(workingPackPath), Path.GetFileNameWithoutExtension(workingPackPath)); ;

                UpdatePackStatus(aPack, dbent, PackStatus.Starting, null);
                args.Pack = aPack;

                ExtractZip(workingPackPath, workingDirectory);
                DeserializePackInfo(workingDirectory);
                PackInfo.TerminalToCodes = new List<int>(GetTerminalDestinationList(args.Pack));
                args.PackInfo = PackInfo;
                
                aPack.IsMasterData = aPack.IsMasterData.GetValueOrDefault(false);
                aPack.IsUpgrade = aPack.IsUpgrade.GetValueOrDefault(false);
                UpdatePackStatus(aPack, dbent, PackStatus.InProgress, null);
                ImportProcess(args);
                UpdatePackStatus(aPack, dbent, PackStatus.Imported, null);
                OnImported(args);
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
            zip.CreateZip(Path.Combine(workingPackPath, string.Format(kPackNameFromat, PackInfo.ModuleCode, terminals, PackInfo.Stamp.ToString("yyyyMMddhhmmss"))), PackInfo.WorkingDirectory, true, "");
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
                    PackInfo = new PackInfoBase { TerminalCode = -1, ModuleCode = GetModule(workingPackPath), Stamp = DateTime.Now };

            }

            PackInfo.WorkingDirectory = dir;
        }

        private void Clean()
        {
            if (Directory.Exists(PackInfo.WorkingDirectory))
                Directory.Delete(PackInfo.WorkingDirectory, true);

            if (!string.IsNullOrEmpty(workingPackPath) && File.Exists(workingPackPath))
                File.Delete(workingPackPath);
        }

        #endregion

    }
}
