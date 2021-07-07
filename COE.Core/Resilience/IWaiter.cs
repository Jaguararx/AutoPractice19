using System;

using OpenQA.Selenium;

namespace COE.Core.Resilience
{
    public interface IWaiter
    {
        IWaiter LongWait { get; }
        IWaiter ShortWait { get; }
        IWaiter Wait { get; }

        IWaiter CustomWait(TimeSpan timeout);
        TResult Until<TResult>(Func<IWebDriver, TResult> waitCondition);
        TResult Until<TResult>(Func<IWebDriver, TResult> waitCondition, string failureReason);
        TResult Until<TResult>(Func<IWebDriver, TResult> waitCondition, bool throwOnTimeout);
    }
}