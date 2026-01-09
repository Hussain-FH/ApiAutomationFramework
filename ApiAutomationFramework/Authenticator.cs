
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace ApiAutomationFramework
{
    public class Authenticator
    {
        private readonly string _authUrl;
        private string _token;

        public Authenticator()
        {
            // Dynamic AuthUrl selection based on Env
            var env = ConfigReader.GetConfigValue("Env")?.Trim();
            if (string.IsNullOrWhiteSpace(env)) env = "UAT";

            switch (env.ToUpperInvariant())
            {
                case "DEV":
                    _authUrl = ConfigReader.GetConfigValue("AuthUrl_Dev");
                    break;
                case "QA":
                    _authUrl = ConfigReader.GetConfigValue("AuthUrl_QA");
                    break;
                case "UAT":
                default:
                    _authUrl = ConfigReader.GetConfigValue("AuthUrl_UAT");
                    break;
            }

            if (string.IsNullOrWhiteSpace(_authUrl))
                throw new ArgumentException($"AuthUrl_{env} is missing or empty in appsettings.json");
        }

        /// <summary>
        /// Generates token using GET request and caches it.
        /// Removes 'Bearer Token:' prefix and returns a clean JWT.
        /// </summary>
        public async Task<string> GenerateTokenAsync()
        {
            if (!string.IsNullOrWhiteSpace(_token))
                return _token;

            try
            {
                var client = new RestClient();
                var request = new RestRequest(_authUrl, Method.Get);

                // Query parameters (same as Postman)
                request.AddQueryParameter("username", ConfigReader.GetConfigValue("Username"));
                request.AddQueryParameter("password", ConfigReader.GetConfigValue("Password"));

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
                {
                    throw new Exception(
                        $"Token generation failed: {response.StatusCode} - {response.Content}");
                }

                dynamic json = JsonConvert.DeserializeObject(response.Content);

                // Read token from configured field (idToken by default)
                string tokenField = ConfigReader.GetConfigValue("TokenFieldName");
                if (string.IsNullOrWhiteSpace(tokenField)) tokenField = "idToken";

                string raw = null;
                try { raw = json[tokenField]?.ToString(); } catch { }

                // Fallbacks
                if (string.IsNullOrWhiteSpace(raw))
                {
                    try { raw = json.idToken?.ToString(); } catch { }
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        try { raw = json.accessToken?.ToString(); } catch { }
                    }
                }

                if (string.IsNullOrWhiteSpace(raw))
                    throw new Exception("Token not found in response.");

                // ✅ Clean the token prefix and cache
                _token = CleanTokenPrefix(raw);

                if (string.IsNullOrWhiteSpace(_token))
                    throw new Exception("Token cleanup failed.");

                return _token;
            }
            catch
            {
                throw;
            }
        }

        public Task<string> GetTokenAsync() => GenerateTokenAsync();

        /// <summary>
        /// Removes 'Bearer Token:', 'Bearer', and 'Token:' prefixes.
        /// </summary>
        private static string CleanTokenPrefix(string tokenWithPrefix)
        {
            if (string.IsNullOrWhiteSpace(tokenWithPrefix)) return tokenWithPrefix;

            string cleaned = tokenWithPrefix;

            if (cleaned.StartsWith("Bearer Token:", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring("Bearer Token:".Length).Trim();
            else if (cleaned.StartsWith("Bearer:", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring("Bearer:".Length).Trim();
            else if (cleaned.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring("Bearer ".Length).Trim();

            if (cleaned.StartsWith("Token:", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring("Token:".Length).Trim();

            return cleaned;
        }
    }
}
