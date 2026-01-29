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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;

namespace APITestSolution.TestsScripts.EMV
{
    [TestFixture]
    public class EMVCardProfileTests : BaseTest
    {
        //common method to create new CardProfile record and used this Id and perform Put and Get and Delete operations
        private async Task<int> CreateEmvCardProfileAndReturnIdAsync()
        {
            var endpoint = ApiEndpoints.EMVCardProfiles_create;

            // Build a positive payload 
            var payload = new EMVCardProfilesCreateRequests
            {
                name = "Auto_EMVProfile_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                issuerId = 8440,
                expirationDate =DateTime.UtcNow.AddDays(new Random().Next(30, 365)).ToString("yyyy-MM-dd")//random valid dates 

            };

            _test.Info("Pre-requisite: Creating EMV Card Profile for UPDATE/GET test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Pre-Req Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Pre-Req Response Status: {response.StatusCode}");
            _test.Info($"Pre-Req Response Body: {response.Content}");

            // Expect 201 Created (same as your positive create test)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            // Your API returns something like: "EMV Card Profile 1234 Added Successfully."
            var idString = new string(actualMessage.Where(char.IsDigit).ToArray());

            Assert.That(idString, Is.Not.Empty,
                "Pre-requisite CREATE did not return a valid numeric EMV Card Profile Id in the message.");

            int cardProfileId = int.Parse(idString);

            _test.Info($"Pre-requisite: EMV Card Profile created successfully with Id = {cardProfileId}");

            return cardProfileId;
        }

        // 🔖 POST – Create (Positive)
        [TestCaseSource(
            typeof(UserDataProvider),
            nameof(UserDataProvider.EMV_CardProfile_Create_Positive_TestData))]
        public async Task EMVCardProfile_Creation_PositiveTestMethod(EMVCardProfilesCreateRequests payload)
        {
            var endpoint = ApiEndpoints.EMVCardProfiles_create;

            _test.Info("Running EMV Card Profile CREATE POSITIVE test...");
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
                "EMV Card Profile " + CardProfileId + " Added Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("EMV Card Profile CREATE (positive) assertions passed.");
        }

        // 🔖 POST – Create (Negative)
        [TestCaseSource(
            typeof(UserDataProvider),
            nameof(UserDataProvider.EMV_CardProfile_Create_Negative_TestData))]
        public async Task EMVCardProfile_Creation_NegativeTestMethod(EMVCardProfilesCreateRequests payload)
        {
            var endpoint = ApiEndpoints.EMVCardProfiles_create;

            _test.Info("Running EMV Card Profile CREATE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);


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
                // if plain string, just use it
            }

            Assert.That(messageForAssert,
                Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));
            _test.Pass("EMV Card Profile CREATE (negative) assertions passed.");
        }

        // 🔖 PUT – Update (Positive)  ✅ UPDATED TO USE COMMON PRE-REQ
        [TestCaseSource(
            typeof(UserDataProvider),
            nameof(UserDataProvider.EMV_CardProfile_Update_Positive_TestData))]
        public async Task EMVCardProfile_Update_PositiveTestMethod(EMVCardProfilePutRequest payload)
        {
            // 1️⃣ Pre-requisite: Create a valid EMV Card Profile and get its Id
            int cardProfileId = await CreateEmvCardProfileAndReturnIdAsync();

            // 2️⃣ Inject the valid id into the PUT payload
            payload.cardprofileid = cardProfileId;

            // Optional: If you want to always ensure name is marked as updated:
            if (!string.IsNullOrWhiteSpace(payload.name))
            {
                payload.name = "AutoUpdate_" + payload.name;
            }

            var endpoint = ApiEndpoints.EMVCardProfiles_update;

            _test.Info("Running EMV Card Profile UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var CardProfileId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "EMV Card Profile " + CardProfileId + " Updated Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("EMV Card Profile Update (positive) assertions passed.");


        }

        // 🔖 PUT – Update (Negative)  (UNCHANGED)
        [TestCaseSource(
            typeof(UserDataProvider),
            nameof(UserDataProvider.EMV_CardProfile_Update_Negative_TestData))]
        public async Task EMVCardProfile_Update_NegativeTestMethod(EMVCardProfilePutRequest payload)
        {
            // 1️⃣ Pre-requisite: Create a valid EMV Card Profile and get its Id
            int cardProfileId = await CreateEmvCardProfileAndReturnIdAsync();

            // 2️⃣ Inject the valid id into the PUT payload
            payload.cardprofileid = cardProfileId;

            var endpoint = ApiEndpoints.EMVCardProfiles_update;

            _test.Info("Running EMV Card Profile UPDATE NEGATIVE test...");
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
            _test.Pass("EMV Card Profile UPDATE (negative) assertions passed.");
        }

        [Test]
        public async Task EMVCardProfile_GetAll_Positive_TestMethod()
        {
            // 1️⃣ Pre-requisite: Create a valid EMV Card Profile and get its Id
            int cardProfileId = await CreateEmvCardProfileAndReturnIdAsync();

            // 2️⃣ Call the GET ALL endpoint (no id in URL)
            var endpoint = ApiEndpoints.EMVCardProfiles_Get; // maps to /cardprofiles

            _test.Info("Running EMV Card Profile GET ALL POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // ✅ Expect 200 OK
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0), "Expected non-empty EMV Card Profile list.");

            // ✅ Response is an array
            var arr = JArray.Parse(body);
            Assert.That(arr.Count, Is.GreaterThan(0), "Expected one or more EMV Card Profiles.");

            // 3️⃣ Check that the created profile exists in the list
            var matchingItems = arr.Where(x =>
                (int?)x["id"] == cardProfileId
            ).ToList();

            Assert.That(matchingItems.Count, Is.GreaterThan(0),
                $"Expected to find EMV Card Profile with id = {cardProfileId} in GET ALL response, but it was not found.");

            // Optionally, add a few more checks on the first matching item
            var createdProfile = matchingItems.First();

            // issuerId validation if needed
            var issuerIdToken = createdProfile["issuerId"];
            if (issuerIdToken != null && issuerIdToken.Type != JTokenType.Null)
            {
                int issuerId = issuerIdToken.ToObject<int>();
                Assert.That(issuerId, Is.GreaterThan(0), "issuerId should be a positive integer.");
            }

            // name not empty
            var name = (createdProfile["name"]?.ToString() ?? string.Empty).Trim();
            Assert.That(name.Length, Is.GreaterThan(0), "Name should not be empty for created EMV Card Profile.");

            _test.Pass("EMV Card Profile GET ALL (positive) assertions passed with created profile present in the list.");
        }
    
    }
}