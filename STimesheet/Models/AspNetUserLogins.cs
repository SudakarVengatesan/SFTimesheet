using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STimesheet.Models
{
    public  class AspNetUserLogins
    {
       [Key]
       public string LoginProvider { get; set; }
       public string ProviderKey { get; set; }
       public string UserId { get; set; }
    }

}
