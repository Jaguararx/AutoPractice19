using System;

using COE.Core.Resilience;
using COE.Core.Utils;

using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace COE.Core.Objects.Components
{
    public class ClickableComponent : WebComponent
    {
        public ClickableComponent(IWebDriver driver, IWaiter waiter, Actions actionsBuilder, By locator) : base(driver, waiter, actionsBuilder, locator)
        {
        }

        public ClickableComponent(IWebDriver driver, IWaiter waiter, Actions actionsBuilder, By locator, EventHandler<EventArgs> stateChangedHandler) : base(driver,
            waiter, actionsBuilder, locator)
        {
            StateChanged += stateChangedHandler;
        }

        public event EventHandler<EventArgs> StateChanged;

        public void Click()
        {
            WaitForComponentStateToMatch(ComponentState.Enabled, throwOnTimeout: true);
            Execute(() => { WrappedElement.Click(); },
                new DefaultRetryStrategy { ThrowOnRetryTimeout = true });

            OnStateChanged(new EventArgs());
        }

        public void DragTo(WebComponent dropTarget, bool primeBeforeDrag = true)
        {
            WaitForComponentStateToMatch(ComponentState.Enabled, throwOnTimeout: true);
            Execute(() =>
                {
                    ActionsBuilder.ClickAndHold(WrappedElement).Build().Perform();
                    // INFO: Due to the way angular manipulates the DOM during a drag and drop,
                    // we need to prime it first by moving the element a short distance from its offset
                    // before completing the full action. Performing the whole action sequence in a single hit
                    // does not work.
                    if (primeBeforeDrag)
                        ActionsBuilder.MoveByOffset(5, 5).Build().Perform();
                    ActionsBuilder.DragAndDrop(WrappedElement, dropTarget.WrappedElement).Build().Perform();
                },
                new DefaultRetryStrategy { ThrowOnRetryTimeout = true });
        }

        /// <summary>
        /// Performs a click with extra handling for targets with animated transitions.
        /// For instances where the target element does not have a static location when first appearing in the DOM
        /// </summary>
        public void DynamicClick()
        {
            _waiter.Wait.Until(JsExpectedConditions.IsElementLocationStable(Locator));
            Click();
        }

        /// <summary>
        /// Performs a click with extra scroll handling for instances where the click target may be temporarily obstructed.
        /// Will attempt to scroll the page a short distance on each loop until the Click action is successful. 
        /// </summary>
        public void ScrollClick()
        {
            Execute(() =>
                {
                    WaitForComponentStateToMatch(ComponentState.Exists, throwOnTimeout: true);
                    JsExecutor.ExecuteScript($"document.querySelector('{Locator.GetDescription()}').scrollIntoView(true)");
                    try
                    {
                        WrappedElement.Click();
                    }
                    catch (ElementClickInterceptedException)
                    {
                        // INFO: Click target is still obstructed.
                        // Scroll the document a short distance to attempt to clear the obstructing element for the next retry.
                        JsExecutor.ExecuteScript(JavaScriptActions.ScrollBy(100), WrappedElement);
                    }
                },
                new DefaultRetryStrategy { ThrowOnRetryTimeout = true });

            OnStateChanged(new EventArgs());
        }

        protected virtual void OnStateChanged(EventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}