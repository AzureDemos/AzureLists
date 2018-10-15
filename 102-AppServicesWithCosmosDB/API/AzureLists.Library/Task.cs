using System;

namespace AzureLists.Library
{
    public class Task : IIdentifiable
    {
        public Task()
        {

        }
       
        public string Id { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public bool Important { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
