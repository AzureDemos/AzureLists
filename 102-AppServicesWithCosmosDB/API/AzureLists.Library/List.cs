using System.Collections.Generic;

namespace AzureLists.Library
{
    public class List : IIdentifiable
    {
        public List() { }
        public List(string name)
        {
            Name = name;
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public List<Task> Tasks { get; set; }
    }
}
