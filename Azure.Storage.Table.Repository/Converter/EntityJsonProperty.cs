using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.Storage.Table.Repository
{

    internal class EntityJsonPropertyConverter
    {
        public static void Serialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> results)
        {
            ((System.Reflection.TypeInfo)entity.GetType()).DeclaredProperties
                .Where(x => x.PropertyType is object && !results.ContainsKey(x.Name))
                .ToList()
                .ForEach(x =>
                {
                    var p = entity.GetType().GetNestedTypes();
                    if (x.GetValue(entity) == null)
                        results.Add(x.Name, new EntityProperty("null"));
                    else
                        results.Add(x.Name, new EntityProperty(JsonConvert.SerializeObject(x.GetValue(entity))));
                });
        }

        public static void Deserialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
        {
            ((System.Reflection.TypeInfo)entity.GetType()).DeclaredProperties
                .Where(x => x.PropertyType.IsClass)
                .ToList()
                .ForEach(x =>
                {
                    if (properties.ContainsKey(x.Name)
                        && parseJson(properties[x.Name].StringValue, x.PropertyType, out object result))
                    {
                        x.SetValue(entity, result);
                    }
                });
        }

        private static bool parseJson(string val, Type propertyType, out object result)
        {
            try
            {
                result = JsonConvert.DeserializeObject(val, propertyType);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }
}
