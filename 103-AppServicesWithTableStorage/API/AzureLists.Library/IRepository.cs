using System.Collections.Generic;
using Tasks = System.Threading.Tasks;

namespace AzureLists.Library
{
    public interface IListRepository
    {
        Tasks.Task<IEnumerable<T>> Get<T>() where T : class, IIdentifiable, new();
        
        Tasks.Task<IEnumerable<T>> Get<T>(string id = null, bool? important = null, bool? completed = null) where T : class, IIdentifiable, new();

        Tasks.Task Create<T>(string listId, T item) where T : IIdentifiable;
        
        Tasks.Task Delete<T>(string id) where T : IIdentifiable;

        Tasks.Task Update<T>(string id, T item) where T : IIdentifiable;
    }
}
