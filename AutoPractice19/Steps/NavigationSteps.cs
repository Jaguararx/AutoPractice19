using COE.Core.Objects;
using COE.Example.Objects.Pages.Widgets;
using System;
using TechTalk.SpecFlow;

namespace COE.Example.Tests.Steps
{
    [Binding]
    public sealed class NavigationSteps
    {
        public LinkMenu Navigation { get; }
        public NavigationSteps(IPageObjectFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            Navigation = factory.CreatePageObject<LinkMenu>();
        }

        [Given(@"the user is on the Home page")]
        public void GivenTheUserIsOnTheHomePage()
        {
            Navigation.HomeLink.Click();
        }

        [Given(@"the calculation page is loaded")]
        public void GivenTheCalculationPageIsLoaded()
        {
            Navigation.CalculationLink.Click();
        }

        [When(@"the plan page is loaded")]
        public void GivenThePlanPageIsLoaded()
        {
            Navigation.PlanLink.Click();
        }


    }
}
