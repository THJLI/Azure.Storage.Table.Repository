using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace Azure.Storage.Table.Repository
{
    public abstract class EntityBase : TableEntity, ITableEntity
    {

        public EntityBase()
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = this.PartitionKey;
        }

        public string Id { get { return this.PartitionKey; } set { this.PartitionKey = value; this.RowKey = value; } }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            EntityJsonPropertyConverter.Serialize(this, results);
            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            EntityJsonPropertyConverter.Deserialize(this, properties);
        }

    }
}
