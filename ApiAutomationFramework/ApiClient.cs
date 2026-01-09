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
    }

}
