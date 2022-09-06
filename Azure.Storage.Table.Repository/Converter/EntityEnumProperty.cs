using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.Storage.Table.Repository.Converter
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityEnumPropertyConverterAttribute : Attribute
    {
        public EntityEnumPropertyConverterAttribute()
        {
        }
    }

    public class EntityEnumPropertyConverter
    {
        public static void Serialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> results)
        {
            entity.GetType().GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(EntityEnumPropertyConverterAttribute), false).Count() > 0)
                .ToList()
                .ForEach(x => {
                    if (results.ContainsKey(x.Name)) results.Remove(x.Name);
                    results.Add(x.Name, new EntityProperty(x.GetValue(entity) != null ? x.GetValue(entity).ToString() : null));
                });
        }

        public static void Deserialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
        {
            entity.GetType().GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(EntityEnumPropertyConverterAttribute), false).Count() > 0)
                .ToList()
                .ForEach(x => x.SetValue(entity, Enum.Parse(x.PropertyType, properties[x.Name].StringValue)));
        }
    }
}
