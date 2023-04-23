using STimesheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STimesheet.Services
{
    public interface IMailService
    {

        Task SendEmailAsync(MailRequest mailRequest);
        
    }
}
