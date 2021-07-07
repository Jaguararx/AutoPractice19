using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace COE.Core.Visual
{
    public class VisualDriver : IVisualDriver
    {
        private readonly List<TestResult> _allSessions = new List<TestResult>();
        private List<ComparisonResult> _currentSession;

        private readonly List<By> _globalIgnoreRegions = new List<By>();
        private readonly IImageDifferenceProcessor _imageProcessor;
        private readonly IImageStore _imageStore;
        private readonly VisualSettings _settings;
        private readonly IWebDriver _webDriver;
        private readonly VisualResultFactory _resultFactory;

        public VisualDriver(IWebDriver webDriver, IImageDifferenceProcessor imageProcessor, IImageStore imageStore, VisualResultFactory resultFactory, VisualSettings settings)
        {
            _webDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
            _imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
            _imageStore = imageStore ?? throw new ArgumentNullException(nameof(imageStore));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _resultFactory = resultFactory ?? throw new ArgumentNullException(nameof(resultFactory));
        }

        /// <summary>
        /// Adds regions to the specified image that will be ignored during visual comparison. 
        /// Regions are added based on the element selectors passed to the method
        /// </summary>
        /// <param name="image">The image to add ignore regions to</param>
        /// <param name="elementSelectors">Selectors to be used to map the ignore regions for the image</param>
        /// <param name="throwOnElementNotFound">Throws an exception if any element locators cannot resolve a page element</param>
        /// <returns>The image with ignore regions added to it</returns>
        private Image CoverDynamicElementsBySelector(Image image, List<By> elementSelectors, bool throwOnElementNotFound = false)
        {
            foreach (var selector in elementSelectors)
            {
                var elements = _webDriver.FindElements(selector);

                if (elements.Count == 0 && throwOnElementNotFound)
                {
                    throw new NoSuchElementException($"Unable to find Dynamic Element by specified selector: {selector}");
                }

                foreach (var element in elements)
                {
                    // Draw an ignore region on the image using the X, Y and Size properties of the element
                    image = _imageProcessor.DrawIgnoreRegion(image, element.Location.X, element.Location.Y, element.Size.Width, element.Size.Height);
                }
            }

            return image;
        }

        private Image GetScreenshotOfCurrentPage()
        {
            if (!(_webDriver is ITakesScreenshot takesScreenshot))
            {
                throw new WebDriverException("Unable to create capture of the current page. The supplied WebDriver instance does not implement ITakesScreenshot");
            }

            var screenshot = takesScreenshot.GetScreenshot();

            return _imageProcessor.ScreenshotToImage(screenshot);
        }

        private string ValidateAndSanitizeTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentException("Tag must be 1 or more characters", nameof(tag));
            }

            // Remove any invalid characters that we can't perform a save with
            // Remove any empty spaces and replace with underscore
            tag = tag.Sanitize().RemoveSpaces();

            // Re-check in case all characters were removed during sanitization
            if (tag == string.Empty)
            {
                throw new ArgumentException("Tag length must be 1 or more and must not contain invalid characters", nameof(tag));
            }

            return tag;
        }

        public void AddGlobalIgnoreRegions(params By[] ignoreRegions)
        {
            if (ignoreRegions.Length > 0)
            {
                _globalIgnoreRegions.AddRange(ignoreRegions);
            }
        }

        /// <summary>
        /// Triggers a visual comparison to be performed on the currently active page.
        /// </summary>
        /// <param name="tag">The tag to be used to associate with the comparison image</param>
        /// <param name="options">The comparision options</param>
        /// <returns>The comparison result</returns>
        public async Task<ComparisonResult> CheckWindowAsync(string tag, ComparisonOptions options = null)
        {
            Open();

            options ??= new ComparisonOptions();

            var allIgnoreRegions = new List<By>(_globalIgnoreRegions);

            if (options.IgnoreRegions != null && options.IgnoreRegions.Length > 0)
            {
                allIgnoreRegions.AddRange(options.IgnoreRegions);
            }

            tag = ValidateAndSanitizeTag(tag);

            VisualComparisonInfo compareInfo;
            ComparisonResult result = null;
            Image imageFromWebDriver;

            // Retrieve the baseline image from the store. Will return null if baseline does not exist
            var baselineImage = await _imageStore.GetBaselineAsync(tag).ConfigureAwait(false);

            // Check if we have a baseline image for the current page
            if (baselineImage != null)
            {
                // Do an initial wait to allow the page to settle
                await Task.Delay(_settings.WaitBeforeImageComparison);

                // Wait for the spec image to match the currently rendered page to avoid creating a capture where the page has not fully settled
                var sw = Stopwatch.StartNew();
                do
                {
                    imageFromWebDriver = GetScreenshotOfCurrentPage();
                    if (allIgnoreRegions.Count > 0)
                    {
                        imageFromWebDriver = CoverDynamicElementsBySelector(imageFromWebDriver, allIgnoreRegions);
                    }

                    compareInfo = _imageProcessor.GetDifferences(baselineImage, imageFromWebDriver, _settings.UpdateBaseline);

                } while (sw.Elapsed < TimeSpan.FromSeconds(5) && !compareInfo.Match);

                if (compareInfo.Match)
                {
                    result = _resultFactory.ImageMatchSuccessResult(baselineImage, tag);
                }

                if (!compareInfo.Match && !_settings.UpdateBaseline)
                {
                    result = _resultFactory.ImageMatchFailureResult(compareInfo, baselineImage, imageFromWebDriver, tag);
                }
                if (!compareInfo.Match && _settings.UpdateBaseline)
                {
                    result = await _resultFactory.UpdateBaselineResult(compareInfo, baselineImage, imageFromWebDriver, tag);
                }
            }
            // No baseline image exists in the store so create a new one using the currently rendered page
            else
            {
                // Do an initial wait to allow the page to settle
                await Task.Delay(_settings.WaitBeforeImageComparison);

                imageFromWebDriver = GetScreenshotOfCurrentPage();
                if (allIgnoreRegions.Count > 0)
                {
                    imageFromWebDriver = CoverDynamicElementsBySelector(imageFromWebDriver, allIgnoreRegions);
                }

                result = await _resultFactory.NoExistingBaselineResult(baselineImage, imageFromWebDriver, tag);
            }

            _currentSession.Add(result);
            return result;
        }

        /// <summary>
        /// Initializes a new visual testing session. All visual comparison results will be associated with this session until it is closed.
        /// </summary>
        private void Open()
        {
            if (_currentSession == null)
            {
                _currentSession = new List<ComparisonResult>();
            }
        }

        /// <summary>
        /// End the current visual test and trigger results processing.
        /// </summary>
        public void EndVisualTest()
        {
            var testResult = GetCurrentTestResult();
            _allSessions.Add(testResult);
            _currentSession = null;
        }

        /// <summary>
        /// Gets the visual test result for the current test session
        /// </summary>
        /// <returns>The test result</returns>
        public TestResult GetCurrentTestResult()
        {
            if (_currentSession == null)
            {
                throw new InvalidSessionStateException("No active session found.");
            }
            if (_currentSession.Count == 0)
            {
                throw new InvalidSessionStateException("No results exist for the current session.");
            }

            return new TestResult(_currentSession);
        }

        /// <summary>
        /// Returns a summary of all visual test results executed within the lifetime of this instance.
        /// </summary>
        /// <param name="throwExceptionOnFailure">Triggers an exception if any visual test failures were detected during the test run</param>
        /// <returns>The test result summary</returns>
        public TestResultSummary GetAllTestResults(bool throwExceptionOnFailure = false)
        {
            if (_allSessions == null || _allSessions.Count == 0)
            {
                throw new InvalidSessionStateException("No test results found. Call CheckWindowAsync to generate test results.");
            }

            var result = new TestResultSummary(_allSessions);

            if (!result.AllMatched && throwExceptionOnFailure)
            {
                throw new VisualMismatchException("Visual mismatches were detected during the test run. Please check result output");
            }

            return result;
        }
    }
}
