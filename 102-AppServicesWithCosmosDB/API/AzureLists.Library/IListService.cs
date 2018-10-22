using System.Collections.Generic;

namespace AzureLists.Library
{
    public interface IListService
    {
        System.Threading.Tasks.Task<IEnumerable<List>> GetAllLists();
        System.Threading.Tasks.Task<List> GetListById(string listId);
        System.Threading.Tasks.Task<List> CreateOrUpdateList(List item);
        System.Threading.Tasks.Task DeleteList(string listId);

        System.Threading.Tasks.Task<Library.Task> GetTaskById(string taskId, string listId = null);
        System.Threading.Tasks.Task<IEnumerable<Library.Task>> SearchTasks(string listId = null, bool? important = null, bool? completed = null);
        System.Threading.Tasks.Task<Library.Task> AddTaskToList(string listId, Library.Task item);
        System.Threading.Tasks.Task<Library.Task> ReplaceTask(Library.Task item, string listId = null);
        System.Threading.Tasks.Task DeleteTask(string taskId, string listId = null);

    }
}
