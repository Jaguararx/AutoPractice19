using System;
using Azure.Storage.Blobs;
using BoDi;
using COE.Core;
using COE.Core.Helpers;
using COE.Core.Objects;
using COE.Core.Resilience;
using COE.Core.Visual;
using COE.Example.Objects.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace COE.Example.Tests
{
    public static class ObjectContainerExtensions
    {
        public static IObjectContainer AddActionsBuilder(this IObjectContainer container)
        {
            container.RegisterFactoryAs(c => new Actions(container.Resolve<IWebDriver>()));
            return container;
        }

        public static IObjectContainer AddConfiguration(this IObjectContainer container, Action<TestSettings> configureSettings, out TestSettings settings)
        {
            settings = new TestSettings();
            configureSettings?.Invoke(settings);
            container.RegisterInstanceAs(settings);
            return container;
        }

        public static IObjectContainer AddPageObjectFactory(this IObjectContainer container)
        {
            container.RegisterInstanceAs<IPageObjectFactory>(new PageObjectFactory(container.Resolve<IWebDriver>(), container.Resolve<IWaiter>(),
                container.Resolve<IPageReadyProvider>(),
                container.Resolve<Actions>()));
            return container;
        }

        public static IObjectContainer AddPageReadyProvider(this IObjectContainer container)
        {
            container.RegisterInstanceAs<IPageReadyProvider>(new AngularPageReadyProvider());
            return container;
        }

        public static IObjectContainer AddWebDriver(this IObjectContainer container)
        {
            var settings = container.Resolve<TestSettings>();
            var webDriver = new WebDriverFactory(settings.WebDriverSettings, new DriverOptionsFactory(settings.WebDriverSettings)).CreateDriver();
            container.RegisterInstanceAs<IWebDriver>(webDriver, dispose: true);
            container.ConfigureWebDriver();
            return container;
        }

        public static IObjectContainer AddWebWaiter(this IObjectContainer container)
        {
            container.RegisterFactoryAs<IWaiter>(c => new WebWaiter(container.Resolve<IWebDriver>(), TimeSpan.FromMilliseconds(50)));
            return container;
        }

        public static IObjectContainer AddSqlHelper(this IObjectContainer container)
        {
            var settings = container.Resolve<TestSettings>();
            var sqlHelper = new SqlHelper(settings.StorageSettings.SqlConnectionString);
            container.RegisterInstanceAs<SqlHelper>(sqlHelper, dispose: true);
            return container;
        }

        private static IObjectContainer ConfigureWebDriver(this IObjectContainer container)
        {
            var settings = container.Resolve<TestSettings>();
            var driver = container.Resolve<IWebDriver>();

            if (settings.WebDriverSettings.MaximiseWindow)
            {
                driver.Manage().Window.Maximize();
            }

            return container;
        }

        public static IObjectContainer AddVisualDriver(this IObjectContainer container, TestSettings settings)
        {
            if (settings.VisualTest)
            {
                var driver = container.Resolve<IWebDriver>();
                var imageProcessor = new ImageDifferenceProcessor();
                IImageStore imageStore;
                if (settings.VisualSettings.UseLocalStorage)
                {
                    imageStore = new LocalDriveImageStore(settings.VisualSettings.LocalStoragePath);
                }
                else
                {
                    var blobServiceClient = new BlobServiceClient(settings.VisualSettings.ImageStorageConnectionString);
                    imageStore = new CloudImageStore(blobServiceClient);
                    container.RegisterInstanceAs(blobServiceClient);
                }
                var resultFactory = new VisualResultFactory(imageProcessor, imageStore);
                var visualDriver = new VisualDriver(driver, imageProcessor, imageStore, resultFactory, settings.VisualSettings);

                container.RegisterInstanceAs(imageStore);
                container.RegisterInstanceAs<IImageDifferenceProcessor>(imageProcessor);
                container.RegisterInstanceAs<IVisualDriver>(visualDriver);
            }
            else
            {
                container.AddNullVisualDriver();
            }

            return container;
        }

        public static IObjectContainer AddNullVisualDriver(this IObjectContainer container)
        {
            container.RegisterInstanceAs<IVisualDriver>(new NullVisualDriver());
            return container;
        }

        public static IObjectContainer AddMainPage(this IObjectContainer container)
        {
            var factory = container.Resolve<PageObjectFactory>();
            container.RegisterInstanceAs(factory.CreatePageObject<MainPage>());
            return container;
        }
    }
}
