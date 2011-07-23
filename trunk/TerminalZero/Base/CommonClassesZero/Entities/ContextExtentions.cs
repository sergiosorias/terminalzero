using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ZeroCommonClasses.Interfaces;

namespace ZeroCommonClasses.Entities
{
    public static class  ContextExtentions
    {
        public static EntityValidationResult ValidateEntity(object entity)
        {
            ValidationContext c = new ValidationContext(entity, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
                
            bool isValid = Validator.TryValidateObject(entity, c, validationResults, true);

            return new EntityValidationResult {IsValid = isValid, Errors = validationResults.Select(s=>s.ErrorMessage)};
        }

        public static string GetEntitiesAsXMLObjectList<T>(IEnumerable<T> list)
        {
            var ser = new XmlSerializer(typeof(List<T>));
            string ret;
            using(var sw = new StringWriter())
            {
                ser.Serialize(sw, list.ToList());
                ret = sw.ToString();
                sw.Close();
            }
            return ret;
        }

        public static IEnumerable<T> GetEntitiesFromXMLObjectList<T>(string list)
        {
            var ser = new XmlSerializer(typeof(List<T>));
            List<T> ret = null;
            using(var sr = new StringReader(list))
            {
                ret = (List<T>) ser.Deserialize(sr);
                sr.Close();
            }
            return ret;
        }

        public static DataTable ToADOTable<T>(IEnumerable<T> query)
        {
            var dtReturn = new DataTable();
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
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(record, null) ?? DBNull.Value;
                }

                dtReturn.Rows.Add(dataRow);
            }

            return dtReturn;
        }

        private static void MergeItem<T>(IEnumerable<T> list, T mergeItem, Action<T> insertMethod, MergeOption mergeOptions, string[] PropertiesKey, Dictionary<string, PropertyInfo> baseProperties, Dictionary<string, PropertyInfo> columnProperties) 
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
            else if (mergeOptions != MergeOption.AppendOnly)
            {
                foreach (var property in columnProperties.Keys.Where(p => !PropertiesKey.Contains(p)))
                {
                    columnProperties[property].SetValue(aux, columnProperties[property].GetValue(mergeItem, null), null);
                }
            }
        }

        public static void MergeEntities(ObjectContext context, IEnumerable<EntityObject> entities, Action<EntityObject> mergingEntityCallback)
        {
            foreach (var item in entities)
            {
                if (mergingEntityCallback != null)
                    mergingEntityCallback(item);
                MergeEntity(item, context);
            }
        }

        private static void MergeEntity(EntityObject item, ObjectContext context)
        {
            object entity;
            if (context.TryGetObjectByKey(item.EntityKey, out entity))
            {
                context.ApplyCurrentValues(item.EntityKey.EntitySetName, item);
            }
            else
            {
                context.AddObject(item.EntityKey.EntitySetName, item);
            }
        }

        public static void ImportEntities(ObjectContext context, IEnumerable<EntityObject> entities, Action<EntityObject> importingEntityCallback)
        {
            foreach (var item in entities)
            {
                if (importingEntityCallback != null)
                    importingEntityCallback(item);
                context.AddObject(item.EntityKey.EntitySetName, item);
            }
        }
        
        public static void DetachEntities<T>(ObjectContext contect, EntityObject parent, params EntityCollection<T>[] childs)
            where T : EntityObject
        {
            if (childs != null)
            {
                EntityObject obj;
                foreach (EntityCollection<T> entityCollection in childs)
                {
                    while (entityCollection.Count > 0)
                    {
                        obj = entityCollection.First();
                        if (obj.EntityState == EntityState.Unchanged)
                        {
                            throw new InvalidOperationException(
                                string.Format("Invalid detach operation was tried, Entity: {0}",
                                              obj.EntityKey.EntitySetName));

                        }
                        contect.Detach(entityCollection.First());
                    }
                }
            }
            if (parent != null)
            {
                switch (parent.EntityState)
                {
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    case EntityState.Deleted:
                        throw new InvalidOperationException(
                                string.Format("Invalid detach operation was tried, Entity: {0}",
                                              parent.EntityKey.EntitySetName));
                        break;
                    case EntityState.Added:
                    case EntityState.Modified:
                        contect.Detach(parent);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
        }

        private static class ObsoleteMethods
        {
            [Obsolete]
            public static void Merge<T>(IEnumerable<T> targetList, T mergeItem, Action<T> insertMethod, MergeOption mergeOptions,
                params string[] PropertiesKey)
                where T : EntityObject
            {

                Type entobj = typeof(EntityObject);
                Type rela = typeof(EntityReference);
                Type key = typeof(EntityKey);

                Dictionary<string, PropertyInfo> baseProperties = entobj.GetProperties().ToDictionary(p => p.Name);
                Dictionary<string, PropertyInfo> columnProperties = typeof(T).GetProperties().ToList().Where(cp =>
                        cp.CanWrite
                        && cp.PropertyType != key
                        && cp.PropertyType.BaseType != rela
                        && cp.PropertyType.BaseType != entobj
                        ).ToDictionary(p => p.Name);

                MergeItem(targetList, mergeItem, insertMethod, mergeOptions, PropertiesKey, baseProperties, columnProperties);

            }
            [Obsolete]
            public static void Merge<T>(IEnumerable<T> targetList, IEnumerable<T> sourceList, Action<T> insertMethod, MergeOption mergeOptions,
                params string[] PropertiesKey)
                where T : EntityObject
            {

                Type entobj = typeof(EntityObject);
                Type rela = typeof(EntityReference);
                Type baseEntColl = typeof(RelatedEnd);
                Type key = typeof(EntityKey);

                Dictionary<string, PropertyInfo> baseProperties = entobj.GetProperties().ToDictionary(p => p.Name);
                Dictionary<string, PropertyInfo> columnProperties = typeof(T).GetProperties().ToList().Where(cp =>
                        cp.CanWrite
                        && cp.PropertyType != key
                        && cp.PropertyType.BaseType != rela
                        && cp.PropertyType.BaseType != entobj
                        && cp.PropertyType.BaseType != baseEntColl
                        ).ToDictionary(p => p.Name);

                foreach (var item in sourceList)
                {
                    if (item is IExportableEntity)
                    {
                        IExportableEntity ent = item as IExportableEntity;
                        ent.UpdateStatus(EntityStatus.Imported);
                    }
                    MergeItem(targetList, item, insertMethod, mergeOptions, PropertiesKey, baseProperties, columnProperties);
                }


            }
        }
    }
}
