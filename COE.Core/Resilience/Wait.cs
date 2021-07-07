using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace COE.Core.Resilience
{
    public static class Wait
    {
        public static async Task<bool> ForConditionAsync(Func<Task<bool>> waitCondition, TimeSpan timeToWait)
        {
            var timer = Stopwatch.StartNew();
            do
            {
                if (await waitCondition.Invoke())
                {
                    return true;
                }

                await Task.Delay(500);
            } while (timer.Elapsed < timeToWait);

            return false;
        }
    }
}
