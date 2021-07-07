using System;
using COE.Core.Resilience;
using COE.Core.Objects.Components;

namespace COE.Core.Objects.Widgets
{
    // Base abstraction for any logical grouping of WebComponents on a page which combine to perform a common functionality 
    // For example, dropdown menus, pickers, linked button groups and other related controls.
    public abstract class PageObject
    {
        protected readonly IPageObjectFactory _factory;
        protected readonly IPageReadyProvider _pageReadyProvider;
        protected readonly IWaiter _waiter;

        protected PageObject(IWaiter waiter, IPageReadyProvider pageReadyProvider, IPageObjectFactory factory)
        {
            _waiter = waiter ?? throw new ArgumentNullException(nameof(waiter));
            _pageReadyProvider = pageReadyProvider ?? throw new ArgumentNullException(nameof(pageReadyProvider));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        protected virtual void WaitForPageReadyOperations() => _waiter.Wait.Until(_pageReadyProvider.PageIsReady(), "Timed out while waiting for page ready state");

        public WebComponent GetComponentProperty(string property)
        {
            return (WebComponent)this.GetType().GetProperty(property).GetValue(this, null);
        }
    }
}