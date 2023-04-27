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
        public async Task<ActionResult<List<TimesheetDetails>>> GetTimesheetDetails([FromBody]TimesheetDetails timesheet)
        {
            try
            {

                TimesheetDetails addwork = new TimesheetDetails();
                string format = "yyyy-MM-dd HH:mm:ss";
                DateTime date = timesheet.Date;
                DateTime approvedDate = timesheet.ApprovedDate;
                DateTime createdate = timesheet.CreatedDate;
                DateTime updatedDate = timesheet.UpdatedDate;
                addwork.EmpID = timesheet.EmpID;
                addwork.ProjectID = timesheet.ProjectID;
                addwork.TaskID = timesheet.TaskID;
                addwork.ClientID = timesheet.ClientID;
                addwork.Date = DateTime.ParseExact(date.ToString(format), format, CultureInfo.InvariantCulture);
                addwork.Hours = timesheet.Hours;
                addwork.Notes = timesheet.Notes;
                addwork.ApprovalStatus = timesheet.ApprovalStatus;           
                addwork.ApprovedDate = DateTime.ParseExact(approvedDate.ToString(format), format, CultureInfo.InvariantCulture);
                addwork.ApprovedBy = timesheet.ApprovedBy;
                addwork.ReviewComments = timesheet.ReviewComments;
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
                //List<TimesheetDetails> details = await db.TimesheetDetails.Where(user => user.ID == Id).ToListAsync();
                var query = from tsd in db.TimesheetDetails where tsd.ID == Id join proj in db.Project on tsd.ProjectID equals proj.ID select new
                            {tsd.ID, tsd.EmpID,tsd.ProjectID,tsd.TaskID,tsd.ClientID,tsd.Date,tsd.Hours,tsd.Notes,tsd.ApprovalStatus,tsd.ApprovedDate,tsd.ApprovedBy,tsd.ReviewComments,tsd.CreatedDate,
                             tsd.CreatedBy,tsd.UpdatedDate,tsd.UpdatedBy,tsd.Starttime,tsd.Endtime,proj.ProjectName,proj.ProjectType,proj.EstimationHrs,proj.SoNumber,proj.ProjectManager,
                             proj.TeamLead,proj.StartDate,proj.EndDate,proj.Billable,proj.IsActive,proj.Deleted};
                // Execute the query to get the results
                var results = query.ToList();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error ViewData: {ex.Message}");
            }

        }



    }
}
