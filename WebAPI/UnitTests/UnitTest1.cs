using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI.Controllers;
using WebAPI.Models;
using WebAPI.Context;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using WebAPI;
using System.Collections.Generic;
using System.Security.Claims;
using System.Net;
using System.Net.Http;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void CreateNewUser()
        {
            UserController userController = UserControllerSetup();

            User testUser = new User() { username = "tobitest" + System.DateTime.Now, password = "123qwe123" };
            ActionResult<User> response = userController.RegisterUser(testUser).Result;

            var result = response.Result as StatusCodeResult;
            Assert.AreEqual(result.StatusCode, 201);
        }

        [TestMethod]
        public void LoginUser()
        {
            User testUser = CreateUser();

            UserController userController = UserControllerSetup();

            ActionResult<User> response = userController.GetUser(testUser).Result;

            var result = response.Result as OkObjectResult;
            Assert.AreEqual(result.StatusCode, 200);
        }

        [TestMethod]
        public void GetRuns()
        {

        }

        [TestMethod]
        public async Task GetRunDataAsync()
        {
            User testUser = CreateUser();
            string token = LoginUser(testUser);

            HttpClient client = new HttpClient();

            var url = "http://localhost:5268/api/Run";

            client.DefaultRequestHeaders.Add("accept", "text/plain");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

        }

        [TestMethod]
        public void CreateNewRun()
        {
            User testUser = CreateUser();
            string token = LoginUser(testUser);
            //var token2 = token.Result.ToString() as JwtSecurityToken;

            RunController runController = RunControllerSetup();

            //runController.HttpContext.User.Identity(token);
            var claims = new List<Claim>();
            claims.Add(new Claim("userId", "0"));
            var appidentity = new ClaimsIdentity(claims);

            runController.User.AddIdentity(appidentity);

            ActionResult<Run> response = runController.PostRun().Result;


        }

        [TestMethod]
        public void DeleteRun()
        {

        }

        [TestMethod]
        public void CreatePoint()
        {

        }

        private static UserController UserControllerSetup()
        {
            DatabaseContext databaseContext = DatabaseSetup();

            UserController userController = new UserController(databaseContext, null);
            return userController;
        }

        private static RunController RunControllerSetup()
        {
            DatabaseContext databaseContext = DatabaseSetup();

            RunController runController = new RunController(databaseContext);
            return runController;
        }

        private static PointController PointControllerSetup()
        {
            DatabaseContext databaseContext = DatabaseSetup();

            PointController pointController = new PointController(databaseContext);
            return pointController;
        }

        private static DatabaseContext DatabaseSetup()
        {
            var connectionstring = "Data Source =.; Initial Catalog = RunDBtest; Integrated Security = True;";

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(connectionstring);

            DatabaseContext databaseContext = new DatabaseContext(optionsBuilder.Options);
            databaseContext.Database.Migrate();
            return databaseContext;
        }

        private static User CreateUser()
        {
            UserController userController = UserControllerSetup();

            User user = new User() { username = "tobitest" + System.DateTime.Now, password = "123qwe123" };
            ActionResult<User> response = userController.RegisterUser(user).Result;

            //Reset password after hashing
            user.password = "123qwe123";

            return user;
        }

        private static string LoginUser(User user)
        {
            UserController userController = UserControllerSetup();

            ActionResult<User> response = userController.GetUser(user).Result;
            //JwtSecurityToken jwtSecurityToken = response.Value.ToString() as JwtSecurityToken;
            var result = response.Result;
            var result2 = result.GetType().GetProperty("Value");
            var result3 = result2.GetValue(result, null).ToString();
            var result4 = new JwtSecurityTokenHandler().ReadToken(result3.ToString());

            return result3;
        }
    }
}