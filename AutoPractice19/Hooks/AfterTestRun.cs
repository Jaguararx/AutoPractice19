using BoDi;
using COE.Core.Helpers;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace COE.Example.Tests.Hooks
{
    [Binding]
    public sealed class AfterTestRun
    {
        [AfterTestRun]
        public static void TearDown(IWebDriver driver, SqlHelper sqlHelper, IObjectContainer container, TestSettings settings)
        {
            // Bug - Specflow container does not correctly dispose dependencies in some cases, so call dispose explicitly on each one
            driver?.Dispose();
            sqlHelper.Dispose();
            container?.Dispose();
        }

    }
}