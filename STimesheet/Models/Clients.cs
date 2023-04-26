using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public class Clients
    {
        [Key]
        public int ID { get; set; }
        public string ClientName { get; set; }
        public string ClientShortName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Web { get; set; }
        public string Notes { get; set; }
        public bool Status { get; set; }
        public int DeptID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
    }

}
