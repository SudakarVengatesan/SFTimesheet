using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public class Employee
    {
        [Key]
        public int ID { get; set; }
        public int EmpId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public int Gender { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int PinCode { get; set; }
        public string WorkPhoneNo { get; set; }
        public string MobilePhoneNo { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public int ManagerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public bool? Deleted { get; set; }
    }

}
