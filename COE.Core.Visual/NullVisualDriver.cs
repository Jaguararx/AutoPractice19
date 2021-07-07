using System.Threading.Tasks;
using OpenQA.Selenium;

namespace COE.Core.Visual
{
    /// <summary>
    /// Null implementation of IVisualDriver which can be used to satisfy DI requirements
    /// </summary>
    public class NullVisualDriver : IVisualDriver
    {
        public void AddGlobalIgnoreRegions(params By[] ignoreRegions) { }

        public Task<ComparisonResult> CheckWindowAsync(string tag, ComparisonOptions options = null) => Task.FromResult<ComparisonResult>(null);

        public void EndVisualTest() { }

        public TestResultSummary GetAllTestResults(bool throwExceptionOnFailure) => null;

        public TestResult GetCurrentTestResult() => null;
    }
}
