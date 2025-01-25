using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class TimerDecoratorAllocationCallbacksFactory
    {
        public static SetRuntimeTimerCallback<T> BuildSetRuntimeTimerCallback<T>(
            ILoggerResolver loggerResolver,

            string id = TimerDecoratorConsts.POOL_ELEMENT_METADATA_TIMER_ID,
            float defaultDuration = 0f)
        {
            ILogger logger = loggerResolver?.GetLogger<SetRuntimeTimerCallback<T>>();

            return new SetRuntimeTimerCallback<T>(
                loggerResolver,
                logger,
                id,
                defaultDuration);
        }

        public static SetDurationAndPushSubscriptionCallback<T> BuildSetDurationAndPushSubscriptionCallback<T>(
            ILoggerResolver loggerResolver,
            float duration = 0f)
        {
            var logger = loggerResolver?.GetLogger<SetDurationAndPushSubscriptionCallback<T>>();

            return new SetDurationAndPushSubscriptionCallback<T>(
                loggerResolver,
                logger,
                duration);
        }
    }
}