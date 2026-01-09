
using Microsoft.Extensions.Configuration;

namespace ApiAutomationFramework
{
    public static class ApiEndpoints
    {
        // Load appsettings.json once
        private static readonly IConfiguration _config =
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

        // Read values from appsettings.json
        private static readonly string BaseUrl = _config["BaseUrl_UAT"] ?? string.Empty;
        private static readonly string AuthUrl = _config["AuthUrl_UAT"] ?? string.Empty;

        // Helper method to build URLs
        private static string Url(string path) => $"{BaseUrl}/{path}";

        // ✅ API Endpoints
        public static string EMVCardProfiles_create => Url("cardprofiles");
        public static string EMVCardProfiles_Gett(string id) => Url($"users/{id}");
        public static string EMVCardProfiles_Delete => Url("users");
        public static string UpdateUser(string id) => Url($"users/{id}");
        public static string DeleteUser(string id) => Url($"users/{id}");


        //users  

        public static string Users_createuser => Url("users"); 
        public static string Users_updateuser => Url("users/updateuser");

        // ✅ Auth Endpoint
        public static string SignIn => AuthUrl;
    }
}
