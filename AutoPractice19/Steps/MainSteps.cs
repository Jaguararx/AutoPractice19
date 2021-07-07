using BoDi;
using COE.Example.Objects.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;

namespace COE.Example.Tests.Steps
{
    [Binding]
    public sealed class MainSteps
    {
        private readonly IWebDriver _driver;
        private readonly TestContext _context;
        private readonly MainPage _mainPge;
        private readonly IObjectContainer _container;
        public MainSteps(IWebDriver driver, TestContext context, MainPage mainPage, IObjectContainer container)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mainPge = mainPage ?? throw new ArgumentNullException(nameof(mainPage));
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        [Given(@"a driver is created")]
        public void GivenADriverIsCreated()
        {
            if (_driver == null) {
                _container.AddWebDriver();
            }
        }

        [When(@"the main page is loaded")]
        public void WhenTheMainPageIsLoaded()
        {
            _driver.Navigate().GoToUrl(_context.WebDriverSettings.PageUrl.ToLower());
        }

        [Then(@"the main page have a title")]
        public void ThenTheMainPageHaveATitle()
        {
            Assert.That(_driver.Title, Is.Not.Empty);
        }

        [Then(@"the title is equal to '(.*)'")]
        public void ThenTheTitleIsEqualTo(string title)
        {
            Assert.That(_driver.Title, Is.EqualTo(title));
        }

        [Then(@"the main page have a header")]
        public void ThenTheMainPageHaveAHeader()
        {
            Assert.IsTrue(_mainPge.Header.Displayed);
        }

        [Then(@"the header is equal to '(.*)'")]
        public void ThenTheHeaderIsEqualTo(string header)
        {
            Assert.That(_mainPge.Header.Text, Is.EqualTo(header));
        }

        [Then(@"the main page have a description")]
        public void ThenTheMainPageHaveADescription()
        {
            Assert.IsTrue(_mainPge.Description.Displayed);
        }

        [Then(@"the description contains '(.*)'")]
        public void ThenTheDescriptionContains(string descriptionPart)
        {
            Assert.That(_mainPge.Description.Text, Contains.Substring(descriptionPart));
        }

        [When(@"user select '(.*)' in the dropdown")]
        public void WhenUserSelectInTheDropdown(string option)
        {
            _mainPge.DropDown.SelectByValue(option);
        }

        [Then(@"dropdown has value '(.*)'")]
        public void ThenDropdownHasValue(string option)
        {
            Assert.AreEqual(option, _mainPge.DropDown.SelectedItem);
        }

        [Then(@"list item with index '(.*)' has value '(.*)'")]
        public void ThenListItemWithIndexHasValue(int index, string value)
        {
            Assert.AreEqual(value, _mainPge.ListItems[index].Text);
        }

    }
}
