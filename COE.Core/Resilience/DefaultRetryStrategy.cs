using System;
using System.Reflection;

using OpenQA.Selenium;

namespace COE.Core.Resilience
{
    public class DefaultRetryStrategy : RetryStrategy
    {
        public DefaultRetryStrategy()
        {
            MaxRetryDuration = TimeSpan.FromSeconds(5);
            SleepBeforeRetryAction = TimeSpan.FromMilliseconds(500);
            SleepBeforeReCheckCondition = TimeSpan.Zero;
            ThrowOnRetryTimeout = true;
            IgnoredTransientExceptions = new[]
            {
                typeof(StaleElementReferenceException),
                typeof(TargetInvocationException),
                typeof(NoSuchElementException),
                typeof(WebDriverTimeoutException),
                typeof(ElementNotVisibleException),
                typeof(ElementClickInterceptedException)
            };
        }
    }
}