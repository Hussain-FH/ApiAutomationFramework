
using ApiAutomationFramework.Models.Request;
using ApiAutomationFramework;
using NUnit.Framework;
using APITestSolution.DataProviders;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using ApiAutomationFramework.Models.Response.EMV;

namespace APITestSolution.TestsScripts.Users
{
    [TestFixture]
    public class EMVCardProfiles : BaseTest
    {
        // ✅ POST - Create User
        [Test, TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.EMVCardProfile_Create_TestData))]
        public async Task CreateUser(EMVCardProfilesCreateRequest payload)
        {
        //    _test.Info("Running EMVCardProfile Create test...");
        //    _test.Info($"Endpoint: {ApiEndpoints.EMVCardProfiles_create}");
        //    _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

        //    var response = await _apiClient.PostAsync(ApiEndpoints.EMVCardProfiles_create, payload);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    ResponseValidator.ValidateStatusCode(response, System.Net.HttpStatusCode.Created);
        //    var responseObject = SerializationHelper.DeserializeObject<EMVCardProfilesCreateResponse>(response.Content);

        //    // Assertions
        //    Assert.That(responseObject.name, Is.EqualTo(payload.name), "Name should match");
        //    Assert.That(responseObject.userName, Is.EqualTo(payload.userName), "UserName should match");
        //    Assert.That(responseObject.issuerId, Is.EqualTo(payload.issuerId), "issuerId should match");
        //    Assert.That(responseObject.expirationDate, Is.EqualTo(payload.expirationDate), "expirationDate should match");
        //    Assert.That(responseObject.discription, Is.EqualTo(payload.discription), "Description should match");
        //    //Assert.That(responseObject.Id, Is.Not.Null, "Id should not be null");

        //    // ✅ DB Validation
        //    //var dbResult = DatabaseHelper.ExecuteQuery($"SELECT TOP 1 Name, Email FROM Users WHERE Id = {responseObject.Id}");
        //    //Assert.That(dbResult.Rows.Count, Is.EqualTo(1), "User should exist in DB");
        //    //Assert.That(dbResult.Rows[0]["Name"].ToString(), Is.EqualTo(payload.Name), "DB Name should match");
        //    //Assert.That(dbResult.Rows[0]["Email"].ToString(), Is.EqualTo(payload.Email), "DB Email should match");

        //    _test.Pass("CreateUser assertions and DB validation passed");
        }

        //// ✅ PUT - Update User
        //[Test, TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.GetUpdateUserTestData))]
        //public async Task UpdateUser(int userId, emv payload)
        //{
        //    _test.Info("Running UpdateUser test...");
        //    var endpoint = ApiEndpoints.UpdateUser.Replace("{id}", userId.ToString());
        //    _test.Info($"Endpoint: {endpoint}");
        //    _test.Info($"Request Payload: {JsonConvert.SerializeObject(payload)}");

        //    var response = await _apiClient.PutAsync(endpoint, payload);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    ResponseValidator.ValidateStatusCode(response, System.Net.HttpStatusCode.OK);
        //    var responseObject = SerializationHelper.DeserializeObject<UpdateUserResponse>(response.Content);

        //    Assert.That(responseObject.Name, Is.EqualTo(payload.Name), "Updated name should match");

        //    // ✅ DB Validation
        //    var dbResult = DatabaseHelper.ExecuteQuery($"SELECT Name FROM Users WHERE Id = {userId}");
        //    Assert.That(dbResult.Rows[0]["Name"].ToString(), Is.EqualTo(payload.Name), "DB Name should match after update");

        //    _test.Pass("UpdateUser assertions and DB validation passed");
        //}

        //// ✅ GET - Get User Details
        //[Test, TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.GetGetUserTestData))]
        //public async Task GetUser(int userId)
        //{
        //    _test.Info("Running GetUser test...");
        //    var endpoint = ApiEndpoints.GetUser.Replace("{id}", userId.ToString());
        //    _test.Info($"Endpoint: {endpoint}");

        //    var response = await _apiClient.GetAsync(endpoint);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    ResponseValidator.ValidateStatusCode(response, System.Net.HttpStatusCode.OK);
        //    var responseObject = SerializationHelper.DeserializeObject<GetUserResponse>(response.Content);

        //    Assert.That(responseObject.Id, Is.EqualTo(userId), "User ID should match");

        //    // ✅ DB Validation
        //    var dbResult = DatabaseHelper.ExecuteQuery($"SELECT Id FROM Users WHERE Id = {userId}");
        //    Assert.That(dbResult.Rows.Count, Is.EqualTo(1), "User should exist in DB");

        //    _test.Pass("GetUser assertions and DB validation passed");
        //}

        //// ✅ GETALL - Get All Users
        //[Test]
        //public async Task GetAllUsers()
        //{
        //    _test.Info("Running GetAllUsers test...");
        //    _test.Info($"Endpoint: {ApiEndpoints.GetAllUsers}");

        //    var response = await _apiClient.GetAllAsync(ApiEndpoints.GetAllUsers);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    ResponseValidator.ValidateStatusCode(response, System.Net.HttpStatusCode.OK);
        //    var responseObject = SerializationHelper.DeserializeObject<List<GetUserResponse>>(response.Content);

        //    Assert.That(responseObject.Count, Is.GreaterThan(0), "User list should not be empty");

        //    _test.Pass("GetAllUsers assertions passed");
        //}

        //// ✅ DELETE - Delete User
        //[Test, TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.GetDeleteUserTestData))]
        //public async Task DeleteUser(int userId)
        //{
        //    _test.Info("Running DeleteUser test...");
        //    var endpoint = ApiEndpoints.DeleteUser.Replace("{id}", userId.ToString());
        //    _test.Info($"Endpoint: {endpoint}");

        //    var response = await _apiClient.DeleteAsync(endpoint);

        //    _test.Info($"Response Status: {response.StatusCode}");
        //    _test.Info($"Response Body: {response.Content}");

        //    ResponseValidator.ValidateStatusCode(response, System.Net.HttpStatusCode.NoContent);

        //    // ✅ DB Validation
        //    var dbResult = DatabaseHelper.ExecuteQuery($"SELECT Id FROM Users WHERE Id = {userId}");
        //    Assert.That(dbResult.Rows.Count, Is.EqualTo(0), "User should be deleted from DB");

        //    _test.Pass("DeleteUser assertions and DB validation passed");
        //}
    }
}
