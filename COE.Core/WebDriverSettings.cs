using System.IO;
using System.Reflection;

namespace COE.Core
{
    public class WebDriverSettings
    {
        public BrowserType BrowserType { get; set; }
        public string PageUrl { get; set; }
        public string SeleniumDriversPath { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public string SeleniumServerUrl { get; set; }
        public bool UseRemoteDriver { get; set; }
        public bool UseHeadless { get; set; }
        public string BrowserLanguage { get; set; } = "en-US";
        public bool MaximiseWindow { get; set; } = true;
    }
}