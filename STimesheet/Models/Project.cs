using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    using System;

    public class Project
    {
        [Key]
        public int ID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public decimal EstimationHrs { get; set; }
        public string SoNumber { get; set; }
        public string ProjectManager { get; set; }
        public string TeamLead { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Billable { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public int ClientID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
    }

}
