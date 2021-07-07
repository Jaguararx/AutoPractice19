using System;

using Microsoft.Extensions.DependencyInjection;

namespace COE.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebDriver(this IServiceCollection services, Action<WebDriverSettings> configure)
        {
            return services.AddSingleton(s =>
                           {
                               var options = new WebDriverSettings();
                               configure?.Invoke(options);
                               return options;
                           })
                           .AddSingleton<DriverOptionsFactory>()
                           .AddSingleton<WebDriverFactory>()
                           .AddSingleton(s => s.GetRequiredService<WebDriverFactory>().CreateDriver());
        }
    }
}