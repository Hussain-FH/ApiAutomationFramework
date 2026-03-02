using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request.EMV;
using ApiAutomationFramework;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiAutomationFramework.Models.Request.IssuingbanksArchive;

namespace APITestSolution.TestsScripts.IssuingBankArchive
{
    public class ArchiveTests :BaseTest
    {

        [TestCaseSource(typeof(UserDataProvider),nameof(UserDataProvider.IssuingBankArchive_Update_Positive_TestData))]
        public async Task IssuingBankArchive_Update_PositiveTestMethod(ArchiveUpdateRequest payload)
        {
           
            var endpoint = ApiEndpoints.BankArchive_update;

            _test.Info("Running IssuingBankArchive UPDATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var CardProfileId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "Issuing Bank " + CardProfileId + " Archived Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Issuing Bank Update (positive) assertions passed.");


        }






    }
}
