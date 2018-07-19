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
        public readonly UserCredentials AuthenticatedUser; //shouldn't be public but accessing it in the console to show functionality

        public TableStorageRepository(UserCredentials authenticatedUser)
        {
            this.connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            this.AuthenticatedUser = authenticatedUser;
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



        public async Task<List> CreateOrUpdateList(List item) 
        {
            CloudTable table = await CreateTableAsync("list");
            ListEntity list = new ListEntity(item as List, this.AuthenticatedUser.PartionKey, GenerateRowKey(this.AuthenticatedUser.UserID, item.Id));
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(list);
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            ListEntity updatedList = result.Result as ListEntity;
            return ConvertListEntityToDomainModel(updatedList);
        }

        public async System.Threading.Tasks.Task DeleteList(string listId)
        {
            CloudTable table = await CreateTableAsync("list");
            ListEntity listEntity = await GetListEntityById(listId);
            TableOperation deleteOperation = TableOperation.Delete(listEntity);
            await table.ExecuteAsync(deleteOperation);
        }

        public async Task<List> GetListById(string listId)
        {
            ListEntity listEntity = await GetListEntityById(listId);
            return ConvertListEntityToDomainModel(listEntity);
        }

        private async Task<ListEntity> GetListEntityById(string listId)
        {
            CloudTable table = await CreateTableAsync("list");
            TableOperation retrieveOperation = TableOperation.Retrieve<ListEntity>(this.AuthenticatedUser.PartionKey, GenerateRowKey(this.AuthenticatedUser.UserID, listId));
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            ListEntity entity = result.Result as ListEntity;
            return entity;
        }

        public async Task<IEnumerable<List>> GetListsByUser()
        {
            var listEntities = await GetListEntitesByUser();
            List<List> lists = new List<List>();
            foreach (var le in listEntities)
                lists.Add(ConvertListEntityToDomainModel(le));
            return lists;
        }

        private async Task<IEnumerable<ListEntity>> GetListEntitesByUser() 
        {
            // To access a list we need to partition key and row key
            // The partition key in this demo is constant, but in a real work senarion it would be something that can evenly distribute the lists.
            // Our example uses a composite row key "{userid}-{listId}", so we need need to find all rows that start with the userid to get the lists for the logged in user. 
            // Again for this demo the userid is constant as we dont have authentication, but we are still writing the query as if there were more users. 

            CloudTable table = await CreateTableAsync("list");
            var userPatern = GetListsForAUserPatern(this.AuthenticatedUser.UserID);
            var userCondition = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, userPatern.Item1),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, userPatern.Item2)
            );

            var filterString = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, this.AuthenticatedUser.PartionKey),
                TableOperators.And,
                userCondition
            );
            var query = new TableQuery<ListEntity>();
            var results = table.ExecuteQuery(query.Where(filterString));
            return results;
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

        private List ConvertListEntityToDomainModel(ListEntity le)
        {
            if (le == null) return null;

            le.DeSerializeNestedTypes();
            var l = new List() { Id = le.Id, Name = le.Name, Tasks = le.Tasks };
            return l;
        }

        #endregion


    }
}
