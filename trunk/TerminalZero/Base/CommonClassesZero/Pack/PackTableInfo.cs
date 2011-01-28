using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace ZeroCommonClasses.Pack
{
    [DataContract]
    public class PackTableInfo
    {
        private PackTableInfo()
        {
            
        }

        public static PackTableInfo Create<T>(IEnumerable<T> entity)
        {
            var inf = new PackTableInfo();
            inf.SetContent(entity.ToList());
            return inf;
        }

        [DataMember]
        public int RowsCount { get; set; }
        [DataMember]
        public string RowTypeName { get; set; }
        [DataMember]
        public int Part { get; set; }
        [XmlIgnore]
        private XmlSerializer serializer;

        [XmlIgnore] 
        private object Rows;

        public IEnumerable<T> GetRowsAs<T>()
        {
            return (IEnumerable<T>) Rows;
        }

        private void SetContent<T>(IEnumerable<T> content)
        {
            RowTypeName = typeof(T).ToString();
            RowsCount = content.Count();
            serializer = new XmlSerializer(typeof(List<T>));
            Rows = content.ToList();
        }

        public void SerializeRows(string directory)
        {
            using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(directory, RowTypeName)))
            {
                serializer.Serialize(xmlwriter, Rows);
                xmlwriter.Close();
            }
        }

        public IEnumerable<T> DeserializeRows<T>(string fileDirectory)
        {
            XmlReader file = XmlReader.Create(Path.Combine(fileDirectory, RowTypeName));
            try
            {
                serializer = serializer ?? new XmlSerializer(typeof(List<T>));
                Rows = serializer.Deserialize(file);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                file.Close();
            }


            return Rows as IEnumerable<T>;
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
}