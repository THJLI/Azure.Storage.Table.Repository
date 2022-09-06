using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public virtual T GetById(string id)
        {
            try
            {
                var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id);
                var query = new TableQuery<T>().Where(condition);
                var lst = _table.ExecuteQuery(query);
                return lst.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public virtual IEnumerable<T> GetAll()
        {
            try
            {
                return _table.CreateQuery<T>().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<T>();
            }
        }

        public virtual IEnumerable<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var query = _table.CreateQuery<T>().Where(predicate);
                return query.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<T>();
            }
        }

        public virtual bool Insert(T entity)
        {
            try
            {
                entity.SetNewId();

                TableOperation op = TableOperation.Insert(entity);
                var result = _table.Execute(op);
                return isValidResult(result.HttpStatusCode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public virtual bool Update(T entity)
        {
            try
            {
                TableOperation op = TableOperation.Replace(entity);
                var result = _table.Execute(op);
                return isValidResult(result.HttpStatusCode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public virtual bool DeleteById(string id)
        {
            try
            {
                var obj = GetById(id);
                TableOperation op = TableOperation.Delete(obj);
                var result = _table.Execute(op);
                return isValidResult(result.HttpStatusCode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public virtual bool DeleteBy(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var lstObj = GetBy(predicate);
                var result = new List<int>();

                foreach (var obj in lstObj)
                {
                    TableOperation op = TableOperation.Delete(obj);
                    var res = _table.Execute(op);
                    result.Add(res.HttpStatusCode);
                }
                return result.Any(e => isValidResult(e));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        private bool isValidResult(int statusCode)
        {
            if ((statusCode >= 300 && statusCode < 500 && statusCode != 408)
                    || statusCode == 501 || statusCode == 505)
            {
                return false;
            }

            return true;
        }

    }
}
