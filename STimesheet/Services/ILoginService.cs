using STimesheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STimesheet.Services
{
    public interface ILoginService
    {

        Task SendEmailAsync(MailRequest mailRequest);
        string Encrypt(string clearText);
        string Decrypt(string cipherText);


    }
}
