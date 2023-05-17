
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using STimesheet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using STimesheet.DBcontext;
using System.Net.Mail;
using System.Net;

namespace STimesheet.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly DataContext db = new DataContext();
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        [HttpGet]
        public string dummy()
        {
            var s = "welcome";
            return s;

        }
        [HttpPost]
        [Route("signin")]
        public async Task<ActionResult> Signin(string Username, string Password)
        {
            try
            {
                string hash = Encrypt(Password);
                List<AspNetUsers> Aspnetusers = await db.AspNetUsers.ToListAsync();
                List<AspNetRoles> Aspnetroles = await db.AspNetRoles.ToListAsync();
                List<AspNetUserRoles> Aspnetuserrole = await db.AspNetUserRoles.ToListAsync();
                AspNetUsers user = Aspnetusers.FirstOrDefault(x => x.UserName == Username||x.Email==Username && x.PasswordHash == hash);
                var userRoles = from role in db.AspNetRoles
                                join userRole in db.AspNetUserRoles
                                on role.Id equals userRole.RoleId
                                where userRole.UserId == user.Id
                                select role.Name;
                var result = new
                {
                    Roles = userRoles,
                    Empid = user.EmpId

                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("Forgetpassword")]
        public async Task<IActionResult> forgetpassword(string Emailid)
        {
            try
            {
                if (Emailid == null)
                {
                    return BadRequest("Must supply an email");
                }
                List<AspNetUsers> Users = await db.AspNetUsers.Where(user => user.Email == Emailid).ToListAsync();
                string result = GenerateRandomPassword(10);
                string updatepaassword = Encrypt(result);
                AspNetUsers forgetuser = Users.First();
                forgetuser.PasswordHash = updatepaassword;
                await db.SaveChangesAsync();
                var email = SendEmail(Emailid, result,forgetuser.FirstName);
                return Ok(Response.StatusCode);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("ChangePassword")]

        public async Task ChangePassword(int Empid,string NewPassword)
        {
            try
            {
                string Username = db.AspNetUsers.Where(x => x.EmpId == Empid).Select(x => x.PasswordHash).FirstOrDefault();
                string Hashnewpassword = Encrypt(NewPassword);
                if (Username != null && Username.Any())
                {
                    Username = NewPassword;
                    await db.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Route("Encrypt")]
        public string Encrypt(string clearText)
        {
            string EncryptionKey = _configuration.GetSection("Key")["EncryptionKey"];
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            Aes encryptor = Aes.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.FlushFinalBlock();
            clearText = Convert.ToBase64String(ms.ToArray());
            return Base64UrlEncode(clearText);
        }
        private static string Base64UrlEncode(string text)
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
            return base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private static string Base64UrlDecode(string text)
        {
            string base64 = text.Replace("-", "+").Replace("_", "/");
            switch (base64.Length % 4)
            {
                case 0: break;
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                default: throw new ArgumentException("Invalid base64 url string.");
            }
            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
        [HttpPost]
        [Route("decode")]
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = _configuration.GetSection("Key")["EncryptionKey"];
            cipherText = Base64UrlDecode(cipherText);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            Aes encryptor = Aes.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.FlushFinalBlock();
            cipherText = Encoding.Unicode.GetString(ms.ToArray());
            return cipherText;
        }

        //Function for generate automatically password to user
        public static string GenerateRandomPassword(int length)
        {
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            byte[] randomBytes = new byte[length];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            StringBuilder result = new StringBuilder(length);
            foreach (var b in randomBytes)
            {
                result.Append(allowedChars[b % allowedChars.Length]);
            }
            return result.ToString();
        }

        public async Task<IActionResult> SendEmail(string request, string newpassword,string Username)
        {
            try
            {
                string smtpHost = _configuration.GetSection("MailSettings")["Host"];
                int smtpPort = int.Parse(_configuration.GetSection("MailSettings")["Port"]);
                string smtpUsername = _configuration.GetSection("MailSettings")["Mail"];
                string smtpPassword = _configuration.GetSection("MailSettings")["Password"];
                // Create a new MailMessage object
                MailMessage mail = new MailMessage();
                mail.To.Add(new MailAddress(request));
                mail.From = new MailAddress(_configuration.GetSection("MailSettings")["Mail"]);
                mail.Subject = "Reset Password..";
                string path = "Template/ResetPass.html";
                string pathtem = System.IO.File.ReadAllText(path);
                pathtem.Replace("Username", Username);
                pathtem.Replace("newpassword", newpassword);
                mail.Body = pathtem;
                mail.IsBodyHtml = true;
                // Create a new SmtpClient object
                SmtpClient smtp = new SmtpClient();
                smtp.Host = smtpHost; 
                smtp.Port = smtpPort;
                smtp.EnableSsl = bool.Parse(_configuration.GetSection("MailSettings")["enableSsl"]);
                // Set the credentials if necessary
                smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                // Send the email
                await smtp.SendMailAsync(mail);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
