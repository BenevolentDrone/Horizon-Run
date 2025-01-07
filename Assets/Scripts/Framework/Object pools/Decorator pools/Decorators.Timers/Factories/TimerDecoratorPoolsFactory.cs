using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class TimerDecoratorPoolsFactory
    {
        public static ManagedPoolWithRuntimeTimer<T> BuildManagedPoolWithRuntimeTimer<T>(
            IManagedPool<T> innerPool,
            ITimerManager timerManager,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithRuntimeTimer<T>>()
                ?? null;

            return new ManagedPoolWithRuntimeTimer<T>(
                innerPool,
                timerManager,
                logger);
        }
    }
}