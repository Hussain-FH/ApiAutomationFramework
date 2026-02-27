using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework;
using ApiAutomationFramework.Models.Request.SOPConfig;
using ApiAutomationFramework.Models.Request.Users;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace APITestSolution.TestsScripts.SOPConfig
{
    public class SOPConfigTests : BaseTest
    {
        //SOP Config Get
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SOPConfig_Get_Positive_TestData))]
        public async Task SOPConfig_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.SOPConfig_Get;

            _test.Info("Running CSP Program Dynamic Info GET POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0), "Response body is empty.");

            var arr = JArray.Parse(body);

            Assert.That(arr.Count, Is.GreaterThan(0), "Response array is empty.");

            // ✅ Validate first element exists
            var firstItem = arr.First as JObject;
            Assert.That(firstItem, Is.Not.Null, "First element is null.");

            int? firstId = (int?)firstItem["id"];
            Assert.That(firstId.HasValue, "First array element does not contain 'id'.");

            _test.Pass("CSP Program Dynamic Info GET (positive) passed.");
        }

        //SOPConfig Post
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.SOPConfig_Update_Positive_TestData))]
        public async Task SOPConfig_Update_PositiveTestMethod(SOPConfigUpdateRequest payload)
        {
            var endpoint = ApiEndpoints.SOPConfig_Update;

            _test.Info("Running Users UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage = "SOP Configuration Updated Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));

            _test.Pass("SOP Configuration UPDATE (positive) assertions passed.");
        }

    }
}
