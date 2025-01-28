using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Factories
{
    public static partial class TimeManagementFactory
    {
        public const string APPLICATION_RUNTIME_TIMER_ID = "Application runtime timer";

        public const string APPLICATION_PERSISTENT_TIMER_ID = "Application persistent timer";

        public static TimeManager BuildTimeManager(
            ILoggerResolver loggerResolver)
        {
            var applicationActiveTimer = TimerFactory.BuildRuntimeTimer(
                APPLICATION_RUNTIME_TIMER_ID,
                0f,
                loggerResolver);

            applicationActiveTimer.Accumulate = true;

            applicationActiveTimer.Start();

            var applicationPersistentTimer = TimerFactory.BuildPersistentTimer(
                APPLICATION_PERSISTENT_TIMER_ID,
                default,
                loggerResolver);

            applicationActiveTimer.Accumulate = true;

            applicationPersistentTimer.Start();

            return new TimeManager(
                RepositoryFactory.BuildDictionaryRepository<string, ISynchronizableGenericArg<float>>(),
                applicationActiveTimer,
                applicationPersistentTimer);
        }

        public static TimerManager BuildTimerManager(
            string managerID,
            ISynchronizationProvider provider,
            ILoggerResolver loggerResolver,
            
            bool renameTimersOnPop = true)
        {
            return new TimerManager(
                managerID,
                IDAllocationFactory.BuildUShortIDAllocationController(
                    loggerResolver),
                RepositoryFactory.BuildDictionaryRepository<int, IPoolElementFacade<TimerWithSubscriptionsContainer>>(),
                RepositoryFactory.BuildDictionaryRepository<string, List<DurationHandlePair>>(),
                TimerPoolFactory.BuildRuntimeTimerPool(
                    provider,
                    loggerResolver),
                renameTimersOnPop);
        }        
    }
}