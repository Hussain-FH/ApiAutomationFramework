using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request.SLA;
using ApiAutomationFramework;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiAutomationFramework.Models.Request.GeneralsetContactinfo;

namespace APITestSolution.TestsScripts.GeneralsettingsContactinfo
{
    public class GeneralsetContactTests :BaseTest
    {

        // 🔖 POST – GeneralsetContactinfo Create (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.GeneralsetContactinfo_Create_Positive_TestData))]
        public async Task GeneralsetContactinfo_Creation_PositiveTestMethod(ContactinfoCreateRequest payload)
        {
            var endpoint = ApiEndpoints.Generalsettings_Create;

            _test.Info("Running GeneralsetContactinfo Create POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Adjust expected status code as per your API (Created / OK)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            // Optional: add assertions based on your SLA create response contract
            _test.Pass("Positive GeneralsetContactinfo assertions passed");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            // Extract the word after "for " and before " Added"
            string userId = null;

            var prefix = "Contact Information for ";
            var suffix = " Added Successfully.";

            if (actualMessage.StartsWith(prefix) && actualMessage.EndsWith(suffix))
            {
                userId = actualMessage
                            .Replace(prefix, "")
                            .Replace(suffix, "")
                            .Trim();
            }

            var expectedMessage = $"{prefix}{userId}{suffix}";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }


        // 🔖 PUT – GeneralsetContactinfo Create (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.GeneralsetContactinfo_Update_Positive_TestData))]
        public async Task GeneralsetContactinfo_Update_PositiveTestMethod(ContactinfoUpdateRequest payload)
        {
            var endpoint = ApiEndpoints.Generalsettings_Update;

            _test.Info("Running GeneralsetContactinfo update POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Adjust expected status code as per your API (Created / OK)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            // Optional: add assertions based on your SLA create response contract
            _test.Pass("Positive GeneralsetContactinfo assertions passed");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            // Extract the word after "for " and before " Added"
            string userId = null;

            var prefix = "Contact Information for ";
            var suffix = " Updated Successfully.";

            if (actualMessage.StartsWith(prefix) && actualMessage.EndsWith(suffix))
            {
                userId = actualMessage
                            .Replace(prefix, "")
                            .Replace(suffix, "")
                            .Trim();
            }

            var expectedMessage = $"{prefix}{userId}{suffix}";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }










    }
}
