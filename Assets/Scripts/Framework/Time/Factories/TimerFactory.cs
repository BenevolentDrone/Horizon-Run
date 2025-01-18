using System;
using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Time.Strategies;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Factories
{
    public static partial class TimerFactory
    {
        #region Persistent timer

        public static PersistentTimer BuildPersistentTimer(
            string id,
            TimeSpan defaultDurationSpan,
            ILoggerResolver loggerResolver = null)
        {
            var onStart = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IPersistentTimer>(loggerResolver);
            
            var onStartRepeated = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IPersistentTimer>(loggerResolver);
            
            var onFinish = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IPersistentTimer>(loggerResolver);
            
            var onFinishRepeated = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IPersistentTimer>(loggerResolver);
            
            ILogger logger =
                loggerResolver?.GetLogger<PersistentTimer>();

            return new PersistentTimer(
                id,
                defaultDurationSpan,
                
                onStart,
                onStart,
                
                onStartRepeated,
                onStartRepeated,
                
                onFinish,
                onFinish,
                
                onFinishRepeated,
                onFinishRepeated,
                
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
            var repository = RepositoryFactory.BuildDictionaryRepository<ETimerState, ITimerStrategy<IPersistentTimerContext, TimeSpan>>(
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
            var onStart = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onStartRepeated = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onFinish = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onFinishRepeated = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            ILogger logger =
                loggerResolver?.GetLogger<RuntimeTimer>();

            return new RuntimeTimer(
                id,
                defaultDuration,
                
                onStart,
                onStart,
                
                onStartRepeated,
                onStartRepeated,
                
                onFinish,
                onFinish,
                
                onFinishRepeated,
                onFinishRepeated,
                
                BuildRuntimeStrategyRepository(),
                
                logger);
        }
        
        public static RuntimeTimer BuildRuntimeTimerWithPrivateSubscriptions(
            string id,
            float defaultDuration,
            out INonAllocSubscribable onStartPrivateSubscribable,
            out INonAllocSubscribable onFinishPrivateSubscribable,
            ILoggerResolver loggerResolver = null)
        {
            
            var onStart = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onStartPrivate = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onStartRepeated = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onFinish = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onFinishPrivate = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            var onFinishRepeated = BroadcasterFactory.BuildNonAllocBroadcasterGeneric<IRuntimeTimer>(loggerResolver);
            
            
            INonAllocSubscription privateStartSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                onStart.Publish,
                loggerResolver);
            
            onStartPrivate.Subscribe(
                (INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<IRuntimeTimer>>)
                privateStartSubscription);
            
            
            INonAllocSubscription privateFinishSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                onFinish.Publish,
                loggerResolver);
            
            onFinishPrivate.Subscribe(
                (INonAllocSubscriptionHandler<
                    INonAllocSubscribable,
                    IInvokableSingleArgGeneric<IRuntimeTimer>>)
                privateFinishSubscription);

            
            onStartPrivateSubscribable = onStartPrivate;
            
            onFinishPrivateSubscribable = onFinishPrivate;
            
            
            ILogger logger =
                loggerResolver?.GetLogger<RuntimeTimer>();

            return new RuntimeTimer(
                id,
                defaultDuration,
                
                onStartPrivate,
                onStart,
                
                onStartRepeated,
                onStartRepeated,
                
                onFinishPrivate,
                onFinish,
                
                onFinishRepeated,
                onFinishRepeated,
                
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
            var repository = RepositoryFactory.BuildDictionaryRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>>(
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
            var logger = loggerResolver?.GetLogger<RuntimeTimer>();
            
            var timer = TimerFactory.BuildRuntimeTimerWithPrivateSubscriptions(
                id,
                duration,
                out var onStartPrivateSubscribable,
                out var onFinishPrivateSubscribable,
                loggerResolver);

            // Subscribe to the runtime timer's tick event
            var tickSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<float>(
                timer.Tick,
                loggerResolver);


            var startTimerSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                (timer) =>
                {
                    if (!tickSubscription.Active)
                    {
                        //logger?.Log<RuntimeTimer>($"TIMER {timer.ID} STARTED. SUBSCRIBING TO TICKS");
                        
                        provider.Subscribe(tickSubscription);
                    }
                },
                loggerResolver);

            //timer.OnStart.Subscribe(startTimerSubscription);


            var finishTimerSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                (timer) =>
                {
                    if (tickSubscription.Active)
                    {
                        //logger?.Log<RuntimeTimer>($"TIMER {timer.ID} STOPPED. UNSUBSCRIBING FROM TICKS");
                        
                        provider.Unsubscribe(tickSubscription);
                    }
                },
                loggerResolver);

            //timer.OnFinish.Subscribe(finishTimerSubscription);


            return new TimerWithSubscriptionsContainer
            {
                Timer = timer,
                
                TickSubscription = tickSubscription,
                
                StartTimerSubscription = startTimerSubscription,
                
                OnStartPrivateSubscribable = onStartPrivateSubscribable,
                
                FinishTimerSubscription = finishTimerSubscription,
                
                OnFinishPrivateSubscribable = onFinishPrivateSubscribable
            };
        }
    }
}