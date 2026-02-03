
using ApiAutomationFramework;
using ApiAutomationFramework.Models.Request.Users;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APITestSolution.TestsScripts.Users
{
    [TestFixture]
    public class UserTests : BaseTest
    {

        //Commom method can used to create new users and get the UserId
        private async Task<int> CreateUserAndReturnIdAsync()
        {
            var endpoint = ApiEndpoints.Users_createuser;

            // Minimal positive payload 
            var payload = new UserscreateRequest
            {
                userName= "AutoFN_" + Guid.NewGuid().ToString("N").Substring(0, 3),
                StatusId =1216,
                firstName = "AutoFN_" + Guid.NewGuid().ToString("N").Substring(0, 5),
                lastName = "AutoLN_" + Guid.NewGuid().ToString("N").Substring(0, 5),
                email = $"auto_{Guid.NewGuid().ToString("N").Substring(0, 8)}@mail.com",
                pclIds = new List<int> { 13 },
                roleIds = new List<int> { 5 }
            };

            _test.Info("Pre-Req : Creating User to get UserId..");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Pre-Req Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Pre-Req Response Status: {response.StatusCode}");
            _test.Info($"Pre-Req Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            string msg = (response.Content ?? string.Empty).Trim().Trim('"');
            string extractedId = new string(msg.Where(char.IsDigit).ToArray());

            Assert.That(extractedId, Is.Not.Empty, "Could not extract UserId from create response.");

            int userId = int.Parse(extractedId);

            _test.Info($"Pre-requisite: User created successfully with userId = {userId}");

            return userId;
        }

        // 🔖 POST – Create (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_Create_Positive_TestData))]
        public async Task Users_Creation_PositiveTestMethod(UserscreateRequest payload)
        {
            var endpoint = ApiEndpoints.Users_createuser;

            _test.Info("Running user Create POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "User " + userId + " created successfully. An email has been sent to the registered email address to configure the credentials.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Positive CreateUser assertions passed");
        }

        // 🔖 POST – Create (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_Create_Negative_TestData))]
        public async Task Users_Creation_NegativeTestMethod(UserscreateRequest payload)
        {
            var endpoint = ApiEndpoints.Users_createuser;

            _test.Info("Running user Create NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response,HttpStatusCode.NotFound);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            string messageForAssert = actualMessage;
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(actualMessage);
                messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
            }
            catch { /* plain string */ }

            Assert.That(messageForAssert, Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));
            _test.Pass("Negative CreateUser assertions passed");
        }

        // 🔖 PUT – Update (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_Update_Positive_TestData))]
        public async Task Users_Update_PositiveTestMethod(UsersUpdateRequest payload)
        {
            // 1️⃣ Pre-requisite: Create a new user dynamically
            int newUserId = await CreateUserAndReturnIdAsync();

            // 2️⃣ Inject the valid dynamic userId into the payload
            payload.userId = newUserId;

            var endpoint = ApiEndpoints.Users_updateuser;

            _test.Info("Running Users UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response,HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage = "User " + userId + " Updated Successfully.";
          
            Assert.That(actualMessage, Is.EqualTo(expectedMessage));

            _test.Pass("Users UPDATE (positive) assertions passed.");
        }


        // 🔖 PUT – Update (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_Update_Negative_TestData))]
        public async Task Users_Update_NegativeTestMethod(UsersUpdateRequest payload)
        {

            // 1️⃣ Pre-requisite: Create a new user dynamically
            int newUserId = await CreateUserAndReturnIdAsync();

            // 2️⃣ Inject the valid dynamic userId into the payload
            payload.userId = newUserId;
            var endpoint = ApiEndpoints.Users_updateuser;

            _test.Info("Running Users UPDATE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");
            ResponseValidator.ValidateStatusCode(response,HttpStatusCode.NotFound);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            string messageForAssert = actualMessage;
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(actualMessage);
                messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
            }
            catch { }

            Assert.That(messageForAssert, Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));
            _test.Pass("Users UPDATE (negative) assertions passed.");
        }

        // 🔖 GET – User by ID (Positive)
        [Test]
        public async Task Users_GetById_Positive_TestMethod()
        {
            // 1️⃣ Pre‑req: create user first
            int userId = await CreateUserAndReturnIdAsync();

            var endpoint = $"{ApiEndpoints.Users_GetUsers}";

            _test.Info("Running Users GET BY ID POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0));

            var obj = JObject.Parse(body);

            int returnedId = obj["id"]?.ToObject<int>() ?? 0;

            Assert.That(returnedId, Is.EqualTo(userId),
                $"Expected returned userId {returnedId} to match created userId {userId}");

            _test.Pass("Users GET BY ID (positive) assertions passed.");
        }

        // 🔖 GET – Users by PCL (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.User_GetBy_InValidPcl_TestData))]
        public async Task Users_GetByPcl_NegativeTestMethod(int pclId, bool isInternal)
        {
            var endpoint = ApiEndpoints.Users_GetUsers;

            _test.Info("Running Users GET BY PCL NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // If API returns 4xx for invalid PCL, accept that:
            if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
            {
                Assert.Pass("Negative GET returned a client error as expected.");
                return;
            }

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var arr = JArray.Parse((response.Content ?? string.Empty).Trim());
            Assert.That(arr.Count, Is.EqualTo(0), "Negative GET should return empty list for invalid PCL.");
            _test.Pass("Users GET BY PCL (negative) assertions passed.");
        }
    }
}
