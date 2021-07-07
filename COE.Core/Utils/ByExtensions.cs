using System.Text.RegularExpressions;

using OpenQA.Selenium;

namespace COE.Core.Utils
{
    public static class ByExtensions
    {
        /// <summary>
        /// Gets the string representation of this By's locator with the "By.LocatorStrategy:" prefix removed.
        /// </summary>
        public static string GetDescription(this By by) => Regex.Replace(by.ToString(), @"By\.\w+:", "").Trim();

        /// <summary>
        /// Gets the string representation of this By's locator strategy prefix with it's selector value removed.
        /// </summary>
        public static string GetStrategy(this By by) => Regex.Match(by.ToString(), @"By\.\w+:").Value;

    }
}