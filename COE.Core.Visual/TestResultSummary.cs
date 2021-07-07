using System;
using System.Collections.Generic;
using System.Linq;

namespace COE.Core.Visual
{
    public class TestResultSummary
    {
        public TestResultSummary(IReadOnlyCollection<TestResult> allTestResults) => AllTestResults = allTestResults ?? throw new ArgumentNullException(nameof(allTestResults));

        public bool AllMatched => AllTestResults.All(t => t.ComparisonResults.All(r => r.Match));
        public IReadOnlyCollection<TestResult> AllTestResults { get; }
    }
}