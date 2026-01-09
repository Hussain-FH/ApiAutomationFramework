
using ApiAutomationFramework;
using ApiAutomationFramework.Models.Request.Users;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APITestSolution.TestsScripts.Users
{
    [TestFixture]
    public class UserTests : BaseTest
    {
        // ✅ POST - Create User (Positive datasets only: SetName starts with "Positive_")
        [Test, TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_Create_TestData))]
        public async Task Users_Creation_WithPositiveData(UserscreateRequest payload)
        {
            var caseName = TestContext.CurrentContext.Test.Name;
            if (!caseName.StartsWith("Positive_", StringComparison.OrdinalIgnoreCase))
                Assert.Ignore($"Skipping non-positive dataset '{caseName}' in Positive test.");

            _test.Info("Running user Create POSITIVE test...");
            _test.Info($"Dataset: {caseName}");
            _test.Info($"Endpoint: {ApiEndpoints.Users_createuser}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(ApiEndpoints.Users_createuser, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect 201 Created
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            // Normalize (API sometimes returns quoted string)
            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            // Extract dynamic id and assert exact toast
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "User " + userId + " created successfully. An email has been sent to the registered email address to configure the credentials.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage),
                "Success message should match exactly for positive dataset.");

            _test.Pass("Positive CreateUser assertions passed");
        }

        // ✅ POST - Create User (Negative datasets only: SetName starts with "Negative_")
        [Test, TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Users_Create_TestData))]
        public async Task Users_Creation_WithNegativeData(UserscreateRequest payload)
        {
            var caseName = TestContext.CurrentContext.Test.Name;
            if (!caseName.StartsWith("Negative_", StringComparison.OrdinalIgnoreCase))
                Assert.Ignore($"Skipping non-negative dataset '{caseName}' in Negative test.");

            _test.Info("Running user Create NEGATIVE test...");
            _test.Info($"Dataset: {caseName}");
            _test.Info($"Endpoint: {ApiEndpoints.Users_createuser}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(ApiEndpoints.Users_createuser, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect 4xx for invalid payload
            Assert.That((int)response.StatusCode, Is.InRange(400, 499),
                $"Expected 4xx for invalid payload in '{caseName}', but was {(int)response.StatusCode} ({response.StatusCode}).");

            // Normalize message (strip surrounding quotes)
            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            // Must NOT be the success toast
            Assert.That(actualMessage, Does.Not.Match(@"^User\s+\d+\s+created successfully"),
                "Negative case should not return success toast.");

            // Try to extract a meaningful text if server returns ProblemDetails JSON
            string messageForAssert = actualMessage;
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(actualMessage);
                messageForAssert = (string)(obj?.title ?? obj?.message ?? actualMessage);
            }
            catch { /* plain string, keep as-is */ }

            // Should contain a validation keyword (tolerant assertion)
            Assert.That(messageForAssert, Does.Match(@"(?i)(required|invalid|error|missing|failed|not\s*allowed|format)"),
                $"Negative case '{caseName}' should contain a validation/error keyword. Actual: {messageForAssert}");

            _test.Pass("Negative CreateUser assertions passed");
        }
    }
}
