using System.Collections.Generic;
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

        public void AddEntities<T>(IEnumerable<T> entity)
        {
            if (entity != null)
            {
                PackTableInfo inf = PackTableInfo.Create(entity);
                if (inf.RowsCount > 0)
                {
                    TableCount++;
                    Tables.Add(inf);
                    Token();
                }

            }
        }

        public void AddExportableEntities<T>(IEnumerable<T> entity) where T : IExportableEntity
        {
            if (entity != null)
            {
                PackTableInfo inf = PackTableInfo.Create(entity.Where(item=>item.Status == (int)EntityStatus.New));
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
            foreach (PackTableInfo info in Tables)
            {
                info.SerializeRows(WorkingDirectory);

                foreach (IExportableEntity exportable in info.GetRows().OfType<IExportableEntity>())
                {
                    exportable.UpdateStatus(EntityStatus.Exported);
                    if (!TerminalToCodes.Contains(exportable.TerminalDestination))
                    {
                        TerminalToCodes.Add(exportable.TerminalDestination);
                    }
                }
            }

            
        }

        

    }
}
