using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using STimesheet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace STimesheet.Services
{
    public class LoginService : ILoginService
    {
        private readonly MailSettings _mailSettings;

        private readonly IConfiguration _configuration;
        public LoginService(IOptions<MailSettings> mailSettings,IConfiguration configuration)
        {
            _mailSettings = mailSettings.Value;
            this._configuration = configuration;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            /*email.Subject = mailRequest.Subject;*/
            email.Subject = "Reset Password Link..";
            var builder = new BodyBuilder();
            var Body = "please click link";
            /* builder.HtmlBody = mailRequest.Body;*/
            builder.HtmlBody = Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
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

        private static string Base64UrlEncode(string text)
        {
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
            return base64.Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private static string Base64UrlDecode(string text)
        {
            var base64 = text.Replace("-", "+").Replace("_", "/");
            switch (base64.Length % 4)
            {
                case 0: break;
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                default: throw new ArgumentException("Invalid base64 url string.");
            }
            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Accounts.Update(account);
            _context.SaveChanges();

            // send email
            sendPasswordResetEmail(account, origin);
        }
        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
