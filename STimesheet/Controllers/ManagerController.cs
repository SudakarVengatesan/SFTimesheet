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
using System.Threading.Tasks;

namespace STimesheet.Controllers
{
    [Route("api/[controller]")]
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
                var query = from tsd in db.TimesheetDetails
                            where empdata.Contains(tsd.EmpID) && !tsd.Deleted
                            join emp in db.Employee on tsd.EmpID equals emp.ID
                            join proj in db.Project on tsd.ProjectID equals proj.ID
                            join tak in db.Tasks on tsd.TaskID equals tak.ID
                            join cli in db.Clients on tsd.ClientID equals cli.ID
                            select new
                            {
                                tsd.ID,
                                tsd.EmpID,
                                emp.FirstName,
                                proj.ProjectName,
                                tak.TaskName,
                                cli.ClientName,
                                tsd.Date,
                                tsd.Hours,
                                tsd.Notes,
                                tsd.ApprovalStatus,
                                tsd.CreatedDate,
                                tsd.CreatedBy,
                                tsd.UpdatedDate,
                                tsd.UpdatedBy,
                                tsd.Starttime,
                                tsd.Endtime,
                                proj.Billable
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
                    var query = from tsd in db.TimesheetDetails
                                where empdata.Contains(tsd.EmpID) && !tsd.Deleted
                                join emp in db.Employee on tsd.EmpID equals emp.ID
                                join proj in db.Project on tsd.ProjectID equals proj.ID
                                join tak in db.Tasks on tsd.TaskID equals tak.ID
                                join cli in db.Clients on tsd.ClientID equals cli.ID
                                where (Searchname == null || proj.ProjectName.ToLower().Contains(Searchname.ToLower()))
                                 && (Searchname == null || tsd.ApprovalStatus.ToLower() == Searchname.ToLower())
                                select new
                                {
                                    tsd.ID,
                                    tsd.EmpID,
                                    emp.FirstName,
                                    proj.ProjectName,
                                    tak.TaskName,
                                    cli.ClientName,
                                    tsd.Date,
                                    tsd.Hours,
                                    tsd.Notes,
                                    tsd.ApprovalStatus,
                                    tsd.CreatedDate,
                                    tsd.CreatedBy,
                                    tsd.UpdatedDate,
                                    tsd.UpdatedBy,
                                    tsd.Starttime,
                                    tsd.Endtime,
                                    proj.Billable
                                };
                    var results = query.ToList();
                    return Ok(results);
                }
                List<int> Empdatas = await db.Employee.Where(x => x.ManagerID == managerID).Select(x => x.ID).ToListAsync();
                int ProjectID = await db.Project.Where(x => x.ProjectName == Searchname).Select(x => x.ID).FirstOrDefaultAsync();
                List<TimesheetDetails> Sname = await db.TimesheetDetails.Where(x => x.ProjectID == ProjectID || x.ApprovalStatus == Searchname).ToListAsync();

                return (Sname);
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

                List<TimesheetDetails> recordsToUpdate = await db.TimesheetDetails.Where(tsd => tsd.EmpID == approve.EmpID && tsd.Submit==true && tsd.ID==approve.ID && tsd.Deleted==false).ToListAsync();
                var managerID = await db.Employee.Where(x => x.ID == approve.EmpID).Select(x=>x.ManagerID).FirstOrDefaultAsync();

                if (recordsToUpdate != null && recordsToUpdate.Any())
                {
                    recordsToUpdate.ForEach(r => r.ApprovalStatus = "Approved");
                    recordsToUpdate.ForEach(r => r.ApprovedBy = managerID);
                    await db.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                return Ok("Approved successfully");
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

                List<TimesheetDetails> recordsToUpdate = await db.TimesheetDetails.Where(tsd => tsd.EmpID == approve.EmpID && tsd.Submit == true && tsd.ID == approve.ID && tsd.Deleted == false).ToListAsync();

                if (recordsToUpdate != null && recordsToUpdate.Any())
                {
                    recordsToUpdate.ForEach(r => r.ApprovalStatus = "Rejected");
                    await db.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }

                return Ok("Rejected successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error Reejected status:{ex.Message}");
            }
        }
    }
}
