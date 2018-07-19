using AzureLists.Library;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AzureLists.TableStorage.Entities
{
    /// <summary>
    /// Table storage does not support nested types. Therefore we have two options:
    /// 1. Serialize the nested type - each row can be 1MB in size, so we could store thousands of tasks without reaching the limit. (We cant however query the tasks directly)
    /// 2. Use a seperate table for the tasks. This would require an additional query to retrieve each list with its tasks, but would allow us to query tasks. 
    /// Table storage query is relatively limited as it relies on the row key and partition key not the data. Therefore we couldn't query thing like importancy or due dates without changing the row key (which is very bad) or creating additional index tables
    /// Both approaches have positives and negatives, but as we cannot search tasks anyway without additional index tables, we chosing to option 1
    /// </summary>
    public class ListEntity : TableEntity, IIdentifiable
    {
        public ListEntity(List list, string partition, string row) : this(partition, row)
        {
            Id = list.Id;
            Name = list.Name;
            Tasks = list.Tasks;
            SerializeNestedTypes();
        }

        public ListEntity(string partition, string row)
        {
            this.PartitionKey = partition;
            this.RowKey = row;
        }


        public ListEntity() { }

        public string Id { get; set; }
        public string Name { get; set; }

        public string TasksString { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();

        public void SerializeNestedTypes()
        {
            if (Tasks != null && Tasks.Any())
                TasksString = JsonConvert.SerializeObject(Tasks);
        }
        public void DeSerializeNestedTypes()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TasksString))
                    Tasks = JsonConvert.DeserializeObject<List<Task>>(TasksString);
            }
            catch { }
            
        }
    }
}
