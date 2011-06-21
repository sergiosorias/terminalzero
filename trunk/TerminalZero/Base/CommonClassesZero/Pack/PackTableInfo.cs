using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace ZeroCommonClasses.Pack
{
    [DataContract]
    public class PackTableInfo
    {
        #region Builder
        public static PackTableInfo Create<T>(IEnumerable<T> entity)
        {
            var inf = new PackTableInfo();
            inf.SetContent(entity.ToList());
            return inf;
        }
        #endregion

        private PackTableInfo()
        {
            
        }

        #region Properties
        [DataMember]
        public int RowsCount { get; set; }
        [DataMember]
        public string RowTypeName { get; set; }
        [DataMember]
        public int Part { get; set; }
        [XmlIgnore]
        protected XmlSerializer Serializer;
        [XmlIgnore] 
        private object Rows;
        #endregion

        private void SetContent<T>(IEnumerable<T> content)
        {
            RowTypeName = typeof(T).ToString();
            RowsCount = content.Count();
            Serializer = new XmlSerializer(typeof(List<T>));
            Rows = content.ToList();
        }

        public void SerializeRows(string directory)
        {
            using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(directory, RowTypeName)))
            {
                Serializer.Serialize(xmlwriter, Rows);
                xmlwriter.Close();
            }
        }

        private void DeserializeRows(string fileDirectory)
        {
            XmlReader file = XmlReader.Create(Path.Combine(fileDirectory, RowTypeName));
            try
            {
                Serializer = Serializer ?? new XmlSerializer(Rows.GetType());
                Rows = Serializer.Deserialize(file);
            }
            finally
            {
                file.Close();
            }
        }

        public IEnumerable GetRows()
        {
            return (IEnumerable)Rows;
        }

        public IEnumerable<T> GetRows<T>(string fileDirectory)
        {
            if (Rows == null)
            {
                Rows = new List<T>();
            }

            return Rows as T[];
        }

        public IEnumerable<EntityObject> GetRows(string fileDirectory, Assembly metadataAssembly)
        {
            if (Rows == null)
            {
                Rows = Array.CreateInstance(metadataAssembly.GetType(RowTypeName), 0);
                DeserializeRows(fileDirectory);
            }

            return Rows as EntityObject[];
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