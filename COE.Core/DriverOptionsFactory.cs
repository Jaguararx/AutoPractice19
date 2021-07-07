using System;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace COE.Core
{
    public class DriverOptionsFactory
    {
        private readonly WebDriverSettings _settings;

        public DriverOptionsFactory(WebDriverSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public ChromeOptions CreateChromeOptions()
        {
            var options = new ChromeOptions();
            options.SetLoggingPreference(LogType.Browser, LogLevel.All);
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument($"--lang={_settings.BrowserLanguage}");

            if (_settings.UseHeadless)
            {
                options.AddArgument("headless");
                options.AddArgument("--window-size=1920,1080");
            }
            return options;
        }

        public FirefoxOptions CreateFirefoxOptions()
        {
            var options = new FirefoxOptions();
            options.SetLoggingPreference(LogType.Browser, LogLevel.All);
            options.AcceptInsecureCertificates = true;
            return options;
        }

        public ChromeOptions CreateHeadlessChromeOptions()
        {
            var options = CreateChromeOptions();
            options.AddArgument("headless");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--lang=en-US");
            options.AddArgument("--no-sandbox");
            return options;
        }

        public DriverOptions CreateOptions()
        {
            switch (_settings.BrowserType)
            {
                case BrowserType.Chrome:
                    return CreateChromeOptions();
                case BrowserType.ChromeHeadless:
                    return CreateHeadlessChromeOptions();
                case BrowserType.Firefox:
                    return CreateFirefoxOptions();
                default:
                    throw new ArgumentOutOfRangeException(nameof(_settings.BrowserType), _settings.BrowserType, null);
            }
        }
    }
}