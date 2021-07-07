using COE.Core.Resilience;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace COE.Core.Utils
{
    public static class Alerts
    {
        public static IAlert GetAlert(IWebDriver driver, IWaiter waiter)
        {
            if (!IsAlertPresent(driver, waiter))
            {
                throw new NoSuchElementException("Unable to find an alert message.");
            }
            return driver.SwitchTo().Alert();
        }

        public static void AcceptAlert(IWebDriver driver, IWaiter waiter)
        {
            GetAlert(driver, waiter).Accept();
            waiter.ShortWait.Until(ExpectedConditions.AlertState(false), throwOnTimeout: true);
        }

        public static void DismissAlert(IWebDriver driver, IWaiter waiter)
        {
            GetAlert(driver, waiter).Dismiss();
            waiter.ShortWait.Until(ExpectedConditions.AlertState(false), throwOnTimeout: true);
        }

        public static bool IsAlertPresent(IWebDriver driver, IWaiter waiter)
        {
            try
            {
                waiter.ShortWait.Until(ExpectedConditions.AlertState(true), throwOnTimeout: true);
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}