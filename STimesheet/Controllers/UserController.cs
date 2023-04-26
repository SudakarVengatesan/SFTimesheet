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
        public async Task Getdatafromuser()
        {

        }

        [HttpPost]
        [Route("Addwork")]
        public async Task<ActionResult<List<TimesheetDetails>>> GetTimesheetDetails([FromBody]TimesheetDetails timesheet)
        {
            try
            {

                TimesheetDetails addwork = new TimesheetDetails();
                //addwork.Id = timesheet.Id;
                addwork.EmpID = timesheet.EmpID;
                addwork.ProjectID = timesheet.ProjectID;
                addwork.TaskID = timesheet.TaskID;
                addwork.ClientID = timesheet.ClientID;
                addwork.Date = timesheet.Date;
                addwork.Hours = timesheet.Hours;
                addwork.Notes = timesheet.Notes;
                addwork.ApprovalStatus = timesheet.ApprovalStatus;
                addwork.ApprovedBy = timesheet.ApprovedBy;
                addwork.ApprovedDate = timesheet.ApprovedDate;
                addwork.ReviewComments = timesheet.ReviewComments;
                addwork.CreatedDate = timesheet.CreatedDate;
                addwork.UpdatedBy = timesheet.UpdatedBy;
                addwork.UpdatedDate = timesheet.UpdatedDate;
                addwork.Starttime = timesheet.Starttime;
                addwork.Endtime = timesheet.Endtime;
                db.TimesheetDetails.Add(addwork);
                return Ok("Ok work added");

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }


            }

    }
}
