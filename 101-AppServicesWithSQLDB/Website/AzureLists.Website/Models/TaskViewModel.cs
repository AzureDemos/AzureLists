using AzureLists.Website.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureLists.Website.Models
{
    public class TaskViewModel 
    {
        public Task Task { get; set; }
        public string ListId { get; set; }
      
    }

 
}