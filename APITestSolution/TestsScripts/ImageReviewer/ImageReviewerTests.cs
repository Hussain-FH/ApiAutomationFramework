using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework;
using ApiAutomationFramework.Models.Request.ImageReviewers;
using ApiAutomationFramework.Models.Request.SOPConfig;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace APITestSolution.TestsScripts.ImageReviewer
{
    public class ImageReviewerTests : BaseTest
    {

        //ImageReviewer Get
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.ImageReviewer_Get_Positive_TestData))]
        public async Task ImageReviewer_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.ImageReviewer_Get;

            _test.Info("ImageReviewer GET POSITIVE test...");
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

            _test.Pass("ImageReviewer (positive) passed.");
        }


        //ImageReviewer Post
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.ImageReviewer_Update_Positive_TestData))]
        public async Task ImageReviewer_Update_PositiveTestMethod(ImageRevUpdateRequest payload)
        {
            var endpoint = ApiEndpoints.ImageReviewer_update;

            _test.Info("Running Users UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage = "Image "+userId+ " Rejected Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));

            _test.Pass("ImageReviewer UPDATE (positive) assertions passed.");
        }



    }
}
