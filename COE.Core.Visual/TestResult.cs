using System;
using System.Collections.Generic;
using System.Linq;

namespace COE.Core.Visual
{
    public class TestResult
    {
        public TestResult(IReadOnlyCollection<ComparisonResult> comparisonResults) => ComparisonResults = comparisonResults ?? throw new ArgumentNullException(nameof(comparisonResults));
        public bool AllMatched => ComparisonResults.All(r => r.Match);
        public IReadOnlyCollection<ComparisonResult> ComparisonResults { get; }
    }
}