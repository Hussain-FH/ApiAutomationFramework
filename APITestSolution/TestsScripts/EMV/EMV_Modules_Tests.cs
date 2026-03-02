using ApiAutomationFramework.Models.Request;
using ApiAutomationFramework.Models.Request.EMV;
using ApiAutomationFramework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using APITestSolution.DataProviders;

namespace APITestSolution.TestsScripts.EMV
{
    [TestFixture]
    public class EMV_Modules_Tests : BaseTest
    {
        // =====================================================
        // Common pre-requisite: Create EMV Module and return Id
        // =====================================================
        private async Task<int> CreateEmvModuleAndReturnIdAsync()
        {
            var endpoint = ApiEndpoints.EMVModules_create;

            var payload = new EMV_Module_Create_Request
            {
                // basic valid values; other strings will be auto-filled by Prepare in real tests
                name = "Auto_EMVModule_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                description = "Auto_Description",
                travelerLabel = "Auto_TL",
                cmiProgram = "Auto_CMIP",
                groupId = new Random().Next(1, 999),
            };

            _test.Info("Pre-requisite: Creating EMV Module for UPDATE/GET/DELETE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Pre-Req Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Pre-Req Response Status: {response.StatusCode}");
            _test.Info($"Pre-Req Response Body: {response.Content}");

            // Expect 201 Created
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            // Assuming API returns something like: "EMV Module 123 Added Successfully."
            var idString = new string(actualMessage.Where(char.IsDigit).ToArray());

            Assert.That(idString, Is.Not.Empty,
                "Pre-requisite CREATE did not return a valid numeric EMV Module Id in the message.");

            int moduleId = int.Parse(idString);

            _test.Info($"Pre-requisite: EMV Module created successfully with Id = {moduleId}");

            return moduleId;
        }

        // =====================================================
        // POST – Create (Positive)
        // =====================================================

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.EMV_Modules_Create_Positive_TestData))]
        public async Task EMVModules_Creation_PositiveTestMethod(EMV_Module_Create_Request payload)
        {
            var endpoint = ApiEndpoints.EMVModules_create;

            _test.Info("Running EMV Modules CREATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect 201 Created
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var moduleId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "EMV Module " + moduleId + " Added Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("EMV Modules CREATE (positive) assertions passed.");
        }

        // =====================================================
        // POST – Create (Negative)
        // =====================================================
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.EMV_Modules_Create_Negative_TestData))]
        public async Task EMVModules_Creation_NegativeTestMethod(EMV_Module_Create_Request payload)
        {
            var endpoint = ApiEndpoints.EMVModules_create;

            _test.Info("Running EMV Modules CREATE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect 400 BadRequest
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.BadRequest);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            string messageForAssert = actualMessage;
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(actualMessage);
                messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
            }
            catch
            {
                // if plain string, just use it
            }

            Assert.That(messageForAssert,
                Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));
            _test.Pass("EMV Modules CREATE (negative) assertions passed.");
        }

        // =====================================================
        // PUT – Update (Positive)
        // =====================================================

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.EMV_Modules_Update_Positive_TestData))]
        public async Task EMVModules_Update_PositiveTestMethod(EMV_Module_Update_Request payload)
        {
            // Pre-req: create a module
            int moduleId = await CreateEmvModuleAndReturnIdAsync();

            // Inject the valid id into the PUT payload
            payload.id = moduleId;

            var endpoint = ApiEndpoints.EMVModules_update;

            _test.Info("Running EMV Modules UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var modulesId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "EMV Module " + modulesId + " Updated Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("EMV Modules UPDATE (positive) assertions passed.");
        }

        // =====================================================
        // PUT – Update (Negative)
        // =====================================================

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.EMV_Modules_Update_Negative_TestData))]
        public async Task EMVModules_Update_NegativeTestMethod(EMV_Module_Update_Request payload)
        {
            // Pre-req: create a module
            int moduleId = await CreateEmvModuleAndReturnIdAsync();

            // Inject the valid id into the PUT payload
            payload.id = moduleId;

            var endpoint = ApiEndpoints.EMVModules_update;

            _test.Info("Running EMV Modules UPDATE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.BadRequest);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            string messageForAssert = actualMessage;
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(actualMessage);
                messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
            }
            catch
            {
                // ignore
            }

            Assert.That(messageForAssert,
                Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));
            _test.Pass("EMV Modules UPDATE (negative) assertions passed.");
        }

        // =====================================================
        // GET ALL – Positive
        // =====================================================
        [Test]
        public async Task EMVModules_GetAll_Positive_TestMethod()
        {
            // Pre-req: create at least one module
            int moduleId = await CreateEmvModuleAndReturnIdAsync();

            var endpoint = ApiEndpoints.EMVModules_GetAll;

            _test.Info("Running EMV Modules GET ALL POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0), "Expected non-empty EMV Modules list.");

            var arr = JArray.Parse(body);
            Assert.That(arr.Count, Is.GreaterThan(0), "Expected one or more EMV Modules.");

            var matchingItems = arr.Where(x =>
                (int?)x["id"] == moduleId
            ).ToList();

            Assert.That(matchingItems.Count, Is.GreaterThan(0),
                $"Expected to find EMV Module with id = {moduleId} in GET ALL response, but it was not found.");

            _test.Pass("EMV Modules GET ALL (positive) assertions passed with created module present in the list.");
        }

    }
}