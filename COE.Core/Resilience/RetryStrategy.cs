using System;

namespace COE.Core.Resilience
{
    public class RetryStrategy
    {
        public Type[] IgnoredTransientExceptions { get; set; }
        public TimeSpan MaxRetryDuration { get; set; }
        public TimeSpan SleepBeforeReCheckCondition { get; set; }
        public TimeSpan SleepBeforeRetryAction { get; set; }
        public bool ThrowOnRetryTimeout { get; set; }
    }
}