using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework;
using ApiAutomationFramework.Models.Request.Approval;
using ApiAutomationFramework.Models.Request.BulkPrgUpload;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace APITestSolution.TestsScripts.BulkProgramUpload
{
    internal class BulkUploadTests : BaseTest
    {
        // 🔖 GET – BulkUpload by Id (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.BulkUpload_Get_Positive_TestData))]
        public async Task BulkUploadTemplate_Get_Positive_TestMethod(int slaId)
        {
           
            var endpoint = ApiEndpoints.bulkuploadtemplate_Get;

            _test.Info("Running BulkUpload BY ID POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0), "Expected non-empty response body.");

            _test.Pass("SLA GET BY ID (positive) assertions passed.");

        }


        // 🔖 GET – BulkUpload by Id (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.BulkUploadbyPCL_Get_Positive_TestData))]
        public async Task BulkUpload_Get_Positive_TestMethod(int slaId)
        {

            var endpoint = ApiEndpoints.BulkUpload_Get;

            _test.Info("Running BulkUploadbyPCL BY ID POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0), "Expected non-empty response body.");

            _test.Pass("BulkUploadbyPCL ID (positive) assertions passed.");

        }

        //Post

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.BulkPrgCancel_Update_Positive_TestData))]
        public async Task Approval_Create_PositiveTestMethod(BulkPrgCancelUpdateReq payload)
        {
            var endpoint = ApiEndpoints.BulkUploadcancel_Update;

            _test.Info("BulkPrg Cancel Update  POSITIVE test...");
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
                "Programs Creation from File " + CardProfileId + " Cancelled Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Bulk PrgCancel Update (positive) assertions passed.");
        }
















    }
}
