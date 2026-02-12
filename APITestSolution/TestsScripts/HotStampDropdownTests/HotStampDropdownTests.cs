using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request.ClientSettingsHotStamps;
using ApiAutomationFramework;
using NUnit.Framework;
using APITestSolution.DataProviders;
using Newtonsoft.Json.Linq;

namespace APITestSolution.TestsScripts.HotStampDropdownTests
{
    public class HotStampDropdownTests:BaseTest
    {
        // 🔖 GET – HotStampDie (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.HotStampdrp_Get_Positive_TestData))]
        public async Task HotStampDie_Get_Positive_Test()
        {
            var endpoint = ApiEndpoints.HotStampDieDrp_Get;

            _test.Info("Running ClientProfile GET MASTERDATA POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body, Is.Not.Empty);

            var arr = JArray.Parse(body);

            // 🔍 Extract all ids from the array
            var allIds = arr.Select(x => (int?)x["id"]).Where(id => id.HasValue).Select(id => id.Value).ToList();

            Assert.That(allIds.Count, Is.GreaterThan(0), "Expected at least one SLA in the response array.");

             
        }



        // 🔖 GET – Tipping Module DropDown (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.TippingModuleDrp_Get_Positive_TestData))]
        public async Task TippingModuleDrp_Get_Positive_Test()
        {
            var endpoint = ApiEndpoints.TippingModulDrp_Get;

            _test.Info("Running ClientProfile GET MASTERDATA POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.GetAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var body = (response.Content ?? string.Empty).Trim();
            Assert.That(body, Is.Not.Empty);

            var arr = JArray.Parse(body);

            // 🔍 Extract all ids from the array
            var allIds = arr.Select(x => (int?)x["id"]).Where(id => id.HasValue).Select(id => id.Value).ToList();

            Assert.That(allIds.Count, Is.GreaterThan(0), "Expected at least one SLA in the response array.");


        }

    }
 }
