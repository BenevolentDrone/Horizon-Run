using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Logging;
using HereticalSolutions.Time;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class TimersDecoratorsPoolsFactory
    {
        #region Decorator pools

        #endregion

        #region Non alloc decorator pools

        public static NonAllocPoolWithRuntimeTimer<T> BuildNonAllocPoolWithRuntimeTimer<T>(
            INonAllocDecoratedPool<T> innerPool,
            ITimerManager timerManager,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<NonAllocPoolWithRuntimeTimer<T>>()
                ?? null;

            return new NonAllocPoolWithRuntimeTimer<T>(
                innerPool,
                timerManager,
                logger);
        }
        
        #endregion
    }
}