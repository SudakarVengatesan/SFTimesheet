using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STimesheet.DBcontext;
using STimesheet.Models;
using STimesheet.Models.Modify_Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace STimesheet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly DataContext db = new DataContext();
        private readonly IConfiguration _configuration;
        public ManagerController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        [HttpGet]
        [Route("AllReport")]
        public async Task<ActionResult<List<TimesheetDetails>>> AllDataReport(int managerID)
        {
            try
            {
                List<int> empdata = await db.Employee.Where(x => x.ManagerID == managerID).Select(x => x.ID).ToListAsync();
                var query = from tsd in db.TimesheetDetails where empdata.Contains(tsd.EmpID) && !tsd.Deleted && tsd.Submit
                            join emp in db.Employee on tsd.EmpID equals emp.ID
                            join proj in db.Project on tsd.ProjectID equals proj.ID
                            join tak in db.Tasks on tsd.TaskID equals tak.ID
                            join cli in db.Clients on tsd.ClientID equals cli.ID
                            select new
                            {tsd.ID,tsd.EmpID,emp.FirstName,proj.ProjectName,tak.TaskName,cli.ClientName,tsd.Date,tsd.Hours,
                             tsd.Notes,tsd.ApprovalStatus,tsd.Starttime,tsd.Endtime
                            };
                var results = await query.ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error showing records: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult<List<TimesheetDetails>>> AllDataReport(int managerID,string Searchname)
        {
            try
            {
                if(Searchname== null)
                {
                    List<int> empdata = await db.Employee.Where(x => x.ManagerID == managerID).Select(x => x.ID).ToListAsync();
                    var query = from tsd in db.TimesheetDetails where empdata.Contains(tsd.EmpID) && !tsd.Deleted && tsd.Submit
                                join emp in db.Employee on tsd.EmpID equals emp.ID
                                join proj in db.Project on tsd.ProjectID equals proj.ID
                                join tak in db.Tasks on tsd.TaskID equals tak.ID
                                join cli in db.Clients on tsd.ClientID equals cli.ID
                                where (Searchname == null || proj.ProjectName.ToLower().Contains(Searchname.ToLower()))
                                select new
                                {tsd.ID,tsd.EmpID,emp.FirstName, proj.ProjectName,tak.TaskName,cli.ClientName,tsd.Date,tsd.Hours,tsd.Notes,
                                tsd.ApprovalStatus,tsd.CreatedDate,tsd.CreatedBy,tsd.UpdatedDate,tsd.UpdatedBy,tsd.Starttime,tsd.Endtime};
                    var results = query.ToList();
                    return Ok(results);
                }
                List<int> Empdatas = await db.Employee.Where(x => x.ManagerID == managerID).Select(x => x.ID).ToListAsync();
                var query1 = from tsd in db.TimesheetDetails where Empdatas.Contains(tsd.EmpID) && !tsd.Deleted && tsd.Submit
                            join emp in db.Employee on tsd.EmpID equals emp.ID
                            join proj in db.Project on tsd.ProjectID equals proj.ID
                            join tak in db.Tasks on tsd.TaskID equals tak.ID
                            join cli in db.Clients on tsd.ClientID equals cli.ID
                            where (Searchname == proj.ProjectName || proj.ProjectName.ToLower().Contains(Searchname.ToLower()))
                            select new{tsd.ID, tsd.EmpID,emp.FirstName,proj.ProjectName,tak.TaskName,cli.ClientName,tsd.Date,tsd.Hours,tsd.Notes,tsd.ApprovalStatus,
                                tsd.CreatedDate,tsd.CreatedBy,tsd.UpdatedDate,tsd.UpdatedBy,tsd.Starttime,tsd.Endtime,proj.Billable};
               
                var final = query1.ToList();
                if (!final.Any())
                {
                    throw new Exception();
                }

                return Ok(final);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Show record: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("Approve")]
        public async Task<ActionResult> ApproveReport([FromBody]Approve approve)
        {
            try
            {
                List<TimesheetDetails> recordsToUpdate = await db.TimesheetDetails.Where(tsd => tsd.ID == approve.ID && tsd.Submit==true && tsd.Deleted==false).ToListAsync();
                if (recordsToUpdate != null && recordsToUpdate.Any())
                {
                    recordsToUpdate.ForEach(r => r.ApprovalStatus = "Approved");
                    recordsToUpdate.ForEach(r => r.ApprovedBy = approve.EmpID);
                    recordsToUpdate.ForEach(r => r.ApprovedDate = approve.ApprovedDate);
                    await db.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                return Ok(Response.StatusCode);
            }
            catch(Exception ex)
            {
                return BadRequest($"Error Approve status:{ex.Message}");
            }
        }
        [HttpPut]
        [Route("Reject")]
        public async Task<ActionResult> RejectReport([FromBody] Approve approve)
        {
            try
            {
                List<TimesheetDetails> recordsToUpdate = await db.TimesheetDetails.Where(tsd => tsd.ID == approve.ID && tsd.Submit == true && tsd.Deleted == false).ToListAsync();
                List<AspNetUsers> Emprec = await db.AspNetUsers.Where(x => recordsToUpdate.Select(r => r.EmpID).Contains(x.EmpId)).ToListAsync();

                if (recordsToUpdate != null && recordsToUpdate.Any())
                {
                    recordsToUpdate.ForEach(r => r.ApprovalStatus = "Rejected");
                    recordsToUpdate.ForEach(r => r.ApprovedBy = approve.EmpID);
                    recordsToUpdate.ForEach(r => r.Submit = false);
                    await db.SaveChangesAsync();
                    var response = SendEmail(Emprec.Select(x=>x.Email).FirstOrDefault(),Emprec.Select(x=>x.FirstName).FirstOrDefault(),approve.ID);

                }
                else
                {
                    return NotFound();
                }
                return Ok(Response.StatusCode);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Reejected status:{ex.Message}");
            }
        }

        [HttpPost]
        [Route("Empdrop")]
        public async Task<ActionResult> SelectedDrop([FromBody] SelectDropRequest request)
        {
          
            try
            {
                List<string> names = request.names;
                List<string> status = request.status;
              
                if (names.Any()&& status.Any())
                {
                    List<int> empdata = await db.Employee.Where(x => names.Contains(x.FirstName)).Select(x => x.ID).ToListAsync();
                    var result = from tsd in db.TimesheetDetails where (empdata.Contains(tsd.EmpID) && status.Contains(tsd.ApprovalStatus)) && !tsd.Deleted && tsd.Submit
                                join emp in db.Employee on tsd.EmpID equals emp.ID
                                join proj in db.Project on tsd.ProjectID equals proj.ID
                                join tak in db.Tasks on tsd.TaskID equals tak.ID
                                join cli in db.Clients on tsd.ClientID equals cli.ID
                                select new{tsd.ID,tsd.EmpID,emp.FirstName,proj.ProjectName,tak.TaskName,cli.ClientName,tsd.Date,tsd.Hours,tsd.Notes,
                                 tsd.ApprovalStatus,tsd.Starttime,tsd.Endtime,proj.Billable};
                    var response = await result.ToListAsync();
                    return Ok(response);
                }
                if(names.Count!=0 || status.Count!=0)
                {
                    List<int> Empids = await db.Employee.Where(x => names.Contains(x.FirstName)).Select(x => x.ID).ToListAsync();
                    var query1 = from tsd in db.TimesheetDetails where (Empids.Contains(tsd.EmpID)||status.Contains(tsd.ApprovalStatus)) && !tsd.Deleted && tsd.Submit
                                 join emp in db.Employee on tsd.EmpID equals emp.ID
                                 join proj in db.Project on tsd.ProjectID equals proj.ID
                                 join tak in db.Tasks on tsd.TaskID equals tak.ID
                                 join cli in db.Clients on tsd.ClientID equals cli.ID
                                 select new{tsd.ID, tsd.EmpID,emp.FirstName,proj.ProjectName,tak.TaskName,
                                  cli.ClientName,tsd.Date,tsd.Hours,tsd.Notes,tsd.ApprovalStatus,tsd.Starttime,tsd.Endtime,proj.Billable};
                    var final = query1.ToList();
                    return Ok(final);
                }
                return Ok("Get data");
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        [Route("Empname")]
        public async Task<ActionResult> DropDwonName(int ManagerId)
        {
            try
            {
                List<string> Empname = await db.Employee.Where(x => x.ManagerID == ManagerId).Select(x => x.FirstName).ToListAsync();
                return Ok(Empname);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> SendEmail(string request, string Username,int ID)
        {
            try
            {
                string smtpHost = _configuration.GetSection("MailSettings")["Host"];
                int smtpPort = int.Parse(_configuration.GetSection("MailSettings")["Port"]);
                string smtpUsername = _configuration.GetSection("MailSettings")["Mail"];
                string smtpPassword = _configuration.GetSection("MailSettings")["Password"];
                string tem = Temp(ID,Username);
                MailMessage mail = new MailMessage();
                mail.To.Add(new MailAddress(request));
                mail.From = new MailAddress(_configuration.GetSection("MailSettings")["Mail"]);
                mail.Subject = "Reject work";
                string Body =tem;
                mail.Body = Body;
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
        public string Temp(int Recid,string Username)
        {
            var rejectdata = db.TimesheetDetails.Where(x => x.ID == Recid).FirstOrDefault();
            string Clientname = db.Clients.Where(x => x.ID == rejectdata.ClientID).Select(x => x.ClientName).FirstOrDefault();
            string Projectname = db.Project.Where(x => x.ID == rejectdata.ProjectID).Select(x => x.ProjectName).FirstOrDefault();
            string Taskname = db.Tasks.Where(x => x.ID == rejectdata.TaskID).Select(x => x.TaskName).FirstOrDefault();
            var updatedDate = rejectdata.Date.AddHours(9).AddMinutes(30);
            string path = "Template/RejectTem.html";
            string pathtem = System.IO.File.ReadAllText(path);
            pathtem = pathtem.Replace("Username", Username);
            pathtem = pathtem.Replace("clientname", Clientname);
            pathtem = pathtem.Replace("projectname", Projectname);
            pathtem = pathtem.Replace("taskname", Taskname);
            pathtem = pathtem.Replace("date", updatedDate.ToString());
            pathtem = pathtem.Replace("starttime", rejectdata.Starttime);
            pathtem = pathtem.Replace("endtime", rejectdata.Endtime);
            pathtem = pathtem.Replace("descript", rejectdata.Notes);
            return pathtem;
        }

    }
}
