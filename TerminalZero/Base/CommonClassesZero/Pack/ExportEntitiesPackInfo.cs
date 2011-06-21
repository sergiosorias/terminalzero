using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroCommonClasses.Pack
{
    [DataContract]
    public class ExportEntitiesPackInfo : PackInfoBase 
    {
        public ExportEntitiesPackInfo()
        {
            
        }

        public ExportEntitiesPackInfo(int moduleCode, string workingDir)
        {
            ModuleCode = moduleCode;
            RootDirectory = workingDir;
            Tables = new List<PackTableInfo>();
        }

        [DataMember]
        public int TableCount { get; set; }

        [IgnoreDataMember]
        public bool SomeEntityHasRows 
        {
            get { return TableCount > 0; }
        }

        [DataMember]
        public List<PackTableInfo> Tables { get; set; }

        public void AddTable<T>(IEnumerable<T> entity)
        {
            if (entity != null)
            {
                PackTableInfo inf;
                if (entity.FirstOrDefault() is IExportableEntity)
                    inf = PackTableInfo.Create(entity.Where(item => ((IExportableEntity)item).Status == (int)EntityStatus.New || ((IExportableEntity)item).Status == (int)EntityStatus.Modified));    
                else
                    inf = PackTableInfo.Create(entity);
                if (inf.RowsCount > 0)
                {
                    TableCount++;
                    Tables.Add(inf);
                    Token();
                }

            }
        }
        
        public bool ContainsTable<T>()
        {
            string typeToSearch = typeof(T).ToString();
            return Tables.Count>0 && Tables.FirstOrDefault(table => table.RowTypeName == typeToSearch)!=null;
        }

        public IEnumerable<T> GetTable<T>()
        {
            string typeToSearch = typeof(T).ToString();
            return Tables.FirstOrDefault(table => table.RowTypeName == typeToSearch).GetRows<T>(WorkingDirectory);
        }

        public void ExportTables()
        {
            IEnumerable<IExportableEntity> exportableEntities;
            foreach (PackTableInfo info in Tables)
            {
                info.SerializeRows(WorkingDirectory);
                exportableEntities = info.GetRows().OfType<IExportableEntity>();
                foreach (IExportableEntity exportable in exportableEntities)
                {
                    exportable.UpdateStatus(EntityStatus.Exported);
                }
                foreach (int terminalToCode in exportableEntities.Select(item => item.TerminalDestination).Distinct())
                {
                    if (!TerminalToCodes.Contains(terminalToCode))
                    {
                        TerminalToCodes.Add(terminalToCode);
                    }
                }
                
            }
        }

        public void ImportTables(System.Data.Objects.ObjectContext context)
        {
            foreach (PackTableInfo info in Tables)
            {
                ContextExtentions.ImportEntities(context, info.GetRows(WorkingDirectory, System.Reflection.Assembly.GetAssembly(context.GetType())), SetEntityAsImported);
            }
        }

        private static void SetEntityAsImported(EntityObject entity)
        {
            if (entity is IExportableEntity)
                ((IExportableEntity) entity).UpdateStatus(EntityStatus.Imported);
        }

        public void MergeTables(System.Data.Objects.ObjectContext context)
        {
            foreach (PackTableInfo info in Tables)
            {
                ContextExtentions.MergeEntities(context, info.GetRows(WorkingDirectory, System.Reflection.Assembly.GetAssembly(context.GetType())), SetEntityAsImported);
            }
        }
    }
}
