using System;
using COE.Core.Objects;
using COE.Core.Objects.Components;
using COE.Core.Objects.Widgets;
using COE.Core.Resilience;
using OpenQA.Selenium;

namespace COE.Example.Objects.Pages.Widgets
{
    public class LinkMenu : PageObject
    {
        public static readonly By HomeLinkLocator = By.CssSelector("#brand-title");
        public static readonly By CalculationLinkLocator = By.XPath("//a[@href='#/calculations']");
        public static readonly By PlanLinkLocator = By.XPath("//a[@href='#/plans']");
        
        public LinkMenu(IWaiter waiter,
                           IPageReadyProvider pageReadyProvider,
                           IPageObjectFactory factory) : base(waiter, pageReadyProvider, factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            HomeLink = factory.CreateComponent<ClickableComponent>(HomeLinkLocator);
            CalculationLink = factory.CreateComponent<ClickableComponent>(CalculationLinkLocator);
            PlanLink = factory.CreateComponent<ClickableComponent>(PlanLinkLocator);
        }

        public ClickableComponent HomeLink { get;  }
        public ClickableComponent CalculationLink { get; }
        public ClickableComponent PlanLink { get; }
    }
}