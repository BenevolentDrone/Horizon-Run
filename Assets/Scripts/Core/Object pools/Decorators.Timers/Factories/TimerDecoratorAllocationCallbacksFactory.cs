using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class TimerDecoratorAllocationCallbacksFactory
    {
        public static SetRuntimeTimerCallback<T> BuildSetRuntimeTimerCallback<T>(
            string id = TimerDecoratorConsts.POOL_ELEMENT_METADATA_TIMER_ID,
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
    }
}