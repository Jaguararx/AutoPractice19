using System;
using System.IO;
using System.Linq;
using System.Text;
using Allure.Commons;
using COE.Core.Resilience;
using COE.Core.Utils;
using COE.Core.Visual;
using OpenQA.Selenium;
//using ReportPortal.Client.Abstractions.Requests;
//using ReportPortal.Shared.Reporter;
//using ReportPortal.SpecFlowPlugin;
using TechTalk.SpecFlow;

namespace COE.Example.Tests.Hooks
{
    [Binding]
    public sealed class AfterScenario
    {
        private readonly TestContext _testContext;
        private readonly ScenarioContext _scenarioContext;
        private readonly IWebDriver _webDriver;
        private readonly IWaiter _waiter;
        private readonly IVisualDriver _visualDriver;

        public AfterScenario(TestContext testContext,
            ScenarioContext scenarioContext,
            IWebDriver webDriver,
            IWaiter waiter,
            IVisualDriver visualDriver)
        {
            _testContext = testContext ?? throw new ArgumentNullException(nameof(testContext));
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _webDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
            _waiter = waiter ?? throw new ArgumentNullException(nameof(waiter));
            _visualDriver = visualDriver ?? throw new ArgumentNullException(nameof(visualDriver));
        }

        #region AfterScenario hooks
        [AfterScenario(Order = 1)]
        private void OnError()
        {
            if (_testContext.Current.TestError == null)
            {
                return;
            }

            //var testReporter = ReportPortalAddin.GetScenarioTestReporter(_scenarioContext);

            TakeScreenshot();
            //Add page source to Allure in Failure
            AllureLifecycle.Instance.AddAttachment($"PageSource_{DateTime.Now:HH:mm:ss}",
            "text/html",
            Encoding.Unicode.GetBytes(_webDriver.PageSource),
            ".html");

            // Issue with getting browser logs fixed in selenium-4.0.0-alpha-7
            // https://github.com/SeleniumHQ/selenium/commit/1156fbc60277cb0a241809c022221541e118ab28
            // SaveBrowserConsoleLogs();

            LogTestContext();

            CloseSecondaryTabs();
        }

        [AfterScenario(Order = 2)]
        public void HandleAlerts()
        {
            var lastTestException = _testContext.Current.TestError;
            // Check if we are dealing with an expected or unexpected alert.
            var shouldHandleUnexpectedAlerts = lastTestException != null && lastTestException.Message.Contains("unexpected alert", StringComparison.OrdinalIgnoreCase);
            var shouldHandleExpectedAlerts = _testContext.Current.ScenarioInfo.Tags.Contains(nameof(TestTags.HandleAlerts));

            if (shouldHandleUnexpectedAlerts | shouldHandleExpectedAlerts)
            {
                var alertIsAlreadyPresent = Alerts.IsAlertPresent(_webDriver, _waiter);

                if (!alertIsAlreadyPresent)
                {
                    try
                    {
                        _webDriver.Navigate().Refresh();
                    }
                    catch (UnhandledAlertException)
                    {
                        // Expected - navigation event will trigger alert if page requires confirmation before moving away.
                        // Swallow and continue.
                    }
                }

                while (Alerts.IsAlertPresent(_webDriver, _waiter))
                {
                    try
                    {
                        Alerts.AcceptAlert(_webDriver, _waiter);
                    }
                    // Fail-safe so that test cleanup can continue if anything unexpected goes wrong.
                    catch (NoSuchElementException)
                    {
                        Console.WriteLine("=================== ALERT ERROR =======================");
                        Console.WriteLine($"Tried to dismiss an alert but no alert was found.");
                        Console.WriteLine("=======================================================");
                        break;
                    }
                }
            }
        }

        [AfterScenario(Order = 5)]
        [Scope(Tag = TestTags.CloseSecondaryTabs)]
        private void CloseSecondaryTabs()
        {
            var tabsCount = _webDriver.WindowHandles.Count;

            for (int i = tabsCount - 1; i > 0; --i)
            {
                _webDriver.SwitchTo().Window(_webDriver.WindowHandles[i]);
                _webDriver.Close();
            }
            _webDriver.SwitchTo().Window(_webDriver.WindowHandles[0]);
        }

        [Scope(Tag = TestTags.VisualTest)]
        [AfterScenario(Order = 8)]
        public void EndVisualTest()
        {
            var results = _visualDriver.GetCurrentTestResult();
            foreach (var result in results.ComparisonResults)
            {
                if (result.Difference != null)
                {
                    // Add the difference image to test output
                    _testContext.AddAttachmentToTestResult(result.Difference.Path);
                }
                // Add the baseline image to test output
                _testContext.AddAttachmentToTestResult(result.Baseline.Path);
            }

            _visualDriver.EndVisualTest();
        }

        #endregion

        #region private methods
        private void TakeScreenshot()
        {
            var fileName =
                $"ERROR_FEATURE_{_testContext.FeatureContext.FeatureInfo.Title}_SCENARIO_{_testContext.Current.ScenarioInfo.Title}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";

            Screenshots.TakeScreenshot(_webDriver, fileName, path =>
            {
                _testContext.AddAttachmentToTestResult(path);
                var bytes = File.ReadAllBytes(path);
                //Add screenshot to Allure on Failure
                if (_testContext.TestSettings.AllureEnabled)
                {
                    AllureLifecycle.Instance.AddAttachment($"Screenshot [{DateTime.Now:HH:mm:ss}]",
                    "image/png",
                    bytes);
                }
                //Add screenshot to ReportPortal on Failure
                //string name = "Screenshot [{DateTime.Now:HH:mm:ss}]";
                //testReporter.Log(new CreateLogItemRequest
                //{
                //    Time = DateTime.UtcNow,
                //    Level = ReportPortal.Client.Abstractions.Models.LogLevel.Error,
                //    Text = name,
                //    Attach = new LogItemAttach
                //    {
                //        Name = name,
                //        MimeType = "images/png",
                //        Data = bytes
                //    }
                //});
            });
        }

        private void SaveBrowserConsoleLogs()
        {
            var fileName =
                $"ERROR_FEATURE_{_testContext.FeatureContext.FeatureInfo.Title}_SCENARIO_{_testContext.Current.ScenarioInfo.Title}_LOGS_{DateTime.UtcNow:yyyyMMdd_HHmmss}";

            TestLogs.SaveBrowserConsoleLogs(_webDriver, fileName, _testContext.AddAttachmentToTestResult);
            TestLogs.SaveBrowserConsoleLogs(_webDriver, fileName, path => {
                //Add browser console log to Allure
                AllureLifecycle.Instance.AddAttachment($"BrowserConsoleLog_{DateTime.Now:HH:mm:ss}",
                "text/plain",
                File.ReadAllBytes(path),
                ".txt");
            });
        }

        private void LogTestContext()
        {
            Console.WriteLine("===================TEST CONTEXT =======================");
            Console.WriteLine($"{_testContext}");
            Console.WriteLine("=======================================================");
        }
        #endregion
    }
}
