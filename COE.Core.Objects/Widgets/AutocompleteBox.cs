using System;
using System.Collections.Generic;
using System.Linq;

using COE.Core.Resilience;
using COE.Core.Utils;
using COE.Core.Objects.Components;

using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    public class AutocompleteBox : PageObject
    {
        /// <summary>
        /// The root element locator for this widget
        /// </summary>
        private readonly By _rootElementLocator;

        private readonly TextBox _textBox;

        public AutocompleteBox(IWaiter waiter,
                               IPageReadyProvider pageReadyProvider,
                               IPageObjectFactory factory,
                               By rootElementLocator)
            : base(waiter, pageReadyProvider, factory)
        {
            _rootElementLocator = rootElementLocator ?? throw new ArgumentNullException(nameof(rootElementLocator));
            _textBox = _factory.CreatePageObject<TextBox>(rootElementLocator, (o, e) => InitializeOptions());
        }

        /// <summary>
        /// Checks if a corresponding autocomplete field currently exists in the DOM for this object
        /// </summary>
        public bool IsInitialized => _textBox.IsInitialized;

        public bool Displayed => _textBox.Input.Displayed;

        public string SelectedOption => _textBox.GetValue();
        private List<ClickableComponent> Options { get; set; }
        private By OptionsLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} li");

        public void EnterSearchValue(string text)
        {
            _textBox.ClickArea.WaitForComponentStateToMatch(ComponentState.Exists);
            _textBox.ClickArea.DynamicClick();
            _textBox.Input.Clear();
            _textBox.Input.SendKeys(text);
            WaitForPageReadyOperations();
        }

        public void SearchAndSelectByValue(string text)
        {
            EnterSearchValue(text);
            SelectByValue(text);
        }

        public void SelectByValue(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text), "target text for Select must not be null or only whitespace");
            }

            ClickableComponent option;

            _waiter.ShortWait.Until(d =>
            {
                InitializeOptions();
                option = Options.FirstOrDefault(b => b.Text == text);
                option?.Click();
                return option != null;
            }, "Cannot locate element with text: " + text);
        }

        public bool OptionsExistAndAreNotEmpty()
        {
            InitializeOptions();
            return (Options.Count > 0);
        }

        private void InitializeOptions() => Options = _factory.CreateComponents<ClickableComponent>(OptionsLocator, settings =>
        {
            settings.InitializerStrategy = InitializerStrategy.NthChild;
            settings.ThrowOnItemsNotFound = false;
            settings.Handler = (o, e) => WaitForPageReadyOperations();
        });
    }
}