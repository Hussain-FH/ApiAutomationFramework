using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApiAutomationFramework;
using APITestSolution.DataProviders;
using NUnit.Framework;

namespace APITestSolution.TestsScripts.Orders
{
    public class OrdersTests : BaseTest
    {
        // 🔖 Get – View Order Details - Special Handling / Pull Request Section Get API by Shipment Id (Positive)
        [TestCaseSource(typeof(UserDataProvider), nameof(UserDataProvider.OrderDetailsView_Get_Positive_TestData))]
        public async Task OrderDetailsView_Get_Positive_TestMethod(int shipmentId)
        {
            {
                // (If needed) any pre-reqs can be added here

                var endpoint = ApiEndpoints.OrderDetailsView_Get(shipmentId);

                _test.Info("Running GET PullOrCancelOrders POSITIVE test...");
                _test.Info($"Endpoint: {endpoint}");

                var response = await _apiClient.GetAsync(endpoint);

                _test.Info($"Response Status: {response.StatusCode}");
                _test.Info($"Response Body: {response.Content}");

                // ✅ Expect 200 OK for valid ShipmentId
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                    "Expected 200 OK for valid pull/cancel orders request.");

                // ✅ Basic content checks

                //var contentType = response.Headers
                //    .FirstOrDefault(h => string.Equals(h.Name, "Content-Type", StringComparison.OrdinalIgnoreCase))
                //    ?.Value?.ToString() ?? string.Empty;

                //StringAssert.Contains("json", contentType.ToLowerInvariant(), "Content-Type should be application/json");               

                var body = (response.Content ?? string.Empty).Trim();

                // ❌ Should not contain common error indicators
                Assert.That(body,
                    Does.Not.Match(@"(?i)(invalid|error|required|missing|failed|not\s*allowed|unauthori[sz]ed|forbidden)"),
                    "Response body indicates an error for a positive scenario.");

                _test.Pass("GET PullOrCancelOrders (positive) assertions passed.");
            }
        }
    }
}
