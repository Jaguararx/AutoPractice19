using System;
using System.Collections.Generic;

using COE.Core.Resilience;
using COE.Core.Utils;
using COE.Core.Objects.Components;
using COE.Core.Objects.Widgets;

using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace COE.Core.Objects
{
    public enum InitializerStrategy
    {
        NthChild,
        NthType
    }

    public class InitializerSettings
    {
        public EventHandler<EventArgs> Handler { get; set; }
        public InitializerStrategy InitializerStrategy { get; set; } = InitializerStrategy.NthChild;
        public bool ThrowOnItemsNotFound { get; set; } = true;
    }

    public class PageObjectFactory : IPageObjectFactory
    {
        private readonly Actions _actions;
        private readonly IWebDriver _driver;
        private readonly IPageReadyProvider _pageReadyProvider;
        private readonly IWaiter _waiter;
        private InitializerSettings _settings;

        public PageObjectFactory(IWebDriver driver, IWaiter waiter, IPageReadyProvider pageReadyProvider, Actions actions)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _waiter = waiter ?? throw new ArgumentNullException(nameof(waiter));
            _pageReadyProvider = pageReadyProvider ?? throw new ArgumentNullException(nameof(pageReadyProvider));
            _actions = actions ?? throw new ArgumentNullException(nameof(actions));
            _settings = new InitializerSettings();
        }


        private IEnumerable<string> FindElements(By itemsLocator)
        {
            var selectors = new List<string>();
            var itemsCount = 0;
            try
            {
                _waiter.ShortWait.Until(d =>
                {
                    itemsCount = d.FindElements(itemsLocator).Count;
                    return itemsCount > 0;
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                if (_settings.ThrowOnItemsNotFound)
                {
                    throw new NoSuchElementException($"Unable to find any elements for the current page context using locator: {itemsLocator}", ex);
                }
            }

            // Construct a unique selector for each found item found in the DOM
            for (var i = 1; i <= itemsCount; i++)
            {
                var selector = $"{itemsLocator.GetDescription()}{GetStrategySuffix(i)}";
                selectors.Add(selector);
            }

            return selectors;
        }

        private string GetStrategySuffix(int itemIndex) =>
            _settings.InitializerStrategy == InitializerStrategy.NthChild ? $":nth-child({itemIndex})" : $":nth-of-type({itemIndex})";

        public T CreatePageObject<T>() where T : PageObject
        {
            return (T)Activator.CreateInstance(typeof(T), _waiter, _pageReadyProvider, this);
        }

        public T CreatePageObject<T>(By locator) where T : PageObject
        {
            return (T)Activator.CreateInstance(typeof(T), _waiter, _pageReadyProvider, this, locator);
        }

        public T CreatePageObject<T>(By locator, EventHandler<EventArgs> handler) where T : PageObject
        {
            return (T)Activator.CreateInstance(typeof(T), _waiter, _pageReadyProvider, this, handler, locator);
        }

        public T CreateComponent<T>(By locator) where T : WebComponent
        {
            return (T)Activator.CreateInstance(typeof(T), _driver, _waiter, _actions, locator);
        }

        public T CreateComponent<T>(By locator, EventHandler<EventArgs> handler) where T : WebComponent
        {
            return (T)Activator.CreateInstance(typeof(T), _driver, _waiter, _actions, locator, handler);
        }

        public List<T> CreatePageObjects<T>(By itemsLocator, Action<InitializerSettings> initializerSettings) where T : PageObject
        {
            _settings = new InitializerSettings();
            initializerSettings?.Invoke(_settings);

            var selectors = FindElements(itemsLocator);
            var items = new List<T>();

            // Initialize an item of T for each found item found in the DOM
            foreach (var selector in selectors)
            {
                items.Add(_settings.Handler == null ? CreatePageObject<T>(By.CssSelector(selector)) : CreatePageObject<T>(By.CssSelector(selector), _settings.Handler));
            }

            return items;
        }

        public List<T> CreateComponents<T>(By itemsLocator, Action<InitializerSettings> initializerSettings) where T : WebComponent
        {
            _settings = new InitializerSettings();
            initializerSettings?.Invoke(_settings);

            var selectors = FindElements(itemsLocator);
            var items = new List<T>();

            // Initialize an item of T for each found item found in the DOM
            foreach (var selector in selectors)
            {
                items.Add(_settings.Handler == null ? CreateComponent<T>(By.CssSelector(selector)) : CreateComponent<T>(By.CssSelector(selector), _settings.Handler));
            }

            return items;
        }
    }
}