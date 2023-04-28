using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace STimesheet.Models.Modify_Class
{
    public class AddWorks
    {
        [Key]
        public int ID { get; set; }
        public int EmpID { get; set; }
        public string ProjectName{ get; set; }
        public string TaskName { get; set; }
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public string Notes { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public string Starttime { get; set; }
        public string Endtime { get; set; }

    }
}
