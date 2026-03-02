using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace ApiAutomationFramework
{
    
public class ApiClient : IApiClient
    {
        private readonly RestClient _client;
        private readonly string _token;

        public ApiClient(string baseUrl, string token)
        {
            _client = new RestClient(baseUrl);
            _token = token;
        }

        private RestRequest CreateRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);
            request.AddHeader("Authorization", $"Bearer {_token}");
            return request;
        }

        public async Task<RestResponse> GetAsync(string endpoint)
        {
            var request = CreateRequest(endpoint, Method.Get);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> GetAllAsync(string endpoint) 
        {
            var request = CreateRequest(endpoint, Method.Get);
            return await _client.ExecuteAsync(request);
        }


        public async Task<RestResponse> PostAsync(string endpoint, object payload)
        {
            var request = CreateRequest(endpoint, Method.Post);
            request.AddJsonBody(payload);
            return await _client.ExecuteAsync(request);
        }


        public async Task<RestResponse> PostAsync(string endpoint)
        {
            var request = CreateRequest(endpoint, Method.Post);
            return await _client.ExecuteAsync(request);
        }



        public async Task<RestResponse> PutAsync(string endpoint, object payload)
        {
            var request = CreateRequest(endpoint, Method.Put);
            request.AddJsonBody(payload);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> DeleteAsync(string endpoint)
        {
            var request = CreateRequest(endpoint, Method.Delete);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> DeleteAsync(string endpoint, object payload)
        {
            var request = CreateRequest(endpoint, Method.Delete);
            request.AddJsonBody(payload);
            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> PostFileAsync(string endpoint, object payload)
        {
            var request = CreateRequest(endpoint, Method.Post);
            request.AlwaysMultipartFormData = true;

            string? fileParamName = null;
            string? filePath = null;

            // Use reflection to extract properties
            foreach (var prop in payload.GetType().GetProperties())
            {
                var name = prop.Name;
                var value = prop.GetValue(payload);

                if (value == null) continue;

                // Detect file property (by convention: filePath)
                if (name.Equals("filePath", StringComparison.OrdinalIgnoreCase))
                {
                    fileParamName = "file";
                    filePath = value.ToString();
                }
                else
                {
                    request.AddParameter(name, value.ToString());
                }
            }

            if (fileParamName == null || filePath == null)
                throw new ArgumentException("Payload must contain a 'filePath' property.");

            request.AddFile(fileParamName, filePath);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> GetFileAsync(string endpoint, object payload)
        {
            var request = CreateRequest(endpoint, Method.Get);

            // Add query parameters from payload properties
            foreach (var prop in payload.GetType().GetProperties())
            {
                var name = prop.Name;
                var value = prop.GetValue(payload);
                if (value != null)
                {
                    request.AddParameter(name, value.ToString());
                }
            }

            return await _client.ExecuteAsync(request);
        }

    }

}
