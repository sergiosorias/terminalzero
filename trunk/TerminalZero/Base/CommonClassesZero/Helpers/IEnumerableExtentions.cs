using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Data.Objects;

namespace ZeroCommonClasses.Helpers
{
    public static class  IEnumerableExtentions
    {
        public static string GetEntitiesAsXMLObjectList<T>(IEnumerable<T> list)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
            StringWriter sw = new StringWriter();
            ser.Serialize(sw, list.ToList());
            string ret = sw.ToString();
            sw.Close();
            sw.Dispose();
            return ret;
        }

        public static IEnumerable<T> GetEntitiesFromXMLObjectList<T>(string list)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
            List<T> ret = null;
            StringReader sr = new StringReader(list);
            ret = (List<T>)ser.Deserialize(sr);
            sr.Close();
            sr.Dispose();
            return ret;
        }

        public static DataTable ToADOTable<T>(IEnumerable<T> query)
        {
            DataTable dtReturn = new DataTable();
            PropertyInfo[] columnProperties = null;

            columnProperties = typeof(T).GetProperties();
            foreach (PropertyInfo propertyInfo in columnProperties)
            {
                // sort out the issue of nullable types
                Type columnType = propertyInfo.PropertyType;
                if ((columnType.IsGenericType) && (columnType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    columnType = columnType.GetGenericArguments()[0];
                }

                if (!columnType.Name.Contains("Entity") && columnType.BaseType != typeof(EntityObject))
                    dtReturn.Columns.Add(new DataColumn(propertyInfo.Name, columnType));
            }

            foreach (var record in query)
            {
                DataRow dataRow = dtReturn.NewRow();

                foreach (PropertyInfo propertyInfo in columnProperties)
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(record, null) == null ? DBNull.Value : propertyInfo.GetValue(record, null);
                }

                dtReturn.Rows.Add(dataRow);
            }

            return dtReturn;
        }
        
        public static void Merge<T>(IEnumerable<T> targetList, T mergeItem, Action<T> insertMethod, System.Data.Objects.MergeOption mergeOptions, 
            params string[] PropertiesKey)
            where T : EntityObject
        {
                       
            Type entobj = typeof(EntityObject);
            Type rela = typeof(EntityReference);
            Type key = typeof(EntityKey);

            Dictionary<string, PropertyInfo> baseProperties = entobj.GetProperties().ToDictionary(p=>p.Name);
            Dictionary<string, PropertyInfo> columnProperties = typeof(T).GetProperties().ToList().Where(cp =>
                    cp.CanWrite
                    && cp.PropertyType != key
                    && cp.PropertyType.BaseType != rela
                    && cp.PropertyType.BaseType != entobj
                    ).ToDictionary(p=>p.Name);

            MergeItem<T>(targetList, mergeItem, insertMethod, mergeOptions, PropertiesKey, baseProperties, columnProperties);

        }

        public static void Merge<T>(IEnumerable<T> targetList, IEnumerable<T> sourceList, Action<T> insertMethod, System.Data.Objects.MergeOption mergeOptions, 
            params string[] PropertiesKey)
            where T : EntityObject
        {
                       
            Type entobj = typeof(EntityObject);
            Type rela = typeof(EntityReference);
            Type baseEntColl = typeof(RelatedEnd);
            Type key = typeof(EntityKey);

            Dictionary<string, PropertyInfo> baseProperties = entobj.GetProperties().ToDictionary(p=>p.Name);
            Dictionary<string, PropertyInfo> columnProperties = typeof(T).GetProperties().ToList().Where(cp =>
                    cp.CanWrite
                    && cp.PropertyType != key
                    && cp.PropertyType.BaseType != rela
                    && cp.PropertyType.BaseType != entobj
                    && cp.PropertyType.BaseType != baseEntColl
                    ).ToDictionary(p=>p.Name);

            foreach (var item in sourceList)
            {
                MergeItem<T>(targetList, item, insertMethod, mergeOptions, PropertiesKey, baseProperties, columnProperties);
            }
            

        }
        
        private static void MergeItem<T>(IEnumerable<T> list, T mergeItem, Action<T> insertMethod, System.Data.Objects.MergeOption mergeOptions, string[] PropertiesKey, Dictionary<string, PropertyInfo> baseProperties, Dictionary<string, PropertyInfo> columnProperties) 
            where T : EntityObject
        {
            foreach (var Property in baseProperties.Keys)
            {
                if (baseProperties[Property].CanWrite)
                    baseProperties[Property].SetValue(mergeItem, null, null);
            }

            T aux = null;
            foreach (var item in list)
            {
                bool allOK = true;
                foreach (var column in PropertiesKey)
                {
                    PropertyInfo pInf = columnProperties[column];
                    if (!pInf.GetValue(mergeItem, null).Equals(pInf.GetValue(item, null)))
                    {
                        allOK = false;
                        break;
                    }
                }
                if (allOK)
                {
                    aux = item;
                    break;
                }
            }

            //si no existe, la inserto, sino.. la actualizo
            if (aux == null)
            {
                insertMethod.Invoke(mergeItem);
            }
            else if (mergeOptions != System.Data.Objects.MergeOption.AppendOnly)
            {
                foreach (var property in columnProperties.Keys.Where(p => !PropertiesKey.Contains(p)))
                {
                    columnProperties[property].SetValue(aux, columnProperties[property].GetValue(mergeItem, null), null);
                }
            }
        }

        //public static void Merge<T>(ObjectSet<T> list, T item)
        //    where T : EntityObject
        //{
        //    T found = list.FirstOrDefault(i => i.EntityKey == item.EntityKey);
        //    if (found == default(T))
        //    {
        //        list.AddObject(item);
        //    }
        //    else
        //    {
        //        list.ApplyCurrentValues(item);
        //    }

        //}

        //public static void Merge(ObjectContext context, EntityObject item)
        //{
        //    object obj = null;
        //    if (context.TryGetObjectByKey(item.EntityKey, out obj))
        //        context.ApplyCurrentValues(item.EntityKey.EntitySetName, item);
        //    else
        //        context.AddObject(item.EntityKey.EntitySetName, item);
        //}
    }
}
