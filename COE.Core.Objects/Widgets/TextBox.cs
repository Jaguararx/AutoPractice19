using System;

using COE.Core.Resilience;
using COE.Core.Utils;
using COE.Core.Objects.Components;

using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    public class TextBox : PageObject
    {
        private readonly By _rootElementLocator;

        public TextBox(IWaiter waiter, IPageReadyProvider pageReadyProvider, IPageObjectFactory factory, By rootElementLocator) : base(waiter,
            pageReadyProvider, factory)
        {
            _rootElementLocator = rootElementLocator ?? throw new ArgumentNullException(nameof(rootElementLocator));

            ClickArea = _factory.CreateComponent<ClickableComponent>(_rootElementLocator, (sender, args) => WaitForPageReadyOperations());
            Input = _factory.CreateComponent<InputComponent>(InputLocator);
            Hint = _factory.CreateComponent<TextOnlyComponent>(HintLocator);
        }

        public TextBox(IWaiter waiter,
                       IPageReadyProvider pageReadyProvider,
                       PageObjectFactory factory,
                       EventHandler<EventArgs> inputSentHandler,
                       By rootElementLocator) : base(waiter,
            pageReadyProvider, factory)
        {
            _rootElementLocator = rootElementLocator ?? throw new ArgumentNullException(nameof(rootElementLocator));

            Input = _factory.CreateComponent<InputComponent>(InputLocator, inputSentHandler);
            ClickArea = _factory.CreateComponent<ClickableComponent>(_rootElementLocator, (sender, args) => WaitForPageReadyOperations());
            Hint = _factory.CreateComponent<TextOnlyComponent>(HintLocator);
        }

        public ClickableComponent ClickArea { get; private set; }
        public TextOnlyComponent Hint { get; private set; }
        public InputComponent Input { get; }

        /// <summary>
        /// Checks if a corresponding text input field currently exists in the DOM for this object
        /// </summary>
        public bool IsInitialized => Input.WaitForComponentStateToMatch(ComponentState.Exists,
            new DefaultRetryStrategy { MaxRetryDuration = TimeSpan.FromSeconds(2), ThrowOnRetryTimeout = false });

        private By HintLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} .hint");
        private By InputLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} input");

        public void Clear() => Input.Clear();

        public string GetValue() => Input.GetValue();

        public void SendKeys(string text) => Input.SendKeys(text);
    }
}