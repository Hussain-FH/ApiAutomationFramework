using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request.ClientSettingsHotStamps;
using ApiAutomationFramework;
using NUnit.Framework;
using APITestSolution.DataProviders;
using Newtonsoft.Json.Linq;

namespace APITestSolution.TestsScripts.CSHotStampsTests
{
    public class ClientSetHot_StampsTests : BaseTest
    {

        private async Task<int> CreateHotStampAndReturnIdAsync()
        {
            var endpoint = ApiEndpoints.CSHotStamps_create;

            var payload = new CSHotStampsCreateR
            {
                pclId = 13,
                name = "Gold Stamp",
                description = "Auto-created for tests",
                userName = "USNAME"
            };

            var response = await _apiClient.PostAsync(endpoint, payload);

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var message = (response.Content ?? "").Trim().Trim('"');
            var idString = new string(message.Where(char.IsDigit).ToArray());

            Assert.That(idString, Is.Not.Empty, "No valid Hot Stamp ID returned.");

            return int.Parse(idString);
        }

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSHotStamp_Create_Positive_TestData))]
        public async Task CSHotStamp_Create_Positive_Test(CSHotStampsCreateR payload)
        {
            var endpoint = ApiEndpoints.CSHotStamps_create;

            var response = await _apiClient.PostAsync(endpoint, payload);

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            _test.Pass("Client Setting Hot Stamp Create (positive) assertions passed.");
        }


        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSHotStamp_Create_Negative_TestData))]
        public async Task CSHotStamp_Create_Negative_Test(CSHotStampsCreateR payload)
        {
            var endpoint = ApiEndpoints.CSHotStamps_create;

            var response = await _apiClient.PostAsync(endpoint, payload);

            Assert.That((int)response.StatusCode, Is.InRange(400, 499));

            _test.Pass("Client Setting Hot Stamp Create (negative) assertions passed.");
        }

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSHotStamp_Update_Positive_TestData))]
        public async Task CSHotStamp_Update_Positive_Test(CSHotStampsUpdateR payload)
        {
            int newId = await CreateHotStampAndReturnIdAsync();

            payload.id = newId;

            var endpoint = ApiEndpoints.CSHotStamps_update;

            var response = await _apiClient.PutAsync(endpoint, payload);

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            _test.Pass("Client Setting Hot Stamp Update (positive) passed.");
        }



        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSHotStamp_Update_Negative_TestData))]
        public async Task CSHotStamp_Update_Negative_Test(CSHotStampsUpdateR payload)
        {
            var endpoint = ApiEndpoints.CSHotStamps_update;

            var response = await _apiClient.PutAsync(endpoint, payload);

            Assert.That((int)response.StatusCode, Is.InRange(400, 499));

            _test.Pass("Client Setting Hot Stamp Update (negative) passed.");
        }



        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSHotStamp_Get_Positive_TestData))]
        public async Task CSHotStamp_Get_Positive_Test(int id)
        {
            // 1️⃣ Pre‑req: create user first
            int userId = await CreateHotStampAndReturnIdAsync();

            var endpoint = $"{ApiEndpoints.CSHotStamps_Get}";

            _test.Info("Running Users GET BY ID POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0));

            var arr = JArray.Parse(body);

            // 🔍 Extract all ids from the array
            var allIds = arr.Select(x => (int?)x["id"]).Where(id => id.HasValue).Select(id => id.Value).ToList();

            Assert.That(allIds.Count, Is.GreaterThan(0), "Expected at least one SLA in the response array.");

            _test.Pass("SLA GET BY ID (positive) assertions passed.");
            // ✅ Check whether the newly created userId exists anywhere in the returned array
            Assert.That(allIds.Contains(userId),
                $"Expected Hot Stamp Id {userId} to be present in response, but it was not found.");
        }


        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSHotStamp_Get_Negative_TestData))]
        public async Task CSHotStamp_Get_Negative_Test(int id)
        {
            var endpoint = $"{ApiEndpoints.CSHotStamps_Get}?Id={id}";

            var response = await _apiClient.GetAsync(endpoint);

            Assert.That((int)response.StatusCode, Is.GreaterThanOrEqualTo(400));

            _test.Pass("Client Setting Hot Stamp GET (negative) passed.");
        }


    }
}
