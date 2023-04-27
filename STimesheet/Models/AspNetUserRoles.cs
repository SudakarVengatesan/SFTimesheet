using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public class AspNetUserRoles
    {
        [Key]
        [Required]
        public string UserId { get; set; }
        [Required]
        public string RoleId { get; set; }

    }

}
