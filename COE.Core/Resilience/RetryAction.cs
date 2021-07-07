using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

namespace COE.Core.Resilience
{
    public static class RetryAction
    {
        private static readonly IClock Clock = new SystemClock();

        public static bool DefaultRetry(RetryStrategy strategy, Func<bool> action)
        {
            var endTime = Clock.LaterBy(strategy.MaxRetryDuration);

            var result = action.Invoke();

            if (result)
            {
                return true;
            }

            while (Clock.IsNowBefore(endTime) && result == false)
            {
                Thread.Sleep(strategy.SleepBeforeRetryAction);
                result = action.Invoke();
            }

            return result;
        }

        public static RetryResult RetryWithExceptions(RetryStrategy strategy, Func<bool> action)
        {
            return RetryWithExceptions<Exception>(strategy, action);
        }

        public static RetryResult RetryWithExceptions<T>(RetryStrategy strategy, Func<bool> action) where T : Exception
        {
            Exception lastException = null;
            var success = DefaultRetry(strategy, () =>
            {
                try
                {
                    return action();
                }
                catch (T ex) when (strategy.IgnoredTransientExceptions.Any(type => type.IsInstanceOfType(ex)))
                {
                    lastException = ex;
                    return false;
                }
            });

            if (!success && strategy.ThrowOnRetryTimeout)
            {
                throw new Exception(
                    $"{action.GetMethodInfo().Name} operation did not return success within the specified retry timeout. Throwing with last seen exception.",
                    lastException);
            }

            return new RetryResult
            {
                Success = success,
                LastException = lastException
            };
        }

        public static async Task<bool> DefaultRetryAsync(RetryStrategy strategy, Func<Task<bool>> action)
        {
            var endTime = Clock.LaterBy(strategy.MaxRetryDuration);

            var result = await action.Invoke();

            if (result)
            {
                return true;
            }

            while (Clock.IsNowBefore(endTime) && result == false)
            {
                await Task.Delay(strategy.SleepBeforeRetryAction);
                result = await action.Invoke();
            }

            return result;
        }

        public static async Task<RetryResult> RetryWithExceptionsAsync<T>(RetryStrategy strategy, Func<Task<bool>> action) where T : Exception
        {
            Exception lastException = null;
            var success = await DefaultRetryAsync(strategy, async () =>
            {
                try
                {
                    return await action();
                }
                catch (T ex) when (strategy.IgnoredTransientExceptions.Any(type => type.IsInstanceOfType(ex)))
                {
                    lastException = ex;
                    return false;
                }
            });

            if (!success && strategy.ThrowOnRetryTimeout)
            {
                throw new Exception(
                    $"{action.GetMethodInfo().Name} operation did not return success within the specified retry timeout. Throwing with last seen exception.",
                    lastException);
            }

            return new RetryResult
            {
                Success = success,
                LastException = lastException
            };
        }
    }
}