using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutomationFramework
{

    public static class Logger
    {
        public static void LogRequest(string endpoint, string payload)
        {
            Console.WriteLine($"Request to {endpoint}: {payload}");
        }

        public static void LogResponse(string response)
        {
            Console.WriteLine($"Response: {response}");
        }

    }
}
