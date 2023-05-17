using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STimesheet.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using STimesheet.DBcontext;
using STimesheet.Models.Modify_Class;
using System.Net.Mail;
using System.Net;

namespace STimesheet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {


        private readonly DataContext db= new DataContext();

        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            this._configuration = configuration;           
        }

        [HttpGet]
        [Route("Getalldata")]
        public async Task<ActionResult> Getdatafromuser(int Empid,DateTime date)
        {
            try
            {
                var query = from tsd in db.TimesheetDetails
                            where tsd.EmpID == Empid && tsd.Date==date &&tsd.Deleted!=true
                            join proj in db.Project on tsd.ProjectID equals proj.ID
                            join tak in db.Tasks on tsd.TaskID equals tak.ID
                            join cli in db.Clients on tsd.ClientID equals cli.ID
                            select new {tsd.ID,tsd.EmpID,proj.ProjectName,tak.TaskName,cli.ClientName,tsd.Date,tsd.Hours, tsd.Notes,
                             tsd.ApprovalStatus,tsd.Starttime,tsd.Endtime,proj.Billable};
                decimal totalHours = query.Sum(q => q.Hours);
                var results = query.ToList();
                var response = new {
                    TotalHours = totalHours,Results = results};
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest($"Error Get all data: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("Addwork")]
        public async Task<ActionResult<List<TimesheetDetails>>> GetTimesheetDetails([FromBody]AddWorks timesheet)
        {
            try
            {
                int ProjectID = await db.Project.Where(x => x.ProjectName == timesheet.ProjectName).Select(x => x.ID).FirstOrDefaultAsync(); 
                int ClientID = await db.Clients.Where(x => x.ClientName == timesheet.ClientName).Select(x => x.ID).FirstOrDefaultAsync();
                int TaskID = await db.Tasks.Where(x => x.TaskName == timesheet.TaskName).Select(x => x.ID).FirstOrDefaultAsync();
                DateTime startTime = DateTime.Parse(timesheet.Starttime);
                DateTime endTime = DateTime.Parse(timesheet.Endtime);
                List<TimesheetDetails> existingTimesheets = await db.TimesheetDetails.Where(x => x.EmpID == timesheet.EmpID && x.Date == timesheet.Date).ToListAsync();
                bool hasOverlappingTimes = existingTimesheets.Any(x =>DateTime.Parse(x.Starttime) < endTime && DateTime.Parse(x.Endtime) > startTime);
                if (hasOverlappingTimes)
                {
                    throw new Exception();
                }
                TimesheetDetails addwork = new TimesheetDetails();
                string format = "yyyy-MM-dd HH:mm:ss";
                DateTime date = timesheet.Date;
                DateTime createdate = timesheet.CreatedDate;
                DateTime updatedDate = timesheet.UpdatedDate;
                addwork.EmpID = timesheet.EmpID;
                addwork.ProjectID = ProjectID;
                addwork.TaskID = TaskID;
                addwork.ClientID = ClientID;
                addwork.Date = DateTime.ParseExact(date.ToString(format), format, CultureInfo.InvariantCulture);
                addwork.Hours = timesheet.Hours;
                addwork.Notes = timesheet.Notes;
                addwork.ApprovalStatus = timesheet.ApprovalStatus;           
                addwork.CreatedDate = DateTime.ParseExact(createdate.ToString(format), format, CultureInfo.InvariantCulture);
                addwork.CreatedBy = timesheet.CreatedBy;
                addwork.Starttime = timesheet.Starttime;
                addwork.Endtime = timesheet.Endtime;
                db.TimesheetDetails.Add(addwork);
                await db.SaveChangesAsync();
                return Ok(Response.StatusCode);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [Route("Viewdata")]
        public async Task<ActionResult<List<TimesheetDetails>>> Viewdatafromuser(int Id)
        {
            try
            {
                var query = from tsd in db.TimesheetDetails
                            where tsd.ID == Id
                            join proj in db.Project on tsd.ProjectID equals proj.ID
                            join tak in db.Tasks on tsd.TaskID equals tak.ID                            
                            join cli in db.Clients on tsd.ClientID equals cli.ID
                            select new{tsd.ID, tsd.EmpID,proj.ProjectName,tak.TaskName,cli.ClientName,tsd.Date,tsd.Hours,tsd.Notes,tsd.ApprovalStatus,tsd.CreatedDate,
                            tsd.CreatedBy,tsd.UpdatedDate,tsd.UpdatedBy,tsd.Starttime,tsd.Endtime,tsd.Submit,proj.Billable};
                var results = query.ToList();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error ViewData: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("Updatedata")]
        public async Task<ActionResult<List<TimesheetDetails>>> UpdateDataFromUser(AddWorks timesheet)
        {
            try
            {
                int ProjectID = await db.Project.Where(x => x.ProjectName == timesheet.ProjectName).Select(x => x.ID).FirstOrDefaultAsync();
                int ClientID = await db.Clients.Where(x => x.ClientName == timesheet.ClientName).Select(x => x.ID).FirstOrDefaultAsync();
                int TaskID = await db.Tasks.Where(x => x.TaskName == timesheet.TaskName).Select(x => x.ID).FirstOrDefaultAsync();
                TimesheetDetails existingRecord = await db.TimesheetDetails.FindAsync(timesheet.ID);
                if (existingRecord != null)
                {
                    string format = "yyyy-MM-dd HH:mm:ss";
                    DateTime date = timesheet.Date;
                    DateTime createdate = timesheet.CreatedDate;
                    DateTime updatedDate = timesheet.UpdatedDate;
                    existingRecord.Hours = timesheet.Hours;
                    existingRecord.Notes = timesheet.Notes;
                    existingRecord.UpdatedDate = DateTime.ParseExact(updatedDate.ToString(format), format, CultureInfo.InvariantCulture);
                    existingRecord.UpdatedBy = timesheet.UpdatedBy;
                    existingRecord.Starttime = timesheet.Starttime;
                    existingRecord.Endtime = timesheet.Endtime;
                    db.Entry(existingRecord).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Ok(Response.StatusCode);
                }
                else
                {
                    return BadRequest($"Record with ID {timesheet.ID} not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating record: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("Deletedata")]
        public async Task<ActionResult<List<TimesheetDetails>>> DeleteDataFromUser(int Id)
        {
            try     
            {
                TimesheetDetails FindRecord = await db.TimesheetDetails.FindAsync(Id);
                if (FindRecord == null)
                {
                    return NotFound();
                }
                FindRecord.Deleted = true;
                await db.SaveChangesAsync();
                return Ok(Response.StatusCode);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Deleted record: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("Submitdata")]
        public async Task<ActionResult<List<TimesheetDetails>>> SubmitDataFromUser(int EmpID, [FromBody]DateSubmit dates)
        {
            try
            {
                List<TimesheetDetails> recordsToUpdate = await db.TimesheetDetails.Where(tsd => tsd.EmpID == EmpID && tsd.Deleted == false && tsd.Date==dates.firstDay||tsd.Date==dates.secondDay || tsd.Date==dates.thirdDay
                ||tsd.Date==dates.fourthDay||tsd.Date==dates.fifthDay|tsd.Date==dates.sixthDay||tsd.Date==dates.lastDay ).ToListAsync();
                if (recordsToUpdate != null && recordsToUpdate.Any())
                {
                    recordsToUpdate.ForEach(r => r.Submit = true);
                    await db.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }
                return Ok(Response.StatusCode);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Deleted record: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("CheckSubmit")]
        public async Task<ActionResult> CheckSubmit(int EmpID, [FromBody] DateSubmit dates)
        {
            try
            {
                bool recordsToUpdate = await db.TimesheetDetails.Where(tsd => tsd.EmpID == EmpID && !tsd.Deleted && tsd.Submit==true && (tsd.Date==dates.firstDay||tsd.Date==dates.secondDay||tsd.Date==dates.thirdDay|| tsd.Date==dates.fourthDay||tsd.Date==dates.fifthDay)).AnyAsync();
                if (recordsToUpdate)
                {
                    return Ok(Response.StatusCode); 
                }
               else
                {
                    throw new Exception(); 
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Deleted record: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("Resubmit")]
        public async Task<ActionResult> Resubmit(int ReportId)
        {
            try
            {
                List<TimesheetDetails> recordsToResent = await db.TimesheetDetails.Where(tsd => tsd.ID == ReportId && tsd.Submit == false && tsd.ApprovalStatus=="Rejected" && tsd.Deleted == false).ToListAsync();

                if (recordsToResent != null && recordsToResent.Any())
                {
                    recordsToResent.ForEach(r => r.ApprovalStatus = "Pending");
                    recordsToResent.ForEach(r => r.Submit = true);
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
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        [Route("DropProject")]
        public async Task<ActionResult<List<Project>>> DropProject()
        {
            try
            {
                List<string> projectNames = await db.Project.Select(p => p.ProjectName).ToListAsync();
                return Ok(projectNames);
            }
           catch(Exception ex)
            {
                return BadRequest($"Error Project name getting:{ ex.Message}");
            }
        }

        [HttpGet]
        [Route("DropTask")]
        public async Task<ActionResult<List<Tasks>>> DropTask()
        {
            try
            {
                List<string> TaskNames = await db.Tasks.Select(p => p.TaskName).ToListAsync();
                return Ok(TaskNames);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Task name getting:{ ex.Message}");
            }
        }


        [HttpGet]
        [Route("DropClient")]
        public async Task<ActionResult<List<Tasks>>> DropClient()
        {
            try
            {
                List<string> ClientNames = await db.Clients.Select(p => p.ClientName).ToListAsync();
                return Ok(ClientNames);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Client Names getting:{ ex.Message}");
            }
        }
        
    }
}
