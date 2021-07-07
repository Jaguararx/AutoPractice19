using System;
using System.IO;
using System.Linq;

using OpenQA.Selenium;

namespace COE.Core.Utils
{
    public static class TestLogs
    {
        /// <summary>
        /// Gets the browser logs and attaches it to the test result output using the mechanism provided by the supplied delegate.
        /// For example, NUnit's TestContext.AddTestAttachment() can be used to make the logs available to test publishing providers such as Azure DevOps' Publish Test Result task.
        /// </summary>
        /// <param name="addTestAttachment">The delegate to be used to attach the log to the result output</param>
        public static void SaveBrowserConsoleLogs(IWebDriver driver, string logName, Action<string> addTestAttachment)
        {
            var logPath = SaveLogs(driver, LogType.Browser, logName);

            if (!File.Exists(logPath))
            {
                Console.WriteLine($"Unable to attach log {logName} to test output");
                return;
            }

            addTestAttachment.Invoke(logPath);
        }

        /// <summary>
        /// Gets logs and saves them to the executing directory of the current assembly.
        /// </summary>
        private static string SaveLogs(IWebDriver driver, string logType, string logName)
        {
            try
            {
                var artifactDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"{logType}Logs");

                if (!Directory.Exists(artifactDirectory))
                {
                    Directory.CreateDirectory(artifactDirectory);
                }

                var logEntries = driver.Manage().Logs.GetLog(logType);
                var logEntriesList = logEntries.ToList().Select(l => l.ToString()).ToList();

                var logFilePath = Path.Combine(artifactDirectory, logName + "_log.txt");

                File.WriteAllLines(logFilePath, logEntriesList);

                Console.WriteLine($"================== {logType.ToUpper()} LOGS =======================");
                Console.WriteLine($"Saved to: {new Uri(logFilePath)}");
                foreach (var logEntry in logEntriesList)
                {
                    Console.WriteLine(logEntry);
                }
                Console.WriteLine("=======================================================");

                return logFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving logs: {ex}");
                return "";
            }
        }
    }
}