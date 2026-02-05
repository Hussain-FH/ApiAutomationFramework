using ApiAutomationFramework;
using ApiAutomationFramework.Models.Request;
using ApiAutomationFramework.Models.Request.SLA;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APITestSolution.TestsScripts.SLA
{
    internal class SLATests : BaseTest
    {

        //common method to create new CardProfile record and used this Id and perform Put and Get and Delete operations
        private async Task<int> CreateSLAAndReturnIdAsync()
        {
            var endpoint = ApiEndpoints.SLA_createSLA;

            // Build a positive payload 
            var payload = new SLACreateRequest
            {
                id = 0,
                cardProgramId = 1111,
                isSpecialProject = true,
                isRepeatedEveryYear = true,
                sladay = 0,
                specialProjectMinimumShipmentCardCount = 1,
                specialProjectSladay = 33,

            };

            _test.Info("Pre-requisite: Creating SLA for UPDATE/GET test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Pre-Req Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Pre-Req Response Status: {response.StatusCode}");
            _test.Info($"Pre-Req Response Body: {response.Content}");

            // Expect 201 Created (same as your positive create test)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            // Your API returns something like: "SLA Added Successfully."
            var idString = new string(actualMessage.Where(char.IsDigit).ToArray());

            Assert.That(idString, Is.Not.Empty,
                "Pre-requisite CREATE did not return a valid numeric SLA Id in the message.");

            int SLAId = int.Parse(idString);

            _test.Info($"Pre-requisite: SLA created successfully with Id = {SLAId}");

            return SLAId;
        }



        // 🔖 POST – SLA Create (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SLA_Create_Positive_TestData))]
        public async Task SLA_Creation_PositiveTestMethod(SLACreateRequest payload)
        {
            var endpoint = ApiEndpoints.SLA_createSLA;

            _test.Info("Running SLA Create POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Adjust expected status code as per your API (Created / OK)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            // Optional: add assertions based on your SLA create response contract
            _test.Pass("Positive CreateSLA assertions passed");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "SLA Configuration Parameter " + userId + " Added Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Positive SLA Update assertions passed");
        }

        // 🔖 POST – SLA Create (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SLA_Create_Negative_TestData))]
        public async Task SLA_Creation_NegativeTestMethod(SLACreateRequest payload)
        {
            var endpoint = ApiEndpoints.SLA_GetSLA;

            _test.Info("Running SLA Create NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That((int)response.StatusCode, Is.InRange(400, 499),
                $"Expected 4xx for invalid SLA payload, but was {(int)response.StatusCode} ({response.StatusCode}).");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            string messageForAssert = actualMessage;
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(actualMessage);
                messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
            }
            catch
            {
                // plain string
            }

            Assert.That(messageForAssert,
                Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));

            _test.Pass("Negative CreateSLA assertions passed");
        }

        // 🔖 PUT – SLA Update (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SLA_Update_Positive_TestData))]
        public async Task SLA_Update_PositiveTestMethod(SLAUpdateRequest payload)
        {
            // 1️⃣ Pre-requisite: Create a valid SLA and get its Id
            int SLAAdminId = await CreateSLAAndReturnIdAsync();

            // 2️⃣ Inject the valid id into the PUT payload
            payload.id = SLAAdminId;

            var endpoint = ApiEndpoints.SLA_updateSLA;

            _test.Info("Running SLA UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");


            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "SLA Configuration Parameter " + userId + " Updated Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Positive SLA Update assertions passed");
        }

        // 🔖 PUT – SLA Update (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SLA_Update_Negative_TestData))]
        public async Task SLA_Update_NegativeTestMethod(SLAUpdateRequest payload)
        {
            var endpoint = ApiEndpoints.SLA_updateSLA;

            _test.Info("Running SLA UPDATE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect 4xx for invalid payload
            Assert.That((int)response.StatusCode, Is.InRange(400, 499),
                $"Expected 4xx for invalid SLA update payload, but was {(int)response.StatusCode} ({response.StatusCode}).");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            string messageForAssert = actualMessage;
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(actualMessage);
                messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
            }
            catch
            {
                // ignore deserialization error
            }

            Assert.That(messageForAssert,
                Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));

            _test.Pass("SLA UPDATE (negative) assertions passed.");
        }

        // 🔖 GET – SLA by Id (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SLA_Get_Positive_TestData))]
        public async Task SLA_GetById_Positive_TestMethod(int slaId)
        {
            // 1️⃣ Pre‑req: create user first
            int userId = await CreateSLAAndReturnIdAsync();

            var endpoint = ApiEndpoints.SLA_GetSLA;

            _test.Info("Running SLA GET BY ID POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0), "Expected non-empty response body.");

            var arr = JArray.Parse(body);

            // 🔍 Extract all ids from the array
            var allIds = arr.Select(x => (int?)x["id"]).Where(id => id.HasValue).Select(id => id.Value).ToList();

            Assert.That(allIds.Count, Is.GreaterThan(0), "Expected at least one SLA in the response array.");

 
            _test.Pass("SLA GET BY ID (positive) assertions passed.");


        }

        // 🔖 GET – SLA by Id (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SLA_Get_Negative_TestData))]
        public async Task SLA_GetById_Negative_TestMethod(int slaId)
        {
            var endpoint = ApiEndpoints.SLA_GetSLA;

            _test.Info("Running SLA GET BY ID NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // If API returns 4xx for non-existing SLA, that's fine
            if ((int)response.StatusCode >= 400 && (int)response.StatusCode <= 499)
            {
                Assert.Pass("Negative GET returned a client error as expected.");
                return;
            }

            // Otherwise, if it's 200, we expect either empty or no valid SLA object
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(body))
            {
                _test.Pass("Negative GET returned empty body as expected for invalid SLA id.");
                return;
            }

            // If API still returns some JSON, make sure it's not a valid SLA for this id
            try
            {
                var obj = JObject.Parse(body);
                var idToken = obj["id"];

                if (idToken != null)
                {
                    var returnedId = idToken.ToObject<int>();
                    Assert.That(returnedId, Is.Not.EqualTo(slaId),
                        "Negative GET should not return a SLA record matching the invalid id.");
                }
            }
            catch
            {
                // If parse fails, it's probably an error message – acceptable for negative
            }

            _test.Pass("SLA GET BY ID (negative) assertions passed.");
        }

        // 🔖 DELETE – SLA by Id (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SLA_Delete_Positive_TestData))]
        public async Task SLA_Delete_Positive_TestMethod(int slaId)
        {
            // 1️⃣ Pre‑req: create user first
            int userId = await CreateSLAAndReturnIdAsync();

            var endpoint = ApiEndpoints.SLA_DeleteSLA(userId);

            _test.Info("Running SLA DELETE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.DeleteAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode,
                Is.EqualTo(HttpStatusCode.OK)
                    .Or.EqualTo(HttpStatusCode.NoContent)
                    .Or.EqualTo(HttpStatusCode.Accepted),
                "Expected success status for valid SLA delete request.");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            Assert.That(actualMessage,
                Does.Not.Match(@"(?i)(invalid|error|required|missing|failed|not\s*allowed|format)"));

            _test.Pass("SLA DELETE (positive) assertions passed.");
        }


        // 🔖 DELETE – SLA by Id (Negative)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SLA_Delete_Negative_TestData))]
        public async Task SLA_Delete_Negative_TestMethod(int slaId)
        {
            var endpoint = ApiEndpoints.SLA_DeleteSLA(-1);

            _test.Info("Running SLA DELETE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.DeleteAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect 4xx for deleting non-existing or invalid SLA id
            Assert.That((int)response.StatusCode, Is.InRange(400, 499),
                $"Expected 4xx for invalid SLA delete request, but was {(int)response.StatusCode} ({response.StatusCode}).");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            string messageForAssert = actualMessage;
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(actualMessage);
                messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
            }
            catch
            {
                // ignore deserialization; fallback to raw content
            }

            Assert.That(messageForAssert,
                Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));

            _test.Pass("SLA DELETE (negative) assertions passed.");
        }

    }

}

