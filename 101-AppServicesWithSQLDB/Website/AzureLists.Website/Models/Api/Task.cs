using AzureLists.Website.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzureLists.Website.Models.Api
{
    public class Task
    {
        public string ListId { get; set; }
        public string Id { get; set; }
        public string Title { get; set; } = "";
        public string Notes { get; set; } = "";
        public bool Important { get; set; }
        public DateTime? CompletedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }

        public bool Completed { get { return CompletedDate != null && CompletedDate.HasValue; } }

        public string DueDatePretty { get { return DueDate != null ? DueDate.Value.TimeSinceOrUntil() : ""; } }

        public string CompletedPretty { get { return CompletedDate != null ? CompletedDate.Value.TimeSinceOrUntil() : ""; } }

    }
}

