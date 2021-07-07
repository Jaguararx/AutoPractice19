using System;

using OpenQA.Selenium;

namespace COE.Core.Resilience
{
    public class AngularPageReadyProvider : IPageReadyProvider
    {
        public Func<IWebDriver, bool> PageIsReady() => JsExpectedConditions.AngularIsReady();
    }
}