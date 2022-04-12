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
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PointController(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Takes a runId as part of URL and a Point object from the webservice.
        /// Returns Unauthorized if run does not belong to userId found in JWT token.
        /// Otherwise, adds point to database and returns Status Code 201.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="runId"></param>
        /// <returns></returns>
        // POST: api/Point/{runId}
        [HttpPost("{runId}")]
        public async Task<ActionResult<Point>> NewPoint(Point point, int runId)
        {
            point.dateTime = DateTime.UtcNow; //Do not trust client time, server time is reliable
            var run = _context.Runs.Include("user").Include("points").FirstOrDefault(r => r.runId == runId);

            if (run.user.userId != GetUserId())
            {
                return Unauthorized();
            }

            run.points.Add(point);
            //_context.Points.Add(point);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        private bool PointExists(int id)
        {
            return _context.Points.Any(e => e.pointId == id);
        }

        //Gets userId from supplied token
        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First(i => i.Type == "userId").Value);
        }
    }
}
