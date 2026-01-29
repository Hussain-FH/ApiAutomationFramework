using ApiAutomationFramework.Models.Request;
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

namespace APITestSolution.TestsScripts.EMV
{
    [TestFixture]
    public class EMVCardProfileTests : BaseTest
    {
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

            Assert.That((int)response.StatusCode, Is.InRange(400, 499),
                $"Expected 4xx for invalid EMV Card Profile payload, but was {(int)response.StatusCode} ({response.StatusCode}).");

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

        //// 🔖 PUT – Update (Positive)
        //// (Assuming you have EMVCardProfilesUpdateRequest model and DataProvider)
        //[TestCaseSource(
        //    typeof(EMVCardProfileDataProvider),
        //    nameof(EMVCardProfileDataProvider.EMV_CardProfile_Update_Positive_TestData))]
        //public async Task EMVCardProfile_Update_PositiveTestMethod(EMVCardProfilesCreateRequests payload /* or EMVCardProfilesUpdateRequest */)
        //{
        //    var endpoint = ApiEndpoints.EMVCardProfile_Update;

        //    _test.Info("Running EMV Card Profile UPDATE POSITIVE test...");
        //    _test.Info($"Endpoint: {endpoint}");
        //    _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

        //    var response = await _apiClient.PutAsync(endpoint, payload);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    Assert.That(response.StatusCode,
        //        Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NoContent));

        //    var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
        //    Assert.That(actualMessage,
        //        Does.Not.Match(@"(?i)(invalid|error|required|missing|failed|not\s*allowed|format)"));

        //    _test.Pass("EMV Card Profile UPDATE (positive) assertions passed.");
        //}

        //// 🔖 PUT – Update (Negative)
        //[TestCaseSource(
        //    typeof(EMVCardProfileDataProvider),
        //    nameof(EMVCardProfileDataProvider.EMV_CardProfile_Update_Negative_TestData))]
        //public async Task EMVCardProfile_Update_NegativeTestMethod(EMVCardProfilesCreateRequests payload /* or EMVCardProfilesUpdateRequest */)
        //{
        //    var endpoint = ApiEndpoints.EMVCardProfile_Update;

        //    _test.Info("Running EMV Card Profile UPDATE NEGATIVE test...");
        //    _test.Info($"Endpoint: {endpoint}");
        //    _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

        //    var response = await _apiClient.PutAsync(endpoint, payload);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    Assert.That((int)response.StatusCode, Is.InRange(400, 499),
        //        $"Expected 4xx for invalid EMV Card Profile update payload, but was {(int)response.StatusCode} ({response.StatusCode}).");

        //    var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

        //    string messageForAssert = actualMessage;
        //    try
        //    {
        //        dynamic obj = JsonConvert.DeserializeObject(actualMessage);
        //        messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
        //    }
        //    catch
        //    {
        //        // ignore
        //    }

        //    Assert.That(messageForAssert,
        //        Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"));
        //    _test.Pass("EMV Card Profile UPDATE (negative) assertions passed.");
        //}

        //// 🔖 GET – List or Get by Issuer / Id (Positive)
        //[Test]
        //public async Task EMVCardProfile_GetAll_Positive_TestMethod()
        //{
        //    var endpoint = ApiEndpoints.EMVCardProfile_GetAll;

        //    _test.Info("Running EMV Card Profile GET ALL POSITIVE test...");
        //    _test.Info($"Endpoint: {endpoint}");

        //    var response = await _apiClient.GetAsync(endpoint);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        //    var body = (response.Content ?? string.Empty).Trim();
        //    Assert.That(body.Length, Is.GreaterThan(0), "Expected non-empty EMV Card Profile list.");

        //    var arr = JArray.Parse(body);
        //    Assert.That(arr.Count, Is.GreaterThan(0), "Expected one or more EMV Card Profiles.");

        //    _test.Pass("EMV Card Profile GET ALL (positive) assertions passed.");
        //}

        //// 🔖 DELETE – Delete (Positive)
        //// Here we assume you pass a valid profileId, maybe from DataProvider
        //[TestCase(1001)] // replace with valid id or use TestCaseSource
        //public async Task EMVCardProfile_Delete_Positive_TestMethod(int profileId)
        //{
        //    var endpoint = $"{ApiEndpoints.EMVCardProfile_Delete}/{profileId}";

        //    _test.Info("Running EMV Card Profile DELETE POSITIVE test...");
        //    _test.Info($"Endpoint: {endpoint}");

        //    var response = await _apiClient.DeleteAsync(endpoint);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    Assert.That(response.StatusCode,
        //        Is.EqualTo(HttpStatusCode.OK).Or.EqualTo(HttpStatusCode.NoContent));

        //    _test.Pass("EMV Card Profile DELETE (positive) assertions passed.");
        //}

        //// 🔖 DELETE – Delete (Negative: invalid id)
        //[TestCase(0)]
        //[TestCase(-1)]
        //public async Task EMVCardProfile_Delete_Negative_TestMethod(int profileId)
        //{
        //    var endpoint = $"{ApiEndpoints.EMVCardProfile_Delete}/{profileId}";

        //    _test.Info("Running EMV Card Profile DELETE NEGATIVE test...");
        //    _test.Info($"Endpoint: {endpoint}");

        //    var response = await _apiClient.DeleteAsync(endpoint);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    // Either 4xx or 404 for invalid id
        //    Assert.That((int)response.StatusCode, Is.InRange(400, 499),
        //        $"Expected 4xx for invalid EMV Card Profile Id, but was {(int)response.StatusCode} ({response.StatusCode}).");

        //    _test.Pass("EMV Card Profile DELETE (negative) assertions passed.");
        //}
    }
}
