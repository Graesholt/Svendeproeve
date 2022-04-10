#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.DTO;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RunController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public RunController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GetAllForUser
        /// </summary>
        /// <returns></returns>
        // GET: api/Run
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Run>>> GetRuns()
        {
            //return await _context.Runs.ToListAsync();
            return await _context.Runs.Where(r => (r.user.userId == GetUserId()) && (r.deleted == false)).OrderByDescending(r => r.dateTime).ToListAsync();
        }

        /// <summary>
        /// GetRun
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Run/5
        [HttpGet("{runId}")]
        public async Task<ActionResult<RunStats>> GetRun(int runId)
        {
            var run = await _context.Runs.Include("user").Include("points").FirstOrDefaultAsync(r => r.runId == runId);
            //.Include("points") gets a list of points associated with this run as a property on the object.

            if (run.user.userId != GetUserId())
            {
                return Unauthorized();
            }

            if (run == null)
            {
                return NotFound();
            }

            RunStats runStats = new RunStats(run);

            return runStats;
        }

        /// <summary>
        /// NewRun
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        // POST: api/Run
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Run>> NewRun()
        {
            var run = new Run();

            run.dateTime = DateTime.UtcNow; //Do not trust client time, server time is reliable
            run.user = await _context.Users.FirstOrDefaultAsync(u => u.userId == GetUserId());
            run.deleted = false;

            _context.Runs.Add(run);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRun", new { id = run.runId }, run);
        }

        /// <summary>
        /// DeleteRun
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Run/5
        [HttpDelete("{runId}")]
        public async Task<IActionResult> DeleteRun(int runId)
        {
            var run = await _context.Runs.FindAsync(runId);
            if (run == null)
            {
                return NotFound();
            }

            run.deleted = true;
            //_context.Runs.Remove(run);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RunExists(int id)
        {
            return _context.Runs.Any(e => e.runId == id);
        }

        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First(i => i.Type == "userId").Value);
        }
    }
}
