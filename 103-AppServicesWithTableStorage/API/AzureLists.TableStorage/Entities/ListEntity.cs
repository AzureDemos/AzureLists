using AzureLists.Library;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AzureLists.TableStorage.Entities
{
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
