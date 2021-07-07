using System;

using COE.Core.Resilience;
using COE.Core.Utils;
using COE.Core.Objects.Components;

using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    public class RadioButton : PageObject
    {
        /// <summary>
        /// The root or parent element for the radio button. This must have an input and label tag as children.
        /// </summary>
        private readonly By _rootElementLocator;

        public RadioButton(IWaiter waiter, IPageReadyProvider pageReadyProvider, IPageObjectFactory factory, By rootElementLocator) : base(waiter,
            pageReadyProvider, factory)
        {
            _rootElementLocator = rootElementLocator ?? throw new ArgumentNullException(nameof(rootElementLocator));

            Initialize();
        }

        public ClickableComponent ClickArea { get; private set; }
        public InputComponent Input { get; private set; }

        public string Label => ClickArea.Text;

        public void Click() => ClickArea.Click();

        public bool IsSelected() => Input.Selected;

        /// <summary>
        /// Initializes the components for this widget
        /// </summary>
        private void Initialize()
        {
            var inputSelector = _rootElementLocator.GetDescription() + " input";

            ClickArea = _factory.CreateComponent<ClickableComponent>(_rootElementLocator, (o, e) => WaitForPageReadyOperations());
            Input = _factory.CreateComponent<InputComponent>(By.CssSelector(inputSelector));
        }
    }
}