using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Entities;

namespace ZeroCommonClasses.PackClasses
{
    public abstract class PackManager : IDisposable
    {
        private const string kPackExtention = ".zpack";

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

        public static string[] GetPacks(string workingDirectory)
        {
            List<string> res = new List<string>();
            res.AddRange(Directory.GetFiles(workingDirectory, "*" + kPackExtention));
            return res.ToArray();
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
        private string CurrentDirectory = "";
        public string ConnectionID { get; set; }

        private PackInfoBase PackInfo { get; set; }

        public PackManager(string packPath)
        {
            RunMode = Mode.Import;
            CurrentDirectory = packPath;
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
            try
            {
                args.PackInfo = PackInfo;
                args.WorkingDirectory = CurrentDirectory;
                OnExporting(args);
                AddPackageData();
                CreateZip();
                Clean();
                OnExported(args);
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
            }
        }

        private void CreateZip()
        {
            FastZipEvents events = new FastZipEvents();
            FastZip zip = new FastZip(events);
            zip.CreateZip(Path.Combine(PackInfo.Path, PackInfo.ModuleCode.ToString() + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + kPackExtention), CurrentDirectory, true, "");
        }

        private void ImportProcess()
        {
            string[] packs = Directory.GetFiles(CurrentDirectory, "*" + kPackExtention);
            string dirAux = CurrentDirectory;
            Pack aPack = null;
            CommonEntities dbent = null;
            PackEventArgs args = new PackEventArgs();
            try
            {
                dbent = new CommonEntities();
                foreach (var item in packs)
                {
                    CurrentDirectory = Path.Combine(dirAux, Path.GetFileNameWithoutExtension(item));
                    aPack = InsertPackInDB(item, dbent);
                    args.WorkingDirectory = CurrentDirectory;

                    UpdatePackStatus(aPack, dbent, 0);
                    args.Pack = aPack;
                    
                    ExtractZip(item);
                    
                    UpdatePackStatus(aPack, dbent, 1);
                    OnImporting(args);
                    UpdatePackStatus(aPack, dbent, 2);
                    OnImported(args);
                    
                    Clean();
                }
            }
            catch (Exception ex)
            {
                if (dbent != null && aPack != null)
                    UpdatePackStatus(aPack, dbent, 3);
                
                ErrorEventArgs er = new ErrorEventArgs(ex);
                OnError(er);
            }
            finally
            {
                if (dbent != null)
                {
                    dbent.Dispose();
                }
            }


        }

        private static void UpdatePackStatus(Pack aPack, CommonEntities dbent, int newStatus)
        {
            aPack.Stamp = DateTime.Now;
            aPack.PackStatusCode = newStatus;
            dbent.SaveChanges();
        }

        private Pack InsertPackInDB(string packFilePath, CommonEntities dbent)
        {

            string name = Path.GetFileName(packFilePath);
            Pack P = dbent.Packs.FirstOrDefault(p=>p.Name == name);
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
            zip.ExtractZip(packFilePath, CurrentDirectory, FastZip.Overwrite.Always, null, "", "", false);
        }

        private void SetCurrentDirectory()
        {
            CurrentDirectory = Path.Combine(PackInfo.Path, Guid.NewGuid().ToString());
            if (!Directory.Exists(CurrentDirectory))
                Directory.CreateDirectory(CurrentDirectory);
        }

        private void AddPackageData()
        {
            XmlSerializer writer = new XmlSerializer(PackInfo.GetType());
            using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(CurrentDirectory, PackInfo.GetType().ToString())))
            {
                writer.Serialize(xmlwriter, PackInfo);
                xmlwriter.Close();
            }
        }

        private void Clean()
        {
            if (Directory.Exists(CurrentDirectory))
                Directory.Delete(CurrentDirectory, true);
        }

    }
}
