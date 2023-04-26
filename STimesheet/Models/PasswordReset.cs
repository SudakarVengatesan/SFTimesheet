using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public class PasswordReset
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ResetLink { get; set; }
    }

}
