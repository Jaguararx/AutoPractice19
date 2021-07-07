using System;
using System.IO;

using OpenQA.Selenium;

namespace COE.Core.Utils
{
    public static class Screenshots
    {
        /// <summary>
        /// Takes a screenshot and attaches it to the test result output using the mechanism provided by the supplied delegate.
        /// For example, NUnit's TestContext.AddTestAttachment() can be used to make the screenshot available to test publishing providers such as Azure DevOps' Publish Test Result task.
        /// </summary>
        /// <param name="addTestAttachment">The delegate to be used to attach the screenshot to the result output</param>
        public static void TakeScreenshot(IWebDriver driver, string screenshotName, Action<string> addTestAttachment)
        {
            var screenshotPath = TakeScreenshot(driver, screenshotName);

            if (!File.Exists(screenshotPath))
            {
                Console.WriteLine($"Unable to attach screenshot {screenshotName} to test output");
                return;
            }

            addTestAttachment.Invoke(screenshotPath);
        }

        /// <summary>
        /// Takes a screenshot and saves it to the executing directory of the current assembly
        /// </summary>
        private static string TakeScreenshot(IWebDriver driver, string screenshotName)
        {
            if (!(driver is ITakesScreenshot takesScreenshot))
            {
                throw new WebDriverException("Unable to take screenshot. The supplied WebDriver instance does not implement ITakesScreenshot");
            }

            try
            {
                var artifactDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");

                if (!Directory.Exists(artifactDirectory))
                {
                    Directory.CreateDirectory(artifactDirectory);
                }

                var screenshot = takesScreenshot.GetScreenshot();

                var screenshotFilePath = Path.Combine(artifactDirectory, screenshotName + "_screenshot.png");

                screenshot.SaveAsFile(screenshotFilePath, ScreenshotImageFormat.Png);

                Console.WriteLine("=================== SCREENSHOT ========================");
                Console.WriteLine($"Saved to: {new Uri(screenshotFilePath)}");
                Console.WriteLine("=======================================================");

                return screenshotFilePath;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error while taking screenshot: {ex}");
                return "";
            }
        }
    }
}