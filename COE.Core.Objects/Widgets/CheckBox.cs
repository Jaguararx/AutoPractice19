using System;

using COE.Core.Resilience;
using COE.Core.Utils;
using COE.Core.Objects.Components;

using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    public class CheckBox : PageObject
    {
        private readonly By _rootElementLocator;

        public CheckBox(IWaiter waiter,
                        IPageReadyProvider pageReadyProvider,
                        IPageObjectFactory factory,
                        By rootElementLocator) :
            base(waiter, pageReadyProvider, factory)
        {
            _rootElementLocator = rootElementLocator ?? throw new ArgumentNullException(nameof(rootElementLocator));

            Button = _factory.CreateComponent<ClickableComponent>(LabelLocator, (sender, args) => WaitForPageReadyOperations());
            Label = _factory.CreateComponent<TextOnlyComponent>(LabelLocator);
            Input = _factory.CreateComponent<InputComponent>(InputLocator);
        }

        public TextOnlyComponent Label { get; }

        private ClickableComponent Button { get; }

        private InputComponent Input { get; }

        private By InputLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} input");
        private By LabelLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} label");

        private void Click() => Button.DynamicClick();

        public bool IsChecked() => Input.Selected;

        public void Toggle(bool enable)
        {
            if (IsChecked() != enable)
            {
                Click();

                _waiter.Wait.Until(d =>
                {
                    return IsChecked() == enable;
                });
            }
        }
    }
}