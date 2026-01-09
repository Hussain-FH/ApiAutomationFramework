
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ApiAutomationFramework
{
    public static class ConfigReader
    {
        private static readonly IConfigurationRoot Configuration;

        static ConfigReader()
        {
            try
            {
                var basePath = AppContext.BaseDirectory;
                var configFile = Path.Combine(basePath, "appsettings.json");

                if (!File.Exists(configFile))
                {
                    throw new FileNotFoundException($"Configuration file not found at: {configFile}");
                }

                Configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConfigReader] Error loading configuration: {ex.Message}");
                throw; // Re-throw so you know the root cause
            }
        }

        public static string GetConfigValue(string key)
        {
            var value = Configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"Configuration key '{key}' not found or empty.");
            }
            return value;
        }
    }
}
