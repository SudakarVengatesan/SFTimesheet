using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public class TimeSheetLogTable
    {
        [Key]
        public int Id { get; set; }
        public int? EmpTableId { get; set; }
        public int? EmpId { get; set; }
        public string EmpFirstName { get; set; }
        public string Email { get; set; }
        public DateTime? NotUpdatedDate { get; set; }
        public int? MailCounter { get; set; }
        public bool? MailStatus { get; set; }
        public bool? IsTimeSheetEntered { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Status { get; set; }
        public string EmpLastName { get; set; }
        public int? ManagerId { get; set; }
    }

}
