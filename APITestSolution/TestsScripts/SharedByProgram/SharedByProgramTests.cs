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

namespace APITestSolution.TestsScripts.SharedByProgram
{
    public class SharedByProgramTests : BaseTest
    {

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Insert_SharedByProgram_Get_Positive_TestData))]
        public async Task InsertSharedByPrg_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.InsertSharedbyPrg_Get;

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

            _test.Pass("Insert shared by Program GET (positive) passed.");
        }


        //PackingSlip
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.PackingSlip_SharedByProgram_Get_Positive_TestData))]
        public async Task Packingslips_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.packingslipsSharedbyPrg_Get;

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

            _test.Pass("Packing slip shared by Program GET (positive) passed.");
        }

        // ActivationLabel

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.ActivationLabel_SharedByProgram_Get_Positive_TestData))]
        public async Task ActivationLabel_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.activationlabelsSharedbyPrg_Get;

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

            _test.Pass("Activation Label shared by Program GET (positive) passed.");
        }

        // ActivationLabel

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.LetterCarrier_SharedByProgram_Get_Positive_TestData))]
        public async Task LetterCarrier_Get_Positive_Test(int id)
        {
            var endpoint = ApiEndpoints.LetterCarrierSharedbyPrg_Get;

            _test.Info("Running LetterCarrier GET POSITIVE test...");
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

            _test.Pass("LetterCarrier GET (positive) passed.");
        }



















    }









}
