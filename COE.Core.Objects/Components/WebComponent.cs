using System;

using COE.Core.Resilience;

using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

using SeleniumExtras.WaitHelpers;

namespace COE.Core.Objects.Components
{
    /// <summary>
    /// Basic wrapper class for an IWebElement.
    /// Provides a more stable set of methods for interacting with an IWebElement by automatically handling common transient failures and exceptions without the need to write custom handling code.
    /// </summary>
    public abstract class WebComponent
    {
        protected readonly Actions ActionsBuilder;
        protected readonly RetryStrategy DefaultStrategy;
        protected IWebDriver _driver;
        protected IWaiter _waiter;
        private IWebElement _element;

        protected WebComponent(IWebDriver driver, IWaiter waiter, Actions actionsBuilder, By locator)
        {
            Locator = locator;
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _waiter = waiter ?? throw new ArgumentNullException(nameof(waiter));
            ActionsBuilder = actionsBuilder;
            DefaultStrategy = new DefaultRetryStrategy();
        }

        public bool Displayed => Execute(() => WrappedElement.Displayed, DefaultStrategy);
        public bool NotDisplayed => this.WaitForComponentStateToMatch(ComponentState.Hidden, DefaultStrategy);

        public By Locator { get; }

        public bool Selected => Execute(() => WrappedElement.Selected, DefaultStrategy);

        public string Text => Execute(() => WrappedElement.Text, DefaultStrategy);

        public IWebElement WrappedElement
        {
            get
            {
                if (_element == null)
                {
                    InitializeComponent();
                }
                return _element;
            }
        }

        protected IJavaScriptExecutor JsExecutor => (IJavaScriptExecutor)_driver;

        public string GetAttribute(string attribute) => Execute(() => WrappedElement.GetAttribute(attribute), DefaultStrategy);

        public void InitializeComponent()
        {
            var result = RetryAction.DefaultRetry(DefaultStrategy, () =>
            {
                try
                {
                    _element = _driver.FindElement(Locator);
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });

            if (!result)
            {
                throw new NoSuchElementException($"Unable to find element with locator '{Locator}' within the current page context");
            }
        }

        /// <summary>
        /// Performs a mouse-over action on this web component
        /// </summary>
        public void MouseOver()
        {
            WaitForComponentStateToMatch(ComponentState.Exists, throwOnTimeout: true);
            Execute(() => { ActionsBuilder.MoveToElement(WrappedElement).Perform(); }, DefaultStrategy);
        }

        /// <summary>
        /// Performs a waited check on the current state of the component, using the expected Component State specified as a param.
        /// Allows assertions to be performed which are resilient to potential race conditions.
        /// </summary>
        /// <param name="expectedState">The expected state that will be waited for this component</param>
        /// <param name="strategy">The retry strategy to be used for the wait operation</param>
        public bool WaitForComponentStateToMatch(ComponentState expectedState, RetryStrategy strategy, string text = "")
        {
            switch (expectedState)
            {
                case ComponentState.Exists:
                    return Execute(() =>
                    {
                        _waiter.CustomWait(strategy.MaxRetryDuration).Until(ExpectedConditions.ElementExists(Locator));
                        return true;
                    }, strategy);

                case ComponentState.DoesNotExist:
                    return Execute(() =>
                    {
                        _waiter.CustomWait(strategy.MaxRetryDuration).Until(d => d.FindElements(Locator).Count == 0);
                        return true;
                    }, strategy);

                case ComponentState.Visible:
                    return Execute(() =>
                    {
                        _waiter.CustomWait(strategy.MaxRetryDuration).Until(ExpectedConditions.ElementIsVisible(Locator));
                        return true;
                    }, strategy);

                case ComponentState.Hidden:
                    return Execute(() => _waiter.CustomWait(strategy.MaxRetryDuration).Until(ExpectedConditions.InvisibilityOfElementLocated(Locator)), strategy);

                case ComponentState.Enabled:
                    return Execute(() =>
                    {
                        _waiter.CustomWait(strategy.MaxRetryDuration).Until(ExpectedConditions.ElementToBeClickable(Locator));
                        return true;
                    }, strategy);
                case ComponentState.Disabled:
                    return Execute(() =>
                    {
                        _waiter.CustomWait(strategy.MaxRetryDuration)
                               .Until(d =>
                               {
                                   return !WrappedElement.Enabled
                                        || WrappedElement.GetAttribute("disabled") == "true"
                                        || WrappedElement.GetAttribute("class").Contains("disabled");
                               });
                        return true;
                    }, strategy);
                case ComponentState.TextToBePresent:
                    return Execute(() =>
                        {
                            try
                            {
                                return _waiter.CustomWait(strategy.MaxRetryDuration).Until(ExpectedConditions.TextToBePresentInElement(WrappedElement, text));
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"Expected text: {text}. Actual text: {WrappedElement.Text}", e);
                            }
                        }, strategy);
                default:
                    throw new ArgumentOutOfRangeException(nameof(expectedState), "An invalid Expected State was passed into the method");
            }
        }

        public bool WaitForComponentStateToMatch(ComponentState expectedState, bool throwOnTimeout = false)
        {
            var strategy = new DefaultRetryStrategy { ThrowOnRetryTimeout = throwOnTimeout };
            return WaitForComponentStateToMatch(expectedState, strategy);
        }

        protected void Execute(Action action, RetryStrategy strategy)
        {
            RetryAction.RetryWithExceptions<Exception>(strategy, () =>
            {
                try
                {
                    action.Invoke();
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    InitializeComponent();
                    return false;
                }
            });
        }

        protected T Execute<T>(Func<T> function, RetryStrategy strategy)
        {
            T result = default;
            Execute(() => { result = function(); }, strategy);
            return result;
        }
    }
}