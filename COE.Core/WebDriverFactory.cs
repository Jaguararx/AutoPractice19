using System;
using System.IO;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace COE.Core
{
    public class WebDriverFactory
    {
        private readonly WebDriverSettings _config;
        private readonly DriverOptionsFactory _driverOptionsFactory;

        public WebDriverFactory(WebDriverSettings config, DriverOptionsFactory driverOptionsFactory)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _driverOptionsFactory = driverOptionsFactory ?? throw new ArgumentNullException(nameof(driverOptionsFactory));
        }

        public IWebDriver CreateDriver()
        {
            return _config.UseRemoteDriver ? CreateRemoteDriver() : CreateLocalDriver();
        }

        private IWebDriver CreateLocalDriver()
        {
            switch (_config.BrowserType)
            {
                case BrowserType.Chrome:
                case BrowserType.ChromeHeadless:
                    var options = _driverOptionsFactory.CreateOptions() as ChromeOptions;
                    var pathToDriver = GetDriverPath(typeof(ChromeDriver));
                    return new ChromeDriver(pathToDriver, options);
                default:
                    throw new NotImplementedException($"Support for the specified browser type {nameof(_config.BrowserType)} not yet implemented");
            }
        }

        private IWebDriver CreateRemoteDriver()
        {
            if (string.IsNullOrWhiteSpace(_config.SeleniumServerUrl))
            {
                throw new ArgumentException("For remote driver selenium server url is required.");
            }
            var remoteDriverOptions = _driverOptionsFactory.CreateOptions();
            return new RemoteWebDriver(new Uri(_config.SeleniumServerUrl), remoteDriverOptions);
        }

        private string GetDriverPath(Type driverType)
        {
            var nameOfDriver = driverType.Name;
            var pathFromConfig = Path.Combine(_config.SeleniumDriversPath, nameOfDriver + ".exe");
            var linuxPathFromConfig = Path.Combine(_config.SeleniumDriversPath, nameOfDriver.ToLowerInvariant());

            if (!string.IsNullOrEmpty(_config.SeleniumDriversPath) &&
                (File.Exists(pathFromConfig) ||
                 File.Exists(linuxPathFromConfig)))
            {
                return _config.SeleniumDriversPath;
            }

            throw new FileNotFoundException($"Path to {nameOfDriver} could not be resolved using path {pathFromConfig}");
        }
    }
}