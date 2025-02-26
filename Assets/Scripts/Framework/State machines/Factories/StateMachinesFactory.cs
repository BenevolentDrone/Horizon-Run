using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;
using HereticalSolutions.Delegates.Factories;

namespace HereticalSolutions.StateMachines.Factories
{
    public static class StateMachinesFactory
    {
        public static BaseStateMachine<TBaseState> BuildBaseStateMachine<TBaseState>(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,

            ITransitionController<TBaseState> transitionController,

            TBaseState initialState,

            ILoggerResolver loggerResolver)
            where TBaseState : IState
        {
            ILogger logger =
                loggerResolver?.GetLogger<BaseStateMachine<TBaseState>>();

            return new BaseStateMachine<TBaseState>(
                states,
                events,

                transitionController,
                new Queue<ITransitionRequest>(),

                BroadcasterFactory.BuildNonAllocBroadcasterMultipleArgs(
                    loggerResolver),
                BroadcasterFactory.BuildNonAllocBroadcasterMultipleArgs(
                    loggerResolver),
                BroadcasterFactory.BuildNonAllocBroadcasterGeneric<ITransitionEvent<TBaseState>>(
                    loggerResolver),

                initialState,

                logger);
        }

        public static ConcurrentBaseStateMachine<TBaseState> BuildConcurrentBaseStateMachine<TBaseState>(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, ITransitionEvent<TBaseState>> events,

            ITransitionController<TBaseState> transitionController,

            TBaseState initialState,
            ILoggerResolver loggerResolver)
            where TBaseState : IState
        {
            ILogger logger =
                loggerResolver?.GetLogger<BaseStateMachine<TBaseState>>();

            return new ConcurrentBaseStateMachine<TBaseState>(
                new BaseStateMachine<TBaseState>(
                    states,
                    events,
    
                    transitionController,
                    new Queue<ITransitionRequest>(),
    
                    BroadcasterFactory.BuildConcurrentNonAllocBroadcasterMultipleArgs(
                        loggerResolver),
                    BroadcasterFactory.BuildConcurrentNonAllocBroadcasterMultipleArgs(
                        loggerResolver),
                    BroadcasterFactory.BuildConcurrentNonAllocBroadcasterGeneric<ITransitionEvent<TBaseState>>(
                        loggerResolver),
    
                    initialState,
    
                    logger),
                new object());
        }

        public static BaseAsyncStateMachine<TBaseState> BuildBaseAsyncStateMachine<TBaseState>(
            IReadOnlyRepository<Type, TBaseState> states,
            IReadOnlyRepository<Type, IAsyncTransitionEvent<TBaseState>> events,

            IAsyncTransitionController<TBaseState> transitionController,

            TBaseState initialState,
            ILoggerResolver loggerResolver)
            where TBaseState : IAsyncState
        {
            ILogger logger =
                loggerResolver?.GetLogger<BaseAsyncStateMachine<TBaseState>>();

            return new BaseAsyncStateMachine<TBaseState>(
                states,
                events,

                transitionController,
                new Queue<IAsyncTransitionRequest>(),

                BroadcasterFactory.BuildConcurrentNonAllocBroadcasterMultipleArgs(
                    loggerResolver),
                BroadcasterFactory.BuildConcurrentNonAllocBroadcasterMultipleArgs(
                    loggerResolver),
                BroadcasterFactory.BuildConcurrentNonAllocBroadcasterGeneric<IAsyncTransitionEvent<TBaseState>>(
                    loggerResolver),

                initialState,

                logger);
        }

        public static T AddState<TBaseState, T>(
            IRepository<Type, TBaseState> states)
            where TBaseState : IState
            where T : TBaseState
        {
            var state = (T)Activator.CreateInstance(
                typeof(T));
            
            states.Add(typeof(T), (TBaseState)state);

            return state;
        }

        public static T AddStateWithArguments<TBaseState, T>(
            IRepository<Type, TBaseState> states,
            object[] arguments)
            where TBaseState : IState
            where T : TBaseState
        {
            var state = (T)Activator.CreateInstance(
                typeof(T),
                arguments);
            
            states.Add(typeof(T), (TBaseState)state);

            return state;
        }

        public static TEvent AddTransitionEvent<TBaseState, TEvent, TFrom, TTo>(
            IRepository<Type, TBaseState> states,
            IRepository<Type, ITransitionEvent<TBaseState>> transitionEvents)
            where TBaseState : IState
            where TEvent : ITransitionEvent<TBaseState>
            where TFrom : TBaseState
            where TTo : TBaseState
        {
            var transitionEvent = (TEvent)Activator.CreateInstance(
                typeof(TEvent),
                new object[]
                {
                    states.Get(typeof(TFrom)),
                    states.Get(typeof(TTo)),
                });

            transitionEvents.Add(typeof(TEvent), transitionEvent);

            return transitionEvent;
        }
    }
}