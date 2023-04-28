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

namespace STimesheet.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<List<TimesheetDetails>>> Getdatafromuser(int Empid,DateTime date)
        {
            try
            {
                List<TimesheetDetails> details= await db.TimesheetDetails.Where(user => user.EmpID == Empid && user.Date==date).ToListAsync();
                return Ok(details);
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

                TimesheetDetails addwork = new TimesheetDetails();
                string format = "yyyy-MM-dd HH:mm:ss";
                DateTime date = timesheet.Date;
               // DateTime approvedDate = timesheet.ApprovedDate;
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
                //addwork.ApprovedDate = DateTime.ParseExact(approvedDate.ToString(format), format, CultureInfo.InvariantCulture);
                //addwork.ApprovedBy = timesheet.ApprovedBy;
                //addwork.ReviewComments = timesheet.ReviewComments;
                addwork.CreatedDate = DateTime.ParseExact(createdate.ToString(format), format, CultureInfo.InvariantCulture);
                addwork.CreatedBy = timesheet.CreatedBy;
                addwork.UpdatedDate = DateTime.ParseExact(updatedDate.ToString(format), format, CultureInfo.InvariantCulture);
                addwork.UpdatedBy = timesheet.UpdatedBy;
                addwork.Starttime = timesheet.Starttime;
                addwork.Endtime = timesheet.Endtime;
                db.TimesheetDetails.Add(addwork);
                await db.SaveChangesAsync();
                return Ok("Ok work added");
            }
            catch(Exception ex)
            {
                return BadRequest($"Error adding work: {ex.Message}");
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
                            select new
                            {
                            
                            tsd.ID, tsd.EmpID,proj.ProjectName,tak.TaskName,cli.ClientName,tsd.Date,tsd.Hours,tsd.Notes,tsd.ApprovalStatus,tsd.CreatedDate,
                             tsd.CreatedBy,tsd.UpdatedDate,tsd.UpdatedBy,tsd.Starttime,tsd.Endtime,proj.Billable};
                // Execute the query to get the results
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
        public async Task<ActionResult<List<TimesheetDetails>>> UpdateDataFromUser(int Id, AddWorks timesheet)
        {
            try
            {
                int ProjectID = await db.Project.Where(x => x.ProjectName == timesheet.ProjectName).Select(x => x.ID).FirstOrDefaultAsync();
                int ClientID = await db.Clients.Where(x => x.ClientName == timesheet.ClientName).Select(x => x.ID).FirstOrDefaultAsync();
                int TaskID = await db.Tasks.Where(x => x.TaskName == timesheet.TaskName).Select(x => x.ID).FirstOrDefaultAsync();
                TimesheetDetails existingRecord = await db.TimesheetDetails.FindAsync(Id);

                if (existingRecord != null)
                {
                    string format = "yyyy-MM-dd HH:mm:ss";
                    DateTime date = timesheet.Date;
                    //DateTime approvedDate = timesheet.ApprovedDate;
                    DateTime createdate = timesheet.CreatedDate;
                    DateTime updatedDate = timesheet.UpdatedDate;
                    existingRecord.EmpID = timesheet.EmpID;
                    existingRecord.ProjectID = ProjectID;
                    existingRecord.TaskID = TaskID;
                    existingRecord.ClientID = ClientID;
                    existingRecord.Hours = timesheet.Hours;
                    existingRecord.Notes = timesheet.Notes;
                    existingRecord.ApprovalStatus = timesheet.ApprovalStatus;
                    //existingRecord.ApprovedDate = DateTime.ParseExact(approvedDate.ToString(format), format, CultureInfo.InvariantCulture);
                    //existingRecord.ApprovedBy = timesheet.ApprovedBy;
                   // existingRecord.ReviewComments = timesheet.ReviewComments;
                    existingRecord.CreatedDate = DateTime.ParseExact(createdate.ToString(format), format, CultureInfo.InvariantCulture);
                    existingRecord.CreatedBy = timesheet.CreatedBy;
                    existingRecord.UpdatedDate = DateTime.ParseExact(updatedDate.ToString(format), format, CultureInfo.InvariantCulture);
                    existingRecord.UpdatedBy = timesheet.UpdatedBy;
                    existingRecord.Starttime = timesheet.Starttime;
                    existingRecord.Endtime = timesheet.Endtime;
                    db.Entry(existingRecord).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Ok("Record updated successfully");
                }
                else
                {
                    return BadRequest($"Record with ID {Id} not found");
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
                //db.TimesheetDetails.Remove(FindRecord);
                await db.SaveChangesAsync();
                return Ok("Record deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Deleted record: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("Submitdata")]
        public async Task<ActionResult<List<TimesheetDetails>>> SubmitDataFromUser(int EmpID)
        {
            try
            {
                List<TimesheetDetails> recordsToUpdate = await db.TimesheetDetails.Where(tsd => tsd.EmpID == EmpID).ToListAsync();

                if (recordsToUpdate != null && recordsToUpdate.Any())
                {
                    recordsToUpdate.ForEach(r => r.Submit = true);
                    await db.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                return Ok("Submit successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Deleted record: {ex.Message}");
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
