using System;

using COE.Core.Resilience;
using COE.Core.Utils;
using COE.Core.Objects.Components;

using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    public class DatePicker : PageObject
    {
        private readonly InputComponent _dateInput;
        private readonly DateTimeCalendar _dateTimeCalendar;
        private readonly TextOnlyComponent _owlDateTime;

        /// <summary>
        /// The root element locator for this widget
        /// </summary>
        private readonly By _rootElementLocator;

        private readonly ClickableComponent _toggleOpen;

        /// <summary>
        /// Used to capture the current internal DateTime value of the DatePicker so we can correctly wait for a successful change
        /// </summary>
        private DateTime _currentDateTimeInternal;

        public DatePicker(IWaiter waiter,
                          IPageReadyProvider pageReadyProvider,
                          IPageObjectFactory factory,
                          By rootElementLocator) : base(waiter, pageReadyProvider, factory)
        {
            _rootElementLocator = rootElementLocator ?? throw new ArgumentNullException(nameof(rootElementLocator));
            _toggleOpen = factory.CreateComponent<ClickableComponent>(ToggleOpenLocator, (sender, args) => WaitForPageReadyOperations());
            _dateInput = factory.CreateComponent<InputComponent>(InputLocator);
            _owlDateTime = factory.CreateComponent<TextOnlyComponent>(OwlDateTimeLocator);
            _dateTimeCalendar = factory.CreatePageObject<DateTimeCalendar>();
            _dateTimeCalendar.ValueChanged += WaitForValueUpdate;
        }

        public string SelectedDate => _dateInput.GetAttribute("value");

        private By InputLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} input");

        private By OwlDateTimeLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} owl-date-time");

        private By ToggleOpenLocator => By.CssSelector($"{_rootElementLocator.GetDescription()} em");

        /// <summary>
        /// Enters the specified value into the datepicker's input field
        /// </summary>
        public void EnterDate(string date)
        {
            _dateInput.Clear();
            _dateInput.SendKeys(date);
        }

        /// <summary>
        /// Selects the specified day from the datepicker for the currently active month
        /// </summary>
        public void SelectDay(int day)
        {
            _toggleOpen.ScrollClick();
            _currentDateTimeInternal = CurrentDateTimeInternal();
            _dateTimeCalendar.SelectDay(day);
        }

        public void SelectTime(DateTime time)
        {
            _toggleOpen.ScrollClick();
            _currentDateTimeInternal = CurrentDateTimeInternal();
            _dateTimeCalendar.SelectTime(time);
        }

        /// <summary>
        /// Gets the internal DateTime value of the DatePicker component.
        /// Note: this is not the same as the value displayed in the field as there is a narrow delay when processing changes)
        /// </summary>
        private DateTime CurrentDateTimeInternal() => DateTime.Parse(_owlDateTime.GetAttribute("date-selected"));

        private void WaitForValueUpdate(object sender, EventArgs e)
        {
            _waiter.ShortWait.Until(d => { return _currentDateTimeInternal != CurrentDateTimeInternal(); },
                $"Action {nameof(WaitForValueUpdate)} timed out while waiting for the selected calendar value to be reflected in the parent control.");
        }
    }
}