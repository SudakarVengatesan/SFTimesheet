using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STimesheet.Models;
using STimesheet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STimesheet.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            this._mailService = mailService;
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendMail([FromBody] MailRequest request)
        {
            try
            {
                await _mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }

        }
          
    }
}
