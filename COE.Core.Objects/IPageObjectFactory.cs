using System;
using System.Collections.Generic;

using COE.Core.Objects.Components;
using COE.Core.Objects.Widgets;

using OpenQA.Selenium;

namespace COE.Core.Objects
{
    public interface IPageObjectFactory
    {
        T CreateComponent<T>(By locator) where T : WebComponent;
        T CreateComponent<T>(By locator, EventHandler<EventArgs> handler) where T : WebComponent;
        T CreatePageObject<T>(By locator) where T : PageObject;
        T CreatePageObject<T>(By locator, EventHandler<EventArgs> handler) where T : PageObject;
        T CreatePageObject<T>() where T : PageObject;
        List<T> CreatePageObjects<T>(By itemsLocator, Action<InitializerSettings> initializerSettings) where T : PageObject;
        List<T> CreateComponents<T>(By itemsLocator, Action<InitializerSettings> initializerSettings) where T : WebComponent;
    }
}