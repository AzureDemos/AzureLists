using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureLists.Website.Models.Api
{
    public class Task
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public bool Important { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool Completed { get; set; }
        public DateTime? DueDate { get; set; }

    }
}

