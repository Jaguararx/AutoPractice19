using System;

using COE.Core.Resilience;

using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    // Base abstraction for any logical grouping of WebComponents on a page which combine to perform a common functionality 
    // For example, dropdown menus, pickers, linked button groups and other related controls.
    public abstract class WebWidget
    {
        protected readonly IPageObjectFactory _factory;
        protected readonly IPageReadyProvider _pageReadyProvider;
        protected readonly IWaiter _waiter;

        protected WebWidget(IWaiter waiter, IPageReadyProvider pageReadyProvider, IPageObjectFactory factory)
        {
            _waiter = waiter ?? throw new ArgumentNullException(nameof(waiter));
            _pageReadyProvider = pageReadyProvider ?? throw new ArgumentNullException(nameof(pageReadyProvider));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        protected virtual void WaitForPageReadyOperations()
        {
            try
            {
                _waiter.Wait.Until(_pageReadyProvider.PageIsReady());
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new WebDriverTimeoutException("Operation timed out waiting for page load", ex);
            }
        }
    }
}