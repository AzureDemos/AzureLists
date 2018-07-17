using AzureLists.Library;
using AzureLists.TableStorage.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLists.TableStorage
{
    public class TableStorageListService
    {
        private readonly TableStorageRepository listRepository;

        public TableStorageListService()
        {
            this.listRepository = new TableStorageRepository();
        }

        public async System.Threading.Tasks.Task<T> Create<T>(UserCredentials userCreds, T item, string listId = null) where T : Library.IIdentifiable
        {
            this.GenerateId(item);
            if (typeof(T) == typeof(List))
            {
                List list = item as List;
                if (list.Tasks != null) list.Tasks.ForEach(this.GenerateId);
                var listEntity = await this.listRepository.Create(userCreds, item.Id, list);
                item.Id = listEntity.Id;
                return item;
            }
            else
            {
                if (listId == null)
                    throw new Exception("Cannot update task with list ID");
                return default(T);//not done yet
            }

        }

        public async System.Threading.Tasks.Task<IEnumerable<T>> Get<T>(UserCredentials userCreds) where T : class, Library.IIdentifiable, new()
        {
            var listsEntities = await this.listRepository.Get<T>(userCreds);
            System.Collections.Generic.List<List> lists = new System.Collections.Generic.List<List>();
            foreach (var le in listsEntities)
                lists.Add(ConvertListEntityToDomainModel(le));
            return lists as IEnumerable<T>;
        }

        //public async System.Threading.Tasks.Task<IEnumerable<T>> Get<T>(bool? important = null, bool? completed = null) where T : class, Library.IIdentifiable, new()
        //{
        //   // return await this.listRepository.Get<T>(important: important, completed: completed);
        //}

        //public async System.Threading.Tasks.Task<IEnumerable<T>> Get<T>(string id, bool? important = null, bool? completed = null) where T : class, Library.IIdentifiable, new()
        //{
        //  //  return await this.listRepository.Get<T>(id, important, completed);
        //}

        public async System.Threading.Tasks.Task Delete<T>(UserCredentials userCreds, string id) where T : Library.IIdentifiable
        {
            await this.listRepository.Delete<T>(userCreds, id);
        }

        //public async System.Threading.Tasks.Task<T> Replace<T>(string id, T item) where T : Library.IIdentifiable
        //{
        ////    await this.listRepository.Update<T>(id, item);
        // //   return item;
        //}


        private void GenerateId<T>(T item) where T : IIdentifiable
        {
            // this is awful and not intended to be an example
            // of generating ids. The purpose of this wider
            // solution example is to show the use of Azure
            // PaaS services and not a how-to-code generate ids
            item.Id = Guid.NewGuid().ToString();
        }


        private List ConvertListEntityToDomainModel(ListEntity le)
        {
            le.DeSerializeNestedTypes();
            var l = new List() { Id = le.Id, Name = le.Name, Tasks = le.Tasks };
            return l;
        }
    }
}
