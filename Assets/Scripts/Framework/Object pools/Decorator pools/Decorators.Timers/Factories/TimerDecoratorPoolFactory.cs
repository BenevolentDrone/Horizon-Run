using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
    public static class TimerDecoratorPoolFactory
    {
        public static ManagedPoolWithRuntimeTimer<T> BuildManagedPoolWithRuntimeTimer<T>(
            IManagedPool<T> innerPool,
            ITimerManager timerManager,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ManagedPoolWithRuntimeTimer<T>>();

            return new ManagedPoolWithRuntimeTimer<T>(
                innerPool,
                timerManager,
                logger);
        }
    }
}