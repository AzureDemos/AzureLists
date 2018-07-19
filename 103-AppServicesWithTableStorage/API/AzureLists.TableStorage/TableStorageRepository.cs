using AzureLists.Library;
using AzureLists.TableStorage.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLists.TableStorage
{
    public class TableStorageRepository
    {
        private readonly string connectionString;

        public TableStorageRepository()
        {
            this.connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
        }


        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (Exception ex)
            {
                //log error here
                throw;
            }
            return storageAccount;
        }


        private async Task<CloudTable> CreateTableAsync(string tableName)
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(this.connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            try
            {
                await table.CreateIfNotExistsAsync();
            }
            catch (StorageException)
            {
                //log error here
                throw;
            }
            return table;
        }

        /// <summary>
        /// The partition Id would give us an even distribution across our partitions by using something from the users identity. 
        /// We now need to search lists in a partition for a user, or select a single list by its id. 
        /// Therefore we are make the row key as composite key comprised of the full user id and the list id
        /// </summary>
        private string GenerateRowKey(string userid, string listId)
        {
            return $"{userid}-{listId}";
        }

        #region IListRepository Implementation


        public async Task<ListEntity> Create<T>(UserCredentials userCreds, string listId, T item) where T : IIdentifiable
        {
            return await Update(userCreds, listId, item);
        }

      

        public async Task<ListEntity> Update<T>(UserCredentials userCreds, string listId, T item) where T : IIdentifiable
        {
            CloudTable table = await CreateTableAsync("list");
            if (typeof(T) == typeof(Library.List))
            {
                ListEntity list = new ListEntity(item as List, userCreds.PartionKey, GenerateRowKey(userCreds.UserID, listId));
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(list);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                ListEntity updatedList = result.Result as ListEntity;
                return updatedList;
            }
            else
            {
                throw new NotImplementedException("Table storage only implements updates of lists not task. Task is already part of the list object. ");
            }
        }

        public async System.Threading.Tasks.Task Delete<T>(UserCredentials userCreds, string id) where T : IIdentifiable
        {
            CloudTable table = await CreateTableAsync("list");
            if (typeof(T) == typeof(Library.List))
            {
                ListEntity listEntity = await Get<T>(userCreds, id);
                TableOperation deleteOperation = TableOperation.Delete(listEntity);
                await table.ExecuteAsync(deleteOperation);
            }
            else
            {
                throw new NotImplementedException("Table storage only implements deletions of lists not task. Task is already part of the list object. ");
            }

        }

        public async Task<IEnumerable<ListEntity>> Get<T>(UserCredentials userCreds) where T : IIdentifiable
        {
            // To access a list we need to partition key and row key
            // The partition key in this demo is constant, but in a real work senarion it would be something that can evenly distribute the lists.
            // Our example uses a composite row key "{userid}-{listId}", so we need need to find all rows that start with the userid to get the lists for the logged in user. 
            // Again for this demo the userid is constant as we dont have authentication, but we are still writing the query as if there were more users. 

            CloudTable table = await CreateTableAsync("list");
            var userPatern = GetListsForAUserPatern(userCreds.UserID);
            var userCondition = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, userPatern.Item1),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, userPatern.Item2)
            );

            var filterString = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userCreds.PartionKey),
                TableOperators.And,
                userCondition
            );
            var query = new TableQuery<ListEntity>();
            var results = table.ExecuteQuery(query.Where(filterString));
            return results;

        }



        public async Task<ListEntity> Get<T>(UserCredentials userCreds, string id)
        {
            CloudTable table = await CreateTableAsync("list");
            TableOperation retrieveOperation = TableOperation.Retrieve<ListEntity>(userCreds.PartionKey, GenerateRowKey(userCreds.UserID, id));
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            ListEntity entity = result.Result as ListEntity;
            return entity;
        }

        public Tuple<string, string> GetListsForAUserPatern(string userId)
        {
            var startsWithPattern = $"{userId}-";
            var length = startsWithPattern.Length - 1;
            var lastChar = startsWithPattern[length];
            var nextLastChar = (char)(lastChar + 1);
            var startsWithEndPattern = startsWithPattern.Substring(0, length) + nextLastChar;
            return Tuple.Create(startsWithPattern, startsWithEndPattern);
        }

        #endregion


    }
}
