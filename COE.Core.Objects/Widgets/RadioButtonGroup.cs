using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using COE.Core.Resilience;
using COE.Core.Utils;
using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    [ExcludeFromCodeCoverage]
    public class RadioButtonGroup : PageObject
    {
        public const string RadioButtonLocator = ".radio";
        private List<RadioButton> _radioButtons;

        public RadioButtonGroup(IWaiter waiter, IPageReadyProvider pageReadyProvider, IPageObjectFactory factory, By rootElementLocator) : base(waiter,
            pageReadyProvider, factory)
        {
            RootElementLocator = rootElementLocator ?? throw new ArgumentNullException(nameof(rootElementLocator));
        }

        public List<RadioButton> RadioButtons
        {
            get
            {
                if (_radioButtons == null || _radioButtons.Count == 0)
                {
                    Initialize();
                }
                return _radioButtons;
            }
        }

        private By ButtonsLocator => By.CssSelector($"{RootElementLocator.GetDescription()} {RadioButtonLocator}");

        /// <summary>
        /// The root element locator for the Radio Button group.
        /// </summary>
        private By RootElementLocator { get; }

        /// <summary>
        /// Gets a child radio button from the group using the specified label
        /// </summary>
        public RadioButton GetButton(string label)
        {
            WaitForPageReadyOperations();
            var targetButton = RadioButtons.FirstOrDefault(b => b.Label.StartsWith(label));

            _waiter.ShortWait.Until(d =>
            {
                Initialize();
                targetButton = RadioButtons.FirstOrDefault(b => b.Label.StartsWith(label));
                return targetButton != null;
            }, $"Unable to find Radio Button with specified label: {label}");

            return targetButton;
        }

        /// <summary>
        /// Returns a reference to the currently selected radio button in the DOM
        /// </summary>
        public RadioButton GetSelected()
        {
            var selectedButton = RadioButtons.FirstOrDefault(b => b.Input.Selected);

            if (selectedButton == null)
            {
                throw new NoSuchElementException("Unable to find a selected radio button in the group");
            }

            return selectedButton;
        }

        public bool IsFirstButton(RadioButton radio) => RadioButtons[0].Equals(radio);

        /// <summary>
        /// Clicks on a radio button from the list of radio buttons with the corresponding label
        /// </summary>
        public void SelectByLabel(string label) => GetButton(label).Click();
        private void Initialize() => _radioButtons = _factory.CreatePageObjects<RadioButton>(ButtonsLocator, (settings) =>
        {
            settings.InitializerStrategy = InitializerStrategy.NthChild;
            settings.ThrowOnItemsNotFound = true;
        });
    }
}