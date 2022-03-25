﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Models;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BCryptNet = BCrypt.Net.BCrypt;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration configuration;

        public UserController(DatabaseContext context, IConfiguration iConfig)
        {
            _context = context;
            configuration = iConfig;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/User/5
        [HttpPost("Login")]
        public async Task<ActionResult<User>> GetUser(User loginUser)
        {
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username);
            
            var user = (await _context.Users.Where(u => u.Username == loginUser.Username).ToListAsync()).FirstOrDefault(u => u.Username == loginUser.Username); //Case sensitivity hack

            if ((user == null) || (!BCryptNet.Verify(loginUser.Password, user.Password)))
            {
                return NotFound("User info not correct");
            }

            string key = configuration.GetValue<string>("Jwt:Key"); //Secret key which will be used later during validation
            var issuer = configuration.GetValue<string>("Jwt:Issuer"); //normally this will be your site URL
            var audience = configuration.GetValue<string>("Jwt:Audience");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            ////Create a List of Claims, Keep claims name short
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("UserId", user.UserId.ToString()));

            ////Create Security Token object by giving required parameters
            var token = new JwtSecurityToken(issuer, ////Issuer
                            audience,  ////Audience
                            permClaims,
                            expires: DateTime.Now.AddDays(31), //31 days to avoid logout while running.
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(jwt_token);
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Register")]
        public async Task<ActionResult<User>> RegisterUser(User registerUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == registerUser.Username);

            if (user != null)
            {
                return Conflict("Username already registered");
            }

            registerUser.Password = BCryptNet.HashPassword(registerUser.Password, BCryptNet.GenerateSalt());

            _context.Users.Add(registerUser);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}