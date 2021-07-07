using System;
using COE.Core.Resilience;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace COE.Core.Objects.Components
{
    public class TextOnlyComponent : WebComponent
    {
        public TextOnlyComponent(IWebDriver driver, IWaiter waiter, Actions actions, By locator) : base(driver, waiter, actions, locator)
        {
        }

        public TextOnlyComponent(IWebDriver driver, IWaiter waiter, Actions actionsBuilder, By locator, EventHandler<EventArgs> stateChangedHandler) : base(driver,
    waiter, actionsBuilder, locator)
        {
            StateChanged += stateChangedHandler;
        }

        public event EventHandler<EventArgs> StateChanged;
    }
}