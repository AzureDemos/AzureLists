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
        public readonly TableStorageRepository listRepository; //shouldn't be public but using it in the console to show functionality

        public TableStorageListService(TableStorageRepository repo)
        {
            this.listRepository = repo;
        }

        #region Lists Actions

        public async Task<List> CreateOrUpdateList(List item)
        {
            List list = item as List;
            GenerateId(list);
            if (list.Tasks != null) list.Tasks.ForEach(this.GenerateId);
            var updatedList = await this.listRepository.CreateOrUpdateList(list);
            return updatedList;
        }

        public async System.Threading.Tasks.Task DeleteList(string listId)
        {
            await this.listRepository.DeleteList(listId);
        }
        public async Task<IEnumerable<List>> GetAllLists()
        {
            var lists = await this.listRepository.GetListsByUser();
            return lists;
        }
        public async Task<List> GetListById(string listId)
        {
            var list = await this.listRepository.GetListById(listId);
            return list;
        }

        #endregion

        #region Tasks Actions

        public async Task<Library.Task> GetTaskById(string taskId, string listId = null)
        {
            if (listId != null)
            {
                var list = await GetListById(listId);
                return list.Tasks.Where(x => x.Id == taskId).FirstOrDefault();
            }
            else
            {
                var listTaskTuple = await GetListAndTaskByTaskId(taskId);
                if (listTaskTuple == null) return null;
                return listTaskTuple.Item2;
            }
        }

        public async Task<Library.Task> AddTaskToList(string listId, Library.Task item)
        {
            var list = await GetListById(listId);
            list.Tasks.Add(item);
            var updatedList = await this.listRepository.CreateOrUpdateList(list);
            return updatedList.Tasks.FirstOrDefault(x => x.Id == item.Id);
        }

        public async Task<Library.Task> ReplaceTask(Library.Task item, string listId = null)
        {
            List list = null;
            Library.Task task = null;
            if (listId != null)
            {
                list = await GetListById(listId);
                task = list.Tasks.Where(x => x.Id == item.Id).FirstOrDefault();
            }
            else
            {
                var listTaskTuple = await GetListAndTaskByTaskId(item.Id);
                if (listTaskTuple == null) return null;
                list = listTaskTuple.Item1;
                task = listTaskTuple.Item2;
            }
            list.Tasks.Remove(task);
            list.Tasks.Add(item);
            //Update List and return Task
            var updatedList = await this.listRepository.CreateOrUpdateList(list);
            return updatedList.Tasks.FirstOrDefault(x => x.Id == item.Id);
        }

      
        public async System.Threading.Tasks.Task DeleteTask(string taskId, string listId = null)
        {
            List list = null;
            Library.Task task = null;
            if (listId != null)
            {
                list = await GetListById(listId);
                task = list.Tasks.Where(x => x.Id == taskId).FirstOrDefault();
            }
            else
            {
                var listTaskTuple = await GetListAndTaskByTaskId(taskId);
                if (listTaskTuple != null)
                {
                    list = listTaskTuple.Item1;
                    task = listTaskTuple.Item2;
                }
            }
            if (task != null)
            {
                list.Tasks.Remove(task);
                await this.listRepository.CreateOrUpdateList(list);
            }
        }


        /// <summary>
        /// Search Tasks by Importand or Complete
        /// See summary notes for GetListAndTaskByTaskId()
        /// </summary>
        public async Task<IEnumerable<Library.Task>> SearchTasks(string listId = null, bool? important = null, bool? completed = null)
        {
            if (completed == null && important == null) throw new Exception("You must query by at least completed or important");
            List<Library.Task> tasks = new List<Library.Task>();
            if (listId != null)
            {
                var list = await GetListById(listId);
                var matches = SearchTasksWithinList(list, important, completed);
                if (matches.Any())
                    tasks.AddRange(matches);
            }
            else
            {
                var lists = await GetAllLists();
                foreach (var l in lists)
                {
                    var matches = SearchTasksWithinList(l, important, completed);
                    if (matches.Any())
                        tasks.AddRange(matches);
                }
            }
            return tasks;
        }

        private List<Library.Task> SearchTasksWithinList(List list, bool? important = null, bool? completed = null)
        {
            if (completed != null && completed.Value && important != null && important.Value)
                return list.Tasks.Where(x => x.CompletedDate != null && x.Important).ToList();
            else if (completed != null && completed.Value)
                return list.Tasks.Where(x => x.CompletedDate != null).ToList();
            else
                return list.Tasks.Where(x => x.Important).ToList();
        }


        #endregion

        #region Internal Helpers

        /// <summary>
        /// We cannot get a task directly without loading the list unless we stored the tasks in a seperate table. 
        /// The API however requires us to search tasks by due date or whether they are important. 
        /// Even if we used a seperate table, this would still be a tricky task for table storage, without the use of additional index tables.
        /// Therefore we have not split tasks into a seperate table. This is not very performant if a user has a lot of lists, so the right choice it really depends on how we think the lists will be used by the users. 
        /// These demos are created to discus the pro and cons of different database in Azure, and therefore these performance considerations are a design choice for a specific use case
        /// </summary>
        private async Task<Tuple<Library.List, Library.Task>> GetListAndTaskByTaskId(string id)
        {
            var lists = await GetAllLists();
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
            if (task == null) return null;
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


      

        #endregion
    }
}
