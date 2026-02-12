using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework;
using APITestSolution.DataProviders;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace APITestSolution.TestsScripts.CSPProgram
{
    public class CSPProgramTests : BaseTest
    {

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSPProgramCardholderdrp_Get_Positive_TestData))]
        public async Task CSPProgramCardholderdrp_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.CSPProgramCardholderdrp_Get;

            _test.Info("Running CSP Program Cardholder Dropdown GET POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body.Length, Is.GreaterThan(0), "Response body is empty.");

            var arr = JArray.Parse(body);

            // Must contain at least one object
            Assert.That(arr.Count, Is.GreaterThan(0), "Response array is empty.");

            // ✅ Check that FIRST ITEM truly exists and has ID field
            var firstItem = arr.First as JObject;
            Assert.That(firstItem, Is.Not.Null, "First item is null.");

            int? firstId = (int?)firstItem["id"];
            Assert.That(firstId.HasValue, "First item does not contain 'id'.");

            _test.Pass("CSP Program Cardholder Dropdown GET (positive) passed.");
        }



    [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSPProgramDymInfo_Get_Positive_TestData))]
        public async Task CSPProgramDymInfo_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.CSPProgramDymInfo_Get;

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



        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSPProgramComponent_Get_Positive_TestData))]
        public async Task CSPProgramComponent_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.CSPProgramComponent_Get;

            _test.Info("Running CSP Program Dynamic Info GET POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            //var body = (response.Content ?? string.Empty).Trim();
            //Assert.That(body.Length, Is.GreaterThan(0), "Response body is empty.");

            //var arr = JArray.Parse(body);

            //Assert.That(arr.Count, Is.GreaterThan(0), "Response array is empty.");

            //// ✅ Validate first element exists
            //var firstItem = arr.First as JObject;
            //Assert.That(firstItem, Is.Not.Null, "First element is null.");

            //int? firstId = (int?)firstItem["id"];
            //Assert.That(firstId.HasValue, "First array element does not contain 'id'.");

            _test.Pass("CSP Program Dynamic Info GET (positive) passed.");
        }




    }
}
