using System;

namespace COE.Core.Resilience
{
    public class RetryResult
    {
        public Exception LastException { get; set; }
        public bool Success { get; set; }
    }
}