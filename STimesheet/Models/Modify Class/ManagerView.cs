using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STimesheet.Models.Modify_Class
{
    public class ManagerView
    {
        public int ID { get; set; }
        public int EmpID { get; set; }
        public string EmpName { get; set; }
        public string Clientname { get; set; }
        public string ProjectName { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public decimal Hours { get; set; }
        public string Status { get; set; }

    }
}
