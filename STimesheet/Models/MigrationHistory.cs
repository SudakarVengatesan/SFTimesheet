using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    
    public class MigrationHistory
    {
        [Key]
        public string MigrationId { get; set; }
        public string ContextKey { get; set; }
        [Required]
        public byte[] Model { get; set; }
        [Required]

        public string ProductVersion { get; set; }
    }
}
