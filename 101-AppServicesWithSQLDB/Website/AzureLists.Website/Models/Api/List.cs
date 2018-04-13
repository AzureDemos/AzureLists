using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureLists.Website.Models.Api
{
    public class List
    {
        public List()
        {
            Tasks = new List<Task>();
        }
        public string Id { get; set; }
        public string Name { get; set; }

        public List<Task> Tasks { get; set; } = new List<Task>();

       
    }
}