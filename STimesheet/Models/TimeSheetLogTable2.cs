using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public class TimeSheetLogTable2
    {
        [Key]
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string NotUpYearMonth { get; set; }
        public int NotUpWeek { get; set; }
        public decimal TotalHours { get; set; }
        public string EmpEmail { get; set; }
        public string MgrEmail { get; set; }
        public string TimeSheetStatus { get; set; }
        public int MailCounter { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
        public string Status { get; set; }
        public int NotUpMonth { get; set; }
        public string ManagerName { get; set; }
        public int MGRMailCounter { get; set; }
        public int MGREmpId { get; set; }
    }

}
