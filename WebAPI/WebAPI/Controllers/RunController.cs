#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Models;

namespace WebAPI.Controllers
{
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
            return await _context.Runs.ToListAsync();
        }

        /// <summary>
        /// GetRun
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Run/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Run>> GetRun(int id)
        {
            var run = await _context.Runs.FindAsync(id);

            if (run == null)
            {
                return NotFound();
            }

            return run;
        }

        /// <summary>
        /// CreateNewRun
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        // POST: api/Run
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Run>> PostRun(Run run)
        {
            _context.Runs.Add(run);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRun", new { id = run.RunId }, run);
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

            _context.Runs.Remove(run);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RunExists(int id)
        {
            return _context.Runs.Any(e => e.RunId == id);
        }
    }
}
