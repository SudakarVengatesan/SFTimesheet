using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    
    public partial class PowerBiLinks
    {
       [Key]
        public int? EmpId { get; set; }
        public string EmailId { get; set; }
        public string PowerBiLink { get; set; }
    }
}
