using Azure.Storage.Table.Repository.Converter;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace Azure.Storage.Table.Repository
{
    public abstract class EntityBase : TableEntity, ITableEntity
    {
        public string Id
        {
            get
            {
                SetNewId();
                return this.PartitionKey;
            }
            set { this.PartitionKey = value; this.RowKey = value; }
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            EntityJsonPropertyConverter.Serialize(this, results);
            EntityEnumPropertyConverter.Serialize(this, results);

            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);
            EntityJsonPropertyConverter.Deserialize(this, properties);
            EntityEnumPropertyConverter.Deserialize(this, properties);
        }

        public void SetNewId()
        {
            if (string.IsNullOrEmpty(this.PartitionKey))
            {
                this.PartitionKey = Guid.NewGuid().ToString();
                this.RowKey = this.PartitionKey;
            }
        }

    }
}
