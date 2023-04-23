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
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _mailService;
        public LoginController(ILoginService mailService)
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

        [HttpPost]
        [Route("Encrypt")]
        public string Encry(string text)
        {
            var s=_mailService.Encrypt(text);
            return s;
        }

        [HttpPost]
        [Route("Decrypt")]
        public string Decry(string text)
        {
            var s = _mailService.Decrypt(text);
            return s;
        }

        public IActionResult ForgotPassword(ForgotPasswordRequest model)
        {
            _accountService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }
    }
}
