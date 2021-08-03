﻿using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.Storage.Table.Repository
{

    [AttributeUsage(AttributeTargets.Property)]
    public class EntityJsonPropertyAttribute : Attribute
    {
        public EntityJsonPropertyAttribute()
        {
        }
    }

    internal class EntityJsonPropertyConverter
    {
        public static void Serialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> results)
        {
            entity.GetType().GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(EntityJsonPropertyAttribute), false).Count() > 0)
                .ToList()
                .ForEach(x => results.Add(x.Name, new EntityProperty(JsonConvert.SerializeObject(x.GetValue(entity)))));
        }

        public static void Deserialize<TEntity>(TEntity entity, IDictionary<string, EntityProperty> properties)
        {
            entity.GetType().GetProperties()
                .Where(x => x.GetCustomAttributes(typeof(EntityJsonPropertyAttribute), false).Count() > 0)
                .ToList()
                .ForEach(x => x.SetValue(entity, JsonConvert.DeserializeObject(properties[x.Name].StringValue, x.PropertyType)));
        }
    }
}
