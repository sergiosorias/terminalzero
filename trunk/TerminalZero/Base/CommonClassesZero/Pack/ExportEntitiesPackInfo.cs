using System.Collections.Generic;
using System.Runtime.Serialization;

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
            Path = workingDir;
            Tables = new List<PackTableInfo>();
        }

        [DataMember]
        public int TableCount { get; set; }

        [IgnoreDataMember]
        public List<PackTableInfo> Tables { get; set; }

        public void AddTable<T>(IEnumerable<T> entity)
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
    }
}
