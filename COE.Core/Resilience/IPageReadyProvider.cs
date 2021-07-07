using System;

using OpenQA.Selenium;

namespace COE.Core.Resilience
{
    public interface IPageReadyProvider
    {
        Func<IWebDriver, bool> PageIsReady();
    }
}