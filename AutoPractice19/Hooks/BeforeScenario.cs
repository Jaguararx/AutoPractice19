using System;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

using TechTalk.SpecFlow;

namespace COE.Example.Tests.Hooks
{
    [Binding]
    public sealed class BeforeScenario
    {
        private readonly TestContext _testContext;
        private readonly TestSettings _settings;
        private readonly IWebDriver _webDriver;


        public BeforeScenario(TestContext testContext, TestSettings settings, IWebDriver webDriver)
        {
            _webDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _testContext = testContext ?? throw new ArgumentNullException(nameof(testContext));
        }

        [BeforeScenario(Order = 1)]
        public void ResetPage()
        {
            _webDriver.Navigate().GoToUrl(_testContext.WebDriverSettings.PageUrl);
        }


    }
}