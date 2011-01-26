using System;
using System.Collections.Generic;
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
        public static PackTableInfo Create<T>(IEnumerable<T> entity)
        {
            var inf = new PackTableInfo();
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

        public IEnumerable<T> GetRowsAs<T>()
        {
            return (IEnumerable<T>) Rows;
        }

        private void SetContent<T>(IEnumerable<T> content)
        {
            RowType = typeof(T);
            RowTypeName = RowType.ToString();
            RowsCount = content.Count();
            serializer = new XmlSerializer(typeof(List<T>));
            Rows = content.ToList();
        }

        public void SerializeRows(string directory)
        {
            using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(directory, RowType.ToString())))
            {
                serializer.Serialize(xmlwriter, Rows);
                xmlwriter.Close();
            }
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
}