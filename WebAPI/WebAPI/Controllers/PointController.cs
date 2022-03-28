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

        // POST: api/Point
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("NewPoint")]
        public async Task<ActionResult<Point>> PostPoint(Point point)
        {
            point.dateTime = DateTime.Now; //Do not trust client time, server time is infallable

            _context.Points.Add(point);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        private bool PointExists(int id)
        {
            return _context.Points.Any(e => e.pointId == id);
        }
    }
}
