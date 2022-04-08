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
using System.Text;
using Newtonsoft.Json;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        string url = "http://localhost:5268/";

        [TestMethod]
        public async Task RegisterUser()
        {
            HttpClient client = new HttpClient();

            User user = new User();
            user.username = "FunRunTesting" + System.DateTime.Now;
            user.password = "tqewsetritnygu";
            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url + "api/User/Register", stringContent);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

            //Test users following this exact naming scheme are cleaned up by a job running once an hour on the database.
        }

        [TestMethod]
        public async Task LoginUser()
        {
            HttpClient client = new HttpClient();

            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tqewsetritnygu";
            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url + "api/User/Login", stringContent);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetRuns()
        {
            Task<string> token = LoginTestUser();

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Result);
            HttpResponseMessage response = await client.GetAsync(url + "api/Run");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetRun()
        {
            string token = await LoginTestUser();
            int runId = await CreateNewRun(token);
            await CreatePoints(token, runId);

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.GetAsync(url + "api/Run/" + runId);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task NewRun()
        {
            Task<string> token = LoginTestUser();

            HttpClient client = new HttpClient();

            var stringContent = new StringContent("");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Result);
            HttpResponseMessage response = await client.PostAsync(url + "api/Run", stringContent);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task DeleteRun()
        {
            string token = await LoginTestUser();
            int runId = await CreateNewRun(token);
            await CreatePoints(token, runId);

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.DeleteAsync(url + "api/Run/" + runId);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task NewPoint()
        {
            string token = await LoginTestUser();
            int runId = await CreateNewRun(token);

            HttpClient client = new HttpClient();

            Point point = new Point();
            point.latitude = 56.3730402904326;
            point.longitude = 9.65708833471725;
            point.altitude = 47;
            var stringContent = new StringContent(JsonConvert.SerializeObject(point), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PostAsync(url + "api/Point/" + runId, stringContent);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
        }

        public async Task<string> LoginTestUser()
        {
            HttpClient client = new HttpClient();

            var stringContent = new StringContent("{\"username\":\"FunRunTesting\", \"password\":\"tqewsetritnygu\"}", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url + "api/User/Login", stringContent);
            string token = await response.Content.ReadAsStringAsync();

            return token;
        }

        public async Task<int> CreateNewRun(string token)
        {
            HttpClient client = new HttpClient();

            var stringContent = new StringContent("");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PostAsync(url + "api/Run", stringContent);
            Run run = await response.Content.ReadAsAsync<Run>();

            return run.runId;
        }

        public async Task CreatePoints(string token, int runId)
        {
            Point[] testRoutePoints = new Point[5];

            testRoutePoints[0] = new Point();
            testRoutePoints[0].latitude = 56.3730402904326;
            testRoutePoints[0].longitude = 9.65708833471725;
            testRoutePoints[0].altitude = 47;

            testRoutePoints[1] = new Point();
            testRoutePoints[1].latitude = 56.3728954245536;
            testRoutePoints[1].longitude = 9.6573206444902;
            testRoutePoints[1].altitude = 47;

            testRoutePoints[2] = new Point();
            testRoutePoints[2].latitude = 56.3730111010834;
            testRoutePoints[2].longitude = 9.65762616112437;
            testRoutePoints[2].altitude = 47;

            testRoutePoints[3] = new Point();
            testRoutePoints[3].latitude = 56.3731640746466;
            testRoutePoints[3].longitude = 9.65734211850282;
            testRoutePoints[3].altitude = 46;

            testRoutePoints[4] = new Point();
            testRoutePoints[4].latitude = 56.3730402904326;
            testRoutePoints[4].longitude = 9.65708638253428;
            testRoutePoints[4].altitude = 47;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            foreach (Point point in testRoutePoints)
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(point), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url + "api/Point/" + runId, stringContent);
            }
        }
    }
}