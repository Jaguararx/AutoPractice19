using System;

using COE.Core.Resilience;
using COE.Core.Objects.Components;

using OpenQA.Selenium;

namespace COE.Core.Objects.Widgets
{
    public class DateTimeCalendar : PageObject
    {
        private readonly InputComponent _hourInput;
        private readonly InputComponent _minuteInput;

        private readonly ClickableComponent _setButton;

        public DateTimeCalendar(IWaiter waiter,
                                IPageReadyProvider pageReadyProvider,
                                IPageObjectFactory factory) : base(waiter, pageReadyProvider, factory)
        {
            _hourInput = factory.CreateComponent<InputComponent>(HourInputLocator, (s, a) => WaitForPageReadyOperations());
            _minuteInput = factory.CreateComponent<InputComponent>(MinuteInputLocator, (s, a) => WaitForPageReadyOperations());
            _setButton = _factory.CreateComponent<ClickableComponent>(SetButtonLocator, (o, e) => WaitForPageReadyOperations());
        }

        public event EventHandler<EventArgs> ValueChanged;

        private By HourInputLocator => By.CssSelector("owl-date-time-timer-box:nth-child(1) input");
        private By MinuteInputLocator => By.CssSelector("owl-date-time-timer-box:nth-child(2) input");
        private By SetButtonLocator => By.CssSelector(".owl-dt-container-buttons button:nth-child(2)");

        /// <summary>
        /// Selects the specified day from the datepicker for the currently active month
        /// </summary>
        public void SelectDay(int day)
        {
            try
            {
                _waiter.Wait.Until(d =>
                    d.FindElements(DayLocator(day)));
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new NoSuchElementException($"Unable to find a matching day on the datepicker calendar using locator: {DayLocator(day)}", ex);
            }

            var dayButton = _factory.CreateComponent<ClickableComponent>(DayLocator(day), (o, e) => WaitForPageReadyOperations());

            dayButton.DynamicClick();
            _setButton.DynamicClick();
        }

        public void SelectTime(DateTime time)
        {
            var currentHour = _hourInput.GetAttribute("value");
            var currentMinute = _minuteInput.GetAttribute("value");

            if (currentMinute == time.Minute.ToString() && currentHour == time.Hour.ToString())
            {
                return;
            }

            _hourInput.SendKeys(time.Hour.ToString());
            _minuteInput.SendKeys(time.Minute.ToString());

            OnValueChange(new EventArgs());

            _setButton.DynamicClick();
        }

        private void OnValueChange(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Locator for each day cell in the calendar.
        /// Ugly but probably the best we can do here as it's from a third party library
        /// </summary>
        /// <param name="day">The target day to select</param>
        private static By DayLocator(int day) =>
            By.XPath($"//*[contains(@class, 'owl-dt-calendar-cell-content') and not(contains(@class,'owl-dt-calendar-cell-out'))][contains(text(), '{day}')]");
    }
}