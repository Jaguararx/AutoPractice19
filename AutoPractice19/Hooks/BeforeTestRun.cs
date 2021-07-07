using BoDi;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;

namespace COE.Example.Tests.Hooks
{
    [Binding]
    internal static class BeforeTestRun
    {
        [BeforeTestRun]
        public static void RegisterDependencies(IObjectContainer container)
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile("appsettings.development.json", optional: true)
                                .Build();

            container.AddConfiguration(configureSettings =>
            {
                configuration.GetSection("TestSettings").Bind(configureSettings);
            }, out var settings);

            container.AddWebDriver();
            container.AddWebWaiter();
            container.AddPageReadyProvider();
            container.AddActionsBuilder();
            container.AddPageObjectFactory();
            container.AddSqlHelper();
            container.AddVisualDriver(settings);
            container.AddMainPage();
        }
    }
}
