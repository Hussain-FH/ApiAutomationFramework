
using NUnit.Framework;
using AventStack.ExtentReports;
using ApiAutomationFramework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace APITestSolution.TestsScripts
{
    public class BaseTest
    {
        protected ExtentTest _test;
        protected ApiClient _apiClient;
        protected string _token;
        protected string _baseUrl;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            ReportManager.InitReport();
            CleanupOldFailureLogs();
            CleanupOldReports();

            var authenticator = new Authenticator();
            _token = await authenticator.GetTokenAsync();

            // Dynamic env-based base URL
            var env = ConfigReader.GetConfigValue("Env")?.Trim();
            if (string.IsNullOrWhiteSpace(env)) env = "UAT";

            _baseUrl = ConfigReader.GetConfigValue($"BaseUrl_{env}");
            if (string.IsNullOrWhiteSpace(_baseUrl))
                throw new ArgumentException($"BaseUrl_{env} missing in appsettings.json");
        }

        [SetUp]
        public void TestSetup()
        {
            _apiClient = new ApiClient(_baseUrl, _token);
            _test = ReportManager.CreateTest(TestContext.CurrentContext.Test.Name);
            ReportManager.LogInfo("Starting test: " + TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void TestCleanup()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                string errorMsg = TestContext.CurrentContext.Result.Message;
                ReportManager.LogFail("Test failed: " + errorMsg);
                string filePath = CaptureFailureLog(TestContext.CurrentContext.Test.Name, errorMsg);
                _test.AddScreenCaptureFromPath(filePath);
            }
            else if (status == NUnit.Framework.Interfaces.TestStatus.Passed)
            {
                ReportManager.LogPass("Test passed successfully");
            }
        }

        [OneTimeTearDown]
        public void GlobalCleanup()
        {
            ReportManager.FlushReport();
        }

        private string CaptureFailureLog(string testName, string errorMsg)
        {
            string folderPath = Path.Combine(AppContext.BaseDirectory, "FailureLogs");
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(
                folderPath,
                $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            File.WriteAllText(filePath, $"Error: {errorMsg}");
            return filePath;
        }

        private void CleanupOldFailureLogs()
        {
            string folderPath = Path.Combine(AppContext.BaseDirectory, "FailureLogs");
            if (!Directory.Exists(folderPath)) return;

            foreach (var file in Directory.GetFiles(folderPath))
            {
                if (File.GetCreationTime(file) < DateTime.Now.AddDays(-7))
                    File.Delete(file);
            }
        }

        private void CleanupOldReports()
        {
            var solutionRoot = Directory
                .GetParent(AppContext.BaseDirectory)?
                .Parent?
                .Parent?
                .FullName;

            if (string.IsNullOrWhiteSpace(solutionRoot)) return;

            var reportsDir = Path.Combine(solutionRoot, "Reports");
            if (!Directory.Exists(reportsDir)) return;

            foreach (var file in Directory.GetFiles(reportsDir))
            {
                if (File.GetCreationTime(file) < DateTime.Now.AddDays(-30))
                    File.Delete(file);
            }
        }
    }
}
