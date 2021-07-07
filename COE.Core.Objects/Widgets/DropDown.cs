using System;
using System.Collections.Generic;
using System.Linq;
using COE.Core.Resilience;
using COE.Core.Utils;
using COE.Core.Objects.Components;
using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    public class DropDown : PageObject
    {
        private readonly By _rootElementLocator;
        private readonly ClickableComponent _toggleOpen;
        private readonly InputComponent _input;

        public DropDown(IWaiter waiter,
                  IPageReadyProvider pageReadyProvider,
                  IPageObjectFactory factory,
                  By rootElementLocator) : base(waiter, pageReadyProvider, factory)
        {
            _rootElementLocator = rootElementLocator ?? throw new ArgumentNullException(nameof(rootElementLocator));

            _toggleOpen = _factory.CreateComponent<ClickableComponent>(_rootElementLocator, (sender, args) => InitializeOptions());
            _input = _factory.CreateComponent<InputComponent>(InputLocator);
        }

        private List<ClickableComponent> Options { get; set; }

        public string SelectedItem => _input.GetAttribute("aria-label");
        private By InputLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} input");
        private By OptionsLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} excl-dropdown-item");

        private void InitializeOptions() => Options = _factory.CreateComponents<ClickableComponent>(OptionsLocator, (settings) =>
        {
            WaitForPageReadyOperations();
            _waiter.ShortWait.Until(JsExpectedConditions.IsElementLocationStable(OptionsLocator));
            settings.InitializerStrategy = InitializerStrategy.NthChild;
            settings.ThrowOnItemsNotFound = true;
            settings.Handler = (sender, args) => WaitForPageReadyOperations();
        });

        public void SelectByIndex(int index)
        {
            _toggleOpen.Click();

            if (Options.ElementAtOrDefault(index) == null)
            {
                throw new IndexOutOfRangeException($"Option does not exist at the specified index: {index} ");
            }

            Options[index].ScrollClick();
        }

        public void SelectByValue(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text), "target text for Select must not be null or only whitespace");
            }

            _toggleOpen.Click();

            var option = GetOption(text);

            if (option == null)
            {
                throw new NoSuchElementException("Cannot locate element with text: " + text);
            }

            option.Click();
            WaitForPageReadyOperations();
        }

        public IEnumerable<string> GetAvailableOptions()
        {
            _toggleOpen.Click();
            WaitForPageReadyOperations();
            return Options.Select(x =>
            {
                x.WaitForComponentStateToMatch(ComponentState.Exists);
                return x.WrappedElement.Text;
            });
        }

        public virtual bool IsOptionDisabled(string optionText)
        {
            return GetOption(optionText).WrappedElement.FindElement(By.CssSelector("li")).GetAttribute("class").Contains("disabled");
        }

        private ClickableComponent GetOption(string optionText)
        {
            return Options.FirstOrDefault(b => b.GetAttribute("innerText") == optionText);
        }
    }
}
