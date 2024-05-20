using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    /// <summary>
    /// Represents a factory for creating timers decorators pools.
    /// </summary>
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Allocation callbacks

        public static SetRuntimeTimerCallback<T> BuildSetRuntimeTimerCallback<T>(
            string id = TimerConsts.ANONYMOUS_TIMER_ID,
            float defaultDuration = 0f,
            ILoggerResolver loggerResolver = null)
        {
            return new SetRuntimeTimerCallback<T>(
                id,
                defaultDuration,
                loggerResolver);
        }

        public static SetDurationAndPushSubscriptionCallback<T> BuildSetDurationAndPushSubscriptionCallback<T>(
            float duration = 0f,
            ILoggerResolver loggerResolver = null)
        {
            return new SetDurationAndPushSubscriptionCallback<T>(
                duration,
                loggerResolver);
        }
        
        #endregion
    }
}