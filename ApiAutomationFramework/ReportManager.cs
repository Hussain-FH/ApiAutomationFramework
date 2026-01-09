
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using System;
using System.IO;

namespace ApiAutomationFramework
{
    public static class ReportManager
    {
        private static ExtentReports _extent;
        private static ExtentTest _currentTest;
        private static string _reportPath;

        /// <summary>
        /// Initialize Extent Report
        /// </summary>
        public static void InitReport()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // ✅ Move Reports folder to solution root
            var solutionRoot = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.FullName;
            var reportsDir = Path.Combine(solutionRoot, "Reports");
            Directory.CreateDirectory(reportsDir);

            _reportPath = Path.Combine(reportsDir, $"ExtentReport_{timestamp}.html");

            // ✅ Use ExtentSparkReporter for v5
            var sparkReporter = new ExtentSparkReporter(_reportPath);
            sparkReporter.Config.DocumentTitle = "API Automation Report";
            sparkReporter.Config.ReportName = "API Test Execution";
            sparkReporter.Config.Theme = Theme.Standard;

            _extent = new ExtentReports();
            _extent.AttachReporter(sparkReporter);
        }

        /// <summary>
        /// Create a new test entry in the report
        /// </summary>
        public static ExtentTest CreateTest(string testName)
        {
            _currentTest = _extent.CreateTest(testName);
            return _currentTest;
        }

        public static void LogInfo(string message) => _currentTest.Info(message);
        public static void LogPass(string message) => _currentTest.Pass(message);
        public static void LogFail(string message) => _currentTest.Fail(message);
        public static void LogSkip(string message) => _currentTest.Skip(message);

        /// <summary>
        /// Flush the report to generate HTML file
        /// </summary>
        public static void FlushReport() => _extent.Flush();
    }
}
