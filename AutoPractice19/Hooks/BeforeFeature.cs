using COE.Core.Visual;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace COE.Example.Tests.Hooks
{
    [Binding]
    public sealed class BeforeFeature
    {
        [BeforeFeature]
        [Scope(Tag = TestTags.VisualTest)]
        public static void ConfigureVisualTest(IWebDriver driver, IVisualDriver visualDriver)
        {
            // Set the window to a consistent window size for visual comparison
            driver.Manage().Window.Size = VisualTestConstants.DefaultWindowSize;

            visualDriver.AddGlobalIgnoreRegions(VisualTestConstants.GlobalIgnoreRegions);
        }
    }
}
