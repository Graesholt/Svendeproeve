using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace UnitTests
{
    [TestClass]
    public class BlackboxTests
    {
        //Blackbox tests of API endpoints
        //Must be run in a seperate Visual Studio instance while API is running
        //tests call the endpoints of the API from the outside

        string url = "http://localhost:5268/"; //Base URL of the test API

        //Tests that new users can be registered
        [TestMethod]
        public async Task RegisterUser()
        {
            HttpClient client = new HttpClient();

            User user = new User();
            user.username = "FRT-" + System.DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss.fff");
            user.password = "tQeWsEtRiTnYg5";
            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url + "api/User/Register", stringContent);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

            //Test users following this exact naming scheme are cleaned up by a job running once an hour on the in-house test database
            //Should be practically impossible to generate two users with the same username, as the test would have to run twice in one millisecond
            //Depending on the size of the dev team running tests this is unlikely to happen
        }

        //Tests that user can log in
        [TestMethod]
        public async Task LoginUser()
        {
            HttpClient client = new HttpClient();

            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tQeWsEtRiTnYg5";
            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url + "api/User/Login", stringContent);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        //Tests that user can get a list of their Runs
        [TestMethod]
        public async Task GetRuns()
        {
            Task<string> token = LoginTestUser();

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Result);
            HttpResponseMessage response = await client.GetAsync(url + "api/Run");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
        }

        //Tests that users can get detailed information about a Run
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

        //Tests that the API can return Runs containing no Points
        //Should not happen, but might if user is too fast to use system back-button in app
        [TestMethod]
        public async Task GetRunNoPoints()
        {
            string token = await LoginTestUser();
            int runId = await CreateNewRun(token);

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.GetAsync(url + "api/Run/" + runId);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.UnprocessableEntity);
        }

        //Tests that the API can return Runs containing only a single Point
        [TestMethod]
        public async Task GetRunSinglePoint()
        {
            string token = await LoginTestUser();
            int runId = await CreateNewRun(token);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            Point point = new Point();
            point.latitude = 56.3730402904326;
            point.longitude = 9.65708833471725;
            point.altitude = 47;
            var stringContent = new StringContent(JsonConvert.SerializeObject(point), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url + "api/Point/" + runId, stringContent);

            HttpResponseMessage response2 = await client.GetAsync(url + "api/Run/" + runId);

            Assert.AreEqual(response2.StatusCode, HttpStatusCode.OK);
        }

        //Tests that NotFound is returned if Run does not exist
        [TestMethod]
        public async Task GetRunNotFound()
        {
            string token = await LoginTestUser();

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.GetAsync(url + "api/Run/-1");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        //Tests that users can NOT get Runs which do not belong to them
        [TestMethod]
        public async Task GetRunUnauthorized()
        {
            string token = await LoginTestUser();
            int runId = await CreateNewRun(token);
            await CreatePoints(token, runId);

            string unauthorizedToken = await LoginUnauthorizedTestUser();

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + unauthorizedToken);
            HttpResponseMessage response = await client.GetAsync(url + "api/Run/" + runId);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }

        //Tests that users can create new runs
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

        //Tests that users can delete Runs
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

        //Tests that NotFound is returned if Run does not exist
        [TestMethod]
        public async Task DeleteRunNotFound()
        {
            string token = await LoginTestUser();

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.DeleteAsync(url + "api/Run/-1");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        //Tests that users can NOT delete Runs which do not belong to them
        [TestMethod]
        public async Task DeleteRunUnauthorized()
        {
            string token = await LoginTestUser();
            int runId = await CreateNewRun(token);
            await CreatePoints(token, runId);

            string unauthorizedToken = await LoginUnauthorizedTestUser();

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + unauthorizedToken);
            HttpResponseMessage response = await client.DeleteAsync(url + "api/Run/" + runId);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }

        //Tests that users can add Points to Runs
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

        //Tests that NotFound is returned if Run does not exist
        [TestMethod]
        public async Task NewPointNotFound()
        {
            string token = await LoginTestUser();

            HttpClient client = new HttpClient();

            Point point = new Point();
            point.latitude = 56.3730402904326;
            point.longitude = 9.65708833471725;
            point.altitude = 47;
            var stringContent = new StringContent(JsonConvert.SerializeObject(point), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PostAsync(url + "api/Point/-1", stringContent);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        //Tests that user can NOT add points to Runs which do not belong to them
        [TestMethod]
        public async Task NewPointUnauthorized()
        {
            string token = await LoginTestUser();
            int runId = await CreateNewRun(token);

            string unauthorizedToken = await LoginUnauthorizedTestUser();

            HttpClient client = new HttpClient();

            Point point = new Point();
            point.latitude = 56.3730402904326;
            point.longitude = 9.65708833471725;
            point.altitude = 47;
            var stringContent = new StringContent(JsonConvert.SerializeObject(point), Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + unauthorizedToken);
            HttpResponseMessage response = await client.PostAsync(url + "api/Point/" + runId, stringContent);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }

        //Logs in a user with test credentials
        //Returns the users token
        public async Task<string> LoginTestUser()
        {
            HttpClient client = new HttpClient();

            User user = new User();
            user.username = "FunRunTesting";
            user.password = "tQeWsEtRiTnYg5";
            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url + "api/User/Login", stringContent);
            string token = await response.Content.ReadAsStringAsync();

            return token;
        }

        //Logs in a user with different test credentials
        //Returns the users token
        public async Task<string> LoginUnauthorizedTestUser()
        {
            HttpClient client = new HttpClient();

            User user = new User();
            user.username = "FunRunTestingUnauthorized";
            user.password = "tQeWsEtRiTnYg5";
            var stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url + "api/User/Login", stringContent);
            string token = await response.Content.ReadAsStringAsync();

            return token;
        }

        //Takes a token
        //Creates a new run
        //Returns the runId
        public async Task<int> CreateNewRun(string token)
        {
            HttpClient client = new HttpClient();

            var stringContent = new StringContent("");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await client.PostAsync(url + "api/Run", stringContent);
            Run run = await response.Content.ReadAsAsync<Run>();

            return run.runId;
        }

        //Takes a runId
        //Creates five points comprising a small test route
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