using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request.BulkPrgUpload;
using ApiAutomationFramework;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiAutomationFramework.Models.Request.bcssconfigurations;
using ApiAutomationFramework.Models.Request.Approval;
using Newtonsoft.Json.Linq;

namespace APITestSolution.TestsScripts.bcssconfigurations
{
    public class bcssconfigurationsTests : BaseTest
    {

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.bcssconfigurations_Create_Positive_TestData))]
        public async Task bcssconf_Create_PositiveTestMethod(bcssconfCreateRequest payload)
        {
            var endpoint = ApiEndpoints.bcssconfigurations_Update;

            _test.Info("bcssconfigurations create POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect 201 Created
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var CardProfileId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "BCSS Configuration " + CardProfileId + " Added Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("bcssconf Create assertions passed.");
        }


        // 🔖 Approval PUT – Update (Positive) 
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.bcssconfigurations_Update_Positive_TestData))]
        public async Task bcssconfi_Update_PositiveTestMethod(bcssconfUpdateRequest payload)
        {

            var endpoint = ApiEndpoints.bcssconfigurations_Update;

            _test.Info("bcssconfUpdate UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var CardProfileId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "BCSS Configuration " + CardProfileId + " Updated Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("BCSS Configuration assertions passed.");


        }


        // 🔖 GET – SLA by Id (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.BankidNumber_Get_Positive_TestData))]
        public async Task BankidNumber_GetById_Positive_TestMethod(int slaId)
        {
            

            var endpoint = ApiEndpoints.bankidNumber_Get;

            _test.Info("BankidNumber BY ID POSITIVE test...");
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
















    }
}
