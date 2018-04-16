using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureLists.Website.Models
{
    public class ListsViewModel
    {
        public List<Api.List> Lists { get; set; }
        public Api.List SelectedList { get; set; }

        public TaskViewModel SelectedTask { get; set; } = new TaskViewModel();
    }

 
}