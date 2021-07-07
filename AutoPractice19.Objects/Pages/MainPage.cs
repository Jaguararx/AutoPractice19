using System;
using System.Collections.Generic;
using COE.Core.Objects;
using COE.Core.Objects.Components;
using COE.Core.Objects.Widgets;
using COE.Core.Resilience;
using COE.Example.Objects.Pages.Widgets;
using OpenQA.Selenium;

namespace COE.Example.Objects.Pages
{
    public class MainPage : PageObject
    {
        public static readonly By HeaderLocator = By.CssSelector(".page-header h1");
        public static readonly By DescriptionLocator = By.CssSelector(".md-content");
        public static readonly By DropdownLocator = By.CssSelector("#dropdown");
        public static readonly By ListItemsLocator = By.CssSelector("#ul1 li");


        public MainPage(IWaiter waiter, IPageReadyProvider pageReadyProvider, IPageObjectFactory factory): base (waiter, pageReadyProvider, factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            Header = factory.CreateComponent<TextOnlyComponent>(HeaderLocator);
            Description = factory.CreateComponent<TextOnlyComponent>(DescriptionLocator);
            Navigation = factory.CreatePageObject<LinkMenu>();
            DropDown = factory.CreatePageObject<DropDown>(DropdownLocator);
            ListItems = factory.CreateComponents<TextOnlyComponent>(ListItemsLocator, (settings) =>
            {
                settings.InitializerStrategy = InitializerStrategy.NthChild;
                settings.ThrowOnItemsNotFound = true;
            });
        }

        public TextOnlyComponent Header { get; }
        public TextOnlyComponent Description { get; }
        public LinkMenu Navigation { get; }
        public DropDown DropDown { get; }

        public List<TextOnlyComponent> ListItems { get;  }
    }
}