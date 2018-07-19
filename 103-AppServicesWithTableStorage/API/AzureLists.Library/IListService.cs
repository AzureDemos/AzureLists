using System.Collections.Generic;
using System.Threading.Tasks;
using Tasks = System.Threading.Tasks;

namespace AzureLists.Library
{
    public interface IListService
    {
        Task<IEnumerable<List>> GetAllLists();
        Task<List> GetListById(string listId);
        Task<List> CreateOrUpdateList(List item);
        System.Threading.Tasks.Task DeleteList(string listId);

        Task<Library.Task> GetTaskById(string taskId, string listId = null);
        Task<IEnumerable<Library.Task>> SearchTasks(string listId = null, bool? important = null, bool? completed = null);
        Task<Library.Task> AddTaskToList(string listId, Library.Task item);
        Task<Library.Task> ReplaceTask(Library.Task item, string listId = null);
        Tasks.Task DeleteTask(string taskId, string listId = null);

    }
}
