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

        #region Lists Actions

        public async Task<List> CreateOrUpdateList(UserCredentials userCreds, List item)
        {
            List list = item as List;
            GenerateId(list);
            if (list.Tasks != null) list.Tasks.ForEach(this.GenerateId);
            var updatedList = await this.listRepository.CreateOrUpdateList(userCreds, list);
            return updatedList;
        }

        public async System.Threading.Tasks.Task DeleteList(UserCredentials userCreds, string listId)
        {
            await this.listRepository.DeleteList(userCreds, listId);
        }
        public async Task<IEnumerable<List>> GetAllLists(UserCredentials userCreds)
        {
            var lists = await this.listRepository.GetListsByUser(userCreds);
            return lists;
        }
        public async Task<List> GetListById(UserCredentials userCreds, string listId)
        {
            var list = await this.listRepository.GetListById(userCreds, listId);
            return list;
        }

        #endregion

        #region Tasks Actions

        public async Task<Library.Task> GetTaskById(UserCredentials userCreds, string taskId)
        {
            var listTaskTuple = await GetListAndTaskByTaskId(userCreds, taskId);
            return listTaskTuple.Item2;
        }

        public async Task<Library.Task> ReplaceTask(UserCredentials userCreds, Library.Task item)
        {
            //Get Task and Parent List
            var listTaskTuple = await GetListAndTaskByTaskId(userCreds, item.Id);
            var list = listTaskTuple.Item1;
            //Update Task on List
            list.Tasks.Remove(listTaskTuple.Item2);
            list.Tasks.Add(item as Library.Task);
            //Update List and return Task
            var updatedList = await this.listRepository.CreateOrUpdateList(userCreds, list);
            return updatedList.Tasks.FirstOrDefault(x => x.Id == item.Id);
        }

        public async System.Threading.Tasks.Task DeleteTask(UserCredentials userCreds, string taskId)
        {
            //Get Task and Parent List
            var listTaskTuple = await GetListAndTaskByTaskId(userCreds, taskId);
            var list = listTaskTuple.Item1;
            //Update Task on List
            list.Tasks.Remove(listTaskTuple.Item2);
            //Update List and return Task
            await this.listRepository.CreateOrUpdateList(userCreds, list);
        }

        /// <summary>
        /// Search Tasks by Importand or Complete
        /// See summary notes for GetListAndTaskByTaskId()
        /// </summary>
        public async Task<IEnumerable<Library.Task>> SearchTasks(UserCredentials userCreds, bool? important = null, bool? completed = null)
        {
            if (completed == null && important == null) throw new Exception("You must query by at least completed or important");
            var lists = await GetAllLists(userCreds);
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
            return tasks;
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
        private async Task<Tuple<Library.List, Library.Task>> GetListAndTaskByTaskId(UserCredentials userCreds, string id)
        {
            var lists = await GetAllLists(userCreds);
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


      

        #endregion
    }
}
