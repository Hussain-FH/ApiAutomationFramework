using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request;
using ApiAutomationFramework;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiAutomationFramework.Models.Request.Approval;
using ApiAutomationFramework.Models.Request.EMV;

namespace APITestSolution.TestsScripts.Approval
{
    public class ApprovalTests :BaseTest
    {

        // 🔖 POST – Approval (Positive)
        [TestCaseSource(typeof(UserDataProvider),nameof(UserDataProvider.Approval_Create_Positive_TestData))]
        public async Task Approval_Create_PositiveTestMethod(ApprovalCreateRequest payload)
        {
            var endpoint = ApiEndpoints.Approval_Create;

            _test.Info("Approval CREATE POSITIVE test...");
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
                "Job No. " + CardProfileId + " Created Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("EMV Card Profile CREATE (positive) assertions passed.");
        }

        // 🔖 Approval PUT – Update (Positive) 
        [TestCaseSource(typeof(UserDataProvider),nameof(UserDataProvider.Approval_Update_Positive_TestData))]
        public async Task EMVCardProfile_Update_PositiveTestMethod(ApprovalUpdateRequest payload)
        {
            
            var endpoint = ApiEndpoints.Approval_Update;

            _test.Info("Running approval UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var CardProfileId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "Approval " + CardProfileId + " Updated Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Approval Update (positive) assertions passed.");


        }


        // 🔖 POST – Approval (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.ApprovalDuplicate_Create_Positive_TestData))]
        public async Task ApprovalDuplicate_Create_PositiveTestMethod()
        {
            var endpoint = ApiEndpoints.ApprovalDuplicate_Create;

            _test.Info("Running SLA GET BY ID POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.PostAsync(endpoint);


            // Expect 201 Created
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var CardProfileId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage ="Approval " + CardProfileId + " Added Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Approval Duplicate CREATE (positive) assertions passed.");
        }


    }
}
