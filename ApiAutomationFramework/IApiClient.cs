using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace ApiAutomationFramework
{

    public interface IApiClient
    {
        Task<RestResponse> GetAsync(string endpoint);
        Task<RestResponse> GetAllAsync(string endpoint);
        Task<RestResponse> PostAsync(string endpoint, object payload);
        Task<RestResponse> PutAsync(string endpoint, object payload);
        Task<RestResponse> DeleteAsync(string endpoint);
    }

}
