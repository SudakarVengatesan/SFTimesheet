using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public class Tasks
    {
        [Key]
        public int ID { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string TaskComments { get; set; }
        public string TaskType { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public string Duration { get; set; }
        public string TaskStatus { get; set; }
        public string TaskCode { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
    }

}
