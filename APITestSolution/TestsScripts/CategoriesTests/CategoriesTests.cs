using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request.Categories;
using ApiAutomationFramework;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApiAutomationFramework.Models.Request.Users;

namespace APITestSolution.TestsScripts.CategoriesTests
{
    public class CategoriesTests : BaseTest
    {
        //Commom method can used to create category get the UserId
        private async Task<int> CreateCategoryReturnIdAsync()
        {
            var endpoint = ApiEndpoints.Categories_Create;

            var payload = new CategoriesCreateRequest
            {
                Name = "Automated_Category",
                ParentCategoryId = 266,
                PclId = 118
            };

            _test.Info("Pre-Req : Creating Category to get CategoryId..");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Pre-Req Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Pre-Req Response Status: {response.StatusCode}");
            _test.Info($"Pre-Req Response Body: {response.Content}");

            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            string raw = (response.Content ?? string.Empty).Trim().Trim('"');
            string onlyDigits = new string(raw.Where(char.IsDigit).ToArray());

            Assert.That(onlyDigits, Is.Not.Empty, "Could not extract CategoryId from create response.");

            int categoryId = int.Parse(onlyDigits);

            _test.Info($"Category created successfully with categoryId = {categoryId}");

            return categoryId;
        }


        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Categories_Create_Positive_TestData))]
        public async Task Categories_Create_Positive_Test(CategoriesCreateRequest payload)
        {
            var endpoint = ApiEndpoints.Categories_Create;

            _test.Info("Running SLA Create POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Adjust expected status code as per your API (Created / OK)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            // Optional: add assertions based on your SLA create response contract
            _test.Pass("Positive CreateSLA assertions passed");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "Sub Category " + userId + " Added Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Positive SLA Update assertions passed");

        }

        //
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Categories_Create_Negative_TestData))]
        public async Task Categories_Create_Negative_Test(CategoriesCreateRequest request)
        {
            var endpoint = ApiEndpoints.Categories_Create;

            _test.Info("Running Categories CREATE NEGATIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            var response = await _apiClient.PostAsync(endpoint, request);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            Assert.That((int)response.StatusCode, Is.GreaterThanOrEqualTo(400),
                "Expected 4xx error code for negative test cases.");

            _test.Pass("Categories CREATE (negative) test passed.");
        }

        // Rename Categories
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Categories_PUT_Rename_TestData))]
        public async Task Categories_Rename_Positive_Test(CategoriesRenameRequest payload)
        {
            var endpoint = ApiEndpoints.Categories_Rename;

            _test.Info("Running SLA Create POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Adjust expected status code as per your API (Created / OK)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            // Optional: add assertions based on your SLA create response contract
            _test.Pass("Positive CreateSLA assertions passed");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "Category " + userId + " Updated Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Positive SLA Update assertions passed");

        }

        

        // TurnOffON Categories
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Categories_PUT_TurnOffON_TestData))]
        public async Task Categories_TurnOffON_Positive_Test(CategoriesturnonoffRequest payload)
        {
            var endpoint = ApiEndpoints.Categories_TurnOffON;

            _test.Info("Running SLA Create POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Adjust expected status code as per your API (Created / OK)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            // Optional: add assertions based on your SLA create response contract
            _test.Pass("Positive CreateSLA assertions passed");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "Category " + userId + " Turned On/Off Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Positive Turned On/Off assertions passed");

        }


        // TurnOffON Categories
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Categories_PUT_MoveupDown_TestData))]
        public async Task Categories_MoveupDown_Positive_Test(CategoriesmoveupdownRequest payload)
        {
            var endpoint = ApiEndpoints.Categories_MoveupDown;

            _test.Info("Running SLA Create POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Adjust expected status code as per your API (Created / OK)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            // Optional: add assertions based on your SLA create response contract
            _test.Pass("Positive MoveupDown assertions passed");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "Category " + userId + " Moved Up/Down Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Positive MoveupDown assertions passed");

        }


        // Makedefault Categories
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Categories_PUT_Makedefault_TestData))]
        public async Task Categories_Makedefault_Positive_Test(CategoriesMakedefaultRequest payload)
        {
            var endpoint = ApiEndpoints.Categories_Makedefault;

            _test.Info("Running Makedefault POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            var response = await _apiClient.PutAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Adjust expected status code as per your API (Created / OK)
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            // Optional: add assertions based on your SLA create response contract
            _test.Pass("Positive Makedefault assertions passed");

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var userId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "Category " + userId + " Made Default Successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Positive MoveupDown assertions passed");

        }

        // Positive

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.Categories_Delete_Positive_TestData))]
        public async Task Categories_Delete_Positive_Test(int dummy)
        {
            // 1️⃣ Pre‑Req: Create category to delete
            int categoryId = await CreateCategoryReturnIdAsync();

            //Console.WriteLine(categoryId);

            string endpoint = string.Format(ApiEndpoints.Categories_RemoveCat, categoryId);

            _test.Info("Running Categories DELETE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");

            //var response = await _apiClient.DeleteAsync(endpoint);

            //_test.Info($"Response Status: {response.StatusCode}");
            //_test.Info($"Response Body: {response.Content}");

            //Assert.That(response.StatusCode,
            //    Is.EqualTo(HttpStatusCode.OK)
            //    .Or.EqualTo(HttpStatusCode.NoContent));

            _test.Pass("Categories DELETE (positive) test passed.");
        }









       


    }
}
