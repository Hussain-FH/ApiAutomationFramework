using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request.ClientSettiClientprofiles;
using ApiAutomationFramework;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using APITestSolution.DataProviders;

namespace APITestSolution.TestsScripts.CSClientProfileTests
{
    public class CSClientProfileTests : BaseTest
    {

   
            // =====================================================
            // Helper: Get a valid keyId from GET masterdata
            // Robust: scans JSON for the first keyId token
            // =====================================================
            private async Task<int> GetValidKeyIdFromMasterDataAsync()
            {
                var endpoint = ApiEndpoints.CSClientProfile_Get;

                _test.Info("Pre-Req : Fetching ClientProfile MasterData to get valid keyId..");
                _test.Info($"Endpoint: {endpoint}");

                var response = await _apiClient.GetAsync(endpoint);

                _test.Info($"Pre-Req Response Status: {response.StatusCode}");
                _test.Info($"Pre-Req Response Body: {response.Content}");

                ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

                var body = (response.Content ?? string.Empty).Trim();
                Assert.That(body, Is.Not.Empty, "MasterData response is empty.");

                JToken token;
                try
                {
                    token = JToken.Parse(body);
                }
                catch
                {
                    Assert.Fail("MasterData response is not valid JSON.");
                    return 0;
                }

                // Find first "keyId" anywhere in the JSON
                var keyIdToken = token.SelectTokens("$..keyId").FirstOrDefault();
                Assert.That(keyIdToken, Is.Not.Null, "Could not find keyId in MasterData response.");

                int keyId = keyIdToken!.Value<int>();
                Assert.That(keyId, Is.GreaterThan(0), "Extracted keyId is not valid.");

                _test.Info($"Pre-requisite: keyId fetched successfully = {keyId}");

                return keyId;
            }

            // =====================================================
            // Helper: Create ClientProfile and return context (pclId, keyId, userName)
            // =====================================================
            private async Task<(int pclId, int keyId, string userName)> CreateClientProfileAndReturnContextAsync()
            {
                var endpoint = ApiEndpoints.CSClientProfile_create;

                int pclId = 13;
                int keyId = await GetValidKeyIdFromMasterDataAsync();
                string userName = "AutoUser_CP_" + Guid.NewGuid().ToString("N").Substring(0, 6);

                var payload = new ClientprofilesCreateRequest
                {
                    pclId = "pclId",
                    keyId = keyId,
                    userName = userName,
                    value = "AutoValue_CP_" + Guid.NewGuid().ToString("N").Substring(0, 6)
                };

                _test.Info("Pre-Req : Creating ClientProfile to use in Update/Delete..");
                _test.Info($"Endpoint: {endpoint}");
                _test.Info($"Pre-Req Request Payload: {JsonConvert.SerializeObject(payload)}");

                var response = await _apiClient.PostAsync(endpoint, payload);

                _test.Info($"Pre-Req Response Status: {response.StatusCode}");
                _test.Info($"Pre-Req Response Body: {response.Content}");

                // Most APIs return 201 for create; change to OK if your API returns 200
                ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

                var msg = (response.Content ?? string.Empty).Trim().Trim('"');
                Assert.That(msg, Is.Not.Empty, "Create ClientProfile returned empty response.");

                _test.Info($"Pre-requisite: ClientProfile created successfully with pclId={pclId}, keyId={keyId}, userName={userName}");

                return (pclId, keyId, userName);
            }

            // =====================================================
            // POST – Create (Positive)
            // =====================================================
            [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSClientProfile_Create_Positive_TestData))]
            public async Task CSClientProfile_Create_PositiveTestMethod(ClientprofilesCreateRequest payload)
            {
                var endpoint = ApiEndpoints.CSClientProfile_create;

                _test.Info("Running ClientProfile CREATE POSITIVE test...");
                _test.Info($"Endpoint: {endpoint}");
                _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

                var response = await _apiClient.PostAsync(endpoint, payload);

                _test.Info($"Response Status: {response.StatusCode}");
                _test.Info($"Response Body: {response.Content}");

                ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

                var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
                Assert.That(actualMessage, Is.Not.Empty);

                // success keyword check (stable across environments)
                //Assert.That(actualMessage, Does.Match(@"(?i)(success|created|added|saved)"));

                _test.Pass("ClientProfile CREATE (positive) assertions passed.");
            }

        // =====================================================
        // POST – Create (Negative)
        // =====================================================
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSClientProfile_Create_Negative_TestData))]
        public async Task CSClientProfile_Create_NegativeTestMethod(ClientprofilesCreateRequest payload)
        {
            var endpoint = ApiEndpoints.CSClientProfile_create;

            _test.Info("Running ClientProfile CREATE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");


            //var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

        }

            // =====================================================
            // PUT – Update (Positive)
            // =====================================================
            [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSClientProfile_Update_Positive_TestData))]
            public async Task CSClientProfile_Update_PositiveTestMethod(ClientprofilesUpdateRequest payload)
            {

            var endpoint = ApiEndpoints.CSClientProfile_update;

                _test.Info("Running ClientProfile UPDATE POSITIVE test...");
                _test.Info($"Endpoint: {endpoint}");
                _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

                var response = await _apiClient.PutAsync(endpoint, payload);

                _test.Info($"Response Status: {response.StatusCode}");
                _test.Info($"Response Body: {response.Content}");

                ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

                var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
                Assert.That(actualMessage, Is.Not.Empty);
                Assert.That(actualMessage, Does.Match(@"(?i)(success|updated|modified|saved)"));

                _test.Pass("ClientProfile UPDATE (positive) assertions passed.");
            }

            

            // =====================================================
            // GET – MasterData (Positive)
            // =====================================================
            [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSClientProfile_Get_Positive_TestData))]
            public async Task CSClientProfile_GetMasterData_PositiveTestMethod()
            {
                var endpoint = ApiEndpoints.CSClientProfile_Get;

                _test.Info("Running ClientProfile GET MASTERDATA POSITIVE test...");
                _test.Info($"Endpoint: {endpoint}");

                var response = await _apiClient.GetAsync(endpoint);

                _test.Info($"Response Status: {response.StatusCode}");
                _test.Info($"Response Body: {response.Content}");

                ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

                var body = (response.Content ?? string.Empty).Trim();
                Assert.That(body, Is.Not.Empty);

                // Ensure at least one keyId exists
                var token = JToken.Parse(body);
                var keyIdToken = token.SelectTokens("$..keyId").FirstOrDefault();
                Assert.That(keyIdToken, Is.Not.Null);

                _test.Pass("ClientProfile GET MASTERDATA (positive) assertions passed.");
            }

            // =====================================================
            // DELETE – ClientProfile (Positive)
            // =====================================================
            [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSClientProfile_Delete_Positive_TestData))]
            public async Task CSClientProfile_Delete_PositiveTestMethod(ClientprofilesDeleteRequest payload)
            {
                

                var endpoint = ApiEndpoints.CSClientProfile_Delete;

                _test.Info("Running ClientProfile DELETE POSITIVE test...");
                _test.Info($"Endpoint: {endpoint}");
                _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

                var response = await _apiClient.DeleteAsync(endpoint, payload);

                _test.Info($"Response Status: {response.StatusCode}");
                _test.Info($"Response Body: {response.Content}");

                // Delete often returns OK or NoContent; adjust if needed
                ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

                var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
                Assert.That(actualMessage, Is.Not.Empty);
                Assert.That(actualMessage, Does.Match(@"(?i)(success|deleted|removed)"));

                _test.Pass("ClientProfile DELETE (positive) assertions passed.");
            }

        // =====================================================
        // DELETE – ClientProfile (Negative)
        // =====================================================
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSClientProfile_Delete_Negative_TestData))]
        public async Task CSClientProfile_Delete_NegativeTestMethod(ClientprofilesDeleteRequest payload)
        {
            var endpoint = ApiEndpoints.CSClientProfile_Delete;

            _test.Info("Running ClientProfile DELETE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.DeleteAsync(endpoint);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");
       
            //Assert.That(messageForAssert, Does.Match(@"(?i)(NotFound|required|invalid|error|missing|failed|not\s*allowed|format)"));
            //_test.Pass("ClientProfile DELETE (negative) assertions passed.");
        }
    }
}