using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ApiAutomationFramework
{
    
public static class RetryHelper
    {
        public static async Task<RestResponse> ExecuteWithRetry(Func<Task<RestResponse>> apiCall, int retries = 3, int delayMs = 1000)
        {
            RestResponse response = null;
            for (int i = 0; i < retries; i++)
            {
                response = await apiCall();
                if (response.IsSuccessful)
                    return response;

                Console.WriteLine($"Retry {i + 1} failed. Status: {response.StatusCode}. Retrying...");
                await Task.Delay(delayMs);
            }
            return response; // Return last response even if failed
        }
    }

}
