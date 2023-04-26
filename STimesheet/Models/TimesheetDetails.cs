using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public class TimesheetDetails
    {
        [Key]
        public int ID { get; set; }
        public int EmpID { get; set; }
        public int ProjectID { get; set; }
        public int TaskID { get; set; }
        public int ClientID { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public string Notes { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime ApprovedDate { get; set; }
        public int ApprovedBy { get; set; }
        public string ReviewComments { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public string Starttime { get; set; }
        public string Endtime { get; set; }
    }

}
