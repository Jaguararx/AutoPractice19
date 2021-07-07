using System;

using COE.Core.Utils;

using OpenQA.Selenium;

namespace COE.Core.Resilience
{
    public static class JsExpectedConditions
    {
        /// <summary>
        /// Condition for checking that all Angular page operations are complete for the entire document.
        /// </summary>
        public static Func<IWebDriver, bool> AngularIsReady()
        {
            return driver =>
            {
                var jsExecutor = (IJavaScriptExecutor)driver;

                try
                {
                    var angularCheck = jsExecutor.ExecuteScript("return getAllAngularRootElements()[0].attributes['ng-version']");

                    if (angularCheck == null)
                    {
                        // angular is not defined here so we don't need to wait for it.
                        return true;
                    }

                    var angularPageLoaded = (bool)jsExecutor.ExecuteScript(JavaScriptActions.AngularIsReady());

                    return angularPageLoaded;
                }
                catch (WebDriverException)
                {
                    return false;
                }
            };
        }

        /// <summary>
        /// Condition for checking that a specific element's location and dimensions are stable on the page.
        /// Useful for situations where elements have animated transitions.
        /// </summary>
        /// <param name="by">The By locator for the target element</param>
        public static Func<IWebDriver, bool> IsElementLocationStable(By by)
        {
            return driver =>
            {
                var jsExecutor = (IJavaScriptExecutor)driver;

                try
                {
                    return (bool)jsExecutor.ExecuteScript(JavaScriptActions.CheckElementLocationIsStable(by));
                }
                catch (WebDriverException)
                {
                    return false;
                }
            };
        }
    }
}