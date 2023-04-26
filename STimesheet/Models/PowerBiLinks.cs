using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    [Table("PowerBi_links", Schema = "dbo")]
    public partial class PowerBiLinks
    {
        [Column("EmpID")]
        public int? EmpId { get; set; }
        [Column("EmailID")]
        public string EmailId { get; set; }
        [Column("PowerBi_Link")]
        public string PowerBiLink { get; set; }
    }
}
