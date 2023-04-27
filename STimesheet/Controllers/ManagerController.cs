using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using STimesheet.DBcontext;
using STimesheet.Models;
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
        public async Task<ActionResult<List<TimesheetDetails>>> AllDataReport()
        {
            try
            {
                List<TimesheetDetails> Alldata = db.TimesheetDetails.ToList();
               
                return (Alldata);
            }
            catch(Exception ex)
            {
                return BadRequest($"Error Show record: {ex.Message}");
            }
        }
    }
}
