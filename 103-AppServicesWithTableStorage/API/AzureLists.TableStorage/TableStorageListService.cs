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
            if (typeof(T) == typeof(List))
            {
                List list = item as List;
                GenerateId(list);
                if (list.Tasks != null) list.Tasks.ForEach(this.GenerateId);
                var listEntity = await this.listRepository.Create(userCreds, item.Id, list);
                item.Id = listEntity.Id;
                return item;
            }
            else
            {
                if (listId == null)
                    throw new Exception("Cannot create task with list ID");
                return default(T);//not done yet
            }

        }

        public async Task<T> Replace<T>(UserCredentials userCreds, string id, T item) where T : class, Library.IIdentifiable, new()
        {
            if (typeof(T) == typeof(List))
            {
                List list = item as List;
                GenerateId(list);
                if (list.Tasks != null) list.Tasks.ForEach(this.GenerateId);
                var le = await this.listRepository.Update<T>(userCreds, id, item);
                return ConvertListEntityToDomainModel(le) as T;
            }
            else
            {
                //Get Task and Parent List
                var listTaskTuple = await GetListAndTaskByTaskId(userCreds, id);
                var list = listTaskTuple.Item1;
                //Update Task on List
                list.Tasks.Remove(listTaskTuple.Item2);
                list.Tasks.Add(item as Library.Task);
                //Update List and return Task
                var le = await this.listRepository.Update<List>(userCreds, list.Id, list);
                var updatedList = ConvertListEntityToDomainModel(le);
                return updatedList.Tasks.FirstOrDefault(x => x.Id == id) as T;
            }
        }


        public async System.Threading.Tasks.Task Delete<T>(UserCredentials userCreds, string id) where T : Library.IIdentifiable
        {
            if (typeof(T) == typeof(List))
            {
                await this.listRepository.Delete<T>(userCreds, id);
            }
            else
            {
                //Get Task and Parent List
                var listTaskTuple = await GetListAndTaskByTaskId(userCreds, id);
                var list = listTaskTuple.Item1;
                //Update Task on List
                list.Tasks.Remove(listTaskTuple.Item2);
                //Update List and return Task
                await this.listRepository.Update<List>(userCreds, list.Id, list);
            }
        }

        /// <summary>
        /// Get ALL only applies to lists
        /// </summary>
        public async System.Threading.Tasks.Task<IEnumerable<T>> Get<T>(UserCredentials userCreds) where T : AzureLists.Library.List, Library.IIdentifiable, new()
        {
            var listsEntities = await this.listRepository.Get<T>(userCreds);
            System.Collections.Generic.List<List> lists = new System.Collections.Generic.List<List>();
            foreach (var le in listsEntities)
                lists.Add(ConvertListEntityToDomainModel(le));
            return lists as IEnumerable<T>;
        }

        /// <summary>
        /// Search Tasks by Importand or Complete
        /// See summary notes for GetListAndTaskByTaskId()
        /// </summary>
        public async System.Threading.Tasks.Task<IEnumerable<T>> Get<T>(UserCredentials userCreds, bool? important = null, bool? completed = null) where T : Library.Task, Library.IIdentifiable, new()
        {
            if (completed == null && important == null) throw new Exception("You must query by at least completed or important");
            var lists = await Get<List>(userCreds);
            List<Library.Task> tasks = new List<Library.Task>();
            foreach (var l in lists)
            {
                List<Library.Task> matches = new List<Library.Task>();
                if (completed != null && completed.Value && important != null && important.Value)
                    matches = l.Tasks.Where(x => x.CompletedDate != null && x.Important).ToList();
                else if (completed != null && completed.Value)
                    matches = l.Tasks.Where(x => x.CompletedDate != null).ToList();
                else
                    matches = l.Tasks.Where(x => x.Important).ToList();

                if (matches.Any())
                    tasks.AddRange(matches);
            }
            return tasks as IEnumerable<T>;
        }

        /// <summary>
        /// Get By ID
        /// </summary>
        public async Task<T> Get<T>(UserCredentials userCreds, string id) where T : class, Library.IIdentifiable, new()
        {
            if (typeof(T) == typeof(List))
            {
                var listEntity = await this.listRepository.Get<T>(userCreds, id);
                return ConvertListEntityToDomainModel(listEntity) as T;
            }
            else
            {
                var listTaskTuple = await GetListAndTaskByTaskId(userCreds, id);
                return listTaskTuple.Item2 as T;
            }
        }


        /// <summary>
        /// We cannot get a task directly without loading the list unless we stored the tasks in a seperate table. 
        /// The API however requires us to search tasks by due date or whether they are important. 
        /// Even if we used a seperate table, this would still be a tricky task for table storage, without the use of additional index tables.
        /// Therefore we have not split tasks into a seperate table. This is not very performant if a user has a lot of lists, so the right choice it really depends on how we think the lists will be used by the users. 
        /// These demos are created to discus the pro and cons of different database in Azure, and therefore these performance considerations are a design choice for a specific use case
        /// </summary>
        private async Task<Tuple<Library.List, Library.Task>> GetListAndTaskByTaskId(UserCredentials userCreds, string id)
        {
            var lists = await Get<List>(userCreds);
            Library.Task task = null;
            Library.List list = null;
            foreach (var l in lists)
            {
                var match = l.Tasks.FirstOrDefault(x => x.Id == id);
                if (match != null)
                {
                    task = match;
                    list = l;
                    break;
                }
            }
            if (task == null) throw new Exception("Task not found");
            return Tuple.Create(list, task);
        }



        private void GenerateId<T>(T item) where T : IIdentifiable
        {
            // this is awful and not intended to be an example
            // of generating ids. The purpose of this wider
            // solution example is to show the use of Azure
            // PaaS services and not a how-to-code generate ids
            if (string.IsNullOrWhiteSpace(item.Id))
                item.Id = Guid.NewGuid().ToString();
        }


        private List ConvertListEntityToDomainModel(ListEntity le)
        {
            if (le == null) return null;

            le.DeSerializeNestedTypes();
            var l = new List() { Id = le.Id, Name = le.Name, Tasks = le.Tasks };
            return l;
        }
    }
}
