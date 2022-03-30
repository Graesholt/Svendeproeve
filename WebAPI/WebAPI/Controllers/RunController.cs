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
            return await _context.Runs.Where(r => (r.user.userId == GetUserId()) && (r.deleted == false)).ToListAsync();
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
            var run = await _context.Runs.Include("points").FirstOrDefaultAsync(r => r.runId == runId);
            //.Include("points") gets a list of points associated with this run as a property on the object.

            if (run == null)
            {
                return NotFound();
            }

            //Distance
            double distance = 0;
            foreach(Point point in run.points)
            {

            }

            //Duration
            TimeSpan duration = new TimeSpan(0, 0, 0);
            duration = run.points.Last().dateTime.Subtract(run.dateTime);

            //Average Speed
            float avgSpeedInKmt = 0;

            //Average Speed pr minute (for chart)
            List<float> avgSpeedInKmtPerMinute = new List<float>();

            RunStats runStats = new RunStats(run, distance, duration, avgSpeedInKmt, avgSpeedInKmtPerMinute);

            return runStats;
        }

        /// <summary>
        /// NewRun
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        // POST: api/Run
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("NewRun")]
        public async Task<ActionResult<Run>> PostRun()
        {
            var run = new Run();

            run.dateTime = DateTime.Now; //Do not trust client time, server time is infallable
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRun(int id)
        {
            var run = await _context.Runs.FindAsync(id);
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

        //Calculating distance from coordinates is no simple task, so the following function has been stolen with pride from stackoverflow.
        //Originally written in JavaScript, the small task of converting it to C# fell to myself.
        //It is based on the Haversine formula, a formula for calculating distance on a spherical surface, the sphere in this case being the planet.
        //https://stackoverflow.com/questions/639695/how-to-convert-latitude-or-longitude-to-meters
        protected double latLongDistanceiInMeters(float lat1, float lon1, float lat2, float lon2)
        {
            double R = 6378.137; // Radius of earth in KM
            double dLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            double dLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;

            return d * 1000; // meters
        }
    }
}
