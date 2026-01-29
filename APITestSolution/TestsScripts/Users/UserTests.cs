
using ApiAutomationFramework;
using ApiAutomationFramework.Models.Request.Users;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APITestSolution.TestsScripts.Users
{
    [TestFixture]
    public class UserTests : BaseTest
    {
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

            Assert.That((int)response.StatusCode, Is.InRange(400, 499),
                $"Expected 4xx for invalid payload, but was {(int)response.StatusCode} ({response.StatusCode}).");

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
            var endpoint = ApiEndpoints.Users_updateuser;

            _test.Info("Running Users UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NoContent));

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            Assert.That(actualMessage, Does.Not.Match(@"(?i)(invalid|error|required|missing|failed|not\s*allowed|format)"));

            _test.Pass("Users UPDATE (positive) assertions passed.");
        }

        // 🔖 PUT – Update (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_Update_Negative_TestData))]
        public async Task Users_Update_NegativeTestMethod(UsersUpdateRequest payload)
        {
            var endpoint = ApiEndpoints.Users_updateuser;

            _test.Info("Running Users UPDATE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That((int)response.StatusCode, Is.InRange(400, 499),
                $"Expected 4xx for invalid payload, but was {(int)response.StatusCode} ({response.StatusCode}).");

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

        // 🔖 GET – Users by PCL (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_GetByPcl_Positive_TestData))]
        public async Task Users_GetByPcl_Positive_TestMethod(int pclId, bool isInternal)
        {
            var endpoint = ApiEndpoints.Users_GetUsers;

            _test.Info("Running Users GET BY PCL POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0));

            var arr = JArray.Parse(body);
            Assert.That(arr.Count, Is.GreaterThan(0), "Expected one or more users for PclId filter.");

            foreach (var item in arr)
            {
                var pclIds = item["pclIds"]?.ToObject<int[]>() ?? System.Array.Empty<int>();
                Assert.That(pclIds.Contains(pclId), $"User {item["id"]} does not have expected PCL {pclId}.");

                if (isInternal)
                {
                    var flag = (item["internalUserFlag"]?.ToString() ?? "").Trim();
                    Assert.That(flag.Equals("Y", System.StringComparison.OrdinalIgnoreCase),
                        $"User {item["id"]} should be internal (Y).");
                }

                var statusIdToken = item["statusId"];
                if (statusIdToken != null && statusIdToken.Type != JTokenType.Null)
                {
                    var sid = statusIdToken.ToObject<int>();
                    Assert.That(sid, Is.EqualTo(1216), $"User {item["id"]} has unexpected statusId {sid}.");
                }
            }

            _test.Pass("Users GET BY PCL (positive) assertions passed.");
        }

        // 🔖 GET – Users by PCL (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_GetByPcl_Negative_TestData))]
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
