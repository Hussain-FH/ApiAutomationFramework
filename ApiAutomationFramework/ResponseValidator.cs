using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework
{

    public static class ResponseValidator
    {
        public static void ValidateStatusCode(RestResponse response, System.Net.HttpStatusCode expectedStatus)
        {
            if (response.StatusCode != expectedStatus)
            {
                throw new Exception($"Expected {expectedStatus}, but got {response.StatusCode}. Response: {response.Content}");
            }
        }

    }
}
