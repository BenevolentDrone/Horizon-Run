using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Time.Strategies;
using HereticalSolutions.Time.Timers;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Factories
{
    public static partial class TimeFactory
    {
        

        #region Persistent timer

        public static PersistentTimer BuildPersistentTimer(
            string id,
            TimeSpan defaultDurationSpan,
            ILoggerResolver loggerResolver = null)
        {
            var onStart = DelegatesFactory.BuildNonAllocBroadcasterGeneric<IPersistentTimer>(loggerResolver);
            
            var onFinish = DelegatesFactory.BuildNonAllocBroadcasterGeneric<IPersistentTimer>(loggerResolver);
            
            ILogger logger =
                loggerResolver?.GetLogger<PersistentTimer>()
                ?? null;

            return new PersistentTimer(
                id,
                defaultDurationSpan,
                
                onStart,
                onStart,
                
                onFinish,
                onFinish,
                
                BuildPersistentStrategyRepository(),
                
                logger);
        }

        /// <summary>
        /// Builds the repository of timer strategies for a persistent timer
        /// </summary>
        /// <returns>The built repository of timer strategies.</returns>
        private static IReadOnlyRepository<ETimerState, ITimerStrategy<IPersistentTimerContext, TimeSpan>>
            BuildPersistentStrategyRepository()
        {
            var repository = RepositoriesFactory.BuildDictionaryRepository<ETimerState, ITimerStrategy<IPersistentTimerContext, TimeSpan>>(
                new ETimerStateComparer());
            
            repository.Add(ETimerState.INACTIVE, new PersistentInactiveStrategy());
            
            repository.Add(ETimerState.STARTED, new PersistentStartedStrategy());
            
            repository.Add(ETimerState.PAUSED, new PersistentPausedStrategy());
            
            repository.Add(ETimerState.FINISHED, new PersistentFinishedStrategy());

            return repository;
        }

        #endregion
        
        #region Runtime timer
        
        public static RuntimeTimer BuildRuntimeTimer(
            string id,
            float defaultDuration,
            ILoggerResolver loggerResolver = null)
        {
            var onStart = DelegatesFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onFinish = DelegatesFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            ILogger logger =
                loggerResolver?.GetLogger<RuntimeTimer>()
                ?? null;

            return new RuntimeTimer(
                id,
                defaultDuration,
                
                onStart,
                onStart,
                
                onFinish,
                onFinish,
                
                BuildRuntimeStrategyRepository(),
                
                logger);
        }

        /// <summary>
        /// Builds the repository of timer strategies for a runtime timer
        /// </summary>
        /// <returns>The built repository of timer strategies.</returns>
        private static IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>>
            BuildRuntimeStrategyRepository()
        {
            var repository = RepositoriesFactory.BuildDictionaryRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>>(
                new ETimerStateComparer());
            
            repository.Add(ETimerState.INACTIVE, new RuntimeInactiveStrategy());
            
            repository.Add(ETimerState.STARTED, new RuntimeStartedStrategy());
            
            repository.Add(ETimerState.PAUSED, new RuntimePausedStrategy());
            
            repository.Add(ETimerState.FINISHED, new RuntimeFinishedStrategy());

            return repository;
        }

        #endregion

        public static TimerWithSubscriptionsContainer BuildRuntimeTimerWithSubscriptionsContainer(
            ISynchronizationProvider provider,
            string id = TimerConsts.ANONYMOUS_TIMER_ID,
            float duration = 0f,
            ILoggerResolver loggerResolver = null)
        {
            var timer = TimeFactory.BuildRuntimeTimer(
                id,
                duration,
                loggerResolver);

            // Subscribe to the runtime timer's tick event
            var tickSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(
                timer.Tick,
                loggerResolver);


            var startTimerSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                (timer) =>
                {
                    if (!tickSubscription.Active)
                        provider.Subscribe(tickSubscription);
                },
                loggerResolver);

            //timer.OnStart.Subscribe(startTimerSubscription);


            var finishTimerSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                (timer) =>
                {
                    if (tickSubscription.Active)
                        provider.Unsubscribe(tickSubscription);
                },
                loggerResolver);

            //timer.OnFinish.Subscribe(finishTimerSubscription);


            return new TimerWithSubscriptionsContainer
            {
                Timer = timer,
                
                TickSubscription = tickSubscription,
                
                StartTimerSubscription = startTimerSubscription,
                
                FinishTimerSubscription = finishTimerSubscription
            };
        }
    }
}