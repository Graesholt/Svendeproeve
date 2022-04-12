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
        /// Returns all runs associated with userId found in JWT token.
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
        /// Takes a runId as part of URL.
        /// Returns Unauthorized if run does not belong to userId found in JWT token.
        /// Returns NotFound if run does not exist.
        /// Returns UnprocessableEntity if run contains no points, which should not happen, but can if user redirects from page before sending any points.
        /// Otherwise, returns run in the form of a RunStats DTO.
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        // GET: api/Run/{runId}
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

            if (run.points.Count() == 0)
            {
                return UnprocessableEntity();
            }

            RunStats runStats = new RunStats(run);

            return runStats;
        }

        /// <summary>
        /// Creates a new Run in database for userId found in JWT token.
        /// Returns the Run object that was created.
        /// </summary>
        /// <returns></returns>
        // POST: api/Run
        [HttpPost]
        public async Task<ActionResult<Run>> NewRun()
        {
            var run = new Run();

            //Do not trust client time, server time is reliable
            run.dateTime = DateTime.UtcNow;
            run.user = await _context.Users.FirstOrDefaultAsync(u => u.userId == GetUserId());
            run.deleted = false;

            _context.Runs.Add(run);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRun", new { id = run.runId }, run);
        }

        /// <summary>
        /// Takes a runId as part of URL.
        /// Returns Unauthorized if run does not belong to userId found in JWT token.
        /// Returns NotFound if run does not exist.
        /// Otherwise, flags run in database as deteled and returns NoContent.
        /// Does NOT actually delete data.
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        // DELETE: api/Run/{runId}
        [HttpDelete("{runId}")]
        public async Task<IActionResult> DeleteRun(int runId)
        {
            //var run = await _context.Runs.FindAsync(runId);
            var run = await _context.Runs.Include("user").FirstOrDefaultAsync(r => r.runId == runId);

            if (run.user.userId != GetUserId())
            {
                return Unauthorized();
            }

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

        //Gets userId from supplied token
        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First(i => i.Type == "userId").Value);
        }
    }
}
