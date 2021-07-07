using System;

using COE.Core.Resilience;

using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace COE.Core.Objects.Components
{
    public class InputComponent : WebComponent
    {
        public InputComponent(IWebDriver driver, IWaiter waiter, Actions actionsBuilder, By locator) : base(driver, waiter, actionsBuilder, locator)
        {
        }

        public InputComponent(IWebDriver driver, IWaiter waiter, Actions actionsBuilder, By locator, EventHandler<EventArgs> inputSentHandler) : base(driver, waiter,
            actionsBuilder, locator)
        {
            InputSent += inputSentHandler;
        }

        public event EventHandler<EventArgs> InputSent;

        public void Clear()
        {
            WaitForComponentStateToMatch(ComponentState.Exists, true);
            Execute(() => { WrappedElement.Clear(); }, DefaultStrategy);
        }

        public string GetValue()
        {
            WaitForComponentStateToMatch(ComponentState.Exists, true);
            return Execute(() => WrappedElement.GetAttribute("value"), DefaultStrategy);
        }

        public void OnInputSent(EventArgs e)
        {
            InputSent?.Invoke(this, e);
        }

        public void SendKeys(string text, bool clearContent = true)
        {
            WaitForComponentStateToMatch(ComponentState.Enabled, true);

            if (clearContent)
            {
                Clear();
            }

            Execute(() => { WrappedElement.SendKeys(text); }, DefaultStrategy);
            OnInputSent(new EventArgs());
        }
    }
}