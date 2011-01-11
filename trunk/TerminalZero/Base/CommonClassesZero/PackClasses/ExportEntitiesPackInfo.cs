using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.GlobalObjects;
using System.Runtime.Serialization;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace ZeroCommonClasses.PackClasses
{
    [DataContract]
    public class PackTableInfo
    {
        public static PackTableInfo Create<T>(IEnumerable<T> entity)
        {
            PackTableInfo inf = new PackTableInfo();
            inf.SetContent(entity);
            return inf;
        }

        [DataMember]
        public int RowsCount { get; set; }
        [DataMember]
        public string RowTypeName { get; set; }
        [DataMember]
        public int Part { get; set; }
        [XmlIgnore]
        public Type RowType { get; set; }
        [XmlIgnore]
        private XmlSerializer serializer;
        [XmlIgnore]
        private object Rows;

        private void SetContent<T>(IEnumerable<T> content)
        {
            RowType = typeof(T);
            RowTypeName = RowType.ToString();
            RowsCount = content.Count();
            serializer = new XmlSerializer(typeof(List<T>));
            Rows = content.ToList();
        }

        public void SerializeRows(XmlWriter file)
        {
            serializer.Serialize(file, Rows);
        }

        public List<T> DeserializeRows<T>(string fileDirectory)
        {
            List<T> ret;
            XmlReader file = XmlReader.Create(Path.Combine(fileDirectory, RowTypeName));
            try
            {
                serializer = serializer ?? new XmlSerializer(typeof(List<T>));
                ret = (List<T>)serializer.Deserialize(file);
            }
            catch (Exception ex)
            {
                ret = null;
                throw ex;
            }
            finally
            {
                file.Close();
            }


            return ret;
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == typeof(string))
            {
                return RowTypeName == (string)obj;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract]
    public class ExportEntitiesPackInfo : PackInfoBase
    {
        private ExportEntitiesPackInfo()
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
            TableCount++;
            Tables.Add(PackTableInfo.Create(entity));
            Token();
        }
    }
}
