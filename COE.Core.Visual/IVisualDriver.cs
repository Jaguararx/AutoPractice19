using System.Threading.Tasks;

using OpenQA.Selenium;

namespace COE.Core.Visual
{
    public interface IVisualDriver
    {
        void AddGlobalIgnoreRegions(params By[] ignoreRegions);
        Task<ComparisonResult> CheckWindowAsync(string tag, ComparisonOptions options = null);
        void EndVisualTest();
        TestResultSummary GetAllTestResults(bool throwExceptionOnFailure);
        TestResult GetCurrentTestResult();
    }
}