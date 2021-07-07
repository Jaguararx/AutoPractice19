using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace COE.Core.Resilience
{
    public class WebWaiter : IWaiter
    {
        private readonly WebDriverWait _defaultWait;

        public WebWaiter(IWebDriver driver, TimeSpan pollingInterval)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver));
            }
            _defaultWait = new WebDriverWait(new SystemClock(), driver, TimeSpan.FromSeconds(5), pollingInterval);
        }

        private IWaiter SetWaitTimeout(TimeSpan timespan)
        {
            if (_defaultWait.Timeout != timespan)
            {
                _defaultWait.Timeout = timespan;
            }
            return this;
        }

        /// <summary>
        /// Sets a predefined standard wait timeout for this wait object
        /// </summary>
        public IWaiter Wait => SetWaitTimeout(TimeSpan.FromSeconds(5));

        /// <summary>
        /// Sets a predefined short wait timeout for this wait object
        /// </summary>
        public IWaiter ShortWait => SetWaitTimeout(TimeSpan.FromSeconds(2));

        /// <summary>
        /// Sets a predefined long wait timeout for this wait object
        /// </summary>
        public IWaiter LongWait => SetWaitTimeout(TimeSpan.FromSeconds(15));

        /// <summary>
        /// Sets a custom wait timeout for this wait object
        /// </summary>
        public IWaiter CustomWait(TimeSpan timeout)
        {
            return SetWaitTimeout(timeout);
        }

        /// <summary>
        /// Repeatedly applies this instance's input value to the given function until one
        /// of the following occurs:
        /// the function returns neither null nor false, the function throws an exception that
        /// is not in the list of ignored exception types, or the timeout expires
        /// </summary>
        public TResult Until<TResult>(Func<IWebDriver, TResult> waitCondition)
        {
            return _defaultWait.Until(waitCondition);
        }

        /// <summary>
        /// Optionally ignore exceptions if the wait operation times out
        /// </summary>
        public TResult Until<TResult>(Func<IWebDriver, TResult> waitCondition, bool throwOnTimeout)
        {
            try
            {
                return Until(waitCondition);
            }
            catch (WebDriverTimeoutException)
            {
                if (throwOnTimeout)
                {
                    throw;
                }
            }
            return default(TResult);
        }

        /// <summary>
        /// Repeatedly applies this instance's input value to the given function until one
        /// of the following occurs:
        /// the function returns neither null nor false, the function throws an exception that
        /// is not in the list of ignored exception types, or the timeout expires
        /// </summary>
        /// <param name="waitCondition">The condition to be waited for</param>
        /// <param name="failureReason">If the wait operation fails, this will be used to provide additional context to the exception</param>
        public TResult Until<TResult>(Func<IWebDriver, TResult> waitCondition, string failureReason)
        {
            try
            {
                return _defaultWait.Until(waitCondition);
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException(failureReason, ex);
            }
        }
    }
}