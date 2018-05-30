using System;
using System.Collections.Generic;
using Tasks = System.Threading.Tasks;

namespace AzureLists.Library
{
    public class ListService
    {
        private readonly IListRepository listRepository;

        public ListService(IListRepository listRepository)
        {
            this.listRepository = listRepository;
        }

        public async Tasks.Task<T> Create<T>(T item, string listId = null) where T : Library.IIdentifiable
        {
            this.GenerateId(item);
            if(typeof(T) == typeof(Library.List))
            {
                Library.List list = item as Library.List;
                if (list.Tasks != null) list.Tasks.ForEach(this.GenerateId);
                await this.listRepository.Create<Library.List>(item.Id, list);
                return item;
            }
            await this.listRepository.Create<T>(listId, item);
            return item;
        }

        public async Tasks.Task<IEnumerable<T>> Get<T>() where T : class, Library.IIdentifiable, new()
        {
            return await this.listRepository.Get<T>();
        }

        public async Tasks.Task<IEnumerable<T>> Get<T>(bool? important = null, bool? completed = null) where T : class, Library.IIdentifiable, new()
        {
            return await this.listRepository.Get<T>(important: important, completed: completed);
        }

        public async Tasks.Task<IEnumerable<T>> Get<T>(string id, bool? important = null, bool? completed = null) where T : class, Library.IIdentifiable, new()
        {
            return await this.listRepository.Get<T>(id, important, completed);
        }

        public async Tasks.Task Delete<T>(string id) where T : Library.IIdentifiable
        {
            await this.listRepository.Delete<T>(id);
        }

        public async Tasks.Task<T> Replace<T>(string id, T item) where T : Library.IIdentifiable
        {
            await this.listRepository.Update<T>(id, item);
            return item;
        }
        
        private void GenerateId<T>(T item) where T : IIdentifiable
        {
            // this is awful and not intended to be an example
            // of generating ids. The purpose of this wider
            // solution example is to show the use of Azure
            // PaaS services and not a how-to-code generated ids
            item.Id = Guid.NewGuid().ToString();
        }
    }
}
