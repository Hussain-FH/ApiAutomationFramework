using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework;
using ApiAutomationFramework.Models.Request;
using ApiAutomationFramework.Models.Request.Bulletins;
using ApiAutomationFramework.Models.Request.ClientSettingsHotStamps;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace APITestSolution.TestsScripts.Bulletins
{
    public class BulletinsTests:BaseTest
    {
        public async Task<int> CSPBulletins_File_Upload()
        {
            var endpoint = ApiEndpoints.CSPBulletins_FileUpload;
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "sample.pdf");
            var payload = new FileUploadRequest
            {
                pclmapid = 13,
                fileaccess = 1,
                filePath = filePath
            };

            if (!File.Exists(payload.filePath))
            {
                Assert.Fail($"Test file not found: {payload.filePath}");
            }

            _test.Info("Running CSP Bulletin FILE UPLOAD POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            // Use the generic PostFileAsync method from ApiClient
            var response = await _apiClient.PostFileAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect Ok
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var FileUploadId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage = FileUploadId;

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Bulletin FILE UPLOAD (positive) assertions passed.");
            int UploadId = int.Parse(FileUploadId);
            return UploadId;
        }

        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSPBulletins_Get_Positive_TestData))]
        public async Task CSPBulletins_Get_Positive_Test()
        {
            var endpoint = ApiEndpoints.CSPBulletins_Get;

            _test.Info("Running CSP Bulletins GET POSITIVE test...");
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

            _test.Pass("CSP Bulletins GET (positive) passed.");
        }

        // 🔖 POST – Create (Positive)
        [TestCaseSource(
            typeof(UserDataProvider),
            nameof(UserDataProvider.CSPBulletins_Create_Positive_TestData))]
        public async Task CSPBulletins_Creation_PositiveTestMethod(CSPBulletinsCreateRequest payload)
        {
            var endpoint = ApiEndpoints.CSPBulletins_Get;
            int FileUploadId = await CSPBulletins_File_Upload();
            payload.fileUploadedId= FileUploadId;
            _test.Info("Running CSP Bulletins CREATE POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");
            
            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");
            
            // Expect 201 Created
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var BulletinId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage =
                "Bulletin ID " + BulletinId + " Added successfully.";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Bulletin CREATE (positive) assertions passed.");
        }

        // 🔖 Upload – Bulletin File Upload (Positive)
        [TestCaseSource(
            typeof(UserDataProvider),
            nameof(UserDataProvider.CSPBulletins_Upload_File_Positive_TestData))]
        public async Task CSPBulletins_File_Upload_PositiveTestMethod(FileUploadRequest payload)
        {
            var endpoint = ApiEndpoints.CSPBulletins_FileUpload;

            if (!File.Exists(payload.filePath))
            {
                Assert.Fail($"Test file not found: {payload.filePath}");
            }

            _test.Info("Running CSP Bulletin FILE UPLOAD POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            // Use the generic PostFileAsync method from ApiClient
            var response = await _apiClient.PostFileAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect Ok
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');
            var BulletinId = new string(actualMessage.Where(char.IsDigit).ToArray());
            var expectedMessage = BulletinId;

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            _test.Pass("Bulletin FILE UPLOAD (positive) assertions passed.");
        }

        // 🔖 Download – Bulletin File Download (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.CSPBulletins_Download_File_Positive_TestData))]
        public async Task CSPBulletins_File_Download_PositiveTestMethod(FileDownloadRequest payload)
        {
            var endpoint = ApiEndpoints.CSPBulletins_FileDownload;
            _test.Info("Running CSP Bulletin FILE DOWNLOAD POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: fileformat={payload.fileformat}, entityid={payload.entityid}");

            var response = await _apiClient.GetFileAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Content-Type: {response.ContentType}");            

            // Expect 200 OK
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.OK);
     
            Assert.That(response.ContentType, Is.Not.Null.And.Not.Empty, "Content-Type is missing.");      

            _test.Pass("Bulletin FILE DOWNLOAD (positive) assertions passed.");
        }

       
// 🔖 POST – Bulletin Acknowledge(Positive)
    [TestCaseSource(typeof(UserDataProvider),nameof(UserDataProvider.Bulletins_Ack_Positive_TestData))]
        public async Task Bulletin_Acknowledge_Positive_TestMethod(BulletinAcknowledgeRequest payload)
        {
            var endpoint = ApiEndpoints.BulletinAcknowledge_Post;

            _test.Info("Running Bulletin Acknowledge POSITIVE test...");
            _test.Info($"Endpoint: {endpoint}");
            _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

            // automatically sends {"bulletinId":[1]}
            var response = await _apiClient.PostAsync(endpoint, payload);

            _test.Info($"Response Status: {response.StatusCode}");
            _test.Info($"Response Body: {response.Content}");

            // Expect 201 Created
            ResponseValidator.ValidateStatusCode(response, HttpStatusCode.Created);

            var actualMessage = (response.Content ?? string.Empty).Trim().Trim('"');

            Assert.That(actualMessage,
                Does.Contain("Added").IgnoreCase,
                "Expected the bulletin acknowledgment success message.");

            _test.Pass("Bulletin Acknowledge (positive) assertions passed.");
        }

    }

}

