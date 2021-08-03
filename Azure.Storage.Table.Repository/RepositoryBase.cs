using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Azure.Storage.Table.Repository
{
    public abstract class RepositoryBase<T>
        where T : EntityBase, new()
    {
        private readonly CloudTableClient _client;
        private readonly CloudTable _table;

        protected RepositoryBase(RepositorySettings repositorySettings)
        {
            var account = CloudStorageAccount.Parse(repositorySettings.AzureWebJobsStorage);
            _client = account.CreateCloudTableClient();
            _table = _client.GetTableReference(typeof(T).Name);
            _table.CreateIfNotExists();
        }

        public T GetById(string id)
        {
            var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id);
            var query = new TableQuery<T>().Where(condition);
            var lst = _table.ExecuteQuery(query);
            return lst.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            return _table.CreateQuery<T>().ToList();
        }

        public IEnumerable<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            var query = _table.CreateQuery<T>().Where(predicate);
            return query.ToList();
        }

        public bool Insert(T entity)
        {
            TableOperation op = TableOperation.Insert(entity);
            var result = _table.Execute(op);
            return result.HttpStatusCode == 200;
        }

        public bool Update(T entity)
        {
            TableOperation op = TableOperation.Replace(entity);
            var result = _table.Execute(op);
            return result.HttpStatusCode == 200;
        }

        public bool DeleteById(string id)
        {
            var obj = GetById(id);
            TableOperation op = TableOperation.Delete(obj);
            var result = _table.Execute(op);
            return result.HttpStatusCode == 200;
        }

        public bool DeleteBy(Expression<Func<T, bool>> predicate)
        {
            var lstObj = GetBy(predicate);
            var result = new List<int>();

            foreach (var obj in lstObj)
            {
                TableOperation op = TableOperation.Delete(obj);
                var res = _table.Execute(op);
                result.Add(res.HttpStatusCode);
            }
            return result.Any(e => e != 200);
        }

    }
}
