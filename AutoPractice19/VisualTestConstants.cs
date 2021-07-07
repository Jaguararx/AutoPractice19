using System.Drawing;
using OpenQA.Selenium;

namespace COE.Example.Tests
{
    public static class VisualTestConstants
    {
        #region Global ignore regions
        /// <summary>
        /// Selector to add an ignore region for the embedded video on the homepage
        /// </summary>
        public static readonly By HomePageMenu = By.CssSelector(".navbar");

        public static readonly By InvisibleTextBlock = By.XPath("//div[@class='demo_block'][1]");
        

        public static readonly By[] GlobalIgnoreRegions = { HomePageMenu , InvisibleTextBlock };
        #endregion

        public static readonly Size DefaultWindowSize = new Size(1920, 1080);
    }
}