namespace AzureLists.CosmosDB
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    public class DocumentDBRepository<T> : IRepository<T> where T : class
    {
        private readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
        private readonly DocumentClient client;

        public DocumentDBRepository()
        {
            this.client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }


        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(this.DatabaseId, this.CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = this.client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(this.DatabaseId, this.CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<T> CreateItemAsync(T item)
        {
            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(this.DatabaseId, this.CollectionId), item);
            return item;
        }

        public async Task UpdateItemAsync(string id, T item)
        {
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(this.DatabaseId, this.CollectionId, id), item);            
        }

        public async Task DeleteItemAsync(string id)
        {
            await this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(this.DatabaseId, this.CollectionId, id));
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await this.client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(this.DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await this.client.CreateDatabaseAsync(new Database { Id = this.DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await this.client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(this.DatabaseId, this.CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(this.DatabaseId),
                        new DocumentCollection { Id = this.CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}